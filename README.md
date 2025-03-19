# AddressValidator

> **Note:** This project is currently in early development and is not yet in a runnable state.

## Project Overview

A console application that validates addresses using Azure Maps and provides detailed validation information. The application will validate user-entered addresses to determine:

- If the address exists according to Azure Maps
- The confidence level of the match
- Detailed address information from the API
- Save both the input address and the API response

## Development Progress

- ✅ Created Azure Maps service integration
- ✅ Implemented data models for the API responses
- ✅ Added custom exception handling
- ⏳ Address validation service - In progress
- ⏳ Storage functionality - Planned
- ⏳ Console UI - Planned
- ⏳ Test cases - Planned

## Technologies Used

- C# / .NET 9.0
- Azure Maps Search API
- Spectre.Console (for enhanced console UI)

## Project Structure

- **Models/**: Contains data models for the Azure Maps API responses
- **Services/**: Contains the core services 
  - `IAzureMapsService` / `AzureMapsService`: Interface and implementation for Azure Maps API communication
  - `AzureMapsServiceException`: Custom exception handling for the service
  - More services coming soon...
- **UI/**: Will contain the console user interface (coming soon)

## Project Goals

This application aims to:

- Validate addresses using the Azure Maps service
- Determine the confidence level of address matches
- Store both user input and validation results
- Provide a simple and intuitive console interface
- Include test cases for both valid and invalid addresses

The initial version is a console application, with potential for future expansion into other interfaces.
