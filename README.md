# AddressValidator

A console application that validates addresses using Azure Maps with Entra ID authentication and provides detailed validation information.

## Project Overview

The application validates user-entered addresses to determine:

- If the address exists according to Azure Maps
- The confidence level of the match
- Detailed address information from the API
- Save both the input address and the API response

<!-- Demo GIF placeholder - will be visible when uncommenting -->
<!--
## Demo

![Address Validator Demo](demo/address-validator-demo.gif)
*Quick demonstration of the address validation workflow*
-->

## Getting Started

### Prerequisites
- .NET 9.0 SDK
- Azure account with Maps service and Entra ID set up

### Configuration
The application uses appsettings.json for configuration. For security, this file is not included in source control.

1. Copy the example configuration:
```bash
cp AddressValidator.Console/appsettings.example.json AddressValidator.Console/appsettings.json
```

2. Edit appsettings.json with your Azure Maps credentials:
```json
{
  "AzureMaps": {
    "ClientId": "YOUR_CLIENT_ID_HERE",
    "Endpoint": "https://atlas.microsoft.com/"
  }
}
```

3. Replace `YOUR_CLIENT_ID_HERE` with your Azure Entra ID client ID.

Note: Never commit appsettings.json to source control as it contains sensitive credentials.

### Running the Application
From the solution directory:
```bash
# Run from solution directory, specifying the project
dotnet run --project AddressValidator.Console/AddressValidator.Console.csproj

# OR navigate to the project directory and run
cd AddressValidator.Console
dotnet run
```

### Running Tests
```bash
# Run all tests from solution directory
dotnet test

# Run tests with filter
dotnet test --filter "FullyQualifiedName~EdgeCases"
dotnet test --filter "Category!=AzureIntegration"
```

## Development Workflow

This project uses GitHub Actions for continuous integration and deployment:

**Build and Test**: Automatically runs on every push and pull request
- Builds the solution
- Runs all unit and integration tests
- Reports test results

## Development Progress

- ✅ Created Azure Maps service integration with Entra ID authentication
- ✅ Implemented data models for the API responses
- ✅ Added custom exception handling
- ✅ Implemented address validation service with confidence threshold
- ✅ Enhanced validation to handle multiple potential address matches
- ✅ Added comprehensive test suite with 55 tests
- ✅ Set up GitHub Actions for CI/CD
- ✅ Implemented storage functionality with JSON repository
- ✅ Added basic console UI with menu system
- ✅ Enhanced Console UI with Spectre.Console
- ✅ Implemented improved address input form with review capabilities
- ✅ Added validation history view and management

## Features

### Address Validation
- Validates addresses against Azure Maps API
- Configurable confidence threshold (currently 80%)
- Detailed validation results including coordinates and formatted addresses

### Enhanced User Interface
- Rich, colorful console UI powered by Spectre.Console
- Intuitive navigation with clear menus and prompts
- Interactive address form with field validation
- Address review screen with editable fields
- Bold formatting for user input for better visibility
- Comprehensive error handling with user-friendly messages

### Storage and History
- Persistent storage of validation results
- View validation history with chronological ordering
- Re-validate addresses from history
- Clear history when needed
- Sample data included for testing

## Technologies Used

- C# / .NET 9.0
- Azure Maps Search API with Entra ID authentication
- System.Text.Json for data serialization
- Spectre.Console for enhanced terminal UI
- xUnit, Moq, and FluentAssertions for testing
- GitHub Actions for CI/CD

## Project Structure

- **Models/**: Contains data models for the Azure Maps API responses and application models
  - `AddressInput`: Structured model for address input
  - `AddressValidationResult`: Model representing validation results
  - `ValidationRecord`: Record for storing validation history
  - `AddressSearchResponse`/`AddressSearchResult`: Models for Azure Maps API responses
- **Services/**: Contains the core services 
  - `IAzureMapsService` / `AzureMapsService`: Interface and implementation for Azure Maps API communication
  - `IAzureMapsTokenService` / `AzureMapsTokenService`: Handles Entra ID token acquisition
  - `IAddressValidationService` / `AddressValidationService`: Validates addresses using configurable thresholds
  - `AzureMapsServiceException`: Custom exception handling for the service
- **Repositories/**: Contains storage implementations
  - `IAddressValidationRepository`: Interface for storing validation results
  - `JsonAddressValidationRepository`: JSON file-based implementation
- **UI/**: Contains user interface implementations
  - `AddressFormUI`: Handles address input form and validation results display
  - `ValidationHistoryUI`: Manages display and interaction with validation history
  - `ConsoleUIService`: Core UI service with spinners and loading indicators
- **Data/**: Contains sample data for the application
  - `sample-validation-history.json`: Sample address validation records
- **Tests/**: Comprehensive test suite organized by type
  - **Models/**: Tests for address input models and validation
  - **Services/**: Tests for service implementations and mocking
  - **Repositories/**: Tests for storage implementations
  - **UI/**: Tests for user interface components and formatting logic
  - **Integration/**: End-to-end tests including file system operations
  - **EdgeCases/**: Tests for special scenarios:
    - Special character handling
    - International address formats
    - Non-Latin characters
    - Error conditions

## Project Goals

This application aims to:

- Validate addresses using the Azure Maps service
- Determine the confidence level of address matches
- Store both user input and validation results in a simple way
- Provide a simple and intuitive console interface
- Maintain high code quality with comprehensive tests

The initial version is a console application, with potential for future expansion into other interfaces.
