using System.Numerics;
using GameEngine.Input;

namespace GameEngine.UI;

public abstract class UIElement
{
	public Vector2 Position { get; set; }
	public Vector2 Size     { get; set; }

	public abstract void Render();
	public abstract void Update();
	public abstract bool HandleInput(InputEvent evt);
}