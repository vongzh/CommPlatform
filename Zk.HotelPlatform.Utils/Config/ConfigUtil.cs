using System;
using System.Configuration;

namespace Zk.HotelPlatform.Utils.Config
{
    public class ConfigUtil
    {
        public static string ConnectionString(string key)
        {
            if (string.IsNullOrEmpty(key))
                return string.Empty;

            return ConfigurationManager.ConnectionStrings[key].ConnectionString;
        }

        public static string GetValue(string key)
        {
            if (string.IsNullOrEmpty(key))
                return string.Empty;

            return ConfigurationManager.AppSettings[key];
        }

        public static string GetValue(string key, string defaultValue)
        {
            var value = GetValue(key);
            if (string.IsNullOrEmpty(value))
            {
                if (string.IsNullOrEmpty(defaultValue))
                    return string.Empty;

                return defaultValue;
            }
            return value;
        }

        public static T GetValue<T>(string key)
        {
            var value = GetValue(key);
            if (string.IsNullOrEmpty(value))
                return default(T);

            return (T)Convert.ChangeType(value, typeof(T));
        }

        public static T GetValue<T>(string key, T defaultValue)
        {
            var value = GetValue(key);
            if (string.IsNullOrEmpty(value))
            {
                if (defaultValue != null)
                    return defaultValue;

                return default(T);
            }
            return (T)Convert.ChangeType(value, typeof(T));
        }
    }
}
