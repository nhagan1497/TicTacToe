//using TicTacToe;

using TicTacToe;

namespace TicTacToeTests
{
    [TestClass]
    public class TicTacToeBoardTest
    {
        [TestMethod]
        public void ColumnWin()
        {
            Player[,] boardArray = new Player[3, 3];

            boardArray[0, 0] = Player.PlayerOne;
            boardArray[1, 0] = Player.PlayerOne;
            boardArray[2, 0] = Player.PlayerOne;
            TicTacToeBoard board = new TicTacToeBoard(boardArray, Player.PlayerTwo);
            bool isOver = board.GetWinner(out Player winner);

            Assert.IsTrue(isOver);
            Assert.AreEqual(winner, Player.PlayerOne);
        }

        [TestMethod]
        public void RowWin()
        {
            Player[,] boardArray = new Player[3, 3];

            boardArray[1, 0] = Player.PlayerTwo;
            boardArray[1, 1] = Player.PlayerTwo;
            boardArray[1, 2] = Player.PlayerTwo;
            TicTacToeBoard board = new TicTacToeBoard(boardArray, Player.PlayerTwo);
            bool isOver = board.GetWinner(out Player winner);

            Assert.IsTrue(isOver);
            Assert.AreEqual(winner, Player.PlayerTwo);
        }

        [TestMethod]
        public void DiagWin()
        {
            bool onMove, onWin;
            TicTacToeBoard board = new TicTacToeBoard();
            onMove = board.MakeMove(5);
            board.MakeMove(2);
            board.MakeMove(1);
            board.MakeMove(3);
            board.MakeMove(9);

            onWin = board.GetWinner(out Player winner);

            Assert.IsTrue(onWin);
            Assert.IsTrue(onMove);
            Assert.AreEqual(winner, Player.PlayerOne);
        }

        [TestMethod]
        public void InvalidMove()
        {
            bool onMove;
            TicTacToeBoard board = new TicTacToeBoard();

            board.MakeMove(2);
            board.MakeMove(1);
            onMove = board.MakeMove(0, 0);

            Assert.IsFalse(onMove);
        }

        [TestMethod]
        public void Draw()
        {
            bool onMove, onWin;
            Player[,] playerBoard = new Player[,]{ { Player.PlayerOne, Player.PlayerTwo, Player.PlayerOne},
                                          { Player.PlayerOne, Player.PlayerTwo, Player.PlayerOne},
                                          { Player.PlayerTwo, Player.PlayerOne, Player.PlayerTwo}
                                     };

            TicTacToeBoard board = new(playerBoard, Player.PlayerTwo);

            onMove = board.MakeMove(1);
            onWin = board.GetWinner(out Player winner);

            Assert.IsFalse(onMove);
            Assert.IsTrue(onWin);
            Assert.AreEqual(winner, Player.None);

        }
    }
}
