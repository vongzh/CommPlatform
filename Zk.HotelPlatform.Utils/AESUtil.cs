using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Zk.HotelPlatform.Utils
{
    public class AESUtil
    {

        public static  string DecryptString(string data,string  key )
        {
            byte[] keyArray = UTF8Encoding.UTF8.GetBytes( key);
            byte[] toEncryptArray = Convert.FromBase64String(data);
            RijndaelManaged rDel = new RijndaelManaged();
            rDel.Key = keyArray;
            rDel.Mode = CipherMode.ECB;
            rDel.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = rDel.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            return UTF8Encoding.UTF8.GetString(resultArray);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
        public static string AesEncrypt(string value, string key, string iv = "")
        {
            try
            {
                if (string.IsNullOrEmpty(value)) return string.Empty;
                if (key == null) throw new Exception("密钥不存在");
                if (key.Length < 16) throw new Exception("指定的密钥长度不能少于16位。");
                if (key.Length > 32) throw new Exception("指定的密钥长度不能多于32位。");
                if (key.Length != 16 && key.Length != 24 && key.Length != 32) throw new Exception("指定的密钥长度不明确。");
                if (!string.IsNullOrEmpty(iv))
                {
                    if (iv.Length < 16) throw new Exception("指定的向量长度不能少于16位。");
                }

                var _keyByte = Encoding.UTF8.GetBytes(key);
                var _valueByte = Encoding.UTF8.GetBytes(value);
                using (var aes = new RijndaelManaged())
                {
                    aes.IV = !string.IsNullOrEmpty(iv) ? Encoding.UTF8.GetBytes(iv) : Encoding.UTF8.GetBytes(key.Substring(0, 16));
                    aes.Key = _keyByte;
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;
                    var cryptoTransform = aes.CreateEncryptor();
                    var resultArray = cryptoTransform.TransformFinalBlock(_valueByte, 0, _valueByte.Length);
                    return Convert.ToBase64String(resultArray, 0, resultArray.Length);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("加密认证失败");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
        public static string AesDecrypt(string value, string key, string iv = "")
        {
            try
            {
                if (string.IsNullOrEmpty(value)) return string.Empty;
                if (key == null) throw new Exception("未将对象引用设置到对象的实例。");
                if (key.Length < 16) throw new Exception("指定的密钥长度不能少于16位。");
                if (key.Length > 32) throw new Exception("指定的密钥长度不能多于32位。");
                if (key.Length != 16 && key.Length != 24 && key.Length != 32) throw new Exception("指定的密钥长度不明确。");
                if (!string.IsNullOrEmpty(iv))
                {
                    if (iv.Length < 16) throw new Exception("指定的向量长度不能少于16位。");
                }

                var _keyByte = Encoding.UTF8.GetBytes(key);
                var _valueByte = Convert.FromBase64String(value);
                using (var aes = new RijndaelManaged())
                {
                    aes.IV = !string.IsNullOrEmpty(iv) ? Encoding.UTF8.GetBytes(iv) : Encoding.UTF8.GetBytes(key.Substring(0, 16));
                    aes.Key = _keyByte;
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;
                    var cryptoTransform = aes.CreateDecryptor();
                    var resultArray = cryptoTransform.TransformFinalBlock(_valueByte, 0, _valueByte.Length);
                    return Encoding.UTF8.GetString(resultArray);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("加密认证失败");
            }
        }
        public  static string Decrypt(string toDecrypt, string key)
        {
            if (string.IsNullOrEmpty(toDecrypt)) return null;
            Byte[] toEncryptArray = Convert.FromBase64String(toDecrypt);

            System.Security.Cryptography.RijndaelManaged rm = new System.Security.Cryptography.RijndaelManaged
            {
                Key = Encoding.UTF8.GetBytes(key),
                IV = Encoding.UTF8.GetBytes(key),
                Mode = System.Security.Cryptography.CipherMode.ECB,
                Padding = System.Security.Cryptography.PaddingMode.PKCS7
            };

            System.Security.Cryptography.ICryptoTransform cTransform = rm.CreateDecryptor();
            Byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            return Encoding.UTF8.GetString(resultArray);
        }
        /// <summary>
        /// AES解密  对应java中的 aes/cbc/pkcs5padding 模式的算法
        /// </summary>
        /// <param name="s">待解密的字符串</param>
        /// <param name="sKey">密钥</param>
        /// <returns>返回空为解析失败</returns>
        public static string AesDecrypt1(string str, string key, string IV)
        {
            RijndaelManaged rijndaelCipher = new RijndaelManaged();
            rijndaelCipher.Mode = CipherMode.ECB;
            rijndaelCipher.Padding = PaddingMode.PKCS7;
            rijndaelCipher.KeySize = 128;
            rijndaelCipher.BlockSize = 128;
            byte[] encryptedData = Convert.FromBase64String(str);
            byte[] pwdBytes = System.Text.Encoding.UTF8.GetBytes(key);
            byte[] keyBytes = new byte[16];
            int len = pwdBytes.Length;
            if (len > keyBytes.Length)
                len = keyBytes.Length;
            System.Array.Copy(pwdBytes, keyBytes, len);
            rijndaelCipher.Key = keyBytes;
            rijndaelCipher.IV = Encoding.UTF8.GetBytes(IV);
            ICryptoTransform transform = rijndaelCipher.CreateDecryptor();
            byte[] plainText = transform.TransformFinalBlock(encryptedData, 0, encryptedData.Length);
            return Encoding.UTF8.GetString(plainText);
        }

        public static string Encrypt(string toEncrypt, string key, string iv)
        {
            byte[] keyArray = UTF8Encoding.UTF8.GetBytes(key);
            byte[] ivArray = UTF8Encoding.UTF8.GetBytes(iv);
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);
            RijndaelManaged rDel = new RijndaelManaged();
            rDel.Key = keyArray;
            rDel.IV = ivArray;
            rDel.Mode = CipherMode.ECB;
            rDel.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = rDel.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }
        public static string AesDecrypt2(string str, string key)
        {
            if (string.IsNullOrEmpty(str)) return null;
            Byte[] toEncryptArray = Convert.FromBase64String(str);

            RijndaelManaged rm = new RijndaelManaged
            {
                Key = Encoding.UTF8.GetBytes(key),
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };

            ICryptoTransform cTransform = rm.CreateDecryptor();
            Byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            return Encoding.UTF8.GetString(resultArray);
        }
        public static string AesEncrypt1(string str, string key)
        {
            if (string.IsNullOrEmpty(str)) return null;
            Byte[] toEncryptArray = Encoding.UTF8.GetBytes(str);

            System.Security.Cryptography.RijndaelManaged rm = new System.Security.Cryptography.RijndaelManaged
            {
                Key = Encoding.UTF8.GetBytes(key),
                Mode = System.Security.Cryptography.CipherMode.ECB,
                Padding = System.Security.Cryptography.PaddingMode.PKCS7
            };

            System.Security.Cryptography.ICryptoTransform cTransform = rm.CreateEncryptor();
            Byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }
        public static string AESEncrypt3(String Data, String Key)
        {
            MemoryStream mStream = new MemoryStream();
	RijndaelManaged aes = new RijndaelManaged();
	byte[] plainBytes= Encoding.UTF8.GetBytes(Data);
	Byte[] bKey = new Byte[32];
	Array.Copy(Encoding.UTF8.GetBytes(Key.PadRight(bKey.Length)), bKey, bKey.Length);

            aes.Mode = CipherMode.ECB;
	aes.Padding = PaddingMode.PKCS7;
	aes.KeySize =128;
	//aes.Key &#61; _key;  
	aes.Key = bKey;
	//aes.IV &#61; _iV;  
	CryptoStream cryptoStream = new CryptoStream(mStream, aes.CreateEncryptor(), CryptoStreamMode.Write);
	try
            {
                cryptoStream.Write(plainBytes, 0, plainBytes.Length);
                cryptoStream.FlushFinalBlock();
                return Convert.ToBase64String(mStream.ToArray());
            }
            finally
            {
                cryptoStream.Close();
                mStream.Close();
                aes.Clear();
            }
        }

        public static string AesDecrypt3(string str, string key)
        {
            if (string.IsNullOrEmpty(str)) return null;
            str=decryptUrlSafe(str);
              Byte[] toEncryptArray = Convert.FromBase64String(str);

            System.Security.Cryptography.RijndaelManaged rm = new System.Security.Cryptography.RijndaelManaged
            {
                Key = Encoding.UTF8.GetBytes(key),
                Mode = System.Security.Cryptography.CipherMode.ECB,
                Padding = System.Security.Cryptography.PaddingMode.PKCS7
            };

            System.Security.Cryptography.ICryptoTransform cTransform = rm.CreateDecryptor();
            Byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            return Encoding.UTF8.GetString(resultArray);
        }
        public static string decryptUrlSafe(string key )
        {
            string decodeStr = key.Replace("-", "+").Replace("_", "/"); ;
            string qualsStr = "";
            if ((key.Length % 4) != 0)
            {
                for (int i = 0; i < 4 - (key.Length % 4); i++)
                {
                    qualsStr += "=";
                }
            }
            return decodeStr + qualsStr;
        }
        public static string Encrypt4(string toEncrypt, string key)
        {
            byte[] keyArray = UTF8Encoding.UTF8.GetBytes(key);
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);

            RijndaelManaged rDel = new RijndaelManaged();
            rDel.Key = keyArray;
            rDel.Mode = CipherMode.ECB;
            rDel.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = rDel.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            return Convert.ToBase64String(resultArray, 0, resultArray.Length).Replace("\\+","-").Replace("/","_").Replace("=","");
        }
    }
}
