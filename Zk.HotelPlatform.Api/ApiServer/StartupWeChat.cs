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
                .UseSenparcGlobal()
                .RegisterTraceLog(ConfigWeixinTraceLog);

            //缓存策略
            //Senparc.CO2NET.Cache.Redis.Register.SetConfigurationOption(senparcSetting.Cache_Redis_Configuration);
            //Senparc.CO2NET.Cache.Redis.Register.UseKeyValueRedisNow();

            //微信配置
            var senparcWeixinSetting = SenparcWeixinSetting.BuildFromWebConfig(isDebug);
            register.UseSenparcWeixin(senparcWeixinSetting, senparcSetting)

                //微信支付
                .RegisterTenpayV3(senparcWeixinSetting, "【众客网络】")

                .RegisterWorkAccount(senparcWeixinSetting, "【众客网络】企业微信")

                //微信开放平台
                .RegisterOpenComponent(senparcWeixinSetting,
                 async componentAppId =>
                 {
                     return string.Empty;
                 },
                 async (componentAppId, auhtorizerId) =>
                 {
                     return string.Empty;
                 },
                async (componentAppId, auhtorizerId, refreshResult) =>
                {

                }, "【众客网络】开放平台");

            app.UseWeChatMiddlewareForWork();
        }

        /// <summary>
        /// 配置微信跟踪日志
        /// </summary>
        private void ConfigWeixinTraceLog()
        {
            //Senparc.CO2NET.Config.IsDebug = false;

            //这里设为Debug状态时，/App_Data/WeixinTraceLog/目录下会生成日志文件记录所有的API请求日志，正式发布版本建议关闭
            Senparc.Weixin.WeixinTrace.SendCustomLog("系统日志", "系统启动");//只在Senparc.Weixin.Config.IsDebug = true的情况下生效

            //自定义日志记录回调
            Senparc.Weixin.WeixinTrace.OnLogFunc = () =>
            {

            };

            //当发生基于WeixinException的异常时触发
            Senparc.Weixin.WeixinTrace.OnWeixinExceptionFunc = async ex =>
            {
                //每次触发WeixinExceptionLog后需要执行的代码
            };
        }
    }
}
