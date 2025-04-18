using AnimalAdoption.Models;
using Moq;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Xunit;

namespace AnimalAdoption.Tests
{
    public class AnimalGeneratorTests
    {
        private readonly AnimalGenerator _generator;
        private readonly Mock<WebSocket> _mockWebSocket;

        public AnimalGeneratorTests()
        {
            _generator = new AnimalGenerator();
            _mockWebSocket = new Mock<WebSocket>();
            _mockWebSocket.Setup(x => x.State).Returns(WebSocketState.Open);
        }

        [Fact]
        public void AddClient_ShouldAddWebSocketToClientsList()
        {
            // Act
            _generator.AddClient(_mockWebSocket.Object);

            // Assert
            Assert.Contains(_mockWebSocket.Object, GetClientsList());
        }

        [Fact]
        public void RemoveClient_ShouldRemoveWebSocketFromClientsList()
        {
            // Arrange
            _generator.AddClient(_mockWebSocket.Object);

            // Act
            _generator.RemoveClient(_mockWebSocket.Object);

            // Assert
            Assert.DoesNotContain(_mockWebSocket.Object, GetClientsList());
        }

        [Fact]
        public void GenerateRandomAnimal_ShouldReturnValidAnimal()
        {
            // Act
            var animal = _generator.GenerateRandomAnimal();

            // Assert
            Assert.NotNull(animal);
            Assert.InRange(animal.Id, 1000, 9999);
            Assert.InRange(animal.Age, 1, 15);
            Assert.False(animal.IsAdopted);
            Assert.NotNull(animal.Name);
            Assert.NotNull(animal.Location);
            Assert.NotNull(animal.Photo);
            Assert.Equal("Friendly and looking for a new home", animal.Description);
        }

        [Fact]
        public async Task BroadcastAnimal_ShouldSendMessageToAllOpenClients()
        {
            // Arrange
            var mockWebSocket1 = new Mock<WebSocket>();
            mockWebSocket1.Setup(x => x.State).Returns(WebSocketState.Open);

            var mockWebSocket2 = new Mock<WebSocket>();
            mockWebSocket2.Setup(x => x.State).Returns(WebSocketState.Open);

            _generator.AddClient(mockWebSocket1.Object);
            _generator.AddClient(mockWebSocket2.Object);

            var animal = new Animal
            {
                Id = 1234,
                Name = "Test",
                Age = 5,
                Description = "Test description",
                Location = "Test location",
                Photo = "test.jpg",
                IsAdopted = false
            };

            // Act
            await _generator.BroadcastAnimal(animal);

            // Assert
            var expectedJson = JsonSerializer.Serialize(animal);
            var expectedBytes = Encoding.UTF8.GetBytes(expectedJson);

            mockWebSocket1.Verify(x => x.SendAsync(
                It.Is<ArraySegment<byte>>(b => Encoding.UTF8.GetString(b.Array!) == expectedJson),
                WebSocketMessageType.Text,
                true,
                CancellationToken.None), Times.Once);

            mockWebSocket2.Verify(x => x.SendAsync(
                It.Is<ArraySegment<byte>>(b => Encoding.UTF8.GetString(b.Array!) == expectedJson),
                WebSocketMessageType.Text,
                true,
                CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task BroadcastAnimal_ShouldRemoveClosedClients()
        {
            // Arrange
            var mockClosedWebSocket = new Mock<WebSocket>();
            mockClosedWebSocket.Setup(x => x.State).Returns(WebSocketState.Closed);

            _generator.AddClient(mockClosedWebSocket.Object);
            _generator.AddClient(_mockWebSocket.Object);

            var animal = new Animal();

            // Act
            await _generator.BroadcastAnimal(animal);

            // Assert
            var clients = GetClientsList();
            Assert.DoesNotContain(mockClosedWebSocket.Object, clients);
            Assert.Contains(_mockWebSocket.Object, clients);
        }

        private List<WebSocket> GetClientsList()
        {
            // Using reflection to access the private _clients field for verification
            var field = typeof(AnimalGenerator).GetField("_clients", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            return (List<WebSocket>)field!.GetValue(_generator)!;
        }
    }
}