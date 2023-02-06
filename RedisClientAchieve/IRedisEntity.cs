using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ServiceStack.Redis;

namespace RedisClientAchieve
{
    public interface IRedisEntity
    {
        /// <summary>
        /// 获取并发锁，直到超时
        /// </summary>
        /// <param name="key">key </param>
        /// <param name="timeOut">获取锁的超时时间</param>
        /// <returns>IDisposable</returns>
        IDisposable AcquireLock(string key, TimeSpan? timeOut = null);

        /// <summary>
        /// 获取并发锁，并立刻返回获取是否成功
        /// </summary>
        /// <param name="key"></param>
        /// <returns>成功返回True 失败返回False</returns>
        bool AcquireLockImmediate(string key, TimeSpan? timeOut = null);

        /// <summary>
        /// 立即释放并发锁
        /// </summary>
        /// <param name="key"></param>
        /// <returns>成功返回True 失败返回False</returns>
        void ReleaseLockImmediate(string key);

        /// <summary>
        /// 根据传入的key移除对应得数据
        /// </summary>
        /// <param name="keys">待移除数据的key</param>
        /// <returns>移除成功返回true 移除失败返回false </returns>
        bool RemoveEntry(params string[] keys);

        /// <summary>
        /// 设置key的数据在指定的时长后过期
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="expireIn">过期时长</param>
        /// <returns>设置成功返回true 设置失败返回false </returns>
        bool ExpireEntryIn(string key, TimeSpan expireIn);

        /// <summary>
        /// 设置指定key的数据在指定时刻过期
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="expiresAt">过期时刻</param>
        /// <returns>设置成功返回true 设置失败返回false </returns>
        bool ExpireEntryAt(string key, DateTime expiresAt);

        /// <summary>
        /// 以秒为单位，返回给定key的剩余生存时间
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>时间（秒）</returns>
        TimeSpan? GetTimeToLive(string key);

        /// <summary>
        /// 返回key所储存的值的类型
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>RedisKeyType</returns>
        RedisKeyType GetKeyType(string key);

        /// <summary>
        /// 检查给定 key 是否存在
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>存在返回true 不存在返回false</returns>
        bool ContainsKey(string key);

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
        // Token: 0x0600007E RID: 126
        long SetBit(string key, int offset, int value);

        /// <summary>
        /// 对key所储存的字符串值，获取指定偏移量上的位(bit)。
        /// 当 offset 比字符串值的长度大，或者key不存在时，返回 0 
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="offset">偏移量</param>
        /// <returns>字符串值指定偏移量上的位(bit)</returns>
        // Token: 0x0600007F RID: 127
        long GetBit(string key, int offset);

        /// <summary>
        /// 获取key所储存的字符值内被设置为 1 的比特位的数量。
        /// 对一个不存在的key进行 BITCOUNT 操作，结果为 0
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>被设置为 1 的位的数量</returns>
        // Token: 0x06000080 RID: 128
        long BitCount(string key);

        // Token: 0x06000081 RID: 129
        long BitCount(string intoKey, params string[] keys);

        /// <summary>
        /// 为指定的key赋值 value,key不存在则先新建key然后在对key赋值
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        /// <returns>赋值成功返回true 赋值失败返回false </returns>
        // Token: 0x06000082 RID: 130
        bool ItemSet<T>(string key, T value);

        /// <summary>
        /// 为指定的key赋值 value,key不存在则先新建key然后在对key赋值
        /// 同时必须指定key的过期时间
        /// 注意：每次设置都会重新更新过期时间
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        /// <param name="expireIn">过期时间</param>
        /// <returns>赋值成功返回true 赋值失败返回false </returns>
        // Token: 0x06000083 RID: 131
        bool ItemSet<T>(string key, T value, TimeSpan expireIn);

        /// <summary>
        /// 批量为 多个key 赋各自对应的值 value；key不存在 则先新建key然后在对key赋值
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="values">待设置的key：value 集合</param>
        // Token: 0x06000084 RID: 132
        void ItemSetAll<T>(IDictionary<string, T> values);

        /// <summary>
        /// 获取key对应的值，key不存在则返回对应值的类型的默认值 -- default(T)
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="key">key</param>
        /// <returns>T</returns>
        // Token: 0x06000085 RID: 133
        T ItemGet<T>(string key);

        /// <summary>
        /// 将给定 key 的值设为 value ，并返回 key 的旧值(old value)
        /// key不存在则返回对应值的类型的默认值 -- default(T)
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="value">新的要设定的value</param>
        /// <returns>T</returns>
        // Token: 0x06000086 RID: 134
        T ItemGetSet<T>(string key, T value);

        /// <summary>
        /// 将给定 key 的值设为 value ，并返回 key 的旧值(old value)
        /// key不存在则返回对应值的类型的默认值 -- default(T)
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="value">新的要设定的value</param>
        /// <param name="expireIn">过期时间</param>
        /// <returns>T</returns>
        // Token: 0x06000087 RID: 135
        T ItemGetSet<T>(string key, T value, TimeSpan expireIn);

        /// <summary>
        /// 获取多个Key各自对应得值，返回key：value的对应集合
        /// key不存在则对应值的类型的默认值 -- default(T)
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="keys">keys</param>
        /// <returns>IDictionary  key：value的对应集合</returns>
        // Token: 0x06000088 RID: 136
        IDictionary<string, T> ItemGetAll<T>(IEnumerable<string> keys);

        /// <summary>
        /// 移除key对应的数据
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>移除成功返回true 移除失败返回false</returns>
        // Token: 0x06000089 RID: 137
        bool ItemRemove(string key);

        /// <summary>
        /// 移除多个key各自对应的数据
        /// </summary>
        /// <param name="keys">keys</param>
        // Token: 0x0600008A RID: 138
        void ItemRemoveAll(IEnumerable<string> keys);

        /// <summary>
        /// 将key中储存的数字值增加增量 increment。
        /// 如果key不存在，那么key的值会先被初始化为 0 ，然后再执行增值操作
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="increment">增加的数值</param>
        /// <returns>加上 increment 之后， key 的值</returns>
        // Token: 0x0600008B RID: 139
        long ItemIncrement(string key, uint increment);

        /// <summary>
        /// 将key中储存的数字值增加增量 increment。
        /// 如果key不存在，那么key的值会先被初始化为 0 ，然后再执行增值操作
        /// 同时必须指定key的过期时间
        /// 注意：每次设置都会重新更新过期时间
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="increment">增加的数值</param>
        /// <param name="expireIn">过期时间</param>
        /// <returns>加上 increment 之后， key 的值</returns>
        // Token: 0x0600008C RID: 140
        long ItemIncrement(string key, uint increment, TimeSpan expireIn);

