using AddressValidator.Console.Models;

namespace AddressValidator.Console.Services
{
    public class AddressValidationService : IAddressValidationService
    {
        private readonly IAzureMapsService _azureMapsService;
        private readonly double _minimumConfidenceThresholdPercentage;

        public AddressValidationService(IAzureMapsService azureMapsService, double minimumConfidenceThreshold)
        {
            _azureMapsService = azureMapsService;
            _minimumConfidenceThresholdPercentage = minimumConfidenceThreshold * 100;
        }

        public async Task<AddressValidationResult> ValidateAddressAsync(AddressInput address)
        {
            string addressString = address.ToSingleLineString();

            var result = await ValidateAddressAsync(addressString);

            result.OriginalInput = address;

            return result;
        }

        public async Task<AddressValidationResult> ValidateAddressAsync(string addressString)
        {
            if (string.IsNullOrWhiteSpace(addressString))
            {
                throw new ArgumentException("Address cannot be empty or whitespace", nameof(addressString));
            }
            
            var result = new AddressValidationResult
            {
                IsValid = false,
                ConfidencePercentage = 0
            };

            try
            {
                var searchResponse = await _azureMapsService.SearchAddressAsync(addressString);

                if (searchResponse.Results.Length == 0)
                {
                    result.ValidationMessage = "No matching address found";
                    return result;
                }

                // Find the result with the highest confidence score
                var topMatch = searchResponse.Results[0];
                foreach (var currentResult in searchResponse.Results)
                {
                    if (currentResult.Score > topMatch.Score)
                    {
                        topMatch = currentResult;
                    }
                }

                result.ConfidencePercentage = Math.Round(topMatch.Score*100, 2);

                result.IsValid = result.ConfidencePercentage >= _minimumConfidenceThresholdPercentage;

                result.MatchedAddress = topMatch.Address;
                result.FreeformAddress = topMatch.Address?.FreeformAddress;
                result.Position = topMatch.Position;

                if (result.IsValid)
                {
                    result.ValidationMessage = "Address is valid";
                }
                else
                {
                    result.ValidationMessage = $"Address found but confidence score of {result.ConfidencePercentage}% is below threshold ({_minimumConfidenceThresholdPercentage}%).";
                }
            }
            catch (AzureMapsServiceException ex)
            {
                result.ValidationMessage = $"Error validating address: {ex.Message}";
            }
            catch (Exception ex)
            {
                result.ValidationMessage = $"Unexpected error: {ex.Message}";
            }
            
            return result;
        }
    }
}