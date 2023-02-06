using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Layout;
using log4net.Repository;
using log4net.Repository.Hierarchy;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Web;

namespace RedisClientAchieve.Utils
{
    public class LogInfoWriter
    {
        private static readonly string ServerIp = GetLocalIpAll();
        private static readonly string ServerHost = Dns.GetHostName();

        private static readonly string LogLevel = (ConfigurationManager.AppSettings["LogLevel"] ?? string.Empty);
        private static readonly string LogFolder = (ConfigurationManager.AppSettings["LogFolder"] ?? string.Empty);
        private static readonly string MaxLogFileSize = (ConfigurationManager.AppSettings["MaxLogFileSize"] ?? string.Empty);

        private static readonly Dictionary<string, LogInfoWriter> InstanceDict = new Dictionary<string, LogInfoWriter>();
        private readonly ILog _errorlogger;
        private readonly ILog _infologger;
        private readonly ILog _warnlogger;
        private readonly string _logFile;

        private static readonly object LockObj = new object();

        public LogInfoWriter(string logFile = "", int maxLogFileSize = 0, LogLayout logLayout = 0)
        {
            this._logFile = logFile;
            this._infologger = CreateloggerByLevel(Level.Info, logFile, maxLogFileSize, logLayout);
            this._warnlogger = CreateloggerByLevel(Level.Warn, logFile, maxLogFileSize, logLayout);
            this._errorlogger = CreateloggerByLevel(Level.Error, logFile, maxLogFileSize, logLayout);
        }

        public static LogInfoWriter GetInstance(string logFile = "", int maxLogFileSize = 0, LogLayout logLayout = 0)
        {
            if (InstanceDict.ContainsKey(logFile))
            {
                return InstanceDict[logFile];
            }
            lock (LockObj)
            {
                if (InstanceDict.ContainsKey(logFile))
                {
                    return InstanceDict[logFile];
                }
                LogInfoWriter writer = new LogInfoWriter(logFile, maxLogFileSize, logLayout);
                InstanceDict.Add(logFile, writer);
                return writer;
            }
        }

        public void Error(object message)
        {
            if (MyLogLevel <= Level.Error)
            {
                this._errorlogger.Error(message);
            }
        }

        public void Error(object message, Exception exception)
        {
            if (MyLogLevel <= Level.Error)
            {
                this._errorlogger.Error(message, exception);
            }
        }

        public void ErrorFormat(string format, object arg0)
        {
            if (MyLogLevel <= Level.Error)
            {
                this._errorlogger.ErrorFormat(format, arg0);
            }
        }

        public void ErrorFormat(string format, params object[] args)
        {
            if (MyLogLevel <= Level.Error)
            {
                this._errorlogger.ErrorFormat(format, args);
            }
        }

        public void ErrorFormat(IFormatProvider provider, string format, params object[] args)
        {
            if (MyLogLevel <= Level.Error)
            {
                this._errorlogger.ErrorFormat(provider, format, args);
            }
        }

        public void ErrorFormat(string format, object arg0, object arg1)
        {
            if (MyLogLevel <= Level.Error)
            {
                this._errorlogger.ErrorFormat(format, arg0, arg1);
            }
        }

        public void ErrorFormat(string format, object arg0, object arg1, object arg2)
        {
            if (MyLogLevel <= Level.Error)
            {
                this._errorlogger.ErrorFormat(format, arg0, arg1, arg2);
            }
        }

        internal static string GetLocalIpAll()
        {
            try
            {
                IEnumerable<IPAddress> values = from p in Dns.GetHostEntry(Dns.GetHostName()).AddressList
                                                where p.AddressFamily == AddressFamily.InterNetwork
                                                select p;
                return string.Join<IPAddress>(",", values);
            }
            catch
            {
                return string.Empty;
            }
        }

        public void Info(object message)
        {
            if (MyLogLevel <= Level.Info)
            {
                this._infologger.Info(message);
            }
        }

        public void Info(object message, Exception exception)
        {
            if (MyLogLevel <= Level.Info)
            {
                this._infologger.Info(message, exception);
            }
        }

        public void InfoFormat(string format, object arg0)
        {
            if (MyLogLevel <= Level.Info)
            {
                this._infologger.InfoFormat(format, arg0);
            }
        }

        public void InfoFormat(string format, params object[] args)
        {
            if (MyLogLevel <= Level.Info)
            {
                this._infologger.InfoFormat(format, args);
            }
        }

        public void InfoFormat(IFormatProvider provider, string format, params object[] args)
        {
            if (MyLogLevel <= Level.Info)
            {
                this._infologger.InfoFormat(provider, format, args);
            }
        }

        public void InfoFormat(string format, object arg0, object arg1)
        {
            if (MyLogLevel <= Level.Info)
            {
                this._infologger.InfoFormat(format, arg0, arg1);
            }
        }

        public void InfoFormat(string format, object arg0, object arg1, object arg2)
        {
            if (MyLogLevel <= Level.Info)
            {
                this._infologger.InfoFormat(format, arg0, arg1, arg2);
            }
        }

        public void Warn(object message)
        {
            if (MyLogLevel <= Level.Warn)
            {
                this._warnlogger.Warn(message);
            }
        }

        public void Warn(object message, Exception exception)
        {
            if (MyLogLevel <= Level.Warn)
            {
                this._warnlogger.Warn(message, exception);
            }
        }

        public void WarnFormat(string format, object arg0)
        {
            if (MyLogLevel <= Level.Warn)
            {
                this._warnlogger.WarnFormat(format, arg0);
            }
        }

        public void WarnFormat(string format, params object[] args)
        {
            if (MyLogLevel <= Level.Warn)
            {
                this._warnlogger.WarnFormat(format, args);
            }
        }

