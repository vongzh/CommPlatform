using Bert.RateLimiters;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zk.HotelPlatform.Utils
{
	/// <summary>
	/// 限速工具类
	/// </summary>
	public class RateLimitersUtils
	{
		private static IDictionary<string, FixedTokenBucket> dict = new ConcurrentDictionary<string, FixedTokenBucket>();

		private static IReadOnlyList<int> FACTOR_ARR { get; } = new int[] { 5, 3, 2, 2 };
		private static readonly object _objLock = new object();

		static RateLimitersUtils()
		{
		}
		public static bool ShouldThrottle(string bucketKey, long apiLimiterPerMin = 6000)
		{

			FixedTokenBucket bucket;
			if (dict.ContainsKey(bucketKey))
			{
				bucket = dict[bucketKey];
				if (null != bucket)
				{
					return bucket.ShouldThrottle(1);
				}
			}
			lock (_objLock)
			{
				bucket = GetFixedTokenBucket(apiLimiterPerMin);
				dict[bucketKey] = bucket;
			}
			bool result = bucket.ShouldThrottle(1);

			return result;
		}
		private static FixedTokenBucket GetFixedTokenBucket(long apiLimiterPerMin)
		{
			int factor = 1;
			long temp = apiLimiterPerMin;

			foreach (int fac in FACTOR_ARR)
			{
				if (temp % fac != 0)
				{
					continue;
				}
				temp = temp / fac;
				factor = factor * fac;
			}

			long maxToken = apiLimiterPerMin / factor;
			long refillIntervalMilliseconds = 60 / factor;

			return new FixedTokenBucket(maxToken <= 0 ? 1 : maxToken, 1, refillIntervalMilliseconds * 1000);
		}

		public static FixedTokenBucket Bucket { get; set; }
	}
}
