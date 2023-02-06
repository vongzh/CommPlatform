
namespace Zk.HotelPlatform.Api.Providers
{
    public interface IAuthService
    {
        bool ValidateClient(string clientId, string clientSecret);
    }
}
