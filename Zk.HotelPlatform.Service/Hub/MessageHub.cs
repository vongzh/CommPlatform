using Autofac;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System.Collections.Generic;
using System.Threading.Tasks;
using Zk.HotelPlatform.Utils.Global;
using Zk.HotelPlatform.Model;
using static Zk.HotelPlatform.Utils.Global.GlobalEnum;
using System;

namespace Zk.HotelPlatform.Service
{
    [HubName("MessageHub")]
    public class MessageHub : Hub
    {
        private const string _userId = "uid";
        private const string _sessionId = "sid";

        private readonly Dictionary<int, string> _userType = new Dictionary<int, string>()
        {
            { (int)UserType.CUSTOMER,UserType.CUSTOMER.ToString() },
            { (int)UserType.USER,UserType.USER.ToString() },
            { (int)UserType.API,UserType.API.ToString() },
            { (int)UserType.BACKSTAGE,UserType.BACKSTAGE.ToString() },
            { (int)UserType.UNKNOWN,UserType.UNKNOWN.ToString() }
        };

        private readonly ISysUserService _sysUserService;
        public MessageHub(ISysUserService sysUserService)
        {
            _sysUserService = sysUserService;
        }

        [HubMethodName("SendToAll")]
        public void SendToAll(Msg msg)
        {
            var message = new Message()
            {
                Title = "系统通知",
                Body = msg.Body
            };

            switch (msg.To)
            {
                case (int)UserType.CUSTOMER:
                    Clients.Group(UserType.CUSTOMER.ToString()).Notify(message.ToString());
                    break;
                case (int)UserType.BACKSTAGE:
                    Clients.Group(UserType.BACKSTAGE.ToString()).Notify(message.ToString());
                    break;
                case (int)UserType.USER:
                    Clients.Group(UserType.USER.ToString()).Notify(message.ToString());
                    break;
                case (int)UserType.API:
                    Clients.Group(UserType.API.ToString()).Notify(message.ToString());
                    break;
                case (int)UserType.ALL:
                    Clients.All.Notify(message.ToString());
                    break;
                default:
                    Clients.Caller.Notify(new Message()
                    {
                        Title = message.Title,
                        Body = $"消息发送失败,未知的发送对象.({message.Body})",
                        Type = MessageTypeEnum.error.ToString()
                    }.ToString());
                    break;
            }
        }

        public override Task OnReconnected()
        {
            var sid = Context.QueryString[_sessionId];
            if (string.IsNullOrWhiteSpace(sid))
            {
                throw new Exception("连接失败，未知的连接");
            }

            var sysUserSession = _sysUserService.GetUserSessionCache(sid);
            if (sysUserSession == null)
            {
                return Task.FromException(new Exception("连接失败，未知的用户"));
            }

            int.TryParse(Context.QueryString[_userId], out int uid);
            if (uid != sysUserSession.UserId)
            {
                throw new Exception("用户连接错误");
            }

            //var sysUser = _sysUserService.GetUserInfo(uid);

            string userType = _userType[(int)GlobalEnum.UserType.UNKNOWN];
            if (sysUserSession != null)
            {
                _userType.TryGetValue(sysUserSession.UserType, out userType);
            }
            Groups.Add(Context.ConnectionId, userType);

            return base.OnReconnected();
        }

        public override Task OnConnected()
        {
            var sid = Context.QueryString[_sessionId];
            if (string.IsNullOrWhiteSpace(sid))
            {
                throw new Exception("连接失败，未知的连接");
            }

            var sysUserSession = _sysUserService.GetUserSessionCache(sid);
            if (sysUserSession == null)
            {
                throw new Exception("连接失败，未知的用户");
            }

            int.TryParse(Context.QueryString[_userId], out int uid);
            if (uid != sysUserSession.UserId)
            {
                throw new Exception("用户连接错误");
            }

            //var sysUser = _sysUserService.GetUserInfo(uid);

            string userType = _userType[(int)GlobalEnum.UserType.UNKNOWN];
            if (sysUserSession != null)
            {
                _userType.TryGetValue(sysUserSession.UserType, out userType);
            }
            Groups.Add(Context.ConnectionId, userType);

            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            return base.OnDisconnected(stopCalled);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }

    public class Msg
    {
        public string Body { get; set; }
        public int To { get; set; }
    }
}
