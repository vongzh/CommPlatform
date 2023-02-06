using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Zk.HotelPlatform.Service.Impl
{
    public class MainService: IMainService
    {
        public dynamic GetServerInfo()
        {
            var assembly = Assembly.GetEntryAssembly();
            return new
            {
                Api = assembly.GetName().Name,
                Version = assembly.GetName().Version.ToString(),
                Architecture = assembly.GetName().ProcessorArchitecture.ToString(),
                RunTime = assembly.ImageRuntimeVersion,
                DateTime.Now
            };
        }
    }
}
