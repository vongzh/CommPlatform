using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zk.HotelPlatform.Utils
{
    public class BusinessException : Exception
    {
        public int Handler { get; set; }

        public enum BusinessesExceptionCode
        {
            Auto = -100,
            Manual = -10000
        }

        public BusinessException(string message, BusinessesExceptionCode handler = BusinessesExceptionCode.Auto) : base(message)
        {
            this.Handler = (int)handler;
        }
    }

    public class OuterBusinessException : Exception
    {
        public int Code { get; set; }

        public OuterBusinessException(string message) : base(message)
        {

        }

        public OuterBusinessException(int code,string message) : base(message)
        {
            this.Code = code;
        }
    }
}
