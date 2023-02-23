using Autofac;
using Autofac.Integration.SignalR;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Owin;
using System;
using System.Net.Http.Formatting;
using System.Reflection;
using System.Web.Http;
using Zk.HotelPlatform.Api.Filters;
using Zk.HotelPlatform.Utils.Global;
using Zk.HotelPlatform.Utils.Log;

namespace Zk.HotelPlatform.Api
{
    public class ApiServer
    {
        private AppServer _appServer;
        public static ApiServer Instance => InstanceLazy.Value;
        private static readonly Lazy<ApiServer> InstanceLazy = new Lazy<ApiServer>();

        public void Start()
        {
            try
            {
                _appServer = new AppServer();
                _appServer.Start(GlobalConfig.Port, (app, config) =>
                {
                    //AutoFac
                    StartupAutofac.Instance.Register(config);
                    app.UseAutofacMiddleware(StartupAutofac.Instance.Container);

                    //Throttle
                    //StartupThrottling.Instance.InitThrottlePolicy(app, StartupAutofac.Instance.Container);

                    //WeChat
                    StartupWeChat.Instance.ConfigureWebChat(app);

#if DEBUG
                    //Swagger
                    StartupSwagger.Instance.ConfigureSwagger(config);
#endif

                    config.Register();
                    config.ConfigureAuth();
                    config.ConfigureJsonFormatter();

                    //Filter
                    config.Filters.Add(new CrosAttribute());
                    config.Filters.Add(new PermissionAttribute());
                    config.Filters.Add(new OperationLogAttribute());
                    config.Filters.Add(new ExceptionHandlerAttribute());

                    //Owin Context
                    app.UseRequestScopeContext();

                    //Cros
                    app.UseCors(CorsOptions.AllowAll);

                    //OAuth
                    app.UseOAuthAuthorizationServer(HttpConfigruationExtend.OAuthOptions);
                    app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());
                    
                    //SignalR
                    var dependencyResolver = new AutofacDependencyResolver(StartupAutofac.Instance.Container);
                    app.MapSignalR(new HubConfiguration()
                    {
                        Resolver = dependencyResolver
                    });
                    GlobalHost.DependencyResolver = dependencyResolver;

                    app.UseAutofacWebApi(config);
                });

                {

                    LogInfoWriter.GetInstance().Info($"{ Assembly.GetExecutingAssembly().GetName().Name } is start on http port :{ GlobalConfig.Port }");
                }
            }
            catch (Exception ex)
            {

                LogInfoWriter.GetInstance().Error(ex);
            }
        }

        public void Stop()
        {
            try
            {
                StartupAutofac.Instance.Container?.Dispose();

                _appServer?.Stop();
            }
            catch (Exception ex)
            {
                LogInfoWriter.GetInstance().Error(ex);
            }
        }
    }

    public static class HttpConfigruationExtend
    {
        #region Routes
        public static void Register(this HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "{controller}/{action}/{id}",
                defaults: new { controller = "Main", action = "Index", id = RouteParameter.Optional }
            );
        }
        #endregion

        #region OAuth
        public static OAuthAuthorizationServerOptions OAuthOptions { get; private set; }
        public static string PublicClientId { get; private set; }
        public static void ConfigureAuth(this HttpConfiguration config)
        {
            OAuthOptions = new OAuthAuthorizationServerOptions
            {
                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("/token"),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(30),
                Provider = StartupAutofac.Instance.Container.Resolve<IOAuthAuthorizationServerProvider>()
            };
        }
        #endregion

        #region Json
        public static void ConfigureJsonFormatter(this HttpConfiguration config)
        {
            var cfg = new JsonMediaTypeFormatter();
            cfg.SerializerSettings.Formatting = Formatting.Indented;
            cfg.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            cfg.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            cfg.SerializerSettings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
            cfg.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver()
            {
                IgnoreSerializableAttribute = true
            };
            config.Formatters.Clear();
            config.Formatters.Add(cfg);
        }
        #endregion
    }
}
