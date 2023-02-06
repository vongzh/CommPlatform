using System;
using System.Collections.Generic;
using System.Linq;
using Zk.HotelPlatform.Utils.Config;

namespace Zk.HotelPlatform.Utils.Global
{
    public class GlobalConfig
    {
        #region Port
        public static int Port
        {
            get
            {
                return ConfigUtil.GetValue<int>("Port", 8555);
            }
        }

        public static int GaoDePort
        {
            get
            {
                return ConfigUtil.GetValue<int>("Port", 8055);
            }
        }

        public static int OpenPort
        {
            get
            {
                return ConfigUtil.GetValue<int>("Port", 9555);
            }
        }

        public static int FeiZhuApiPort
        {
            get
            {
                return ConfigUtil.GetValue<int>("Port", 8655);
            }
        }

        public static int MeiTuanApiPort
        {
            get
            {
                return ConfigUtil.GetValue<int>("Port", 8756);
            }
        }

        public static int TongChengApiPort
        {
            get
            {
                return ConfigUtil.GetValue<int>("Port", 8855);
            }
        }

        public static int QunarApiPort
        {
            get
            {
                return ConfigUtil.GetValue<int>("Port", 8955);
            }
        }

        public static int CtripApiPort
        {
            get
            {
                return ConfigUtil.GetValue<int>("Port", 9055);
            }
        }
        #endregion

        #region Cache
        public static string CachingRedisConnection
        {
            get
            {
                // return ConfigUtil.GetValue("CachingRedisConnection", "192.168.1.252:6378,password=123456");
                return ConfigUtil.GetValue("CachingRedisConnection", "219.150.218.125:16379,password=zhongke**66");
            }
        }

        public static string HangfireRedisConnection
        {
            get
            {
                return ConfigUtil.GetValue("HangfireRedisConnection", "192.168.1.252:6378,password=123456");
            }
        }
        #endregion

        #region MQ
        public static string MQConnection
        {
            get
            {
                return ConfigUtil.GetValue("MQConnection", "host=192.168.1.252");
            }
        }
        #endregion

        #region EsService
        public static string EsService
        {
            get
            {
                return ConfigUtil.GetValue("EsService", "http://219.150.218.21:19200/");
            }
        }
        #endregion

        #region ApiAddress
        public static string ApiAddress
        {
            get
            {
                return ConfigUtil.GetValue("ApiAddress", "https://api.51zhu.cn");
                // return ConfigUtil.GetValue("ApiAddress", "http://219.150.218.21:18555");
                // return ConfigUtil.GetValue("ApiAddress", "http://localhost:8555");
            }
        }
        #endregion

        #region OAuth
        public static string ClientId
        {
            get
            {
                return ConfigUtil.GetValue("ClientId");
            }
        }

        public static string ClientSecret
        {
            get
            {
                return ConfigUtil.GetValue("ClientSecret");
            }
        }

        public static Dictionary<string, string> ClientDictionary
        {
            get
            {
                try
                {
                    string config = ConfigUtil.GetValue("Client");
                    if (string.IsNullOrEmpty(config))
                        return new Dictionary<string, string>();

                    var param = config.Split(';');
                    var data = new Dictionary<string, string>();
                    param.ToList().ForEach(x =>
                    {
                        var val = x.Split(',');

                        if (val.Length == 2)
                            data.Add(val[0], val[1]);
                    });
                    return data;
                }
                catch
                {
                    return new Dictionary<string, string>();
                }
            }
        }

        public static Dictionary<string, decimal> DynamicRadioDictionary
        {
            get
            {
                try
                {
                    string config = ConfigUtil.GetValue("DynamicRadio");
                    if (string.IsNullOrEmpty(config))
                        return new Dictionary<string, decimal>();

                    var param = config.Split(';');
                    var data = new Dictionary<string, decimal>();
                    param.ToList().ForEach(x =>
                    {
                        var val = x.Split(',');

                        if (val.Length == 2)
                            data.Add(val[0], decimal.Parse(val[1]));
                    });
                    return data;
                }
                catch
                {
                    return new Dictionary<string, decimal>();
                }
            }
        }
        #endregion

        #region AMap
        public static string AMapAPI
        {
            get
            {
                return ConfigUtil.GetValue("AMapApi", "http://restapi.amap.com/v3");
            }
        }

        public static string AMapKey
        {
            get
            {
                return ConfigUtil.GetValue("AMapKey", "3538693b602c9adb032dd6b64c8fd522");
            }
        }
        #endregion

        #region Pay
        public static string TenPayCallBackUrl
        {
            get
            {
#if DEBUG
                return ConfigUtil.GetValue("TenPay_Notify", "http://115.159.74.70:8555/Trade/TenPayCallback");
#else
                return ConfigUtil.GetValue("TenPay_Notify", "https://115.159.74.70:8555/Trade/TenPayCallback");
#endif
            }
        }

        public static string AliPayCallbackUrl
        {
            get
            {
#if DEBUG
                return ConfigUtil.GetValue("AliPay_Notify", "http://115.159.74.70:8555/Trade/AliPayCallback");
#else
                return ConfigUtil.GetValue("AliPay_Notify", "https://api.51zhu.cn/Trade/AliPayCallback");
#endif
            }
        }

