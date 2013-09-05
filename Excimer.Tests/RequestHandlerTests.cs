using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Moq;
using System.Net;

namespace Excimer.Tests
{
    [TestFixture]
    public class RequestHandlerTests
    {
        [Test]
        public void ShouldRegisterCommandInOperationRegistry()
        {
            // Arrange
            var operationRegistryMock = new Mock<IOperationRegistry>();
            var requestHandler = new RequestHandler(operationRegistryMock.Object);

            // Act
            requestHandler.RegisterCommand("testCommand", new Action(() =>{}));

            // Assert
            operationRegistryMock.Verify(x => x.RegisterCommand("testCommand", It.IsAny<System.Delegate>()), Times.Once());
        }

        [Test]
        public void ShouldHandlePingRequests()
        {
            // Arrange
            var requestHandler = new RequestHandler(null);

            // Act
            var actual = requestHandler.HandleRequest("http://localhost:8100/ping", "", "GET");

            // Assert
            var responseString = actual.ResponseStream.ToString();
            Assert.That(responseString, Is.EqualTo("pong"));
            Assert.That(actual.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public void ShouldHandleApiGetRequest()
        {
            // Arrange
            var operationRegistryMock = new Mock<IOperationRegistry>();
            var requestHandler = new RequestHandler(operationRegistryMock.Object);
            operationRegistryMock.Setup(x => x.InvokeCommand("command", It.IsAny<Dictionary<string, string>>())).Returns("result");
            
            // Act
            var actual = requestHandler.HandleRequest("http://localhist:8100/api/command?param1=42", "", "GET");

            // Assert
            var responseString = actual.ResponseStream.ToString();
            Assert.That(responseString, Is.EqualTo("\"result\""));
            Assert.That(actual.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public void ShouldHandleApiPostRequest()
        {
            // Arrange
            var operationRegistryMock = new Mock<IOperationRegistry>();
            var requestHandler = new RequestHandler(operationRegistryMock.Object);
            operationRegistryMock.Setup(x => x.InvokeCommand("command", It.IsAny<Dictionary<string, string>>())).Returns("result");

            // Act
            var actual = requestHandler.HandleRequest("http://localhist:8100/api/command?param1=42", "", "GET");

            // Assert
            var responseString = actual.ResponseStream.ToString();
            Assert.That(responseString, Is.EqualTo("\"result\""));
            Assert.That(actual.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

    }
}
