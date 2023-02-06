using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Zk.HotelPlatform.Utils
{
    public class DictionaryUtil
    {

        public static Dictionary<string, string> StrToDictionary(string s)
        {
            var dic = new Dictionary<string, string>();
            if (s.Contains("&") || s.Contains("="))
            {
                var args = HttpUtility.ParseQueryString(s);
                if (args == null || args.Count <= 0) { return dic; }
                foreach (var key in args.AllKeys)
                {
                    if (key == null) { continue; }
                    if (dic.ContainsKey(key))
                    {
                        dic[key] = args[key];
                    }
                    else
                    {
                        dic.Add(key, args[key]);
                    }

                }
            }

            return dic;

        }
    }
}
