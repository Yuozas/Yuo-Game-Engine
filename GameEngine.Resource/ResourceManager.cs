namespace GameEngine.Resource;

public class ResourceManager
{
	private readonly Dictionary<string, object> _assets = new();

	public void LoadAsset(string assetName, object asset)
	{
		if (_assets.ContainsKey(assetName))
		{
			Console.WriteLine($"Asset {assetName} is already loaded.");
		}
		else
		{
			_assets[assetName] = asset;
			Console.WriteLine($"Asset {assetName} loaded.");
		}
	}

	public void UnloadAsset(string assetName)
	{
		if (_assets.ContainsKey(assetName))
		{
			_assets.Remove(assetName);
			Console.WriteLine($"Asset {assetName} unloaded.");
		}
		else
		{
			Console.WriteLine($"Asset {assetName} is not loaded.");
		}
	}
}