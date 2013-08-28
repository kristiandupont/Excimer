using Excimer.Drawing;
using NUnit.Framework;

namespace Excimer.Tests
{
    [TestFixture]
    public class ColorTests
    {
        [Test]
        public void AddColorsShouldAddIndividualComponents()
        {
            // Arrange
            var color1 = Color.FromArgb(255, 255, 64, 0);
            var color2 = Color.FromArgb(255, 0, 64, 128);

            // Act
            var actual = color1 + color2;

            // Assert
            var expected = Color.FromArgb(255, 255, 128, 128);
            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}
