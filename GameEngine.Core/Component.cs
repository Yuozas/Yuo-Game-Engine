namespace GameEngine.Core;

public abstract class Component
{
	protected GameObject GameObject;

	public virtual void Initialize()
	{ }

	public abstract void Update();
}