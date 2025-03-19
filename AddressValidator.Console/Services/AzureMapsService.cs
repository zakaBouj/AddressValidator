
namespace AddressValidator.Console.Services
{
    using System.Web;
    using AddressValidator.Console.Models;
    using Newtonsoft.Json;

    public class AzureMapsService : IAzureMapsService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public AzureMapsService(HttpClient httpClient, string apiKey)
        {
            _apiKey = apiKey;
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://atlas.microsoft.com/");
        }

        public async Task<AddressSearchResponse> SearchAddressAsync(string address)
        {
            if (string.IsNullOrWhiteSpace(address))
            {
                throw new ArgumentException("Address cannot be empty", nameof(address));
            }

            try
            {
                // Encode the address
                var encodedAddress = HttpUtility.UrlEncode(address);
                
                // Build the request URI
                var requestUri = $"search/address/json?api-version=1.0&subscription-key={_apiKey}&query={encodedAddress}";
                requestUri += "&typeahead=false&limit=1";
                
                // Send the request
                var response = await _httpClient.GetAsync(requestUri);
                response.EnsureSuccessStatusCode();
                
                // Read and parse the response
                var content = await response.Content.ReadAsStringAsync();
                var searchResponse = JsonConvert.DeserializeObject<AddressSearchResponse>(content) ?? throw new AzureMapsServiceException("Failed to deserialize response from Azure Maps");
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
