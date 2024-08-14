using SharpDX;

namespace GameEngine.Rendering;

public class RenderingSystem : IDisposable
{
	private IRenderingApi? _currentApi;

	public void SetApi(IRenderingApi api)
	{
		_currentApi = api;
		_currentApi.Initialize();
	}

	public IRenderingApi? GetApi() => _currentApi;

	public void Clear(Vector4 color)
	{
		if (_currentApi is null)
			throw new InvalidOperationException($"{nameof(_currentApi)} is not set");

		_currentApi.Clear(color);
	}

	public void DrawRectangle(Vector2 position, Vector2 size, Vector4 color)
	{
		if (_currentApi is null)
			throw new InvalidOperationException($"{nameof(_currentApi)} is not set");

		_currentApi.DrawRectangle(position, size, color);
	}

	public void Present()
	{
		if (_currentApi is null)
			throw new InvalidOperationException($"{nameof(_currentApi)} is not set");

		_currentApi.Present();
	}

	public void Dispose() => _currentApi?.Dispose();
}