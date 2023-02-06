using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;

namespace RedisClientAchieve
{
    public class ByteHexHelper
    {
        private static byte[] _buffedBytes;

        private static char[] _buffedChars = new char[512];

        public static byte[] ObjectToBytes(object obj)
        {
            byte[] buffer;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                ((IFormatter)new BinaryFormatter()).Serialize(memoryStream, obj);
                buffer = memoryStream.GetBuffer();
            }
            return buffer;
        }

        public static T BytesToObject<T>(byte[] Bytes)
        {
            T result;
            using (MemoryStream memoryStream = new MemoryStream(Bytes))
            {
                result = (T)((object)((IFormatter)new BinaryFormatter()).Deserialize(memoryStream));
            }
            return result;
        }

        public static string ByteToString16(byte[] bytearray)
        {
            string text = "";
            for (int i = 0; i < bytearray.Length; i++)
            {
                string text2 = Convert.ToString(bytearray[i], 16);
                if (text2.Length < 2)
                {
                    text += 0;
                }
                text += text2;
            }
            return text;
        }

        public static byte[] StringToByte16(string hexString)
        {
            byte[] array = new byte[hexString.Length / 2];
            for (int i = 0; i < array.Length; i++)
            {
                string value = hexString.Substring(i * 2, 2);
                array[i] = Convert.ToByte(value, 16);
            }
            return array;
        }

        public static byte[] SignData(byte[] key, byte[] data, HMAC alg)
        {
            alg.Key = key;
            return alg.ComputeHash(data).ToArray<byte>();
        }

        static ByteHexHelper()
        {
            int num = 0;
            byte b = 0;
            for (; ; )
            {
                string text = b.ToString("X2");
                ByteHexHelper._buffedChars[num++] = text[0];
                ByteHexHelper._buffedChars[num++] = text[1];
                if (b == 255)
                {
                    break;
                }
                b += 1;
            }
            ByteHexHelper._buffedBytes = new byte[103];
            num = ByteHexHelper._buffedBytes.Length;
            while (--num >= 0)
            {
                if (48 <= num && num <= 57)
                {
                    ByteHexHelper._buffedBytes[num] = (byte)(num - 48);
                }
                else if (97 <= num && num <= 102)
                {
                    ByteHexHelper._buffedBytes[num] = (byte)(num - 97 + 10);
                }
                else if (65 <= num && num <= 70)
                {
                    ByteHexHelper._buffedBytes[num] = (byte)(num - 65 + 10);
                }
            }
        }

        /// <summary>
        /// 字节数组转16进制字符串
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string ByteToHex(byte[] bytes)
        {
            if (bytes == null)
            {
                return null;
            }
            char[] array = new char[bytes.Length * 2];
            for (int i = 0; i < bytes.Length; i++)
            {
                int num = (int)(bytes[i] * 2);
                array[i * 2] = ByteHexHelper._buffedChars[num];
                array[i * 2 + 1] = ByteHexHelper._buffedChars[num + 1];
            }
            return new string(array);
        }

        /// <summary>
        /// 16进制字符串转字节数组
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static byte[] HexToByte(string str)
        {
            if (str == null || (str.Length & 1) == 1)
            {
                return null;
            }
            byte[] array = new byte[str.Length / 2];
            int num = 0;
            int num2 = 0;
            int num3 = array.Length;
            while (--num3 >= 0)
            {
                int num4 = 0;
                int num5 = 0;
                try
                {
                    num4 = (int)ByteHexHelper._buffedBytes[(int)str[num++]];
                    num5 = (int)ByteHexHelper._buffedBytes[(int)str[num++]];
                }
                catch
                {
                    return null;
                }
                array[num2++] = (byte)((num4 << 4) + num5);
            }
            return array;
        }
    }
}
