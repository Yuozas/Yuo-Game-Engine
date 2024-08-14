namespace GameEngine.Core;

public class RenderComponent
{
	private Mesh     _mesh;
	private Material _material;

	public RenderComponent(Mesh mesh, Material material)
	{
		_mesh     = mesh;
		_material = material;
	}

	public void Render() =>
			// Example rendering code:
			Console.WriteLine("Rendering Mesh with given Material...");
	// You would replace this with your actual rendering logic
}

public class Mesh
{
	public float[] Vertices { get; set; }
	public int[]   Indices  { get; set; }

	public Mesh(float[] vertices, int[] indices)
	{
		Vertices = vertices;
		Indices  = indices;
	}
}

public class Material
{
	public string  Shader { get; set; }
	public float[] Color  { get; set; }

	public Material(string shader, float[] color)
	{
		Shader = shader;
		Color  = color;
	}
}