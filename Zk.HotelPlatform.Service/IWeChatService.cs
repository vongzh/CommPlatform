using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zk.HotelPlatform.Model;

namespace Zk.HotelPlatform.Service
{
    public interface IWeChatService : IDataService<WeChatUserBind>
    {
        int GetWechatBindUser(string code);
    }
}
