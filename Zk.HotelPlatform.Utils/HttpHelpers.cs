using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Zk.HotelPlatform.Utils.Log;

namespace Zk.HotelPlatform.Utils
{
    public class HttpHelpers
    {
        private static string method = "GET";
        private static string CharSet = "UTF-8";

        private static string ContentType = "text/html;charset=UTF-8;";
        private static string UserAgent = "Mozilla/5.0 (Linux; Android 7.0; SM-G892A Build/NRD90M; wv) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/67.0.3396.87 Mobile Safari/537.36";

        public HttpHelpers()
        {
            ServicePointManager.DefaultConnectionLimit = 2048;
            ServicePointManager.UseNagleAlgorithm = false;
            ServicePointManager.Expect100Continue = false;
        }

        public static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true;
        }

        private static bool RemoteCertificateValidate(object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors error)
        {
            return true;
        }

        public static string GetCookie(string url)
        {
            var cookie = "";
            HttpWebRequest httpRequest = null;
            HttpWebResponse httpResponse = null;
            try
            {
                httpRequest = GetRequest(url);
                using (httpResponse = (HttpWebResponse)httpRequest.GetResponse())
                {
                    cookie = httpResponse.Headers["Set-Cookie"]?.ToString();
                }
            }
            catch (Exception ex)
            {
                LogInfoWriter.GetInstance().Error("GetCookie:" + ex);

            }
            return cookie;
        }

        public static string GetHtml(HttpItems item)
        {
            String bodys = "";
            var result = "";
            HttpWebRequest httpRequest = null;
            HttpWebResponse httpResponse = null;


            try
            {
                if (item == null || string.IsNullOrWhiteSpace(item.URL))
                {
                    throw new Exception("URL不能为空！");
                }
                var url = item.URL;
                if (item.Method == HttpMethod.GET)
                {
                    url = SetUrlParam(url, item.Params);
                }
                else if (item.Method == HttpMethod.POST)
                {
                    if (item.PostType == PostDataType.FORM)
                    {
                        if (!string.IsNullOrWhiteSpace(item.BodyData))
                        {
                            bodys = item.BodyData;
                        }
                        else
                        {
                            bodys = SetBodyParam(item.Params);
                        }
                    }
                    else if (item.PostType == PostDataType.JSON)
                    {
                        bodys = item.JsonData;
                    }
                }

                httpRequest = GetRequest(url, item.ConnectionNum);

                //httpRequest.AllowAutoRedirect = false;
                //httpRequest.UseDefaultCredentials = false;
                httpRequest.KeepAlive = item.KeepAlive;
                if (item.CredentialCache != null)
                {
                    httpRequest.Credentials = item.CredentialCache;
                    httpRequest.PreAuthenticate = true;
                }
                if (!string.IsNullOrWhiteSpace(item.UserAgent))
                {
                    httpRequest.UserAgent = item.UserAgent;
                }
                if (!string.IsNullOrWhiteSpace(item.Referer))
                {
                    httpRequest.Referer = item.Referer;
                }
                if (!string.IsNullOrWhiteSpace(item.Host))
                {
                    httpRequest.Host = item.Host;
                }
                if (!string.IsNullOrWhiteSpace(item.Accept))
                {
                    httpRequest.Accept = item.Accept;
                }
                httpRequest.Method = item.Method.ToString();

                if (item.Headers != null)
                {
                    foreach (var header in item.Headers)
                    {
                        httpRequest.Headers.Add(header.Key, header.Value);
                    }
                }
                if (item.Cookie != null)
                {
                    httpRequest.CookieContainer = item.Cookie;
                }
                if (item.Proxy != null)
                {
                    httpRequest.Proxy = item.Proxy;
                }
                if (!string.IsNullOrWhiteSpace(item.ContentType))
                {
                    httpRequest.ContentType = item.ContentType;
                }
                if (item.Timeout > 0)
                {
                    httpRequest.Timeout = item.Timeout;
                    httpRequest.ReadWriteTimeout = item.Timeout;
                }
                if (item.ConnectionNum > 0)
                {
                    httpRequest.ServicePoint.ConnectionLimit = item.ConnectionNum;
                }

                if (item.PostType == PostDataType.File)
                {
                    var data = SetPostData(httpRequest, item);
                    using (Stream stream = httpRequest.GetRequestStream())
                    {
                        stream.Write(data, 0, data.Length);
                    }
                }
                else
                {
                    if (0 < bodys.Length)
                    {
                        byte[] data = Encoding.UTF8.GetBytes(bodys);
                        using (Stream stream = httpRequest.GetRequestStream())
                        {
                            stream.Write(data, 0, data.Length);
                        }
                    }
                }

                using (httpResponse = (HttpWebResponse)httpRequest.GetResponse())
                {
                    result = GetResule(httpResponse, item.CharSet);
                    item.ResponseStatusCode = httpResponse.StatusCode;
                    item.ResponseStatusDescription = httpResponse.StatusDescription;

                    if (item.IsGetCookie)
                    {
                        item.ReturnCookies = httpResponse.Headers["Set-Cookie"];
                    }

                }

            }
            catch (WebException ex)
            {
                LogInfoWriter.GetInstance().Error("WebException:" + ex);
                httpResponse = (HttpWebResponse)ex.Response;
                item.ResponseStatusCode = httpResponse?.StatusCode ?? HttpStatusCode.ExpectationFailed;
                item.ResponseStatusDescription = httpResponse?.StatusDescription ?? ex.Message;
            }
            catch (Exception ex)
            {
                item.ResponseStatusCode = HttpStatusCode.ExpectationFailed; ;
                item.ResponseStatusDescription = ex.Message;

                LogInfoWriter.GetInstance().Error("HttpHelper-Request:" + item.ToJson() + "," + ex);

            }
            finally
            {
                //关闭连接和流
                if (httpResponse != null)
                {
                    httpResponse.Close();
                }
                if (httpRequest != null)
                {
                    httpRequest.Abort();
                }
            }
            return result;
        }

