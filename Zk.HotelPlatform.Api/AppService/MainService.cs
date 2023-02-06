using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Zk.HotelPlatform.Api
{
    public class MainService : ServiceBase
    {
        protected override void OnStart(string[] args)
        {
            ApiServer.Instance.Start();
        }

        protected override void OnStop()
        {
            ApiServer.Instance.Stop();
        }
    }
}
