using Microsoft.International.Converters.PinYinConverter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zk.HotelPlatform.Utils
{
    public static class PinYinUtil
    {
        public static string ToPinYin(this string str)
        {
            List<string> chrPinYin = new List<string>();
            str.ToList().ForEach(s =>
            {
                var pinyin = new ChineseChar(s).Pinyins[0];
                var c = pinyin.Substring(0, pinyin.Length - 1);
                chrPinYin.Add(c);
            });
            return string.Join("", chrPinYin);
        }

        public static string ToPinYin(this char chr)
        {
            var pinyin = new ChineseChar(chr).Pinyins[0];
            return pinyin.Substring(0, pinyin.Length - 1);
        }


        /// <summary>
        /// 获得第一个汉字的首字母（大写）；
        /// </summary>
        /// <param name="cnChar"></param>
        /// <returns></returns>
        //public static string getSpell(string cnChar)
        //{
        //    byte[] arrCN = Encoding.Default.GetBytes(cnChar);
        //    if (arrCN.Length > 1)
        //    {
        //        int area = (short)arrCN[0];
        //        int pos = (short)arrCN[1];
        //        int code = (area << 8) + pos;
        //        int[] areacode = { 45217, 45253, 45761, 46318, 46826, 47010, 47297, 47614, 48119, 48119, 49062, 49324, 49896, 50371, 50614, 50622, 50906, 51387, 51446, 52218, 52698, 52698, 52698, 52980, 53689, 54481 };
        //        for (int i = 0; i < 26; i++)
        //        {
        //            int max = 55290;
        //            if (i != 25) max = areacode[i + 1];
        //            if (areacode[i] <= code && code < max)
        //            {
        //                return Encoding.Default.GetString(new byte[] { (byte)(65 + i) });
        //            }
        //        }
        //        return "*";
        //    }
        //    else return cnChar;
        //}

        /// <summary>
        /// 获取汉字的首字母(大写)
        /// </summary>
        /// <param name="str"></param>
        /// <returns>string</returns>
        public static string getSpell(string str)
        {
            if (str.CompareTo("吖") < 0)
            {
                return str;
            }
            if (str.CompareTo("八") < 0)
            {
                return "A";
            }

            if (str.CompareTo("嚓") < 0)
            {
                return "B";
            }

            if (str.CompareTo("咑") < 0)
            {
                return "C";
            }
            if (str.CompareTo("妸") < 0)
            {
                return "D";
            }
            if (str.CompareTo("发") < 0)
            {
                return "E";
            }
            if (str.CompareTo("旮") < 0)
            {
                return "F";
            }
            if (str.CompareTo("铪") < 0)
            {
                return "G";
            }
            if (str.CompareTo("讥") < 0)
            {
                return "H";
            }
            if (str.CompareTo("咔") < 0)
            {
                return "J";
            }
            if (str.CompareTo("垃") < 0)
            {
                return "K";
            }
            if (str.CompareTo("嘸") < 0)
            {
                return "L";
            }
            if (str.CompareTo("拏") < 0)
            {
                return "M";
            }
            if (str.CompareTo("噢") < 0)
            {
                return "N";
            }
            if (str.CompareTo("妑") < 0)
            {
                return "O";
            }
            if (str.CompareTo("七") < 0)
            {
                return "P";
            }
            if (str.CompareTo("亽") < 0)
            {
                return "Q";
            }
            if (str.CompareTo("仨") < 0)
            {
                return "R";
            }
            if (str.CompareTo("他") < 0)
            {
                return "S";
            }
            if (str.CompareTo("哇") < 0)
            {
                return "T";
            }
            if (str.CompareTo("夕") < 0)
            {
                return "W";
            }
            if (str.CompareTo("丫") < 0)
            {
                return "X";
            }
            if (str.CompareTo("帀") < 0)
            {
                return "Y";
            }
            if (str.CompareTo("咗") < 0)
            {
                return "Z";
            }
            return str;
        }
    }
}
