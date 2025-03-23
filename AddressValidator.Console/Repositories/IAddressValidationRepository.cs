namespace AddressValidator.Console.Repositories
{
    public interface IAddressValidationRepository
    {
        Task SaveValidationResultAsync(string originalQuery, AddressInput? addressInput, AddressValidationResult result);
        Task<IEnumerable<ValidationRecord>> GetValidationHistoryAsync(int maxResults = 10);
        Task<ValidationRecord?> GetValidationByIdAsync(string id);
        Task<bool> ClearHistoryAsync();
    }
}