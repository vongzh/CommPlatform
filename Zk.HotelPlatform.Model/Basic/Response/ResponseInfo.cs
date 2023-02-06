using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zk.HotelPlatform.Model.Basic.Response
{
    public class ResponseInfo
    {
        public bool Result { get; set; } = false;
        public int Code { get; set; }
        public string Message { get; set; }

        public static ResponseInfo Failtrue(string msg)
        {
            return new ResponseInfo()
            {
                Result = false,
                Message = msg
            };
        }

        public static ResponseInfo Failtrue(string msg, int code)
        {
            return new ResponseInfo()
            {
                Code = code,
                Result = false,
                Message = msg
            };
        }

        public static ResponseInfo Success(string msg = null)
        {
            var response = new ResponseInfo()
            {
                Result = true
            };

            if (!string.IsNullOrEmpty(msg))
                response.Message = msg;

            return response;
        }
    }

    public class ResponseInfo<T> : ResponseInfo where T : class, new()
    {
        public T Data { get; set; }

        public new static ResponseInfo<T> Failtrue(string msg)
        {
            return new ResponseInfo<T>()
            {
                Result = false,
                Message = msg,
                Data = default(T)
            };
        }

        public new static ResponseInfo<T> Failtrue(string msg, int code)
        {
            return new ResponseInfo<T>()
            {
                Code = code,
                Result = false,
                Message = msg,
                Data = default(T)
            };
        }

        public static ResponseInfo<T> Success(T val, string msg = null)
        {
            var response = new ResponseInfo<T>()
            {
                Result = true,
                Data = val
            };

            if (!string.IsNullOrEmpty(msg))
                response.Message = msg;

            return response;
        }
    }
}
