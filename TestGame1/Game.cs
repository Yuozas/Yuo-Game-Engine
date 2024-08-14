using GameEngine.Core;
using GameEngine.Rendering;
using GameEngine.Input;
using SharpDX;
using Keys = GameEngine.Input.Keys;

namespace TestGame1;

public class Game : IDisposable
{
	private readonly Engine                                     _engine;
	private          Vector2                                    _playerPosition;
	private readonly Vector2                                    _playerSize  = new(50, 50);
	private readonly float                                      _playerSpeed = 5.0f;
	private readonly List<(Vector2 Position, Vector2 Velocity)> _obstacles   = new();
	private readonly Random                                     _random      = new();

	public Game()
	{
		try
		{
			_engine         = new();
			_playerPosition = new(400, 300);
			InitializeObstacles();
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Error initializing Game: {ex.Message}");
			Console.WriteLine($"StackTrace: {ex.StackTrace}");
			throw; // Re-throw the exception after logging
		}
	}

	private void InitializeObstacles()
	{
		for (var i = 0; i < 5; i++)
		{
			Vector2 position = new(_random.Next(0, 800), _random.Next(0, 600));
			Vector2 velocity = new((float)(_random.NextDouble() * 4 - 2), (float)(_random.NextDouble() * 4 - 2));
			_obstacles.Add((position, velocity));
		}
	}

	public void Run()
	{
		try
		{
			Console.WriteLine("Initializing DirectX12API...");
			_engine.Initialize(new DirectX12API("TestGame1"));
			Console.WriteLine("DirectX12API initialized successfully.");

			_engine.SetUpdateCallback(Update);
			_engine.SetRenderCallback(Render);
			_engine.Run();
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Error in Game.Run: {ex.Message}");
			Console.WriteLine($"StackTrace: {ex.StackTrace}");
		}
	}

	private void Update()
	{
		Console.WriteLine("Update method called"); // Debug log

		var input = InputManager.GetInput();

		if (input.IsKeyPressed(Keys.Up))
			_playerPosition.Y = Math.Max(0, _playerPosition.Y - _playerSpeed);

		if (input.IsKeyPressed(Keys.Down))
			_playerPosition.Y = Math.Min(600 - _playerSize.Y, _playerPosition.Y + _playerSpeed);

		if (input.IsKeyPressed(Keys.Left))
			_playerPosition.X = Math.Max(0, _playerPosition.X - _playerSpeed);

		if (input.IsKeyPressed(Keys.Right))
			_playerPosition.X = Math.Min(800 - _playerSize.X, _playerPosition.X + _playerSpeed);

		for (var i = 0; i < _obstacles.Count; i++)
		{
			var (position, velocity) =  _obstacles[i];
			position                 += velocity;

			if (position.X < 0 || position.X > 800 - 30)
				velocity.X *= -1;
			if (position.Y < 0 || position.Y > 600 - 30)
				velocity.Y *= -1;

			_obstacles[i] = (position, velocity);
		}
	}

	private void Render()
	{
		Console.WriteLine("Render method called");

		try
		{
			var renderingSystem = _engine.GetRenderingSystem();
			if (renderingSystem == null)
				throw new InvalidOperationException("RenderingSystem is not initialized.");

			renderingSystem.Clear(new(0.1f, 0.1f, 0.3f, 1.0f)); // Dark blue background

			Console.WriteLine($"Drawing player at position: {_playerPosition}");
			renderingSystem.DrawRectangle(_playerPosition, _playerSize, new(0, 1, 0, 1)); // Green player

			// Draw obstacles
			foreach (var (position, _) in _obstacles)
			{
				Console.WriteLine($"Drawing obstacle at position: {position}");
				renderingSystem.DrawRectangle(position, new(30, 30), new(1, 0, 0, 1)); // Red obstacles
			}

			renderingSystem.Present();
		}
		catch (SharpDXException ex)
		{
			Console.WriteLine($"SharpDX Exception in Render: {ex.Message}");
			Console.WriteLine($"HRESULT: {ex.ResultCode}");
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Exception in Render: {ex.Message}");
			Console.WriteLine($"StackTrace: {ex.StackTrace}");
		}
	}

	public void Shutdown() => _engine.Shutdown();

	public void Dispose() => _engine.Dispose();
}