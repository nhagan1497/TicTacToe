using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TicTacToe;

namespace TicTacToeNetwork
{
    public class TicTacToeClient : IDisposable
    {
        private IPEndPoint EndPoint { get; set; }
        private TicTacToeBoard Board { get; set; }
        private Player PlayerNumber { get; set; }
        private TcpClient Client { get; set; }
        private int ConnectionID { get; set; }

        public TicTacToeClient(string ipAddress, int port) : this(new IPEndPoint(IPAddress.Parse(ipAddress), port))
        {
        }

        public TicTacToeClient() : this(new IPEndPoint(IPAddress.Loopback, 12345))
        {
        }
        public TicTacToeClient(IPEndPoint endPoint)
        {
            EndPoint = endPoint;
            Board = new TicTacToeBoard();
            PlayerNumber = Player.None;
            Client = new TcpClient();
            ConnectionID = -1;
        }

        public async Task ConnectToServer()
        {
            await Client.ConnectAsync(EndPoint);
            await using NetworkStream stream = Client.GetStream();

            var buffer = new byte[1_024];
            int received = await stream.ReadAsync(buffer);

            var jsonMessage = Encoding.UTF8.GetString(buffer, 0, received);
            Message? message = JsonSerializer.Deserialize<Message>(jsonMessage);

            if (message == null)
            {
                Console.WriteLine("Client failed to deserialize message.");
                return;
            }
            Console.WriteLine($"Message received: \"{message}\"");
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Client.Dispose();
            }
        }
    }
}
