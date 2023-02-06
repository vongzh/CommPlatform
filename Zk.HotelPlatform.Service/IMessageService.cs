using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Zk.HotelPlatform.Utils.Global.GlobalEnum;

namespace Zk.HotelPlatform.Service
{
    public interface IMessageService
    {
        void TestPub();
        void TestSub();
        void Notify(string msg, MessageTypeEnum messageType, int duration = 0, string redirect = "", string qs = "");
        void NotifyToUser(int userId, string msg, MessageTypeEnum messageType, int duration = 0, string redirect = "", string qs = "");
        void NotifyToUserApi(string token, int userId, string msg, MessageTypeEnum messageType, int duration = 0, string redirect = "", string qs = "");
        void NotifyToApi(string token, string msg, MessageTypeEnum messageType, int duration = 0, string redirect = "", string qs = "");
    }
}
