using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zk.HotelPlatform.CacheModel;
//using Zk.HotelPlatform.Utils.Cache;
using Zk.HotelPlatform.Utils;
using RedisClientAchieve;

namespace Zk.HotelPlatform.CacheProvider.Impl
{
    public class CaptchaCacheProvider : ICaptchaCacheProvider
    {
        private const string _captchaKey = "HotelPlatform:Captcha";
        //private const string _captchaKeyMail = "HotelPlatform:MailCaptcha";
        private const string _captchaCountKey = "HotelPlatform:CaptchaCount";

        /// <summary>
        /// 设置手机号验证码
        /// </summary>
        /// <param name="type"></param>
        /// <param name="dataKey"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public bool SetCaptcha(string type, string dataKey, CaptchaUse val)
        {
            var _key = $"{_captchaKey}:{type}";
            return RedisEntities.Default.HashSet(_key, dataKey, val, TimeSpan.FromDays(1));
            //var result = RedisUtil.Hash_Set(_key, dataKey, val, RedisCacheConfig.DBId);
            //if (result)
            //{
            //    DateTime td = DateTime.Now.Date;
            //    var expireTime = td.AddDays(1);
            //    RedisUtil.SetExpire(_key, expireTime, RedisCacheConfig.DBId);
            //}
            //return result;
        }

        ///// <summary>
        ///// 设置邮箱验证码
        ///// </summary>
        ///// <param name="type"></param>
        ///// <param name="dataKey"></param>
        ///// <param name="val"></param>
        ///// <returns></returns>
        //public bool SetMailCaptcha(string type, string dataKey, CaptchaUse val)
        //{
        //    var _key = $"{_captchaKeyMail}:{type}";
        //    var result = RedisUtil.Hash_Set(_key, dataKey, val, RedisCacheConfig.DBId);
        //    if (result)
        //    {
        //        RedisUtil.SetExpire(_key, DateTime.Now.AddMinutes(5), RedisCacheConfig.DBId);
        //    }
        //    return result;
        //}

        ///// <summary>
        ///// 获取邮箱验证码
        ///// </summary>
        ///// <param name="type"></param>
        ///// <param name="dataKey"></param>
        ///// <param name="val"></param>
        ///// <returns></returns>
        //public CaptchaUse GetMailCaptcha(string type, string dataKey)
        //{
        //    var _key = $"{_captchaKeyMail}:{type}";
        //    return RedisUtil.Hash_Get<CaptchaUse>(_key, dataKey, RedisCacheConfig.DBId);
        //}

        ///// <summary>
        /////删除邮箱安全码
        ///// </summary>
        ///// <param name="type"></param>
        ///// <param name="dataKey"></param>
        ///// <returns></returns>
        //public bool RemoveMailCaptcha(string type, string dataKey)
        //{
        //    var _key = $"{_captchaKeyMail}:{type}:";
        //    return RedisUtil.Hash_Remove(_key, dataKey, RedisCacheConfig.DBId);
        //}

        /// <summary>
        /// 设置手机发送验证码次数
        /// </summary>
        /// <param name="type"></param>
        /// <param name="dataKey"></param>
        /// <param name="sendCount"></param>
        /// <returns></returns>
        //public bool SetCaptchaCount_Mobile(string type, string dataKey, int sendCount)
        //{
        //    var _key = $"{_captchaCountKey}:{type}:Mobile";
        //    var result = RedisEntities.Default.HashSet(_key, dataKey, sendCount.ToString());
        //    if (result)
        //    {
        //        RedisEntities.Default.ExpireEntryAt(_key, DateTime.Now.Date.AddDays(1).AddMinutes(-1));
        //    }
        //    return result;
        //    //var result = RedisUtil.Hash_Set(_key, dataKey, sendCount.ToString(), RedisCacheConfig.DBId);
        //    //if (result)
        //    //{
        //    //    //当天生效
        //    //    RedisUtil.SetExpire(_key, DateTime.Now.AddDays(1).AddSeconds(-1), RedisCacheConfig.DBId);
        //    //}
        //    //return result;
        //}

