namespace AddressValidator.Console.Services
{
    using AddressValidator.Console.Models;
    
    public interface IAzureMapsService
    {
        Task<AddressSearchResponse> SearchAddressAsync(string address);
    }
}
    