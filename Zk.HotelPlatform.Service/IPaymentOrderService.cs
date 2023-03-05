using SKIT.FlurlHttpClient.Wechat.TenpayV3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zk.HotelPlatform.Model;

namespace Zk.HotelPlatform.Service
{
    public interface IPaymentOrderService
    {
        void CreatePaymentOrders(OrderInfo orderInfo);
        IEnumerable<PaymentOrder> GetPaymentOrders(string orderNo, string openId);
        Task<IDictionary<string, string>> Payment(string[] paymentNos, string openId);
    }
}
