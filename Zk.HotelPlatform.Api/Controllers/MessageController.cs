using System.Web.Http;
using Zk.HotelPlatform.Api.Filters;
using Zk.HotelPlatform.Service;
using static Zk.HotelPlatform.Utils.Global.GlobalEnum;

namespace Zk.HotelPlatform.Api.Controllers
{
    [Authorize(Roles = "Client")]
    [ResponseHandler]
    public class MessageController : BaseController
    {
        private readonly IMessageService _messageService = null;
        public MessageController(IMessageService messageService)
        {
            _messageService = messageService;
        }

        /// <summary>
        /// 系统消息通知
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="messageType"></param>
        /// <param name="duration"></param>
        /// <param name="redirect"></param>
        /// <param name="qs"></param>
        /// <param name="popupWinType"></param>
        
        [HttpGet]
        [Route("Message/Notify")]
        public void Notify(string msg, MessageTypeEnum messageType, int duration = 0, string redirect = "", string qs = "")
        {
            _messageService.Notify(msg, messageType, duration, redirect, qs);
        }


        /// <summary>
        /// 系统消息通知给个人
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="msg"></param>
        /// <param name="messageType"></param>
        /// <param name="duration"></param>
        /// <param name="redirect"></param>
        /// <param name="qs"></param>
        
        [HttpGet]
        [Route("Message/NotifyToUser")]
        public void NotifyToUser(int userId, string msg, MessageTypeEnum messageType, int duration = 0, string redirect = "", string qs = "")
        {
            _messageService.NotifyToUser(userId, msg, messageType);

        }
    }
}
