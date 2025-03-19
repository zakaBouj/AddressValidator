public interface IAzureMapsService
{
    Task<AddressSearchResponse> SearchAddressAsync(string address);
}