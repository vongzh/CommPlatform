using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedisClientAchieve.Utils;
using ServiceStack;
using ServiceStack.Redis;
using ServiceStack.Redis.Support;
using ServiceStack.Redis.Pipeline;
using ServiceStack.Redis.Generic;
using RedisClientAchieve.Units;
using ServiceStack.Text;
using JsonSerializer = RedisClientAchieve.Units.JsonSerializer;

namespace RedisClientAchieve
{
    internal class RedisEntityAchieve : Managers, IRedisEntity
    {
        private RedisConnectionSource redisConnection;

        private ISerializerUnits _serializerUnits;

        private string bitoporOfLuaBodySha;

        private readonly int CanRemoveRangeMaxCount = AppConfigRedis.RedisCanRemoveRangeMaxCount;

        public RedisEntityAchieve(RedisConnectionSource redisConn)
        {
            this.redisConnection = redisConn;
            this._serializerUnits = new JsonSerializer();
        }

        public RedisEntityAchieve(RedisConnectionSource redisConn, SerializeMode serializeMode)
        {
            this.redisConnection = redisConn;
            switch (serializeMode)
            {
                case SerializeMode.Json:
                    this._serializerUnits = new JsonSerializer();
                    break;
                case SerializeMode.MsgPack:
                    this._serializerUnits = new MsgPackSerializer();
                    break;
                default:
                    break;
            }
        }


