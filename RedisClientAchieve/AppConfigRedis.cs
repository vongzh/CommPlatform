using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace RedisClientAchieve
{
    public static class AppConfigRedis
    {

        private const int KBSIZE = 1024;

        public static string RedisConnection
        {
            get
            {
                return ConfigurationManager.AppSettings["RedisConnection"];
            }
        }

        public static string RedisConnectionData
        {
            get
            {
                return ConfigurationManager.AppSettings["RedisConnectionData"];
            }
        }

        public static int MaxReadPoolSize
        {
            get
            {
                int.TryParse(ConfigurationManager.AppSettings["MaxReadPoolSize"], out int result);
                if (result <= 0)
                {
                    result = 4096;
                }
                return result;
            }
        }

        public static int MaxWritePoolSize
        {
            get
            {
                int.TryParse(ConfigurationManager.AppSettings["MaxWritePoolSize"], out int result);
                if (result <= 0)
                {
                    result = 4096;
                }
                return result;
            }
        }

        /// <summary>
        /// 是否启用GZip压缩 1压缩（默认） 0不压缩
        /// </summary>
        public static int IsUseGZip
        {
            get
            {
                if (!int.TryParse(ConfigurationManager.AppSettings["IsUseGZip"], out int result))
                {
                    result = 1;
                }
                return result;
            }
        }

        /// <summary>
        /// 使用Gzip压缩的起始字符串大小（kb）  最小为2kb
        /// </summary>
        public static double UseGZipMinSize
        {
            get
            {
                double.TryParse(ConfigurationManager.AppSettings["UseGZipMinSize"], out double num);

                if (num < 2.0)
                {
                    num = 2.0;
                }
                return num * KBSIZE;
            }
        }

        /// <summary>
        /// 使用ItemRemoveAll时 能够移除的最大Key的数量
        /// </summary>
        public static int RedisCanRemoveRangeMaxCount
        {
            get
            {
                int.TryParse(ConfigurationManager.AppSettings["RedisCanRemoveRangeMaxCount"], out int result);
                return result;
            }
        }

        /// <summary>
        /// 默认DB ID
        /// </summary>
        public static int RedisDefaultDb
        {
            get
            {
                int.TryParse(ConfigurationManager.AppSettings["RedisDefaultDb"], out int result);
                return result;
            }
        }
    }
}
