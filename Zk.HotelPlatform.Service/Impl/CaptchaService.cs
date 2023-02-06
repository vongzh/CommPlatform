using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TencentCloud.Captcha.V20190722;
using TencentCloud.Captcha.V20190722.Models;
using TencentCloud.Common;
using Zk.HotelPlatform.CacheModel;
using Zk.HotelPlatform.CacheProvider;
using Zk.HotelPlatform.DBProvider;
using Zk.HotelPlatform.Model;
using Zk.HotelPlatform.Utils;
using Zk.HotelPlatform.Utils.Global;

namespace Zk.HotelPlatform.Service.Impl
{
    public class CaptchaService : DataService<Captcha>, ICaptchaService
    {
        private readonly ICaptchaCacheProvider _captchaCacheProvider = null;
        private readonly ISysConfigService _sysConfigService = null;
        public CaptchaService(IDataProvider<Captcha> dataProvider,ICaptchaCacheProvider captchaCacheProvider, ISysConfigService sysConfigService)
            : base(dataProvider)
        {
            this._captchaCacheProvider = captchaCacheProvider;
            this._sysConfigService = sysConfigService;
        }

        /// <summary>
        /// 验证滑动验证
        /// </summary>
        /// <param name="mobile"></param>
        /// <param name="randstr"></param>
        /// <param name="ticket"></param>
        /// <param name="ret"></param>
        /// <returns></returns>
        public (bool Result, string RequestId) SlideVerification(string mobile, string randstr, string ticket, int ret, string ip, GlobalEnum.CaptchaType captchaType)
        {
            try
            {
                CaptchaClient captchaClient = new CaptchaClient(new Credential
                {
                    SecretId = GlobalConfig.TencentCloud_SecretId,
                    SecretKey = GlobalConfig.TencentCloud_SecretKey,

                }, GlobalConfig.TencentCloud_Region);

                DescribeCaptchaResultResponse describeCaptchaResult = captchaClient.DescribeCaptchaResult(new DescribeCaptchaResultRequest()
                {
                    CaptchaType = 9,
                    Ticket = ticket,
                    UserIp = ip,
                    Randstr = randstr,
                    CaptchaAppId = GlobalConfig.TencentCloud_Captcha_AppId,
                    AppSecretKey = GlobalConfig.TencentCloud_Captcha_AppSecret,
                }).Result;

                if (describeCaptchaResult.CaptchaCode != (int)CaptchaCode.OK)
                {
                    throw new BusinessException(describeCaptchaResult.CaptchaMsg);
                }

                Task.Run(() =>
                {
                    base.AddEntity(new Captcha()
                    {
                        Time = DateTime.Now,
                        Mobile = mobile,
                        IP = ip,
                        IsUse = (int)GlobalEnum.YESOrNO.N,
                        CaptchaCode = Convert.ToInt32(describeCaptchaResult.CaptchaCode.Value),
                        EvilLevel = Convert.ToInt32(describeCaptchaResult.EvilLevel.Value),
                        RequestId = describeCaptchaResult.RequestId,
                        CaptchaType = (int)captchaType
                    });
                });
                return (true, describeCaptchaResult.RequestId);
            }
            catch (Exception ex)
            {
                throw new BusinessException("验证失败");
            }
        }

        #region 验证邮箱安全码
        public bool VerifyMailCaptcha(string mail, string code, GlobalEnum.CaptchaType captchType)
        {
            var captchaUse = _captchaCacheProvider.GetCaptcha(captchType.ToString(), mail);
            if (captchaUse == null)
                throw new BusinessException("请重新获取验证");

            if (captchaUse.ExpireTime < DateTime.Now)
            {
                throw new BusinessException("安全码已失效");
            }
            if (captchaUse.Code != code)
                throw new BusinessException("安全码错误");

            return true;
        }

