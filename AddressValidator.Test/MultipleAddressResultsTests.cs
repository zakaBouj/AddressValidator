using System;
using System.Threading.Tasks;
using AddressValidator.Console.Models;
using AddressValidator.Console.Services;
using Moq;
using Xunit;
using FluentAssertions;

namespace AddressValidator.Test
{
    public class MultipleAddressResultsTests
    {
        private readonly Mock<IAzureMapsService> _mockAzureMapsService;
        private readonly AddressValidationService _validationService;
        private readonly double _confidenceThreshold = 0.8; // 80% threshold

        public MultipleAddressResultsTests()
        {
            _mockAzureMapsService = new Mock<IAzureMapsService>();
            _validationService = new AddressValidationService(_mockAzureMapsService.Object, _confidenceThreshold);
        }

        [Fact]
        public async Task ValidateAddressAsync_WithMultipleResults_UsesHighestConfidenceResult()
        {
            // Arrange
            var address = "123 Main St, Seattle, WA";
            
            // Mock response with multiple results with different confidence scores
            var searchResponse = new AddressSearchResponse
            {
                Results = new[]
                {
                    new AddressSearchResult
                    {
                        Type = "Point Address",
                        Score = 0.85, // Primary result
                        Position = new Position { Lat = 47.6062, Lon = -122.3321 },
                        Address = new Address
                        {
                            FreeformAddress = "123 Main St, Seattle, WA 98101",
                            Country = "United States",
                            CountryCode = "US"
                        }
                    },
                    new AddressSearchResult
                    {
                        Type = "Point Address",
                        Score = 0.75, // Secondary result
                        Position = new Position { Lat = 47.6062, Lon = -122.3421 },
                        Address = new Address
                        {
                            FreeformAddress = "123 Main St, Seattle, WA 98102",
                            Country = "United States",
                            CountryCode = "US"
                        }
                    }
                }
            };
            
            _mockAzureMapsService
                .Setup(service => service.SearchAddressAsync(address))
                .ReturnsAsync(searchResponse);

            // Act
            var result = await _validationService.ValidateAddressAsync(address);

            // Assert
            result.Should().NotBeNull();
            result.IsValid.Should().BeTrue(); // Should be valid based on highest score
            result.ConfidencePercentage.Should().Be(85); // Should use the highest score
            result.FreeformAddress.Should().NotBeNull();
            result.FreeformAddress.Should().Contain("98101"); // Should use data from the highest confidence result
        }

        [Fact]
        public async Task ValidateAddressAsync_WithSimilarConfidenceScores_UsesFirstResult()
        {
            // Arrange
            var address = "123 Ambiguous St";
            
            // Set up similar but not identical confidence scores
            var searchResponse = new AddressSearchResponse
            {
                Results = new[]
                {
                    new AddressSearchResult
                    {
                        Type = "Point Address",
                        Score = 0.86, // Marginally higher
                        Position = new Position { Lat = 40.7128, Lon = -74.0060 },
                        Address = new Address
                        {
                            FreeformAddress = "123 Ambiguous St, New York, NY 10001",
                            Country = "United States",
                            CountryCode = "US"
                        }
                    },
                    new AddressSearchResult
                    {
                        Type = "Point Address",
                        Score = 0.85, // Very close second
                        Position = new Position { Lat = 40.7128, Lon = -74.0070 },
                        Address = new Address
                        {
                            FreeformAddress = "123 Ambiguous St, New York, NY 10002",
                            Country = "United States",
                            CountryCode = "US"
                        }
                    }
                }
            };
            
            _mockAzureMapsService
                .Setup(service => service.SearchAddressAsync(address))
                .ReturnsAsync(searchResponse);

            // Act
            var result = await _validationService.ValidateAddressAsync(address);

            // Assert
            result.Should().NotBeNull();
            result.IsValid.Should().BeTrue();
            result.ConfidencePercentage.Should().Be(86); // Should select the highest score
            result.FreeformAddress.Should().NotBeNull();
            result.FreeformAddress.Should().Contain("10001"); // Should use the first address
        }

        [Fact]
        public async Task ValidateAddressAsync_WithManyResults_PicksHighestScoreEvenIfNotFirst()
        {
            // Arrange
            var address = "123 Multiple Results St";
            
            // Create a mock response with multiple results in non-descending order
            var searchResponse = new AddressSearchResponse
            {
                Results = new[]
                {
                    new AddressSearchResult
                    {
                        Type = "Point Address",
                        Score = 0.82,
                        Position = new Position { Lat = 34.0522, Lon = -118.2437 },
                        Address = new Address
                        {
                            FreeformAddress = "123 Multiple Results St, Los Angeles, CA 90001",
                            Country = "United States",
                            CountryCode = "US"
                        }
                    },
                    new AddressSearchResult
                    {
                        Type = "Point Address",
                        Score = 0.91, // Highest score but not first in the array
                        Position = new Position { Lat = 34.0522, Lon = -118.2437 },
                        Address = new Address
                        {
                            FreeformAddress = "123 Multiple Results St, Los Angeles, CA 90210",
                            Country = "United States",
                            CountryCode = "US"
                        }
                    },
                    new AddressSearchResult
                    {
                        Type = "Point Address",
                        Score = 0.75,
                        Position = new Position { Lat = 34.0522, Lon = -118.2437 },
                        Address = new Address
                        {
                            FreeformAddress = "123 Multiple Results St, Los Angeles, CA 90003",
                            Country = "United States",
                            CountryCode = "US"
                        }
                    }
                }
            };
            
            _mockAzureMapsService
                .Setup(service => service.SearchAddressAsync(address))
                .ReturnsAsync(searchResponse);

            // Act
            var result = await _validationService.ValidateAddressAsync(address);

            // Assert
            result.Should().NotBeNull();
            result.IsValid.Should().BeTrue();
            result.ConfidencePercentage.Should().Be(91); // Should pick the highest score even if not first
            result.FreeformAddress.Should().NotBeNull();
            result.FreeformAddress.Should().Contain("90210"); // Should use the highest confidence result regardless of position
        }
    }
}