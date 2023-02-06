using System;

namespace RedisClientAchieve.Units
{
	public interface ISerializerUnits
	{
		byte[] SerializeToByteArray<T>(T t);

		string Serialize<T>(T t);

		T DeserializeFromByteArraye<T>(byte[] content);

		T Deserialize<T>(string content);
	}
}
