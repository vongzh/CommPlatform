using RedisClientAchieve.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisClientAchieve
{
	public class RedisMsgPackEntities
	{
		private static IRedisEntity _default;
		private static IRedisEntity _data;

		private static object _defaultLockObj = new object();
		private static object _dataLockObj = new object();

		public static IRedisEntity Default
		{
			get
			{
				if (_default == null)
				{
					object defaultLockObj = _defaultLockObj;
					lock (defaultLockObj)
					{
						if (_default == null)
						{
							_default = new RedisEntityAchieve(RedisConnectionSource.DefaultConn, SerializeMode.MsgPack);
						}
					}
				}
				return _default;
			}
		}

		public static IRedisEntity Data
		{
			get
			{
				if (_data == null)
				{
					lock (_dataLockObj)
					{
						if (_data == null)
						{
							_data = new RedisEntityAchieve(RedisConnectionSource.DataConn, SerializeMode.MsgPack);
						}
					}
				}
				return _data;
			}
		}
	}
}
