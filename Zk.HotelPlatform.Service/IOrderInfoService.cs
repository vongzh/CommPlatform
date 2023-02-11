using Zk.HotelPlatform.Model;
using Zk.HotelPlatform.Model.Basic.Pager;
using Zk.HotelPlatform.Model.Request;

namespace Zk.HotelPlatform.Service
{
    public interface IOrderInfoService
    {
        bool CerateOrder(CreateOrderRequest createOrderRequest);
        OrderInfo GetOrderInfo(string orderNo);
        PageResult<OrderInfo> QueryOrders(QueryOrderRequest queryRequest);
    }
}
