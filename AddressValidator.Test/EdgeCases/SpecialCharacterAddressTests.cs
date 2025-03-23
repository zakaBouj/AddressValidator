using System;
using System.Threading.Tasks;
using AddressValidator.Console.Models;
using AddressValidator.Console.Services;
using Moq;
using Xunit;
using FluentAssertions;

namespace AddressValidator.Test
{
    public class SpecialCharacterAddressTests
    {
        private readonly Mock<IAzureMapsService> _mockAzureMapsService;
        private readonly AddressValidationService _validationService;
        private readonly double _confidenceThreshold = 0.8; // 80% threshold

        public SpecialCharacterAddressTests()
        {
            _mockAzureMapsService = new Mock<IAzureMapsService>();
            _validationService = new AddressValidationService(_mockAzureMapsService.Object, _confidenceThreshold);
        }

        [Fact]
        public async Task ValidateAddressAsync_WithHashtag_HandlesCorrectly()
        {
            // Arrange - Address with # character
            var addressWithHash = "123# Main St, Seattle, WA 98101";
            
            var searchResponse = CreateMockResponse(
                "123# Main St, Seattle, WA 98101", 
                0.85, 
                new Position { Lat = 47.6062, Lon = -122.3321 });
            
            _mockAzureMapsService
                .Setup(service => service.SearchAddressAsync(addressWithHash))
                .ReturnsAsync(searchResponse);

            // Act
            var result = await _validationService.ValidateAddressAsync(addressWithHash);

            // Assert
            result.Should().NotBeNull();
            result.IsValid.Should().BeTrue();
            result.ConfidencePercentage.Should().BeGreaterOrEqualTo(80);
            result.FreeformAddress.Should().Contain("#");
        }

        [Fact]
        public async Task ValidateAddressAsync_WithAmpersand_HandlesCorrectly()
        {
            // Arrange - Address with & character
            var addressWithAmpersand = "Smith & Jones Building, 42 Oak Ave, Chicago, IL 60601";
            
            var searchResponse = CreateMockResponse(
                "Smith & Jones Building, 42 Oak Ave, Chicago, IL 60601", 
                0.88, 
                new Position { Lat = 41.8781, Lon = -87.6298 });
            
            _mockAzureMapsService
                .Setup(service => service.SearchAddressAsync(addressWithAmpersand))
                .ReturnsAsync(searchResponse);

            // Act
            var result = await _validationService.ValidateAddressAsync(addressWithAmpersand);

            // Assert
            result.Should().NotBeNull();
            result.IsValid.Should().BeTrue();
            result.ConfidencePercentage.Should().BeGreaterOrEqualTo(80);
            result.FreeformAddress.Should().Contain("&");
        }

        [Fact]
        public async Task ValidateAddressAsync_WithSlashFormatting_HandlesCorrectly()
        {
            // Arrange - Address with / character for Apt/Suite
            var addressWithSlash = "789 Pine Street, Apt/Suite 42B, Portland, OR 97204";
            
            var searchResponse = CreateMockResponse(
                "789 Pine Street, Suite 42B, Portland, OR 97204", 
                0.82, 
                new Position { Lat = 45.5152, Lon = -122.6784 });
            
            _mockAzureMapsService
                .Setup(service => service.SearchAddressAsync(addressWithSlash))
                .ReturnsAsync(searchResponse);

            // Act
            var result = await _validationService.ValidateAddressAsync(addressWithSlash);

            // Assert
            result.Should().NotBeNull();
            result.IsValid.Should().BeTrue();
            result.ConfidencePercentage.Should().BeGreaterOrEqualTo(80);
            // The API might normalize Apt/Suite to just Suite in the response
            result.FreeformAddress.Should().Contain("Suite");
        }

        [Fact]
        public async Task ValidateAddressAsync_WithPercentSign_HandlesCorrectly()
        {
            // Arrange - Address with % character (common in some business addresses)
            var addressWithPercent = "100% Natural Foods, 555 Market St, San Francisco, CA 94105";
            
            var searchResponse = CreateMockResponse(
                "100% Natural Foods, 555 Market St, San Francisco, CA 94105", 
                0.81, 
                new Position { Lat = 37.7749, Lon = -122.4194 });
            
            _mockAzureMapsService
                .Setup(service => service.SearchAddressAsync(addressWithPercent))
                .ReturnsAsync(searchResponse);

            // Act
            var result = await _validationService.ValidateAddressAsync(addressWithPercent);

            // Assert
            result.Should().NotBeNull();
            result.IsValid.Should().BeTrue();
            result.ConfidencePercentage.Should().BeGreaterOrEqualTo(80);
            result.FreeformAddress.Should().Contain("%");
        }

        [Fact]
        public async Task ValidateAddressAsync_WithApostropheInName_HandlesCorrectly()
        {
            // Arrange - Address with apostrophe in name
            var addressWithApostrophe = "Joe's Pizza, 123 Broadway, New York, NY 10010";
            
            var searchResponse = CreateMockResponse(
                "Joe's Pizza, 123 Broadway, New York, NY 10010", 
                0.90, 
                new Position { Lat = 40.7128, Lon = -74.0060 });
            
            _mockAzureMapsService
                .Setup(service => service.SearchAddressAsync(addressWithApostrophe))
                .ReturnsAsync(searchResponse);

            // Act
            var result = await _validationService.ValidateAddressAsync(addressWithApostrophe);

            // Assert
            result.Should().NotBeNull();
            result.IsValid.Should().BeTrue();
            result.ConfidencePercentage.Should().BeGreaterOrEqualTo(80);
            result.FreeformAddress.Should().Contain("Joe's");
        }

        private AddressSearchResponse CreateMockResponse(string freeformAddress, double score, Position position)
        {
            return new AddressSearchResponse
            {
                Results = new[]
                {
                    new AddressSearchResult
                    {
                        Type = "Point Address",
                        Score = score,
                        Position = position,
                        Address = new Address
                        {
                            FreeformAddress = freeformAddress,
                            Country = "United States",
                            CountryCode = "US"
                        }
                    }
                }
            };
        }
    }
}