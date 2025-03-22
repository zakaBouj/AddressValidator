using AddressValidator.Console.Config;
using Azure.Identity;

namespace AddressValidator.Console.Services
{
    public class AzureMapsTokenService : IAzureMapsTokenService
    {
        private readonly AzureMapsConfig _configuration;
        
        public AzureMapsTokenService(AzureMapsConfig config)
        {
            _configuration = config;
        }

        public async Task<string> GetAccessTokenAsync()
        {
            var credentials = new DefaultAzureCredential();

            var accessToken = await credentials.GetTokenAsync(
                new Azure.Core.TokenRequestContext(
                    new[] {"https://atlas.microsoft.com/.default"}));
            
            return accessToken.Token;
        }
    }
    
}