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
        private Player MyPlayerNumber { get; set; }
        private int PlayerID { get; set; }
        private CancellationTokenSource CTSource { get; set; }
        private CancellationToken CToken { get; set; }

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
            MyPlayerNumber = 0;
            PlayerID = -1;
            CTSource = new CancellationTokenSource();
            CToken = CTSource.Token;
        }

        private async Task<Message?> SendToServer(Message message)
        {
            using TcpClient tcpClient = new TcpClient();
            await tcpClient.ConnectAsync(EndPoint, CToken);

            using NetworkStream stream = tcpClient.GetStream();
            await message.SendMessageAsync(stream, CToken);

            return await Message.GetMessageAsync(stream, CToken);
        }

        public async Task<bool> StartNewGame()
        {
            Message message = new();
            Message? returnMessage = await SendToServer(message);

            if (returnMessage == null)
                return false;

            if (returnMessage.PlayerID == null || returnMessage.Board == null || returnMessage.PlayerNumber == null || returnMessage.Turn == null)
                return false;

            MyPlayerNumber = returnMessage.PlayerNumber switch
            {
                1 => Player.PlayerOne, 2 => Player.PlayerTwo, _ => Player.None
            };

            Board = returnMessage.GetTTTBoard();
            PlayerID = (int)returnMessage.PlayerID;

            return true;

        }

        public async Task<bool> MakeMove(int square)
        {
            if (square < 1 || square > 9)
                return false;

            if (Board.Turn != MyPlayerNumber)
            {
                await GetBoard();
                return false;
            }

            Message message = new Message();
            message.Move = square;
            message.PlayerID = PlayerID;

            Message? returnMessage = await SendToServer(message);

            if (returnMessage == null)
                return false;

            if (returnMessage.Success == false)
            {
                if (returnMessage.Board != null)
                {
                    Board = returnMessage.GetTTTBoard();
                }
                return false;
            }

            // Move made successfully, make the change locally and return true
            Board.MakeMove(square);
            return true;
        }

        public async Task<bool> GetBoard()
        {
            Message message = new Message();
            message.GetBoard = true;
            message.PlayerID = PlayerID;

            Message? returnMessage = await SendToServer(message);
            if(returnMessage == null)
            {
                return false;
            }

            Board = returnMessage.GetTTTBoard();
            return true;
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
                CTSource.Cancel();
            }
        }
    }
}
