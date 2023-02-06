using RedisClientAchieve.Units;
using System;

namespace RedisClientAchieve
{
	public class RedisEntities
	{
		private static IRedisEntity _default;
		private static IRedisEntity _data;

		private static object _defaultLockObj = new object();
		private static object _dataLockObj = new object();

		public static IRedisEntity Default
		{
			get
			{
				if (RedisEntities._default == null)
				{
					lock (_defaultLockObj)
					{
						if (RedisEntities._default == null)
						{
							RedisEntities._default = new RedisEntityAchieve(RedisConnectionSource.DefaultConn);
						}
					}
				}
				return RedisEntities._default;
			}
		}

		public static IRedisEntity Data
        {
			get
			{
				if (RedisEntities._data == null)
				{
					lock (_dataLockObj)
					{
						if (RedisEntities._data == null)
						{
							RedisEntities._data = new RedisEntityAchieve(RedisConnectionSource.DataConn);
						}
					}
				}
				return RedisEntities._data;
			}
		}
	}
}