        public static string GetProxyAuth(string url, Dictionary<string, object> paras, WebProxy proxy, string authorization, string userAgent, string referer, string contentType = "text/html;charset=UTF-8;", string charSet = "UTF-8")
        {
            String bodys = "";
            var result = "";
            HttpWebRequest httpRequest = null;
            HttpWebResponse httpResponse = null;

            try
            {
                if (paras != null)
                {
                    url = SetUrlParam(url, paras);
                }
                if (url.Contains("https://"))
                {
                    ServicePointManager.Expect100Continue = false;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                    ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                    httpRequest = (HttpWebRequest)WebRequest.CreateDefault(new Uri(url));
                }
                else
                {
                    httpRequest = (HttpWebRequest)WebRequest.Create(url);
                }
                httpRequest.UserAgent = userAgent;
                httpRequest.Method = "GET";
                httpRequest.Referer = referer;

                httpRequest.Headers.Add("Proxy-Authorization", authorization);

                httpRequest.Proxy = proxy;
                httpRequest.ContentType = contentType;
                //httpRequest.Timeout = 15000;
                //httpRequest.ServicePoint.ConnectionLimit = 512;
                if (0 < bodys.Length)
                {
                    byte[] data = Encoding.UTF8.GetBytes(bodys);
                    using (Stream stream = httpRequest.GetRequestStream())
                    {
                        stream.Write(data, 0, data.Length);
                    }
                }
                httpRequest.KeepAlive = false;
                using (httpResponse = (HttpWebResponse)httpRequest.GetResponse())
                {
                    if (httpResponse.ContentEncoding.ToLower().Contains("gzip"))
                    {
                        using (GZipStream stream = new GZipStream(httpResponse.GetResponseStream(), CompressionMode.Decompress))
                        {
                            using (StreamReader reader = new StreamReader(stream, Encoding.GetEncoding(charSet)))
                            {
                                result = reader.ReadToEnd();
                            }
                        }
                    }
                    else if (httpResponse.ContentEncoding.ToLower().Contains("deflate"))
                    {
                        using (DeflateStream stream = new DeflateStream(httpResponse.GetResponseStream(), CompressionMode.Decompress))
                        {
                            using (StreamReader reader = new StreamReader(stream, Encoding.GetEncoding(charSet)))
                            {
                                result = reader.ReadToEnd();
                            }
                        }
                    }
                    else
                    {
                        using (Stream st = httpResponse.GetResponseStream())
                        {
                            using (StreamReader reader = new StreamReader(st, Encoding.GetEncoding(charSet)))
                            {
                                result = reader.ReadToEnd();
                            }
                        }
                    }
                }

            }
            catch (WebException ex)
            {
                LogInfoWriter.GetInstance().Error("WebException:" + ex);

                httpResponse = (HttpWebResponse)ex.Response;
            }
            finally
            {
                //关闭连接和流
                if (httpResponse != null)
                {
                    httpResponse.Close();
                }
                if (httpRequest != null)
                {
                    httpRequest.Abort();
                }
            }
            return result;
        }


