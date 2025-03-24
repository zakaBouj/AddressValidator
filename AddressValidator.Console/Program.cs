using AddressValidator.Console.Config;
using AddressValidator.Console.Models;
using AddressValidator.Console.Repositories;
using AddressValidator.Console.Services;
using AddressValidator.Console.UI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

// Setup configuration
var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .Build();

// Create service collection
var services = new ServiceCollection();

// Configure Azure Maps settings
var azureMapsConfig = new AzureMapsConfig
{
    ClientId = configuration.GetSection("AzureMaps:ClientId").Value
        ?? throw new InvalidOperationException("Azure Maps Client ID is not configured in appsettings.json"),
    AzureMapsEndpoint = configuration.GetSection("AzureMaps:Endpoint").Value
        ?? "https://atlas.microsoft.com/"
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
services.AddSingleton<ConsoleUIService>();

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
var uiService = serviceProvider.GetRequiredService<ConsoleUIService>();

// Initialize UI
uiService.Initialize();

// Main application loop
bool exitApplication = false;

while (!exitApplication)
{
    var option = uiService.ShowMainMenu();

    switch (option)
    {
        case "1. Validate new address":
            await ValidateNewAddressAsync();
            break;
        case "2. View validation history":
            await ViewValidationHistoryAsync();
            break;
        case "3. Re-validate address from history":
            await RevalidateFromHistoryAsync();
            break;
        case "4. Clear validation history":
            await ClearValidationHistoryAsync();
            break;
        case "5. Exit application":
            exitApplication = true;
            uiService.ShowGoodbye();
            break;
        default:
            // Show error for invalid selection instead of goodbye message
            uiService.ShowError("Invalid option. Please try again.");
            break;
    }

    if (!exitApplication)
    {
        uiService.PressAnyKeyToContinue();
    }
}

// Method to validate a new address
async Task ValidateNewAddressAsync()
{
    try
    {
        // Use the new AddressFormUI to collect address input with validation
        var addressInput = AddressFormUI.CollectAddressInput();
        
        // Check if user canceled the operation
        if (addressInput == null)
        {
            return; // Return to main menu
        }
        
        // Show a spinner while validating
        var result = await uiService.ShowSpinnerAsync(
            "Validating address with Azure Maps...",
            async () => {
                // Convert to single line for original query tracking
                var singleLineAddress = addressInput.ToSingleLineString();
                
                // Call the validation service
                var validationResult = await validationService.ValidateAddressAsync(addressInput);
                
                // Save validation result to repository
                await repository.SaveValidationResultAsync(singleLineAddress, addressInput, validationResult);
                
                return validationResult;
            });
        
        // Display results using the enhanced UI, including original user input
        AddressFormUI.DisplayValidationResult(result, addressInput);
    }
    catch (Exception ex)
    {
        uiService.ShowError($"Error validating address: {ex.Message}");
    }
}

// Method to view validation history
async Task ViewValidationHistoryAsync()
{
    try
    {
        // Get validation history with a loading spinner
        var validationHistory = await uiService.ShowSpinnerAsync(
            "Loading validation history...",
            async () => await repository.GetValidationHistoryAsync()
        );
        
        // Display formatted history using ValidationHistoryUI
        ValidationHistoryUI.DisplayValidationHistory(validationHistory);
    }
    catch (Exception ex)
    {
        uiService.ShowError($"Error retrieving validation history: {ex.Message}");
    }
}

// Method to re-validate an address from history
async Task RevalidateFromHistoryAsync()
{
    try
    {
        // Get validation history with a loading spinner
        var validationHistory = await uiService.ShowSpinnerAsync(
            "Loading validation history...",
            async () => await repository.GetValidationHistoryAsync()
        );
        
        // Let the user select a record to re-validate
        var selectedRecord = ValidationHistoryUI.SelectRecordFromHistory(validationHistory);
        
        // If the user selected a record, re-validate it
        if (selectedRecord != null)
        {
            // Show a spinner while re-validating
            var result = await uiService.ShowSpinnerAsync(
                "Re-validating address with Azure Maps...",
                async () => {
                    // Use original input if available, otherwise use the query string
                    AddressValidationResult validationResult;
                    
                    if (selectedRecord.OriginalAddressInput != null)
                    {
                        validationResult = await validationService.ValidateAddressAsync(
                            selectedRecord.OriginalAddressInput);
                            
                        // Save the new validation result
                        await repository.SaveValidationResultAsync(
                            selectedRecord.OriginalQuery,
                            selectedRecord.OriginalAddressInput,
                            validationResult);
                    }
                    else
                    {
                        validationResult = await validationService.ValidateAddressAsync(
                            selectedRecord.OriginalQuery);
                            
                        // Save the new validation result
                        await repository.SaveValidationResultAsync(
                            selectedRecord.OriginalQuery,
                            null,
                            validationResult);
                    }
                    
                    return validationResult;
                });
                
            // Display the results using the enhanced UI
            // Check if we have original address input to display
            if (selectedRecord.OriginalAddressInput != null)
            {
                AddressFormUI.DisplayValidationResult(result, selectedRecord.OriginalAddressInput);
            }
            else
            {
                AddressFormUI.DisplayValidationResult(result);
            }
        }
    }
    catch (Exception ex)
    {
        uiService.ShowError($"Error re-validating address: {ex.Message}");
    }
}

// Method to clear validation history
async Task ClearValidationHistoryAsync()
{
    try
    {
        // Ask for confirmation using ValidationHistoryUI
        bool confirmClear = ValidationHistoryUI.ConfirmClearHistory();
        
        if (confirmClear)
        {
            // Show a spinner while clearing history
            var result = await uiService.ShowSpinnerAsync(
                "Clearing validation history...",
                async () => await repository.ClearHistoryAsync()
            );
            
            // Show appropriate message
            if (result)
            {
                ValidationHistoryUI.ShowClearSuccessMessage();
            }
            else
            {
                ValidationHistoryUI.ShowNoHistoryToClear();
            }
        }
    }
    catch (Exception ex)
    {
        uiService.ShowError($"Error clearing validation history: {ex.Message}");
    }
}