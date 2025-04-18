using AnimalAdoption.Models;
using System.Net.WebSockets;
using System.Text.Json;
using System.Text;

namespace AnimalAdoption
{
    public interface IAnimalGenerator
    {
        void AddClient(WebSocket webSocket);
        void RemoveClient(WebSocket webSocket);
        Task StartGeneratingAsync(CancellationToken cancellationToken);
    }

    public class AnimalGenerator : IAnimalGenerator
    {
        private readonly List<WebSocket> _clients = new();
        private readonly Random _random = new();
        private readonly string[] _names = { "Buddy", "Bella", "Max", "Lucy", "Charlie", "Luna" };
        private readonly string[] _locations = { "New York", "Los Angeles", "Chicago", "Houston", "Miami" };
        private readonly string[] _photos = { "dog1.jpg", "cat1.jpg", "dog2.jpg", "rabbit1.jpg" };
        private readonly TimeSpan _generationInterval = TimeSpan.FromSeconds(5);

        public void AddClient(WebSocket webSocket)
        {
            lock (_clients)
            {
                _clients.Add(webSocket);
            }
        }

        public void RemoveClient(WebSocket webSocket)
        {
            lock (_clients)
            {
                _clients.Remove(webSocket);
            }
        }

        public virtual async Task StartGeneratingAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(_generationInterval, cancellationToken);

                var newAnimal = GenerateRandomAnimal();
                await BroadcastAnimal(newAnimal);
            }
        }

        public Animal GenerateRandomAnimal()
        {
            return new Animal
            {
                Id = _random.Next(1000, 9999),
                Photo = _photos[_random.Next(_photos.Length)],
                Name = _names[_random.Next(_names.Length)],
                Age = _random.Next(1, 15),
                Description = "Friendly and looking for a new home",
                Location = _locations[_random.Next(_locations.Length)],
                IsAdopted = false
            };
        }

        public async Task BroadcastAnimal(Animal animal)
        {
            var json = JsonSerializer.Serialize(animal);
            var buffer = Encoding.UTF8.GetBytes(json);

            List<WebSocket> clientsToRemove = new();

            lock (_clients)
            {
                foreach (var client in _clients)
                {
                    if (client.State == WebSocketState.Open)
                    {
                        try
                        {
                               client.SendAsync(
                                new ArraySegment<byte>(buffer),
                                WebSocketMessageType.Text,
                                true,
                                CancellationToken.None);
                        }
                        catch
                        {
                            clientsToRemove.Add(client);
                        }
                    }
                    else
                    {
                        clientsToRemove.Add(client);
                    }
                }

                // Remove disconnected clients
                foreach (var client in clientsToRemove)
                {
                    _clients.Remove(client);
                }
            }
        }
    }
}