        public void WarnFormat(IFormatProvider provider, string format, params object[] args)
        {
            if (MyLogLevel <= Level.Warn)
            {
                this._warnlogger.WarnFormat(provider, format, args);
            }
        }

        public void WarnFormat(string format, object arg0, object arg1)
        {
            if (MyLogLevel <= Level.Warn)
            {
                this._warnlogger.WarnFormat(format, arg0, arg1);
            }
        }

        public void WarnFormat(string format, object arg0, object arg1, object arg2)
        {
            if (MyLogLevel <= Level.Warn)
            {
                this._warnlogger.WarnFormat(format, arg0, arg1, arg2);
            }
        }


        private static ILog CreateloggerByLevel(Level level, string logFile, int maxLogFileSize, LogLayout logLayout)
        {
            string str = logFile + level;
            ConsoleAppender newAppender = new ConsoleAppender
            {
                Layout = new PatternLayout("%d [%t] %-5p %c [%x] - %m%n")
            };
            ILoggerRepository repository;
            try
            {
                repository = LogManager.CreateRepository(str + "Repository");
            }
            catch
            {
                repository = LogManager.GetRepository(str + "Repository");
            }
            Hierarchy hierarchy = (Hierarchy)repository;
            hierarchy.Root.AddAppender(newAppender);
            newAppender.ActivateOptions();
            RollingFileAppender fileAppender = new RollingFileAppender();
            string levelStr = level.ToString();
            if (level == Level.Warn)
            {
                levelStr = "WARNING";
            }
            string logFolder = LogFolder;
            if ((HttpContext.Current == null) || (HttpContext.Current.Handler == null))
            {
                Assembly entryAssembly = Assembly.GetEntryAssembly();
                if (entryAssembly == null)
                {
                    entryAssembly = Assembly.GetExecutingAssembly();
                }

                logFolder = logFolder.Replace("{CurrentDomain}", entryAssembly?.GetName().Name);
                if (string.IsNullOrWhiteSpace(logFolder))
                {
                    var appName = entryAssembly?.GetName().Name;
                    if (appName.Contains('.'))
                    {
                        if (!string.IsNullOrWhiteSpace(appName))
                        {
                            var index = appName.LastIndexOf('.');
                            var length = appName.Length - 1;
                            logFolder = appName.Substring(index + 1, length - index);
                        }
                    }
                    else
                    {
                        logFolder = appName;
                    }
                }
            }
            else
            {
                Type baseType = HttpContext.Current.Handler.GetType().BaseType;
                if (baseType != null)
                {
                    var domain = ((baseType.Assembly == null) ? null : (baseType.Assembly.GetName().Name));
                    logFolder = logFolder.Replace("{CurrentDomain}", domain);
                    if (string.IsNullOrWhiteSpace(logFolder))
                    {
                        if (domain.Contains('.'))
                        {
                            if (!string.IsNullOrWhiteSpace(domain))
                            {
                                logFolder = domain;
                            }
                        }
                        else
                        {
                            logFolder = domain;
                        }
                    }
                }
            }
            var defaultDirectory = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).Name;
            logFolder = logFolder.Replace("{CurrentDirectory}", defaultDirectory);
            if (!string.IsNullOrEmpty(logFolder) && !logFolder.EndsWith(@"\"))
            {
                logFolder += @"\";
            }
            if (string.IsNullOrWhiteSpace(logFolder))
            {
                logFolder = defaultDirectory;
            }
            string[] textArray = new string[] { logFolder, logFile, @"\", levelStr, @"\" };
            fileAppender.File = string.Concat(textArray);
            fileAppender.AppendToFile = true;
            fileAppender.MaxSizeRollBackups = -1;
            if (!string.IsNullOrEmpty(MaxLogFileSize))
            {
                fileAppender.MaximumFileSize = MaxLogFileSize + "MB";
                fileAppender.RollingStyle = RollingFileAppender.RollingMode.Composite;
            }
            else if (maxLogFileSize > 0)
            {
                fileAppender.MaximumFileSize = maxLogFileSize + "MB";
                fileAppender.RollingStyle = RollingFileAppender.RollingMode.Composite;
            }
            else
            {
                fileAppender.RollingStyle = RollingFileAppender.RollingMode.Date;
            }
            fileAppender.DatePattern = "yyyy-MM-dd\".txt\"";
            fileAppender.StaticLogFileName = false;
            fileAppender.LockingModel = new FileAppender.MinimalLock();
            if (logLayout == LogLayout.Default)
            {
                fileAppender.Layout = new PatternLayout("%date [%thread] %-5level - %message%newline");
            }
            else if (logLayout == LogLayout.WebCustom)
            {
                string[] txtArr = new string[] { "%date %newline L-message ：%message%newline L_Level ：%-5level%newline L_Folder ：", logFile, "%newline L_CreatTime:%date%newline L_ServerHostName ：", ServerHost, "%newline L_ServerHostIP ：", ServerIp, "%newline---------------------------------------%newline" };
                fileAppender.Layout = new PatternLayout(string.Concat(txtArr));
            }
            fileAppender.Encoding = Encoding.UTF8;
            fileAppender.ActivateOptions();
            hierarchy.Root.AddAppender(fileAppender);
            hierarchy.Configured = true;
            return LogManager.GetLogger(repository.Name, str + "Log");
        }

        private static Level MyLogLevel
        {
            get
            {
                Level all = Level.All;
                string str = LogLevel.ToUpper();
                if (str != "INFO")
                {
                    if (str != "WARN")
                    {
                        if (str != "ERROR")
                            return all;

                        return Level.Error;
                    }
                }
                else
                    return Level.Info;

                return Level.Warn;
            }
        }

        public enum LogLayout
        {
            Default,
            WebCustom,
            Console
        }
    }
}

