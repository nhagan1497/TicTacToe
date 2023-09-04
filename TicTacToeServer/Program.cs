using System.Text.Json;
using TicTacToeNetwork;


/*
using TicTacToeServer server = new();
using TicTacToeClient player1 = new();
using TicTacToeClient player2 = new();
    

Task serverRun = server.Start();

await player1.ConnectToServer();
await player2.ConnectToServer();

server.Stop();
await serverRun;
*/

Message message = new();
int[] board = new int[9];
for (int i = 0; i < 9; i++)
    board[i] = i + 1;
message.Board = board;
Console.WriteLine(message);

Message? test = JsonSerializer.Deserialize<Message>(message.ToString());
Console.WriteLine(test);