        /// <summary>
        /// 获取手机发送验证码次数
        /// </summary>
        /// <param name="type"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        //public int GetCaptchaCount_Mobile(string type, string dataKey)
        //{
        //    var _key = $"{_captchaCountKey}:{type}:Mobile";

        //    int count = 0;
        //    if (RedisEntities.Default.HashExistField(_key, dataKey))
        //    {
        //        int.TryParse(RedisEntities.Default.HashGet<string>(_key, dataKey), out count);
        //    }
        //    return count;
        //    //int count = 0;
        //    //if (RedisUtil.Hash_Get(_key, dataKey, RedisCacheConfig.DBId) != null)
        //    //{
        //    //    count = Convert.ToInt32(RedisUtil.Hash_Get(_key, dataKey, RedisCacheConfig.DBId));
        //    //}
        //    //return count;
        //}

        /// <summary>
        /// 设置IP发送验证码次数
        /// </summary>
        /// <param name="type"></param>
        /// <param name="sendCount"></param>
        /// <returns></returns>
        //public bool SetCaptchaCount_IP(string type, IpSend ipSend)
        //{
        //    var _key = $"{_captchaCountKey}:{type}:IP";
        //    var list = GetCaptchaCount_IP(type);
        //    if (list != null)
        //    {
        //        var model = list.Where(x => x.Ip == ipSend.Ip).FirstOrDefault();
        //        model.SendCount = ipSend.SendCount;
        //    }
        //    else
        //    {
        //        list = new List<IpSend>();
        //        list.Add(ipSend);
        //    }

        //    var result = RedisEntities.Default.ItemSet<List<IpSend>>(_key, list);
        //    if (result)
        //    {
        //        RedisEntities.Default.ExpireEntryAt(_key, DateTime.Now.Date.AddDays(1).AddMinutes(-1));
        //    }
        //    return result;

        //    //var result = RedisUtil.Set<List<IpSend>>(_key, list, RedisCacheConfig.DBId);
        //    //if (result)
        //    //{
        //    //    //当天生效
        //    //    RedisUtil.SetExpire(_key, DateTime.Now.AddDays(1).AddSeconds(-1), RedisCacheConfig.DBId);
        //    //}
        //    //return result;
        //}

        /// <summary>
        /// 获取IP发送验证码次数
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        //public List<IpSend> GetCaptchaCount_IP(string type)
        //{
        //    var _key = $"{_captchaCountKey}:{type}:IP";
        //    //return RedisUtil.Get<List<IpSend>>(_key, RedisCacheConfig.DBId);
        //    return RedisEntities.Default.ItemGet<List<IpSend>>(_key);
        //}

        /// <summary>
        /// 获取验证码
        /// </summary>
        /// <param name="type"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        public CaptchaUse GetCaptcha(string type, string dataKey)
        {
            var _key = $"{_captchaKey}:{type}";
            //return RedisUtil.Hash_Get<CaptchaUse>(_key, dataKey, RedisCacheConfig.DBId);
            return RedisEntities.Default.HashGet<CaptchaUse>(_key, dataKey);
        }

        /// <summary>
        /// 获取敏感词
        /// </summary>
        /// <returns></returns>
        public List<string> GetSensitiveWord()
        {
            string key = "SensitiveWord";
            //return RedisUtil.List_GetList<string>(key, RedisCacheConfig.DBId);
            return RedisEntities.Default.ListGetAll<string>(key);
        }

        /// <summary>
        ///删除验证码
        /// </summary>
        /// <param name="type"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        //public bool RemoveCaptcha(string type, string dataKey)
        //{
        //    var _key = $"{_captchaKey}:{type}";
        //    //return RedisUtil.Hash_Remove(_key, dataKey, RedisCacheConfig.DBId);
        //    return RedisEntities.Default.HashRemoveField(_key, dataKey);
        //}
    }
}
