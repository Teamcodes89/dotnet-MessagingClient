using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace dotnet_MessagingClient
{
    class Program
    {

        public static Objects.External.ConnectionRequest ConnectionRequest = null;
        public static string URL = string.Empty;
        static Thread connectingThread = null;
        public static bool Connecting = true;

        static void Main(string[] args)
        {
            Startup();
        }

        private static void Startup()
        {
            Console.WriteLine($"Please write the server to connect with:\n");
            URL = Console.ReadLine();
            ConnectionRequest = new Objects.External.ConnectionRequest()
            {
                PhoneNumber = Guid.NewGuid().ToString()
            };

            connectingThread = new Thread(ConnectingThread.Start);
            connectingThread.Start();
            while(Connecting)
            {
                Thread.Sleep(100);
            }
            MessagingPTP.Startup();
        }

    }
}
