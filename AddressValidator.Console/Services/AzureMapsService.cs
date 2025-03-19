public class AzureMapService : IAzureMapsService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public AzureMapService(HttpClient httpClient, string apiKey)
    {
        _apiKey = apiKey;
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri("https://atlas.microsoft.com/");
    }

    // TODO: Implement the SearchAddressAsync method
    public Task<AddressSearchResponse> SearchAddressAsync(string address)
    {
        throw new NotImplementedException();
    }
}