using System;
using System.Threading.Tasks;
using AddressValidator.Console.Models;
using AddressValidator.Console.Services;
using Moq;
using Xunit;
using FluentAssertions;

namespace AddressValidator.Test
{
    public class IntegrationTests
    {
        [Fact]
        public async Task AddressValidation_CompleteFlow_SuccessfullyProcessesAddress()
        {
            // Arrange
            // 1. Create a mock for the Azure Maps Service
            var mockAzureMapsService = new Mock<IAzureMapsService>();
            
            // 2. Set up the mock to return a valid response for our test address
            var testAddressInput = new AddressInput
            {
                AddressLine1 = "123 Main St",
                AddressLine2 = "Apt 4B",
                AddressLine3 = "Building C",
                PostalCode = "98052",
                City = "Redmond",
                Country = "USA"
            };
            
            var formattedAddress = testAddressInput.ToSingleLineString();
            
            mockAzureMapsService
                .Setup(service => service.SearchAddressAsync(formattedAddress))
                .ReturnsAsync(new AddressSearchResponse
                {
                    Results = new[]
                    {
                        new AddressSearchResult
                        {
                            Type = "Point Address",
                            Score = 0.95, // High confidence
                            Position = new Position { Lat = 47.6801, Lon = -122.1206 },
                            Address = new Address
                            {
                                FreeformAddress = "123 Main St, Apt 4B, Redmond, WA 98052",
                                StreetNumber = "123",
                                StreetName = "Main St",
                                Municipality = "Redmond",
                                CountrySubdivisionName = "Washington",
                                PostalCode = "98052",
                                Country = "United States",
                                CountryCode = "US"
                            }
                        }
                    }
                });
            
            // 3. Create the validation service with our mock
            var validationService = new AddressValidationService(mockAzureMapsService.Object, 0.8);
            
            // Act
            // 4. Process the complete flow - from input to validation result
            var result = await validationService.ValidateAddressAsync(testAddressInput);
            
            // Assert
            // 5. Verify the entire flow produced the expected result
            result.Should().NotBeNull();
            result.IsValid.Should().BeTrue();
            result.ConfidencePercentage.Should().BeGreaterOrEqualTo(80);
            result.FreeformAddress.Should().NotBeNull();
            result.FreeformAddress!.Should().Contain("123 Main St");
            result.FreeformAddress!.Should().Contain("Redmond");
            result.Position.Should().NotBeNull();
            result.Position.Lat.Should().BeApproximately(47.6801, 0.001);
            result.Position.Lon.Should().BeApproximately(-122.1206, 0.001);
            result.OriginalInput.Should().NotBeNull();
            result.OriginalInput.Should().BeSameAs(testAddressInput);
            result.ValidationMessage.Should().Contain("valid");
            
            // 6. Verify the service was called with the correctly formatted address
            mockAzureMapsService.Verify(service => 
                service.SearchAddressAsync(formattedAddress), Times.Once);
        }
        
        [Fact]
        public async Task AddressValidation_WithInvalidInput_ReturnsDetailedErrorMessage()
        {
            // Arrange
            var mockAzureMapsService = new Mock<IAzureMapsService>();
            var invalidAddressInput = new AddressInput
            {
                AddressLine1 = "Invalid Address",
                PostalCode = "XXXXX",
                City = "Unknown",
                Country = "Nowhere"
            };
            
            var formattedAddress = invalidAddressInput.ToSingleLineString();
            
            mockAzureMapsService
                .Setup(service => service.SearchAddressAsync(formattedAddress))
                .ReturnsAsync(new AddressSearchResponse
                {
                    Results = new[]
                    {
                        new AddressSearchResult
                        {
                            Score = 0.3, // Low confidence
                            Position = new Position { Lat = 0, Lon = 0 },
                            Address = new Address
                            {
                                FreeformAddress = "1111 Some St, Some City, XX 00000"
                            }
                        }
                    }
                });
            
            var validationService = new AddressValidationService(mockAzureMapsService.Object, 0.8);
            
            // Act
            var result = await validationService.ValidateAddressAsync(invalidAddressInput);
            
            // Assert
            result.Should().NotBeNull();
            result.IsValid.Should().BeFalse();
            result.ConfidencePercentage.Should().BeLessThan(80);
            result.ValidationMessage.Should().Contain("confidence");
            result.ValidationMessage.Should().Contain("below threshold");
            result.OriginalInput.Should().BeSameAs(invalidAddressInput);
        }
    }
}