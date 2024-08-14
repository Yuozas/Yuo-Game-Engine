namespace GameEngine.DataSaving;

public class JsonSerializer
{
	// Serialize object to JSON string
	public string Serialize(object obj) => System.Text.Json.JsonSerializer.Serialize(obj);

	// Deserialize JSON string to object of type T
	public T? Deserialize<T>(string json) => System.Text.Json.JsonSerializer.Deserialize<T>(json);
}