        /// <summary>
        /// 将 key 所储存的值减去减量 decrement 。
        /// 如果 key 不存在，那么 key 的值会先被初始化为 0 ，然后再执行减法操作
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="decrement">减去的数值</param>
        /// <returns>减去 decrement 之后， key 的值</returns>
        // Token: 0x0600008D RID: 141
        long ItemDecrement(string key, uint decrement);

        /// <summary>
        /// 将 key 所储存的值减去减量 decrement 。
        /// 如果 key 不存在，那么 key 的值会先被初始化为 0 ，然后再执行减法操作
        /// 同时必须指定key的过期时间
        /// 注意：每次设置都会重新更新过期时间
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="decrement">减去的数值</param>
        /// <param name="expireIn">过期时间</param>
        /// <returns>减去 decrement 之后， key 的值</returns>
        // Token: 0x0600008E RID: 142
        long ItemDecrement(string key, uint decrement, TimeSpan expireIn);

        /// <summary>
        /// 检查给定 key的Item 是否存在
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>存在返回true 不存在返回false</returns>
        // Token: 0x0600008F RID: 143
        bool ItemExist(string key);

        /// <summary>
        /// 将一个value 插入到列表 key 的列表头(最左边)
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        // Token: 0x06000090 RID: 144
        void ListAddToHead<T>(string key, T value);

        /// <summary>
        /// 将一个value 插入到列表 key 的列表头(最左边)
        /// 同时必须指定key的过期时间
        /// 注意：每次设置都会重新更新过期时间
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        /// <param name="expireIn">过期时间</param>
        // Token: 0x06000091 RID: 145
        void ListAddToHead<T>(string key, T value, TimeSpan expireIn);

        /// <summary>
        /// 将多个value 插入到列表 key 的列表头(最左边)
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="values">value的集合</param>
        // Token: 0x06000092 RID: 146
        void ListAddRangeToHead<T>(string key, List<T> values);

        /// <summary>
        /// 将多个value 插入到列表 key 的列表头(最左边)
        /// 同时必须指定key的过期时间
        /// 注意：每次设置都会重新更新过期时间
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="values">value的集合</param>
        /// <param name="expireIn">过期时间</param>
        // Token: 0x06000093 RID: 147
        void ListAddRangeToHead<T>(string key, List<T> values, TimeSpan expireIn);

        /// <summary>
        /// 将一个value 插入到列表 key 的列表尾(最右边)
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        // Token: 0x06000094 RID: 148
        void ListAddToEnd<T>(string key, T value);

        /// <summary>
        /// 将一个value 插入到列表 key 的列表尾(最右边)
        /// 同时必须指定key的过期时间
        /// 注意：每次设置都会重新更新过期时间
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        /// <param name="expireIn">过期时间</param>
        // Token: 0x06000095 RID: 149
        void ListAddToEnd<T>(string key, T value, TimeSpan expireIn);

        /// <summary>
        /// 将多个value 插入到列表 key 的列表尾(最右边)
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="values">value的集合</param>
        // Token: 0x06000096 RID: 150
        void ListAddRangeToEnd<T>(string key, List<T> values);

        /// <summary>
        /// 将多个value 插入到列表 key 的列表尾(最右边)
        /// 同时必须指定key的过期时间
        /// 注意：每次设置都会重新更新过期时间
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="values">value的集合</param>
        /// <param name="expireIn">过期时间</param>
        // Token: 0x06000097 RID: 151
        void ListAddRangeToEnd<T>(string key, List<T> values, TimeSpan expireIn);

        /// <summary>
        /// 移除列表 key中与 value 相等的元素
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        /// <returns>移除成功返回true 移除失败返回false</returns>
        // Token: 0x06000098 RID: 152
        bool ListRemoveItem<T>(string key, T value);

        /// <summary>
        /// 移除并返回列表 key 的列表头元素（最左边）
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="key">key</param>
        /// <returns>T</returns>
        // Token: 0x06000099 RID: 153
        T ListRemoveHead<T>(string key);

        /// <summary>
        /// 移除并返回列表 key 的列表尾元素（最右边）
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="key">key</param>
        /// <returns>T</returns>
        // Token: 0x0600009A RID: 154
        T ListRemoveEnd<T>(string key);

        /// <summary>
        /// 移除列表key中 除了从 keepStartingFrom 到 keepEndingAt的元素之外的其他所有元素
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="keepStartingFrom"></param>
        /// <param name="keepEndingAt"></param>
        // Token: 0x0600009B RID: 155
        void ListRemoveTrim(string key, int keepStartingFrom, int keepEndingAt);

        /// <summary>
        /// 移除列表 key内的所有元素
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="key">key</param>
        // Token: 0x0600009C RID: 156
        void ListRemoveAll<T>(string key);

        /// <summary>
        /// 根据传入列表的key移除整个列表
        /// </summary>
        /// <param name="key">待移除列表的key</param>
        /// <returns>移除成功返回true 移除失败返回false </returns>
        // Token: 0x0600009D RID: 157
        bool ListRemove(string key);

        /// <summary>
        /// 获取列表key内的所有元素
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="key">key</param>
        /// <returns>T的实体集合</returns>
        // Token: 0x0600009E RID: 158
        List<T> ListGetAll<T>(string key);

        /// <summary>
        /// 获取列表key中从第start个元素起长度为count的元素集合
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="start">待获取元素的起始索引值</param>
        /// <param name="count">待获取元素的个数</param>
        /// <returns>T的实体集合</returns>
        // Token: 0x0600009F RID: 159
        List<T> ListGetRange<T>(string key, int start, int count);

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
        // Token: 0x060000A0 RID: 160
        List<T> ListGetRangeFromSortedList<T>(string key, int start, int count, bool isAlpha = false, bool isDesc = false);

        /// <summary>
        /// 获取列表Key的元素个数
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>元素个数</returns>
        // Token: 0x060000A1 RID: 161
        long ListCount(string key);

        /// <summary>
        /// 检查给定 key的列表 是否存在
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>存在返回true 不存在返回false</returns>
        // Token: 0x060000A2 RID: 162
        bool ListExist(string key);

