using System;
using System.Threading.Tasks;
using AddressValidator.Console.Models;
using AddressValidator.Console.Services;
using Moq;
using Xunit;
using FluentAssertions;

namespace AddressValidator.Test
{
    public class AddressValidationServiceTests
    {
        private readonly Mock<IAzureMapsService> _mockAzureMapsService;
        private readonly AddressValidationService _validationService;
        private readonly double _confidenceThreshold = 0.8; // 80% threshold

        public AddressValidationServiceTests()
        {
            _mockAzureMapsService = new Mock<IAzureMapsService>();
            _validationService = new AddressValidationService(_mockAzureMapsService.Object, _confidenceThreshold);
        }

        [Fact]
        public async Task ValidateAddressAsync_WithHighConfidence_ReturnsValidResult()
        {
            // Arrange
            var testAddress = "123 Main St, Seattle, WA 98101, USA";
            var highConfidenceResponse = CreateMockResponseWithScore(0.95); // 95% confidence
            
            _mockAzureMapsService
                .Setup(service => service.SearchAddressAsync(testAddress))
                .ReturnsAsync(highConfidenceResponse);

            // Act
            var result = await _validationService.ValidateAddressAsync(testAddress);

            // Assert
            result.Should().NotBeNull();
            result.IsValid.Should().BeTrue();
            result.ConfidencePercentage.Should().BeGreaterOrEqualTo(80);
            result.ValidationMessage.Should().Contain("valid");
        }

        [Fact]
        public async Task ValidateAddressAsync_WithLowConfidence_ReturnsInvalidResult()
        {
            // Arrange
            var testAddress = "Incomplete Address, Somewhere";
            var lowConfidenceResponse = CreateMockResponseWithScore(0.4); // 40% confidence
            
            _mockAzureMapsService
                .Setup(service => service.SearchAddressAsync(testAddress))
                .ReturnsAsync(lowConfidenceResponse);

            // Act
            var result = await _validationService.ValidateAddressAsync(testAddress);

            // Assert
            result.Should().NotBeNull();
            result.IsValid.Should().BeFalse();
            result.ConfidencePercentage.Should().BeLessThan(80);
            result.ValidationMessage.Should().Contain("below threshold");
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