        /// <summary>
        /// 根据传入的byte[]获取对于的实体，可能会要进行GZip解压缩操作
        /// </summary>
        /// <typeparam name="T">待返回的值的类型</typeparam>
        /// <param name="value">byte[]数组</param>
        /// <returns>T</returns>
        private T GetValue<T>(byte[] value)
        {
            T result;
            if (value == null || value.Length == 0)
            {
                result = default(T);
                return result;
            }
            bool flag = typeof(T) == typeof(string);
            bool flag2 = false;
            try
            {
                RedisCompressType redisCompressType = RedisCompressType.None;
                byte[] array = new byte[(value.Length >= 4) ? (value.Length - 4) : value.Length];
                if (value.Length >= 4)
                {
                    byte[] array2 = new byte[4];
                    Buffer.BlockCopy(value, 0, array2, 0, 4);
                    try
                    {
                        redisCompressType = (RedisCompressType)Enum.Parse(typeof(RedisCompressType), Encoding.UTF8.GetString(array2));
                    }
                    catch
                    {
                        redisCompressType = RedisCompressType.None;
                        flag2 = true;
                    }
                    if (flag2)
                    {
                        array = value;
                    }
                    else
                    {
                        Buffer.BlockCopy(value, 4, array, 0, array.Length);
                    }
                }
                else
                {
                    array = value;
                }
                switch (redisCompressType)
                {
                    case RedisCompressType.None:
                    case RedisCompressType.MsgP:
                        if (flag)
                        {
                            result = (T)((object)Encoding.UTF8.GetString(array));
                        }
                        else
                        {
                            result = this._serializerUnits.DeserializeFromByteArraye<T>(array);
                        }
                        break;
                    case RedisCompressType.GZip:
                    case RedisCompressType.MZip:
                        if (flag)
                        {
                            result = (T)((object)Encoding.UTF8.GetString(GZipUtil.Decompress(array)));
                        }
                        else
                        {
                            result = this._serializerUnits.DeserializeFromByteArraye<T>(GZipUtil.Decompress(array));
                        }
                        break;
                    default:
                        if (flag)
                        {
                            result = (T)((object)Encoding.UTF8.GetString(value));
                        }
                        else
                        {
                            result = this._serializerUnits.DeserializeFromByteArraye<T>(value);
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        // Token: 0x06000142 RID: 322 RVA: 0x00006C6C File Offset: 0x00004E6C
        internal T ItemGetForDataConsistencyTool<T>(string key)
        {
            T result;
            try
            {
                using (IRedisClient redisClient = this.ReadOnlyClient(this.redisConnection))
                {
                    result = redisClient.Get<T>(key);
                }
            }
            catch (Exception exception)
            {
                Managers.RedisLog.Error(string.Format("redis read error, ErrorFunc:ItemGet.   key:{0}", key), exception);
                result = default(T);
            }
            return result;
        }

        /// <summary>
        /// 根据传入的byte[]获取对于的实体，可能会要进行GZip解压缩操作
        /// </summary>
        /// <typeparam name="T">待返回的值的类型</typeparam>
        /// <param name="value">byte[]数组</param>
        /// <param name="redisDataInfo">Redis数据信息</param>
        /// <returns>T</returns>
        // Token: 0x06000143 RID: 323 RVA: 0x00006CDC File Offset: 0x00004EDC
        private T GetValue<T>(byte[] value, ref RedisDataInfo redisDataInfo)
        {
            T result;
            if (value == null || value.Length == 0)
            {
                result = default(T);
                return result;
            }
            bool flag = typeof(T) == typeof(string);
            bool flag2 = false;
            try
            {
                RedisCompressType redisCompressType = RedisCompressType.None;
                byte[] array = new byte[(value.Length >= 4) ? (value.Length - 4) : value.Length];
                if (value.Length >= 4)
                {
                    byte[] array2 = new byte[4];
                    Buffer.BlockCopy(value, 0, array2, 0, 4);
                    try
                    {
                        redisCompressType = (RedisCompressType)Enum.Parse(typeof(RedisCompressType), Encoding.UTF8.GetString(array2));
                    }
                    catch
                    {
                        redisCompressType = RedisCompressType.None;
                        flag2 = true;
                    }
                    if (flag2)
                    {
                        array = value;
                    }
                    else
                    {
                        Buffer.BlockCopy(value, 4, array, 0, array.Length);
                    }
                }
                else
                {
                    array = value;
                }
                switch (redisCompressType)
                {
                    case RedisCompressType.None:
                    case RedisCompressType.MsgP:
                        redisDataInfo.IsCompress = false;
                        redisDataInfo.Size += (double)array.Length;
                        if (flag)
                        {
                            result = (T)((object)Encoding.UTF8.GetString(array));
                        }
                        else
                        {
                            result = this._serializerUnits.DeserializeFromByteArraye<T>(array);
                        }
                        break;
                    case RedisCompressType.GZip:
                    case RedisCompressType.MZip:
                        {
                            redisDataInfo.IsCompress = true;
                            redisDataInfo.CompressSize += (double)array.Length;
                            byte[] array3 = GZipUtil.Decompress(array);
                            redisDataInfo.Size += (double)array3.Length;
                            if (flag)
                            {
                                result = (T)((object)Encoding.UTF8.GetString(array3));
                            }
                            else
                            {
                                result = this._serializerUnits.DeserializeFromByteArraye<T>(array3);
                            }
                            break;
                        }
                    default:
                        redisDataInfo.IsCompress = false;
                        redisDataInfo.Size += (double)value.Length;
                        if (flag)
                        {
                            result = (T)((object)Encoding.UTF8.GetString(value));
                        }
                        else
                        {
                            result = this._serializerUnits.DeserializeFromByteArraye<T>(value);
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        /// <summary>
        /// 将Value转化为byte数组--可能会进行GZip对Value进行压缩后再返回对应得byte数组
        /// 压缩规则：根据配置是否Gzip压缩 （AppConfigRedis.IsUseGZip ） 以及value的大小是否达到 最低压缩阀值（ AppConfigRedis.UseGZipMinSize）
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="value">value</param>
        /// <returns></returns>
        // Token: 0x06000144 RID: 324 RVA: 0x00006EC4 File Offset: 0x000050C4
        private byte[] ConvertValueToBytes<T>(T value)
        {
            byte[] array = (typeof(T) != typeof(string)) ? this._serializerUnits.SerializeToByteArray<T>(value) : Encoding.UTF8.GetBytes((string)((object)value));
            bool flag = this._serializerUnits is MsgPackSerializer;
            bool flag2 = AppConfigRedis.IsUseGZip == 1 && typeof(T) != typeof(byte[]) && (double)array.Length >= AppConfigRedis.UseGZipMinSize;
            RedisCompressType redisCompressType = flag ? RedisCompressType.MsgP : RedisCompressType.None;
            if (flag2)
            {
                redisCompressType = (flag ? RedisCompressType.MZip : RedisCompressType.GZip);
                array = GZipUtil.Compress(array);
            }
            return Encoding.UTF8.GetBytes(redisCompressType.ToString()).Concat(array).ToArray<byte>();
        }

        /// <summary>
        /// 将传入的key数组 转化为byte[]数组
        /// </summary>
        /// <param name="keys">keys</param>
        /// <returns></returns>
        // Token: 0x06000145 RID: 325 RVA: 0x00006F90 File Offset: 0x00005190
        private byte[][] ConvertToKeysBytes(string[] keys)
        {
            byte[][] array = new byte[keys.Length][];
            for (int i = 0; i < keys.Length; i++)
            {
                string text = keys[i];
                array[i] = ((text != null) ? StringExtensions.ToUtf8Bytes(text) : new byte[0]);
            }
            return array;
        }

        /// <summary>
        /// 获取Redis服务的版本
        /// </summary>
        /// <param name="nRedis">redis本地客户端连接实体</param>
        /// <returns>redis服务版本</returns>
        // Token: 0x06000146 RID: 326 RVA: 0x00006FD0 File Offset: 0x000051D0
        private int AssertServerVersionNumber(RedisNativeClient nRedis)
        {
            string[] array = nRedis.ServerVersion.Split(new char[]
            {
                '.'
            });
            int num = int.Parse(array[0]) * 1000;
            if (array.Length > 1)
            {
                num += int.Parse(array[1]) * 100;
            }
            if (array.Length > 2)
            {
                num += int.Parse(array[2]) * 10;
            }
            if (array.Length > 3)
            {
                num += int.Parse(array[3]);
            }
            return num;
        }

        /// <summary>
        /// 根据传入连接类型获取 ”写“IRedisClient
        /// </summary>
        /// <param name="redisConn"></param>
        /// <returns></returns>
        // Token: 0x06000147 RID: 327 RVA: 0x0000702C File Offset: 0x0000522C
        private IRedisClient ReadWriteClient(RedisConnectionSource redisConn)
        {
            PooledRedisClientManager pooledRedisClientManager = base.GetClientManagers(redisConn);
            IRedisClient result = null;
            try
            {
                result = pooledRedisClientManager.GetClient();
            }
            catch (TimeoutException exception)
            {
                pooledRedisClientManager.Dispose();
                pooledRedisClientManager = null;
                Managers.RedisLog.Error("redis create client error, ErrorFunc:ReadWriteClient", exception);
                pooledRedisClientManager = base.GetClientManagers(redisConn);
                result = pooledRedisClientManager.GetClient();
            }
            return result;
        }

        /// <summary>
        /// 根据传入连接类型获取 ”读“IRedisClient
        /// </summary>
        /// <param name="redisConn"></param>
        /// <returns></returns>
        // Token: 0x06000148 RID: 328 RVA: 0x00007088 File Offset: 0x00005288
        private IRedisClient ReadOnlyClient(RedisConnectionSource redisConn)
        {
            PooledRedisClientManager pooledRedisClientManager = base.GetClientManagers(redisConn);
            IRedisClient result = null;
            try
            {
                result = pooledRedisClientManager.GetReadOnlyClient();
            }
            catch (TimeoutException exception)
            {
                pooledRedisClientManager.Dispose();
                pooledRedisClientManager = null;
                Managers.RedisLog.Error("redis create client error, ErrorFunc:ReadWriteClient", exception);
                pooledRedisClientManager = base.GetClientManagers(redisConn);
                result = pooledRedisClientManager.GetReadOnlyClient();
            }
            return result;
        }

        /// <summary>
        /// 返回根据传入的Redis连接和条件查找对应的KEY列表
        /// 该方法只能在指定程序集内使用
        /// </summary>
        /// <param name="pattern">查询条件，若为空直接返回空的key集合</param>
        /// <param name="redisConnections">redis连接字符串，可多个用","隔开；例：127.0.0.1:6379,127.0.0.1:6389</param>
        /// <returns>keys集合</returns>
        // Token: 0x06000149 RID: 329 RVA: 0x000070E4 File Offset: 0x000052E4
        internal List<string> SearchKeys(string pattern, string redisConnections)
        {
            List<string> list = new List<string>();
            if (string.IsNullOrEmpty(pattern))
            {
                return list;
            }
            if (string.IsNullOrEmpty(redisConnections))
            {
                throw new Exception("redis read error, ErrorFunc:SearchKeys redisConnections连接为空");
            }
            foreach (string text in redisConnections.Split(new char[]
            {
                ','
            }))
            {
                try
                {
                    Tuple<string, string, int> tuple = text.ToHostAndPassword();
                    using (RedisClient redisClient = new RedisClient(tuple.Item2, tuple.Item3, string.IsNullOrWhiteSpace(tuple.Item1) ? null : tuple.Item1, 0L))
                    {
                        list.AddRange(redisClient.SearchKeys(pattern));
                    }
                }
                catch (Exception exception)
                {
                    Managers.RedisLog.Error(string.Format("redis read error, ErrorFunc:SearchKeys[pattern:{0}]  RedisClient:{1}", pattern, text), exception);
                }
            }
            return list.Distinct<string>().ToList<string>();
        }
        /// <summary>
        /// 获取并发锁，直到超时
        /// </summary>
        /// <param name="key">key </param>
        /// <param name="timeOut">获取锁的超时时间</param>
        /// <returns>IDisposable</returns>
        public IDisposable AcquireLock(string key, TimeSpan? timeOut = null)
        {
            //string str = KeyPatternManager.MatchPattern(key);
            IDisposable result;
            try
            {
                using (IRedisClient redisClient = this.ReadWriteClient(this.redisConnection))
                {
                    if (timeOut != null)
                    {
                        result = redisClient.AcquireLock(key, timeOut.Value);
                    }
                    else
                    {
                        result = redisClient.AcquireLock(key);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        /// <summary>
        /// 获取并发锁，并立刻返回获取是否成功
        /// </summary>
        /// <param name="key"></param>
        /// <returns>成功返回True 失败返回False</returns>
        // Token: 0x0600014D RID: 333 RVA: 0x00007310 File Offset: 0x00005510
        public bool AcquireLockImmediate(string key, TimeSpan? timeOut = null)
        {
            //string str = KeyPatternManager.MatchPattern(key);
            bool result;
            try
            {
                using (IRedisClient redisClient = this.ReadWriteClient(this.redisConnection))
                {
                    TimeSpan value = timeOut ?? new TimeSpan(365, 0, 0, 0);
                    string text = (DateTimeExtensions.ToUnixTimeMs(DateTime.UtcNow.Add(value)) + 1L).ToString();
                    if (redisClient.SetValueIfNotExists(key, text))
                    {
                        result = true;
                    }
                    else
                    {
                        string text2 = redisClient.Get<string>(key);
                        long num;
                        if (!long.TryParse(text2, out num))
                        {
                            result = false;
                        }
                        else if (num > DateTimeExtensions.ToUnixTimeMs(DateTime.UtcNow))
                        {
                            result = false;
                        }
                        else
                        {
                            result = (redisClient.GetAndSetValue(key, text) == text2);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return result;
        }

        /// <summary>
        /// 立即释放并发锁
        /// </summary>
        /// <param name="key"></param>
        /// <returns>成功返回True 失败返回False</returns>
        // Token: 0x0600014E RID: 334 RVA: 0x00007460 File Offset: 0x00005660
        public void ReleaseLockImmediate(string key)
        {
            //string str = KeyPatternManager.MatchPattern(key);
            try
            {
                using (IRedisClient redisClient = this.ReadWriteClient(this.redisConnection))
                {
                    redisClient.RemoveEntry(new string[]
                    {
                        key
                    });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 根据传入的key移除对应得数据
        /// </summary>
        /// <param name="keys">待移除数据的key</param>
        /// <returns>移除成功返回true 移除失败返回false </returns>
        // Token: 0x0600014F RID: 335 RVA: 0x0000752C File Offset: 0x0000572C
        public bool RemoveEntry(params string[] keys)
        {
            //string str = KeyPatternManager.MatchPattern(keys[0]);

            bool result;
            try
            {
                using (IRedisClient redisClient = this.ReadWriteClient(this.redisConnection))
                {
                    result = redisClient.RemoveEntry(keys);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        /// <summary>
        /// 设置key的数据在指定的时长后过期
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="expireIn">过期时长</param>
        /// <returns>设置成功返回true 设置失败返回false </returns>
        // Token: 0x06000150 RID: 336 RVA: 0x00007618 File Offset: 0x00005818
        public bool ExpireEntryIn(string key, TimeSpan expireIn)
        {
            //string str = KeyPatternManager.MatchPattern(key);
            bool result;
            try
            {
                using (IRedisClient redisClient = this.ReadWriteClient(this.redisConnection))
                {
                    result = redisClient.ExpireEntryIn(key, expireIn);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        /// <summary>
        /// 设置指定key的数据在指定时刻过期
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="expiresAt">过期时刻</param>
        /// <returns>设置成功返回true 设置失败返回false </returns>
        // Token: 0x06000151 RID: 337 RVA: 0x000076DC File Offset: 0x000058DC
        public bool ExpireEntryAt(string key, DateTime expiresAt)
        {
            //string str = KeyPatternManager.MatchPattern(key);
            //I//transaction //transaction = Cat.New//transaction("Cache.memcached", "System:Set:ExpireEntryAt:" + str);
            //Cat.LogEvent("Cache.memcached.key", "System:ExpireEntryAt", Cat.Success, "key=" + key);
            bool result;
            try
            {
                using (IRedisClient redisClient = this.ReadWriteClient(this.redisConnection))
                {
                    //Cat.LogEvent("Cache.memcached.server", redisClient.Host, "0", null);
                    result = redisClient.ExpireEntryAt(key, expiresAt);
                }
            }
            catch (Exception ex)
            {
                //Cat.LogError(ex);
                //transaction.SetStatus(ex);
                throw;
            }
            finally
            {
                //transaction.Complete();
            }
            return result;
        }

        /// <summary>
        /// 以秒为单位，返回给定key的剩余生存时间
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>时间（秒）</returns>
        // Token: 0x06000152 RID: 338 RVA: 0x000077A0 File Offset: 0x000059A0
        public TimeSpan? GetTimeToLive(string key)
        {
            //string str = KeyPatternManager.MatchPattern(key);
            //I//transaction //transaction = Cat.New//transaction("Cache.memcached", "System:Get:GetTimeToLive:" + str);
            //Cat.LogEvent("Cache.memcached.key", "System:GetTimeToLive", Cat.Success, "key=" + key);
            TimeSpan? timeToLive;
            try
            {
                using (IRedisClient redisClient = this.ReadOnlyClient(this.redisConnection))
                {
                    //Cat.LogEvent("Cache.memcached.server", redisClient.Host, "0", null);
                    timeToLive = redisClient.GetTimeToLive(key);
                }
            }
            catch (Exception ex)
            {
                //Cat.LogError(ex);
                //transaction.SetStatus(ex);
                throw;
            }
            finally
            {
                //transaction.Complete();
            }
            return timeToLive;
        }

        /// <summary>
        /// 返回key所储存的值的类型
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>RedisKeyType</returns>
        // Token: 0x06000153 RID: 339 RVA: 0x00007864 File Offset: 0x00005A64
        public RedisKeyType GetKeyType(string key)
        {
            //string str = KeyPatternManager.MatchPattern(key);
            //I//transaction //transaction = Cat.New//transaction("Cache.memcached", "System:Get:GetKeyType:" + str);
            //Cat.LogEvent("Cache.memcached.key", "System:GetKeyType", Cat.Success, "key=" + key);
            RedisKeyType entryType;
            try
            {
                using (IRedisClient redisClient = this.ReadOnlyClient(this.redisConnection))
                {
                    //Cat.LogEvent("Cache.memcached.server", redisClient.Host, "0", null);
                    entryType = redisClient.GetEntryType(key);
                }
            }
            catch (Exception ex)
            {
                //Cat.LogError(ex);
                //transaction.SetStatus(ex);
                throw;
            }
            finally
            {
                //transaction.Complete();
            }
            return entryType;
        }

        /// <summary>
        /// 检查给定 key 是否存在
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>存在返回true 不存在返回false</returns>
        // Token: 0x06000154 RID: 340 RVA: 0x00007928 File Offset: 0x00005B28
        public bool ContainsKey(string key)
        {
            //string str = KeyPatternManager.MatchPattern(key);
            //I//transaction //transaction = Cat.New//transaction("Cache.memcached", "System:Exists:ContainsKey:" + str);
            //Cat.LogEvent("Cache.memcached.key", "System:ContainsKey", Cat.Success, "key=" + key);
            bool result;
            try
            {
                using (IRedisClient redisClient = this.ReadOnlyClient(this.redisConnection))
                {
                    //Cat.LogEvent("Cache.memcached.server", redisClient.Host, "0", null);
                    result = redisClient.ContainsKey(key);
                }
            }
            catch (Exception ex)
            {
                //Cat.LogError(ex);
                //transaction.SetStatus(ex);
                throw;
            }
            finally
            {
                //transaction.Complete();
            }
            return result;
        }

        /// <summary>
        /// 对key所储存的字符串值，设置或清除指定偏移量上的位(bit)。
        /// 位的设置或清除取决于 value 参数，可以是 0 也可以是 1 。
        /// 当key不存在时，自动生成一个新的字符串值。
        /// 字符串会进行伸展(grown)以确保它可以将 value 保存在指定的偏移量上。当字符串值进行伸展时，空白位置以 0 填充。
        /// offset 参数必须大于或等于 0 ，小于 2^32 (bit 映射被限制在 512 MB 之内)。
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="offset">偏移量 必须大于或等于 0</param>
        /// <param name="value">value</param>
        /// <returns>指定偏移量原来储存的位</returns>
        // Token: 0x06000155 RID: 341 RVA: 0x000079EC File Offset: 0x00005BEC
        public long SetBit(string key, int offset, int value)
        {
            //string str = KeyPatternManager.MatchPattern(key);
            //I//transaction //transaction = Cat.New//transaction("Cache.memcached", "BitMap:Set:SetBit:" + str);
            //Cat.LogEvent("Cache.memcached.key", "BitMap:SetBit", Cat.Success, "key=" + key);
            long result;
            try
            {
                using (IRedisClient redisClient = this.ReadWriteClient(this.redisConnection))
                {
                    //Cat.LogEvent("Cache.memcached.server", redisClient.Host, "0", null);
                    result = ((RedisClient)redisClient).SetBit(key, offset, value);
                }
            }
            catch (Exception ex)
            {
                //Cat.LogError(ex);
                //transaction.SetStatus(ex);
                throw;
            }
            finally
            {
                //transaction.Complete();
            }
            return result;
        }

        /// <summary>
        /// 对key所储存的字符串值，获取指定偏移量上的位(bit)。
        /// 当 offset 比字符串值的长度大，或者key不存在时，返回 0 
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="offset">偏移量</param>
        /// <returns>字符串值指定偏移量上的位(bit)</returns>
        // Token: 0x06000156 RID: 342 RVA: 0x00007AB8 File Offset: 0x00005CB8
        public long GetBit(string key, int offset)
        {
            //string str = KeyPatternManager.MatchPattern(key);
            //I//transaction //transaction = Cat.New//transaction("Cache.memcached", "BitMap:Get:GetBit:" + str);
            //Cat.LogEvent("Cache.memcached.key", "BitMap:GetBit", Cat.Success, "key=" + key);
            long result;
            try
            {
                using (IRedisClient redisClient = this.ReadWriteClient(this.redisConnection))
                {
                    //Cat.LogEvent("Cache.memcached.server", redisClient.Host, "0", null);
                    long bit = ((RedisClient)redisClient).GetBit(key, offset);
                    if (bit == 0L)
                    {
                        //Cat.LogEvent("Cache.memcached", "BitMap:missed", "0", null);
                    }
                    result = bit;
                }
            }
            catch (Exception ex)
            {
                //Cat.LogError(ex);
                //transaction.SetStatus(ex);
                throw;
            }
            finally
            {
                //transaction.Complete();
            }
            return result;
        }

        /// <summary>
        /// 获取key所储存的字符值内被设置为 1 的比特位的数量。
        /// 对一个不存在的key进行 BITCOUNT 操作，结果为 0
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>被设置为 1 的位的数量</returns>
        // Token: 0x06000157 RID: 343 RVA: 0x00007B9C File Offset: 0x00005D9C
        public long BitCount(string key)
        {
            //string str = KeyPatternManager.MatchPattern(key);
            //I//transaction //transaction = Cat.New//transaction("Cache.memcached", "BitMap:Count:BitCount:" + str);
            //Cat.LogEvent("Cache.memcached.key", "BitMap:BitCount", Cat.Success, "key=" + key);
            long result;
            try
            {
                using (IRedisClient redisClient = this.ReadWriteClient(this.redisConnection))
                {
                    //Cat.LogEvent("Cache.memcached.server", redisClient.Host, "0", null);
                    long num = ((RedisClient)redisClient).BitCount(key);
                    if (num == 0L)
                    {
                        //Cat.LogEvent("Cache.memcached", "BitMap:missed", "0", null);
                    }
                    result = num;
                }
            }
            catch (Exception ex)
            {
                //Cat.LogError(ex);
                //transaction.SetStatus(ex);
                throw;
            }
            finally
            {
                //transaction.Complete();
            }
            return result;
        }

        // Token: 0x06000158 RID: 344 RVA: 0x00007C7C File Offset: 0x00005E7C
        public long BitCount(string intoKey, params string[] keys)
        {
            //string str = KeyPatternManager.MatchPattern(keys[0]);
            long result = 0L;
            //I//transaction //transaction = Cat.New//transaction("Cache.memcached", "BitMap:Count:BitCount:" + str);
            //Cat.LogEvent("Cache.memcached.key", "BitMap:BitCount", Cat.Success, "key=" + keys.Aggregate((string a, string s) => a + "," + s));
            long result2;
            try
            {
                using (IRedisClient redisClient = this.ReadOnlyClient(this.redisConnection))
                {
                    //Cat.LogEvent("Cache.memcached.server", redisClient.Host, "0", null);
                    if (string.IsNullOrEmpty(this.bitoporOfLuaBodySha))
                    {
                        this.bitoporOfLuaBodySha = redisClient.LoadLuaScript("redis.call('BITOP','OR', KEYS[1],unpack(ARGV));");
                    }
                    IRedisPipeline redisPipeline = ((RedisClient)redisClient).CreatePipeline();
                    redisPipeline.QueueCommand((IRedisClient p) => p.ExecLuaShaAsList(this.bitoporOfLuaBodySha, new string[]
                    {
                        intoKey
                    }, keys));
                    redisPipeline.QueueCommand((IRedisClient p) => ((RedisNativeClient)p).BitCount(intoKey), delegate (long x)
                    {
                        result = x;
                    });
                    redisPipeline.Flush();
                    if (result == 0L)
                    {
                        //Cat.LogEvent("Cache.memcached", "BitMap:missed", "0", null);
                    }
                    result2 = result;
                }
            }
            catch (Exception ex)
            {
                //Cat.LogError(ex);
                //transaction.SetStatus(ex);
                throw;
            }
            finally
            {
                //transaction.Complete();
            }
            return result2;
        }

        /// <summary>
        /// 为指定的key赋值 value,key不存在则先新建key然后在对key赋值
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        /// <returns>赋值成功返回true 赋值失败返回false </returns>
        // Token: 0x06000159 RID: 345 RVA: 0x00007E10 File Offset: 0x00006010
        public bool ItemSet<T>(string key, T value)
        {
            //string str = KeyPatternManager.MatchPattern(key);
            //I//transaction //transaction = Cat.New//transaction("Cache.memcached", "Item:Set:ItemSet:" + str);
            //Cat.LogEvent("Cache.memcached.key", "Item:ItemSet", Cat.Success, string.Format("key={0}", key));
            bool result;
            try
            {
                byte[] array = this.ConvertValueToBytes<T>(value);
                //transaction.AddData("size", (array != null) ? ((long)array.Length) : 0L);
                using (IRedisClient redisClient = this.ReadWriteClient(this.redisConnection))
                {
                    //Cat.LogEvent("Cache.memcached.server", redisClient.Host, "0", null);
                    ((RedisNativeClient)redisClient).Set(key, array);
                    result = true;
                }
            }
            catch (Exception ex)
            {
                //Cat.LogError(ex);
                //transaction.SetStatus(ex);
                throw;
            }
            finally
            {
                //transaction.Complete();
            }
            return result;
        }

        /// <summary>
        /// 为指定的key赋值 value,key不存在则先新建key然后在对key赋值
        /// 同时必须指定key的过期时间
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        /// <param name="expireIn">过期时间</param>
        /// <returns>赋值成功返回true 赋值失败返回false </returns>
        // Token: 0x0600015A RID: 346 RVA: 0x00007F00 File Offset: 0x00006100
        public bool ItemSet<T>(string key, T value, TimeSpan expireIn)
        {
            //string str = KeyPatternManager.MatchPattern(key);
            //I//transaction //transaction = Cat.New//transaction("Cache.memcached", "Item:Set:ItemSet:" + str);
            //Cat.LogEvent("Cache.memcached.key", "Item:ItemSet", Cat.Success, string.Format("key={0}", key));
            bool result;
            try
            {
                byte[] array = this.ConvertValueToBytes<T>(value);
                //transaction.AddData("size", (array != null) ? ((long)array.Length) : 0L);
                using (IRedisClient redisClient = this.ReadWriteClient(this.redisConnection))
                {
                    //Cat.LogEvent("Cache.memcached.server", redisClient.Host, "0", null);
                    RedisNativeClient redisNativeClient = (RedisNativeClient)redisClient;
                    if (this.AssertServerVersionNumber(redisNativeClient) >= 2610)
                    {
                        if (expireIn.Milliseconds > 0)
                        {
                            redisNativeClient.Set(key, array, 0, (long)expireIn.TotalMilliseconds);
                        }
                        else
                        {
                            redisNativeClient.Set(key, array, (int)expireIn.TotalSeconds, 0L);
                        }
                    }
                    else
                    {
                        redisNativeClient.SetEx(key, (int)expireIn.TotalSeconds, array);
                    }
                    result = true;
                }
            }
            catch (Exception ex)
            {
                //Cat.LogError(ex);
                //transaction.SetStatus(ex);
                throw;
            }
            finally
            {
                //transaction.Complete();
            }
            return result;
        }

        /// <summary>
        /// 批量为 多个key 赋各自对应的值 value；key不存在 则先新建key然后在对key赋值
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="values">待设置的key：value 集合</param>
        // Token: 0x0600015B RID: 347 RVA: 0x00008050 File Offset: 0x00006250
        public void ItemSetAll<T>(IDictionary<string, T> values)
        {
            if (values != null && values.Count <= 0)
            {
                return;
            }
            //string str = KeyPatternManager.MatchPattern(values.Keys.First<string>());
            string[] array = CollectionExtensions.ToArray<string>(values.Keys);
            byte[][] array2 = new byte[values.Count][];
            string[] array3 = new string[values.Count];
            //I//transaction //transaction = Cat.New//transaction("Cache.memcached", "Item:Set:ItemSetAll:" + str);
            string arg = array.Aggregate((string n, string s) => n + "," + s);
            //Cat.LogEvent("Cache.memcached.key", "Item:ItemSetAll", Cat.Success, string.Format("key={0}", arg));
            try
            {
                int num = 0;
                foreach (KeyValuePair<string, T> keyValuePair in values)
                {
                    array2[num] = this.ConvertValueToBytes<T>(keyValuePair.Value);
                    string[] array4 = array3;
                    int num2 = num;
                    byte[] array5 = array2[num];
                    array4[num2] = (((array5 != null) ? ((long)array5.Length).ToString() : null) ?? "0");
                    num++;
                }
                string value = string.Join(",", array3);
                //transaction.AddData("size", value);
                using (IRedisClient redisClient = this.ReadWriteClient(this.redisConnection))
                {
                    //Cat.LogEvent("Cache.memcached.server", redisClient.Host, "0", null);
                    ((RedisNativeClient)redisClient).MSet(array, array2);
                }
            }
            catch (Exception ex)
            {
                //Cat.LogError(ex);
                //transaction.SetStatus(ex);
                throw;
            }
            finally
            {
                //transaction.Complete();
            }
        }

        /// <summary>
        /// 获取key对应的值，key不存在则返回对应值的类型的默认值 -- default(T)
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="key">key</param>
        /// <returns>T</returns>
        // Token: 0x0600015C RID: 348 RVA: 0x00008214 File Offset: 0x00006414
        public T ItemGet<T>(string key)
        {
            //string str = KeyPatternManager.MatchPattern(key);
            //I//transaction //transaction = Cat.New//transaction("Cache.memcached", "Item:Get:ItemGet:" + str);
            //Cat.LogEvent("Cache.memcached.key", "Item:ItemGet", Cat.Success, string.Format("key={0}", key));
            T result;
            try
            {
                using (IRedisClient redisClient = this.ReadOnlyClient(this.redisConnection))
                {
                    //Cat.LogEvent("Cache.memcached.server", redisClient.Host, "0", null);
                    byte[] array = ((RedisNativeClient)redisClient).Get(key);
                    //transaction.AddData("size", (array != null) ? ((long)array.Length) : 0L);
                    if (array == null || array.Length == 0)
                    {
                        //Cat.LogEvent("Cache.memcached", "Item:missed", "0", null);
                    }
                    result = this.GetValue<T>(array);
                }
            }
            catch (Exception ex)
            {
                //Cat.LogError(ex);
                //transaction.SetStatus(ex);
                Managers.RedisLog.Error(string.Format("redis read error, ErrorFunc:ItemGet.   key:{0}", key), ex);
                result = default(T);
            }
            finally
            {
                //transaction.Complete();
            }
            return result;
        }

        /// <summary>
        /// 将给定 key 的值设为 value ，并返回 key 的旧值(old value)
        /// key不存在则返回对应值的类型的默认值 -- default(T)
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="value">新的要设定的value</param>
        /// <returns>T</returns>
        // Token: 0x0600015D RID: 349 RVA: 0x00008340 File Offset: 0x00006540
        public T ItemGetSet<T>(string key, T value)
        {
            //string str = KeyPatternManager.MatchPattern(key);
            //I//transaction //transaction = Cat.New//transaction("Cache.memcached", "Item:GetSet:ItemGetSet:" + str);
            //Cat.LogEvent("Cache.memcached.key", "Item:ItemGetSet", Cat.Success, string.Format("key={0}", key));
            T result;
            try
            {
                byte[] array = this.ConvertValueToBytes<T>(value);
                //transaction.AddData("size", (array != null) ? ((long)array.Length) : 0L);
                using (IRedisClient redisClient = this.ReadOnlyClient(this.redisConnection))
                {
                    //Cat.LogEvent("Cache.memcached.server", redisClient.Host, "0", null);
                    byte[] set = ((RedisNativeClient)redisClient).GetSet(key, array);
                    //transaction.AddData("size", (set != null) ? ((long)set.Length) : 0L);
                    if (set == null || set.Length == 0)
                    {
                        //Cat.LogEvent("Cache.memcached", "Item:missed", "0", null);
                    }
                    result = this.GetValue<T>(set);
                }
            }
            catch (Exception ex)
            {
                //Cat.LogError(ex);
                //transaction.SetStatus(ex);
                Managers.RedisLog.Error(string.Format("redis read error, ErrorFunc:ItemGet.   key:{0}", key), ex);
                result = default(T);
            }
            finally
            {
                //transaction.Complete();
            }
            return result;
        }

        /// <summary>
        /// 将给定 key 的值设为 value ，并返回 key 的旧值(old value)
        /// key不存在则返回对应值的类型的默认值 -- default(T)
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="value">新的要设定的value</param>
        /// <param name="expireIn">过期时间</param>
        /// <returns>T</returns>
        // Token: 0x0600015E RID: 350 RVA: 0x00008494 File Offset: 0x00006694
        public T ItemGetSet<T>(string key, T value, TimeSpan expireIn)
        {
            //string str = KeyPatternManager.MatchPattern(key);
            //I//transaction //transaction = Cat.New//transaction("Cache.memcached", "Item:GetSet:ItemGetSet:" + str);
            //Cat.LogEvent("Cache.memcached.key", "Item:ItemGetSet", Cat.Success, string.Format("key={0}", key));
            T result;
            try
            {
                byte[] array = this.ConvertValueToBytes<T>(value);
                //transaction.AddData("size", (array != null) ? ((long)array.Length) : 0L);
                using (IRedisClient redisClient = this.ReadOnlyClient(this.redisConnection))
                {
                    //Cat.LogEvent("Cache.memcached.server", redisClient.Host, "0", null);
                    byte[] set = ((RedisNativeClient)redisClient).GetSet(key, array);
                    //transaction.AddData("size", (set != null) ? ((long)set.Length) : 0L);
                    if (set == null || set.Length == 0)
                    {
                        //Cat.LogEvent("Cache.memcached", "Item:missed", "0", null);
                    }
                    redisClient.ExpireEntryIn(key, expireIn);
                    result = this.GetValue<T>(set);
                }
            }
            catch (Exception ex)
            {
                //Cat.LogError(ex);
                //transaction.SetStatus(ex);
                Managers.RedisLog.Error(string.Format("redis read error, ErrorFunc:ItemGet.   key:{0}", key), ex);
                result = default(T);
            }
            finally
            {
                //transaction.Complete();
            }
            return result;
        }

        // Token: 0x0600015F RID: 351 RVA: 0x000085F4 File Offset: 0x000067F4
        internal T ItemGet<T>(string key, out RedisDataInfo redisDataInfo)
        {
            //string str = KeyPatternManager.MatchPattern(key);
            //I//transaction //transaction = Cat.New//transaction("Cache.memcached", "Item:Get:ItemGet:" + str);
            //Cat.LogEvent("Cache.memcached.key", "Item:ItemGet", Cat.Success, string.Format("key={0}", key));
            redisDataInfo = new RedisDataInfo
            {
                IsNew = true,
                IsCompress = false,
                CompressSize = 0.0,
                Size = 0.0
            };
            T value;
            try
            {
                using (IRedisClient redisClient = this.ReadOnlyClient(this.redisConnection))
                {
                    //Cat.LogEvent("Cache.memcached.server", redisClient.Host, "0", null);
                    byte[] array = ((RedisNativeClient)redisClient).Get(key);
                    //transaction.AddData("size", (array != null) ? ((long)array.Length) : 0L);
                    if (array == null || array.Length == 0)
                    {
                        //Cat.LogEvent("Cache.memcached", "Item:missed", "0", null);
                    }
                    value = this.GetValue<T>(array, ref redisDataInfo);
                }
            }
            catch (Exception ex)
            {
                //Cat.LogError(ex);
                //transaction.SetStatus(ex);
                Managers.RedisLog.Error(string.Format("redis read error, ErrorFunc:ItemGet.   key:{0}", key), ex);
                throw new Exception(string.Format("redis read error, ErrorFunc:ItemGet.   key:{0}", key), ex);
            }
            finally
            {
                //transaction.Complete();
            }
            return value;
        }

        /// <summary>
        /// 将key中储存的数字值增加增量 increment。
        /// 如果key不存在，那么key的值会先被初始化为 0 ，然后再执行增值操作
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="increment">增加的数值</param>
        /// <returns>加上 increment 之后， key 的值</returns>
        // Token: 0x06000160 RID: 352 RVA: 0x00008758 File Offset: 0x00006958
        public long ItemIncrement(string key, uint increment)
        {
            //string str = KeyPatternManager.MatchPattern(key);
            //I//transaction //transaction = Cat.New//transaction("Cache.memcached", "Item:Set:ItemIncrement:" + str);
            //Cat.LogEvent("Cache.memcached.key", "Item:ItemIncrement", Cat.Success, "key=" + key);
            long result;
            try
            {
                using (IRedisClient redisClient = this.ReadWriteClient(this.redisConnection))
                {
                    //Cat.LogEvent("Cache.memcached.server", redisClient.Host, "0", null);
                    result = redisClient.Increment(key, increment);
                }
            }
            catch (Exception ex)
            {
                //Cat.LogError(ex);
                //transaction.SetStatus(ex);
                throw;
            }
            finally
            {
                //transaction.Complete();
            }
            return result;
        }

        /// <summary>
        /// 将key中储存的数字值增加增量 increment。
        /// 如果key不存在，那么key的值会先被初始化为 0 ，然后再执行增值操作
        /// 同时必须指定key的过期时间
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="increment">增加的数值</param>
        /// <param name="expireIn">过期时间</param>
        /// <returns>加上 increment 之后， key 的值</returns>
        // Token: 0x06000161 RID: 353 RVA: 0x0000881C File Offset: 0x00006A1C
        public long ItemIncrement(string key, uint increment, TimeSpan expireIn)
        {
            //string str = KeyPatternManager.MatchPattern(key);
            //I//transaction //transaction = Cat.New//transaction("Cache.memcached", "Item:Set:ItemIncrement:" + str);
            //Cat.LogEvent("Cache.memcached.key", "Item:ItemIncrement", Cat.Success, "key=" + key);
            long result;
            try
            {
                using (IRedisClient redisClient = this.ReadWriteClient(this.redisConnection))
                {
                    //Cat.LogEvent("Cache.memcached.server", redisClient.Host, "0", null);
                    long num = redisClient.Increment(key, increment);
                    redisClient.ExpireEntryIn(key, expireIn);
                    result = num;
                }
            }
            catch (Exception ex)
            {
                //Cat.LogError(ex);
                //transaction.SetStatus(ex);
                throw;
            }
            finally
            {
                //transaction.Complete();
            }
            return result;
        }

        /// <summary>
        /// 将 key 所储存的值减去减量 decrement 。
        /// 如果 key 不存在，那么 key 的值会先被初始化为 0 ，然后再执行减法操作
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="decrement">减去的数值</param>
        /// <returns>减去 decrement 之后， key 的值</returns>
        // Token: 0x06000162 RID: 354 RVA: 0x000088EC File Offset: 0x00006AEC
        public long ItemDecrement(string key, uint decrement)
        {
            //string str = KeyPatternManager.MatchPattern(key);
            //I//transaction //transaction = Cat.New//transaction("Cache.memcached", "Item:Set:ItemDecrement:" + str);
            //Cat.LogEvent("Cache.memcached.key", "Item:Set", Cat.Success, "key=" + key);
            long result;
            try
            {
                using (IRedisClient redisClient = this.ReadWriteClient(this.redisConnection))
                {
                    //Cat.LogEvent("Cache.memcached.server", redisClient.Host, "0", null);
                    result = redisClient.Decrement(key, decrement);
                }
            }
            catch (Exception ex)
            {
                //Cat.LogError(ex);
                //transaction.SetStatus(ex);
                throw;
            }
            finally
            {
                //transaction.Complete();
            }
            return result;
        }

        /// <summary>
        /// 将 key 所储存的值减去减量 decrement 。
        /// 如果 key 不存在，那么 key 的值会先被初始化为 0 ，然后再执行减法操作
        /// 同时必须指定key的过期时间
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="decrement">减去的数值</param>
        /// <param name="expireIn">过期时间</param>
        /// <returns>减去 decrement 之后， key 的值</returns>
        // Token: 0x06000163 RID: 355 RVA: 0x000089B0 File Offset: 0x00006BB0
        public long ItemDecrement(string key, uint decrement, TimeSpan expireIn)
        {
            //string str = KeyPatternManager.MatchPattern(key);
            //I//transaction //transaction = Cat.New//transaction("Cache.memcached", "Item:Set:ItemDecrement:" + str);
            //Cat.LogEvent("Cache.memcached.key", "Item:Set", Cat.Success, "key=" + key);
            long result;
            try
            {
                using (IRedisClient redisClient = this.ReadWriteClient(this.redisConnection))
                {
                    //Cat.LogEvent("Cache.memcached.server", redisClient.Host, "0", null);
                    long num = redisClient.Decrement(key, decrement);
                    redisClient.ExpireEntryIn(key, expireIn);
                    result = num;
                }
            }
            catch (Exception ex)
            {
                //Cat.LogError(ex);
                //transaction.SetStatus(ex);
                throw;
            }
            finally
            {
                //transaction.Complete();
            }
            return result;
        }

        /// <summary>
        /// 获取多个Key各自对应得值，返回key：value的对应集合
        /// key不存在则对应值的类型的默认值 -- default(T)
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="keys">keys</param>
        /// <returns>IDictionary  key：value的对应集合</returns>
        // Token: 0x06000164 RID: 356 RVA: 0x00008A80 File Offset: 0x00006C80
        public IDictionary<string, T> ItemGetAll<T>(IEnumerable<string> keys)
        {
            if (keys == null || !keys.Any<string>())
            {
                return new Dictionary<string, T>();
            }
            //string str = KeyPatternManager.MatchPattern(keys.First<string>());
            //I//transaction //transaction = Cat.New//transaction("Cache.memcached", "Item:Get:ItemGetAll:" + str);
            string arg = keys.Aggregate((string c, string s) => c + "," + s);
            //Cat.LogEvent("Cache.memcached.key", "Item:ItemGetAll", Cat.Success, string.Format("key={0}", arg));
            IDictionary<string, T> result;
            try
            {
                using (IRedisClient redisClient = this.ReadOnlyClient(this.redisConnection))
                {
                    //Cat.LogEvent("Cache.memcached.server", redisClient.Host, "0", null);
                    IDictionary<string, T> dictionary = new Dictionary<string, T>();
                    string[] array = keys.ToArray<string>();
                    byte[][] array2 = ((RedisNativeClient)redisClient).MGet(array);
                    long num = 0L;
                    for (int i = 0; i < array2.Length; i++)
                    {
                        string key = array[i];
                        T value = this.GetValue<T>(array2[i]);
                        long num2 = num;
                        byte[] array3 = array2[i];
                        num = num2 + ((array3 != null) ? ((long)array3.Length) : 0L);
                        if (value == null)
                        {
                            dictionary[key] = default(T);
                        }
                        else
                        {
                            dictionary.Add(key, value);
                        }
                    }
                    //transaction.AddData("size", num);
                    if (num == 0L)
                    {
                        //Cat.LogEvent("Cache.memcached", "Item:missed", "0", null);
                    }
                    result = dictionary;
                }
            }
            catch (Exception ex)
            {
                //Cat.LogError(ex);
                //transaction.SetStatus(ex);
                Managers.RedisLog.Error(string.Format("redis read error, ErrorFunc:ItemGetAll.   key:{0}", string.Join(",", keys)), ex);
                result = null;
            }
            finally
            {
                //transaction.Complete();
            }
            return result;
        }

        /// <summary>
        /// 移除key对应的数据
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>移除成功返回true 移除失败返回false</returns>
        // Token: 0x06000165 RID: 357 RVA: 0x00008C78 File Offset: 0x00006E78
        public bool ItemRemove(string key)
        {
            //string str = KeyPatternManager.MatchPattern(key);
            //I//transaction //transaction = Cat.New//transaction("Cache.memcached", "Item:Remove:ItemRemove:" + str);
            //Cat.LogEvent("Cache.memcached.key", "Item:ItemRemove", Cat.Success, "key=" + key);
            bool result;
            try
            {
                using (IRedisClient redisClient = this.ReadWriteClient(this.redisConnection))
                {
                    //Cat.LogEvent("Cache.memcached.server", redisClient.Host, "0", null);
                    result = redisClient.Remove(key);
                }
            }
            catch (Exception ex)
            {
                //Cat.LogError(ex);
                //transaction.SetStatus(ex);
                throw;
            }
            finally
            {
                //transaction.Complete();
            }
            return result;
        }

        /// <summary>
        /// 移除多个key各自对应的数据
        /// </summary>
        /// <param name="keys">keys</param>
        // Token: 0x06000166 RID: 358 RVA: 0x00008D3C File Offset: 0x00006F3C
        public void ItemRemoveAll(IEnumerable<string> keys)
        {
            if (keys == null || !keys.Any<string>())
            {
                return;
            }
            //string str = KeyPatternManager.MatchPattern(keys.First<string>());
            //I//transaction //transaction = Cat.New//transaction("Cache.memcached", "Item:Remove:ItemRemoveAll:" + str);
            //Cat.LogEvent("Cache.memcached.key", "Item:ItemRemoveAll", Cat.Success, "key=" + keys.Aggregate((string c, string s) => c + "," + s));
            try
            {
                string[] array = (keys as string[]) ?? keys.ToArray<string>();
                bool flag = this.CanRemoveRangeMaxCount <= 0 || array.Count<string>() <= this.CanRemoveRangeMaxCount;
                using (IRedisClient redisClient = this.ReadWriteClient(this.redisConnection))
                {
                    //Cat.LogEvent("Cache.memcached.server", redisClient.Host, "0", null);
                    if (!flag)
                    {
                        throw new Exception(string.Format("redis read error, ErrorFunc: ItemRemoveAll.keys:{0}, ErrorMsg: can not remove > {1} item at one time ", string.Join(",", array), this.CanRemoveRangeMaxCount));
                    }
                    redisClient.RemoveAll(array);
                }
            }
            catch (Exception ex)
            {
                //Cat.LogError(ex);
                //transaction.SetStatus(ex);
                throw;
            }
            finally
            {
                //transaction.Complete();
            }
        }

        /// <summary>
        /// 检查给定 key的Item 是否存在
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>存在返回true 不存在返回false</returns>
        // Token: 0x06000167 RID: 359 RVA: 0x00008E94 File Offset: 0x00007094
        public bool ItemExist(string key)
        {
            return this.ContainsKey(key);
        }

        /// <summary>
        /// 将一个value 插入到列表 key 的列表头(最左边)
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        // Token: 0x06000168 RID: 360 RVA: 0x00008EA0 File Offset: 0x000070A0
        public void ListAddToHead<T>(string key, T value)
        {
            //string str = KeyPatternManager.MatchPattern(key);
            //I//transaction //transaction = Cat.New//transaction("Cache.memcached", "List:Set:ListAddToHead:" + str);
            //Cat.LogEvent("Cache.memcached.key", "List:ListAddToHead", Cat.Success, string.Format("key={0}", key));
            try
            {
                byte[] array = this.ConvertValueToBytes<T>(value);
                //transaction.AddData("size", (array != null) ? ((long)array.Length) : 0L);
                using (IRedisClient redisClient = this.ReadWriteClient(this.redisConnection))
                {
                    //Cat.LogEvent("Cache.memcached.server", redisClient.Host, "0", null);
                    ((IRedisNativeClient)redisClient).LPush(key, array);
                }
            }
            catch (Exception ex)
            {
                //Cat.LogError(ex);
                //transaction.SetStatus(ex);
                throw;
            }
            finally
            {
                //transaction.Complete();
            }
        }

        /// <summary>
        /// 将一个value 插入到列表 key 的列表头(最左边)
        /// 同时必须指定key的过期时间
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        /// <param name="expireIn">过期时间</param>
        // Token: 0x06000169 RID: 361 RVA: 0x00008F8C File Offset: 0x0000718C
        public void ListAddToHead<T>(string key, T value, TimeSpan expireIn)
        {
            //string str = KeyPatternManager.MatchPattern(key);
            //I//transaction //transaction = Cat.New//transaction("Cache.memcached", "List:Set:ListAddToHead:" + str);
            //Cat.LogEvent("Cache.memcached.key", "List:ListAddToHead", Cat.Success, string.Format("key={0}", key));
            try
            {
                byte[] array = this.ConvertValueToBytes<T>(value);
                //transaction.AddData("size", (array != null) ? ((long)array.Length) : 0L);
                using (IRedisClient redisClient = this.ReadWriteClient(this.redisConnection))
                {
                    //Cat.LogEvent("Cache.memcached.server", redisClient.Host, "0", null);
                    ((IRedisNativeClient)redisClient).LPush(key, array);
                    redisClient.ExpireEntryIn(key, expireIn);
                }
            }
            catch (Exception ex)
            {
                //Cat.LogError(ex);
                //transaction.SetStatus(ex);
                throw;
            }
            finally
            {
                //transaction.Complete();
            }
        }

        /// <summary>
        /// 将多个value 插入到列表 key 的列表头(最左边)
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="values">value的集合</param>
        // Token: 0x0600016A RID: 362 RVA: 0x00009084 File Offset: 0x00007284
        public void ListAddRangeToHead<T>(string key, List<T> values)
        {
            if (values != null && values.Count <= 0)
            {
                return;
            }
            //string str = KeyPatternManager.MatchPattern(key);
            //I//transaction //transaction = Cat.New//transaction("Cache.memcached", "List:Set:ListAddRangeToHead:" + str);
            //Cat.LogEvent("Cache.memcached.key", "List:ListAddRangeToHead", Cat.Success, string.Format("key={0}", key));
            try
            {
                using (IRedisClient redisClient = this.ReadWriteClient(this.redisConnection))
                {
                    //Cat.LogEvent("Cache.memcached.server", redisClient.Host, "0", null);
                    byte[] array = StringExtensions.ToUtf8Bytes(key);
                    string[] array2 = new string[values.Count];
                    int num = 0;
                    RedisPipelineCommand redisPipelineCommand = ((RedisNativeClient)redisClient).CreatePipelineCommand();
                    foreach (T value in values)
                    {
                        byte[] array3 = this.ConvertValueToBytes<T>(value);
                        array2[num++] = (((array3 != null) ? ((long)array3.Length).ToString() : null) ?? "0");
                        redisPipelineCommand.WriteCommand(new byte[][]
                        {
                            Commands.LPush,
                            array,
                            array3
                        });
                    }
                    redisPipelineCommand.Flush();
                    string value2 = string.Join(",", array2);
                    //transaction.AddData("size", value2);
                    redisPipelineCommand.ReadAllAsInts();
                }
            }
            catch (Exception ex)
            {
                //Cat.LogError(ex);
                //transaction.SetStatus(ex);
                throw;
            }
            finally
            {
                //transaction.Complete();
            }
        }

        /// <summary>
        /// 将多个value 插入到列表 key 的列表头(最左边)
        /// 同时必须指定key的过期时间
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="values">value的集合</param>
        /// <param name="expireIn">过期时间</param>
        // Token: 0x0600016B RID: 363 RVA: 0x00009254 File Offset: 0x00007454
        public void ListAddRangeToHead<T>(string key, List<T> values, TimeSpan expireIn)
        {
            if (values != null && values.Count <= 0)
            {
                return;
            }
            //string str = KeyPatternManager.MatchPattern(key);
            //I//transaction //transaction = Cat.New//transaction("Cache.memcached", "List:Set:ListAddRangeToHead:" + str);
            //Cat.LogEvent("Cache.memcached.key", "List:ListAddRangeToHead", Cat.Success, string.Format("key={0}", key));
            try
            {
                using (IRedisClient redisClient = this.ReadWriteClient(this.redisConnection))
                {
                    //Cat.LogEvent("Cache.memcached.server", redisClient.Host, "0", null);
                    byte[] array = StringExtensions.ToUtf8Bytes(key);
                    string[] array2 = new string[values.Count];
                    int num = 0;
                    RedisPipelineCommand redisPipelineCommand = ((RedisNativeClient)redisClient).CreatePipelineCommand();
                    foreach (T value in values)
                    {
                        byte[] array3 = this.ConvertValueToBytes<T>(value);
                        array2[num++] = (((array3 != null) ? ((long)array3.Length).ToString() : null) ?? "0");
                        redisPipelineCommand.WriteCommand(new byte[][]
                        {
                            Commands.LPush,
                            array,
                            array3
                        });
                    }
                    redisPipelineCommand.Flush();
                    string value2 = string.Join(",", array2);
                    //transaction.AddData("size", value2);
                    redisPipelineCommand.ReadAllAsInts();
                    redisClient.ExpireEntryIn(key, expireIn);
                }
            }
            catch (Exception ex)
            {
                //Cat.LogError(ex);
                //transaction.SetStatus(ex);
                throw;
            }
            finally
            {
                //transaction.Complete();
            }
        }

        /// <summary>
        /// 将一个value 插入到列表 key 的列表尾(最右边)
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        // Token: 0x0600016C RID: 364 RVA: 0x0000942C File Offset: 0x0000762C
        public void ListAddToEnd<T>(string key, T value)
        {
            //string str = KeyPatternManager.MatchPattern(key);
            //I//transaction //transaction = Cat.New//transaction("Cache.memcached", "List:Set:ListAddToEnd:" + str);
            //Cat.LogEvent("Cache.memcached.key", "List:ListAddToEnd", Cat.Success, string.Format("key={0}", key));
            try
            {
                using (IRedisClient redisClient = this.ReadWriteClient(this.redisConnection))
                {
                    //Cat.LogEvent("Cache.memcached.server", redisClient.Host, "0", null);
                    byte[] array = this.ConvertValueToBytes<T>(value);
                    //transaction.AddData("size", (array != null) ? ((long)array.Length) : 0L);
                    ((IRedisNativeClient)redisClient).RPush(key, array);
                }
            }
            catch (Exception ex)
            {
                //Cat.LogError(ex);
                //transaction.SetStatus(ex);
                throw;
            }
            finally
            {
                //transaction.Complete();
            }
        }

        /// <summary>
        /// 将一个value 插入到列表 key 的列表尾(最右边)
        /// 同时必须指定key的过期时间
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        /// <param name="expireIn">过期时间</param>
        // Token: 0x0600016D RID: 365 RVA: 0x00009518 File Offset: 0x00007718
        public void ListAddToEnd<T>(string key, T value, TimeSpan expireIn)
        {
            //string str = KeyPatternManager.MatchPattern(key);
            //I//transaction //transaction = Cat.New//transaction("Cache.memcached", "List:Set:ListAddToEnd:" + str);
            //Cat.LogEvent("Cache.memcached.key", "List:Set", Cat.Success, string.Format("key={0}", key));
            try
            {
                using (IRedisClient redisClient = this.ReadWriteClient(this.redisConnection))
                {
                    //Cat.LogEvent("Cache.memcached.server", redisClient.Host, "0", null);
                    byte[] array = this.ConvertValueToBytes<T>(value);
                    //transaction.AddData("size", (array != null) ? ((long)array.Length) : 0L);
                    ((IRedisNativeClient)redisClient).RPush(key, array);
                    redisClient.ExpireEntryIn(key, expireIn);
                }
            }
            catch (Exception ex)
            {
                //Cat.LogError(ex);
                //transaction.SetStatus(ex);
                throw;
            }
            finally
            {
                //transaction.Complete();
            }
        }

        /// <summary>
        /// 将多个value 插入到列表 key 的列表尾(最右边)
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="values">value的集合</param>
        // Token: 0x0600016E RID: 366 RVA: 0x00009610 File Offset: 0x00007810
        public void ListAddRangeToEnd<T>(string key, List<T> values)
        {
            if (values != null && values.Count <= 0)
            {
                return;
            }
            //string str = KeyPatternManager.MatchPattern(key);
            //I//transaction //transaction = Cat.New//transaction("Cache.memcached", "List:Set:ListAddRangeToEnd:" + str);
            //Cat.LogEvent("Cache.memcached.key", "List:ListAddRangeToEnd", Cat.Success, string.Format("key={0}", key));
            try
            {
                using (IRedisClient redisClient = this.ReadWriteClient(this.redisConnection))
                {
                    //Cat.LogEvent("Cache.memcached.server", redisClient.Host, "0", null);
                    byte[] array = StringExtensions.ToUtf8Bytes(key);
                    string[] array2 = new string[values.Count];
                    int num = 0;
                    RedisPipelineCommand redisPipelineCommand = ((RedisNativeClient)redisClient).CreatePipelineCommand();
                    foreach (T value in values)
                    {
                        byte[] array3 = this.ConvertValueToBytes<T>(value);
                        array2[num++] = (((array3 != null) ? ((long)array3.Length).ToString() : null) ?? "0");
                        redisPipelineCommand.WriteCommand(new byte[][]
                        {
                            Commands.RPush,
                            array,
                            array3
                        });
                    }
                    redisPipelineCommand.Flush();
                    string value2 = string.Join(",", array2);
                    //transaction.AddData("size", value2);
                    redisPipelineCommand.ReadAllAsInts();
                }
            }
            catch (Exception ex)
            {
                //Cat.LogError(ex);
                //transaction.SetStatus(ex);
                throw;
            }
            finally
            {
                //transaction.Complete();
            }
        }

        /// <summary>
        /// 将多个value 插入到列表 key 的列表尾(最右边)
        /// 同时必须指定key的过期时间
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="values">value的集合</param>
        /// <param name="expireIn">过期时间</param>
        // Token: 0x0600016F RID: 367 RVA: 0x000097E0 File Offset: 0x000079E0
        public void ListAddRangeToEnd<T>(string key, List<T> values, TimeSpan expireIn)
        {
            if (values != null && values.Count <= 0)
            {
                return;
            }
            //string str = KeyPatternManager.MatchPattern(key);
            //I//transaction //transaction = Cat.New//transaction("Cache.memcached", "List:Set:ListAddRangeToEnd:" + str);
            //Cat.LogEvent("Cache.memcached.key", "List:ListAddRangeToEnd", Cat.Success, string.Format("key={0}", key));
            try
            {
                using (IRedisClient redisClient = this.ReadWriteClient(this.redisConnection))
                {
                    //Cat.LogEvent("Cache.memcached.server", redisClient.Host, "0", null);
                    byte[] array = StringExtensions.ToUtf8Bytes(key);
                    string[] array2 = new string[values.Count];
                    int num = 0;
                    RedisPipelineCommand redisPipelineCommand = ((RedisNativeClient)redisClient).CreatePipelineCommand();
                    foreach (T value in values)
                    {
                        byte[] array3 = this.ConvertValueToBytes<T>(value);
                        array2[num++] = (((array3 != null) ? ((long)array3.Length).ToString() : null) ?? "0");
                        redisPipelineCommand.WriteCommand(new byte[][]
                        {
                            Commands.RPush,
                            array,
                            array3
                        });
                    }
                    redisPipelineCommand.Flush();
                    string value2 = string.Join(",", array2);
                    //transaction.AddData("size", value2);
                    redisPipelineCommand.ReadAllAsInts();
                    redisClient.ExpireEntryIn(key, expireIn);
                }
            }
            catch (Exception ex)
            {
                //Cat.LogError(ex);
                //transaction.SetStatus(ex);
                throw;
            }
            finally
            {
                //transaction.Complete();
            }
        }

        /// <summary>
        /// 移除并返回列表 key 的列表头元素（最左边）
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="key">key</param>
        /// <returns>T</returns>
        // Token: 0x06000170 RID: 368 RVA: 0x000099B8 File Offset: 0x00007BB8
        public T ListRemoveHead<T>(string key)
        {
            //string str = KeyPatternManager.MatchPattern(key);
            //I//transaction //transaction = Cat.New//transaction("Cache.memcached", "List:Remove:ListRemoveHead:" + str);
            //Cat.LogEvent("Cache.memcached.key", "List:ListRemoveHead", Cat.Success, "key=" + key);
            T result;
            try
            {
                using (IRedisClient redisClient = this.ReadWriteClient(this.redisConnection))
                {
                    //Cat.LogEvent("Cache.memcached.server", redisClient.Host, "0", null);
                    byte[] value = ((IRedisNativeClient)redisClient).LPop(key);
                    result = this.GetValue<T>(value);
                }
            }
            catch (Exception ex)
            {
                //Cat.LogError(ex);
                //transaction.SetStatus(ex);
                Managers.RedisLog.Error(string.Format("redis read error, ErrorFunc:ListRemoveHead.   key:{0}", key), ex);
                result = default(T);
            }
            finally
            {
                //transaction.Complete();
            }
            return result;
        }

        /// <summary>
        /// 移除并返回列表 key 的列表尾元素（最右边）
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="key">key</param>
        /// <returns>T</returns>
        // Token: 0x06000171 RID: 369 RVA: 0x00009AB0 File Offset: 0x00007CB0
        public T ListRemoveEnd<T>(string key)
        {
            //string str = KeyPatternManager.MatchPattern(key);
            //I//transaction //transaction = Cat.New//transaction("Cache.memcached", "List:Remove:ListRemoveEnd:" + str);
            //Cat.LogEvent("Cache.memcached.key", "List:ListRemoveEnd", Cat.Success, "key=" + key);
            T result;
            try
            {
                using (IRedisClient redisClient = this.ReadWriteClient(this.redisConnection))
                {
                    //Cat.LogEvent("Cache.memcached.server", redisClient.Host, "0", null);
                    byte[] value = ((IRedisNativeClient)redisClient).RPop(key);
                    result = this.GetValue<T>(value);
                }
            }
            catch (Exception ex)
            {
                //Cat.LogError(ex);
                //transaction.SetStatus(ex);
                Managers.RedisLog.Error(string.Format("redis read error, ErrorFunc:ListRemoveEnd.   key:{0}", key), ex);
                result = default(T);
            }
            finally
            {
                //transaction.Complete();
            }
            return result;
        }

        /// <summary>
        /// 移除列表 key中与 value 相等的元素
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        /// <returns>移除成功返回true 移除失败返回false</returns>
        // Token: 0x06000172 RID: 370 RVA: 0x00009BA8 File Offset: 0x00007DA8
        public bool ListRemoveItem<T>(string key, T value)
        {
            //string str = KeyPatternManager.MatchPattern(key);
            //I//transaction //transaction = Cat.New//transaction("Cache.memcached", "List:Remove:ListRemoveItem:" + str);
            //Cat.LogEvent("Cache.memcached.key", "List:ListRemoveItem", Cat.Success, "key=" + key);
            bool result;
            try
            {
                using (IRedisClient redisClient = this.ReadWriteClient(this.redisConnection))
                {
                    //Cat.LogEvent("Cache.memcached.server", redisClient.Host, "0", null);
                    result = (((IRedisNativeClient)redisClient).LRem(key, 0, this.ConvertValueToBytes<T>(value)) > 0L);
                }
            }
            catch (Exception ex)
            {
                //Cat.LogError(ex);
                //transaction.SetStatus(ex);
                throw;
            }
            finally
            {
                //transaction.Complete();
            }
            return result;
        }

        /// <summary>
        /// 移除列表 key内的所有元素
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="key">key</param>
        // Token: 0x06000173 RID: 371 RVA: 0x00009C7C File Offset: 0x00007E7C
        public void ListRemoveAll<T>(string key)
        {
            //string str = KeyPatternManager.MatchPattern(key);
            //I//transaction //transaction = Cat.New//transaction("Cache.memcached", "List:Remove:ListRemoveAll:" + str);
            //Cat.LogEvent("Cache.memcached.key", "List:ListRemoveAll", Cat.Success, "key=" + key);
            try
            {
                using (IRedisClient redisClient = this.ReadWriteClient(this.redisConnection))
                {
                    //Cat.LogEvent("Cache.memcached.server", redisClient.Host, "0", null);
                    IRedisTypedClient<T> redisTypedClient = redisClient.As<T>();
                    redisTypedClient.RemoveAllFromList(redisTypedClient.Lists[key]);
                }
            }
            catch (Exception ex)
            {
                //Cat.LogError(ex);
                //transaction.SetStatus(ex);
                throw;
            }
            finally
            {
                //transaction.Complete();
            }
        }

        /// <summary>
        /// 移除列表key中 除了从 keepStartingFrom 到 keepEndingAt的元素之外的其他所有元素
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="keepStartingFrom"></param>
        /// <param name="keepEndingAt"></param>
        // Token: 0x06000174 RID: 372 RVA: 0x00009D4C File Offset: 0x00007F4C
        public void ListRemoveTrim(string key, int keepStartingFrom, int keepEndingAt)
        {
            //string str = KeyPatternManager.MatchPattern(key);
            //I//transaction //transaction = Cat.New//transaction("Cache.memcached", "List:Remove:ListRemoveTrim:" + str);
            //Cat.LogEvent("Cache.memcached.key", "List:ListRemoveTrim", Cat.Success, "key=" + key);
            try
            {
                using (IRedisClient redisClient = this.ReadWriteClient(this.redisConnection))
                {
                    //Cat.LogEvent("Cache.memcached.server", redisClient.Host, "0", null);
                    redisClient.TrimList(key, keepStartingFrom, keepEndingAt);
                }
            }
            catch (Exception ex)
            {
                //Cat.LogError(ex);
                //transaction.SetStatus(ex);
                throw;
            }
            finally
            {
                //transaction.Complete();
            }
        }

        /// <summary>
        /// 根据传入列表的key移除整个列表
        /// </summary>
        /// <param name="keys">待移除列表的key</param>
        /// <returns>移除成功返回true 移除失败返回false </returns>
        // Token: 0x06000175 RID: 373 RVA: 0x00009E10 File Offset: 0x00008010
        public bool ListRemove(string key)
        {
            return this.RemoveEntry(new string[]
            {
                key
            });
        }

        /// <summary>
        /// 获取列表key内的所有元素
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="key">key</param>
        /// <returns>T的实体集合</returns>
        // Token: 0x06000176 RID: 374 RVA: 0x00009E24 File Offset: 0x00008024
        public List<T> ListGetAll<T>(string key)
        {
            //string str = KeyPatternManager.MatchPattern(key);
            //I//transaction //transaction = Cat.New//transaction("Cache.memcached", "List:Get:ListGetAll:" + str);
            //Cat.LogEvent("Cache.memcached.key", "List:ListGetAll", Cat.Success, string.Format("key={0}", key));
            List<T> result;
            try
            {
                using (IRedisClient redisClient = this.ReadOnlyClient(this.redisConnection))
                {
                    //Cat.LogEvent("Cache.memcached.server", redisClient.Host, "0", null);
                    byte[][] array = ((IRedisNativeClient)redisClient).LRange(key, 0, -1);
                    List<T> list = new List<T>();
                    long num = 0L;
                    if (array != null && array.Length != 0)
                    {
                        foreach (byte[] array3 in array)
                        {
                            num += ((array3 != null) ? ((long)array3.Length) : 0L);
                            list.Add(this.GetValue<T>(array3));
                        }
                    }
                    //transaction.AddData("size", num);
                    if (num == 0L)
                    {
                        //Cat.LogEvent("Cache.memcached", "List:missed", "0", null);
                    }
                    result = list;
                }
            }
            catch (Exception ex)
            {
                //Cat.LogError(ex);
                //transaction.SetStatus(ex);
                Managers.RedisLog.Error(string.Format("redis read error, ErrorFunc:ListGetAll.   key:{0}", key), ex);
                result = null;
            }
            finally
            {
                //transaction.Complete();
            }
            return result;
        }

        // Token: 0x06000177 RID: 375 RVA: 0x00009F88 File Offset: 0x00008188
        internal List<T> ListGetAll<T>(string key, out RedisDataInfo redisDataInfo)
        {
            redisDataInfo = new RedisDataInfo
            {
                IsNew = true,
                IsCompress = false,
                CompressSize = 0.0,
                Size = 0.0
            };
            //string str = KeyPatternManager.MatchPattern(key);
            //I//transaction //transaction = Cat.New//transaction("Cache.memcached", "List:Get:ListGetAll:" + str);
            //Cat.LogEvent("Cache.memcached.key", "List:ListGetAll", Cat.Success, string.Format("key={0}", key));
            List<T> result;
            try
            {
                using (IRedisClient redisClient = this.ReadOnlyClient(this.redisConnection))
                {
                    //Cat.LogEvent("Cache.memcached.server", redisClient.Host, "0", null);
                    byte[][] array = ((IRedisNativeClient)redisClient).LRange(key, 0, -1);
                    List<T> list = new List<T>();
                    long num = 0L;
                    if (array != null && array.Length != 0)
                    {
                        foreach (byte[] array3 in array)
                        {
                            list.Add(this.GetValue<T>(array3, ref redisDataInfo));
                            num += ((array3 != null) ? ((long)array3.Length) : 0L);
                        }
                    }
                    //transaction.AddData("size", num);
                    if (num == 0L)
                    {
                        //Cat.LogEvent("Cache.memcached", "List:missed", "0", null);
                    }
                    result = list;
                }
            }
            catch (Exception ex)
            {
                //Cat.LogError(ex);
                //transaction.SetStatus(ex);
                Managers.RedisLog.Error(string.Format("redis read error, ErrorFunc:ListGetAll.   key:{0}", key), ex);
                throw new Exception(string.Format("redis read error, ErrorFunc:ListGetAll.   key:{0}", key), ex);
            }
            finally
            {
                //transaction.Complete();
            }
            return result;
        }

        /// <summary>
        /// 获取列表key中从第start个元素起长度为count的元素集合
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="start">待获取元素的起始索引值</param>
        /// <param name="count">待获取元素的个数</param>
        /// <returns>T的实体集合</returns>
        // Token: 0x06000178 RID: 376 RVA: 0x0000A130 File Offset: 0x00008330
        public List<T> ListGetRange<T>(string key, int start, int count)
        {
            //string str = KeyPatternManager.MatchPattern(key);
            //I//transaction //transaction = Cat.New//transaction("Cache.memcached", "List:Get:ListGetRange:" + str);
            //Cat.LogEvent("Cache.memcached.key", "List:ListGetRange", Cat.Success, string.Format("key={0}", key));
            List<T> result;
            try
            {
                if (count < 1)
                {
                    result = new List<T>();
                }
                else
                {
                    int num = start + count - 1;
                    using (IRedisClient redisClient = this.ReadOnlyClient(this.redisConnection))
                    {
                        //Cat.LogEvent("Cache.memcached.server", redisClient.Host, "0", null);
                        byte[][] array = ((IRedisNativeClient)redisClient).LRange(key, start, num);
                        List<T> list = new List<T>();
                        long num2 = 0L;
                        if (array != null && array.Length != 0)
                        {
                            foreach (byte[] array3 in array)
                            {
                                num2 += ((array3 != null) ? ((long)array3.Length) : 0L);
                                list.Add(this.GetValue<T>(array3));
                            }
                        }
                        //transaction.AddData("size", num2);
                        if (num2 == 0L)
                        {
                            //Cat.LogEvent("Cache.memcached", "List:missed", "0", null);
                        }
                        result = list;
                    }
                }
            }
            catch (Exception ex)
            {
                //Cat.LogError(ex);
                //transaction.SetStatus(ex);
                Managers.RedisLog.Error(string.Format("redis read error, ErrorFunc:ListGetRange.   key:{0}  start:{1}  count:{2}", key, start, count), ex);
                result = null;
            }
            finally
            {
                //transaction.Complete();
            }
            return result;
        }

        /// <summary>
        /// 获取排序列表（默认升序）key中从第start个元素起长度为count的元素集合
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="start">待获取元素的起始索引值</param>
        /// <param name="count">待获取元素的个数</param>
        /// <param name="isAlpha">是否按照字典顺序排列非数字元素(默认false)</param>
        /// <param name="isDesc">是否降序排序(默认false)</param>
        /// <returns>T的实体集合</returns>
        // Token: 0x06000179 RID: 377 RVA: 0x0000A2E0 File Offset: 0x000084E0
        public List<T> ListGetRangeFromSortedList<T>(string key, int start, int count, bool isAlpha = false, bool isDesc = false)
        {
            //string str = KeyPatternManager.MatchPattern(key);
            //I//transaction //transaction = Cat.New//transaction("Cache.memcached", "List:Get:ListGetRangeFromSortedList:" + str);
            //Cat.LogEvent("Cache.memcached.key", "List:ListGetRangeFromSortedList", Cat.Success, string.Format("key={0}", key));
            List<T> result;
            try
            {
                if (count < 1)
                {
                    result = new List<T>();
                }
                else
                {
                    using (IRedisClient redisClient = this.ReadOnlyClient(this.redisConnection))
                    {
                        //Cat.LogEvent("Cache.memcached.server", redisClient.Host, "0", null);
                        SortOptions sortOptions = new SortOptions
                        {
                            Skip = new int?(start),
                            Take = new int?(count),
                            SortDesc = isDesc,
                            SortAlpha = isAlpha
                        };
                        byte[][] array = ((IRedisNativeClient)redisClient).Sort(key, sortOptions);
                        List<T> list = new List<T>();
                        long num = 0L;
                        if (array != null && array.Length != 0)
                        {
                            foreach (byte[] array3 in array)
                            {
                                num += ((array3 != null) ? ((long)array3.Length) : 0L);
                                list.Add(this.GetValue<T>(array3));
                            }
                        }
                        //transaction.AddData("size", num);
                        if (num == 0L)
                        {
                            //Cat.LogEvent("Cache.memcached", "List:missed", "0", null);
                        }
                        result = list;
                    }
                }
            }
            catch (Exception ex)
            {
                //Cat.LogError(ex);
                //transaction.SetStatus(ex);
                Managers.RedisLog.Error(string.Format("redis read error, ErrorFunc:ListGetRangeFromSortedList.   key:{0}  start:{1}  count:{2}  isAlpha:{3}  isDesc:{4}", new object[]
                {
                    key,
                    start,
                    count,
                    isAlpha,
                    isDesc
                }), ex);
                result = null;
            }
            finally
            {
                //transaction.Complete();
            }
            return result;
        }

        /// <summary>
        /// 获取列表Key的元素个数
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>元素个数</returns>
        // Token: 0x0600017A RID: 378 RVA: 0x0000A4D8 File Offset: 0x000086D8
        public long ListCount(string key)
        {
            //string str = KeyPatternManager.MatchPattern(key);
            //I//transaction //transaction = Cat.New//transaction("Cache.memcached", "List:Count:ListCount:" + str);
            //Cat.LogEvent("Cache.memcached.key", "List:ListCount", Cat.Success, "key=" + key);
            long listCount;
            try
            {
                using (IRedisClient redisClient = this.ReadOnlyClient(this.redisConnection))
                {
                    //Cat.LogEvent("Cache.memcached.server", redisClient.Host, "0", null);
                    listCount = redisClient.GetListCount(key);
                }
            }
            catch (Exception ex)
            {
                //Cat.LogError(ex);
                //transaction.SetStatus(ex);
                throw;
            }
            finally
            {
                //transaction.Complete();
            }
            return listCount;
        }

        /// <summary>
        /// 检查给定 key的列表 是否存在
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>存在返回true 不存在返回false</returns>
        // Token: 0x0600017B RID: 379 RVA: 0x00008E94 File Offset: 0x00007094
        public bool ListExist(string key)
        {
            return this.ContainsKey(key);
        }

        /// <summary>
        /// 将列表 key 下标为 index 的元素的值设置为 value 
        /// 当 index超出列表的范围 或者列表为空 则报错
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">key</param>
        /// <param name="index">要替换的元素下标</param>
        /// <param name="value">要替换的值</param>
        // Token: 0x0600017C RID: 380 RVA: 0x0000A59C File Offset: 0x0000879C
        public void SetItemInList<T>(string key, int index, T value)
        {
            //string str = KeyPatternManager.MatchPattern(key);
            //I//transaction //transaction = Cat.New//transaction("Cache.memcached", "List:Set:SetItemInList:" + str);
            //Cat.LogEvent("Cache.memcached.key", "List:Set", Cat.Success, string.Format("key={0}", key));
            try
            {
                using (IRedisClient redisClient = this.ReadWriteClient(this.redisConnection))
                {
                    //Cat.LogEvent("Cache.memcached.server", redisClient.Host, "0", null);
                    byte[] array = this.ConvertValueToBytes<T>(value);
                    //transaction.AddData("size", (array != null) ? ((long)array.Length) : 0L);
                    ((IRedisNativeClient)redisClient).LSet(key, index, array);
                }
            }
            catch (Exception ex)
            {
                //Cat.LogError(ex);
                //transaction.SetStatus(ex);
                throw;
            }
            finally
            {
                //transaction.Complete();
            }
        }

        /// <summary>
        /// 检查给定 key的哈希表 是否存在
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>存在返回true 不存在返回false</returns>
        // Token: 0x0600017D RID: 381 RVA: 0x00008E94 File Offset: 0x00007094
        public bool HashExist(string key)
        {
            return this.ContainsKey(key);
        }

        /// <summary>
        /// 查看哈希表 key 中，给定域 dataKey 是否存在
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dataKey">dataKey</param>
        /// <returns>存在返回true 不存在返回false</returns>
        // Token: 0x0600017E RID: 382 RVA: 0x0000A688 File Offset: 0x00008888
        public bool HashExistField(string key, string dataKey)
        {
            //string str = KeyPatternManager.MatchPattern(key);
            //I//transaction //transaction = Cat.New//transaction("Cache.memcached", "HashSet:Exists:HashExistField:" + str);
            //Cat.LogEvent("Cache.memcached.key", "HashSet:HashExistField", Cat.Success, string.Format("key={0}&dKey={1}", key, dataKey));
            bool result;
            try
            {
                using (IRedisClient redisClient = this.ReadOnlyClient(this.redisConnection))
                {
                    //Cat.LogEvent("Cache.memcached.server", redisClient.Host, "0", null);
                    result = redisClient.HashContainsEntry(key, dataKey);
                }
            }
            catch (Exception ex)
            {
                //Cat.LogError(ex);
                //transaction.SetStatus(ex);
                throw;
            }
            finally
            {
                //transaction.Complete();
            }
            return result;
        }

        /// <summary>
        /// 将哈希表 key 中的域 dataKey 的值设为 value
        /// 如果 key 不存在，一个新的哈希表被创建并进行 HashSet 操作
        /// 如果域 dataKey 已经存在于哈希表中，旧值将被覆盖
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="dataKey">dataKey</param>
        /// <param name="value">value</param>
        /// <returns>设置成功返回true 设置失败返回false</returns>
        // Token: 0x0600017F RID: 383 RVA: 0x0000A750 File Offset: 0x00008950
        public bool HashSet<T>(string key, string dataKey, T value)
        {
            //string str = KeyPatternManager.MatchPattern(key);
            //I//transaction //transaction = Cat.New//transaction("Cache.memcached", "HashSet:Set:HashSet:" + str);
            //Cat.LogEvent("Cache.memcached.key", "HashSet:HashSet", Cat.Success, string.Format("key={0}&dKey={1}", key, dataKey));
            bool result;
            try
            {
                byte[] array = this.ConvertValueToBytes<T>(value);
                //transaction.AddData("size", (array != null) ? ((long)array.Length) : 0L);
                using (IRedisClient redisClient = this.ReadWriteClient(this.redisConnection))
                {
                    //Cat.LogEvent("Cache.memcached.server", redisClient.Host, "0", null);
                    result = (((RedisNativeClient)redisClient).HSet(key, StringExtensions.ToUtf8Bytes(dataKey), array) == 1L);
                }
            }
            catch (Exception ex)
            {
                //Cat.LogError(ex);
                //transaction.SetStatus(ex);
                //throw;
                result = false;
            }
            finally
            {
                //transaction.Complete();
            }
            return result;
        }

        /// <summary>
        /// 将哈希表 key 中的域 dataKey 的值设为 value
        /// 如果 key 不存在，一个新的哈希表被创建并进行 HashSet 操作
        /// 如果域 dataKey 已经存在于哈希表中，旧值将被覆盖
        /// 同时必须指定key的过期时间
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="dataKey">dataKey</param>
        /// <param name="value">value</param>
        /// <param name="expireIn">过期时间</param>
        /// <returns>设置成功返回true 设置失败返回false</returns>
        // Token: 0x06000180 RID: 384 RVA: 0x0000A848 File Offset: 0x00008A48
        public bool HashSet<T>(string key, string dataKey, T value, TimeSpan expireIn)
        {
            //string str = KeyPatternManager.MatchPattern(key);
            //I//transaction //transaction = Cat.New//transaction("Cache.memcached", "HashSet:Set:HashSet:" + str);
            //Cat.LogEvent("Cache.memcached.key", "HashSet:HashSet", Cat.Success, string.Format("key={0}&dKey={1}", key, dataKey));
            bool result;
            try
            {
                byte[] array = this.ConvertValueToBytes<T>(value);
                //transaction.AddData("size", (array != null) ? ((long)array.Length) : 0L);
                using (IRedisClient redisClient = this.ReadWriteClient(this.redisConnection))
                {
                    //Cat.LogEvent("Cache.memcached.server", redisClient.Host, "0", null);
                    bool flag = ((RedisNativeClient)redisClient).HSet(key, StringExtensions.ToUtf8Bytes(dataKey), array) == 1L;
                    if (flag)
                    {
                        redisClient.ExpireEntryIn(key, expireIn);
                    }
                    result = flag;
                }
            }
            catch (Exception ex)
            {
                //Cat.LogError(ex);
                //transaction.SetStatus(ex);
                throw;
            }
            finally
            {
                //transaction.Complete();
            }
            return result;
        }

        /// <summary>
        /// 同时将多个 datakey-value (域-值)对设置到哈希表 key 中
        /// 如果 key 不存在，一个新的哈希表被创建并进行 HashSetRange 操作
        /// 如果域 dataKey 已经存在于哈希表中，旧值将被覆盖
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="keyValuePairs">dataKey：value的集合</param>
        // Token: 0x06000181 RID: 385 RVA: 0x0000A950 File Offset: 0x00008B50
        public void HashSetRange<T>(string key, IEnumerable<KeyValuePair<string, T>> keyValuePairs)
        {
            //string str = KeyPatternManager.MatchPattern(key);
            //I//transaction //transaction = Cat.New//transaction("Cache.memcached", "HashSet:Set:HashSetRange:" + str);
            //Cat.LogEvent("Cache.memcached.key", "HashSet:HashSetRange", Cat.Success, string.Format("key={0}", key));
            try
            {
                List<KeyValuePair<string, T>> list = keyValuePairs.ToList<KeyValuePair<string, T>>();
                if (list.Count != 0)
                {
                    byte[][] array = new byte[list.Count][];
                    byte[][] array2 = new byte[list.Count][];
                    string[] array3 = new string[list.Count];
                    for (int i = 0; i < list.Count; i++)
                    {
                        KeyValuePair<string, T> keyValuePair = list[i];
                        array[i] = StringExtensions.ToUtf8Bytes(keyValuePair.Key);
                        array2[i] = this.ConvertValueToBytes<T>(keyValuePair.Value);
                        string[] array4 = array3;
                        int num = i;
                        byte[] array5 = array2[i];
                        array4[num] = (((array5 != null) ? ((long)array5.Length).ToString() : null) ?? "0");
                    }
                    string value = string.Join(",", array3);
                    //transaction.AddData("size", value);
                    using (IRedisClient redisClient = this.ReadWriteClient(this.redisConnection))
                    {
                        //Cat.LogEvent("Cache.memcached.server", redisClient.Host, "0", null);
                        ((IRedisNativeClient)redisClient).HMSet(key, array, array2);
                    }
                }
            }
            catch (Exception ex)
            {
                //Cat.LogError(ex);
                //transaction.SetStatus(ex);
                throw;
            }
            finally
            {
                //transaction.Complete();
            }
        }

        /// <summary>
        /// 同时将多个 datakey-value (域-值)对设置到哈希表 key 中
        /// 如果 key 不存在，一个新的哈希表被创建并进行 HashSetRange 操作
        /// 如果域 dataKey 已经存在于哈希表中，旧值将被覆盖
        /// 同时必须指定key的过期时间
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="keyValuePairs">dataKey：value的集合</param>
        /// <param name="expireIn">过期时间</param>
        // Token: 0x06000182 RID: 386 RVA: 0x0000AB00 File Offset: 0x00008D00
        public void HashSetRange<T>(string key, IEnumerable<KeyValuePair<string, T>> keyValuePairs, TimeSpan expireIn)
        {
            //string str = KeyPatternManager.MatchPattern(key);
            //I//transaction //transaction = Cat.New//transaction("Cache.memcached", "HashSet:Set:HashSetRange:" + str);
            //Cat.LogEvent("Cache.memcached.key", "HashSet:HashSetRange", Cat.Success, string.Format("key={0}", key));
            try
            {
                List<KeyValuePair<string, T>> list = keyValuePairs.ToList<KeyValuePair<string, T>>();
                if (list.Count != 0)
                {
                    byte[][] array = new byte[list.Count][];
                    byte[][] array2 = new byte[list.Count][];
                    string[] array3 = new string[list.Count];
                    for (int i = 0; i < list.Count; i++)
                    {
                        KeyValuePair<string, T> keyValuePair = list[i];
                        array[i] = StringExtensions.ToUtf8Bytes(keyValuePair.Key);
                        array2[i] = this.ConvertValueToBytes<T>(keyValuePair.Value);
                        string[] array4 = array3;
                        int num = i;
                        byte[] array5 = array2[i];
                        array4[num] = (((array5 != null) ? ((long)array5.Length).ToString() : null) ?? "0");
                    }
                    string value = string.Join(",", array3);
                    //transaction.AddData("size", value);
                    using (IRedisClient redisClient = this.ReadWriteClient(this.redisConnection))
                    {
                        //Cat.LogEvent("Cache.memcached.server", redisClient.Host, "0", null);
                        ((IRedisNativeClient)redisClient).HMSet(key, array, array2);
                        redisClient.ExpireEntryIn(key, expireIn);
                    }
                }
            }
            catch (Exception ex)
            {
                //Cat.LogError(ex);
                //transaction.SetStatus(ex);
                throw;
            }
            finally
            {
                //transaction.Complete();
            }
        }

        /// <summary>
        /// 删除哈希表 key 中的指定域 dataKey，不存在的域将被忽略
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dataKey">dataKey</param>
        /// <returns>删除成功返回true 删除失败返回false</returns>
        // Token: 0x06000183 RID: 387 RVA: 0x0000ACBC File Offset: 0x00008EBC
        public bool HashRemoveField(string key, string dataKey)
        {
            //string str = KeyPatternManager.MatchPattern(key);
            //I//transaction //transaction = Cat.New//transaction("Cache.memcached", "HashSet:Remove:HashRemoveField:" + str);
            //Cat.LogEvent("Cache.memcached.key", "HashSet:HashRemoveField", Cat.Success, string.Format("key={0}&dKey={1}", key, dataKey));
            bool result;
            try
            {
                using (IRedisClient redisClient = this.ReadWriteClient(this.redisConnection))
                {
                    //Cat.LogEvent("Cache.memcached.server", redisClient.Host, "0", null);
                    result = redisClient.RemoveEntryFromHash(key, dataKey);
                }
            }
            catch (Exception ex)
            {
                //Cat.LogError(ex);
                //transaction.SetStatus(ex);
                throw;
            }
            finally
            {
                //transaction.Complete();
            }
            return result;
        }

        /// <summary>
        /// 删除哈希表 key 内所有的域
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>删除成功返回true 删除失败返回false</returns>
        // Token: 0x06000184 RID: 388 RVA: 0x0000AD84 File Offset: 0x00008F84
        public bool HashRemove(string key)
        {
            //string str = KeyPatternManager.MatchPattern(key);
            //I//transaction //transaction = Cat.New//transaction("Cache.memcached", "HashSet:Remove:HashRemove:" + str);
            //Cat.LogEvent("Cache.memcached.key", "HashSet:HashRemove", Cat.Success, "key=" + key);
            bool result;
            try
            {
                using (IRedisClient redisClient = this.ReadWriteClient(this.redisConnection))
                {
                    //Cat.LogEvent("Cache.memcached.server", redisClient.Host, "0", null);
                    result = redisClient.Remove(key);
                }
            }
            catch (Exception ex)
            {
                //Cat.LogError(ex);
                //transaction.SetStatus(ex);
                throw;
            }
            finally
            {
                //transaction.Complete();
            }
            return result;
        }

        /// <summary>
        /// 获取哈希表 key 中给定域 dataKey 的值
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="dataKey">dataKey</param>
        /// <returns>T</returns>
        // Token: 0x06000185 RID: 389 RVA: 0x0000AE48 File Offset: 0x00009048
        public T HashGet<T>(string key, string dataKey)
        {
            //string str = KeyPatternManager.MatchPattern(key);
            //I//transaction //transaction = Cat.New//transaction("Cache.memcached", "HashSet:Get:HashGet:" + str);
            //Cat.LogEvent("Cache.memcached.key", "HashSet:HashGet", Cat.Success, string.Format("key={0}&dKey={1}", key, dataKey));
            T result;
            try
            {
                using (IRedisClient redisClient = this.ReadOnlyClient(this.redisConnection))
                {
                    //Cat.LogEvent("Cache.memcached.server", redisClient.Host, "0", null);
                    byte[] array = ((IRedisNativeClient)redisClient).HGet(key, StringExtensions.ToUtf8Bytes(dataKey));
                    //transaction.AddData("size", (array != null) ? ((long)array.Length) : 0L);
                    if (array == null || array.Length == 0)
                    {
                        //Cat.LogEvent("Cache.memcached", "HashSet:missed", "0", null);
                    }
                    result = this.GetValue<T>(array);
                }
            }
            catch (Exception ex)
            {
                //Cat.LogError(ex);
                //transaction.SetStatus(ex);
                // Managers.RedisLog.Error(string.Format("redis read error, ErrorFunc:HashGet.   key:{0}  dataKey:{1}", key, dataKey), ex);
                result = default(T);
            }
            finally
            {
                //transaction.Complete();
            }
            return result;
        }

        /// <summary>
        /// 获取哈希表 key 中，一个或多个给定域的值
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="dataKeys">dataKeys</param>
        /// <returns>T的实体集合</returns>
        // Token: 0x06000186 RID: 390 RVA: 0x0000AF7C File Offset: 0x0000917C
        public List<T> HashGets<T>(string key, params string[] dataKeys)
        {
            //string str = KeyPatternManager.MatchPattern(key);
            //I//transaction //transaction = Cat.New//transaction("Cache.memcached", "HashSet:Get:HashGets:" + str);
            //Cat.LogEvent("Cache.memcached.key", "HashSet:HashGets", Cat.Success, string.Format("key={0}", key));
            List<T> result;
            try
            {
                if (dataKeys.Length == 0)
                {
                    result = new List<T>();
                }
                else
                {
                    List<T> list = new List<T>();
                    using (IRedisClient redisClient = this.ReadOnlyClient(this.redisConnection))
                    {
                        //Cat.LogEvent("Cache.memcached.server", redisClient.Host, "0", null);
                        byte[][] array = this.ConvertToKeysBytes(dataKeys);
                        byte[][] array2 = ((IRedisNativeClient)redisClient).HMGet(key, array);
                        long num = 0L;
                        if (array2 != null && array2.Length != 0)
                        {
                            foreach (byte[] array4 in array2)
                            {
                                num += ((array4 != null) ? ((long)array4.Length) : 0L);
                                list.Add(this.GetValue<T>(array4));
                            }
                        }
                        //transaction.AddData("size", num);
                        if (num == 0L)
                        {
                            //Cat.LogEvent("Cache.memcached", "HashSet:missed", "0", null);
                        }
                        result = list;
                    }
                }
            }
            catch (Exception ex)
            {
                //Cat.LogError(ex);
                //transaction.SetStatus(ex);
                Managers.RedisLog.Error(string.Format("redis read error, ErrorFunc:HashGets.   key:{0}  dataKeys:{1}", key, string.Join(",", dataKeys)), ex);
                result = null;
            }
            finally
            {
                //transaction.Complete();
            }
            return result;
        }

        /// <summary>
        /// 获取哈希表 key 中所有域的值
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="key">key</param>
        /// <returns>T的实体集合</returns>
        // Token: 0x06000187 RID: 391 RVA: 0x0000B12C File Offset: 0x0000932C
        public List<T> HashGetAll<T>(string key)
        {
            //string str = KeyPatternManager.MatchPattern(key);
            //I//transaction //transaction = Cat.New//transaction("Cache.memcached", "HashSet:Get:HashGetAll:" + str);
            //Cat.LogEvent("Cache.memcached.key", "HashSet:HashGetAll", Cat.Success, string.Format("key={0}", key));
            List<T> result;
            try
            {
                List<T> list = new List<T>();
                using (IRedisClient redisClient = this.ReadOnlyClient(this.redisConnection))
                {
                    //Cat.LogEvent("Cache.memcached.server", redisClient.Host, "0", null);
                    byte[][] array = ((IRedisNativeClient)redisClient).HVals(key);
                    long num = 0L;
                    if (array != null && array.Length != 0)
                    {
                        foreach (byte[] array3 in array)
                        {
                            num += ((array3 != null) ? ((long)array3.Length) : 0L);
                            list.Add(this.GetValue<T>(array3));
                        }
                    }
                    //transaction.AddData("size", num);
                    if (num == 0L)
                    {
                        //Cat.LogEvent("Cache.memcached", "HashSet:missed", "0", null);
                    }
                    result = list;
                }
            }
            catch (Exception ex)
            {
                //Cat.LogError(ex);
                //transaction.SetStatus(ex);
                Managers.RedisLog.Error(string.Format("redis read error, ErrorFunc:HashGetAll.   key:{0}", key), ex);
                result = null;
            }
            finally
            {
                //transaction.Complete();
            }
            return result;
        }

        /// <summary>
        /// 获取哈希表 key 中的所有域的键
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>域的键集合</returns>
        // Token: 0x06000188 RID: 392 RVA: 0x0000B290 File Offset: 0x00009490
        public List<string> HashGetKeys(string key)
        {
            //string str = KeyPatternManager.MatchPattern(key);
            //I//transaction //transaction = Cat.New//transaction("Cache.memcached", "HashSet:Other:HashGetKeys:" + str);
            //Cat.LogEvent("Cache.memcached.key", "HashSet:HashGetKeys", Cat.Success, "key=" + key);
            List<string> hashKeys;
            try
            {
                using (IRedisClient redisClient = this.ReadOnlyClient(this.redisConnection))
                {
                    //Cat.LogEvent("Cache.memcached.server", redisClient.Host, "0", null);
                    hashKeys = redisClient.GetHashKeys(key);
                }
            }
            catch (Exception ex)
            {
                //Cat.LogError(ex);
                //transaction.SetStatus(ex);
                throw;
            }
            finally
            {
                //transaction.Complete();
            }
            return hashKeys;
        }

        /// <summary>
        /// 获取哈希表 key 中所有域的值,以Dictionary的字典类型返回
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="key">key</param>
        /// <returns>Dictionary</returns>
        // Token: 0x06000189 RID: 393 RVA: 0x0000B354 File Offset: 0x00009554
        public Dictionary<string, T> HashGetAllDictionary<T>(string key)
        {
            //string str = KeyPatternManager.MatchPattern(key);
            //I//transaction //transaction = Cat.New//transaction("Cache.memcached", "HashSet:Get:HashGetAllDictionary:" + str);
            //Cat.LogEvent("Cache.memcached.key", "HashSet:HashGetAllDictionary", Cat.Success, string.Format("key={0}", key));
            Dictionary<string, T> result;
            try
            {
                Dictionary<string, T> dictionary = new Dictionary<string, T>();
                using (IRedisClient redisClient = this.ReadOnlyClient(this.redisConnection))
                {
                    //Cat.LogEvent("Cache.memcached.server", redisClient.Host, "0", null);
                    byte[][] array = ((IRedisNativeClient)redisClient).HGetAll(key);
                    long num = 0L;
                    if (array != null && array.Length != 0)
                    {
                        for (int i = 0; i < array.Length; i += 2)
                        {
                            long num2 = num;
                            byte[] array2 = array[i];
                            num = num2 + ((array2 != null) ? ((long)array2.Length) : 0L);
                            dictionary.Add(StringExtensions.FromUtf8Bytes(array[i]), this.GetValue<T>(array[i + 1]));
                        }
                    }
                    //transaction.AddData("size", num);
                    if (num == 0L)
                    {
                        //Cat.LogEvent("Cache.memcached", "HashSet:missed", "0", null);
                    }
                    result = dictionary;
                }
            }
            catch (Exception ex)
            {
                //Cat.LogError(ex);
                //transaction.SetStatus(ex);
                Managers.RedisLog.Error(string.Format("redis read error, ErrorFunc:HashGetAllDictionary.   key:{0}", key), ex);
                result = null;
            }
            finally
            {
                //transaction.Complete();
            }
            return result;
        }

        // Token: 0x0600018A RID: 394 RVA: 0x0000B4C0 File Offset: 0x000096C0
        internal Dictionary<string, Tuple<T, RedisDataInfo>> HashGetAllDictionaryWithRedisDataInfo<T>(string key)
        {
            //string str = KeyPatternManager.MatchPattern(key);
            //I//transaction //transaction = Cat.New//transaction("Cache.memcached", "HashSet:Get:HashGetAllDictionaryWithRedisDataInfo:" + str);
            //Cat.LogEvent("Cache.memcached.key", "HashSet:HashGetAllDictionaryWithRedisDataInfo", Cat.Success, string.Format("key={0}", key));
            Dictionary<string, Tuple<T, RedisDataInfo>> result;
            try
            {
                Dictionary<string, Tuple<T, RedisDataInfo>> dictionary = new Dictionary<string, Tuple<T, RedisDataInfo>>();
                using (IRedisClient redisClient = this.ReadOnlyClient(this.redisConnection))
                {
                    //Cat.LogEvent("Cache.memcached.server", redisClient.Host, "0", null);
                    byte[][] array = ((IRedisNativeClient)redisClient).HGetAll(key);
                    long num = 0L;
                    if (array != null && array.Length != 0)
                    {
                        for (int i = 0; i < array.Length; i += 2)
                        {
                            long num2 = num;
                            byte[] array2 = array[i];
                            num = num2 + ((array2 != null) ? ((long)array2.Length) : 0L);
                            RedisDataInfo item = new RedisDataInfo
                            {
                                IsNew = true,
                                IsCompress = false,
                                CompressSize = 0.0,
                                Size = 0.0
                            };
                            dictionary.Add(StringExtensions.FromUtf8Bytes(array[i]), Tuple.Create<T, RedisDataInfo>(this.GetValue<T>(array[i + 1], ref item), item));
                        }
                    }
                    //transaction.AddData("size", num);
                    if (num == 0L)
                    {
                        //Cat.LogEvent("Cache.memcached", "HashSet:missed", "0", null);
                    }
                    result = dictionary;
                }
            }
            catch (Exception ex)
            {
                //Cat.LogError(ex);
                //transaction.SetStatus(ex);
                Managers.RedisLog.Error(string.Format("redis read error, ErrorFunc:HashGetAllDictionary.   key:{0}", key), ex);
                throw new Exception(string.Format("redis read error, ErrorFunc:HashGetAllDictionary.   key:{0}", key), ex);
            }
            finally
            {
                //transaction.Complete();
            }
            return result;
        }

        /// <summary>
        /// 获取哈希表 key 中域的数量
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>域的数量</returns>
        // Token: 0x0600018B RID: 395 RVA: 0x0000B6A0 File Offset: 0x000098A0
        public long HashCount(string key)
        {
            //string str = KeyPatternManager.MatchPattern(key);
            //I//transaction //transaction = Cat.New//transaction("Cache.memcached", "HashSet:Count:HashCount:" + str);
            //Cat.LogEvent("Cache.memcached.key", "HashSet:HashCount", Cat.Success, "key=" + key);
            long hashCount;
            try
            {
                using (IRedisClient redisClient = this.ReadOnlyClient(this.redisConnection))
                {
                    //Cat.LogEvent("Cache.memcached.server", redisClient.Host, "0", null);
                    hashCount = redisClient.GetHashCount(key);
                }
            }
            catch (Exception ex)
            {
                //Cat.LogError(ex);
                //transaction.SetStatus(ex);
                throw;
            }
            finally
            {
                //transaction.Complete();
            }
            return hashCount;
        }

        /// <summary>
        /// 为哈希表 key 中的域 dataKey 的值加上增量 increment
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dataKey"></param>
        /// <param name="increment">增量</param>
        /// <returns>加上 increment 之后， key 的值哈希表 key 中域 dataKey 的值</returns>
        // Token: 0x0600018C RID: 396 RVA: 0x0000B764 File Offset: 0x00009964
        public long HashIncrementValue(string key, string dataKey, int increment)
        {
            //string str = KeyPatternManager.MatchPattern(key);
            //I//transaction //transaction = Cat.New//transaction("Cache.memcached", "HashSet:Set:HashIncrementValue:" + str);
            //Cat.LogEvent("Cache.memcached.key", "HashSet:HashIncrementValue", Cat.Success, "key=" + key);
            long result;
            try
            {
                using (IRedisClient redisClient = this.ReadWriteClient(this.redisConnection))
                {
                    //Cat.LogEvent("Cache.memcached.server", redisClient.Host, "0", null);
                    result = redisClient.IncrementValueInHash(key, dataKey, increment);
                }
            }
            catch (Exception ex)
            {
                //Cat.LogError(ex);
                //transaction.SetStatus(ex);
                throw;
            }
            finally
            {
                //transaction.Complete();
            }
            return result;
        }

        /// <summary>
        /// 为哈希表 key 中的域 dataKey 的值加上增量 increment
        /// 同时必须指定key的过期时间
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dataKey"></param>
        /// <param name="increment">增量</param>
        /// <param name="expireIn">过期时间</param>
        /// <returns>加上 increment 之后， key 的值哈希表 key 中域 dataKey 的值</returns>
        // Token: 0x0600018D RID: 397 RVA: 0x0000B82C File Offset: 0x00009A2C
        public long HashIncrementValue(string key, string dataKey, int increment, TimeSpan expireIn)
        {
            //string str = KeyPatternManager.MatchPattern(key);
            //I//transaction //transaction = Cat.New//transaction("Cache.memcached", "HashSet:Set:HashIncrementValue:" + str);
            //Cat.LogEvent("Cache.memcached.key", "HashSet:HashIncrementValue", Cat.Success, "key=" + key);
            long result;
            try
            {
                using (IRedisClient redisClient = this.ReadWriteClient(this.redisConnection))
                {
                    //Cat.LogEvent("Cache.memcached.server", redisClient.Host, "0", null);
                    long num = redisClient.IncrementValueInHash(key, dataKey, increment);
                    redisClient.ExpireEntryIn(key, expireIn);
                    result = num;
                }
            }
            catch (Exception ex)
            {
                //Cat.LogError(ex);
                //transaction.SetStatus(ex);
                throw;
            }
            finally
            {
                //transaction.Complete();
            }
            return result;
        }

        /// <summary>
        /// 将一个value 元素加入到无序集合 key 当中，已经存在于无序集合的 value 元素将被忽略
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        // Token: 0x0600018E RID: 398 RVA: 0x0000B8FC File Offset: 0x00009AFC
        public void SetAdd<T>(string key, T value)
        {
            //string str = KeyPatternManager.MatchPattern(key);
            //I//transaction //transaction = Cat.New//transaction("Cache.memcached", "Set:Set:SetAdd:" + str);
            //Cat.LogEvent("Cache.memcached.key", "Set:SetAdd", Cat.Success, string.Format("key={0}", key));
            try
            {
                byte[] array = this.ConvertValueToBytes<T>(value);
                //transaction.AddData("size", (array != null) ? ((long)array.Length) : 0L);
                using (IRedisClient redisClient = this.ReadWriteClient(this.redisConnection))
                {
                    //Cat.LogEvent("Cache.memcached.server", redisClient.Host, "0", null);
                    ((IRedisNativeClient)redisClient).SAdd(key, array);
                }
            }
            catch (Exception ex)
            {
                //Cat.LogError(ex);
                //transaction.SetStatus(ex);
                throw;
            }
            finally
            {
                //transaction.Complete();
            }
        }

        /// <summary>
        /// 将一个value 元素加入到无序集合 key 当中，已经存在于无序集合的 value 元素将被忽略
        /// 同时必须指定key的过期时间
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        /// <param name="expireIn">过期时间</param>
        // Token: 0x0600018F RID: 399 RVA: 0x0000B9E8 File Offset: 0x00009BE8
        public void SetAdd<T>(string key, T value, TimeSpan expireIn)
        {
            //string str = KeyPatternManager.MatchPattern(key);
            //I//transaction //transaction = Cat.New//transaction("Cache.memcached", "Set:Set:SetAdd:" + str);
            //Cat.LogEvent("Cache.memcached.key", "Set:SetAdd", Cat.Success, string.Format("key={0}", key));
            try
            {
                byte[] array = this.ConvertValueToBytes<T>(value);
                //transaction.AddData("size", (array != null) ? ((long)array.Length) : 0L);
                using (IRedisClient redisClient = this.ReadWriteClient(this.redisConnection))
                {
                    //Cat.LogEvent("Cache.memcached.server", redisClient.Host, "0", null);
                    ((IRedisNativeClient)redisClient).SAdd(key, array);
                    redisClient.ExpireEntryIn(key, expireIn);
                }
            }
            catch (Exception ex)
            {
                //Cat.LogError(ex);
                //transaction.SetStatus(ex);
                throw;
            }
            finally
            {
                //transaction.Complete();
            }
        }

        /// <summary>
        /// 将多个value 元素加入到无序集合 key 当中，已经存在于无序集合的 value 元素将被忽略
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="values">value的集合</param>
        // Token: 0x06000190 RID: 400 RVA: 0x0000BAE0 File Offset: 0x00009CE0
        public void SetAddRange<T>(string key, List<T> values)
        {
            if (values != null && values.Count <= 0)
            {
                return;
            }
            //string str = KeyPatternManager.MatchPattern(key);
            //I//transaction //transaction = Cat.New//transaction("Cache.memcached", "Set:Set:SetAddRange:" + str);
            //Cat.LogEvent("Cache.memcached.key", "Set:SetAddRange", Cat.Success, string.Format("key={0}", key));
            try
            {
                using (IRedisClient redisClient = this.ReadWriteClient(this.redisConnection))
                {
                    //Cat.LogEvent("Cache.memcached.server", redisClient.Host, "0", null);
                    byte[] array = StringExtensions.ToUtf8Bytes(key);
                    string[] array2 = new string[values.Count];
                    int num = 0;
                    RedisPipelineCommand redisPipelineCommand = ((RedisNativeClient)redisClient).CreatePipelineCommand();
                    foreach (T value in values)
                    {
                        byte[] array3 = this.ConvertValueToBytes<T>(value);
                        redisPipelineCommand.WriteCommand(new byte[][]
                        {
                            Commands.SAdd,
                            array,
                            array3
                        });
                        array2[num++] = (((array3 != null) ? ((long)array3.Length).ToString() : null) ?? "0");
                    }
                    redisPipelineCommand.Flush();
                    string value2 = string.Join(",", array2);
                    //transaction.AddData("size", value2);
                    redisPipelineCommand.ReadAllAsInts();
                }
            }
            catch (Exception ex)
            {
                //Cat.LogError(ex);
                //transaction.SetStatus(ex);
                throw;
            }
            finally
            {
                //transaction.Complete();
            }
        }

        /// <summary>
        /// 将多个value 元素加入到无序集合 key 当中，已经存在于无序集合的 value 元素将被忽略
        /// 同时必须指定key的过期时间
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="values">value的集合</param>
        /// <param name="expireIn">过期时间</param>
        // Token: 0x06000191 RID: 401 RVA: 0x0000BCB0 File Offset: 0x00009EB0
        public void SetAddRange<T>(string key, List<T> values, TimeSpan expireIn)
        {
            if (values != null && values.Count <= 0)
            {
                return;
            }
            //string str = KeyPatternManager.MatchPattern(key);
            try
            {
                using (IRedisClient redisClient = this.ReadWriteClient(this.redisConnection))
                {
                    byte[] array = StringExtensions.ToUtf8Bytes(key);
                    string[] array2 = new string[values.Count];
                    int num = 0;
                    RedisPipelineCommand redisPipelineCommand = ((RedisNativeClient)redisClient).CreatePipelineCommand();
                    foreach (T value in values)
                    {
                        byte[] array3 = this.ConvertValueToBytes<T>(value);
                        redisPipelineCommand.WriteCommand(new byte[][]
                        {
                            Commands.SAdd,
                            array,
                            array3
                        });
                        array2[num++] = (((array3 != null) ? ((long)array3.Length).ToString() : null) ?? "0");
                    }
                    redisPipelineCommand.Flush();
                    string value2 = string.Join(",", array2);
                    //transaction.AddData("size", value2);
                    redisPipelineCommand.ReadAllAsInts();
                    redisClient.ExpireEntryIn(key, expireIn);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// 移除并返回无序集合 key 中的一个随机元素
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="key">key</param>
        /// <returns>T</returns>
        // Token: 0x06000192 RID: 402 RVA: 0x0000BE88 File Offset: 0x0000A088
        public T SetPop<T>(string key)
        {
            //string str = KeyPatternManager.MatchPattern(key);
            T result;
            try
            {
                using (IRedisClient redisClient = this.ReadWriteClient(this.redisConnection))
                {
                    byte[] array = ((IRedisNativeClient)redisClient).SPop(key);
                    //transaction.AddData("size", (array != null) ? ((long)array.Length) : 0L);
                    result = this.GetValue<T>(array);
                }
            }
            catch (Exception ex)
            {
                Managers.RedisLog.Error(string.Format("redis read error, ErrorFunc:SetPop.   key:{0}", key), ex);
                result = default(T);
            }
            return result;
        }

        /// <summary>
        /// 获取无序集合 key 中的所有成员,以HashSet的类型返回结果
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="key">key</param>
        /// <returns>HashSet</returns>
        // Token: 0x06000193 RID: 403 RVA: 0x0000BFB4 File Offset: 0x0000A1B4
        public HashSet<T> SetGetAll<T>(string key)
        {
            //string str = KeyPatternManager.MatchPattern(key);
            HashSet<T> result;
            try
            {
                using (IRedisClient redisClient = this.ReadOnlyClient(this.redisConnection))
                {
                    byte[][] array = ((IRedisNativeClient)redisClient).SMembers(key);
                    HashSet<T> hashSet = new HashSet<T>();
                    long num = 0L;
                    foreach (byte[] array3 in array)
                    {
                        num += ((array3 != null) ? ((long)array3.Length) : 0L);
                        hashSet.Add(this.GetValue<T>(array3));
                    }
                    //transaction.AddData("size", num);
                    result = hashSet;
                }
            }
            catch (Exception ex)
            {
                Managers.RedisLog.Error(string.Format("redis read error, ErrorFunc:SetGetAll.   key:{0}", key), ex);
                result = null;
            }
            return result;
        }

        // Token: 0x06000194 RID: 404 RVA: 0x0000C10C File Offset: 0x0000A30C
        internal HashSet<T> SetGetAll<T>(string key, out RedisDataInfo redisDataInfo)
        {
            redisDataInfo = new RedisDataInfo
            {
                IsNew = true,
                IsCompress = false,
                CompressSize = 0.0,
                Size = 0.0
            };
            //string str = KeyPatternManager.MatchPattern(key);
            HashSet<T> result;
            try
            {
                using (IRedisClient redisClient = this.ReadOnlyClient(this.redisConnection))
                {
                    byte[][] array = ((IRedisNativeClient)redisClient).SMembers(key);
                    HashSet<T> hashSet = new HashSet<T>();
                    long num = 0L;
                    foreach (byte[] array3 in array)
                    {
                        num += ((array3 != null) ? ((long)array3.Length) : 0L);
                        hashSet.Add(this.GetValue<T>(array3, ref redisDataInfo));
                    }
                    //transaction.AddData("size", num);
                    result = hashSet;
                }
            }
            catch (Exception ex)
            {
                Managers.RedisLog.Error(string.Format("redis read error, ErrorFunc:SetGetAll.   key:{0}", key), ex);
                throw new Exception(string.Format("redis read error, ErrorFunc:SetGetAll.   key:{0}", key), ex);
            }
            return result;
        }

        /// <summary>
        /// 移除无序集合 key 中的 value 元素
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        // Token: 0x06000195 RID: 405 RVA: 0x0000C2A4 File Offset: 0x0000A4A4
        public void SetRemoveMember<T>(string key, T value)
        {
            //string str = KeyPatternManager.MatchPattern(key);
            try
            {
                using (IRedisClient redisClient = this.ReadWriteClient(this.redisConnection))
                {
                    ((RedisNativeClient)redisClient).SRem(key, this.ConvertValueToBytes<T>(value));
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// 根据传入无序集合的key移除整个无序集合
        /// </summary>
        /// <param name="keys">待移除无序集合的key</param>
        /// <returns>移除成功返回true 移除失败返回false </returns>
        // Token: 0x06000196 RID: 406 RVA: 0x00009E10 File Offset: 0x00008010
        public bool SetRemove(string key)
        {
            return this.RemoveEntry(new string[]
            {
                key
            });
        }

        /// <summary>
        /// 获取无序集合key中元素的数量
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>集合的元素数量</returns>
        // Token: 0x06000197 RID: 407 RVA: 0x0000C374 File Offset: 0x0000A574
        public long SetCount(string key)
        {
            //string str = KeyPatternManager.MatchPattern(key);
            long setCount;
            try
            {
                using (IRedisClient redisClient = this.ReadOnlyClient(this.redisConnection))
                {
                    setCount = redisClient.GetSetCount(key);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return setCount;
        }

        /// <summary>
        /// 检查给定 key的无序集合 是否存在
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>存在返回true 不存在返回false</returns>
        // Token: 0x06000198 RID: 408 RVA: 0x00008E94 File Offset: 0x00007094
        public bool SetExist(string key)
        {
            return this.ContainsKey(key);
        }

        /// <summary>
        /// 判断无序集合 key 的成员内是否存在 value 元素
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        /// <returns>存在返回true 不存在返回false</returns>
        // Token: 0x06000199 RID: 409 RVA: 0x0000C438 File Offset: 0x0000A638
        public bool SetExistMember<T>(string key, T value)
        {
            //string str = KeyPatternManager.MatchPattern(key);
            bool result;
            try
            {
                byte[] array = this.ConvertValueToBytes<T>(value);
                //transaction.AddData("size", (array != null) ? ((long)array.Length) : 0L);
                using (IRedisClient redisClient = this.ReadWriteClient(this.redisConnection))
                {
                    result = (((IRedisNativeClient)redisClient).SIsMember(key, array) == 1L);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return result;
        }

        /// <summary>
        /// 将 value 元素及其排序值 score 加入到有序集 key 当中。
        /// 如果某个 value 已经是有序集的成员，那么更新这个 member 的 score 值，并通过重新插入这个 value 元素，来保证该 value 在正确的位置上
        ///  key 不存在，则创建一个空的有序集并执行 SortedSetAdd 操作
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        /// <param name="score">value的排序值</param>
        /// <returns>设置成功返回true 设置失败返回false</returns>
        // Token: 0x0600019A RID: 410 RVA: 0x0000C52C File Offset: 0x0000A72C
        public bool SortedSetAdd<T>(string key, T value, long score)
        {
            //string str = KeyPatternManager.MatchPattern(key);
            bool result;
            try
            {
                byte[] array = this.ConvertValueToBytes<T>(value);
                //transaction.AddData("size", (array != null) ? ((long)array.Length) : 0L);
                using (IRedisClient redisClient = this.ReadWriteClient(this.redisConnection))
                {
                    result = (((IRedisNativeClient)redisClient).ZAdd(key, score, array) == 1L);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return result;
        }

        /// <summary>
        /// 将 value 元素及其排序值 score 加入到有序集 key 当中。
        /// 如果某个 value 已经是有序集的成员，那么更新这个 member 的 score 值，并通过重新插入这个 value 元素，来保证该 value 在正确的位置上
        /// key 不存在，则创建一个空的有序集并执行 SortedSetAdd 操作
        /// 同时必须指定key的过期时间
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        /// <param name="score">value的排序值</param>
        /// <param name="expireIn">过期时间</param>
        /// <returns>设置成功返回true 设置失败返回false</returns>
        // Token: 0x0600019B RID: 411 RVA: 0x0000C620 File Offset: 0x0000A820
        public bool SortedSetAdd<T>(string key, T value, long score, TimeSpan expireIn)
        {
            //string str = KeyPatternManager.MatchPattern(key);
            bool result;
            try
            {
                byte[] array = this.ConvertValueToBytes<T>(value);
                //transaction.AddData("size", (array != null) ? ((long)array.Length) : 0L);
                using (IRedisClient redisClient = this.ReadWriteClient(this.redisConnection))
                {
                    bool flag = ((IRedisNativeClient)redisClient).ZAdd(key, score, array) == 1L;
                    if (flag)
                    {
                        redisClient.ExpireEntryIn(key, expireIn);
                    }
                    result = flag;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return result;
        }

        /// <summary>
        /// 为有序集 key 的成员 value 的 score 值加上增量 increment 返回增量后的score
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        /// <param name="increment">增量值</param>
        /// <returns>增加 increment后的score值</returns>
        // Token: 0x0600019C RID: 412 RVA: 0x0000C720 File Offset: 0x0000A920
        public double SortedSetIncrement<T>(string key, T value, long increment)
        {
            //string str = KeyPatternManager.MatchPattern(key);
            double result;
            try
            {
                using (IRedisClient redisClient = this.ReadWriteClient(this.redisConnection))
                {
                    result = ((IRedisNativeClient)redisClient).ZIncrBy(key, increment, this.ConvertValueToBytes<T>(value));
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return result;
        }

        /// <summary>
        /// 将value元素 从有序集key移除
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        /// <returns>移除成功返回true 移除失败返回false</returns>
        // Token: 0x0600019D RID: 413 RVA: 0x0000C7F0 File Offset: 0x0000A9F0
        public bool SortedSetRemove<T>(string key, T value)
        {
            //string str = KeyPatternManager.MatchPattern(key);
            bool result;
            try
            {
                using (IRedisClient redisClient = this.ReadWriteClient(this.redisConnection))
                {
                    result = (((IRedisNativeClient)redisClient).ZRem(key, this.ConvertValueToBytes<T>(value)) == 1L);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return result;
        }

        /// <summary>
        /// 获取有序集key中的所有元素
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="key">key</param>
        /// <returns>T的实体集合</returns>
        // Token: 0x0600019E RID: 414 RVA: 0x0000C8C4 File Offset: 0x0000AAC4
        public List<T> SortedSetGetAll<T>(string key)
        {
            //string str = KeyPatternManager.MatchPattern(key);
            List<T> result;
            try
            {
                List<T> list = new List<T>();
                using (IRedisClient redisClient = this.ReadOnlyClient(this.redisConnection))
                {
                    byte[][] array = ((IRedisNativeClient)redisClient).ZRange(key, 0, -1);
                    long num = 0L;
                    if (array != null && array.Length != 0)
                    {
                        foreach (byte[] array3 in array)
                        {
                            num += ((array3 != null) ? ((long)array3.Length) : 0L);
                            list.Add(this.GetValue<T>(array3));
                        }
                    }
                    //transaction.AddData("size", num);
                    result = list;
                }
            }
            catch (Exception ex)
            {
                Managers.RedisLog.Error(string.Format("redis read error, ErrorFunc:SortedSetGetAll.   key:{0}", key), ex);
                result = null;
            }
            return result;
        }

        // Token: 0x0600019F RID: 415 RVA: 0x0000CA2C File Offset: 0x0000AC2C
        internal List<T> SortedSetGetAll<T>(string key, out RedisDataInfo redisDataInfo)
        {
            redisDataInfo = new RedisDataInfo
            {
                IsNew = true,
                IsCompress = false,
                CompressSize = 0.0,
                Size = 0.0
            };
            //string str = KeyPatternManager.MatchPattern(key);
            List<T> result;
            try
            {
                List<T> list = new List<T>();
                using (IRedisClient redisClient = this.ReadOnlyClient(this.redisConnection))
                {
                    byte[][] array = ((IRedisNativeClient)redisClient).ZRevRange(key, 0, -1);
                    long num = 0L;
                    if (array != null && array.Length != 0)
                    {
                        foreach (byte[] array3 in array)
                        {
                            num += ((array3 != null) ? ((long)array3.Length) : 0L);
                            list.Add(this.GetValue<T>(array3, ref redisDataInfo));
                        }
                    }
                    //transaction.AddData("size", num);
                    result = list;
                }
            }
            catch (Exception ex)
            {
                Managers.RedisLog.Error(string.Format("redis read error, ErrorFunc:SortedSetGetAllDesc.   key:{0}", key), ex);
                result = null;
            }
            return result;
        }

        /// <summary>
        /// 获取有序集key中的所有元素
        /// 其中成员的位置按 score 值递减(从大到小)来排列。
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="key">key</param>
        /// <returns>T的实体集合</returns>
        // Token: 0x060001A0 RID: 416 RVA: 0x0000CBC4 File Offset: 0x0000ADC4
        public List<T> SortedSetGetAllDesc<T>(string key)
        {
            //string str = KeyPatternManager.MatchPattern(key);
            List<T> result;
            try
            {
                List<T> list = new List<T>();
                using (IRedisClient redisClient = this.ReadOnlyClient(this.redisConnection))
                {
                    byte[][] array = ((IRedisNativeClient)redisClient).ZRevRange(key, 0, -1);
                    long num = 0L;
                    if (array != null && array.Length != 0)
                    {
                        foreach (byte[] array3 in array)
                        {
                            num += ((array3 != null) ? ((long)array3.Length) : 0L);
                            list.Add(this.GetValue<T>(array3));
                        }
                    }
                    //transaction.AddData("size", num);
                    result = list;
                }
            }
            catch (Exception ex)
            {
                Managers.RedisLog.Error(string.Format("redis read error, ErrorFunc:SortedSetGetAllDesc.   key:{0}", key), ex);
                result = null;
            }
            return result;
        }

        /// <summary>
        /// 获取有序集 key 中，所有 score 值介于 fromScore 和 toScore 之间(包括等于 fromScore 或 toScore )的成员。有序集成员按 score 值递增(从小到大)次序排列
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="fromScore">起始score值</param>
        /// <param name="toScore">结束score值</param>
        /// <param name="skip">跳过的元素数量</param>
        /// <param name="take">获取的元素数量</param>
        /// <returns>T的实体集合</returns>
        // Token: 0x060001A1 RID: 417 RVA: 0x0000CD28 File Offset: 0x0000AF28
        public List<T> SortedSetGetRangeByLowestScore<T>(string key, long fromScore, long toScore, int? skip, int? take)
        {
            //string str = KeyPatternManager.MatchPattern(key);
            List<T> result;
            try
            {
                List<T> list = new List<T>();
                using (IRedisClient redisClient = this.ReadOnlyClient(this.redisConnection))
                {
                    byte[][] array = ((IRedisNativeClient)redisClient).ZRangeByScore(key, fromScore, toScore, skip, take);
                    long num = 0L;
                    if (array != null && array.Length != 0)
                    {
                        foreach (byte[] array3 in array)
                        {
                            num += ((array3 != null) ? ((long)array3.Length) : 0L);
                            list.Add(this.GetValue<T>(array3));
                        }
                    }
                    //transaction.AddData("size", num);
                    result = list;
                }
            }
            catch (Exception ex)
            {
                Managers.RedisLog.Error(string.Format("redis read error, ErrorFunc:SortedSetGetRangeByLowestScore.   key:{0}  fromScore:{1}  toScore:{2}  skip:{3}  take:{4}", new object[]
                {
                    key,
                    fromScore,
                    toScore,
                    skip,
                    take
                }), ex);
                result = null;
            }
            return result;
        }

        /// <summary>
        /// 获取有序集 key 中， 所有 score 值介于 fromScore 和 toScore 之间(包括等于 fromScore 或 toScore )的成员。有序集成员按 score 值递减(从大到小)的次序排列
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="fromScore">起始score值</param>
        /// <param name="toScore">结束score值</param>
        /// <param name="skip">跳过的元素数量</param>
        /// <param name="take">获取的元素数量</param>
        /// <returns>T的实体集合</returns>
        // Token: 0x060001A2 RID: 418 RVA: 0x0000CEE4 File Offset: 0x0000B0E4
        public List<T> SortedSetGetRangeByHighestScore<T>(string key, long fromScore, long toScore, int? skip, int? take)
        {
            //string str = KeyPatternManager.MatchPattern(key);
            List<T> result;
            try
            {
                List<T> list = new List<T>();
                using (IRedisClient redisClient = this.ReadOnlyClient(this.redisConnection))
                {
                    byte[][] array = ((IRedisNativeClient)redisClient).ZRevRangeByScore(key, fromScore, toScore, skip, take);
                    long num = 0L;
                    if (array != null && array.Length != 0)
                    {
                        foreach (byte[] array3 in array)
                        {
                            num += ((array3 != null) ? ((long)array3.Length) : 0L);
                            list.Add(this.GetValue<T>(array3));
                        }
                    }
                    //transaction.AddData("size", num);
                    result = list;
                }
            }
            catch (Exception ex)
            {
                Managers.RedisLog.Error(string.Format("redis read error, ErrorFunc:SortedSetGetRangeByHighestScore.   key:{0}  fromScore:{1}  toScore:{2}  skip:{3}  take:{4}", new object[]
                {
                    key,
                    fromScore,
                    toScore,
                    skip,
                    take
                }), ex);
                result = null;
            }
            return result;
        }

        /// <summary>
        /// 获取有序集 key 中，score 值介于 fromScore 和 toScore 之间(包括等于 fromScore 或 toScore )的成员数量
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="fromScore">起始score值</param>
        /// <param name="toScore">结束score值</param>
        /// <returns>成员数量</returns>
        // Token: 0x060001A3 RID: 419 RVA: 0x0000D0A0 File Offset: 0x0000B2A0
        public long SortedSetCount(string key, long fromScore, long toScore)
        {
            //string str = KeyPatternManager.MatchPattern(key);
            long sortedSetCount;
            try
            {
                using (IRedisClient redisClient = this.ReadOnlyClient(this.redisConnection))
                {
                    sortedSetCount = redisClient.GetSortedSetCount(key, fromScore, toScore);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return sortedSetCount;
        }

        /// <summary>
        /// 移除有序集 key 中，指定排名(rank)区间内的所有成员，区间分别以下标参数 minRank 和 maxRank 指出，包含 minRank 和 maxRank 在内
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="minRank">区间起始排名</param>
        /// <param name="maxRank">区间结束排名</param>
        /// <returns>被移除的元素数量</returns>
        // Token: 0x060001A4 RID: 420 RVA: 0x0000D168 File Offset: 0x0000B368
        public long SortedSetRemoveRange(string key, int minRank, int maxRank)
        {
            //string str = KeyPatternManager.MatchPattern(key);
            long result;
            try
            {
                using (IRedisClient redisClient = this.ReadWriteClient(this.redisConnection))
                {
                    result = redisClient.RemoveRangeFromSortedSet(key, minRank, maxRank);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        /// <summary>
        /// 移除有序集 key 中，所有 score 值介于 fromScore 和 toScore 之间(包括等于 fromScore 或 toScore )的成员
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="fromScore">起始score值</param>
        /// <param name="toScore">结束score值</param>
        /// <returns>被移除的元素数量</returns>
        // Token: 0x060001A5 RID: 421 RVA: 0x0000D230 File Offset: 0x0000B430
        public long SortedSetRemoveRangeByScore(string key, long fromScore, long toScore)
        {
            //string str = KeyPatternManager.MatchPattern(key);
            long result;
            try
            {
                using (IRedisClient redisClient = this.ReadWriteClient(this.redisConnection))
                {
                    result = redisClient.RemoveRangeFromSortedSetByScore(key, fromScore, toScore);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        /// <summary>
        /// 获取有序集 key 中，指定区间内的成员,其中成员的位置按 score 值递减(从大到小)来排列
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="start">区间的启示成员索引值</param>
        /// <param name="count">区间成员的长度</param>
        /// <returns>T的实体集合</returns>
        // Token: 0x060001A6 RID: 422 RVA: 0x0000D2F8 File Offset: 0x0000B4F8
        public List<T> SortedSetGetRangeDesc<T>(string key, int start, int count)
        {
            //string str = KeyPatternManager.MatchPattern(key);
            List<T> result;
            try
            {
                if (count < 1)
                {
                    result = new List<T>();
                }
                else
                {
                    int num = start + count - 1;
                    List<T> list = new List<T>();
                    using (IRedisClient redisClient = this.ReadOnlyClient(this.redisConnection))
                    {
                        byte[][] array = ((IRedisNativeClient)redisClient).ZRevRange(key, start, num);
                        long num2 = 0L;
                        if (array != null && array.Length != 0)
                        {
                            foreach (byte[] array3 in array)
                            {
                                num2 += ((array3 != null) ? ((long)array3.Length) : 0L);
                                list.Add(this.GetValue<T>(array3));
                            }
                        }
                        //transaction.AddData("size", num2);
                        result = list;
                    }
                }
            }
            catch (Exception ex)
            {
                Managers.RedisLog.Error(string.Format("redis read error, ErrorFunc:SortedSetGetRangeDesc.   key:{0}  start:{1}  count:{2}", key, start, count), ex);
                result = null;
            }
            return result;
        }

        /// <summary>
        /// 获取有序集key内所有成员的值以及排序值，以IDictionary的字典类型返回（Key：成员值，Value：成员的排序值）
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="key">key</param>
        /// <returns>IDictionary</returns>
        // Token: 0x060001A7 RID: 423 RVA: 0x0000D4A8 File Offset: 0x0000B6A8
        public IDictionary<T, double> SortedSetGetAllWithScores<T>(string key)
        {
            //string str = KeyPatternManager.MatchPattern(key);
            IDictionary<T, double> result;
            try
            {
                OrderedDictionary<T, double> orderedDictionary = new OrderedDictionary<T, double>();
                using (IRedisClient redisClient = this.ReadOnlyClient(this.redisConnection))
                {
                    byte[][] array = ((IRedisNativeClient)redisClient).ZRangeWithScores(key, 0, -1);
                    long num = 0L;
                    for (int i = 0; i < array.Length; i += 2)
                    {
                        long num2 = num;
                        byte[] array2 = array[i];
                        num = num2 + ((array2 != null) ? ((long)array2.Length) : 0L);
                        T value = this.GetValue<T>(array[i]);
                        double num3;
                        double.TryParse(StringExtensions.FromUtf8Bytes(array[i + 1]), NumberStyles.Any, CultureInfo.InvariantCulture, out num3);
                        orderedDictionary[value] = num3;
                    }
                    //transaction.AddData("size", num);
                    result = orderedDictionary;
                }
            }
            catch (Exception ex)
            {
                Managers.RedisLog.Error(string.Format("redis read error, ErrorFunc:SortedSetGetAllWithScores.   key:{0}", key), ex);
                result = null;
            }
            return result;
        }

        /// <summary>
        /// 获取有序集key中,value的索引值
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        /// <returns>value的索引值</returns>
        // Token: 0x060001A8 RID: 424 RVA: 0x0000D648 File Offset: 0x0000B848
        public long SortedSetGetIndexDesc<T>(string key, T value)
        {
            //string str = KeyPatternManager.MatchPattern(key);
            long result;
            try
            {
                using (IRedisClient redisClient = this.ReadOnlyClient(this.redisConnection))
                {
                    result = ((IRedisNativeClient)redisClient).ZRevRank(key, this.ConvertValueToBytes<T>(value));
                }
            }
            catch (Exception ex)
            {
                //transaction.SetStatus(ex);
                throw;
            }
            return result;
        }

        /// <summary>
        /// 计算给定的一个或多个有序集的并集，并将该并集(结果集)储存到 intoKey
        /// </summary>
        /// <param name="intoKey">结果集的Key</param>
        /// <param name="keys">待合并的keys</param>
        /// <returns>结果集中成员的数量</returns>
        // Token: 0x060001A9 RID: 425 RVA: 0x0000D718 File Offset: 0x0000B918
        public long SortedSetUnion(string intoKey, params string[] keys)
        {
            //string str = KeyPatternManager.MatchPattern(keys[0]);
            long result;
            try
            {
                using (IRedisClient redisClient = this.ReadWriteClient(this.redisConnection))
                {
                    result = redisClient.StoreUnionFromSortedSets(intoKey, keys);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        // Token: 0x060001AA RID: 426 RVA: 0x0000D804 File Offset: 0x0000BA04
        public async Task<bool> RemoveEntryAsync(params string[] keys)
        {
            return this.RemoveEntry(keys);
        }

        // Token: 0x060001AB RID: 427 RVA: 0x0000D854 File Offset: 0x0000BA54
        public async Task<bool> ExpireEntryInAsync(string key, TimeSpan expireIn)
        {
            return this.ExpireEntryIn(key, expireIn);
        }

        // Token: 0x060001AC RID: 428 RVA: 0x0000D8AC File Offset: 0x0000BAAC
        public async Task<bool> ExpireEntryAtAsync(string key, DateTime expiresAt)
        {
            return this.ExpireEntryAt(key, expiresAt);
        }

        // Token: 0x060001AD RID: 429 RVA: 0x0000D904 File Offset: 0x0000BB04
        public async Task<TimeSpan?> GetTimeToLiveAsync(string key)
        {
            return this.GetTimeToLive(key);
        }

        // Token: 0x060001AE RID: 430 RVA: 0x0000D954 File Offset: 0x0000BB54
        public async Task<RedisKeyType> GetKeyTypeAsync(string key)
        {
            return this.GetKeyType(key);
        }

        // Token: 0x060001AF RID: 431 RVA: 0x0000D9A4 File Offset: 0x0000BBA4
        public async Task<bool> ContainsKeyAsync(string key)
        {
            return this.ContainsKey(key);
        }

        // Token: 0x060001B0 RID: 432 RVA: 0x0000D9F4 File Offset: 0x0000BBF4
        public async Task<long> SetBitAsync(string key, int offset, int value)
        {
            return this.SetBit(key, offset, value);
        }

        // Token: 0x060001B1 RID: 433 RVA: 0x0000DA54 File Offset: 0x0000BC54
        public async Task<long> GetBitAsync(string key, int offset)
        {
            return this.GetBit(key, offset);
        }

        // Token: 0x060001B2 RID: 434 RVA: 0x0000DAAC File Offset: 0x0000BCAC
        public async Task<long> BitCountAsync(string key)
        {
            return this.BitCount(key);
        }

        // Token: 0x060001B3 RID: 435 RVA: 0x0000DAFC File Offset: 0x0000BCFC
        public async Task<long> BitCountAsync(string intoKey, params string[] keys)
        {
            return this.BitCount(intoKey, keys);
        }

        // Token: 0x060001B4 RID: 436 RVA: 0x0000DB54 File Offset: 0x0000BD54
        public async Task<bool> ItemSetAsync<T>(string key, T value)
        {
            return this.ItemSet<T>(key, value);
        }

        // Token: 0x060001B5 RID: 437 RVA: 0x0000DBAC File Offset: 0x0000BDAC
        public async Task<bool> ItemSetAsync<T>(string key, T value, TimeSpan expireIn)
        {
            return this.ItemSet<T>(key, value, expireIn);
        }

        // Token: 0x060001B6 RID: 438 RVA: 0x0000DC0C File Offset: 0x0000BE0C
        public async Task<bool> ItemSetAllAsync<T>(IDictionary<string, T> values)
        {
            this.ItemSetAll<T>(values);
            return true;
        }

        // Token: 0x060001B7 RID: 439 RVA: 0x0000DC5C File Offset: 0x0000BE5C
        public async Task<T> ItemGetAsync<T>(string key)
        {
            return this.ItemGet<T>(key);
        }

        // Token: 0x060001B8 RID: 440 RVA: 0x0000DCAC File Offset: 0x0000BEAC
        public async Task<T> ItemGetSetAsync<T>(string key, T value)
        {
            return this.ItemGetSet<T>(key, value);
        }

        // Token: 0x060001B9 RID: 441 RVA: 0x0000DD04 File Offset: 0x0000BF04
        public async Task<T> ItemGetSetAsync<T>(string key, T value, TimeSpan expireIn)
        {
            return this.ItemGetSet<T>(key, value, expireIn);
        }

        // Token: 0x060001BA RID: 442 RVA: 0x0000DD64 File Offset: 0x0000BF64
        public async Task<IDictionary<string, T>> ItemGetAllAsync<T>(IEnumerable<string> keys)
        {
            return this.ItemGetAll<T>(keys);
        }

        // Token: 0x060001BB RID: 443 RVA: 0x0000DDB4 File Offset: 0x0000BFB4
        public async Task<bool> ItemRemoveAsync(string key)
        {
            return this.ItemRemove(key);
        }

        // Token: 0x060001BC RID: 444 RVA: 0x0000DE04 File Offset: 0x0000C004
        public async Task<bool> ItemRemoveAllAsync(IEnumerable<string> keys)
        {
            this.ItemRemoveAll(keys);
            return true;
        }

        // Token: 0x060001BD RID: 445 RVA: 0x0000DE54 File Offset: 0x0000C054
        public async Task<long> ItemIncrementAsync(string key, uint increment)
        {
            return this.ItemIncrement(key, increment);
        }

        // Token: 0x060001BE RID: 446 RVA: 0x0000DEAC File Offset: 0x0000C0AC
        public async Task<long> ItemIncrementAsync(string key, uint increment, TimeSpan expireIn)
        {
            return this.ItemIncrement(key, increment, expireIn);
        }

        // Token: 0x060001BF RID: 447 RVA: 0x0000DF0C File Offset: 0x0000C10C
        public async Task<long> ItemDecrementAsync(string key, uint decrement)
        {
            return this.ItemDecrement(key, decrement);
        }

        // Token: 0x060001C0 RID: 448 RVA: 0x0000DF64 File Offset: 0x0000C164
        public async Task<long> ItemDecrementAsync(string key, uint decrement, TimeSpan expireIn)
        {
            return this.ItemDecrement(key, decrement, expireIn);
        }

        // Token: 0x060001C1 RID: 449 RVA: 0x0000DFC4 File Offset: 0x0000C1C4
        public async Task<bool> ItemExistAsync(string key)
        {
            return this.ItemExist(key);
        }

        // Token: 0x060001C2 RID: 450 RVA: 0x0000E014 File Offset: 0x0000C214
        public async Task<bool> ListAddToHeadAsync<T>(string key, T value)
        {
            this.ListAddToHead<T>(key, value);
            return true;
        }

        // Token: 0x060001C3 RID: 451 RVA: 0x0000E06C File Offset: 0x0000C26C
        public async Task<bool> ListAddToHeadAsync<T>(string key, T value, TimeSpan expireIn)
        {
            this.ListAddToHead<T>(key, value, expireIn);
            return true;
        }

        // Token: 0x060001C4 RID: 452 RVA: 0x0000E0CC File Offset: 0x0000C2CC
        public async Task<bool> ListAddRangeToHeadAsync<T>(string key, List<T> values)
        {
            this.ListAddRangeToHead<T>(key, values);
            return true;
        }

        // Token: 0x060001C5 RID: 453 RVA: 0x0000E124 File Offset: 0x0000C324
        public async Task<bool> ListAddRangeToHeadAsync<T>(string key, List<T> values, TimeSpan expireIn)
        {
            this.ListAddRangeToHead<T>(key, values, expireIn);
            return true;
        }

        // Token: 0x060001C6 RID: 454 RVA: 0x0000E184 File Offset: 0x0000C384
        public async Task<bool> ListAddToEndAsync<T>(string key, T value)
        {
            this.ListAddToEnd<T>(key, value);
            return true;
        }

        // Token: 0x060001C7 RID: 455 RVA: 0x0000E1DC File Offset: 0x0000C3DC
        public async Task<bool> ListAddToEndAsync<T>(string key, T value, TimeSpan expireIn)
        {
            this.ListAddToEnd<T>(key, value, expireIn);
            return true;
        }

        // Token: 0x060001C8 RID: 456 RVA: 0x0000E23C File Offset: 0x0000C43C
        public async Task<bool> ListAddRangeToEndAsync<T>(string key, List<T> values)
        {
            this.ListAddRangeToEnd<T>(key, values);
            return true;
        }

        // Token: 0x060001C9 RID: 457 RVA: 0x0000E294 File Offset: 0x0000C494
        public async Task<bool> ListAddRangeToEndAsync<T>(string key, List<T> values, TimeSpan expireIn)
        {
            this.ListAddRangeToEnd<T>(key, values, expireIn);
            return true;
        }

        // Token: 0x060001CA RID: 458 RVA: 0x0000E2F4 File Offset: 0x0000C4F4
        public async Task<bool> ListRemoveItemAsync<T>(string key, T value)
        {
            return this.ListRemoveItem<T>(key, value);
        }

        // Token: 0x060001CB RID: 459 RVA: 0x0000E34C File Offset: 0x0000C54C
        public async Task<T> ListRemoveHeadAsync<T>(string key)
        {
            return this.ListRemoveHead<T>(key);
        }

        // Token: 0x060001CC RID: 460 RVA: 0x0000E39C File Offset: 0x0000C59C
        public async Task<T> ListRemoveEndAsync<T>(string key)
        {
            return this.ListRemoveEnd<T>(key);
        }

        // Token: 0x060001CD RID: 461 RVA: 0x0000E3EC File Offset: 0x0000C5EC
        public async Task<bool> ListRemoveTrimAsync(string key, int keepStartingFrom, int keepEndingAt)
        {
            this.ListRemoveTrim(key, keepStartingFrom, keepEndingAt);
            return true;
        }

        // Token: 0x060001CE RID: 462 RVA: 0x0000E44C File Offset: 0x0000C64C
        public async Task<bool> ListRemoveAllAsync<T>(string key)
        {
            this.ListRemoveAll<T>(key);
            return true;
        }

        // Token: 0x060001CF RID: 463 RVA: 0x0000E49C File Offset: 0x0000C69C
        public async Task<bool> ListRemoveAsync(string key)
        {
            return this.ListRemove(key);
        }

        // Token: 0x060001D0 RID: 464 RVA: 0x0000E4EC File Offset: 0x0000C6EC
        public async Task<List<T>> ListGetAllAsync<T>(string key)
        {
            return this.ListGetAll<T>(key);
        }

        // Token: 0x060001D1 RID: 465 RVA: 0x0000E53C File Offset: 0x0000C73C
        public async Task<List<T>> ListGetRangeAsync<T>(string key, int start, int count)
        {
            return this.ListGetRange<T>(key, start, count);
        }

        // Token: 0x060001D2 RID: 466 RVA: 0x0000E59C File Offset: 0x0000C79C
        public async Task<List<T>> ListGetRangeFromSortedListAsync<T>(string key, int start, int count, bool isAlpha = false, bool isDesc = false)
        {
            return this.ListGetRangeFromSortedList<T>(key, start, count, isAlpha, isDesc);
        }

        // Token: 0x060001D3 RID: 467 RVA: 0x0000E60C File Offset: 0x0000C80C
        public async Task<long> ListCountAsync(string key)
        {
            return this.ListCount(key);
        }

        // Token: 0x060001D4 RID: 468 RVA: 0x0000E65C File Offset: 0x0000C85C
        public async Task<bool> ListExistAsync(string key)
        {
            return this.ListExist(key);
        }

        // Token: 0x060001D5 RID: 469 RVA: 0x0000E6AC File Offset: 0x0000C8AC
        public async Task<bool> SetItemInListAsync<T>(string key, int index, T value)
        {
            this.SetItemInList<T>(key, index, value);
            return true;
        }

        // Token: 0x060001D6 RID: 470 RVA: 0x0000E70C File Offset: 0x0000C90C
        public async Task<bool> HashExistAsync(string key)
        {
            return this.HashExist(key);
        }

        // Token: 0x060001D7 RID: 471 RVA: 0x0000E75C File Offset: 0x0000C95C
        public async Task<bool> HashExistFieldAsync(string key, string dataKey)
        {
            return this.HashExistField(key, dataKey);
        }

        // Token: 0x060001D8 RID: 472 RVA: 0x0000E7B4 File Offset: 0x0000C9B4
        public async Task<bool> HashSetAsync<T>(string key, string dataKey, T value)
        {
            return this.HashSet<T>(key, dataKey, value);
        }

        // Token: 0x060001D9 RID: 473 RVA: 0x0000E814 File Offset: 0x0000CA14
        public async Task<bool> HashSetAsync<T>(string key, string dataKey, T value, TimeSpan expireIn)
        {
            return this.HashSet<T>(key, dataKey, value, expireIn);
        }

        // Token: 0x060001DA RID: 474 RVA: 0x0000E87C File Offset: 0x0000CA7C
        public async Task<bool> HashSetRangeAsync<T>(string key, IEnumerable<KeyValuePair<string, T>> keyValuePairs)
        {
            this.HashSetRange<T>(key, keyValuePairs);
            return true;
        }

        // Token: 0x060001DB RID: 475 RVA: 0x0000E8D4 File Offset: 0x0000CAD4
        public async Task<bool> HashSetRangeAsync<T>(string key, IEnumerable<KeyValuePair<string, T>> keyValuePairs, TimeSpan expireIn)
        {
            this.HashSetRange<T>(key, keyValuePairs, expireIn);
            return true;
        }

        // Token: 0x060001DC RID: 476 RVA: 0x0000E934 File Offset: 0x0000CB34
        public async Task<bool> HashRemoveFieldAsync(string key, string dataKey)
        {
            return this.HashRemoveField(key, dataKey);
        }

        // Token: 0x060001DD RID: 477 RVA: 0x0000E98C File Offset: 0x0000CB8C
        public async Task<bool> HashRemoveAsync(string key)
        {
            return this.HashRemove(key);
        }

        // Token: 0x060001DE RID: 478 RVA: 0x0000E9DC File Offset: 0x0000CBDC
        public async Task<T> HashGetAsync<T>(string key, string dataKey)
        {
            return this.HashGet<T>(key, dataKey);
        }

        // Token: 0x060001DF RID: 479 RVA: 0x0000EA34 File Offset: 0x0000CC34
        public async Task<List<T>> HashGetsAsync<T>(string key, params string[] dataKeys)
        {
            return this.HashGets<T>(key, dataKeys);
        }

        // Token: 0x060001E0 RID: 480 RVA: 0x0000EA8C File Offset: 0x0000CC8C
        public async Task<List<T>> HashGetAllAsync<T>(string key)
        {
            return this.HashGetAll<T>(key);
        }

        // Token: 0x060001E1 RID: 481 RVA: 0x0000EADC File Offset: 0x0000CCDC
        public async Task<List<string>> HashGetKeysAsync(string key)
        {
            return this.HashGetKeys(key);
        }

        // Token: 0x060001E2 RID: 482 RVA: 0x0000EB2C File Offset: 0x0000CD2C
        public async Task<Dictionary<string, T>> HashGetAllDictionaryAsync<T>(string key)
        {
            return this.HashGetAllDictionary<T>(key);
        }

        // Token: 0x060001E3 RID: 483 RVA: 0x0000EB7C File Offset: 0x0000CD7C
        public async Task<long> HashCountAsync(string key)
        {
            return this.HashCount(key);
        }

        // Token: 0x060001E4 RID: 484 RVA: 0x0000EBCC File Offset: 0x0000CDCC
        public async Task<long> HashIncrementValueAsync(string key, string dataKey, int increment)
        {
            return this.HashIncrementValue(key, dataKey, increment);
        }

        // Token: 0x060001E5 RID: 485 RVA: 0x0000EC2C File Offset: 0x0000CE2C
        public async Task<long> HashIncrementValueAsync(string key, string dataKey, int increment, TimeSpan expireIn)
        {
            return this.HashIncrementValue(key, dataKey, increment, expireIn);
        }

        // Token: 0x060001E6 RID: 486 RVA: 0x0000EC94 File Offset: 0x0000CE94
        public async Task<bool> SetAddAsync<T>(string key, T value)
        {
            this.SetAdd<T>(key, value);
            return true;
        }

        // Token: 0x060001E7 RID: 487 RVA: 0x0000ECEC File Offset: 0x0000CEEC
        public async Task<bool> SetAddAsync<T>(string key, T value, TimeSpan expireIn)
        {
            this.SetAdd<T>(key, value, expireIn);
            return true;
        }

        // Token: 0x060001E8 RID: 488 RVA: 0x0000ED4C File Offset: 0x0000CF4C
        public async Task<bool> SetAddRangeAsync<T>(string key, List<T> values)
        {
            this.SetAddRange<T>(key, values);
            return true;
        }

        // Token: 0x060001E9 RID: 489 RVA: 0x0000EDA4 File Offset: 0x0000CFA4
        public async Task<bool> SetAddRangeAsync<T>(string key, List<T> values, TimeSpan expireIn)
        {
            this.SetAddRange<T>(key, values, expireIn);
            return true;
        }

        // Token: 0x060001EA RID: 490 RVA: 0x0000EE04 File Offset: 0x0000D004
        public async Task<T> SetPopAsync<T>(string key)
        {
            return this.SetPop<T>(key);
        }

        // Token: 0x060001EB RID: 491 RVA: 0x0000EE54 File Offset: 0x0000D054
        public async Task<HashSet<T>> SetGetAllAsync<T>(string key)
        {
            return this.SetGetAll<T>(key);
        }

        // Token: 0x060001EC RID: 492 RVA: 0x0000EEA4 File Offset: 0x0000D0A4
        public async Task<bool> SetExistAsync(string key)
        {
            return this.SetExist(key);
        }

        // Token: 0x060001ED RID: 493 RVA: 0x0000EEF4 File Offset: 0x0000D0F4
        public async Task<bool> SetExistMemberAsync<T>(string key, T value)
        {
            return this.SetExistMember<T>(key, value);
        }

        // Token: 0x060001EE RID: 494 RVA: 0x0000EF4C File Offset: 0x0000D14C
        public async Task<bool> SetRemoveMemberAsync<T>(string key, T value)
        {
            this.SetRemoveMember<T>(key, value);
            return true;
        }

        // Token: 0x060001EF RID: 495 RVA: 0x0000EFA4 File Offset: 0x0000D1A4
        public async Task<bool> SetRemoveAsync(string key)
        {
            return this.SetRemove(key);
        }

        // Token: 0x060001F0 RID: 496 RVA: 0x0000EFF4 File Offset: 0x0000D1F4
        public async Task<long> SetCountAsync(string key)
        {
            return this.SetCount(key);
        }

        // Token: 0x060001F1 RID: 497 RVA: 0x0000F044 File Offset: 0x0000D244
        public async Task<bool> SortedSetAddAsync<T>(string key, T value, long score)
        {
            return this.SortedSetAdd<T>(key, value, score);
        }

        // Token: 0x060001F2 RID: 498 RVA: 0x0000F0A4 File Offset: 0x0000D2A4
        public async Task<bool> SortedSetAddAsync<T>(string key, T value, long score, TimeSpan expireIn)
        {
            return this.SortedSetAdd<T>(key, value, score, expireIn);
        }

        // Token: 0x060001F3 RID: 499 RVA: 0x0000F10C File Offset: 0x0000D30C
        public async Task<bool> SortedSetRemoveAsync<T>(string key, T value)
        {
            return this.SortedSetRemove<T>(key, value);
        }

        // Token: 0x060001F4 RID: 500 RVA: 0x0000F164 File Offset: 0x0000D364
        public async Task<List<T>> SortedSetGetAllAsync<T>(string key)
        {
            return this.SortedSetGetAll<T>(key);
        }

        // Token: 0x060001F5 RID: 501 RVA: 0x0000F1B4 File Offset: 0x0000D3B4
        public async Task<List<T>> SortedSetGetAllDescAsync<T>(string key)
        {
            return this.SortedSetGetAllDesc<T>(key);
        }

        // Token: 0x060001F6 RID: 502 RVA: 0x0000F204 File Offset: 0x0000D404
        public async Task<List<T>> SortedSetGetRangeByLowestScoreAsync<T>(string key, long fromScore, long toScore, int? skip = null, int? take = null)
        {
            return this.SortedSetGetRangeByLowestScore<T>(key, fromScore, toScore, skip, take);
        }

        // Token: 0x060001F7 RID: 503 RVA: 0x0000F274 File Offset: 0x0000D474
        public async Task<List<T>> SortedSetGetRangeByHighestScoreAsync<T>(string key, long fromScore, long toScore, int? skip = null, int? take = null)
        {
            return this.SortedSetGetRangeByHighestScore<T>(key, fromScore, toScore, skip, take);
        }

        // Token: 0x060001F8 RID: 504 RVA: 0x0000F2E4 File Offset: 0x0000D4E4
        public async Task<long> SortedSetCountAsync(string key, long fromScore, long toScore)
        {
            return this.SortedSetCount(key, fromScore, toScore);
        }

        // Token: 0x060001F9 RID: 505 RVA: 0x0000F344 File Offset: 0x0000D544
        public async Task<long> SortedSetRemoveRangeAsync(string key, int minRank, int maxRank)
        {
            return this.SortedSetRemoveRange(key, minRank, maxRank);
        }

        public async Task<long> SortedSetRemoveRangeByScoreAsync(string key, long fromScore, long toScore)
        {
            return this.SortedSetRemoveRangeByScore(key, fromScore, toScore);
        }

        public async Task<List<T>> SortedSetGetRangeDescAsync<T>(string key, int start, int count)
        {
            return this.SortedSetGetRangeDesc<T>(key, start, count);
        }

        public async Task<IDictionary<T, double>> SortedSetGetAllWithScoresAsync<T>(string key)
        {
            return this.SortedSetGetAllWithScores<T>(key);
        }

        public async Task<long> SortedSetGetIndexDescAsync<T>(string key, T value)
        {
            return this.SortedSetGetIndexDesc<T>(key, value);
        }

        public async Task<double> SortedSetIncrementAsync<T>(string key, T value, long increment)
        {
            return this.SortedSetIncrement<T>(key, value, increment);
        }

        public async Task<long> SortedSetUnionAsync(string intoKey, params string[] keys)
        {
            return this.SortedSetUnion(intoKey, keys);
        }
    }
}
