﻿using Senparc.Weixin.Entities;
using SKIT.FlurlHttpClient.Wechat.TenpayV3;
using SKIT.FlurlHttpClient.Wechat.TenpayV3.Events;
using SKIT.FlurlHttpClient.Wechat.TenpayV3.Models;
using SKIT.FlurlHttpClient.Wechat.TenpayV3.Settings;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Web.Configuration;
using Zk.HotelPlatform.Utils;
using static SKIT.FlurlHttpClient.Wechat.TenpayV3.Models.CreateApplyForSubMerchantApplymentRequest.Types.Business.Types.SaleScene.Types;

namespace Zk.HotelPlatform.Service.Impl
{
    public class TencentPayService
    {
        private readonly TenpayOptions tenpayOptions;

        private readonly WechatTenpayClient client;

        private readonly SenparcWeixinSetting senparcWeixinSetting = SenparcWeixinSetting.BuildFromWebConfig(false);

        public TencentPayService()
        {
            tenpayOptions = new TenpayOptions();
            client = new WechatTenpayClient(new WechatTenpayClientOptions()
            {
                MerchantId = tenpayOptions.MerchantId,
                MerchantV3Secret = tenpayOptions.SecretV3,
                MerchantCertificateSerialNumber = tenpayOptions.CertificateSerialNumber,
                MerchantCertificatePrivateKey = tenpayOptions.CertificatePrivateKey,
                PlatformCertificateManager = new InMemoryCertificateManager(),
                AutoEncryptRequestSensitiveProperty = true,
                AutoDecryptResponseSensitiveProperty = true
            });
        }

        public async Task<IDictionary<string,string>> CreateOrderByJsApi(CreatePayTransactionJsapiRequest reqData)
        {
            reqData.AppId = senparcWeixinSetting.WeixinAppId;
            reqData.NotifyUrl = tenpayOptions.NotifyUrl;
            var resData = await client.ExecuteCreatePayTransactionJsapiAsync(reqData);
            if (!resData.IsSuccessful())
                throw new BusinessException($"状态码:{resData.RawStatus},错误代码:{resData.ErrorCode},错误描述:{resData.ErrorMessage}");

            return client.GenerateParametersForJsapiPayRequest(senparcWeixinSetting.WeixinAppId, resData.PrepayId);
        }

        public async Task<TransactionResource> PaymentCallBack(string timestamp, string nonce, string content, string signature, string serialNumber)
        {
            bool valid = client.VerifyEventSignature(
                 callbackTimestamp: timestamp,
                 callbackNonce: nonce,
                 callbackBody: content,
                 callbackSignature: signature,
                 callbackSerialNumber: serialNumber, out Exception ex
             );
            if (!valid) throw new BusinessException("验签失败");

            var callbackModel = client.DeserializeEvent(content);
            switch (callbackModel.EventType?.ToUpper())
            {
                case "TRANSACTION.SUCCESS":
                    {
                        return client.DecryptEventResource<TransactionResource>(callbackModel);
                    }
                default:
                    throw new BusinessException("未知的通知类型");
            }
        }
    }

    public class TenpayOptions
    {
        public TenpayOptions()
        {
            MerchantId = WebConfigurationManager.AppSettings[$"Tenpay_MerchantId"];
            SecretV3 = WebConfigurationManager.AppSettings[$"Tenpay_SecretV3"];
            CertificateSerialNumber = WebConfigurationManager.AppSettings[$"Tenpay_CertificateSerialNumber"];
            CertificatePrivateKey = WebConfigurationManager.AppSettings[$"Tenpay_CertificatePrivateKey"];
            NotifyUrl = WebConfigurationManager.AppSettings[$"Tenpay_NotifyUrl"];
        }

        public string MerchantId { get; set; } = string.Empty;

        public string SecretV3 { get; set; } = string.Empty;

        public string CertificateSerialNumber { get; set; } = string.Empty;

        public string CertificatePrivateKey { get; set; } = string.Empty;

        public string NotifyUrl { get; set; } = string.Empty;
    }
}
