using System.Web;
using AddressValidator.Console.Config;
using AddressValidator.Console.Models;
using Newtonsoft.Json;

namespace AddressValidator.Console.Services
{
    public class AzureMapsService : IAzureMapsService
    {
        private readonly HttpClient _httpClient;
        private readonly IAzureMapsTokenService _tokenService;
        private readonly AzureMapsConfig _config;

        public AzureMapsService(
            HttpClient httpClient, 
            IAzureMapsTokenService tokenService,
            AzureMapsConfig config)
        {
            _httpClient = httpClient;
            _tokenService = tokenService;
            _config = config;
            _httpClient.BaseAddress = new Uri(_config.AzureMapsEndpoint);
        }

        public async Task<AddressSearchResponse> SearchAddressAsync(string address)
        {
            if (string.IsNullOrWhiteSpace(address))
            {
                throw new ArgumentException("Address cannot be empty", nameof(address));
            }

            try
            {
                // Get a token
                var token = await _tokenService.GetAccessTokenAsync();
                
                // Encode the address
                var encodedAddress = HttpUtility.UrlEncode(address);
                
                // Build the request URI - without subscription-key parameter
                var requestUri = $"search/address/json?api-version=1.0&query={encodedAddress}&typeahead=false&limit=1";
                
                // Create request with auth headers
                var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
                
                // Add required headers
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                request.Headers.Add("x-ms-client-id", _config.ClientId);
                
                // Send the request
                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();
                
                // Read and parse the response
                var content = await response.Content.ReadAsStringAsync();
                var searchResponse = JsonConvert.DeserializeObject<AddressSearchResponse>(content) ?? 
                    throw new AzureMapsServiceException("Failed to deserialize response from Azure Maps");
                    
                return searchResponse;
            }
            catch (HttpRequestException ex)
            {
                throw new AzureMapsServiceException($"Error connecting to Azure Maps API: {ex.Message}", ex);
            }
            catch (JsonException ex)
            {
                throw new AzureMapsServiceException($"Error parsing Azure Maps response: {ex.Message}", ex);
            }
        }
    }
}
