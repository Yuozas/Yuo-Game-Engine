using TestGame1;

Console.WriteLine("Starting the game...");

using (var game = new Game())
	game.Run();

Console.WriteLine("Game has ended. Press any key to exit.");