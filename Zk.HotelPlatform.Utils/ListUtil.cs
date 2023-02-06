using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zk.HotelPlatform.Utils
{
    public class ListUtil
    {
        /// <summary>
        /// List<string>转化为List<long>
        /// </summary>
        /// <param name="hotelIds"></param>
        /// <returns></returns>
        public static List<long> ListStrToListLong(List<string> hotelIds)
        {
            var tempList = new List<long>();
            foreach (var item in hotelIds)
            {
                tempList.Add(long.Parse(item));
            }
            return tempList;
        }


        /// <summary>
        /// List<string>转化为List<long>
        /// </summary>
        /// <param name="hotelIds"></param>
        /// <returns></returns>
        public static List<string> ListLongToListStr(List<long> hotelIds)
        {
            var tempList = new List<string>();
            foreach (var item in hotelIds)
            {
                tempList.Add(item.ToString());
            }
            return tempList;
        }

    }
}