        /// <summary>
        /// 发送安全码邮件+redis存储
        /// </summary>
        /// <param name="mail"></param>
        /// <param name="requestId"></param>
        /// <param name="captchType"></param>
        /// <param name="verifyWay"></param>
        public bool SendCaptchaMail(string mailUrl, string code)
        {
            try
            {
                Mail mail = new Mail();
                mail.From = _sysConfigService.GetConfig<string>("business", "mailfrom");
                mail.To = mailUrl;
                mail.Subject = "来自众客网络的邮箱验证";
                string body = $@"您好<br>为了避免您错过后续相关通知事宜,需要您验证邮箱地址<br>请在页面上输入此安全码:{code}<br><br>如果你没有请求此代码，可忽略这封电子邮件<br>谢谢!<br>众客团队";
                mail.Body = body;
                mail.Smtp = new SMTP()
                {
                    SmtpHost = _sysConfigService.GetConfig<string>("business", "SmtpHost"),
                    SmtpPort = _sysConfigService.GetConfig<int>("business", "SmtpPort"),
                    UserName = _sysConfigService.GetConfig<string>("business", "SmtpUserName"),
                    UserPwd = _sysConfigService.GetConfig<string>("business", "SmtpUserPwd")
                };
                //发送邮件
                MailUtil.SendMail(mail);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// 发送支付密码到客户邮箱
        /// </summary>
        /// <param name="mail"></param>
        /// <param name="requestId"></param>
        /// <param name="captchType"></param>
        /// <param name="verifyWay"></param>
        public bool SendPayCodeMail(string mailUrl, string code)
        {
            try
            {
                Mail mail = new Mail();
                mail.From = _sysConfigService.GetConfig<string>("business", "mailfrom");
                mail.To = mailUrl;
                mail.Subject = "51住钱包支付密码";
                string body = $@"您好<br>此安全码:{code}为您在51zhu钱包的支付密码，请妥善保存。<br>河南众客";
                mail.Body = body;
                mail.Smtp = new SMTP()
                {
                    SmtpHost = _sysConfigService.GetConfig<string>("business", "SmtpHost"),
                    SmtpPort = _sysConfigService.GetConfig<int>("business", "SmtpPort"),
                    UserName = _sysConfigService.GetConfig<string>("business", "SmtpUserName"),
                    UserPwd = _sysConfigService.GetConfig<string>("business", "SmtpUserPwd")
                };
                //发送邮件
                MailUtil.SendMail(mail);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        ///// <summary>
        ///// 设置邮箱验证码缓存
        ///// </summary>
        ///// <param name="mail"></param>
        ///// <param name="requestId"></param>
        ///// <param name="captchType"></param>
        ///// <param name="ip"></param>
        ///// <returns></returns>
        //public bool SetVerifyMailCaptcha(string mail, GlobalEnum.CaptchaType captchType)
        //{
        //    var existCaptcha = _captchaCacheProvider.GetCaptcha(captchType.ToString(), mail);
        //    if (existCaptcha != null)
        //    {
        //        if (existCaptcha.SendTime.AddMinutes(1) > DateTime.Now)
        //        {
        //            throw new BusinessException("验证过于频繁，请稍后再试");
        //        }
        //        else
        //        {
        //            var removeCapacha = _captchaCacheProvider.RemoveMailCaptcha(captchType.ToString(), mail);
        //            if (!removeCapacha)
        //            {
        //                throw new BusinessException("验证失败了，请稍后重试");
        //            }
        //        }
        //    }
        //    var captchaUse = new CaptchaUse()
        //    {
        //        SendTime = DateTime.Now,
        //        Code = RandomUtil.RandomNumber(4)
        //    };
        //    var setRet = _captchaCacheProvider.SetMailCaptcha(captchType.ToString(), mail, captchaUse);
        //    return setRet;
        //}
        #endregion
        /// <summary>
        /// 发送手机/邮箱验证码
        /// </summary>
        /// <param name="mobile"></param>
        /// <returns></returns>
        public bool SendCaptcha(string captchaInfo, string code, string requestId, GlobalEnum.CaptchaType captchType, string ip)
        {
            // IpSend ipSend = new IpSend();
            int sendCount = 0;
            var mailLimitCount = _sysConfigService.GetMailSendLimitCount();
            // int phoneSendSum = _captchaCacheProvider.GetCaptchaCount_Mobile(captchType.ToString(), mobile);
            var existCaptcha = _captchaCacheProvider.GetCaptcha(captchType.ToString(), captchaInfo);
            if (existCaptcha != null)
            {
                sendCount = existCaptcha.SendCount;
                if (existCaptcha.SendTime.AddMinutes(1) > DateTime.Now)
                {
                    throw new BusinessException("验证过于频繁，请稍后再试");
                }

                if (existCaptcha.SendCount >= mailLimitCount)
                {
                    throw new BusinessException("发送次数已到上限");
                }

            }

            #region IP
            //var list = _captchaCacheProvider.GetCaptchaCount_IP(captchType.ToString());
            //if (list != null && list.Count > 0)
            //{
            //    list = list.Where(x => x.Ip == ip).ToList();
            //    if (list != null && list.Count() > 0)
            //    {
            //        ipSend = list.FirstOrDefault();
            //    }
            //}
            //else
            //{
            //    ipSend.Ip = ip;
            //}

            //if (phoneSendSum >= 10 || ipSend?.SendCount >= 10)
            //{
            //    throw new BusinessException("当天验证码次数已超");
            //}
            #endregion

            if (string.IsNullOrEmpty(code))
            {
                code = RandomUtil.RandomNumber(4);
            }
            var captchaUse = new CaptchaUse()
            {
                RequestId = requestId,
                SendTime = DateTime.Now,
                Code = code,
                ExpireTime = DateTime.Now.AddMinutes(5),
                SendCount = sendCount + 1,
                Valid = (int)GlobalEnum.YESOrNO.Y
            };

            _captchaCacheProvider.SetCaptcha(captchType.ToString(), captchaInfo, captchaUse);

            //phoneSendSum++;
            //ipSend.SendCount++;
            // var ipSendRe = _captchaCacheProvider.SetCaptchaCount_IP(captchType.ToString(), ipSend);
            //var phoneSendRe = _captchaCacheProvider.SetCaptchaCount_Mobile(captchType.ToString(), mobile, phoneSendSum);

            switch (captchType)
            {
                case GlobalEnum.CaptchaType.REGISTER:
                    return false;
                case GlobalEnum.CaptchaType.FINDPWD:
                    return false;
                case GlobalEnum.CaptchaType.SAVESUCTOMER:
                    return this.SendCaptchaMail(captchaInfo, captchaUse.Code);
                case GlobalEnum.CaptchaType.CHANGE_EMAIL:
                    return this.SendCaptchaMail(captchaInfo, captchaUse.Code);
                case GlobalEnum.CaptchaType.CHANGEUSERINFO:
                    return this.SendCaptchaMail(captchaInfo, captchaUse.Code);
                default:
                    throw new BusinessException("验证失败");
            }
        }

        /// <summary>
        /// 是否包含关键词
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public bool SensitiveWordFilter(string word)
        {
            var words = _captchaCacheProvider.GetSensitiveWord();
            if (words != null && words.Count > 0)
            {
                return words.Contains(word);
            }
            return false;
        }

        /// <summary>
        /// 验证验证码
        /// </summary>
        /// <param name="mobile"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public (bool Result, string RequestId) VerifyCaptcha(string mobile, string code, GlobalEnum.CaptchaType captchType)
        {
            var captchaUse = _captchaCacheProvider.GetCaptcha(captchType.ToString(), mobile);
            if (captchaUse == null)
                throw new BusinessException("请获取验证码");
            if (captchaUse.Valid != (int)GlobalEnum.YESOrNO.Y)
            {
                throw new BusinessException("验证码已失效");
            }
            if (captchaUse.ExpireTime < DateTime.Now)
                throw new BusinessException("验证码已失效");
            if (captchaUse.Code != code)
                throw new BusinessException("验证码错误");


            //校验验证码 RequestId 为滑动验证码的请求ID  为了测试注册用户 暂时先注释
            //var captcha = base.Get(x => x.Mobile == mobile);
            //var captcha = base.Get(x => x.Mobile == mobile && x.RequestId == captchaUse.RequestId);
            //if (captcha == null)
            //    throw new BusinessException("非法验证码");
            //captcha.EndRequestId = Guid.NewGuid().ToString();
            //captcha.EndTime = DateTime.Now;
            //if (base.Update(captcha))
            //{
                //验证码作废
                captchaUse.Valid = (int)GlobalEnum.YESOrNO.N;
                _captchaCacheProvider.SetCaptcha(captchType.ToString(), mobile, captchaUse);
            //_captchaCacheProvider.RemoveCaptcha(captchType.ToString(), mobile);
            //return (true, captcha.EndRequestId);
            // }
            //return (false, null);
            return (true, null);
        }

        /// <summary>
        /// 注册完成
        /// </summary>
        /// <param name="mobile"></param>
        /// <param name="requestId"></param>
        /// <returns></returns>
        public bool IsUse(string mobile, string requestId)
        {
            var captcha = base.Get(x => x.EndRequestId == requestId && x.Mobile == mobile);
            captcha.IsUse = (int)GlobalEnum.YESOrNO.Y;
            return base.Update(captcha);
        }

        /// <summary>
        /// 获取验证码
        /// </summary>
        /// <param name="mobile"></param>
        /// <param name="requestId"></param>
        /// <returns></returns>
        public Captcha GetCaptcha(string mobile, string requestId)
        {
            return base.Get(x => x.EndRequestId == requestId && x.Mobile == mobile && x.IsUse == (int)GlobalEnum.YESOrNO.N);
        }

        /// <summary>
        /// 使用验证码
        /// </summary>
        /// <param name="captcha"></param>
        /// <returns></returns>
        public bool IsUse(Captcha captcha)
        {
            captcha.IsUse = (int)GlobalEnum.YESOrNO.Y;
            return base.Update(captcha);
        }
    }

    public enum CaptchaCode
    {
        OK = 1
    }
}
