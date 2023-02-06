using System;
using ServiceStack;
using ServiceStack.Text;

namespace RedisClientAchieve
{
    public static class RedisExtensions
    {
        /// <summary>
        /// 拆分传入的 Host 
        /// passward：item1
        /// host：item2
        /// port：item3
        /// </summary>
        /// <param name="host">传入Host</param>
        /// <returns>拆分后的Host信息组合</returns>
        public static Tuple<string, string, int> ToHostAndPassword(this string host)
        {
            if (host == null)
            {
                return Tuple.Create<string, string, int>("", "", 0);
            }
            if (host.Contains("@"))
            {
                string[] array = StringExtensions.SplitOnLast(host, '@');
                string item = array[0];
                array = array[1].Split(new char[]
                {
                    ':'
                });
                if (array.Length == 0)
                {
                    throw new ArgumentException("'{0}' is not a valid Host or IP Address: e.g. '127.0.0.0[:11211]'", array[1]);
                }
                int item2 = (array.Length == 1) ? 6379 : int.Parse(array[1]);
                return Tuple.Create<string, string, int>(item, array[0], item2);
            }
            else
            {
                string[] array = host.Split(new char[]
                {
                    ':'
                });
                if (array.Length == 0)
                {
                    throw new ArgumentException("'{0}' is not a valid Host or IP Address: e.g. '127.0.0.0[:11211]'", array[1]);
                }
                int item3 = (array.Length == 1) ? 6379 : int.Parse(array[1]);
                return Tuple.Create<string, string, int>("", array[0], item3);
            }
        }
    }
}
