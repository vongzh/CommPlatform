using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zk.HotelPlatform.Model;

namespace Zk.HotelPlatform.Service
{
    public interface ISignupService : IDataService<Signup>
    {
        bool SaveSignup(Signup signup);
    }
}