        private static byte[] SetPostData(HttpWebRequest request, HttpItems item)
        {
            List<byte> array = new List<byte>();

            if (item.Method != HttpMethod.POST)
            {
                return null;
            }

            if (item.PostType == PostDataType.FORM)
            {
                var bodys = SetBodyParam(item.Params);
                array.AddRange(Encoding.UTF8.GetBytes(bodys));
            }
            else if (item.PostType == PostDataType.JSON)
            {
                array.AddRange(Encoding.UTF8.GetBytes(item.JsonData));
            }
            else if (item.PostType == PostDataType.File)
            {

                var boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x", NumberFormatInfo.InvariantInfo);

                request.ContentType = "multipart/form-data; boundary=" + boundary;
                boundary = "--" + boundary;
                if (item.Params != null)
                {
                    foreach (var pair in item.Params)
                    {
                        array.AddRange(Encoding.UTF8.GetBytes(boundary + Environment.NewLine));
                        array.AddRange(Encoding.UTF8.GetBytes($"Content-Disposition: form-data; name=\"{pair.Key}\"{Environment.NewLine}{Environment.NewLine}"));
                        array.AddRange(Encoding.UTF8.GetBytes(pair.Value + Environment.NewLine));
                    }
                }

                if (item.UploadFiles != null)
                {

                    foreach (var file in item.UploadFiles)
                    {
                        array.AddRange(Encoding.UTF8.GetBytes(boundary + Environment.NewLine));

                        array.AddRange(Encoding.UTF8.GetBytes($"Content-Disposition: form-data; name=\"{file.Name}\";filename=\"{file.Filename}\"{Environment.NewLine}"));

                        array.AddRange(Encoding.UTF8.GetBytes($"Content-Type: {file.ContentType} {Environment.NewLine}{Environment.NewLine}"));

                        array.AddRange(file.FileData);
                        //using (file.Stream)
                        //{
                        //    while ((bytesRead = file.Stream.Read(buffer, 0, buffer.Length)) != 0)
                        //    {
                        //        array.AddRange(buffer);
                        //    }
                        //}
                    }
                }
                array.AddRange(Encoding.UTF8.GetBytes(Environment.NewLine));
                array.AddRange(Encoding.UTF8.GetBytes(boundary + "--"));

            }
            return array.ToArray();

        }
        public static string SetUrlParam(string url, Dictionary<string, object> paras)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(url);