        public static string PaymentRedirectUrl
        {
            get
            {
#if DEBUG
                return ConfigUtil.GetValue("PaymentRedirectUrl", "http://115.159.74.70:9527/order");
#else
                return ConfigUtil.GetValue("PaymentRedirectUrl", "https://www.51zhu.cn/order");
#endif
            }
        }
        #endregion

        #region AliPay
        public static string AliPay_AppId
        {
            get
            {
#if DEBUG
                return ConfigUtil.GetValue("AliPay_AppId", "2016101700706856");
#else
                return ConfigUtil.GetValue("AliPay_AppId", "2021001163608286");
#endif
            }
        }

        public static string AliPay_Gateway
        {
            get
            {
#if DEBUG
                return ConfigUtil.GetValue("AliPay_Gateway", "https://openapi.alipaydev.com/gateway.do");
#else
                return ConfigUtil.GetValue("AliPay_Gateway", "https://openapi.alipay.com/gateway.do");
#endif
            }
        }

        public static string AliPay_Privatekey
        {
            get
            {
                return ConfigUtil.GetValue("AliPay_Privatekey", "MIIEowIBAAKCAQEAjMqeYy0vINSbS9KYkKGJYe6Qd5IqHIOYTXb3hhDm42joI2lPphuBP1xG/YSEoE3UTiXjdU96Pk7MS0L9VfzGG0sxeb0AtIXRf48zNuWw10bTbOQB211Dcdi5FB2z5Rcl0iuvBSgNp9whA22twiyzlOHBj8A9Gv+tzHPl3hMW8p/0FP0ji40eVDIBgP0eklx/hOxkvNr39RC+DKxzjyUDprKvQ5E/j9PqNdt00+EltflBt/F8B7c4OYO7o4L9y7EuGTKtBBfwwfMaPRkO0yophwI0rEybHCy3FohajPvEjEgpPrc/JtErkBGmAajRUJZwPn4J0hf3TmEBRiSur7cFCwIDAQABAoIBAHFlAAyA5+RZWHrJM3fJowztSA4F+0Tb+F7r4pBoLI4cgFuFBlGS9ZZCjCrEAM3b4ZLPkVN6n8pvTLuRSNlorWZuOXsDbv32ibVjcKxWcwfwU3jwa05y309ibi51fd6VtjCAXoaYq+b2tOA+BiTOPBU9fth4zL5iD2kML6edWG4wkBJEbTfWgmb1ZXo+/PzwLmgkTpjtQaNm7pMmiggifzrysL5h1Vb+4cvfvuLcYpgLucujDsJ+I9Eajf5D5g1vpbtBcLlpQqwPYcVomfeQ5VCrkMmnrbYYYBZ3Pte9N7MYUKgJ3tbSlMciUsyRe3drXunYKj/iV/UCqEnIPW75qAECgYEA4JHq+O6gkq/dJQq9L9qOu0IQvgvVY2llFMX/VBCcBQ+sTrUunkefD5dIsW9ffbBIBBZgMJK229Gck3n0hWzwTi43rrIPVbTv9izxywQMF+D9a/NN6SKnIkdRQw2SV/GKDRUy5DzUTzpSIZ556w1ikO2w16uW8dPrBEn2Ntz22ssCgYEAoH8DlI3aBvWeIdZforDBh04sodDvackCkBXDkBjlnL/pEMdogm6s23+ajHCOqqOdcaoeZbx8QaNGOGpXveSBxWdWWOddIVlwBe5sy/618iK3aRhSxu/ylAn/+HaBywqQJy33eqYU3DXSA+NvHNlm0weXUnhjQ0oQqBvW5q139sECgYB1tEWCQCCtgbvZZG2QnW9eOtxnn/AycNVoM/cF6sa76GEZx+EtWMDH/TU+uu9EA5dbfRbhnqR1RsJYkf1VyamScKsCx5U+CN4VG62ICREFmwl5AUd+wIj1cIfpKz3fFlyTSt8nvbJR7HGL6XJhftyAtfSQtgEhsz9lOhbw3pQMTQKBgEAoRfAefGeHy/zOy1AnxFUoROMyuGHbPrEGYjS6Hx93/9z7OaXaRDNCYS9+1ykTWp7TWG9m120NrcZjjEuWiuG0m2nSowHv1L4qP0eUvHrcseDLHlv0E05maPKTvPLDeDiRQXq/5VraEOgJBKPNrDLzt+P5yARgGSR6bH+uHGSBAoGBAKH9eAow3Qj8MCYgGWxISUk0Hw1qsVgqCignBg+Qvic06TxnGB/YNqpKK6zqzdY3rt8+dh/ZyZT77rLp9LQWzI3hB6ANkIcoammFt1bMPx3a01IlcSSagfAqytyR7XMtMrfIYoREufXkUMifQmYjhw2dTFqB/zzluKep88Jge3cu");
            }
        }