        /// <summary>
        /// 将列表 key 下标为 index 的元素的值设置为 value 
        /// 当 index超出列表的范围 或者列表为空 则报错
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">key</param>
        /// <param name="index">要替换的元素下标</param>
        /// <param name="value">要替换的值</param>
        // Token: 0x060000A3 RID: 163
        void SetItemInList<T>(string key, int index, T value);

        /// <summary>
        /// 检查给定 key的哈希表 是否存在
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>存在返回true 不存在返回false</returns>
        // Token: 0x060000A4 RID: 164
        bool HashExist(string key);

        /// <summary>
        /// 查看哈希表 key 中，给定域 dataKey 是否存在
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dataKey">dataKey</param>
        /// <returns>存在返回true 不存在返回false</returns>
        // Token: 0x060000A5 RID: 165
        bool HashExistField(string key, string dataKey);

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
        // Token: 0x060000A6 RID: 166
        bool HashSet<T>(string key, string dataKey, T value);

        /// <summary>
        /// 将哈希表 key 中的域 dataKey 的值设为 value
        /// 如果 key 不存在，一个新的哈希表被创建并进行 HashSet 操作
        /// 如果域 dataKey 已经存在于哈希表中，旧值将被覆盖
        /// 同时必须指定key的过期时间
        /// 注意：每次设置成功（即：返回true）都会重新更新过期时间
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="dataKey">dataKey</param>
        /// <param name="value">value</param>
        /// <param name="expireIn">过期时间</param>
        /// <returns>设置成功返回true 设置失败返回false</returns>
        // Token: 0x060000A7 RID: 167
        bool HashSet<T>(string key, string dataKey, T value, TimeSpan expireIn);

        /// <summary>
        /// 同时将多个 datakey-value (域-值)对设置到哈希表 key 中
        /// 如果 key 不存在，一个新的哈希表被创建并进行 HashSetRange 操作
        /// 如果域 dataKey 已经存在于哈希表中，旧值将被覆盖
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="keyValuePairs">dataKey：value的集合</param>
        // Token: 0x060000A8 RID: 168
        void HashSetRange<T>(string key, IEnumerable<KeyValuePair<string, T>> keyValuePairs);

        /// <summary>
        /// 同时将多个 datakey-value (域-值)对设置到哈希表 key 中
        /// 如果 key 不存在，一个新的哈希表被创建并进行 HashSetRange 操作
        /// 如果域 dataKey 已经存在于哈希表中，旧值将被覆盖
        /// 同时必须指定key的过期时间
        /// 注意：每次设置都会重新更新过期时间
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="keyValuePairs">dataKey：value的集合</param>
        /// <param name="expireIn">过期时间</param>
        // Token: 0x060000A9 RID: 169
        void HashSetRange<T>(string key, IEnumerable<KeyValuePair<string, T>> keyValuePairs, TimeSpan expireIn);

        /// <summary>
        /// 删除哈希表 key 中的指定域 dataKey，不存在的域将被忽略
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dataKey">dataKey</param>
        /// <returns>删除成功返回true 删除失败返回false</returns>
        // Token: 0x060000AA RID: 170
        bool HashRemoveField(string key, string dataKey);

        /// <summary>
        /// 删除哈希表 key 内所有的域
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>删除成功返回true 删除失败返回false</returns>
        // Token: 0x060000AB RID: 171
        bool HashRemove(string key);

        /// <summary>
        /// 获取哈希表 key 中给定域 dataKey 的值
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="dataKey">dataKey</param>
        /// <returns>T</returns>
        // Token: 0x060000AC RID: 172
        T HashGet<T>(string key, string dataKey);

        /// <summary>
        /// 获取哈希表 key 中，一个或多个给定域的值
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="dataKeys">dataKeys</param>
        /// <returns>T的实体集合</returns>
        // Token: 0x060000AD RID: 173
        List<T> HashGets<T>(string key, params string[] dataKeys);

        /// <summary>
        /// 获取哈希表 key 中所有域的值
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="key">key</param>
        /// <returns>T的实体集合</returns>
        // Token: 0x060000AE RID: 174
        List<T> HashGetAll<T>(string key);

        /// <summary>
        /// 获取哈希表 key 中的所有域的键
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>域的键集合</returns>
        // Token: 0x060000AF RID: 175
        List<string> HashGetKeys(string key);

        /// <summary>
        /// 获取哈希表 key 中所有域的值,以Dictionary的字典类型返回
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="key">key</param>
        /// <returns>Dictionary</returns>
        // Token: 0x060000B0 RID: 176
        Dictionary<string, T> HashGetAllDictionary<T>(string key);

        /// <summary>
        /// 获取哈希表 key 中域的数量
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>域的数量</returns>
        // Token: 0x060000B1 RID: 177
        long HashCount(string key);

        /// <summary>
        /// 为哈希表 key 中的域 dataKey 的值加上增量 increment
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dataKey"></param>
        /// <param name="increment">增量</param>
        /// <returns>加上 increment 之后， key 的值哈希表 key 中域 dataKey 的值</returns>
        // Token: 0x060000B2 RID: 178
        long HashIncrementValue(string key, string dataKey, int increment);

        /// <summary>
        /// 为哈希表 key 中的域 dataKey 的值加上增量 increment
        /// 同时必须指定key的过期时间
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dataKey"></param>
        /// <param name="increment">增量</param>
        /// <param name="expireIn">过期时间</param>
        /// <returns>加上 increment 之后， key 的值哈希表 key 中域 dataKey 的值</returns>
        // Token: 0x060000B3 RID: 179
        long HashIncrementValue(string key, string dataKey, int increment, TimeSpan expireIn);

        /// <summary>
        /// 将一个value 元素加入到无序集合 key 当中，已经存在于无序集合的 value 元素将被忽略
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        // Token: 0x060000B4 RID: 180
        void SetAdd<T>(string key, T value);

        /// <summary>
        /// 将一个value 元素加入到无序集合 key 当中，已经存在于无序集合的 value 元素将被忽略
        /// 同时必须指定key的过期时间
        /// 注意：每次设置都会重新更新过期时间
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        /// <param name="expireIn">过期时间</param>
        // Token: 0x060000B5 RID: 181
        void SetAdd<T>(string key, T value, TimeSpan expireIn);

        /// <summary>
        /// 将多个value 元素加入到无序集合 key 当中，已经存在于无序集合的 value 元素将被忽略
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="values">value的集合</param>
        // Token: 0x060000B6 RID: 182
        void SetAddRange<T>(string key, List<T> values);

