using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Owin.Hosting;
using Owin;

namespace Zk.HotelPlatform.Api
{
    public class AppServer
    {
        private readonly Semaphore _sem = new Semaphore(0, 1);

        public void Start(int port, Action<IAppBuilder, HttpConfiguration> action)
        {
            this.Start(port, null, action);
        }

        public void Start(int port, HttpConfiguration config)
        {
            this.Start(port, config, null);
        }

        public void Start(int port, HttpConfiguration config, Action<IAppBuilder, HttpConfiguration> runBuilder)
        {
            void startup(IAppBuilder app)
            {
                if (config == null)
                {
                    config = new HttpConfiguration();
                }
                if (runBuilder != null)
                {
                    runBuilder.Invoke(app, config);
                }
                app.UseWebApi(config);
            }

            Task.Factory.StartNew(() =>
              {
                  using (WebApp.Start(new StartOptions($"http://*:{ port }"), startup))
                  {
                      _sem.WaitOne();
                  }
              },
              TaskCreationOptions.LongRunning | TaskCreationOptions.AttachedToParent)
                  .ContinueWith((Task runTask) =>
                  {
                      if (runTask.IsFaulted)
                      {
                          Console.WriteLine(runTask.Exception.Message);
                          if (runTask.Exception.InnerExceptions != null)
                          {
                              runTask.Exception.InnerExceptions.ToList().ForEach(ex =>
                              {
                                  Console.WriteLine(ex.Message);
                                  Console.WriteLine(ex.StackTrace);
                              });
                          }
                      }
                  });
        }

        public void Stop()
        {
            _sem.Release();
        }
    }
}