        public static string Alipay_PublicKey
        {
            get
            {
                return ConfigUtil.GetValue("Alipay_PublicKey", "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAjMqeYy0vINSbS9KYkKGJYe6Qd5IqHIOYTXb3hhDm42joI2lPphuBP1xG/YSEoE3UTiXjdU96Pk7MS0L9VfzGG0sxeb0AtIXRf48zNuWw10bTbOQB211Dcdi5FB2z5Rcl0iuvBSgNp9whA22twiyzlOHBj8A9Gv+tzHPl3hMW8p/0FP0ji40eVDIBgP0eklx/hOxkvNr39RC+DKxzjyUDprKvQ5E/j9PqNdt00+EltflBt/F8B7c4OYO7o4L9y7EuGTKtBBfwwfMaPRkO0yophwI0rEybHCy3FohajPvEjEgpPrc/JtErkBGmAajRUJZwPn4J0hf3TmEBRiSur7cFCwIDAQAB");
            }
        }

        public static string Alipay_AlipayPublicKey
        {
            get
            {
#if DEBUG
                return ConfigUtil.GetValue("Alipay_AlipayPublicKey", "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAh7+BfAKuAa+y5Fp88i7ZiPe+LFH3L7ozRqgt8WXcN1OWPkFY6PvhM2ve/7ZPBquWoHCgm7NZbwLGFW/k0L+gPhGdUc7FmSXjGpTpZ19Z9p7y9mgNeaC5i+0X+4HHaEQOItgnVF9LIA09e7namdai90AbOgX73iuDKYL5/3zqpf8gPHCXijJbe774hwOe6wP8EfLFBf9z2+RTfGsFxvlZNvKBg8sEhN7Q2g0haLZFYiM/m7A3iP2XQjhysMvI2dnE6jd8SeXHxtKpyGIjwQO0FeoBAe6fn1Imw6Ni1yQ70YRA7oNkSDp556v3pbgRgSh41uo7f7H5ix1mWE+jof3NKQIDAQAB");
#else
                return ConfigUtil.GetValue("Alipay_AlipayPublicKey", "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEA4S+r3J7tFHer1hSMSw+Ua7qtZDni4Za+NwoHNpIkJeEscI2HDqNZNojYjaca3kDIDjnd/6wUysV/6jBAZohDhtQpMDlaYcn0pAm0nKnI6ouw2ejbSj2as23WW9VrmYRfyuyoF0paFKgsHga8LSUHOrF2z8J7yTtvi6CWTLuFCSZpsIzvjzlyppYdwONaNjYowDRf6fzKVN0WUbKvhT0/1QGn5kbgIzPM/i58J2ZFgLY1ASEd5hqyrfsXRfiXNX2IBr07OvehwyDeV93Q5SeKtTHmzAkMPl5pgGFOHVGunaown+miNo8K3A79UHgkotkWlctMYhPnxb1Asa/vbSF4zwIDAQAB");
#endif
                //MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEA4S+r3J7tFHer1hSMSw+Ua7qtZDni4Za+NwoHNpIkJeEscI2HDqNZNojYjaca3kDIDjnd/6wUysV/6jBAZohDhtQpMDlaYcn0pAm0nKnI6ouw2ejbSj2as23WW9VrmYRfyuyoF0paFKgsHga8LSUHOrF2z8J7yTtvi6CWTLuFCSZpsIzvjzlyppYdwONaNjYowDRf6fzKVN0WUbKvhT0/1QGn5kbgIzPM/i58J2ZFgLY1ASEd5hqyrfsXRfiXNX2IBr07OvehwyDeV93Q5SeKtTHmzAkMPl5pgGFOHVGunaown+miNo8K3A79UHgkotkWlctMYhPnxb1Asa/vbSF4zwIDAQAB
            }
        }
        #endregion

        #region TenPay
        public static string TenPay_Cert
        {
            get
            {
                return ConfigUtil.GetValue("TenPay_Cert", $"{Environment.CurrentDirectory}/TenPayCert/apiclient_cert.p12");
            }
        }
        #endregion

        #region TencentCloud
        public static string TencentCloud_SecretId
        {
            get
            {
                return ConfigUtil.GetValue("TencentCloud_SecretId", "AKIDHiEfyAfyb2OZYQDe3DOO8g2CKGE5nKfo");
            }
        }

        public static string TencentCloud_SecretKey
        {
            get
            {
                return ConfigUtil.GetValue("TencentCloud_SecretKey", "60FUYUFoNp6GF2c0HPRHVclsmXHi3C9x");
            }
        }

        public static ulong TencentCloud_Captcha_AppId
        {
            get
            {
                return ulong.Parse(ConfigUtil.GetValue("TencentCloud_Captcha_AppId", "2085651946"));
            }
        }

        public static string TencentCloud_Captcha_AppSecret
        {
            get
            {
                return ConfigUtil.GetValue("TencentCloud_Captcha_AppSecret", "0-g2zF_cX2qfqvyPsCg3_Vg**");
            }
        }

        public static string TencentCloud_Region
        {
            get
            {
                return ConfigUtil.GetValue("TencentCloud_Region", "ap-shanghai");
            }
        }
        #endregion

        #region QCloudSMS
        public static string QCloudSMS_AppId
        {
            get
            {
                return ConfigUtil.GetValue("QCloudSMS_AppId", "1400290036");
            }
        }

        public static string QCloudSMS_AppKey
        {
            get
            {
                return ConfigUtil.GetValue("QCloudSMS_AppKey", "4f2a2f52bdb76c7ed1c7ad5621f12664");
            }
        }
        #endregion

