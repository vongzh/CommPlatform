using System;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using System.Net.Http;
using System.Web.Http.Filters;
using Newtonsoft.Json.Serialization;
using Zk.HotelPlatform.Utils;
using Zk.HotelPlatform.Utils.Log;
using Zk.HotelPlatform.Model.Basic.Response;

using System.Threading.Tasks;
using System.Reflection;

namespace Zk.HotelPlatform.Api.Filters
{
    public class ExceptionHandlerAttribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            {
                if (actionExecutedContext.Exception is BusinessException)
                {
                    var logContent = string.Format("{0} {1}", DateTime.Now, actionExecutedContext.Exception.Message);
                    LogInfoWriter.GetInstance().Warn(actionExecutedContext.Exception.Message, actionExecutedContext.Exception);
                    Console.WriteLine(logContent);
                }
                else
                {
                    var logContent = string.Format("{0} {1}", DateTime.Now, actionExecutedContext.Exception.Message);
                    LogInfoWriter.GetInstance().Error(actionExecutedContext.Exception.Message, actionExecutedContext.Exception);
                 
                    Console.WriteLine(logContent);
                }
            }

            var response = ResponseInfo.Failtrue(actionExecutedContext.Exception.Message);
            if (actionExecutedContext.Exception is BusinessException)
            {
                response.Code = ((BusinessException)actionExecutedContext.Exception).Handler;
            }

            var messageContent = JsonConvert.SerializeObject(response, Settings.JsonSerializerSettings);
            actionExecutedContext.Response = new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(messageContent, Encoding.UTF8, "application/json")
            };

            base.OnException(actionExecutedContext);
        }
    }
}
