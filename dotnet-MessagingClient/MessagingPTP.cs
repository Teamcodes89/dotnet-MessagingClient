using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace dotnet_MessagingClient
{
    class MessagingPTP
    {
        private static Objects.External.MessageRequest messageRequest = new Objects.External.MessageRequest();
        private static string _connectionEndPointUrl = "/api/message/send";
        private static bool _isConnected = false;
        private static ClientWebSocket webSocket = null;


        public static bool IsConnected() => _isConnected;

        public static void Startup()
        {
            try
            {
                _connectionEndPointUrl = Program.URL + _connectionEndPointUrl;
                ConnectToServer(_connectionEndPointUrl).GetAwaiter().GetResult();
                if (IsConnected())
                {

                    StartMessaging();
                }
            }
            catch(Exception e)
            {
                Console.Clear();
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
        }
        private static async Task ConnectToServer(string url)
        {
            try
            {
                CancellationTokenSource source = new CancellationTokenSource();
                webSocket = new ClientWebSocket();
                await webSocket.ConnectAsync(new Uri(_connectionEndPointUrl), CancellationToken.None);
                byte[] buffer = new byte[256];
                if(webSocket.State == WebSocketState.Open)
                {
                    Console.WriteLine($"Connected messaging PTP server: {url}");
                    _isConnected = true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }


        private static void StartMessaging()
        {
            Console.WriteLine($"Write the phone number to send message to:");
            messageRequest.Receiver = Console.ReadLine();
            messageRequest.Sender = Program.ConnectionRequest.PhoneNumber;
            Console.WriteLine($"Start messaging");

            while (true)
            {
                messageRequest.Text = Encoding.ASCII.GetBytes(Console.ReadLine());
                messageRequest.Time = DateTime.Now.TimeOfDay.ToString();
                messageRequest.Date = DateTime.Now.Date.ToShortDateString();
                SendMessage(messageRequest);
            }
        }

        private static void SendMessage(Objects.External.MessageRequest messageRequest)
        {
            try
            {
                if (webSocket.State == WebSocketState.Open)
                {
                    string json = JsonConvert.SerializeObject(messageRequest);
                    webSocket.SendAsync(new ArraySegment<byte>(Encoding.ASCII.GetBytes(json), 0, json.Count()),
                    WebSocketMessageType.Text,
                    false,
                    CancellationToken.None).GetAwaiter().GetResult();
                    DrawSentMessage(messageRequest);
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private static void DrawSentMessage(Objects.External.MessageRequest message)
        {
            int indentLevel = 1;
            int IndentSize = 2;
            string _textInMessage = Encoding.ASCII.GetString(message.Text, 0, message.Text.Length);
            var line = new string(' ', indentLevel * IndentSize);
            line += $"{message.Sender}: {_textInMessage} | {message.Date} - {message.Time}";

            var cursorPosition = Console.GetCursorPosition();
            Console.SetCursorPosition(cursorPosition.Left, cursorPosition.Top - 1);
            Console.WriteLine("");
            Console.SetCursorPosition(cursorPosition.Left, cursorPosition.Top - 1);
            Console.WriteLine(line);
        }
    }
}
