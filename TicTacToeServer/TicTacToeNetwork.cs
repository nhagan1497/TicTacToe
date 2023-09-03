using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using System.Net.Sockets;

using TicTacToe;
using System.IO;

namespace TicTacToeNetwork
{
    public class TicTacToeServer
    {
        private IPEndPoint EndPoint { get; set; }
        public TicTacToeServer(string ipAddress = "192.168.0.71", int port = 12345) 
        {
            EndPoint = new IPEndPoint(IPAddress.Parse(ipAddress), port);
        }
        public async void RunServer()
        {
            TcpListener listener = new TcpListener(EndPoint);

            try
            {
                listener.Start();

                using TcpClient handler = await listener.AcceptTcpClientAsync();
                await using NetworkStream stream = handler.GetStream();

                var message = $"📅 {DateTime.Now} 🕛";
                var dateTimeBytes = Encoding.UTF8.GetBytes(message);
                await stream.WriteAsync(dateTimeBytes);

                Console.WriteLine($"Sent message: \"{message}\"");
            }
            finally
            {
                listener.Stop();
            }
        }
    }

    public class TicTacToeClient
    {
        private IPEndPoint EndPoint { get; set; }

        public TicTacToeClient(string ipAddress = "192.168.0.71", int port = 12345)
        {
            EndPoint = new IPEndPoint(IPAddress.Parse(ipAddress), port);
        }
        public async void ConnectToServer() 
        {
            using TcpClient client = new();
            await client.ConnectAsync(EndPoint);
            await using NetworkStream stream = client.GetStream();

            var buffer = new byte[1_024];
            int received = await stream.ReadAsync(buffer);

            var message = Encoding.UTF8.GetString(buffer, 0, received);
            Console.WriteLine($"Message received: \"{message}\"");
        }
    }
}
