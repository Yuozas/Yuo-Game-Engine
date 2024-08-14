using GameEngine.Rendering;
using GameEngine.Input;
using GameEngine.Audio;
using GameEngine.Physics;

namespace GameEngine.Core;

public class Engine : IDisposable
{
	private readonly RenderingSystem _renderingSystem = new();
	private readonly InputManager    _inputManager    = new();
	private readonly AudioSystem     _audioSystem     = new();
	private readonly PhysicsEngine   _physicsEngine   = new();
	private          bool            _isRunning;

	private Action? _updateCallback;
	private Action? _renderCallback;

	public void Initialize(IRenderingApi renderingApi)
	{
		Console.WriteLine("Initializing engine...");

		_renderingSystem.SetApi(renderingApi);

		Console.WriteLine("Engine initialized.");
	}

	public void Run()
	{
		_isRunning = true;
		Console.WriteLine("Engine running. Close the window to quit.");

		while (_isRunning)
		{
			var api = _renderingSystem.GetApi();
			if (api is null)
				throw new InvalidOperationException("Rendering API is not set.");

			_isRunning = api.ProcessMessages();

			if (_isRunning)
				try
				{
					_updateCallback?.Invoke();
					_renderCallback?.Invoke();
				}
				catch (Exception ex)
				{
					Console.WriteLine($"Error during game loop: {ex.Message}");
					// You might want to add some recovery logic here
					// For example, you could try to reinitialize the rendering system
					// or simply continue to the next frame
				}

			Thread.Sleep(16); // Cap at roughly 60 FPS
		}
	}

	public void SetUpdateCallback(Action callback) => _updateCallback = callback;

	public void SetRenderCallback(Action callback) => _renderCallback = callback;

	public RenderingSystem? GetRenderingSystem() => _renderingSystem;

	public void Shutdown()
	{
		_isRunning = false;
		Console.WriteLine("Engine shutting down.");
	}

	public void Dispose() => _renderingSystem.Dispose();
}