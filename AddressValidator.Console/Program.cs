using AddressValidator.Console.Config;
using AddressValidator.Console.Services;
using Microsoft.Extensions.DependencyInjection;

// Create service collection
var services = new ServiceCollection();

// Configure Azure Maps settings
var azureMapsConfig = new AzureMapsConfig
{
    ClientId = "bbcef7ee-c081-4417-b87d-c591a9a65953",
    AzureMapsEndpoint = "https://atlas.microsoft.com/"
};

// Register services
services.AddSingleton(azureMapsConfig);
services.AddHttpClient<IAzureMapsService, AzureMapsService>();
services.AddSingleton<IAzureMapsTokenService, AzureMapsTokenService>();

// Build service provider
var serviceProvider = services.BuildServiceProvider();

// Get service instance
var azureMapsService = serviceProvider.GetRequiredService<IAzureMapsService>();

// Example usage
Console.WriteLine("Enter an address to search:");
var address = Console.ReadLine();

if (!string.IsNullOrWhiteSpace(address))
{
    try
    {
        var result = await azureMapsService.SearchAddressAsync(address);
        
        Console.WriteLine($"Found {result.Results.Length} results");
        foreach (var item in result.Results)
        {
            Console.WriteLine($"Address: {item.Address?.FreeformAddress}");
            Console.WriteLine($"Confidence: {item.Score}");
            Console.WriteLine($"Position: {item.Position.Lat}, {item.Position.Lon}");
            Console.WriteLine();
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error validating address: {ex.Message}");
    }
}

Console.WriteLine("Press any key to exit...");
Console.ReadKey();