        #region HUAWEISMS
        public static string HuaWeiSMS_API
        {
            get
            {
                return ConfigUtil.GetValue("HuaWeiSMS_API", "https://rtcsms.cn-north-1.myhuaweicloud.com:10743/sms/batchSendSms/v1");
            }
        }

        public static string HuaWeiSMS_ReportAPI
        {
            get
            {
                return ConfigUtil.GetValue("HuaWeiSMS_ReportAPI", "localhost:8555");
            }
        }

        public static string HuaWeiSMS_AppKey
        {
            get
            {
                return ConfigUtil.GetValue("HuaWeiSMS_AppKey", "c8RWg3ggEcyd4D3p94bf3Y7x1Ile");
            }
        }

        public static string HuaWeiSMS_AppSecret
        {
            get
            {
                return ConfigUtil.GetValue("HuaWeiSMS_AppSecret", "q4Ii87BhST9vcs8wvrzN80SfD7Al");
            }
        }

        public static string HuaWeiSMS_Channel
        {
            get
            {
                return ConfigUtil.GetValue("HuaWeiSMS_Channel", "csms12345678");
            }
        }
        #endregion

        #region SMS
        public static string SMS_Signtrue
        {
            get
            {
                return ConfigUtil.GetValue("SMS_Signtrue", "ZKY");
            }
        }
        #endregion

        #region ContactPhone
        public static string PurchaseContactPhone
        {
            get
            {
                //03734066689
                return ConfigUtil.GetValue("ContactPhone", "17737360039");
            }
        }
        #endregion
        #region Ctrip采购Code

        public static string CtripOrderBookCheckCode
        {
            get
            {
                //03734066689
                return ConfigUtil.GetValue("OrderBookCheckCode", "419f7a688e3c45f481d295708b870323");
            }
        }

        public static string CtripOrderSaveCode
        {
            get
            {
                //03734066689
                return ConfigUtil.GetValue("OrderSaveCode", "befc2901fa164bdcb2e0ceec0bcc150e");
            }
        }

        public static string CtripOrderSubmitCode
        {
            get
            {
                //03734066689
                return ConfigUtil.GetValue("OrderSubmitCode", "f40c5c71f84745cbbc73238252d9d43c");
            }
        }


        #endregion

        #region YOP
        public static string Yop_AppKey
        {
            get
            {
                return ConfigUtil.GetValue("Yop_AppKey", "app_10017613352");
            }
        }

