using AddressValidator.Console.Models;

namespace AddressValidator.Console.Services
{
    public class AddressValidationService : IAddressValidationService
    {
        private readonly IAzureMapsService _azureMapsService;
        private readonly double _minimumConfidenceThreshold;

        public Task<AddressValidationResult> ValidateAddressAsync(AddressInput address)
        {
            throw new NotImplementedException();
        }

        public Task<AddressValidationResult> ValidateAddressAsync(string address)
        {
            throw new NotImplementedException();
        }
    }
}