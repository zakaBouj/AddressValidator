using AddressValidator.Console.Config;
using AddressValidator.Console.Models;
using AddressValidator.Console.Repositories;
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

// Set up repository with sample data if needed
string dataDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
string historyFilePath = Path.Combine(dataDirectory, "validation-history.json");
string sampleFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "sample-validation-history.json");

// Make sure the data directory exists
Directory.CreateDirectory(dataDirectory);

// If history file doesn't exist and sample file exists, copy the sample file
if (!File.Exists(historyFilePath) && File.Exists(sampleFilePath))
{
    try
    {
        File.Copy(sampleFilePath, historyFilePath);
        Console.WriteLine("Sample data loaded successfully.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Note: Could not load sample data: {ex.Message}");
    }
}

services.AddSingleton<IAddressValidationRepository>(sp =>
    new JsonAdressValidationRepository(
        historyFilePath,
        maxHistroySize: 20
    )
);

// Build service provider
var serviceProvider = services.BuildServiceProvider();

// Get service instances
var validationService = serviceProvider.GetRequiredService<IAddressValidationService>();
var repository = serviceProvider.GetRequiredService<IAddressValidationRepository>();

// Main application loop
bool exitApplication = false;

while (!exitApplication)
{
    Console.Clear();
    Console.WriteLine("===== Address Validator =====");
    Console.WriteLine();
    Console.WriteLine("1. Validate new address");
    Console.WriteLine("2. View validation history");
    Console.WriteLine("3. Re-validate address from history");
    Console.WriteLine("4. Clear validation history");
    Console.WriteLine("5. Exit");
    Console.WriteLine();
    Console.Write("Select an option (1-5): ");

    var option = Console.ReadLine()?.Trim();

    Console.Clear();

    switch (option)
    {
        case "1":
            await ValidateNewAddressAsync();
            break;
        case "2":
            await ViewValidationHistoryAsync();
            break;
        case "3":
            await RevalidateFromHistoryAsync();
            break;
        case "4":
            await ClearValidationHistoryAsync();
            break;
        case "5":
            exitApplication = true;
            Console.WriteLine("Exiting application. Goodbye!");
            break;
        default:
            Console.WriteLine("Invalid option. Please try again.");
            break;
    }

    if (!exitApplication)
    {
        Console.WriteLine("\nPress any key to return to menu...");
        Console.ReadKey();
    }
}

// Method to validate a new address
async Task ValidateNewAddressAsync()
{
    Console.WriteLine("===== Validate New Address =====");
    Console.WriteLine("Enter address details (* indicates required field)");
    Console.WriteLine();

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
        return;
    }

    try
    {
        Console.WriteLine("\nValidating address...");
        
        // Convert to single line for original query tracking
        var singleLineAddress = addressInput.ToSingleLineString();
        
        var result = await validationService.ValidateAddressAsync(addressInput);
        
        // Save validation result to repository
        await repository.SaveValidationResultAsync(singleLineAddress, addressInput, result);
        
        // Display results
        DisplayValidationResult(result);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error validating address: {ex.Message}");
    }
}

// Method to view validation history
async Task ViewValidationHistoryAsync()
{
    Console.WriteLine("===== Validation History =====");
    
    try
    {
        var history = await repository.GetValidationHistoryAsync();
        var records = history.ToList();
        
        if (records.Count == 0)
        {
            Console.WriteLine("No validation history found.");
            return;
        }
        
        Console.WriteLine($"Found {records.Count} records (showing most recent first):");
        Console.WriteLine();
        
        for (int i = 0; i < records.Count; i++)
        {
            var record = records[i];
            Console.WriteLine($"[{i + 1}] {record.Timestamp.ToLocalTime():yyyy-MM-dd HH:mm:ss}");
            Console.WriteLine($"    Query: {record.OriginalQuery}");
            Console.WriteLine($"    Result: {(record.ValidationResult.IsValid ? "VALID" : "INVALID")} ({record.ValidationResult.ConfidencePercentage}%)");
            if (!string.IsNullOrEmpty(record.ValidationResult.FreeformAddress))
            {
                Console.WriteLine($"    Address: {record.ValidationResult.FreeformAddress}");
            }
            Console.WriteLine();
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error retrieving validation history: {ex.Message}");
    }
}

// Method to re-validate an address from history
async Task RevalidateFromHistoryAsync()
{
    Console.WriteLine("===== Re-validate Address from History =====");
    
    try
    {
        var history = await repository.GetValidationHistoryAsync();
        var records = history.ToList();
        
        if (records.Count == 0)
        {
            Console.WriteLine("No validation history found.");
            return;
        }
        
        // Display numbered list of records
        Console.WriteLine("Select an address to re-validate:");
        Console.WriteLine();
        
        for (int i = 0; i < records.Count; i++)
        {
            var record = records[i];
            Console.WriteLine($"[{i + 1}] {record.OriginalQuery}");
        }
        
        Console.WriteLine();
        Console.Write("Enter number (or 0 to cancel): ");
        
        if (!int.TryParse(Console.ReadLine(), out int selection) || selection < 0 || selection > records.Count)
        {
            Console.WriteLine("Invalid selection.");
            return;
        }
        
        if (selection == 0)
        {
            return;
        }
        
        // Get the selected record (adjust for 0-based index)
        var selectedRecord = records[selection - 1];
        
        Console.WriteLine("\nRe-validating address...");
        
        // Use original input if available, otherwise use the query string
        if (selectedRecord.OriginalAddressInput != null)
        {
            var result = await validationService.ValidateAddressAsync(selectedRecord.OriginalAddressInput);
            
            // Save the new validation result
            await repository.SaveValidationResultAsync(
                selectedRecord.OriginalQuery,
                selectedRecord.OriginalAddressInput,
                result);
                
            // Display results
            DisplayValidationResult(result);
        }
        else
        {
            var result = await validationService.ValidateAddressAsync(selectedRecord.OriginalQuery);
            
            // Save the new validation result
            await repository.SaveValidationResultAsync(
                selectedRecord.OriginalQuery,
                null,
                result);
                
            // Display results
            DisplayValidationResult(result);
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error re-validating address: {ex.Message}");
    }
}

// Method to clear validation history
async Task ClearValidationHistoryAsync()
{
    Console.WriteLine("===== Clear Validation History =====");
    Console.WriteLine();
    Console.WriteLine("Are you sure you want to clear all validation history?");
    Console.Write("Type 'yes' to confirm: ");
    
    var confirmation = Console.ReadLine()?.Trim().ToLower();
    
    if (confirmation == "yes")
    {
        try
        {
            var result = await repository.ClearHistoryAsync();
            
            if (result)
            {
                Console.WriteLine("Validation history cleared successfully.");
            }
            else
            {
                Console.WriteLine("No history found to clear.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error clearing validation history: {ex.Message}");
        }
    }
    else
    {
        Console.WriteLine("Operation cancelled.");
    }
}

// Helper method to display validation results
void DisplayValidationResult(AddressValidationResult result)
{
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