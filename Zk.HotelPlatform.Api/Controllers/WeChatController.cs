using Senparc.Weixin;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.AdvancedAPIs;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using Zk.HotelPlatform.Service;

namespace Zk.HotelPlatform.Api.Controllers
{
    public class WeChatController : BaseController
    {
        private readonly string _token = Config.SenparcWeixinSetting.Token;
        private readonly string _encodingAESKey = Config.SenparcWeixinSetting.EncodingAESKey;
        private readonly string _appId = Config.SenparcWeixinSetting.WeixinAppId;
        private readonly string _appSecret = Config.SenparcWeixinSetting.WeixinAppSecret;

        private readonly IWeChatService _weChatService = null;

        public WeChatController(IWeChatService weChatService)
        {
            _weChatService = weChatService;
        }

        [HttpGet]
        public HttpResponseMessage Index(string signature, string timestamp, string nonce, string echostr)
        {
            if (CheckSignature.Check(signature, timestamp, nonce, _token))
                return new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(echostr, Encoding.UTF8, "text/plain")
                };

            return new HttpResponseMessage(HttpStatusCode.NoContent);
        }

        [HttpGet]
        public string GetOpenId(string code)
        {
            var result = OAuthApi.GetAccessToken(_appId, _appSecret, code);
            if (result.errcode != ReturnCode.请求成功)
                throw new System.Exception(result.errmsg);

            return result.openid;
        }
    }
}
