using Microsoft.Owin;
using Newtonsoft.Json;
using Owin;
using Senparc.NeuChar.Context;
using Senparc.NeuChar.Entities;
using Senparc.Weixin;
using Senparc.Weixin.Work.AdvancedAPIs;
using Senparc.Weixin.Work.Containers;
using Senparc.Weixin.Work.Entities;
using Senparc.Weixin.Work.MessageHandlers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Zk.HotelPlatform.Utils.Log;

namespace Zk.HotelPlatform.Api.Middleware
{
    public class WeChatWorkMiddleware : OwinMiddleware
    {
        private const string _logName = "WeChatWorkMiddleware";

        public WeChatWorkMiddleware(OwinMiddleware next) : base(next)
        {

        }

        public override Task Invoke(IOwinContext context)
        {
            if (context.Request.Path.HasValue && context.Request.Path.Value.ToLower().Contains("/WechatWork".ToLower()))
            {
                var corpId = Config.SenparcWeixinSetting.WorkSetting.WeixinCorpId;
                var corpAgentId = Config.SenparcWeixinSetting.WorkSetting.WeixinCorpAgentId;
                var encodingAESKey = Config.SenparcWeixinSetting.WorkSetting.WeixinCorpEncodingAESKey;
                var token = Config.SenparcWeixinSetting.WorkSetting.WeixinCorpToken;

                try
                {
                    switch (context.Request.Method)
                    {
                        case "GET":

                            if (context.Request.Path.Value.ToLower().Contains("/push".ToLower()))
                            {
                                string toUser = "GuaiRen";
                                if (!string.IsNullOrWhiteSpace(context.Request.Query["toUser"]))
                                {
                                    toUser = context.Request.Query["toUser"];
                                }
                                string title = "系统通知";
                                if (!string.IsNullOrWhiteSpace(context.Request.Query["title"]))
                                {
                                    title = context.Request.Query["title"];
                                }
                                string desc = "来自Api的通知，点击去查看";
                                if (!string.IsNullOrWhiteSpace(context.Request.Query["desc"]))
                                {
                                    desc = context.Request.Query["desc"];
                                }
                                string url = "https://www.51zhu.cn";
                                if (!string.IsNullOrWhiteSpace(context.Request.Query["url"]))
                                {
                                    url = context.Request.Query["url"];
                                }

                                //发送一条客服消息
                                var weixinSetting = Config.SenparcWeixinSetting.WorkSetting;
                                var appKey = AccessTokenContainer.BuildingKey(weixinSetting.WeixinCorpId, weixinSetting.WeixinCorpSecret);
                                MassApi.SendTextCard(AccessTokenContainer.GetToken(appKey), weixinSetting.WeixinCorpAgentId,
                                    title, desc, url, toUser: toUser);

                                return Task.CompletedTask;
                            }

                            var query = context.Request.Query;
                            if (query.Count() > 0)
                            {
                                var msg_signature = query["msg_signature"];
                                var timestamp = query["timestamp"];
                                var nonce = query["nonce"];
                                var echostr = query["echostr"];

                                var verifyUrl = Senparc.Weixin.Work.Signature.VerifyURL(token, encodingAESKey, corpId,
                                    msg_signature, timestamp, nonce, echostr);

                                if (verifyUrl != null)
                                {
                                    context.Response.StatusCode = (int)HttpStatusCode.OK;
                                    context.Response.ContentType = "text/plain";

                                    context.Response.WriteAsync(verifyUrl);

                                    return Task.CompletedTask;
                                }
                            }
                            break;
                        case "POST":

                            if (context.Request.Path.Value.ToLower().Contains("/push".ToLower()))
                            {
                                if (context.Request.Query.Count() <= 0)
                                {
                                    return Task.CompletedTask;
                                }

                                var source = context.Request.Query["source"];
                                switch (source)
                                {
                                    case "Grafana":
                                        {
                                            string content = string.Empty;
                                            using (StreamReader reader = new StreamReader(context.Request.Body))
                                            {
                                                content = reader.ReadToEnd();
                                            }

                                            if (string.IsNullOrEmpty(content))
                                            {
                                                return Task.CompletedTask;
                                            }

                                            //发送一条客服消息
                                            string toUser = "GuaiRen";
                                            if (!string.IsNullOrWhiteSpace(context.Request.Query["receiver"]))
                                            {
                                                toUser = context.Request.Query["receiver"];
                                            }

                                            var weixinSetting = Config.SenparcWeixinSetting.WorkSetting;
                                            MassApi.SendNews(AccessTokenContainer.TryGetToken(weixinSetting.WeixinCorpId, weixinSetting.WeixinCorpSecret), weixinSetting.WeixinCorpAgentId,
                                                new List<Article>()
                                                {
                                                new Article()
                                                {
                                                    Title = "警告",
                                                    Url = string.Empty,
                                                    Description = "警告消息",
                                                    PicUrl = string.Empty
                                                }
                                                }, toUser: toUser);
                                        }
                                        break;
                                    case "CAT":
                                        {
                                            string catContent = string.Empty;
                                            using (StreamReader reader = new StreamReader(context.Request.Body))
                                            {
                                                catContent = reader.ReadToEnd();
                                            }

                                            if (string.IsNullOrEmpty(catContent))
                                            {
                                                return Task.CompletedTask;
                                            }


                                            {
                                                //发送一条客服消息
                                                string toUser = "GuaiRen";

                                                var weixinSetting = Config.SenparcWeixinSetting.WorkSetting;
                                                MassApi.SendTextCard(AccessTokenContainer.TryGetToken(weixinSetting.WeixinCorpId, weixinSetting.WeixinCorpSecret), weixinSetting.WeixinCorpAgentId,
                                                   string.Empty, string.Empty, string.Empty, toUser: toUser);
                                            }
                                        }
                                        break;
                                    default:
                                        break;
                                }

                                return Task.CompletedTask;
                            }

                            int maxRecord = 10;
                            var postModel = new PostModel()
                            {
                                CorpId = corpId,
                                CorpAgentId = corpAgentId,
                                Token = token,
                                EncodingAESKey = encodingAESKey,
                            };

                            var postQuery = context.Request.Query;
                            if (postQuery.Count() > 0)
                            {
                                postModel.Msg_Signature = postQuery["msg_signature"];
                                postModel.Timestamp = postQuery["timestamp"];
                                postModel.Nonce = postQuery["nonce"];
                            }

                            {
                                LogInfoWriter.GetInstance(_logName).Info($"微信请求：{JsonConvert.SerializeObject(postModel)}");
                            }

                            var messageHandler = new WorkCustomMessageHandler(context.Request.Body, postModel, maxRecord);

                            messageHandler.ExecuteAsync(CancellationToken.None);

                            LogInfoWriter.GetInstance(_logName).Info($"微信请求响应：{JsonConvert.SerializeObject(messageHandler)}");

                            context.Response.StatusCode = (int)HttpStatusCode.OK;
                            context.Response.ContentType = "text/xml";
                            context.Response.WriteAsync(messageHandler.FinalResponseDocument.ToString());
                            return Task.CompletedTask;
                        default:
                            break;
                    }

                    context.Response.StatusCode = (int)HttpStatusCode.OK;
                    context.Response.ContentType = "text/plain";
                    context.Response.WriteAsync("unknown");
                    return Task.CompletedTask;
                }
                catch (Exception ex)
                {
                    LogInfoWriter.GetInstance(_logName).Error(ex);

                    context.Response.StatusCode = (int)HttpStatusCode.OK;
                    context.Response.ContentType = "text/plain";
                    context.Response.WriteAsync(ex.Message);
                    return Task.CompletedTask;
                }
            }

            return Next.Invoke(context);
        }
    }

