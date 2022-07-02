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
    class ReceiveMessage
    {
        private static Thread receiveMessageThread = null;
        private static ClientWebSocket clientWebSocket = null;

        public static void Startup()
        {
            receiveMessageThread = new Thread(ReceiveMessages);
            receiveMessageThread.Start();
            clientWebSocket = ConnectingThread.ws;
        }

        private static void ReceiveMessages()
        {
            while (true)
            {
                if (ConnectingThread.ws.State == WebSocketState.Open)
                {
                    byte[] buffer = new byte[256];
                    var result = clientWebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None).GetAwaiter().GetResult();
                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        clientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None).GetAwaiter().GetResult();
                    }
                    else
                    {
                        HandleReceivedMessage(buffer, result.Count);
                    }
                }
                else Thread.Sleep(500);
            }
        }

        private static void HandleReceivedMessage(byte[] buffer, int count)
        {
            string messageReceived = Encoding.UTF8.GetString(buffer, 0, buffer.Length);
            Objects.External.MessageRequest messageRequest = JsonConvert.DeserializeObject<Objects.External.MessageRequest>(messageReceived);
            DrawReceivedMessage(messageRequest);
        }
        private static void DrawReceivedMessage(Objects.External.MessageRequest message)
        {
            int indentLevel = 2;
            int IndentSize = 8;
            string _textInMessage = Encoding.ASCII.GetString(message.Text, 0, message.Text.Length);
            var line = new string(' ', indentLevel * IndentSize);
            line += $"{message.Sender}: {_textInMessage} | {message.Date} - {message.Time}";
            Console.WriteLine(line);
        }
    }
}
