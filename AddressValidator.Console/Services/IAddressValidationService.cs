using AddressValidator.Console.Models;

namespace AddressValidator.Console.Services
{
    public interface IAddressValidationService
    {
        Task<AddressValidationResult> ValidateAddressAsync(AddressInput address);

        Task<AddressValidationResult> ValidateAddressAsync(string address);
    }    
}