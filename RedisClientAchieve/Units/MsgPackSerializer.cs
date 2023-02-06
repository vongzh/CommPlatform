using System;
using System.IO;
using System.Text;
using MsgPack.Serialization;

namespace RedisClientAchieve.Units
{
    public class MsgPackSerializer : ISerializerUnits
    {
        public T Deserialize<T>(string content)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(content);
            return MsgPackSerializerUnits.Instace.DeSerialize<T>(bytes);
        }

        public T DeserializeFromByteArraye<T>(byte[] content)
        {
            return MsgPackSerializerUnits.Instace.DeSerialize<T>(content);
        }

        public string Serialize<T>(T t)
        {
            byte[] bytes = MsgPackSerializerUnits.Instace.Serialize<T>(t);
            return Encoding.UTF8.GetString(bytes);
        }

        public byte[] SerializeToByteArray<T>(T t)
        {
            return MsgPackSerializerUnits.Instace.Serialize<T>(t);
        }
    }

    public class MsgPackSerializerUnits
    {
        private SerializationContext context;

        private static Lazy<MsgPackSerializerUnits> instanceLazy = new Lazy<MsgPackSerializerUnits>(() => new MsgPackSerializerUnits(SerializationMethod.Array));

        private static Lazy<MsgPackSerializerUnits> compatibleInstanceLazy = new Lazy<MsgPackSerializerUnits>(() => new MsgPackSerializerUnits(SerializationMethod.Map));

        public static MsgPackSerializerUnits Instace
        {
            get
            {
                return MsgPackSerializerUnits.instanceLazy.Value;
            }
        }

        public static MsgPackSerializerUnits CompatibleInstace
        {
            get
            {
                return MsgPackSerializerUnits.compatibleInstanceLazy.Value;
            }
        }

        private MsgPackSerializerUnits(SerializationMethod serializationMethod)
        {
            this.context = new SerializationContext();
            this.context.SerializationMethod = serializationMethod;
        }

        public byte[] Serialize<T>(T t)
        {
            byte[] result;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                MessagePackSerializer.Get<T>(this.context).Pack(memoryStream, t);
                result = memoryStream.ToArray();
            }
            return result;
        }

        public T DeSerialize<T>(byte[] content)
        {
            T result;
            using (MemoryStream memoryStream = new MemoryStream(content))
            {
                result = MessagePackSerializer.Get<T>(this.context).Unpack(memoryStream);
            }
            return result;
        }
    }
}
