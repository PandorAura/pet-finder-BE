using System.Net.WebSockets;

namespace AnimalAdoption
{
    public class WebSocketMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IAnimalGenerator _animalGenerator;
        private readonly ILogger<WebSocketMiddleware> _logger;

        public WebSocketMiddleware(
            RequestDelegate next,
            IAnimalGenerator animalGenerator,
            ILogger<WebSocketMiddleware> logger)
        {
            _next = next;
            _animalGenerator = animalGenerator;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                _animalGenerator.AddClient(webSocket);

                _logger.LogInformation("WebSocket client connected");

                await Receive(webSocket, async (result, buffer) =>
                {
                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        _animalGenerator.RemoveClient(webSocket);
                        await webSocket.CloseAsync(result.CloseStatus.Value,
                                               result.CloseStatusDescription,
                                               CancellationToken.None);
                    }
                });
            }
            else
            {
                await _next(context);
            }
        }

        public async Task Receive(WebSocket socket, Action<WebSocketReceiveResult, byte[]> handleMessage)
        {
            var buffer = new byte[1024 * 4];

            while (socket.State == WebSocketState.Open)
            {
                var result = await socket.ReceiveAsync(
                    new ArraySegment<byte>(buffer),
                    CancellationToken.None);

                handleMessage(result, buffer);
            }
        }
    }
}