        public static string Yop_PrivateKey
        {
            get
            {
                return ConfigUtil.GetValue("Yop_PrivateKey", @"MIIEowIBAAKCAQEAker04qMulLhVvAdocvDJeHUrxnLYX5IyKe5bQoRqaNOfh6x3
                                T3uE31ue3lD4X0U7jq2QoOO9JUl1bcXURp0jp0HKAY +/ Oqxq13HYtim / t2dyHxRJ
                                UsetzW0nI6d5yqsjGwY9qbNXPNHrU9hAooXAJeAFpJ7FL3EZ + NCEukmfqfx3oUx +
                                upObri494F4rasHiwnMDSUDa6BryGH6DHq + d1ZLDIgNFjlkknlgZ5074UNH6r7Ul
                                A4ZxuJgrctx2dmgzduZHKGvpTiTuuUGJIfsONpRFbHFY37nk6yQorpTgj4KWTiEd
                                qt5 + 2XOukqygSbCxSNm2Qpvq5sMbqUr8maI2YwIDAQABAoIBADGapQsSnDg0ffjA
                                r / I7BtJrSqlYy1OZ + HGmtOFk8GOYZWgS6jBb9iyUEerTrct9VqTHPh5jrR4q3b + m
                                e + ZRbxhh1hE307NXf66j5h7vDFyFzxbfp5l7hgsKqz6EpXOPQAzkzzuMWwCzAAPt
                                xPM68rotcN2ol7HsCf86q5REiQGY7WwNB7QaFxLUeUtIY65thVNDwRl3KGfKmwh4
                                Vz8IsfaUDaU6irWifEy91S25cIimTsWPVcGjnHfCFblf8JOrFUJx7f3JAm + KAvQQ
                                auW1W9JPsuRDjSSY + VznH3obfzJA3h8 / sbtS3p + qUtZ22gG9vbeQArZ3aP / j9Y8O
                                nhRZ55kCgYEAyiLV5aIGfJ52EtdQCjk85ZdjTf5llOrhAMS1HTyru8SGMVZ2fVd1
                                acrIsFQKxJXljejm1NyOpTYgLGhZzMyEjABSezmBxh7BmWxjZG54ZOBZ63H6CJ9x
                                3w7T2rqZzitcGcgwLoOZe52USZXdr6EtaamU + JKNpDQxFswKilKqJs0CgYEAuM0Q
                                yENyvJ87Mch + L9K4KNur7kWgFiprPVNVk1jXLzwO8gyJ8dHrR6cxfSMt + eiCPCw1
                                qAKMxhf3lTIKxRuX1AeBsZ960zcmisPt9HZJCpMNrGyWBfZfR8iou2v3YT4sS97Y
                                qorl5Mn9T3HsrxX5vjasHn2q + uy / e3hxO + 1F8e8CgYEAlUrtWFE3IUuYQYotX5 + u
                                zpPlkHFYbYic7ohajGGHJYOc6UvG8ARsf9p6J2GrIk7j4NAnzQepg3RahvH7gTt8
                                mjqSsNhkdQO6UaqwLjk5Sqg5QaBI352D6Q92gZJYjuGPgKtOisw / zAEyyLabqWAN
                                voC1unHjk2fI6eGB12 / +o / ECgYAOFyyxaFsmQKndyOC + Or3p953rZAmpxwA4Q9Nh
                                kcOOiXDGSeh2tL / QzdG7LeSCipTri59nuRIMxKUPDiq + DrWL6CqeZX + S / UCGEhxh
                                qGc3VDEVtY3W1Is9SPJ8n + XT106 / VjWFA2Q2BqS22wPl8bICK98B0CkqSsTq4Csa
                                G6RapwKBgFwLEN7IaKTWO6JCUQ8vgjMz8cPixm7BKINaXhk8NisQPOEgYRRXlaV7
                                xFgF5OMOExEq0Z0tRjSsQZe8VyJR1hDkuQV6u7sgxIsCpacC7qtHlZBWQBBpNGQ4
                                9Nky2FcftAm3LBQj8Ay39FbV1rPzb6GQK6wklZI1IUk1ctDdEiwj");
            }
        }

        public static string Yop_PublicKey
        {
            get
            {
                return ConfigUtil.GetValue("Yop_PublicKey", "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEA6p0XWjscY+gsyqKRhw9MeLsEmhFdBRhT2emOck/F1Omw38ZWhJxh9kDfs5HzFJMrVozgU+SJFDONxs8UB0wMILKRmqfLcfClG9MyCNuJkkfm0HFQv1hRGdOvZPXj3Bckuwa7FrEXBRYUhK7vJ40afumspthmse6bs6mZxNn/mALZ2X07uznOrrc2rk41Y2HftduxZw6T4EmtWuN2x4CZ8gwSyPAW5ZzZJLQ6tZDojBK4GZTAGhnn3bg5bBsBlw2+FLkCQBuDsJVsFPiGh/b6K/+zGTvWyUcu+LUj2MejYQELDO3i2vQXVDk7lVi2/TcUYefvIcssnzsfCfjaorxsuwIDAQAB");
            }
        }
        #endregion

        #region RedisDbId
        public static int Redis_DB_Id
        {
            get
            {
                return ConfigUtil.GetValue<int>("Redis_DB_Id", 5);
            }
        }
        #endregion

        #region Ftp
        public static string FtpDomain
        {
            get
            {
                return ConfigUtil.GetValue("FtpDomain", "ftp.51zhu.cn");
            }
        }

        public static int FtpPort
        {
            get
            {
                return ConfigUtil.GetValue<int>("FtpPort", 21);
            }
        }

        public static string FtpUser
        {
            get
            {
                return ConfigUtil.GetValue("FtpUser", "ftpwo");
            }
        }

        public static string FtpPwd
        {
            get
            {
                return ConfigUtil.GetValue("FtpPwd", "zhongke**66..88");
            }
        }

        public static int UploadCount
        {
            get
            {
                return ConfigUtil.GetValue<int>("UploadCount", 10);
            }
        }

        public static string ImagesDomain
        {
            get
            {
                return ConfigUtil.GetValue("ImagesDomain", "http://images.51zhu.cn");
            }
        }

        public static string Images01Domain
        {
            get
            {
                return ConfigUtil.GetValue("Images01Domain", "http://images01.51zhu.cn");
            }
        }
        #endregion

        #region QiYu
        public static string QiYuAppKey
        {
            get
            {
                return ConfigUtil.GetValue<string>("QiYuAppKey", "60dfb82f53cb950482ce44c79331b54a");
            }
        }

        public static string QiYuAppSecret
        {
            get
            {
                return ConfigUtil.GetValue<string>("QiYuAppSecret", "34E62DEFD5904087A6E1F604F5811412");
            }
        }

        public static string QiYuApi
        {
            get
            {
                return ConfigUtil.GetValue<string>("QiYuApi", "http://qiyukf.com/openapi/ipcc");
            }
        }
        #endregion

        #region 数据推送
        public static string TestPushApi
        {
            get
            {
                return ConfigUtil.GetValue("TestPushApi", "http://backend.zkv8.com");
            }
        }
        #endregion

        #region 小猪
        public static string XiaoZhu_AppId
        {
            get
            {
                return ConfigUtil.GetValue("XiaoZhu_AppId", "CajigKVqfte2VwSMm6SQxYfZs2qX9cnr0Awzf1d4");
            }
        }

        public static string XiaoZhu_AppKey
        {
            get
            {
                return ConfigUtil.GetValue("XiaoZhu_AppKey", "3XUmdeCDrAXgHuC2McE6xUxgVGgTI1eqy1AWv60K");
            }
        }

        public static string XiaoZhu_Channel
        {
            get
            {
                return ConfigUtil.GetValue("XiaoZhu_Channel", "zhongke");
            }
        }

        public static string XiaoZhu_Api
        {
            get
            {
                return ConfigUtil.GetValue("XiaoZhu_Api", "https://test-open.xiaozhu.com");
            }
        }
        #endregion

        #region 飞猪

        //public static string FeiZhu_AppKey
        //{
        //    get
        //    {
        //        return ConfigUtil.GetValue("FeiZhu_AppKey", "25329438");
        //    }
        //}

        //public static string FeiZhu_AppSecret
        //{
        //    get
        //    {
        //        return ConfigUtil.GetValue("FeiZhu_AppSecret", "5bdddb67da0e8534e31202ab7064155d");
        //    }
        //}
        //public static string FeiZhu_SessionKey
        //{
        //    get
        //    {
        //        return ConfigUtil.GetValue("FeiZhu_SessionKey", "61007272b1c8aadd96ccdd6412635cd8ed913eb514fd50a4037766515");
        //    }
        //}

        //public static string FeiZhu_Url
        //{
        //    get
        //    {
        //        return ConfigUtil.GetValue("FeiZhu_Url", "http://gw.api.taobao.com/router/rest");
        //    }
        //}

        //public static string FeiZhu_RedirectUrl
        //{
        //    get
        //    {
        //        return ConfigUtil.GetValue("FeiZhu_RedirectUrl", "http://test.zkv8.com/meituan/Order/Validate");
        //    }
        //}

        /// <summary>
        /// 飞猪数据更新推送时效
        /// </summary>
        public static int FeiZhu_DelayTime
        {
            get
            {
                return ConfigUtil.GetValue("FeiZhu_DelayTime", -10);
            }
        }

        #endregion
        public static int CacheTime
        {
            get
            {
                return ConfigUtil.GetValue("CacheTime", 10);
            }
        }

        public static int IsCache
        {
            get
            {
                return ConfigUtil.GetValue("IsCache", 1);
            }
        }

        public static int SyncRatePlanThreadNum
        {
            get
            {
                return ConfigUtil.GetValue("SyncRatePlanThreadNum", 5);
            }
        }

        public static decimal DefaultDynamicRadio
        {
            get
            {
                return ConfigUtil.GetValue<decimal>("DefaultDynamicRadio", 1.00M);
            }
        }

        /// <summary>
        /// 默认数据渠道
        /// </summary>
        public static List<int> DefaultDataChannels
        {
            get
            {
                var channels = new List<int>();
                string config = ConfigUtil.GetValue("DefaultDataChannels");
                if (!string.IsNullOrEmpty(config))
                {
                    if (config.Contains(','))
                    {

                        var param = config.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (var item in param)
                        {
                            int.TryParse(item, out int channel);
                            if (channel > 0)
                            {
                                channels.Add(channel);
                            }
                        }
                    }
                    else
                    {
                        int.TryParse(config, out int channel);
                        if (channel > 0)
                        {
                            channels = new List<int>() { channel };
                        }
                    }
                }
                if (channels == null || !channels.Any())
                {
                    channels = new List<int>() { (int)GlobalEnum.Platform.CTRIP_FENXIAO };
                }

                return channels;
            }
        }

        /// <summary>
        /// 默认数据渠道
        /// </summary>
        public static Dictionary<int, int[]> MasterPlatformMapping
        {
            get
            {
                return new Dictionary<int, int[]>
                {
                    { 9, new []{ 9, 7, 19}},
                    { 4, new []{ 4, 70 }},
                    { 17, new []{ 17 }},
                    { 110, new []{ 110 }},
                };
            }
        }
        #region ElongFen
        public static string ElongFen_User
        {
            get
            {
                return ConfigUtil.GetValue("ElongFen_User", "6b11cc063d124930a05bbc4c14d72ae2");
            }
        }

        public static string ElongFen_AppKey
        {
            get
            {
                return ConfigUtil.GetValue("ElongFen_AppKey", "5b33db6a214224ad7e85297a2412b1cf");
            }
        }
        public static string ElongFen_SecretKey
        {
            get
            {
                return ConfigUtil.GetValue("ElongFen_SecretKey", "52e3f04a12c339800792f33ad05a1aae");
            }
        }
        public static string ElongFen_Api
        {
            get
            {
                return ConfigUtil.GetValue("ElongFen_Api", "http://api.elong.com/rest");
            }
        }
        public static string ElongFenTest_Api
        {
            get
            {
                //http://api-test.elong.com/rest
                return ConfigUtil.GetValue("ElongFenTest_Api", "http://api.elong.com/rest");
            }
        }
        #endregion
        #region 大蚂蚁代理
        public static string MayiProxy_AppKey
        {
            get
            {
                return ConfigUtil.GetValue("MayiProxy_AppKey", "28334608");
            }
        }

        public static string MayiProxy_AppSecret
        {
            get
            {
                return ConfigUtil.GetValue("MayiProxy_AppSecret", "e093a494fc3252c3081a70fa587d43f9");
            }
        }

        public static string MayiProxy_Host
        {
            get
            {
                return ConfigUtil.GetValue("MayiProxy_Host", "s4.proxy.mayidaili.com:8123");
            }
        }
        #endregion

        #region 去哪-我要住,去哪-众游旅行

        public static string Qunar_Hangfire_queue
        {
            get
            {
                return ConfigUtil.GetValue("Qunar_Hangfire_queue", "qunar");
            }
        }
        public static int Qunar_OrderQueryTime
        {
            get
            {
                return ConfigUtil.GetValue<int>("Qunar_OrderQueryTime", 1);
            }
        }
        public static int Qunar_PlatformId
        {
            get
            {
                return ConfigUtil.GetValue<int>("Qunar_PlatformId", 30);
            }
        }

        //去哪儿-我要住
        public static string Qunar_Api
        {
            get
            {
                return ConfigUtil.GetValue("Qunar_Api", "http://c3id.trade.qunar.com/api/ota");
            }
        }
        public static string Qunar_Api1
        {
            get
            {
                return ConfigUtil.GetValue("Qunar_Api1", "http://c3id.trade.qunar.com/ota/changeprice/norm/param/push");
            }
        }
        public static string Qunar_SignKey
        {
            get
            {
                return ConfigUtil.GetValue("Qunar_SignKey", "YEZm2SMFKHqKx8Fv98oJU5RiYWXdNO1N");
            }
        }
        //去哪儿-众游旅行
        public static string Qunar_ZhongYou_Api
        {
            get
            {
                return ConfigUtil.GetValue("Qunar_ZhongYou_Api", "http://c3tj.trade.qunar.com/api/ota");
            }
        }
        public static string Qunar_ZhongYou_Api1
        {
            get
            {
                return ConfigUtil.GetValue("Qunar_ZhongYou_Api1", "http://c3tj.trade.qunar.com/ota/changeprice/norm/param/push");
            }
        }
        public static string Qunar_ZhongYou_SignKey
        {
            get
            {
                return ConfigUtil.GetValue("Qunar_ZhongYou_SignKey", "YEZm2SMFKHqKx8Fv98oJU5RiYWXdNO1N");
            }
        }
        #endregion

        #region 京东
        public static string JD_Api
        {
            get
            {
                return ConfigUtil.GetValue("JD_Api", "https://hotel-openapi.jd.com");
            }
        }

        public static string JD_AppId
        {
            get
            {
                return ConfigUtil.GetValue("JD_AppId", "JD0202931246");
            }
        }

        public static string JD_SecretKey
        {
            get
            {
                return ConfigUtil.GetValue("JD_SecretKey", "2244C8A3A6956F95B0E080B50D4291FB");
            }
        }

        public static decimal JD_DynamicRadio
        {
            get
            {
                return ConfigUtil.GetValue<decimal>("JD_DynamicRadio", 1.00M);
            }
        }
        #endregion

        #region 捷旅
        public static int JieLv_DelayTime
        {
            get
            {
                return ConfigUtil.GetValue<int>("JieLv_DelayTime", 10);
            }
        }
        public static int JieLv_SellDaySetup
        {
            get
            {
                return ConfigUtil.GetValue<int>("JieLv_SellDaySetup", 3);
            }
        }
        public static int JieLv_ThreadsNum
        {
            get
            {
                return ConfigUtil.GetValue<int>("JieLv_ThreadsNum", 30);
            }
        }


        #endregion

        #region MeiTuan

        public static string DefaultPushDataRuleName
        {
            get
            {
                return ConfigUtil.GetValue("DefaultPushDataRuleName", "Z21");
            }
        }

        public static decimal DefaultProfitRatio
        {
            get
            {
                return ConfigUtil.GetValue("DefaultProfitRatio", 0.02M);
            }
        }

        public static decimal DefaultAddAmount
        {
            get
            {
                return ConfigUtil.GetValue("DefaultAddAmount", 0.00M);
            }
        }

        public static int CurrentUserId
        {//采购用的userid
            get
            {
                return ConfigUtil.GetValue("CurrentUserId", 10000274);
            }
        }
        public static int CurrentSourcePlatformID
        {//数据来源平台id  携程分销 艺龙分销
            get
            {
                return ConfigUtil.GetValue("CurrentSourcePlatformID", 4);
            }
        }
        public static int CurrentActivityID
        {// 
            get
            {
                return ConfigUtil.GetValue("CurrentActivityID", 1723171497);
            }
        }
        public static int CurrentPlatformID
        {//推送平台id 美团高星 美团低星美团优优
            get
            {
                return ConfigUtil.GetValue("CurrentPlatformID", 2);
            }
        }
        public static int PlatformID
        {
            get
            {
                return ConfigUtil.GetValue("PlatformID", 2);
            }
        }

        public static string MeiTuan_PartnerId
        {
            get
            {
                return ConfigUtil.GetValue("MeiTuan_PartnerId", "4135161");
            }
        }

        public static string MeiTuan_EncryptKey
        {
            get
            {
                return ConfigUtil.GetValue("MeiTuan_EncryptKey", "b45925c4d4bd4230be85e88f1308059c");
            }
        }
        public static string OnLineUrl
        {
            get
            {
                return ConfigUtil.GetValue("OnLineUrl", "https://openplatform-hotel.meituan.com/v1");
            }
        }
        public static int MeiTuan_StockSetup
        {
            get
            {//库存-1
                return ConfigUtil.GetValue("MeiTuan_StockSetup", 1);
            }
        }
        public static int MeiTuan_SellDaySetup
        {
            get
            {//售卖天数
                return ConfigUtil.GetValue("MeiTuan_SellDaySetup", 1);
            }
        }
        public static int MeiTuan_SellStarDay
        {
            get
            {//售卖开始日期
                return ConfigUtil.GetValue("MeiTuan_SellStarDay", 0);
            }
        }

        public static int MeiTuan_ThreadsNum
        {
            get
            {// 线程数
                return ConfigUtil.GetValue("MeiTuan_ThreadsNum", 1);
            }
        }
        public static bool CtripFen_IsInstantConfirm
        {
            get
            {//是否是立即确认
                return ConfigUtil.GetValue("IsInstantConfirm", false);
            }
        }
        public static int MeiTuan_DelayTime
        {
            get
            {
                return ConfigUtil.GetValue<int>("MeiTuan_DelayTime", 10);
            }
        }

        //public static int MeiTuan_ThreadsNumRoomStockPrice
        //{
        //    get
        //    {//价格库存线程数
        //        return ConfigUtil.GetValue("MeiTuan_ThreadsNumRoomStockPrice", 10);
        //    }
        //}
        
  public static int MeiTuan_PageSize
        {
            get
            {// 线程数
                return ConfigUtil.GetValue("MeiTuan_PageSize", 5000);
            }
        }
        public static int MeiTuan_PageCount
        {
            get
            {// 线程数
                return ConfigUtil.GetValue("MeiTuan_pageCount", 1);
            }
        }

        public static int MeiTuan_BeginHour
        {
            get
            {// 线程数
                return ConfigUtil.GetValue("MeiTuan_BeginHour", 6);
            }
        }
        #endregion

        #region FeiZhuFen
        public static string FeiZhuFen_DistributorTid
        {
            get
            {
                return ConfigUtil.GetValue("FeiZhuFen_DistributorTid", "2213092661782");
            }
        }

        public static string FeiZhuFen_AppKey
        {
            get
            {
                return ConfigUtil.GetValue("FeiZhuFen_AppKey", "33441085");
            }
        }
        public static string FeiZhuFen_AppSecret
        {
            get
            {
                return ConfigUtil.GetValue("FeiZhuFen_AppSecret", "8cc372d46399a1e5287e1d23dcc71e8d");
            }
        }
        public static string FeiZhuFen_SessionKey
        {
            get
            {
                return ConfigUtil.GetValue("FeiZhuFen_SessionKey", "610200905357d79cd1ff85f017efe61ef94e326292b2c3a2213092661782");
            }
        }
        public static string FeiZhuFen_Api
        {
            get
            {
                return ConfigUtil.GetValue("FeiZhuFen_Api", "http://gw.api.taobao.com/router/rest");
            }
        }
        public static string FeiZhuFenTest_Api
        {
            get
            {
                return ConfigUtil.GetValue("FeiZhuFenTest_Api", "http://pre-gw.api.taobao.com/top/router/rest");
            }
        }
        public static string FeiZhuFen_WSApi
        {
            get
            {
                return ConfigUtil.GetValue("FeiZhuFen_WSApi", "ws://mc.api.taobao.com");
            }
        }
        #endregion

        #region JinJiangFen
        public static string JinJiangFen_Api
        {
            get
            {
                return ConfigUtil.GetValue("JinJiangFen_Api", "https://bizfzout.bestwehotel.com/proxy/ms-corp-directly-connect");
            }
        }

        public static string JinJiangFen_AppId
        {
            get
            {
                return ConfigUtil.GetValue("JinJiangFen_AppId", "b9e36001-d4ed-4640-a87f-9df97583608b");
            }
        }

        public static string JinJiangFen_AppKey
        {
            get
            {
                return ConfigUtil.GetValue("JinJiangFen_AppKey", "0a3e8c8d-2670-4c4a-93af-5df8608dcde0");
            }
        }
        #endregion


        #region  智行(携程直连)
        /// <summary>
        /// 锦江数据
        /// </summary>
        public static decimal CtripDirection_JinJiangDataRisePrice
        {
            get
            {
                return ConfigUtil.GetValue("CtripDirection_JinJiangDataRisePrice", 1.0m);
            }
        }

        /// <summary>
        /// 优享会数据
        /// </summary>
        public static decimal CtripDirection_YouXiangHuiDataRisePrice
        {
            get
            {
                return ConfigUtil.GetValue("CtripDirection_YouXiangHuiDataRisePrice", 1.0m);
            }
        }
        #endregion

        #region 高德
        public static int GaoDe_IsElongRealRatePlan
        {
            get
            {
                return ConfigUtil.GetValue<int>("GaoDe_IsElongRealRatePlan", 1);
            }
        }
        #endregion 

        #region 百度

      

        public static string BaiDu_tp_name
        {
            get
            {
                return ConfigUtil.GetValue("BaiDu_tp_name", "zk");
            }
        }

        public static string BaiDu_primary_key
        {
            get
            {
                return ConfigUtil.GetValue("BaiDu_primary_key", "a00370b6655513b62a63aa71b381caaa");
            }
        }
        public static string BaiDu_salt
        {
            get
            {
                return ConfigUtil.GetValue("BaiDu_salt", "5h9m8v");
            }
        }
        #endregion
    }
}
