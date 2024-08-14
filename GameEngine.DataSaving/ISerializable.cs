namespace GameEngine.DataSaving;

public interface ISerializable
{
	byte[] Serialize();
	void   Deserialize(byte[] data);
}