using System;
using System.IO;
using System.Data;
using System.Text;
using System.IO.Compression;

namespace Zk.HotelPlatform.Utils.Gzip
{
    public class GZipUtil
    {
		public static DataSet GetDatasetByString(string Value)
		{
			DataSet dataSet = new DataSet();
			StringReader reader = new StringReader(GZipUtil.GZipDecompressString(Value));
			dataSet.ReadXml(reader);
			return dataSet;
		}

		public static string GetStringByDataset(string ds)
		{
			return GZipUtil.GZipCompressString(ds);
		}

		public static string GZipCompressString(string rawString)
		{
			if (string.IsNullOrEmpty(rawString) || rawString.Length == 0)
			{
				return "";
			}
			return Convert.ToBase64String(GZipUtil.Compress(Encoding.UTF8.GetBytes(rawString.ToString())));
		}

		public static byte[] Compress(byte[] rawData)
		{
			MemoryStream memoryStream = new MemoryStream();
			GZipStream gzipStream = new GZipStream(memoryStream, CompressionMode.Compress, true);
			gzipStream.Write(rawData, 0, rawData.Length);
			gzipStream.Close();
			return memoryStream.ToArray();
		}

		public static string GZipDecompressString(string zippedString)
		{
			if (string.IsNullOrEmpty(zippedString) || zippedString.Length == 0)
			{
				return "";
			}
			byte[] zippedData = Convert.FromBase64String(zippedString.ToString());
			return Encoding.UTF8.GetString(GZipUtil.Decompress(zippedData));
		}

		public static byte[] Decompress(byte[] zippedData)
		{
			GZipStream gzipStream = new GZipStream(new MemoryStream(zippedData), CompressionMode.Decompress);
			MemoryStream memoryStream = new MemoryStream();
			byte[] array = new byte[1024];
			for (; ; )
			{
				int num = gzipStream.Read(array, 0, array.Length);
				if (num <= 0)
				{
					break;
				}
				memoryStream.Write(array, 0, num);
			}
			gzipStream.Close();
			return memoryStream.ToArray();
		}
	}
}
