using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Zk.HotelPlatform.Api
{
    public class ApiServiceHost : ServiceHost
    {
        protected override void RunAsConsole(string[] args)
        {
            var exitEvent = new ManualResetEvent(false);

            Console.CancelKeyPress += (sender, eventArgs) =>
            {
                eventArgs.Cancel = true;
                exitEvent.Set();
            };

            ApiServer.Instance.Start();

            exitEvent.WaitOne();

            ApiServer.Instance.Stop();

            Console.WriteLine("press any key to quit");
            Console.ReadKey();
        }

        protected override void RunAsService(string[] args)
        {
            ServiceBase.Run(new ServiceBase[] { new MainService() });
        }
    }
}
