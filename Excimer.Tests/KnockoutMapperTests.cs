using System;
using System.Collections.Generic;
using NUnit.Framework;
using Moq;
using TestHelpers;
using System.Threading;
using Excimer.KnockoutMapping;

namespace Excimer.Tests
{
    [TestFixture]
    public class KnockoutMapperTests
    {
        private Mock<IOperationRegistry> _operationRegistryMock;
        private Mock<IMonitorCollection> _monitorCollectionMock;
        private KnockoutMapper _knockoutMapper;

        [SetUp]
        public void Setup()
        {
            _operationRegistryMock = new Mock<IOperationRegistry>();
            _monitorCollectionMock = new Mock<IMonitorCollection>();
            _knockoutMapper = new KnockoutMapper(_operationRegistryMock.Object, _monitorCollectionMock.Object);
            _knockoutMapper.RegisterServerSideViewModel<TestViewModel>();
        }

        [Test]
        public void ShouldRegisterApiForViewModel()
        {
            // Act & Assert
            _operationRegistryMock.Verify(or => or.RegisterCommand("TestViewModel_constructor", It.IsAny<Func<KnockoutMapper.ViewModelWrapper>>()), Times.Once());
        }

        [Test]
        public void ShouldConstructViewModel()
        {
            // Act
            var viewModelWrapper = _knockoutMapper.ConstructViewModel(typeof(TestViewModel));
            var instance = _knockoutMapper.GetInstance(viewModelWrapper.RefToken);

            // Assert
            Assert.That(instance["stringProperty"], Is.EqualTo("null"));
        }

        [Test]
        public void ShouldRenderMappingScript()
        {
            // Act
            var koMapping = _knockoutMapper.RenderKoMapping(typeof(TestViewModel)).ToString();

            // Assert
            Assert.That(koMapping, Is.StringContaining("function TestViewModelMapper() {"));
            Assert.That(koMapping, Is.StringContaining("self.stringProperty = ko.observable('');"));
            Assert.That(koMapping, Is.StringContaining("$.get('/api/TestViewModel_constructor', {}, function(result) {"));
            Assert.That(koMapping, Is.StringContaining("self.refToken = result.refToken"));
        }

        [Test]
        public void ShouldConstructWithInitialValues()
        {
            // Act
            var koMapping = _knockoutMapper.RenderKoMapping(typeof(TestViewModel)).ToString();

            // Assert
            Assert.That(koMapping, Is.StringContaining("self.stringProperty(result.viewModel.stringProperty);"));
        }

        [Test]
        public void ShouldCollectPendingChanges()
        {
            // Arrange
            _knockoutMapper.ConstructViewModel(typeof(TestViewModel));

            // Act
            TestViewModel.Instance.StringProperty.Set("New string value");

            // Assert
            Assert.That(_knockoutMapper.PendingChanges.Count, Is.EqualTo(1));
            Assert.That(_knockoutMapper.PendingChanges[0].PropertyName, Is.EqualTo("StringProperty"));
            Assert.That(_knockoutMapper.PendingChanges[0].NewValue, Is.EqualTo("New string value"));
        }

        [Test]
        public void ShouldPublishPendingChanges()
        {
            // Arrange
            _knockoutMapper = new KnockoutMapper(_operationRegistryMock.Object, new MonitorCollection());
            var viewModelToken = _knockoutMapper.ConstructViewModel(typeof(TestViewModel)).RefToken;

            // Act
            List<ChangeEntry> changeEntries = null;
            var pollThread = new Thread(() => changeEntries = _knockoutMapper.Subscribe(viewModelToken));
            pollThread.Start();
            TestViewModel.Instance.StringProperty.Set("New string value");
            pollThread.Join();

            // Assert
            Assert.That(changeEntries.Count, Is.EqualTo(1));
            Assert.That(changeEntries[0].PropertyName, Is.EqualTo("StringProperty"));
            Assert.That(changeEntries[0].NewValue, Is.EqualTo("New string value"));
        }

        [Test]
        public void ShouldOnlyPollUpdatedViewModels()
        {
            // Arrange
            var viewModelToken1 = _knockoutMapper.ConstructViewModel(typeof(TestViewModel)).RefToken;
            var viewModelToken2 = _knockoutMapper.ConstructViewModel(typeof(TestViewModel)).RefToken;
            var viewModel2 = (TestViewModel)_knockoutMapper.GetViewModelObject(viewModelToken2);

            // Act
            var pollThread = new Thread(() => _knockoutMapper.Subscribe(viewModelToken1));
            pollThread.Start();
            viewModel2.StringProperty.Set("New string value");
            pollThread.Join();

            // Assert
            _monitorCollectionMock.Verify(x => x.Wait(viewModelToken1, It.IsAny<int>()), Times.Once());
            _monitorCollectionMock.Verify(x => x.Pulse(viewModelToken2), Times.Once());
        }

        [Test]
        public void ShouldRegisterSubscriptionFunctionAsOperation()
        {
            // Assert
            _operationRegistryMock.Verify(x => x.RegisterCommand("subscribeToViewModel", It.IsAny<MulticastDelegate>()), Times.Once());
        }
    }
}
