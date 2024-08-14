namespace GameEngine.DataSaving;

// Class SaveSystem
public class SaveSystem
{
	// Method to Save data to a file
	public void Save(ISerializable data, string filename)
	{
		var serializedData = data.Serialize();
		File.WriteAllBytes(filename, serializedData);
	}

	// Generic Method to Load data from a file
	public T Load<T>(string filename) where T : ISerializable, new()
	{
		var data = File.ReadAllBytes(filename);
		var obj  = new T();
		obj.Deserialize(data);
		return obj;
	}
}