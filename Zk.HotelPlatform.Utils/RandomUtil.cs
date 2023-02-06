using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zk.HotelPlatform.Utils
{
    public class RandomUtil
    {
        public static string RandomNumber(int length)
        {
            Random random = new Random();

            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                stringBuilder.Append(random.Next(0, 9));
            }
            return stringBuilder.ToString();
        }
    }
}
