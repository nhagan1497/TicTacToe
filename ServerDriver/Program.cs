using TicTacToeNetwork;

// Instance of the server to handle requests for the UI that's being tested
using TicTacToeServer server = new();

// Instance of the client so I can push commands more conveniently from the command line to test functionality from the UI
using TicTacToeClient player2 = new();

// Starts the server and keeps the task for awaiting the shutdown
Task runningServer = server.Start();

Console.WriteLine("Server is running. Type commands for the client, help for options, and exit to quit. Case insensitive.");


string? command = " ";
bool response = true;

while (command != "exit")
{
    command = Console.ReadLine();
    if (String.IsNullOrEmpty(command))
        continue;
    command = command.ToLower();
    switch (command)
    {
        case "newgame":
            response = await player2.StartNewGame();
            Console.WriteLine(response);
            break;

        case "move":
            Console.WriteLine("Enter what number square:");

            string? squareString = Console.ReadLine();
            if (String.IsNullOrEmpty(squareString))
                break;
            int square = Int32.Parse(squareString);
            response = await player2.MakeMove(square);
            Console.WriteLine(response);
            break;
        case "getboard":
            response = await player2.GetBoard();
            Console.WriteLine(response);
            break;
        case "help":
            Console.WriteLine("Commands:");
            Console.WriteLine("    NewGame");
            Console.WriteLine("    Move");
            Console.WriteLine("    GetBoard");
            Console.WriteLine("    Exit");
            Console.WriteLine("    Help");
            break;
        default:
            break;
    }

}

server.Stop();
await runningServer;