namespace AddressValidator.Console.Services
{
    public interface IAzureMapsTokenService
    {   
        Task<string> GetAccessTokenAsync();
    }
}