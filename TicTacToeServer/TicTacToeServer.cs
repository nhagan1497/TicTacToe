using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using System.Net.Sockets;
using System.Text.Json;

using TicTacToe;
using System.IO;
using System.Text.Json.Serialization;

namespace TicTacToeNetwork
{
    
    public class TicTacToeServer : IDisposable
    {
        private TcpListener Listener { get; set; }
        private CancellationTokenSource TokenSource { get; set; }
        private TicTacToeBoard Board { get; set; }
        public TicTacToeServer(string ipAddress, int port) : this(new IPEndPoint(IPAddress.Parse(ipAddress), port))
        {
        }

        public TicTacToeServer() : this(new IPEndPoint(IPAddress.Loopback, 12345))
        {
        }

        public TicTacToeServer(IPEndPoint endPoint)
        {
            Listener = new TcpListener(endPoint);
            TokenSource = new CancellationTokenSource();
            Board = new TicTacToeBoard();
        }
        public Task Start()
        {
            Listener.Start();
            return ListenForConnectionsAsync(TokenSource.Token);
        }

        public void Stop()
        {
            TokenSource.Cancel();
            Listener.Stop();
        }
        private async Task ListenForConnectionsAsync(CancellationToken cancellationToken)
        {
            while(!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    TcpClient client = await Listener.AcceptTcpClientAsync(cancellationToken);
                    NetworkStream stream = client.GetStream();

                    Message message = new();

                    var messageBytes = Encoding.UTF8.GetBytes(message.ToString());
                    await stream.WriteAsync(messageBytes, cancellationToken);
                }
                catch(OperationCanceledException)
                {
                    break;
                }
                
            }
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
                Stop();
            }
        }

        ~TicTacToeServer()
        {
            Dispose(false);
        }
    }

    
}
