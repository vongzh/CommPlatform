namespace Zk.HotelPlatform.Service
{
    public interface IPayService
    {
        //(bool Result, string Message, TradeCreateResponse Data) CreateTrade(TradeCreateRequest trade);
        (bool PayResult, string Message) CloseTrade(string paymentOrderNo);
        (bool PayResult, string Message, string tradrNo) QueryTradeResult(string paymentOrderNo, decimal orderAmount);
        (bool RefundResult, string Message) RefundTrade(string paymentNo, string refundNo, decimal paymentAmount, decimal refundAmount);
    }
}