        /// <summary>
        /// 将多个value 元素加入到无序集合 key 当中，已经存在于无序集合的 value 元素将被忽略
        /// 同时必须指定key的过期时间
        /// 注意：每次设置都会重新更新过期时间
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="values">value的集合</param>
        /// <param name="expireIn">过期时间</param>
        // Token: 0x060000B7 RID: 183
        void SetAddRange<T>(string key, List<T> values, TimeSpan expireIn);

        /// <summary>
        /// 移除并返回无序集合 key 中的一个随机元素
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="key">key</param>
        /// <returns>T</returns>
        // Token: 0x060000B8 RID: 184
        T SetPop<T>(string key);

        /// <summary>
        /// 获取无序集合 key 中的所有成员,以HashSet的类型返回结果
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="key">key</param>
        /// <returns>HashSet</returns>
        // Token: 0x060000B9 RID: 185
        HashSet<T> SetGetAll<T>(string key);

        /// <summary>
        /// 检查给定 key的无序集合 是否存在
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>存在返回true 不存在返回false</returns>
        // Token: 0x060000BA RID: 186
        bool SetExist(string key);

        /// <summary>
        /// 判断无序集合 key 的成员内是否存在 value 元素
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        /// <returns>存在返回true 不存在返回false</returns>
        // Token: 0x060000BB RID: 187
        bool SetExistMember<T>(string key, T value);

        /// <summary>
        /// 移除无序集合 key 中的 value 元素
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        // Token: 0x060000BC RID: 188
        void SetRemoveMember<T>(string key, T value);

        /// <summary>
        /// 根据传入无序集合的key移除整个无序集合
        /// </summary>
        /// <param name="keys">待移除无序集合的key</param>
        /// <returns>移除成功返回true 移除失败返回false </returns>
        // Token: 0x060000BD RID: 189
        bool SetRemove(string key);

        /// <summary>
        /// 获取无序集合key中元素的数量
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>集合的元素数量</returns>
        // Token: 0x060000BE RID: 190
        long SetCount(string key);

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
        // Token: 0x060000BF RID: 191
        bool SortedSetAdd<T>(string key, T value, long score);

        /// <summary>
        /// 将 value 元素及其排序值 score 加入到有序集 key 当中。
        /// 如果某个 value 已经是有序集的成员，那么更新这个 member 的 score 值，并通过重新插入这个 value 元素，来保证该 value 在正确的位置上
        /// key 不存在，则创建一个空的有序集并执行 SortedSetAdd 操作
        /// 同时必须指定key的过期时间
        /// 注意：每次设置成功（即：返回true）都会重新更新过期时间
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        /// <param name="score">value的排序值</param>
        /// <param name="expireIn">过期时间</param>
        /// <returns>设置成功返回true 设置失败返回false</returns>
        // Token: 0x060000C0 RID: 192
        bool SortedSetAdd<T>(string key, T value, long score, TimeSpan expireIn);

        /// <summary>
        /// 将value元素 从有序集key移除
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        /// <returns>移除成功返回true 移除失败返回false</returns>
        // Token: 0x060000C1 RID: 193
        bool SortedSetRemove<T>(string key, T value);

        /// <summary>
        /// 获取有序集key中的所有元素
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="key">key</param>
        /// <returns>T的实体集合</returns>
        // Token: 0x060000C2 RID: 194
        List<T> SortedSetGetAll<T>(string key);

        /// <summary>
        /// 获取有序集key中的所有元素
        /// 其中成员的位置按 score 值递减(从大到小)来排列。
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="key">key</param>
        /// <returns>T的实体集合</returns>
        // Token: 0x060000C3 RID: 195
        List<T> SortedSetGetAllDesc<T>(string key);

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
        // Token: 0x060000C4 RID: 196
        List<T> SortedSetGetRangeByLowestScore<T>(string key, long fromScore, long toScore, int? skip = null, int? take = null);

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
        // Token: 0x060000C5 RID: 197
        List<T> SortedSetGetRangeByHighestScore<T>(string key, long fromScore, long toScore, int? skip = null, int? take = null);

        /// <summary>
        /// 获取有序集 key 中，score 值介于 fromScore 和 toScore 之间(包括等于 fromScore 或 toScore )的成员数量
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="fromScore">起始score值</param>
        /// <param name="toScore">结束score值</param>
        /// <returns>成员数量</returns>
        // Token: 0x060000C6 RID: 198
        long SortedSetCount(string key, long fromScore, long toScore);

        /// <summary>
        /// 移除有序集 key 中，指定排名(rank)区间内的所有成员，区间分别以下标参数 minRank 和 maxRank 指出，包含 minRank 和 maxRank 在内
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="minRank">区间起始排名</param>
        /// <param name="maxRank">区间结束排名</param>
        /// <returns>被移除的元素数量</returns>
        // Token: 0x060000C7 RID: 199
        long SortedSetRemoveRange(string key, int minRank, int maxRank);

        /// <summary>
        /// 移除有序集 key 中，所有 score 值介于 fromScore 和 toScore 之间(包括等于 fromScore 或 toScore )的成员
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="fromScore">起始score值</param>
        /// <param name="toScore">结束score值</param>
        /// <returns>被移除的元素数量</returns>
        // Token: 0x060000C8 RID: 200
        long SortedSetRemoveRangeByScore(string key, long fromScore, long toScore);

        /// <summary>
        /// 获取有序集 key 中，指定区间内的成员,其中成员的位置按 score 值递减(从大到小)来排列
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="start">区间的启示成员索引值</param>
        /// <param name="count">区间成员的长度</param>
        /// <returns>T的实体集合</returns>
        // Token: 0x060000C9 RID: 201
        List<T> SortedSetGetRangeDesc<T>(string key, int start, int count);

        /// <summary>
        /// 获取有序集key内所有成员的值以及排序值，以IDictionary的字典类型返回（Key：成员值，Value：成员的排序值）
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="key">key</param>
        /// <returns>IDictionary</returns>
        // Token: 0x060000CA RID: 202
        IDictionary<T, double> SortedSetGetAllWithScores<T>(string key);

        /// <summary>
        /// 获取有序集key中,value的索引值
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        /// <returns>value的索引值</returns>
        // Token: 0x060000CB RID: 203
        long SortedSetGetIndexDesc<T>(string key, T value);

