using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aop.Api.Domain;
using Senparc.Weixin.Entities;
using Senparc.Weixin.Open.Containers;
using Senparc.Weixin.Open.QRConnect;
using Zk.HotelPlatform.DBProvider;
using Zk.HotelPlatform.Model;
using Zk.HotelPlatform.Utils;

namespace Zk.HotelPlatform.Service.Impl
{
    public class WeChatService : DataService<WeChatUserBind>, IWeChatService
    {
        private readonly SenparcWeixinSetting senparcWeixinSetting = SenparcWeixinSetting.BuildFromWebConfig(false);

        public WeChatService(IDataProvider<WeChatUserBind> dataProvider)
             : base(dataProvider)
        {

        }

        public int GetWechatBindUser(string code)
        {
            QRConnectAccessTokenResult qRConnectAccessToken = this.GetAccessToken(code);

            var wechatUserBind = base.Get(x => x.OpenId == qRConnectAccessToken.openid);
            if (wechatUserBind == null)
            {
                base.AddEntity(new WeChatUserBind()
                {
                    CreateTime = DateTime.Now,
                    OpenId = qRConnectAccessToken.openid,
                    UnionId = qRConnectAccessToken.unionid,
                    AccessToken = qRConnectAccessToken.access_token,
                    RefreshToken = qRConnectAccessToken.refresh_token,
                    Scope = qRConnectAccessToken.scope,
                    ExpiresTime = DateTime.Now.AddMilliseconds(qRConnectAccessToken.expires_in - (600 * 5))
                });
            }
            else
            {
                wechatUserBind.AccessToken = qRConnectAccessToken.access_token;
                wechatUserBind.RefreshToken = qRConnectAccessToken.refresh_token;
                wechatUserBind.Scope = qRConnectAccessToken.scope;
                wechatUserBind.ExpiresTime = DateTime.Now.AddMilliseconds(qRConnectAccessToken.expires_in - (600 * 5));
                if (!string.IsNullOrWhiteSpace(wechatUserBind.UnionId))
                {
                    wechatUserBind.UnionId = qRConnectAccessToken.unionid;
                }
                base.Update(wechatUserBind);
            }
           
            return wechatUserBind.UserId;
        }

        private QRConnectAccessTokenResult GetAccessToken(string code)
        {
            return QRConnectAPI.GetAccessToken(senparcWeixinSetting.Component_Appid, senparcWeixinSetting.Component_Secret, code);
        }
    }
}
