using SharpDX;

namespace GameEngine.Rendering;

public interface IRenderingApi : IDisposable
{
	void Initialize();
	void Clear(Vector4         color);
	void DrawRectangle(Vector2 position, Vector2 size, Vector4 color);
	void Present();
	bool ProcessMessages();
}