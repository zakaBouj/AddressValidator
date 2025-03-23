using AddressValidator.Console.Config;
using AddressValidator.Console.Models;
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
services.AddSingleton<IAddressValidationService, AddressValidationService>(sp =>
    new AddressValidationService(
        sp.GetRequiredService<IAzureMapsService>(),
        0.8 // 80% confidence threshold
    )
);

// Build service provider
var serviceProvider = services.BuildServiceProvider();

// Get service instance
var validationService = serviceProvider.GetRequiredService<IAddressValidationService>();

// Address input interface
Console.WriteLine("===== Address Validator =====");
Console.WriteLine("Enter address details (* indicates required field)");

Console.Write("Address Line 1*: ");
var addressLine1 = Console.ReadLine() ?? string.Empty;

Console.Write("Address Line 2: ");
var addressLine2 = Console.ReadLine();

Console.Write("Address Line 3: ");
var addressLine3 = Console.ReadLine();

Console.Write("Postal Code/ZIP*: ");
var postalCode = Console.ReadLine() ?? string.Empty;

Console.Write("City*: ");
var city = Console.ReadLine() ?? string.Empty;

Console.Write("Country*: ");
var country = Console.ReadLine() ?? string.Empty;

// Create address input model
var addressInput = new AddressInput
{
    AddressLine1 = addressLine1,
    AddressLine2 = addressLine2,
    AddressLine3 = addressLine3,
    PostalCode = postalCode,
    City = city,
    Country = country
};

// Basic validation
if (string.IsNullOrWhiteSpace(addressLine1) ||
    string.IsNullOrWhiteSpace(postalCode) ||
    string.IsNullOrWhiteSpace(city) ||
    string.IsNullOrWhiteSpace(country))
{
    Console.WriteLine("Error: Required fields cannot be empty.");
}
else
{
    try
    {
        Console.WriteLine("\nValidating address...");
        var result = await validationService.ValidateAddressAsync(addressInput);
        
        Console.WriteLine("\n===== Validation Results =====");
        Console.WriteLine($"Valid: {(result.IsValid ? "Yes" : "No")}");
        Console.WriteLine($"Confidence: {result.ConfidencePercentage}%");
        
        if (result.FreeformAddress != null)
        {
            Console.WriteLine($"Formatted Address: {result.FreeformAddress}");
        }
        
        if (result.Position != null)
        {
            Console.WriteLine($"Coordinates: {result.Position.Lat}, {result.Position.Lon}");
        }
        
        Console.WriteLine($"Message: {result.ValidationMessage}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error validating address: {ex.Message}");
    }
}

Console.WriteLine("\nPress any key to exit...");
Console.ReadKey();