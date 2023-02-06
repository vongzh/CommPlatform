using System;

namespace RedisClientAchieve
{
	internal class RedisDataInfo
	{
		/// <summary>
		/// 是否是新的Redis
		/// </summary>
		public bool IsNew { get; set; }

		/// <summary>
		/// 是否压缩值
		/// </summary>
		public bool IsCompress { get; set; }

		/// <summary>
		/// 压缩后的大小
		/// </summary>
		public double CompressSize { get; set; }

		/// <summary>
		/// 解压后或者原始大小
		/// </summary>
		public double Size { get; set; }
	}
}
