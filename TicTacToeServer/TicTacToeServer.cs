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
        private CancellationTokenSource CTSource { get; set; }
        private CancellationToken CToken { get; set; }
        private Dictionary<int, GameSession> Games { get; set; }
        private int CurrentConnectionID {  get; set; }
        private int? WaitingPlayer {  get; set; }
        public TicTacToeServer(string ipAddress, int port) : this(new IPEndPoint(IPAddress.Parse(ipAddress), port))
        {
        }

        public TicTacToeServer() : this(new IPEndPoint(IPAddress.Loopback, 12345))
        {
        }

        public TicTacToeServer(IPEndPoint endPoint)
        {
            Listener = new TcpListener(endPoint);
            CTSource = new CancellationTokenSource();
            CToken = CTSource.Token;
            CurrentConnectionID = 1;
            Games = new Dictionary<int, GameSession>();
            WaitingPlayer = null;
        }
        public Task Start()
        {
            Listener.Start();
            return ListenForConnectionsAsync();
        }

        public void Stop()
        {
            CTSource.Cancel();
            Listener.Stop();
        }
        private async Task ListenForConnectionsAsync()
        {
            while(!CToken.IsCancellationRequested)
            {
                try
                {
                    TcpClient client = await Listener.AcceptTcpClientAsync(CToken);
                    _ = HandleConnection(client);                    
                }
                catch(OperationCanceledException)
                {
                    break;
                }
                
            }
        }

        private async Task HandleConnection(TcpClient client)
        {
            using NetworkStream stream = client.GetStream();
            Message? message = await Message.GetMessageAsync(stream, CToken);
            if(message == null)
                return;

            Message returnMessage;

            // Checks if the player has already been assigned an ID
            if(message.PlayerID == null)
            {
                //Player does not have an ID already and must be assigned one.
                int newPlayerID = CurrentConnectionID;
                CurrentConnectionID++;

                // If we don't have a player waiting, send this player a new board and assign them to a new game as player one
                if (WaitingPlayer == null)
                {
                    TicTacToeBoard board = new();

                    returnMessage = new Message(board);
                    returnMessage.PlayerID = newPlayerID;
                    returnMessage.PlayerNumber = 1;

                    // Creates a new games and stores it in the active games
                    GameSession session = new(board, newPlayerID);
                    Games[newPlayerID] = session;
                    WaitingPlayer = newPlayerID;
                }

                // If another player is waiting on a game, add them to the existing game as player two and send them the board
                else
                {
                    GameSession session = Games[(int)WaitingPlayer];

                    returnMessage = new Message(session.Board);
                    returnMessage.PlayerNumber = 2;
                    returnMessage.PlayerID = newPlayerID;

                    session.PlayerTwoID = newPlayerID;
                    Games[newPlayerID] = session;
                    WaitingPlayer = null;
                    
                }
            }
            else
            {
                int requestingPlayer = (int)message.PlayerID;
                GameSession session = Games[requestingPlayer];
                TicTacToeBoard board = session.Board;

                // Check if the player is making a move
                if(message.Move != null)
                {
                    // Check if it is the current player's turn, if it is not send a fail message with the current board
                    if(board.Turn != session.GetPlayerNumber(requestingPlayer))
                    {
                        returnMessage = new Message(board);
                        returnMessage.Success = false;
                    }
                    else
                    {
                        bool success = board.MakeMove((int)message.Move);
                        if(success)
                        {
                            returnMessage = new Message();
                            returnMessage.Success = true;
                        }
                        else
                        {
                            returnMessage = new Message(board);
                            returnMessage.Success = false;
                        }
                            
                    }
                }

                // Check if the player wants a current copy of the board
                else if (message.GetBoard == true)
                {
                    returnMessage = new Message(board);
                }

                // Not sure what the player wants, return the board with a failed response
                else
                {
                    returnMessage = new Message(board);
                    returnMessage.Success = false;
                }
            }

            await returnMessage.SendMessageAsync(stream, CToken);
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

        private GameSession GetGameSession(int playerID)
        {
            return Games[playerID];
        }

        private void AddGameSession(GameSession game)
        {
            Games[game.PlayerOneID] = game;
            Games[game.PlayerTwoID] = game;
        }

        private void RemoveGameSession(GameSession game) 
        {
            Games.Remove(game.PlayerOneID);
            Games.Remove(game.PlayerTwoID);
        }
    }

    
}
