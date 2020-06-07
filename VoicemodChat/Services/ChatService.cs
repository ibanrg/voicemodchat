using Fleck;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VoicemodChat.Services.Interfaces;

namespace VoicemodChat.Services
{
    public class ChatService : IChatService
    {
        private static ClientWebSocket _client;
        private static string _username;

        public ChatService()
        {
            _client = new ClientWebSocket();
        }

        public void Start(int port)
        {
            bool isAvailable = true;
            IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
            TcpConnectionInformation[] tcpConnInfoArray = ipGlobalProperties.GetActiveTcpConnections();
            FleckLog.Level = LogLevel.Error;
            foreach (TcpConnectionInformation tcpi in tcpConnInfoArray)
            {
                if (tcpi.LocalEndPoint.Port == port)
                {
                    isAvailable = false;
                    break;
                }
            }

            // If post is available, start server
            if (isAvailable)
            {
                Console.WriteLine("First connection. Starting server...");
                var allsockets = new List<IWebSocketConnection>();
                var server = new WebSocketServer($"ws://127.0.0.1:{port}");

                server.Start(socket =>
                {
                    socket.OnOpen = () =>
                    {
                        allsockets.ToList().ForEach(s => s.Send($"{socket.ConnectionInfo.Headers["username"]} logged in!"));
                        allsockets.Add(socket);
                    };

                    socket.OnClose = () =>
                    {
                        allsockets.Remove(socket);
                        allsockets.ToList().ForEach(s => s.Send($"{socket.ConnectionInfo.Headers["username"]} logged out"));
                    };

                    socket.OnMessage = (msg) =>
                    {
                        allsockets.ToList().ForEach(s => s.Send(msg));
                    };
                });
                Console.WriteLine("Server running!");
            }

            Console.Write("Enter your username: ");
            _username = Console.ReadLine();

            //_client = new ClientWebSocket();
            _client.Options.SetRequestHeader("username", _username);
            _client.ConnectAsync(new Uri($"ws://127.0.0.1:{port}"), CancellationToken.None);

            while (_client.State == WebSocketState.Connecting)
            {
                Thread.Sleep(500);
            }

            Console.WriteLine($"Welcome {_username}! You can start chatting right now. Type '#exit' to log out. Enjoy this session!");
            StartListening();
            StartSending();

            while (_client.State == WebSocketState.Open)
            {
                Thread.Sleep(100);
            }
        }

        public static async void StartSending()
        {
            while (_client.State == WebSocketState.Open)
            {
                string msg = Console.ReadLine();
                try
                {
                    if (msg == "#exit")
                    {
                        await _client.CloseAsync(WebSocketCloseStatus.Empty, "", CancellationToken.None);
                    }
                    ArraySegment<byte> bytesToSend = new ArraySegment<byte>(Encoding.UTF8.GetBytes($"{_username}: {msg}"));
                    await _client.SendAsync(bytesToSend, WebSocketMessageType.Text, true, CancellationToken.None);
                }
                catch { }
            }
        }

        public static async void StartListening()
        {
            while (_client.State == WebSocketState.Open)
            {
                ArraySegment<byte> bytesReceived = new ArraySegment<byte>(new byte[1024]);
                try
                {
                    WebSocketReceiveResult result = await _client.ReceiveAsync(bytesReceived, CancellationToken.None);
                    if (result.EndOfMessage)
                    {
                        string message = Encoding.UTF8.GetString(bytesReceived.Array, 0, result.Count);
                        if (!message.StartsWith($"{_username}: "))
                        {
                            Console.WriteLine(message);
                        }
                    }
                }
                catch
                {
                    Console.WriteLine("The server was closed. Press enter key to exit.");
                    Console.ReadKey();
                }
            }
        }
    }
}
