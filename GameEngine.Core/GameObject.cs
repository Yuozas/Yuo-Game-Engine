namespace GameEngine.Core;

public class GameObject
{
	private readonly List<Component> _components = [];

	public void AddComponent(Component component)
	{
		_components.Add(component);
		component.Initialize();
	}

	public T? GetComponent<T>() where T : Component => _components.OfType<T>().FirstOrDefault();

	public void Update()
	{
		foreach (var component in _components)
			component.Update();
	}
}