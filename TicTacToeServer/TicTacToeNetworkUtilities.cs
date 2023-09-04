using System;
using System.Collections.Generic;
using System.Linq;
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
        public int? CurrentPlayer { get; set; }
        public int? PlayerID { get; set; }

        public override string ToString()
        {
            // Configure JsonSerializerOptions for indentation
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };

            // Serialize the object with indentation
            return JsonSerializer.Serialize(this, options);
        }
        public static TicTacToeBoard ConvertToBoard(int[] board, int currentPlayer)
        {
            Player[,] twoDimBoard = new Player[3, 3];
            Player enumPlayer;

            for(int i = 0; i < 3; i++)
            {
                for(int j = 0; j < 3; j++)
                {
                    twoDimBoard[i,j] = board[i * 3 + j] switch
                    {
                        1 => Player.PlayerOne,
                        2 => Player.PlayerTwo,
                        _ => Player.None
                    };
                }
            }
            enumPlayer = currentPlayer switch
            {
                1 => Player.PlayerOne,
                2 => Player.PlayerTwo,
                _ => Player.None
            };

            TicTacToeBoard tttBoard = new TicTacToeBoard(twoDimBoard, enumPlayer);
            return tttBoard;
        }
    }

    
}
