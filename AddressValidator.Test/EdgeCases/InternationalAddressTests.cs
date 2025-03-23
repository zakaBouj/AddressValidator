using System;
using System.Threading.Tasks;
using AddressValidator.Console.Models;
using AddressValidator.Console.Services;
using Moq;
using Xunit;
using FluentAssertions;

namespace AddressValidator.Test
{
    public class InternationalAddressTests
    {
        private readonly Mock<IAzureMapsService> _mockAzureMapsService;
        private readonly AddressValidationService _validationService;
        private readonly double _confidenceThreshold = 0.8; // 80% threshold

        public InternationalAddressTests()
        {
            _mockAzureMapsService = new Mock<IAzureMapsService>();
            _validationService = new AddressValidationService(_mockAzureMapsService.Object, _confidenceThreshold);
        }

        [Fact]
        public async Task ValidateAddressAsync_WithJapaneseCharacters_HandlesCorrectly()
        {
            // Arrange - Japanese address with Kanji
            var japaneseAddress = "東京都新宿区西新宿2-8-1";
            
            var searchResponse = CreateMockResponse(
                "2-8-1 Nishishinjuku, Shinjuku City, Tokyo 160-0023, Japan",
                0.87,
                new Position { Lat = 35.6894, Lon = 139.6917 },
                "Japan",
                "JP");
            
            _mockAzureMapsService
                .Setup(service => service.SearchAddressAsync(japaneseAddress))
                .ReturnsAsync(searchResponse);

            // Act
            var result = await _validationService.ValidateAddressAsync(japaneseAddress);

            // Assert
            result.Should().NotBeNull();
            result.IsValid.Should().BeTrue();
            result.ConfidencePercentage.Should().BeGreaterOrEqualTo(80);
            result.MatchedAddress.Should().NotBeNull();
            result.MatchedAddress!.Country.Should().Be("Japan");
        }

        [Fact]
        public async Task ValidateAddressAsync_WithUKFormat_HandlesCorrectly()
        {
            // Arrange - UK format (different from US format)
            var ukAddress = "10 Downing Street, London, SW1A 2AA, United Kingdom";
            
            var searchResponse = CreateMockResponse(
                "10 Downing St, Westminster, London SW1A 2AA, UK",
                0.92,
                new Position { Lat = 51.5034, Lon = -0.1276 },
                "United Kingdom",
                "GB");
            
            _mockAzureMapsService
                .Setup(service => service.SearchAddressAsync(ukAddress))
                .ReturnsAsync(searchResponse);

            // Act
            var result = await _validationService.ValidateAddressAsync(ukAddress);

            // Assert
            result.Should().NotBeNull();
            result.IsValid.Should().BeTrue();
            result.ConfidencePercentage.Should().BeGreaterOrEqualTo(80);
            result.MatchedAddress.Should().NotBeNull();
            result.MatchedAddress!.CountryCode.Should().Be("GB");
        }

        [Fact]
        public async Task ValidateAddressAsync_WithGermanFormat_HandlesCorrectly()
        {
            // Arrange - German format with postal code before city
            var germanAddress = "Unter den Linden 77, 10117 Berlin, Deutschland";
            
            var searchResponse = CreateMockResponse(
                "Unter den Linden 77, 10117 Berlin, Germany",
                0.89,
                new Position { Lat = 52.5163, Lon = 13.3779 },
                "Germany",
                "DE");
            
            _mockAzureMapsService
                .Setup(service => service.SearchAddressAsync(germanAddress))
                .ReturnsAsync(searchResponse);

            // Act
            var result = await _validationService.ValidateAddressAsync(germanAddress);

            // Assert
            result.Should().NotBeNull();
            result.IsValid.Should().BeTrue();
            result.ConfidencePercentage.Should().BeGreaterOrEqualTo(80);
            result.MatchedAddress.Should().NotBeNull();
            result.MatchedAddress!.CountryCode.Should().Be("DE");
        }

        [Fact]
        public async Task ValidateAddressAsync_WithFrenchFormat_HandlesCorrectly()
        {
            // Arrange - French address format
            var frenchAddress = "16 Rue de la Paix, 75002 Paris, France";
            
            var searchResponse = CreateMockResponse(
                "16 Rue de la Paix, 75002 Paris, France",
                0.91,
                new Position { Lat = 48.8698, Lon = 2.3305 },
                "France",
                "FR");
            
            _mockAzureMapsService
                .Setup(service => service.SearchAddressAsync(frenchAddress))
                .ReturnsAsync(searchResponse);

            // Act
            var result = await _validationService.ValidateAddressAsync(frenchAddress);

            // Assert
            result.Should().NotBeNull();
            result.IsValid.Should().BeTrue();
            result.ConfidencePercentage.Should().BeGreaterOrEqualTo(80);
            result.MatchedAddress.Should().NotBeNull();
            result.MatchedAddress!.CountryCode.Should().Be("FR");
        }

        [Fact]
        public async Task ValidateAddressAsync_WithDiacriticalMarks_HandlesCorrectly()
        {
            // Arrange - Address with diacritical marks (accents, umlauts)
            var addressWithDiacriticals = "Mönckebergstraße 7, 20095 Hamburg, Deutschland";
            
            var searchResponse = CreateMockResponse(
                "Mönckebergstraße 7, 20095 Hamburg, Germany",
                0.88,
                new Position { Lat = 53.5511, Lon = 9.9937 },
                "Germany",
                "DE");
            
            _mockAzureMapsService
                .Setup(service => service.SearchAddressAsync(addressWithDiacriticals))
                .ReturnsAsync(searchResponse);

            // Act
            var result = await _validationService.ValidateAddressAsync(addressWithDiacriticals);

            // Assert
            result.Should().NotBeNull();
            result.IsValid.Should().BeTrue();
            result.ConfidencePercentage.Should().BeGreaterOrEqualTo(80);
            result.FreeformAddress.Should().Contain("Mönckebergstraße");
        }

        [Fact]
        public async Task ValidateAddressAsync_WithCyrillicCharacters_HandlesCorrectly()
        {
            // Arrange - Russian address with Cyrillic characters
            var russianAddress = "Красная площадь, 1, Москва, 109012, Россия";
            
            var searchResponse = CreateMockResponse(
                "Red Square, 1, Moscow, 109012, Russia",
                0.83,
                new Position { Lat = 55.7539, Lon = 37.6208 },
                "Russia",
                "RU");
            
            _mockAzureMapsService
                .Setup(service => service.SearchAddressAsync(russianAddress))
                .ReturnsAsync(searchResponse);

            // Act
            var result = await _validationService.ValidateAddressAsync(russianAddress);

            // Assert
            result.Should().NotBeNull();
            result.IsValid.Should().BeTrue();
            result.ConfidencePercentage.Should().BeGreaterOrEqualTo(80);
            result.MatchedAddress.Should().NotBeNull();
            result.MatchedAddress!.CountryCode.Should().Be("RU");
        }

        private AddressSearchResponse CreateMockResponse(
            string freeformAddress, 
            double score, 
            Position position,
            string country,
            string countryCode)
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
                            Country = country,
                            CountryCode = countryCode
                        }
                    }
                }
            };
        }
    }
}