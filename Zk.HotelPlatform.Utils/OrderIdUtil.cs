using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zk.HotelPlatform.Utils
{
    public class OrderIdUtil
    {
        // 最近的时间戳
        private long lastTimestamp = 0;
        //机器id 2位
        private int machineId = 1;
        // 0，并发控制
        private long sequence = 0L;
        // 序列号的最大值
        private const int sequenceMax = 9999;

        public OrderIdUtil(int workId)
        {
            machineId = workId;
        }

        /**
         * 生成订单号
         */
        public string NextId()
        {
            string time = DateTime.Now.ToString("yyMMddHHmmss");
            long timestamp = DateTime.Now.Millisecond;
            if (this.lastTimestamp == timestamp)
            {
                // 如果上一个timestamp与新产生的相等，则sequence加一(0-4095循环);
                // 对新的timestamp，sequence从0开始
                this.sequence = this.sequence + 1 % sequenceMax;
                if (this.sequence == 0)
                {
                    // 重新生成timestamp
                    timestamp = this.tilNextMillis(this.lastTimestamp);
                }
            }
            else
            {
                this.sequence = 0;
            }
            this.lastTimestamp = timestamp;
            StringBuilder sb = new StringBuilder(time).Append(machineId).Append(leftPad(sequence, 4));
            return sb.ToString();
        }

        /**
         * 补码
         * @param i
         * @param n
         * @return
         */
        private string leftPad(long i, int n)
        {
            StringBuilder sb = new StringBuilder();
            int c = n - i.ToString().Length;
            c = c < 0 ? 0 : c;
            for (int t = 0; t < c; t++)
            {
                sb.Append("0");
            }
            return sb.Append(i.ToString()).ToString();
        }

        /**
         * 等待下一个毫秒的到来, 保证返回的毫秒数在参数lastTimestamp之后
         */
        private long tilNextMillis(long lastTimestamp)
        {
            long timestamp = DateTime.Now.Millisecond;
            while (timestamp <= lastTimestamp)
            {
                timestamp = DateTime.Now.Millisecond;
            }
            return timestamp;
        }
    }
}
