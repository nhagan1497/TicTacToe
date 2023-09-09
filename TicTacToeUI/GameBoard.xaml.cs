using TicTacToeNetwork;
using TicTacToe;

namespace TicTacToeUI;

public partial class GameBoard : ContentPage
{
	private TicTacToeBoard Board { get; set; }
	private ImageButton[] Buttons { get; set; }
	private TicTacToeClient Client { get; set; }

	private bool Quit { get; set; }

	public GameBoard()
	{
		InitializeComponent();
		Board = new TicTacToeBoard();
		Client = new TicTacToeClient();
		Buttons = new ImageButton[9] { Button0, Button1, Button2, Button3, Button4, Button5, Button6, Button7, Button8 };
		Quit = false;
        InitializeGame();
	}

	private void OnButtonClicked(object sender, EventArgs e)
	{
		if (sender is ImageButton button)
		{
			int buttonIndex = (Grid.GetColumn(button) / 2) + (Grid.GetRow(button) / 2) * 3;
			RequestMove(buttonIndex+1);
        }
	}

	private void UpdateButtonImages()
	{
		for(int i = 0; i < 3; i++)
		{
			for(int j = 0; j < 3; j++)
			{
				string newSource = Board.Board[i, j] switch { Player.PlayerOne => "tictactoe_x.png", Player.PlayerTwo => "tictactoe_o.png", _ => null };
				Buttons[i * 3 + j].Source = newSource;
			}
		}
	}

	private async void InitializeGame()
	{
		await Client.StartNewGame();
		Board = Client.Board;
		UpdateButtonImages();

		RefreshBoard();
	}

	private async void RequestMove(int square)
	{
		bool success = await Client.MakeMove(square);
		if(success)
		{
			Board = Client.Board;
			UpdateButtonImages();
		}
	}
	private async void RefreshBoard()
	{
		while (!Quit)
		{
			await Client.GetBoard();
			Board = Client.Board;
			UpdateButtonImages();
			await Task.Delay(10000);
		}
	}
}