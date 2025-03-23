using System;
using System.IO;
using System.Threading.Tasks;
using AddressValidator.Console.Config;
using AddressValidator.Console.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Xunit;

namespace AddressValidator.Test
{
    public class AzureAuthenticationTests
    {
        [Fact]
        [Trait("Category", "AzureIntegration")]
        public async Task GetAccessToken_WithValidCredentials_ReturnsToken()
        {
            // Skip this test in local development if environment variable is not set
            var configPath = Environment.GetEnvironmentVariable("AZURE_MAPS_CONFIG_PATH");
            if (string.IsNullOrEmpty(configPath))
            {
                // Skip test if running locally without credentials
                // This test is primarily meant to run in the GitHub Actions pipeline
                Assert.True(true, "Test skipped - no Azure configuration");
                return;
            }

            // Load configuration from the file specified in environment variable
            var configuration = new ConfigurationBuilder()
                .AddJsonFile(configPath)
                .Build();

            var azureMapsConfig = new AzureMapsConfig
            {
                ClientId = configuration["ClientId"] ?? throw new InvalidOperationException("ClientId is missing from config"),
                AzureMapsEndpoint = configuration["AzureMapsEndpoint"] ?? "https://atlas.microsoft.com/"
            };

            // Create the token service
            var tokenService = new AzureMapsTokenService(azureMapsConfig);

            // Act - attempt to get a token
            var token = await tokenService.GetAccessTokenAsync();

            // Assert - we got a non-empty token
            Assert.NotNull(token);
            Assert.NotEmpty(token);
            
            // Optional: Print token length for debugging (don't print the actual token)
            System.Console.WriteLine($"Token successfully retrieved. Length: {token.Length} characters");
        }
    }
}