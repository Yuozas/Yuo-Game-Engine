using SharpDX;

namespace GameEngine.Core;

public class TransformComponent
{
	public Vector3    Position { get; set; }
	public Quaternion Rotation { get; set; }
	public Vector3    Scale    { get; set; }

	// Constructor to initialize the properties
	public TransformComponent(Vector3 position, Quaternion rotation, Vector3 scale)
	{
		Position = position;
		Rotation = rotation;
		Scale    = scale;
	}

	// Default constructor
	public TransformComponent()
	{
		Position = Vector3.Zero;
		Rotation = Quaternion.Identity;
		Scale    = Vector3.One;
	}
}