        /// <summary>
        /// 为有序集 key 的成员 value 的 score 值加上增量 increment 返回增量后的score
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        /// <param name="increment">增量值</param>
        /// <returns>增加 increment后的score值</returns>
        // Token: 0x060000CC RID: 204
        double SortedSetIncrement<T>(string key, T value, long increment);

        /// <summary>
        /// 计算给定的一个或多个有序集的并集，并将该并集(结果集)储存到 intoKey
        /// </summary>
        /// <param name="intoKey">结果集的Key</param>
        /// <param name="keys">待合并的keys</param>
        /// <returns>结果集中成员的数量</returns>
        // Token: 0x060000CD RID: 205
        long SortedSetUnion(string intoKey, params string[] keys);

        /// <summary>
        /// 根据传入的key移除对应得数据
        /// </summary>
        /// <param name="keys">待移除数据的key</param>
        /// <returns>移除成功返回true 移除失败返回false </returns>
        // Token: 0x060000CE RID: 206
        Task<bool> RemoveEntryAsync(params string[] keys);

        /// <summary>
        /// 设置key的数据在指定的时长后过期
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="expireIn">过期时长</param>
        /// <returns>设置成功返回true 设置失败返回false </returns>
        // Token: 0x060000CF RID: 207
        Task<bool> ExpireEntryInAsync(string key, TimeSpan expireIn);

        /// <summary>
        /// 设置指定key的数据在指定时刻过期
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="expiresAt">过期时刻</param>
        /// <returns>设置成功返回true 设置失败返回false </returns>
        // Token: 0x060000D0 RID: 208
        Task<bool> ExpireEntryAtAsync(string key, DateTime expiresAt);

        /// <summary>
        /// 以秒为单位，返回给定key的剩余生存时间
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>时间（秒）</returns>
        // Token: 0x060000D1 RID: 209
        Task<TimeSpan?> GetTimeToLiveAsync(string key);

        /// <summary>
        /// 返回key所储存的值的类型
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>RedisKeyType</returns>
        // Token: 0x060000D2 RID: 210
        Task<RedisKeyType> GetKeyTypeAsync(string key);

        /// <summary>
        /// 检查给定 key 是否存在
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>存在返回true 不存在返回false</returns>
        // Token: 0x060000D3 RID: 211
        Task<bool> ContainsKeyAsync(string key);

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
        // Token: 0x060000D4 RID: 212
        Task<long> SetBitAsync(string key, int offset, int value);

        /// <summary>
        /// 对key所储存的字符串值，获取指定偏移量上的位(bit)。
        /// 当 offset 比字符串值的长度大，或者key不存在时，返回 0 
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="offset">偏移量</param>
        /// <returns>字符串值指定偏移量上的位(bit)</returns>
        // Token: 0x060000D5 RID: 213
        Task<long> GetBitAsync(string key, int offset);

        /// <summary>
        /// 获取key所储存的字符值内被设置为 1 的比特位的数量。
        /// 对一个不存在的key进行 BITCOUNT 操作，结果为 0
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>被设置为 1 的位的数量</returns>
        // Token: 0x060000D6 RID: 214
        Task<long> BitCountAsync(string key);

        // Token: 0x060000D7 RID: 215
        Task<long> BitCountAsync(string intoKey, params string[] keys);

        /// <summary>
        /// 为指定的key赋值 value,key不存在则先新建key然后在对key赋值
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        /// <returns>赋值成功返回true 赋值失败返回false </returns>
        // Token: 0x060000D8 RID: 216
        Task<bool> ItemSetAsync<T>(string key, T value);

        /// <summary>
        /// 为指定的key赋值 value,key不存在则先新建key然后在对key赋值
        /// 同时必须指定key的过期时间
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        /// <param name="expireIn">过期时间</param>
        /// <returns>赋值成功返回true 赋值失败返回false </returns>
        // Token: 0x060000D9 RID: 217
        Task<bool> ItemSetAsync<T>(string key, T value, TimeSpan expireIn);

        /// <summary>
        /// 批量为 多个key 赋各自对应的值 value；key不存在 则先新建key然后在对key赋值
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="values">待设置的key：value 集合</param>
        // Token: 0x060000DA RID: 218
        Task<bool> ItemSetAllAsync<T>(IDictionary<string, T> values);

        /// <summary>
        /// 获取key对应的值，key不存在则返回对应值的类型的默认值 -- default(T)
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="key">key</param>
        /// <returns>T</returns>
        // Token: 0x060000DB RID: 219
        Task<T> ItemGetAsync<T>(string key);

        /// <summary>
        /// 将给定 key 的值设为 value ，并返回 key 的旧值(old value)
        /// key不存在则返回对应值的类型的默认值 -- default(T)
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="value">新的要设定的value</param>
        /// <returns>T</returns>
        // Token: 0x060000DC RID: 220
        Task<T> ItemGetSetAsync<T>(string key, T value);

        /// <summary>
        /// 将给定 key 的值设为 value ，并返回 key 的旧值(old value)
        /// key不存在则返回对应值的类型的默认值 -- default(T)
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="value">新的要设定的value</param>
        /// <param name="expireIn">过期时间</param>
        /// <returns>T</returns>
        // Token: 0x060000DD RID: 221
        Task<T> ItemGetSetAsync<T>(string key, T value, TimeSpan expireIn);

        /// <summary>
        /// 获取多个Key各自对应得值，返回key：value的对应集合
        /// key不存在则对应值的类型的默认值 -- default(T)
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="keys">keys</param>
        /// <returns>IDictionary  key：value的对应集合</returns>
        // Token: 0x060000DE RID: 222
        Task<IDictionary<string, T>> ItemGetAllAsync<T>(IEnumerable<string> keys);

        /// <summary>
        /// 移除key对应的数据
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>移除成功返回true 移除失败返回false</returns>
        // Token: 0x060000DF RID: 223
        Task<bool> ItemRemoveAsync(string key);

        /// <summary>
        /// 移除多个key各自对应的数据
        /// </summary>
        /// <param name="keys">keys</param>
        // Token: 0x060000E0 RID: 224
        Task<bool> ItemRemoveAllAsync(IEnumerable<string> keys);

        /// <summary>
        /// 将key中储存的数字值增加增量 increment。
        /// 如果key不存在，那么key的值会先被初始化为 0 ，然后再执行增值操作
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="increment">增加的数值</param>
        /// <returns>加上 increment 之后， key 的值</returns>
        // Token: 0x060000E1 RID: 225
        Task<long> ItemIncrementAsync(string key, uint increment);

