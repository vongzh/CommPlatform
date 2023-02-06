using EasyNetQ;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Zk.HotelPlatform.CacheProvider;
using Zk.HotelPlatform.Model;
using Zk.HotelPlatform.Utils.Global;
using Zk.HotelPlatform.Utils.Log;
using static Zk.HotelPlatform.Utils.Global.GlobalEnum;

namespace Zk.HotelPlatform.Service.Impl
{
    public class MessageService : IMessageService
    {
        private readonly IBus _bus = null;
        public MessageService(IBus bus)
        {
            _bus = bus;
        }

        public async void TestPub()
        {
            await _bus.PubSub.PublishAsync(new MessageQueueDemo());
        }

        public async void TestSub()
        {
            _bus.PubSub.Subscribe<MessageQueueDemo>("HotelPlatform.Order.ComprePrice", message =>
             {
                 LogInfoWriter.GetInstance().Info("HotelPlatform.Order.ComprePrice");
                 LogInfoWriter.GetInstance().Info(JsonConvert.SerializeObject(message));
             });

            _bus.PubSub.Subscribe<MessageQueueDemo>("HotelPlatform.Order.OrderLog", message =>
            {
                LogInfoWriter.GetInstance().Info("HotelPlatform.Order.OrderLog");
                LogInfoWriter.GetInstance().Info(JsonConvert.SerializeObject(message));
            });
        }

        /// <summary>
        /// 系统通知到API
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="messageType"></param>
        /// <param name="duration"></param>
        /// <param name="redirect"></param>
        /// <param name="qs"></param>
        public async void NotifyToApi(string token, string msg, MessageTypeEnum messageType, int duration = 10000, string redirect = "", string qs = "")
        {
            try
            {
                string param = $"msg={msg}&messageType={messageType}&duration={duration}&redirect={redirect}&qs={qs}";
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(GlobalConfig.ApiAddress);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    await client.GetAsync($"/Message/Notify?{param}");
                }
            }
            catch (Exception ex)
            {
                LogInfoWriter.GetInstance().Error(ex);
            }
        }

        /// <summary>
        /// 个人通知到API
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="messageType"></param>
        /// <param name="duration"></param>
        /// <param name="redirect"></param>
        /// <param name="qs"></param>
        public async void NotifyToUserApi(string token, int userId, string msg, MessageTypeEnum messageType, int duration = 10000, string redirect = "", string qs = "")
        {
            try
            {
                string param = $"userId={userId}&msg={msg}&messageType={messageType}&duration={duration}&redirect={redirect}&qs={qs}";
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(GlobalConfig.ApiAddress);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    await client.GetAsync($"/Message/NotifyToUser?{param}");

                }
            }
            catch (Exception ex)
            {
                LogInfoWriter.GetInstance().Error(ex);
            }
        }

        /// <summary>
        /// 系统通知
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="messageType"></param>
        /// <param name="duration"></param>
        /// <param name="redirect"></param>
        /// <param name="qs"></param>
        public void Notify(string msg, MessageTypeEnum messageType, int duration = 0, string redirect = "", string qs = "")
        {
            try
            {
                var message = new Message()
                {
                    Title = "系统提示",
                    Body = msg,
                    Redirect = redirect,
                    Duration = duration,
                    Qs = qs,
                    Type = messageType.ToString()
                };
                GlobalHost.ConnectionManager.GetHubContext<MessageHub>().Clients.Group(UserType.BACKSTAGE.ToString()).Notify(message.ToString());
            }
            catch (Exception ex)
            {
                LogInfoWriter.GetInstance().Error(ex);
            }
        }

        /// <summary>
        /// 系统通知到个人
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="messageType"></param>
        /// <param name="duration"></param> 
        /// <param name="redirect"></param>
        /// <param name="qs"></param>
        public void NotifyToUser(int userId, string msg, MessageTypeEnum messageType, int duration = 0, string redirect = "", string qs = "")
        {
            try
            {
                var message = new Message()
                {
                    Title = "系统提示",
                    Body = msg,
                    Redirect = redirect,
                    Duration = duration,
                    Qs = qs,
                    Type = messageType.ToString(),
                    PopupWinType = "msgbox"
                };
                GlobalHost.ConnectionManager.GetHubContext<MessageHub>().Clients.User(userId.ToString()).Notify(message.ToString());
            }
            catch (Exception ex)
            {
                LogInfoWriter.GetInstance().Error(ex);
            }
        }

    }

    [Queue("MessageQueueDemo", ExchangeName = "ExchangeDemo")]
    public class MessageQueueDemo
    {
        public string Content { get; set; } = "Hi";
        public DateTime Date { get; set; } = DateTime.Now;
    }
}
