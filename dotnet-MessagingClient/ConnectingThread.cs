using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace dotnet_MessagingClient
{
    class ConnectingThread
    {
        private static string _connectionEndPointUrl = "/api/connections/connect";
        private static bool _isConnected = false;
        public static ClientWebSocket ws = null;
        public static async void Start()
        {
            _connectionEndPointUrl = Program.URL + _connectionEndPointUrl;
            await ConnectToServer(_connectionEndPointUrl);
        }

        public static bool IsConnected() => _isConnected;

        private static async Task ConnectToServer(string url)
        {
            try
            {
                CancellationTokenSource source = new CancellationTokenSource();
                ws = new ClientWebSocket();   
                await ws.ConnectAsync(new Uri(_connectionEndPointUrl), CancellationToken.None);
                byte[] buffer = new byte[256];
                if(ws.State == WebSocketState.Open)
                {
                    Console.WriteLine($"Connected to: {url}");
                    choose_number:
                    Console.WriteLine("Do you want to use a custom phone number? Y/N");
                    string use_phone = Console.ReadLine();
                    if (use_phone.ToUpper() == "Y" || use_phone.ToUpper() == "N")
                    {
                        if(use_phone.ToUpper() == "Y")
                        {
                            Console.WriteLine("Enter your phone number");
                            Program.ConnectionRequest.PhoneNumber = Console.ReadLine();
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid character. Try again");
                        goto choose_number;
                    }
                    Console.WriteLine($"Your phone number is: {Program.ConnectionRequest.PhoneNumber}");

                    await SendJson(ws);            
                    _isConnected = true;
                    Program.Connecting = false;
                    ReceiveMessage.Startup();
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private static async Task SendJson(WebSocket webSocket)
        {
            string json = "{\"PhoneNumber\": \""+ Program.ConnectionRequest.PhoneNumber + "\"}";
            await webSocket.SendAsync(new ArraySegment<byte>(Encoding.ASCII.GetBytes(json), 0,json.Count()),
                WebSocketMessageType.Text,
                false,
                CancellationToken.None);
        }

        private static void HandleMessage(byte[] buffer, int count)
        {
            Console.WriteLine($"Received: {BitConverter.ToString(buffer, 0, count)}");
        }
    }
}