        /// <summary>
        /// 将key中储存的数字值增加增量 increment。
        /// 如果key不存在，那么key的值会先被初始化为 0 ，然后再执行增值操作
        /// 同时必须指定key的过期时间
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="increment">增加的数值</param>
        /// <param name="expireIn">过期时间</param>
        /// <returns>加上 increment 之后， key 的值</returns>
        // Token: 0x060000E2 RID: 226
        Task<long> ItemIncrementAsync(string key, uint increment, TimeSpan expireIn);

        /// <summary>
        /// 将 key 所储存的值减去减量 decrement 。
        /// 如果 key 不存在，那么 key 的值会先被初始化为 0 ，然后再执行减法操作
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="decrement">减去的数值</param>
        /// <returns>减去 decrement 之后， key 的值</returns>
        // Token: 0x060000E3 RID: 227
        Task<long> ItemDecrementAsync(string key, uint decrement);

        /// <summary>
        /// 将 key 所储存的值减去减量 decrement 。
        /// 如果 key 不存在，那么 key 的值会先被初始化为 0 ，然后再执行减法操作
        /// 同时必须指定key的过期时间
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="decrement">减去的数值</param>
        /// <param name="expireIn">过期时间</param>
        /// <returns>减去 decrement 之后， key 的值</returns>
        // Token: 0x060000E4 RID: 228
        Task<long> ItemDecrementAsync(string key, uint decrement, TimeSpan expireIn);

        /// <summary>
        /// 检查给定 key的Item 是否存在
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>存在返回true 不存在返回false</returns>
        // Token: 0x060000E5 RID: 229
        Task<bool> ItemExistAsync(string key);

        /// <summary>
        /// 将一个value 插入到列表 key 的列表头(最左边)
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        // Token: 0x060000E6 RID: 230
        Task<bool> ListAddToHeadAsync<T>(string key, T value);

        /// <summary>
        /// 将一个value 插入到列表 key 的列表头(最左边)
        /// 同时必须指定key的过期时间
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        /// <param name="expireIn">过期时间</param>
        // Token: 0x060000E7 RID: 231
        Task<bool> ListAddToHeadAsync<T>(string key, T value, TimeSpan expireIn);

        /// <summary>
        /// 将多个value 插入到列表 key 的列表头(最左边)
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="values">value的集合</param>
        // Token: 0x060000E8 RID: 232
        Task<bool> ListAddRangeToHeadAsync<T>(string key, List<T> values);

        /// <summary>
        /// 将多个value 插入到列表 key 的列表头(最左边)
        /// 同时必须指定key的过期时间
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="values">value的集合</param>
        /// <param name="expireIn">过期时间</param>
        // Token: 0x060000E9 RID: 233
        Task<bool> ListAddRangeToHeadAsync<T>(string key, List<T> values, TimeSpan expireIn);

        /// <summary>
        /// 将一个value 插入到列表 key 的列表尾(最右边)
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        // Token: 0x060000EA RID: 234
        Task<bool> ListAddToEndAsync<T>(string key, T value);

        /// <summary>
        /// 将一个value 插入到列表 key 的列表尾(最右边)
        /// 同时必须指定key的过期时间
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        /// <param name="expireIn">过期时间</param>
        // Token: 0x060000EB RID: 235
        Task<bool> ListAddToEndAsync<T>(string key, T value, TimeSpan expireIn);

        /// <summary>
        /// 将多个value 插入到列表 key 的列表尾(最右边)
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="values">value的集合</param>
        // Token: 0x060000EC RID: 236
        Task<bool> ListAddRangeToEndAsync<T>(string key, List<T> values);

        /// <summary>
        /// 将多个value 插入到列表 key 的列表尾(最右边)
        /// 同时必须指定key的过期时间
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="values">value的集合</param>
        /// <param name="expireIn">过期时间</param>
        // Token: 0x060000ED RID: 237
        Task<bool> ListAddRangeToEndAsync<T>(string key, List<T> values, TimeSpan expireIn);

        /// <summary>
        /// 移除列表 key中与 value 相等的元素
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        /// <returns>移除成功返回true 移除失败返回false</returns>
        // Token: 0x060000EE RID: 238
        Task<bool> ListRemoveItemAsync<T>(string key, T value);

        /// <summary>
        /// 移除并返回列表 key 的列表头元素（最左边）
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="key">key</param>
        /// <returns>T</returns>
        // Token: 0x060000EF RID: 239
        Task<T> ListRemoveHeadAsync<T>(string key);

        /// <summary>
        /// 移除并返回列表 key 的列表尾元素（最右边）
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="key">key</param>
        /// <returns>T</returns>
        // Token: 0x060000F0 RID: 240
        Task<T> ListRemoveEndAsync<T>(string key);

        /// <summary>
        /// 移除列表key中 除了从 keepStartingFrom 到 keepEndingAt的元素之外的其他所有元素
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="keepStartingFrom"></param>
        /// <param name="keepEndingAt"></param>
        // Token: 0x060000F1 RID: 241
        Task<bool> ListRemoveTrimAsync(string key, int keepStartingFrom, int keepEndingAt);

        /// <summary>
        /// 移除列表 key内的所有元素
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="key">key</param>
        // Token: 0x060000F2 RID: 242
        Task<bool> ListRemoveAllAsync<T>(string key);

        /// <summary>
        /// 根据传入列表的key移除整个列表
        /// </summary>
        /// <param name="keys">待移除列表的key</param>
        /// <returns>移除成功返回true 移除失败返回false </returns>
        // Token: 0x060000F3 RID: 243
        Task<bool> ListRemoveAsync(string key);

        /// <summary>
        /// 获取列表key内的所有元素
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="key">key</param>
        /// <returns>T的实体集合</returns>
        // Token: 0x060000F4 RID: 244
        Task<List<T>> ListGetAllAsync<T>(string key);

        /// <summary>
        /// 获取列表key中从第start个元素起长度为count的元素集合
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="start">待获取元素的起始索引值</param>
        /// <param name="count">待获取元素的个数</param>
        /// <returns>T的实体集合</returns>
        // Token: 0x060000F5 RID: 245
        Task<List<T>> ListGetRangeAsync<T>(string key, int start, int count);

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
        // Token: 0x060000F6 RID: 246
        Task<List<T>> ListGetRangeFromSortedListAsync<T>(string key, int start, int count, bool isAlpha = false, bool isDesc = false);

