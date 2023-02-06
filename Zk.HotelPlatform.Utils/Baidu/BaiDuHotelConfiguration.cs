using CsharpHttpHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Zk.HotelPlatform.Utils.Baidu
{
   public  class BaiDuHotelConfiguration
    {/** SDK版本号 */
        //public string accessCode = "";//账户编码 商务提供
       
        public string salt = Global.GlobalConfig.BaiDu_salt  ;//随机串
        public string data = ""; //请求参数
        public long timestamp = GetTimestamp();//10位时间戳。若请求发起时间与平台服务端接受请求的时间相差过大，平台将直接拒绝本次请求 
                       
        //测试
        public static string url = "https://maphotel.baidu.com/map_mc/mc/tpnotify?topic=e569f262b2a345588af22b13b5892565__zhongke";
      //  public static string url = "https://maphotel.baidu.com/map_mc/mc/tpnotify?topic=e569f262b2a345588af22b13b5892565__test_tp_notify";
        public string tp_name = Global.GlobalConfig.BaiDu_tp_name;// 渠道号，开发前分配渠道号和私钥（primary_key），请妥善保管
        public string primary_key = Global.GlobalConfig.BaiDu_primary_key ; //私钥

        //正式
        //public static string url = "https://maphotel.baidu.com/map_mc/mc/tpnotify?topic=e569f262b2a345588af22b13b5892565__test_tp_notify";
        //public string tp_name = "baidu";// 渠道号，开发前分配渠道号和私钥（primary_key），请妥善保管
        //public string primary_key = "42b9e6848ee3da027310384c770fceb8"; //私钥
        

        public string sign = ""; //数据签名
        //用于验证此次请求合法性的签名（签名方法见下文）

        //public BaiDuHotelConfiguration(string methods, object datas)
        //{

        //    data = datas.ToJson();
           
          

        //}

        //public static int getnonce()
        //{
        //    Random rd = new Random();
        //    //rd.Next(100000000, 999999999).ToString() + GetTimestamp().ToString() + rd.Next(100000000, 999999999).ToString() + 
        //    string str = rd.Next(1000, 9999).ToString();
        //    int nonce = Convert.ToInt32(str);
        //    return nonce;
        //}
        public static long GetTimestamp()
        {
            TimeSpan ts = DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1);//ToUniversalTime()转换为标准时区的时间,去掉的话直接就用北京时间
                                                                                    // return (long)ts.TotalMilliseconds; //精确到毫秒
            return (long)ts.TotalSeconds;//获取10位



        }

        public string getsignature(string datas)
        {
            //  md5(timestamp + md5(data + accessKey) + accessSecretKey)
            string signature = UserMd5(tp_name+timestamp + salt + primary_key);
            return signature;
        }

        public static string UserMd5(string str)
        {
            string cl = str;
            string pwd = "";
            MD5 md5 = MD5.Create();//实例化一个md5对像
            // 加密后是一个字节类型的数组，这里要注意编码UTF8/Unicode等的选择　
            byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(cl));
            // 通过使用循环，将字节类型的数组转换为字符串，此字符串是常规字符格式化所得
            for (int i = 0; i < s.Length; i++)
            {
                // 将得到的字符串使用十六进制类型格式。格式后的字符是小写的字母，如果使用大写（X）则格式后的字符是大写字符 
                pwd = pwd + s[i].ToString("x2");
            }
            return pwd;
        }


        public string HttpPost(string postdata)
        {
            //创建Httphelper对象
            HttpHelper http = new HttpHelper();
            //创建Httphelper参数对象
            HttpItem item = new HttpItem()
            {
                ContentType = "application/json; charset=utf-8",//返回类型    可选项有默认值
                Postdata = postdata,//Post要发送的数据
                URL = url,//URL     必需项    
                Method = "post",//URL     可选项 默认为Get   
                // Referer = //,
                Accept = "application/json",
                Timeout = 6000,
                PostEncoding = System.Text.Encoding.UTF8,//默认为Default，
            };
            //请求的返回值对象
            HttpResult result = http.GetHtml(item);
            //获取请请求的Html
            string html = result.Html;
            return html;
        }
        public string HttpPosttui(string postdata,string url)
        {
            //创建Httphelper对象
            HttpHelper http = new HttpHelper();
            //创建Httphelper参数对象
            HttpItem item = new HttpItem()
            {
                ContentType = "application/json; charset=utf-8",//返回类型    可选项有默认值
                Postdata = postdata,//Post要发送的数据
                URL = url ,//URL     必需项    
                Method = "post",//URL     可选项 默认为Get   
                // Referer = //,
                Accept = "application/json",
                Timeout = 6000,
                PostEncoding = System.Text.Encoding.UTF8,//默认为Default，
            };
            //请求的返回值对象
            HttpResult result = http.GetHtml(item);
            //获取请请求的Html
            string html = result.Html;
            return html;
        }
        public string HttpPost(string postdata, string url1)
        {
            //创建Httphelper对象
            HttpHelper http = new HttpHelper();
            //创建Httphelper参数对象
            HttpItem item = new HttpItem()
            {
                ContentType = "application/json; charset=utf-8",//返回类型    可选项有默认值
                Postdata = postdata,//Post要发送的数据
                URL = url1,//URL     必需项    
                Method = "post",//URL     可选项 默认为Get   
                // Referer = //,
                Accept = "application/json",
                Timeout = 6000,
                PostEncoding = System.Text.Encoding.UTF8,//默认为Default，
            };
            //请求的返回值对象
            HttpResult result = http.GetHtml(item);
            //获取请请求的Html
            string html = result.Html;
            return html;
        }
    }
}
