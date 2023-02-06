using System.Web.Http;
using WebActivatorEx;
using Zk.HotelPlatform.Api;
using Swashbuckle.Application;
using System;
using System.Reflection;
using System.Linq;

namespace Zk.HotelPlatform.Api
{
    public class StartupSwagger
    {
        private static readonly Lazy<StartupSwagger> LazyInstance = new Lazy<StartupSwagger>();
        public static StartupSwagger Instance => LazyInstance.Value;

        public void ConfigureSwagger(HttpConfiguration config)
        {
            config.EnableSwagger(c =>
              {
                  var assembly = typeof(StartupSwagger).Assembly.GetName();
                  c.SingleApiVersion(assembly.Version.ToString(), assembly.Name);
                  c.IncludeXmlComments(GetXmlCommentsPath());
                  c.OAuth2("oauth2").Flow("client").TokenUrl("/token");
                  c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
              })
              .EnableSwaggerUi(c =>
              {
                  c.DocumentTitle("Api Document");
                  c.EnableOAuth2Support("b2bweb", "wz", "Client", "B2B WEB");
              });
        }

        protected static string GetXmlCommentsPath()
        {
            return $@"{AppDomain.CurrentDomain.BaseDirectory}\{MethodBase.GetCurrentMethod().DeclaringType.Namespace}.XML";
        }
    }
}