        /// <summary>
        /// 获取列表Key的元素个数
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>元素个数</returns>
        // Token: 0x060000F7 RID: 247
        Task<long> ListCountAsync(string key);

        /// <summary>
        /// 检查给定 key的列表 是否存在
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>存在返回true 不存在返回false</returns>
        // Token: 0x060000F8 RID: 248
        Task<bool> ListExistAsync(string key);

        /// <summary>
        /// 将列表 key 下标为 index 的元素的值设置为 value 
        /// 当 index超出列表的范围 或者列表为空 则报错
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">key</param>
        /// <param name="index">要替换的元素下标</param>
        /// <param name="value">要替换的值</param>
        // Token: 0x060000F9 RID: 249
        Task<bool> SetItemInListAsync<T>(string key, int index, T value);

        /// <summary>
        /// 检查给定 key的哈希表 是否存在
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>存在返回true 不存在返回false</returns>
        // Token: 0x060000FA RID: 250
        Task<bool> HashExistAsync(string key);

        /// <summary>
        /// 查看哈希表 key 中，给定域 dataKey 是否存在
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dataKey">dataKey</param>
        /// <returns>存在返回true 不存在返回false</returns>
        // Token: 0x060000FB RID: 251
        Task<bool> HashExistFieldAsync(string key, string dataKey);

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
        // Token: 0x060000FC RID: 252
        Task<bool> HashSetAsync<T>(string key, string dataKey, T value);

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
        // Token: 0x060000FD RID: 253
        Task<bool> HashSetAsync<T>(string key, string dataKey, T value, TimeSpan expireIn);

        /// <summary>
        /// 同时将多个 datakey-value (域-值)对设置到哈希表 key 中
        /// 如果 key 不存在，一个新的哈希表被创建并进行 HashSetRange 操作
        /// 如果域 dataKey 已经存在于哈希表中，旧值将被覆盖
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="keyValuePairs">dataKey：value的集合</param>
        // Token: 0x060000FE RID: 254
        Task<bool> HashSetRangeAsync<T>(string key, IEnumerable<KeyValuePair<string, T>> keyValuePairs);

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
        // Token: 0x060000FF RID: 255
        Task<bool> HashSetRangeAsync<T>(string key, IEnumerable<KeyValuePair<string, T>> keyValuePairs, TimeSpan expireIn);

        /// <summary>
        /// 删除哈希表 key 中的指定域 dataKey，不存在的域将被忽略
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dataKey">dataKey</param>
        /// <returns>删除成功返回true 删除失败返回false</returns>
        // Token: 0x06000100 RID: 256
        Task<bool> HashRemoveFieldAsync(string key, string dataKey);

        /// <summary>
        /// 删除哈希表 key 内所有的域
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>删除成功返回true 删除失败返回false</returns>
        // Token: 0x06000101 RID: 257
        Task<bool> HashRemoveAsync(string key);

        /// <summary>
        /// 获取哈希表 key 中给定域 dataKey 的值
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="dataKey">dataKey</param>
        /// <returns>T</returns>
        // Token: 0x06000102 RID: 258
        Task<T> HashGetAsync<T>(string key, string dataKey);

        /// <summary>
        /// 获取哈希表 key 中，一个或多个给定域的值
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="dataKeys">dataKeys</param>
        /// <returns>T的实体集合</returns>
        // Token: 0x06000103 RID: 259
        Task<List<T>> HashGetsAsync<T>(string key, params string[] dataKeys);

        /// <summary>
        /// 获取哈希表 key 中所有域的值
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="key">key</param>
        /// <returns>T的实体集合</returns>
        // Token: 0x06000104 RID: 260
        Task<List<T>> HashGetAllAsync<T>(string key);

        /// <summary>
        /// 获取哈希表 key 中的所有域的键
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>域的键集合</returns>
        // Token: 0x06000105 RID: 261
        Task<List<string>> HashGetKeysAsync(string key);

        /// <summary>
        /// 获取哈希表 key 中所有域的值,以Dictionary的字典类型返回
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="key">key</param>
        /// <returns>Dictionary</returns>
        // Token: 0x06000106 RID: 262
        Task<Dictionary<string, T>> HashGetAllDictionaryAsync<T>(string key);

        /// <summary>
        /// 获取哈希表 key 中域的数量
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>域的数量</returns>
        // Token: 0x06000107 RID: 263
        Task<long> HashCountAsync(string key);

        /// <summary>
        /// 为哈希表 key 中的域 dataKey 的值加上增量 increment
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dataKey"></param>
        /// <param name="increment">增量</param>
        /// <returns>加上 increment 之后， key 的值哈希表 key 中域 dataKey 的值</returns>
        // Token: 0x06000108 RID: 264
        Task<long> HashIncrementValueAsync(string key, string dataKey, int increment);

        /// <summary>
        /// 为哈希表 key 中的域 dataKey 的值加上增量 increment
        /// 同时必须指定key的过期时间
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dataKey"></param>
        /// <param name="increment">增量</param>
        /// <param name="expireIn">过期时间</param>
        /// <returns>加上 increment 之后， key 的值哈希表 key 中域 dataKey 的值</returns>
        // Token: 0x06000109 RID: 265
        Task<long> HashIncrementValueAsync(string key, string dataKey, int increment, TimeSpan expireIn);

        /// <summary>
        /// 将一个value 元素加入到无序集合 key 当中，已经存在于无序集合的 value 元素将被忽略
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        // Token: 0x0600010A RID: 266
        Task<bool> SetAddAsync<T>(string key, T value);

        /// <summary>
        /// 将一个value 元素加入到无序集合 key 当中，已经存在于无序集合的 value 元素将被忽略
        /// 同时必须指定key的过期时间
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        /// <param name="expireIn">过期时间</param>
        // Token: 0x0600010B RID: 267
        Task<bool> SetAddAsync<T>(string key, T value, TimeSpan expireIn);

        /// <summary>
        /// 将多个value 元素加入到无序集合 key 当中，已经存在于无序集合的 value 元素将被忽略
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="values">value的集合</param>
        // Token: 0x0600010C RID: 268
        Task<bool> SetAddRangeAsync<T>(string key, List<T> values);

        /// <summary>
        /// 将多个value 元素加入到无序集合 key 当中，已经存在于无序集合的 value 元素将被忽略
        /// 同时必须指定key的过期时间
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="values">value的集合</param>
        /// <param name="expireIn">过期时间</param>
        // Token: 0x0600010D RID: 269
        Task<bool> SetAddRangeAsync<T>(string key, List<T> values, TimeSpan expireIn);