    public class WorkCustomMessageHandler : WorkMessageHandler<WorkCustomMessageContext>
    {
        public WorkCustomMessageHandler(Stream inputStream, PostModel postModel, int maxRecordCount = 0)
            : base(inputStream, postModel, maxRecordCount)
        {

        }

        public override IWorkResponseMessageBase OnTextRequest(RequestMessageText requestMessage)
        {
            var responseMessage = this.CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "您发送了消息：" + requestMessage.Content;

            //发送一条客服消息
            var weixinSetting = Config.SenparcWeixinSetting.WorkSetting;
            MassApi.SendText(AccessTokenContainer.TryGetToken(weixinSetting.WeixinCorpId, weixinSetting.WeixinCorpSecret), weixinSetting.WeixinCorpAgentId, "这是一条客服消息，对应您发送的消息：" + requestMessage.Content, OpenId);

            return responseMessage;
        }

        //public override IWorkResponseMessageBase OnImageRequest(RequestMessageImage requestMessage)
        //{
        //    var responseMessage = CreateResponseMessage<ResponseMessageImage>();
        //    responseMessage.Image.MediaId = requestMessage.MediaId;
        //    return responseMessage;
        //}

        //public override IWorkResponseMessageBase OnEvent_PicPhotoOrAlbumRequest(RequestMessageEvent_Pic_Photo_Or_Album requestMessage)
        //{
        //    var responseMessage = this.CreateResponseMessage<ResponseMessageText>();
        //    responseMessage.Content = "您刚发送的图片如下：";
        //    return responseMessage;
        //}

        //public override IWorkResponseMessageBase OnEvent_EnterAgentRequest(RequestMessageEvent_Enter_Agent requestMessage)
        //{
        //    var responseMessage = this.CreateResponseMessage<ResponseMessageText>();
        //    responseMessage.Content = "欢迎进入应用！现在时间是：" + SystemTime.Now.DateTime.ToString();
        //    return responseMessage;
        //}

        public override Senparc.Weixin.Work.Entities.IWorkResponseMessageBase DefaultResponseMessage(Senparc.Weixin.Work.Entities.IWorkRequestMessageBase requestMessage)
        {
            var responseMessage = this.CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = $"监控正在运行：{DateTime.Now}";
            return responseMessage;
        }
    }

    public class WorkCustomMessageContext : Senparc.Weixin.Work.MessageContexts.DefaultWorkMessageContext, IMessageContext<IWorkRequestMessageBase, IWorkResponseMessageBase>
    {
        public WorkCustomMessageContext()
        {
            base.MessageContextRemoved += CustomMessageContext_MessageContextRemoved;
        }

        void CustomMessageContext_MessageContextRemoved(object sender, Senparc.NeuChar.Context.WeixinContextRemovedEventArgs<IWorkRequestMessageBase, IWorkResponseMessageBase> e)
        {
            var messageContext = e.MessageContext as WorkCustomMessageContext;
            if (messageContext == null)
            {
                return;
            }
        }
    }

    public static class WeChatWorkMiddlewareExtend
    {
        public static IAppBuilder UseWeChatMiddlewareForWork(this IAppBuilder app)
        {
            return app.Use(typeof(WeChatWorkMiddleware));
        }
    }
}
