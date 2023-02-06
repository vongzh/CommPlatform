using System;
using System.Linq;
using System.Text;
using System.Threading;

namespace Zk.HotelPlatform.Api
{
    class Program
    {
        /// <summary>
        ///     入口函数
        /// </summary>
        /// <param name="args">-d 启动Console程序（调试用），-i 安装windows服务，-u卸载windows服务</param>
        private static void Main(string[] args)
        {
            //for (int i = 1322762; i < 99999999; i++)
            //{
            //    int id = i;

            //    int length = 8;

            //    var idStr = id.ToString();


            //    idStr = idStr.PadLeft(length - 1, '0');

            //    idStr = idStr.PadLeft(length, '1');

            //    Console.WriteLine(idStr);
            //}

            //Console.ReadLine();

            new ApiServiceHost().Run(args);
        }
    }
}
