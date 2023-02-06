using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zk.HotelPlatform.Utils
{
    public static class PadUtil
    {
        public static int PadTo(this int id, int length = 8)
        {
            var idStr = id.ToString();


            idStr = idStr.PadLeft(length - 1, '0');

            idStr = idStr.PadLeft(length, '1');
            return int.Parse(idStr);
        }

        public static long PadTo(this long id, int length = 8)
        {
            var idStr = id.ToString();


            idStr = idStr.PadLeft(length - 1, '0');

            idStr = idStr.PadLeft(length, '1');
            return long.Parse(idStr);
        }
    }
}
