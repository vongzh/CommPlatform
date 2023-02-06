using Microsoft.Owin.Security.OAuth;
using System.Collections.Generic;
using Microsoft.Owin.Security;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Zk.HotelPlatform.Api.Providers.Impl
{
    internal class OAuthServerProvider : OAuthAuthorizationServerProvider
    {
        private readonly IAuthService _authService;
        public OAuthServerProvider(IAuthService authService)
        {
            _authService = authService;
        }

        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.TryGetBasicCredentials(out string clientId, out string clientSecret);

            if (!_authService.ValidateClient(clientId, clientSecret))
            {
                context.SetError("invalid_client");
                return;
            }

            context.Validated(clientId);
            await base.ValidateClientAuthentication(context);
        }

        public override async Task GrantClientCredentials(OAuthGrantClientCredentialsContext context)
        {
            var oAuthIdentity = new ClaimsIdentity(context.Options.AuthenticationType);
            oAuthIdentity.AddClaim(new Claim(ClaimTypes.Name, context.ClientId));
            oAuthIdentity.AddClaim(new Claim(ClaimTypes.Role, ClientAuthorize.Client));

            var dictionary = new Dictionary<string, string> { { "as:client_id", context.ClientId ?? string.Empty } };
            var props = new AuthenticationProperties(dictionary);
            var ticket = new AuthenticationTicket(oAuthIdentity, props);

            context.Validated(ticket);
            await base.GrantClientCredentials(context);
        }
    }
}
