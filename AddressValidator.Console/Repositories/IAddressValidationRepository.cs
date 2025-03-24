using AddressValidator.Console.Models;

namespace AddressValidator.Console.Repositories
{
    public interface IAddressValidationRepository
    {
        Task SaveValidationResultAsync(string originalQuery, AddressInput? addressInput, AddressValidationResult result);
        Task<IEnumerable<ValidationRecord>> GetValidationHistoryAsync(int maxResults = 20);
        Task<ValidationRecord?> GetValidationByIdAsync(string id);
        Task<bool> ClearHistoryAsync();
    }
}