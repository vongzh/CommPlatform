using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zk.HotelPlatform.Utils 
{
    /// <summary>
    /// JSON格式转换工具类
    /// </summary>
    public class JsonUtils<T>
    {
        public static JsonSerializerSettings settings = new JsonSerializerSettings()
        {
            NullValueHandling = NullValueHandling.Ignore
        };

        public static string Object2String(T param)
        {
            return JsonConvert.SerializeObject(param, Formatting.None, settings);
        }

        public static T String2Object(string str)
        {
            T result = default(T);
            if (string.IsNullOrEmpty(str))
            {
                return result;
            }

            try
            {
                result = JsonConvert.DeserializeObject<T>(str);
            }
            catch (Exception e)
            {
                Console.WriteLine("[JsonUtils] json parse exception! parse string is {0}", str);
                Console.WriteLine(e);
            }
            return result;
        }
    }
}