            if (paras != null && paras.Count > 0)
            {
                if (!url.Contains("?"))
                {
                    builder.Append("?");
                }
                var lastIndex = paras.LastOrDefault();
                foreach (var item in paras)
                {
                    if (lastIndex.Key == item.Key)
                    {
                        builder.AppendFormat("{0}={1}", item.Key, item.Value);
                    }
                    else
                    {
                        builder.AppendFormat("{0}={1}&", item.Key, item.Value);
                    }
                }

            }
            return builder.ToString();
        }
        public static string SetBodyParam(Dictionary<string, object> paras)
        {
            StringBuilder builder = new StringBuilder();
            if (paras != null && paras.Count > 0)
            {
                var lastIndex = paras.LastOrDefault();
                foreach (var item in paras)
                {
                    if (lastIndex.Key == item.Key)
                    {
                        builder.AppendFormat("{0}={1}", item.Key, item.Value);
                    }
                    else
                    {
                        builder.AppendFormat("{0}={1}&", item.Key, item.Value);
                    }
                }
            }
            return builder.ToString();
        }
        private static HttpWebRequest GetRequest(string url, int connNum = 0)
        {
            HttpWebRequest httpRequest;

            if (url.Contains("https://"))
            {
                ServicePointManager.Expect100Continue = false;
                if (connNum > 0)
                {
                    ServicePointManager.DefaultConnectionLimit = connNum;
                }
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls;
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                httpRequest = (HttpWebRequest)WebRequest.CreateDefault(new Uri(url));
            }
            else
            {
                httpRequest = (HttpWebRequest)WebRequest.Create(url);
            }
            //httpRequest.ServicePoint.UseNagleAlgorithm = false;
            //httpRequest.AllowWriteStreamBuffering = false;
            return httpRequest;
        }
        private static string GetResule(HttpWebResponse httpResponse, Encoding charSet)
        {
            string result;
            if (httpResponse.ContentEncoding.ToLower().Contains("gzip"))
            {
                using (GZipStream stream = new GZipStream(httpResponse.GetResponseStream(), CompressionMode.Decompress))
                {
                    using (StreamReader reader = new StreamReader(stream, charSet))
                    {
                        result = reader.ReadToEnd();
                    }
                }
            }
            else if (httpResponse.ContentEncoding.ToLower().Contains("deflate"))
            {
                using (DeflateStream stream = new DeflateStream(httpResponse.GetResponseStream(), CompressionMode.Decompress))
                {
                    using (StreamReader reader = new StreamReader(stream, charSet))
                    {
                        result = reader.ReadToEnd();
                    }
                }
            }
            else
            {
                using (Stream stream = httpResponse.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(stream, charSet))
                    {
                        result = reader.ReadToEnd();
                    }
                }
            }

            return result;
        }


        public static CredentialCache GetCredential(string url, string uname, string upwd)
        {


            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
            CredentialCache credentialCache = new CredentialCache();
            credentialCache.Add(new System.Uri(url), "Bearer", new NetworkCredential(uname, upwd));
            return credentialCache;
        }
    }

    public class HttpItems
    {
        public HttpMethod Method { get; set; } = HttpMethod.GET;
        public string URL { get; set; }
        public string JsonData { get; set; }
        public List<UploadFile> UploadFiles { get; set; }
        public PostDataType PostType { get; set; } = PostDataType.FORM;
        public string IfModifiedSince { get; set; } = "Thu, 01 Jan 1970 00:00:00 GMT";
        public string ContentType { get; set; } = "text/html;charset=UTF-8;";
        public CookieContainer Cookie { get; set; }
        public CredentialCache CredentialCache { get; set; }
        public string Accept { get; set; } = "application/json, text/plain, */*";
        public string BodyData { get; set; }
        public bool KeepAlive { get; set; }
        public bool IsGetCookie { get; set; }
        public string ReturnCookies { get; set; }
        public string Host { get; set; }
        public WebProxy Proxy { get; set; }
        public NetworkCredential ProxyCredentials { get; set; }
        public string Referer { get; set; }
        public string UserAgent { get; set; } = "Mozilla/5.0 (Linux; Android 7.0; SM-G892A Build/NRD90M; wv) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/67.0.3396.87 Mobile Safari/537.36";
        public Dictionary<string, string> Headers { get; set; }
        public Dictionary<string, object> Params { get; set; }
        public Encoding CharSet { get; set; } = Encoding.UTF8;
        public int ConnectionNum { get; set; }
        public int Timeout { get; set; } = 25000;
        public HttpStatusCode ResponseStatusCode { get; set; }
        public string ResponseStatusDescription { get; set; }
    }

    public class UploadFile
    {
        public UploadFile()
        {
            ContentType = "application/octet-stream";
        }
        public string Name { get; set; }
        public string Filename { get; set; }
        public string ContentType { get; set; }
        public byte[] FileData { get; set; }
    }

    public enum PostDataType
    {
        FORM,
        JSON,
        XML,
        File
    }

    public enum HttpMethod
    {
        POST,
        GET,
        PUT,
        DELETE
    }

}
