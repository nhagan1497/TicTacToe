using TicTacToeNetwork;



TicTacToeServer server = new TicTacToeServer();
TicTacToeClient client = new TicTacToeClient();

server.RunServer();
client.ConnectToServer();

await Task.Delay(1000);