        /// <summary>
        /// 移除并返回无序集合 key 中的一个随机元素
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="key">key</param>
        /// <returns>T</returns>
        // Token: 0x0600010E RID: 270
        Task<T> SetPopAsync<T>(string key);

        /// <summary>
        /// 获取无序集合 key 中的所有成员,以HashSet的类型返回结果
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="key">key</param>
        /// <returns>HashSet</returns>
        // Token: 0x0600010F RID: 271
        Task<HashSet<T>> SetGetAllAsync<T>(string key);

        /// <summary>
        /// 检查给定 key的无序集合 是否存在
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>存在返回true 不存在返回false</returns>
        // Token: 0x06000110 RID: 272
        Task<bool> SetExistAsync(string key);

        /// <summary>
        /// 判断无序集合 key 的成员内是否存在 value 元素
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        /// <returns>存在返回true 不存在返回false</returns>
        // Token: 0x06000111 RID: 273
        Task<bool> SetExistMemberAsync<T>(string key, T value);

        /// <summary>
        /// 移除无序集合 key 中的 value 元素
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        // Token: 0x06000112 RID: 274
        Task<bool> SetRemoveMemberAsync<T>(string key, T value);

        /// <summary>
        /// 根据传入无序集合的key移除整个无序集合
        /// </summary>
        /// <param name="keys">待移除无序集合的key</param>
        /// <returns>移除成功返回true 移除失败返回false </returns>
        // Token: 0x06000113 RID: 275
        Task<bool> SetRemoveAsync(string key);

        /// <summary>
        /// 获取无序集合key中元素的数量
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>集合的元素数量</returns>
        // Token: 0x06000114 RID: 276
        Task<long> SetCountAsync(string key);

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
        // Token: 0x06000115 RID: 277
        Task<bool> SortedSetAddAsync<T>(string key, T value, long score);

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
        // Token: 0x06000116 RID: 278
        Task<bool> SortedSetAddAsync<T>(string key, T value, long score, TimeSpan expireIn);

        /// <summary>
        /// 将value元素 从有序集key移除
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        /// <returns>移除成功返回true 移除失败返回false</returns>
        // Token: 0x06000117 RID: 279
        Task<bool> SortedSetRemoveAsync<T>(string key, T value);

        /// <summary>
        /// 获取有序集key中的所有元素
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="key">key</param>
        /// <returns>T的实体集合</returns>
        // Token: 0x06000118 RID: 280
        Task<List<T>> SortedSetGetAllAsync<T>(string key);

        /// <summary>
        /// 获取有序集key中的所有元素
        /// 其中成员的位置按 score 值递减(从大到小)来排列。
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="key">key</param>
        /// <returns>T的实体集合</returns>
        // Token: 0x06000119 RID: 281
        Task<List<T>> SortedSetGetAllDescAsync<T>(string key);

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
        // Token: 0x0600011A RID: 282
        Task<List<T>> SortedSetGetRangeByLowestScoreAsync<T>(string key, long fromScore, long toScore, int? skip = null, int? take = null);

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
        // Token: 0x0600011B RID: 283
        Task<List<T>> SortedSetGetRangeByHighestScoreAsync<T>(string key, long fromScore, long toScore, int? skip = null, int? take = null);

        /// <summary>
        /// 获取有序集 key 中，score 值介于 fromScore 和 toScore 之间(包括等于 fromScore 或 toScore )的成员数量
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="fromScore">起始score值</param>
        /// <param name="toScore">结束score值</param>
        /// <returns>成员数量</returns>
        // Token: 0x0600011C RID: 284
        Task<long> SortedSetCountAsync(string key, long fromScore, long toScore);

        /// <summary>
        /// 移除有序集 key 中，指定排名(rank)区间内的所有成员，区间分别以下标参数 minRank 和 maxRank 指出，包含 minRank 和 maxRank 在内
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="minRank">区间起始排名</param>
        /// <param name="maxRank">区间结束排名</param>
        /// <returns>被移除的元素数量</returns>
        // Token: 0x0600011D RID: 285
        Task<long> SortedSetRemoveRangeAsync(string key, int minRank, int maxRank);

        /// <summary>
        /// 移除有序集 key 中，所有 score 值介于 fromScore 和 toScore 之间(包括等于 fromScore 或 toScore )的成员
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="fromScore">起始score值</param>
        /// <param name="toScore">结束score值</param>
        /// <returns>被移除的元素数量</returns>
        // Token: 0x0600011E RID: 286
        Task<long> SortedSetRemoveRangeByScoreAsync(string key, long fromScore, long toScore);

        /// <summary>
        /// 获取有序集 key 中，指定区间内的成员,其中成员的位置按 score 值递减(从大到小)来排列
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="start">区间的启示成员索引值</param>
        /// <param name="count">区间成员的长度</param>
        /// <returns>T的实体集合</returns>
        // Token: 0x0600011F RID: 287
        Task<List<T>> SortedSetGetRangeDescAsync<T>(string key, int start, int count);

        /// <summary>
        /// 获取有序集key内所有成员的值以及排序值，以IDictionary的字典类型返回（Key：成员值，Value：成员的排序值）
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="key">key</param>
        /// <returns>IDictionary</returns>
        // Token: 0x06000120 RID: 288
        Task<IDictionary<T, double>> SortedSetGetAllWithScoresAsync<T>(string key);

        /// <summary>
        /// 获取有序集key中,value的索引值
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        /// <returns>value的索引值</returns>
        // Token: 0x06000121 RID: 289
        Task<long> SortedSetGetIndexDescAsync<T>(string key, T value);

        /// <summary>
        /// 为有序集 key 的成员 value 的 score 值加上增量 increment 返回增量后的score
        /// </summary>
        /// <typeparam name="T">value的类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        /// <param name="increment">增量值</param>
        /// <returns>增加 increment后的score值</returns>
        // Token: 0x06000122 RID: 290
        Task<double> SortedSetIncrementAsync<T>(string key, T value, long increment);

        /// <summary>
        /// 计算给定的一个或多个有序集的并集，并将该并集(结果集)储存到 intoKey
        /// </summary>
        /// <param name="intoKey">结果集的Key</param>
        /// <param name="keys">待合并的keys</param>
        /// <returns>结果集中成员的数量</returns>
        // Token: 0x06000123 RID: 291
        Task<long> SortedSetUnionAsync(string intoKey, params string[] keys);
    }
}
