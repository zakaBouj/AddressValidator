# AddressValidator

A console application that validates addresses using Azure Maps with Entra ID authentication and provides detailed validation information.

## Project Overview

The application validates user-entered addresses to determine:

- If the address exists according to Azure Maps
- The confidence level of the match
- Detailed address information from the API
- Save both the input address and the API response

## Development Progress

- ✅ Created Azure Maps service integration with Entra ID authentication
- ✅ Implemented data models for the API responses
- ✅ Added custom exception handling
- ✅ Implemented address validation service with confidence threshold
- ✅ Enhanced validation to handle multiple potential address matches
- ✅ Added comprehensive test suite with 21 tests
- ✅ Set up GitHub Actions for CI/CD
- ⏳ Storage functionality - Planned
- ⏳ Console UI - Planned

## Technologies Used

- C# / .NET 9.0
- Azure Maps Search API with Entra ID authentication
- xUnit, Moq, and FluentAssertions for testing
- GitHub Actions for CI/CD
- Spectre.Console (coming soon for enhanced console UI)

## Project Structure

- **Models/**: Contains data models for the Azure Maps API responses and application models
  - `AddressInput`: Structured model for address input
  - `AddressValidationResult`: Model representing validation results
  - `AddressSearchResponse`/`AddressSearchResult`: Models for Azure Maps API responses
- **Services/**: Contains the core services 
  - `IAzureMapsService` / `AzureMapsService`: Interface and implementation for Azure Maps API communication
  - `IAzureMapsTokenService` / `AzureMapsTokenService`: Handles Entra ID token acquisition
  - `IAddressValidationService` / `AddressValidationService`: Validates addresses using configurable thresholds
  - `AzureMapsServiceException`: Custom exception handling for the service
- **Tests/**: Comprehensive test suite
  - Unit tests for individual components
  - Integration tests for the complete validation flow
  - Edge case tests for error handling
  - Tests for multiple address results handling
- **CI/CD**: GitHub Actions workflows for automated building and testing

## Project Goals

This application aims to:

- Validate addresses using the Azure Maps service
- Determine the confidence level of address matches
- Store both user input and validation results
- Provide a simple and intuitive console interface
- Maintain high code quality with comprehensive tests

The initial version is a console application, with potential for future expansion into other interfaces.

## Next Steps

Potential enhancements for future development:

1. **Storage Implementation**: Create a repository to save validation results
2. **Console UI Enhancement**: Implement Spectre.Console for a better user experience
3. **Address Suggestions**: Provide alternative address suggestions when matches aren't exact
4. **Batch Processing**: Add support for validating multiple addresses at once
5. **Geocoding Features**: Expand functionality to include reverse geocoding
6. **Web API Version**: Create a REST API version of the service
