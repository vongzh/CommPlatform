using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Text;

namespace RedisClientAchieve.Units
{
    public class JsonSerializer : ISerializerUnits
    {
        public byte[] SerializeToByteArray<T>(T t)
        {
            return JsonSerializerUnits.SerializeObjectToByteArray(t);
        }
        public string Serialize<T>(T t)
        {
            return JsonSerializerUnits.SerializeObject(t);
        }

        public T DeserializeFromByteArraye<T>(byte[] content)
        {
            return JsonSerializerUnits.DeserializeObjectFromByteArray<T>(content);
        }

        public T Deserialize<T>(string content)
        {
            return JsonSerializerUnits.DeserializeObject<T>(content);
        }
    }

    public static class JsonSerializerUnits
    {
        public static string SerializeObject(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public static string SerializeObject(object obj, JsonSerializerSettings setting)
        {
            return JsonConvert.SerializeObject(obj, setting);
        }

        public static byte[] SerializeObjectToByteArray(object obj)
        {
            return Encoding.UTF8.GetBytes(JsonSerializerUnits.SerializeObject(obj));
        }

        public static T DeserializeObject<T>(string jsonText)
        {
            return JsonConvert.DeserializeObject<T>(jsonText);
        }

        public static object DeserializeObject(string jsonText)
        {
            return JsonConvert.DeserializeObject(jsonText);
        }

        public static object DeserializeObject(string jsonText, Type type)
        {
            return JsonConvert.DeserializeObject(jsonText, type);
        }

        public static T DeserializeObjectFromByteArray<T>(byte[] jsonBytes)
        {
            return JsonSerializerUnits.DeserializeObject<T>((jsonBytes != null) ? Encoding.UTF8.GetString(jsonBytes) : null);
        }

        public static JObject Parse(string json)
        {
            return JObject.Parse(json);
        }
    }
}
