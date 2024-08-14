using System.Text;

namespace GameEngine.DataSaving;

public class BinarySerializer
{
	// Serialize object to byte array
	public byte[] Serialize(object obj)
	{
		ArgumentNullException.ThrowIfNull(obj);

		var json = System.Text.Json.JsonSerializer.Serialize(obj);
		return Encoding.UTF8.GetBytes(json);
	}

	// Deserialize byte array to object of type T
	public T? Deserialize<T>(byte[] data)
	{
		ArgumentNullException.ThrowIfNull(data);

		var json = Encoding.UTF8.GetString(data);
		return System.Text.Json.JsonSerializer.Deserialize<T>(json);
	}
}