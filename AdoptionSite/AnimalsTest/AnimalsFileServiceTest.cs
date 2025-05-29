using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using System.Text;
using Xunit;

namespace AnimalAdoption.Tests
{
    public class AnimalFileServiceTests
    {
        private readonly Mock<IConfiguration> _mockConfig;
        private readonly Mock<ILogger<AnimalFileService>> _mockLogger;
        private readonly AnimalFileService _service;
        private readonly string _testUploadPath;

        public AnimalFileServiceTests()
        {
            _testUploadPath = Path.Combine(Path.GetTempPath(), "AnimalUploads_Test");

            _mockConfig = new Mock<IConfiguration>();
            _mockConfig.Setup(x => x["FileStorage:UploadPath"]).Returns(_testUploadPath);

            _mockLogger = new Mock<ILogger<AnimalFileService>>();

            _service = new AnimalFileService(_mockConfig.Object, _mockLogger.Object);

            // Clean up test directory before each test
            if (Directory.Exists(_testUploadPath))
            {
                Directory.Delete(_testUploadPath, true);
            }
        }

        [Fact]
        public async Task GetAnimalFileAsync_ShouldThrow_WhenFileNotFound()
        {
            // Arrange
            var fileName = "nonexistent.jpg";

            // Act & Assert
            await Assert.ThrowsAsync<FileNotFoundException>(
                () => _service.GetAnimalFileAsync(fileName));

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("File not found")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }


        [Fact]
        public async Task DeleteAnimalFileAsync_ShouldReturnFalse_WhenFileNotExists()
        {
            // Arrange
            var fileName = "nonexistent.jpg";

            // Act
            var result = await _service.DeleteAnimalFileAsync(fileName);

            // Assert
            Assert.False(result);
        }

        [Theory]
        [InlineData("test.jpg", "image/jpeg")]
        [InlineData("test.png", "image/png")]
        [InlineData("test.pdf", "application/pdf")]
        [InlineData("test.unknown", "application/octet-stream")]
        public void GetContentType_ShouldReturnCorrectType(string fileName, string expectedContentType)
        {
            // Arrange
            var filePath = Path.Combine(_testUploadPath, fileName);

            // Act
            var result = _service.GetContentType(filePath);

            // Assert
            Assert.Equal(expectedContentType, result);
        }

        // Clean up after all tests
        public void Dispose()
        {
            if (Directory.Exists(_testUploadPath))
            {
                Directory.Delete(_testUploadPath, true);
            }
        }
    }
}