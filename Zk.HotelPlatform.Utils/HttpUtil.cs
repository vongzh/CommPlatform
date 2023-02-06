namespace Zk.HotelPlatform.Utils
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Net.Security;
    using System.Runtime.InteropServices;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;
    using Zk.HotelPlatform.Utils.Log;

    public class HttpUtil
    {
        public static void ToWechatWork(string title, string desc)
        {
            try
            {
                // string rateplanuplogkey = string.Format("Notice");
                // List<string> listroomtypeid = Cache.Redis.RedisCache.Hash_GetAll<string>(rateplanuplogkey);
                // for (int i = 0; i < listroomtypeid.Count; i++)
                // {
                string url = "https://api.51zhu.cn/WechatWork/Push?toUser=nideyangzi&title=" + title + "&desc=" + desc + "&url=";
                HttpItems item = new HttpItems();
                item.URL = url;
                var result = HttpHelpers.GetHtml(item);

                string url1 = "https://api.51zhu.cn/WechatWork/Push?toUser=ashilikeaia&title=" + title + "&desc=" + desc + "&url=";
                HttpItems item1 = new HttpItems();
                item1.URL = url1;
                var result1 = HttpHelpers.GetHtml(item1);
                //MyLog.ErrorLog("通知", listroomtypeid[i] + "," + result);
                //}
            }
            catch (Exception ex)
            {

                //  MyLog.ErrorLog("通知异常", ex);
            }
        }
        public static string Get(string url, Encoding encoding = null, int timeout = 10000)
        {
            string str;
            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }
            try
            {
                using (Stream stream = new RequestHandler().GetNetStream(url, timeout, null, null))
                {
                    str = new StreamReader(stream, encoding).ReadToEnd();
                }
            }
            catch (WebException exception)
            {
                LogInfoWriter.GetInstance().Warn($"HttpClientUtils get timeout error url:{url}", exception);
                str = string.Empty;
            }
            catch (Exception exception2)
            {
                LogInfoWriter.GetInstance().Error($"HttpClientUtils get error url:{url}", exception2);
                str = string.Empty;
            }
            return str;
        }
        public static string Get(string url, string ua, Dictionary<string, string> addHeader = null, string contentType = "", int timeout = 10000)
        {
            HttpWebRequest request = null;
            if (url.Contains("https://"))
            {
                ServicePointManager.Expect100Continue = false;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                ServicePointManager.ServerCertificateValidationCallback = (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors) => { return true; };
                request = (HttpWebRequest)WebRequest.CreateDefault(new Uri(url));
            }
            else
            {
                request = (HttpWebRequest)WebRequest.Create(url);
            }
            request.Accept = "*/*";
            request.Headers["Accept-Language"] = "zh-cn";
            request.Headers["Accept-Encoding"] = "gzip,deflate";
            if (addHeader != null)
            {
                foreach (KeyValuePair<string, string> pair in addHeader)
                {
                    request.Headers.Add(pair.Key, pair.Value);
                }
            }
            request.KeepAlive = false;
            request.Timeout = timeout;
            request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
            request.UserAgent = "Net Server";
            request.Proxy = null;
            request.Method = "Get";
            request.ServicePoint.ConnectionLimit = 1000;
            ServicePointManager.DefaultConnectionLimit = 1000;
            request.ContentType = string.IsNullOrEmpty(contentType) ? "application/json" : contentType;

            string response = null;
            using (var stream = ((HttpWebResponse)request.GetResponse()).GetResponseStream())
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    response = reader.ReadToEnd();
                }
            }
            return response;
        }
        public static string Get(string url, WebProxy proxy, string ua, int timeout = 10000, Dictionary<string, string> addHeader = null, string contentType = "", string referer = "", CookieContainer cookieJar = null)
        {
            HttpWebRequest request = null;
            if (url.Contains("https://"))
            {
                ServicePointManager.Expect100Continue = false;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                ServicePointManager.ServerCertificateValidationCallback = (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors) => { return true; };
                request = (HttpWebRequest)WebRequest.CreateDefault(new Uri(url));
            }
            else
            {
                request = (HttpWebRequest)WebRequest.Create(url);
            }
            request.Accept = "*/*";
            request.Referer = referer;
            request.Headers["Accept-Language"] = "zh-cn";
            request.Headers["Accept-Encoding"] = "gzip,deflate";
            if (addHeader != null)
            {
                foreach (KeyValuePair<string, string> pair in addHeader)
                {
                    request.Headers.Add(pair.Key, pair.Value);
                }
            }
            request.KeepAlive = false;
            request.Timeout = timeout;
            request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
            request.UserAgent = ua;
            request.Proxy = proxy;
            request.Method = "Get";
            request.ServicePoint.ConnectionLimit = 1000;
            ServicePointManager.DefaultConnectionLimit = 1000;
            request.ContentType = string.IsNullOrEmpty(contentType) ? "application/json" : contentType;
            request.CookieContainer = cookieJar;

            string response = null;
            using (var stream = ((HttpWebResponse)request.GetResponse()).GetResponseStream())
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    response = reader.ReadToEnd();
                }
            }
            return response;
        }
        public static string Post(string url, string data, int timeout = 10000, Dictionary<string, string> addHeader = null, string contentType = null)
        {
            HttpWebRequest request = null;
            byte[] bytes = Encoding.UTF8.GetBytes(data);
            if (url.Contains("https://"))
            {
                ServicePointManager.Expect100Continue = false;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                ServicePointManager.ServerCertificateValidationCallback = (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors) => { return true; };
                request = (HttpWebRequest)WebRequest.CreateDefault(new Uri(url));
            }
            else
            {
                request = (HttpWebRequest)WebRequest.Create(url);
            }
            request.Accept = "*/*";
            request.Referer = "";
            request.Headers["Accept-Language"] = "zh-cn";
            request.Headers["Accept-Encoding"] = "gzip,deflate";
            if (addHeader != null)
            {
                foreach (KeyValuePair<string, string> pair in addHeader)
                {
                    request.Headers.Add(pair.Key, pair.Value);
                }
            }
            request.Timeout = timeout;
            request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
            request.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 5.1; en-US) AppleWebKit/534.7 (KHTML, like Gecko) Chrome/7.0.517.41 Safari/534.7";
            request.Proxy = null;
            request.Method = "POST";
            request.KeepAlive = false;
            request.ServicePoint.ConnectionLimit = 1000;
            ServicePointManager.DefaultConnectionLimit = 1000;
            request.ContentType = string.IsNullOrEmpty(contentType) ? "application/json" : contentType;
            request.ContentLength = bytes.Length;

            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(bytes, 0, bytes.Length);
            }

            string response = null;
            using (var stream = ((HttpWebResponse)request.GetResponse()).GetResponseStream())
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    response = reader.ReadToEnd();
                }
            }
            return response;
        }
        public static string Post(string url, string data, WebProxy proxy, string ua, int timeout = 10000, Dictionary<string, string> addHeader = null, string contentType = null, string referer = "", CookieContainer cookieJar = null)
        {
            HttpWebRequest request = null;
            byte[] bytes = Encoding.UTF8.GetBytes(data);
            request = (HttpWebRequest)WebRequest.Create(url);
            request.Accept = "*/*";
            request.Referer = referer;
            request.Headers["Accept-Language"] = "zh-cn";
            request.Headers["Accept-Encoding"] = "gzip,deflate";
            if (addHeader != null)
            {
                foreach (KeyValuePair<string, string> pair in addHeader)
                {
                    request.Headers.Add(pair.Key, pair.Value);
                }
            }
            request.Proxy = proxy;
            request.Timeout = timeout;
            request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
            request.UserAgent = ua;
            request.Method = "POST";
            request.ServicePoint.ConnectionLimit = 1000;
            ServicePointManager.DefaultConnectionLimit = 1000;
            request.ContentType = string.IsNullOrEmpty(contentType) ? "application/json" : contentType;
            request.ContentLength = bytes.Length;
            request.CookieContainer = cookieJar;
            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(bytes, 0, bytes.Length);
            }

            string response = null;
            using (var stream = ((HttpWebResponse)request.GetResponse()).GetResponseStream())
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    response = reader.ReadToEnd();
                }
            }
            return response;
        }
    }

    internal class RequestHandler
    {
        static RequestHandler()
        {
            ServicePointManager.DefaultConnectionLimit = 0x80;
            ServicePointManager.UseNagleAlgorithm = false;
            ServicePointManager.Expect100Continue = false;
        }

        public Stream GetFileStream(string url)
        {
            Stream stream = null;
            WebClient client1 = new WebClient();
            client1.Headers.Add("User-Agent", "Mozilla/5.0 (Windows; U; Windows NT 5.1; en-US) AppleWebKit/534.7 (KHTML, like Gecko) Chrome/7.0.517.41 Safari/534.7");
            client1.Headers.Add("UserAgent", "Mozilla/5.0 (Windows; U; Windows NT 5.1; en-US) AppleWebKit/534.7 (KHTML, like Gecko) Chrome/7.0.517.41 Safari/534.7");
            byte[] buffer = client1.DownloadData(url);
            if ((buffer != null) && (buffer.Length != 0))
            {
                stream = new MemoryStream(buffer);
            }
            return stream;
        }

        public Stream GetNetStream(string url, int timeout = 0xbb8, string contentType = "", Dictionary<string, string> addHeader = null)
        {
            HttpWebRequest request = null;
            request = (HttpWebRequest)WebRequest.Create(url);
            request.Accept = "*/*";
            request.Referer = "";
            request.Headers["Accept-Language"] = "zh-cn";
            request.Headers["Accept-Encoding"] = "gzip,deflate";
            if (addHeader != null)
            {
                foreach (KeyValuePair<string, string> pair in addHeader)
                {
                    request.Headers.Add(pair.Key, pair.Value);
                }
            }
            request.ContentType = string.IsNullOrEmpty(contentType) ? "application/x-www-form-urlencoded" : contentType;
            request.Timeout = timeout;
            request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
            request.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 5.1; en-US) AppleWebKit/534.7 (KHTML, like Gecko) Chrome/7.0.517.41 Safari/534.7";
            request.Proxy = null;
            return request.GetResponse().GetResponseStream();
        }

        public Stream Post(string url, string data, int timeout = 0xbb8, Dictionary<string, string> addHeader = null, string contentType = "")
        {
            HttpWebRequest request = null;
            byte[] bytes = Encoding.UTF8.GetBytes(data);
            request = (HttpWebRequest)WebRequest.Create(url);
            request.Accept = "*/*";
            request.Referer = "";
            request.Headers["Accept-Language"] = "zh-cn";
            request.Headers["Accept-Encoding"] = "gzip,deflate";
            if (addHeader != null)
            {
                foreach (KeyValuePair<string, string> pair in addHeader)
                {
                    request.Headers.Add(pair.Key, pair.Value);
                }
            }
            request.Timeout = timeout;
            request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
            request.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 5.1; en-US) AppleWebKit/534.7 (KHTML, like Gecko) Chrome/7.0.517.41 Safari/534.7";
            request.Proxy = null;
            request.Method = "POST";
            request.ContentType = string.IsNullOrEmpty(contentType) ? "application/x-www-form-urlencoded" : contentType;
            request.ContentLength = bytes.Length;
            Stream requestStream = request.GetRequestStream();
            requestStream.Write(bytes, 0, bytes.Length);
            requestStream.Close();
            return ((HttpWebResponse)request.GetResponse()).GetResponseStream();
        }
    }
}

