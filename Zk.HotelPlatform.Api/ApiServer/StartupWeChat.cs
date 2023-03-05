using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Senparc.CO2NET;
using Senparc.CO2NET.RegisterServices;
using Senparc.Weixin;
using Senparc.Weixin.Entities;
using Senparc.Weixin.Open;
using Senparc.Weixin.Work;
using Senparc.Weixin.TenPay;
using Owin;
using Zk.HotelPlatform.Api.Middleware;
using Senparc.Weixin.MP;

namespace Zk.HotelPlatform.Api
{
    public class StartupWeChat
    {
        private static readonly Lazy<StartupWeChat> LazyInstance = new Lazy<StartupWeChat>();
        public static StartupWeChat Instance => LazyInstance.Value;

        /// <summary>
        /// 配置微信注册
        /// </summary>
        public void ConfigureWebChat(IAppBuilder app)
        {
            bool isDebug = false;
#if DEBUG
            isDebug = true;
#endif

            //全局配置
            var senparcSetting = SenparcSetting.BuildFromWebConfig(isDebug);
            IRegisterService register = RegisterService.Start(senparcSetting)
                .UseSenparcGlobal();

            //缓存策略
            //Senparc.CO2NET.Cache.Memcached.Register.SetConfigurationOption(senparcSetting.Cache_Memcached_Configuration);
            //Senparc.CO2NET.Cache.Memcached.Register.UseMemcachedNow();

            //微信配置
            var senparcWeixinSetting = SenparcWeixinSetting.BuildFromWebConfig(isDebug);
            register.UseSenparcWeixin(senparcWeixinSetting, senparcSetting)

                .RegisterMpAccount(senparcWeixinSetting)

                .RegisterMpJsApiTicket(senparcWeixinSetting.WeixinAppId, senparcWeixinSetting.WeixinAppSecret, "轻易学付");
        }
    }
}
