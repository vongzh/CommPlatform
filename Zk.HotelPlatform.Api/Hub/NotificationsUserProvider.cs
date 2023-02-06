using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zk.HotelPlatform.Service;

namespace Zk.HotelPlatform.Api
{
    public class NotificationsUserProvider : IUserIdProvider
    {
        private const string _userId = "uid";

        public string GetUserId(IRequest request)
        {
            return request.QueryString[_userId];
        }
    }
}
