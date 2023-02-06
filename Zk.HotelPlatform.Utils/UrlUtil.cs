using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Zk.HotelPlatform.Utils
{
    public static class UrlUtil
    {
        public static SortedDictionary<string, string> ToDictionary(this string str)
        {
            string[] kvs = str.Split('&');

            var paramDic = new SortedDictionary<string, string>();
            foreach (var kv in kvs)
            {
                var kvArr = kv.Split(new char[] { '=' }, 2);
                paramDic.Add(kvArr[0], kvArr[1]);
            }
            return paramDic;
        }

        public static string ReplaceHttpHost(this string url,string replace)
        {
            Regex rgx = new Regex(@"((([A-Za-z]{3,9}:(?:\/\/)?)(?:[-;:&=\+\$,\w]+@)?[A-Za-z0-9.-]+(:[0-9]+)?|(?:ww‌​w.|[-;:&=\+\$,\w]+@)[A-Za-z0-9.-]+)((?:\/[\+~%\/.\w-_]*)?\??(?:[-\+=&;%@.\w_]*)#?‌​(?:[\w]*))?)");
            return rgx.Replace(url, replace);
        }
    }
}
