using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;
using Zk.HotelPlatform.Utils.Log;

namespace Zk.HotelPlatform.Utils
{
    /// <summary>
    /// The sign helper.
    /// </summary>
    public class SignHelper
    {
        /// <summary>
        /// 加签
        /// </summary>
        /// <param name="privateKey">XML格式的私钥</param>
        /// <param name="content">待加签参数字典内容</param>
        /// <returns></returns>
        public static string CreateRSA2Sign(string privateKey, string content)
        {
            try
            {
                byte[] bytValue = Encoding.UTF8.GetBytes(content);
                SHA256 sha256 = new SHA256CryptoServiceProvider();
                byte[] retVal = sha256.ComputeHash(bytValue);
                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                rsa.FromXmlString(privateKey);
                RSAPKCS1SignatureFormatter formatter = new RSAPKCS1SignatureFormatter(rsa);
                formatter.SetHashAlgorithm("SHA256");
                byte[] inArray = formatter.CreateSignature(retVal);
                return Convert.ToBase64String(inArray);
            }
            catch (Exception ex)
            {
                return default;
            }
        }

        /// <summary>
        /// 加签
        /// </summary>
        /// <param name="privateKey">XML格式的私钥</param>
        /// <param name="paras">待加签参数字典</param>
        /// <returns></returns>
        public static string CreateSign(string privateKey, string content)
        {
            try
            {
                // var content = getSignContent(paras);
                byte[] bytValue = Encoding.UTF8.GetBytes(content);
                SHA256 sha256 = new SHA256CryptoServiceProvider();
                byte[] retVal = sha256.ComputeHash(bytValue);

                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                rsa.FromXmlString(privateKey);

                RSAPKCS1SignatureFormatter formatter = new RSAPKCS1SignatureFormatter(rsa);
                formatter.SetHashAlgorithm("SHA256");
                byte[] inArray = formatter.CreateSignature(retVal);

                return Convert.ToBase64String(inArray);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 排序拼接待加签参数
        /// </summary>
        /// <param name="paras"></param>
        /// <returns></returns>
        public static string getSignContent(Dictionary<string, string> paras)
        {
            if (paras == null)
            {
                return string.Empty;
            }
            var orderParas = paras.OrderBy(p => p.Key);
            StringBuilder contentSB = new StringBuilder();
            foreach (var para in orderParas)
            {
                if (!para.Key.IsNullOrEmpty() && !para.Value.IsNullOrEmpty())
                {
                    if (!para.Key.ToLower().StartsWith("sign"))
                    {
                        contentSB.Append($"&{para.Key}={para.Value}");
                    }

                }
            }
            return contentSB.ToString().TrimStart('&');
        }

        /// <summary>
        /// 私钥转换
        /// </summary>
        /// <param name="privateKey">Pkcs8->xml</param>
        /// <returns></returns>
        public static string PrivateKeyPkcs8ToXml(string privateKey)
        {
            RsaPrivateCrtKeyParameters privateKeyParam =
                (RsaPrivateCrtKeyParameters)PrivateKeyFactory.CreateKey(Convert.FromBase64String(privateKey));

            XElement privatElement = new XElement("RSAKeyValue");
            //Modulus
            XElement primodulus = new XElement("Modulus", Convert.ToBase64String(privateKeyParam.Modulus.ToByteArrayUnsigned()));
            //Exponent
            XElement priexponent = new XElement("Exponent", Convert.ToBase64String(privateKeyParam.PublicExponent.ToByteArrayUnsigned()));
            //P
            XElement prip = new XElement("P", Convert.ToBase64String(privateKeyParam.P.ToByteArrayUnsigned()));
            //Q
            XElement priq = new XElement("Q", Convert.ToBase64String(privateKeyParam.Q.ToByteArrayUnsigned()));
            //DP
            XElement pridp = new XElement("DP", Convert.ToBase64String(privateKeyParam.DP.ToByteArrayUnsigned()));
            //DQ
            XElement pridq = new XElement("DQ", Convert.ToBase64String(privateKeyParam.DQ.ToByteArrayUnsigned()));
            //InverseQ
            XElement priinverseQ = new XElement("InverseQ", Convert.ToBase64String(privateKeyParam.QInv.ToByteArrayUnsigned()));
            //D
            XElement prid = new XElement("D", Convert.ToBase64String(privateKeyParam.Exponent.ToByteArrayUnsigned()));
            privatElement.Add(primodulus);
            privatElement.Add(priexponent);
            privatElement.Add(prip);
            privatElement.Add(priq);
            privatElement.Add(pridp);
            privatElement.Add(pridq);
            privatElement.Add(priinverseQ);
            privatElement.Add(prid);
            return privatElement.ToString();


        }

        /// <summary>  
        /// PEM格式私钥转换成XML格式私钥
        /// </summary>  
        /// <param name="privateKey">PEM格式私钥，（纯文字字符，注意不带-----BEGIN PRIVATE KEY-----和-----END PRIVATE KEY-----）</param>  
        /// <returns></returns>  
        public static string RSAPrivateKeyToXml(string privateKey)
        {
            RsaPrivateCrtKeyParameters privateKeyParam = (RsaPrivateCrtKeyParameters)PrivateKeyFactory.CreateKey(Convert.FromBase64String(privateKey));

            return string.Format("<RSAKeyValue><Modulus>{0}</Modulus><Exponent>{1}</Exponent><P>{2}</P><Q>{3}</Q><DP>{4}</DP><DQ>{5}</DQ><InverseQ>{6}</InverseQ><D>{7}</D></RSAKeyValue>",
                Convert.ToBase64String(privateKeyParam.Modulus.ToByteArrayUnsigned()),
                Convert.ToBase64String(privateKeyParam.PublicExponent.ToByteArrayUnsigned()),
                Convert.ToBase64String(privateKeyParam.P.ToByteArrayUnsigned()),
                Convert.ToBase64String(privateKeyParam.Q.ToByteArrayUnsigned()),
                Convert.ToBase64String(privateKeyParam.DP.ToByteArrayUnsigned()),
                Convert.ToBase64String(privateKeyParam.DQ.ToByteArrayUnsigned()),
                Convert.ToBase64String(privateKeyParam.QInv.ToByteArrayUnsigned()),
                Convert.ToBase64String(privateKeyParam.Exponent.ToByteArrayUnsigned()));
        }

        /// <summary>
        /// 验签
        /// </summary>
        /// <param name="sign">待验证签名</param>
        /// <param name="publicKey">XML格式的公钥</param>
        /// <param name="content">待加签参数字典内容</param>
        /// <returns></returns>
        public static bool CheckRSA2Sign(string sign, string publicKey, string content)
        {
            try
            {
                byte[] bt = Encoding.UTF8.GetBytes(content);
                SHA256 sha256 = new SHA256CryptoServiceProvider();
                byte[] retVal = sha256.ComputeHash(bt);

                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                rsa.FromXmlString(publicKey);
                RSAPKCS1SignatureDeformatter deformatter = new RSAPKCS1SignatureDeformatter(rsa);
                deformatter.SetHashAlgorithm("SHA256");
                byte[] rgbSignature = Convert.FromBase64String(sign);
                if (deformatter.VerifySignature(retVal, rgbSignature))
                {
                    return true;
                }
            }
            catch { }
            return false;
        }
    }
}
