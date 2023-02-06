using AutoMapper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Zk.HotelPlatform.DBProvider;
using Zk.HotelPlatform.Model;
using Zk.HotelPlatform.Model.Request;
using Zk.HotelPlatform.Model.Response;
using Zk.HotelPlatform.Utils;
using Zk.HotelPlatform.Utils.Global;

namespace Zk.HotelPlatform.Service.Impl
{
    public class SysUserIPService : DataService<SysUserIP>, ISysUserIPService
    {

        public SysUserIPService(IDataProvider<SysUserIP> dataProvider)
            : base(dataProvider)
        {

        }
        public SysUserIP GetUserIP(int userId)
        {
            Expression<Func<SysUserIP, bool>> filter = x => x.UserId == userId && x.IsDelete == (int)GlobalEnum.YESOrNO.N;
            return base.Get(filter);
        }
    }
}
