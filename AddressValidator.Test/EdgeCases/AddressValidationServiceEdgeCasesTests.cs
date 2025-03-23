using System;
using System.Threading.Tasks;
using AddressValidator.Console.Models;
using AddressValidator.Console.Services;
using Moq;
using Xunit;
using FluentAssertions;

namespace AddressValidator.Test
{
    public class AddressValidationServiceEdgeCasesTests
    {
        private readonly Mock<IAzureMapsService> _mockAzureMapsService;
        private readonly AddressValidationService _validationService;
        private readonly double _confidenceThreshold = 0.8; // 80% threshold

        public AddressValidationServiceEdgeCasesTests()
        {
            _mockAzureMapsService = new Mock<IAzureMapsService>();
            _validationService = new AddressValidationService(_mockAzureMapsService.Object, _confidenceThreshold);
        }

        [Fact]
        public async Task ValidateAddressAsync_WithEmptyAddress_ThrowsArgumentException()
        {
            // Arrange
            string emptyAddress = "";

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => 
                _validationService.ValidateAddressAsync(emptyAddress));
        }

        [Fact]
        public async Task ValidateAddressAsync_WithNoResults_ReturnsInvalidResult()
        {
            // Arrange
            var testAddress = "Non-existent address";
            var emptyResponse = new AddressSearchResponse
            {
                Results = Array.Empty<AddressSearchResult>()
            };
            
            _mockAzureMapsService
                .Setup(service => service.SearchAddressAsync(testAddress))
                .ReturnsAsync(emptyResponse);

            // Act
            var result = await _validationService.ValidateAddressAsync(testAddress);

            // Assert
            result.Should().NotBeNull();
            result.IsValid.Should().BeFalse();
            result.ValidationMessage.Should().Contain("No matching address found");
        }

        [Fact]
        public async Task ValidateAddressAsync_WhenServiceThrowsException_HandlesGracefully()
        {
            // Arrange
            var testAddress = "Test address";
            _mockAzureMapsService
                .Setup(service => service.SearchAddressAsync(testAddress))
                .ThrowsAsync(new AzureMapsServiceException("Test exception"));

            // Act
            var result = await _validationService.ValidateAddressAsync(testAddress);

            // Assert
            result.Should().NotBeNull();
            result.IsValid.Should().BeFalse();
            result.ValidationMessage.Should().Contain("Error validating address");
        }

        [Fact]
        public async Task ValidateAddressAsync_WithExactThresholdConfidence_ReturnsValidResult()
        {
            // Arrange
            var testAddress = "Threshold test address";
            var thresholdResponse = CreateMockResponseWithScore(0.8); // Exactly at threshold
            
            _mockAzureMapsService
                .Setup(service => service.SearchAddressAsync(testAddress))
                .ReturnsAsync(thresholdResponse);

            // Act
            var result = await _validationService.ValidateAddressAsync(testAddress);

            // Assert
            result.Should().NotBeNull();
            result.IsValid.Should().BeTrue();
            result.ConfidencePercentage.Should().Be(80);
            result.ValidationMessage.Should().Contain("valid");
        }

        private AddressSearchResponse CreateMockResponseWithScore(double score)
        {
            return new AddressSearchResponse
            {
                Summary = new Summary
                {
                    Query = "Test query",
                    NumResults = 1,
                    TotalResults = 1
                },
                Results = new[]
                {
                    new AddressSearchResult
                    {
                        Type = "Point Address",
                        Score = score,
                        Position = new Position { Lat = 47.6062, Lon = -122.3321 },
                        Address = new Address
                        {
                            FreeformAddress = "123 Main St, Seattle, WA 98101",
                            Country = "United States",
                            CountryCode = "US"
                        }
                    }
                }
            };
        }
    }
}