using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zk.HotelPlatform.Utils
{
    public class StringReplaceUtil
    {
        public static string RoomTypeNameReplace(string str)
        {
            string result = "";
            if (str != null)
            {
                result = str.Replace("房", "&").
                            Replace("间", "&").
                            Replace("单人", "1").
                            Replace("单床", "1").
                            Replace("大床", "1").
                            Replace("单", "1").
                            Replace("双人", "2").
                            Replace("双床", "2").
                            Replace("标准", "2").
                            Replace("标", "2").
                            Replace("三人", "3").
                            Replace("三床", "3").
                            Replace("(", "").Replace("（", "").
                            Replace(")", "").Replace("）", "").
                            Replace("[", "").Replace("【", "").
                            Replace("]", "").Replace("】", "").
                            Replace("-", "").
                            Replace("，", "").Replace(",", "").
                            Replace("。", "").Replace(".", "").
                            Replace("_", "").
                            Replace("*", "").
                            Replace("/", "").
                            Replace("~", "").
                            Replace("*", "").
                            Replace(" ", "").
                            Replace("·", "").
                            Replace("&#183;", "").
                            Replace("代理", "").
                            Replace("#", "").Trim();
            }
            return result;
        }
        public static string RoomTypeNameSort(string str)
        {
            var result = "";
            if (str != null)
            {
                result = new String(str.ToArray().OrderBy(p => p).ToArray());
            }
            return result;
        }
        public static string BedTypeReplace(string str)
        {
            string result;
            if (str.Contains("单人") || str.Contains("单床") || str.Contains("大床"))
            {
                result = "大床";
                return result;
            }
            else if (str.Contains("双人") || str.Contains("双床") || str.Contains("标准") || str.Contains("标"))
            {
                result = "双床";
                return result;
            }
            else if (str.Contains("三人") || str.Contains("三床"))
            {
                result = "多张单人床";
                return result;
            }
            else
            {
                result = "大床";
                return result;
            }
        }
    }
}
