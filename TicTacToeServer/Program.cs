using System.Text.Json;
using TicTacToeNetwork;

using TicTacToeServer server = new();
using TicTacToeClient player1 = new();
using TicTacToeClient player2 = new();


Task serverRun = server.Start();

await player1.StartNewGame();
await player2.StartNewGame();

await player1.MakeMove(1);
await player2.MakeMove(2);
await player2.MakeMove(2);

server.Stop();
await serverRun;