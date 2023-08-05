using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicTacToe
{
    public class TicTacToeBoard
    {
        public Player Turn { get; private set; }
        public Player[,] Board { get; private set; }

        public TicTacToeBoard()
        {
            Board = new Player[3, 3];
            Turn = Player.PlayerOne;
        }

        public TicTacToeBoard(Player[,] Board, Player Turn)
        {
            this.Turn = Turn;
            this.Board = Board;
        }

        private void SwitchTurn()
        {
            if( Turn == Player.PlayerOne ) 
            {
                Turn = Player.PlayerTwo;
            }
            else
            {
                Turn = Player.PlayerOne;
            }
        }

        public bool MakeMove(int square)
        {
            square--;
            int row = square / 3;
            int col = square % 3;

            return MakeMove(row, col);
        }

        public bool MakeMove(int row, int col)
        {
            if (GetWinner(out Player player))
            {
                return false;
            }

            {
                
            }
            if (Board[row, col] != Player.None)
            {
                return false;
            }

            Board[row, col] = Turn;
            SwitchTurn();
            return true;
        }

        public bool GetWinner(out Player winner)
        {
            winner = Player.None;

            for (int i = 0; i < Board.GetLength(0); i++)
            {
                // Check rows
                if (Board[i, 0] == Board[i, 1] && Board[i, 0] == Board[i, 2] && Board[i, 0] != Player.None)
                {
                    winner = Board[i, 0];
                    return true;
                }

                // Check Columns
                if (Board[0, i] == Board[1, i] && Board[0, i] == Board[2, i] && Board[0, i] != Player.None)
                {
                    winner = Board[0, i];
                    return true;
                }
            }

            // Check diagonals
            if (Board[0, 0] == Board[1, 1] && Board[0, 0] == Board[2, 2] && Board[0, 0] != Player.None)
            {
                winner = Board[0, 0];
                return true;
            }

            if (Board[0, 2] == Board[1, 1] && Board[0, 2] == Board[2, 0] && Board[0, 2] != Player.None)
            {
                winner = Board[0, 2];
                return true;
            }

            // Check if draw
            bool isDraw = true;
            for(int i = 0; i <Board.GetLength(0); i++)
            {
                for(int j = 0; j < Board.GetLength(1); j++)
                {
                    if (Board[i,j] == Player.None)
                    {
                        isDraw = false;
                        break;
                    }
                }
            }

            if (isDraw)
            {
                winner = Player.None;
                return true;
            }

            return false;
        }

        public override string ToString()
        {
            string output_string = "";

            for (int i = 0; i < Board.GetLength(0); i++)
            {
                for (int j = 0; j < Board.GetLength(1); j++)
                {
                    var playerString = Board[i, j] switch
                    {
                        Player.PlayerOne => " X ",
                        Player.PlayerTwo => " O ",
                        _ => "   "
                    };
                    output_string += playerString;
                    if (j < Board.GetLength(1) - 1)
                    {
                        output_string += "|";
                    }
                }
                if (i < Board.GetLength(0) - 1)
                {
                    output_string += '\n';
                    output_string += new string('-', Board.GetLength(1) * 4 - 1) + "\n";
                }
            }

            return output_string;
        }
    }

    public enum Player
    {
        None,
        PlayerOne,
        PlayerTwo
    }

}
