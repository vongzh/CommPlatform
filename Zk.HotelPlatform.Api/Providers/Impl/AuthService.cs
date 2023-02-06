using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zk.HotelPlatform.Utils.Global;

namespace Zk.HotelPlatform.Api.Providers.Impl
{
    public class AuthService : IAuthService
    {
        private static readonly IDictionary<string, string> ClientDictionary;

        static AuthService()
        {
            ClientDictionary = GlobalConfig.ClientDictionary;
        }

        public bool ValidateClient(string clientId, string clientSecret)
        {
            if (clientId == null)
                return false;

            string vailedClientSecret;
            if (!ClientDictionary.TryGetValue(clientId, out vailedClientSecret))
                return false;

            return vailedClientSecret == clientSecret;
        }
    }
}
