using System;
using System.Linq;
using ServiceStack.Redis;
using RedisClientAchieve.Utils;

namespace RedisClientAchieve
{
    /// <summary>
    /// 配置管理和初始化
    /// </summary>
    internal class Managers
    {
        /// <summary>
        /// 最大读数量
        /// </summary>
        internal static int MaxReadCount = 128;
        /// <summary>
        /// 最大写数量
        /// </summary>
        internal static int MaxWriteCount = 128;
        /// <summary>
        /// 默认DB
        /// </summary>
        internal static int DeafultDb = 0;

        private static string LOG_INSTANCE_NAME = "RedisLog";

        internal static LogInfoWriter RedisLog = LogInfoWriter.GetInstance(Managers.LOG_INSTANCE_NAME);

        /// <summary>
        /// Redis连接池 
        private static PooledRedisClientManager ClientManagers { get; set; }

        private static PooledRedisClientManager ClientManagersData { get; set; }

        internal PooledRedisClientManager GetClientManagers(RedisConnectionSource redisConn)
        {
            PooledRedisClientManager result;
            switch (redisConn)
            {
                case RedisConnectionSource.DefaultConn:
                    result = Managers.ClientManagers;
                    break;
                case RedisConnectionSource.DataConn:
                    result = Managers.ClientManagersData;
                    break;
                default:
                    result = Managers.ClientManagers;
                    break;
            }
            return result;
        }

        static Managers()
        {
            Managers.IntiClientManagers();
            Managers.IntiClientManagersData();
        }

        private static void IntiClientManagersData()
        {
            string redisConnectionData = AppConfigRedis.RedisConnectionData;
            if (string.IsNullOrEmpty(redisConnectionData))
            {
                throw new Exception("redis connection is null");
            }
            string[] array = redisConnectionData.Split(new char[]
            {
                char.Parse(",")
            });
            string[] array2 = new string[]
            {
                array[0]
            };
            string[] array3;
            if (array.Count<string>() != 1)
            {
                array3 = array.Skip(1).ToArray<string>();
            }
            else
            {
                (array3 = new string[1])[0] = array[0];
            }
            string[] array4 = array3;
            Managers.RedisLog.InfoFormat("Data WriteHost:{0},readwriteHosts have been connection", string.Join(",", array2));
            Managers.RedisLog.InfoFormat("Data ReadHost:{0},readonlyHosts have been connection", string.Join(",", array4));

            Managers.DeafultDb = AppConfigRedis.RedisDefaultDb;
            Managers.MaxReadCount = AppConfigRedis.MaxReadPoolSize;
            Managers.MaxWriteCount = AppConfigRedis.MaxWritePoolSize;
            Managers.ClientManagersData = new PooledRedisClientManager(array2, array4, new RedisClientManagerConfig
            {
                MaxWritePoolSize = Managers.MaxWriteCount,
                MaxReadPoolSize = Managers.MaxReadCount,
                DefaultDb = Managers.DeafultDb,
                AutoStart = true
            });
        }

        private static void IntiClientManagers()
        {
            string redisConnection = AppConfigRedis.RedisConnection;
            if (string.IsNullOrWhiteSpace(redisConnection))
            {
                throw new Exception("redis connection is null");
            }

            string[] array = redisConnection.Split(new char[]
            {
                char.Parse(",")
            });
            string[] readWriteHosts = new string[]
            {
                array[0]
            };
            string[] readOnlyHosts;
            if (array.Count<string>() != 1)
            {
                readOnlyHosts = array.Skip(1).ToArray<string>();
            }
            else
            {
                (readOnlyHosts = new string[1])[0] = array[0];
            }

            Managers.RedisLog.InfoFormat("Default WriteHost:{0},readwriteHosts have been connection", string.Join(",", readWriteHosts));
            Managers.RedisLog.InfoFormat("Default ReadHost:{0},readonlyHosts have been connection", string.Join(",", readOnlyHosts));

            Managers.DeafultDb = AppConfigRedis.RedisDefaultDb;
            Managers.MaxReadCount = AppConfigRedis.MaxReadPoolSize;
            Managers.MaxWriteCount = AppConfigRedis.MaxWritePoolSize;
            Managers.ClientManagers = new PooledRedisClientManager(readWriteHosts, readOnlyHosts,
            new RedisClientManagerConfig
            {
                MaxWritePoolSize = Managers.MaxWriteCount,
                MaxReadPoolSize = Managers.MaxReadCount,
                DefaultDb = Managers.DeafultDb,
                AutoStart = true,
            });
        }

    }
}
