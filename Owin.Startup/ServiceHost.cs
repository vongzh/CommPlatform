using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration.Install;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;

namespace Owin.Startup
{
    public abstract class ServiceHost
    {
        private readonly List<Installer> installs = new List<Installer>();

        public ServiceHost()
        {
            this.Initialize();
        }

        protected virtual void Initialize()
        {
            installs.Add(new ServiceProcessInstaller()
            {
                Account = ServiceAccount.LocalSystem
            });

            var assembly = Assembly.GetEntryAssembly();
            installs.Add(new ServiceInstaller()
            {
                Description = "Api Service",
                DisplayName = $"Api Service {assembly.GetName().Name}",
                ServiceName = assembly.GetName().Name,
                StartType = ServiceStartMode.Automatic,
            });
        }

        protected virtual void OnInstall()
        {
            try
            {
                using (var installer = new TransactedInstaller())
                {
                    string location = Assembly.GetEntryAssembly().Location;
                    using (var installerAssembly = new AssemblyInstaller())
                    {
                        installerAssembly.Path = location;
                        installerAssembly.UseNewContext = true;
                        installerAssembly.Installers.AddRange(installs.ToArray());

                        installer.Installers.Add(installerAssembly);
                        installer.Install(new Hashtable());
                    };
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine($"install service failed: {exception}");
            }
        }

        protected virtual void OnUninstall()
        {
            try
            {
                using (var installer = new TransactedInstaller())
                {
                    string location = Assembly.GetEntryAssembly().Location;
                    using (var installerAssembly = new AssemblyInstaller())
                    {
                        installerAssembly.Path = location;
                        installerAssembly.UseNewContext = true;
                        installerAssembly.Installers.AddRange(installs.ToArray());

                        installer.Installers.Add(installerAssembly);
                        installer.Uninstall(null);
                    };
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine($"uninstall service failed: {exception}");
            }
        }

        public void Run()
        {
            this.Run(new string[0]);
        }

        public void Run(string[] args)
        {
            if (args == null)
                throw new ArgumentNullException("args");

            if (args.Length == 0)
            {
                this.RunAsService(new string[0]);
                return;
            }

            var cmd = args[0].Trim().ToLower();
            //处理后续命令行
            string[] continueCmd = args.Skip<string>(1).ToArray<string>();
            switch (cmd)
            {
                case "-i":
                case "/i":
                    this.OnInstall();
                    break;
                case "-u":
                case "/u":
                    this.OnUninstall();
                    break;
                case "-d":
                case "/d":
                    this.RunAsConsole(continueCmd);
                    break;
                case "-s":
                case "/s":
                    this.RunAsService(continueCmd);
                    break;
                default:
                    Console.Write("unknown cmd");
                    break;
            }
        }

        protected abstract void RunAsConsole(string[] args);
        protected abstract void RunAsService(string[] args);
    }
}
