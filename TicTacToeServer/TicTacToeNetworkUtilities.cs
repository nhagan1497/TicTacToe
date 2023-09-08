using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

using TicTacToe;

namespace TicTacToeNetwork
{
    public class Message
    {
        public int? Move { get; set; }
        public bool? GetBoard { get; set; }
        public int[]? Board { get; set; }
        public int? PlayerNumber { get; set; }
        public int? PlayerID { get; set; }
        public int? Turn { get; set; }
        public bool? Success { get; set; }

        public Message() { }
        public Message(TicTacToeBoard board) 
        {
            int[] boardMessage = new int[9];
            int turn = board.Turn switch
            {
                Player.PlayerOne => 1,
                Player.PlayerTwo => 2,
                _ => 0
            };
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    boardMessage[i * 3 + j] = board.Board[i, j] switch
                    {
                        Player.PlayerOne => 1,
                        Player.PlayerTwo => 2,
                        _ => 0
                    };
                }
            }

            Board = boardMessage;
            Turn = turn;
        }
        public override string ToString()
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };

            return JsonSerializer.Serialize(this, options);
        }
        public TicTacToeBoard GetTTTBoard()
        {
            if (Board == null || Turn == null)
                return new TicTacToeBoard();

            Player[,] twoDimBoard = new Player[3, 3];
            Player enumPlayer;

            for(int i = 0; i < 3; i++)
            {
                for(int j = 0; j < 3; j++)
                {
                    twoDimBoard[i,j] = Board[i * 3 + j] switch
                    {
                        1 => Player.PlayerOne,
                        2 => Player.PlayerTwo,
                        _ => Player.None
                    };
                }
            }
            enumPlayer = Turn switch
            {
                1 => Player.PlayerOne,
                2 => Player.PlayerTwo,
                _ => Player.None
            };

            TicTacToeBoard tttBoard = new(twoDimBoard, enumPlayer);
            return tttBoard;
        }

        public async Task SendMessageAsync(NetworkStream stream, CancellationToken cancellationToken)
        {
            var messageBytes = Encoding.UTF8.GetBytes(ToString());
            await stream.WriteAsync(messageBytes, cancellationToken);
        }

        public static async Task<Message?> GetMessageAsync(NetworkStream stream, CancellationToken cancellationToken)
        {
            var buffer = new byte[1_024];
            int received = await stream.ReadAsync(buffer, cancellationToken);

            var jsonMessage = Encoding.UTF8.GetString(buffer, 0, received);
            return JsonSerializer.Deserialize<Message>(jsonMessage);
        }
    }

    public class GameSession
    {
        public int PlayerOneID { get;}

        public int _playerTwoID;
        public int PlayerTwoID
        {
            get { return _playerTwoID; }
            set
            {
                if (_playerTwoID == 0)
                {
                    _playerTwoID = value;
                }
            }
        }
        public TicTacToeBoard Board { get;}

        public GameSession(TicTacToeBoard board, int playerOneID, int playerTwoID=0)
        {
            PlayerOneID = playerOneID;
            PlayerTwoID = playerTwoID;
            Board = board;
        }

        public bool AddPlayerTwo(int playerTwoID)
        {
            if(playerTwoID == 0)
            {
                PlayerTwoID = playerTwoID;
                return true;
            }
            return false;
        }

        public Player GetPlayerNumber(int playerID)
        {
            if(PlayerOneID == playerID)
            {
                return Player.PlayerOne;
            }
            else if(PlayerTwoID == playerID)
            {
                return Player.PlayerTwo;
            }
            else
            {
                return Player.None;
            }
        }
    }
}
