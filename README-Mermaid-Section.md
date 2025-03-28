## Architecture Overview

AddressValidator follows a clean, layered architecture designed for maintainability and testability:

```mermaid
flowchart TD
    subgraph UI["UI Layer"]
        A[AddressFormUI] 
        B[ConsoleUIService]
        C[ValidationHistoryUI]
    end
    
    subgraph Services["Service Layer"]
        D[AddressValidationService]
        E[AzureMapsService]
        F[AzureMapsTokenService]
    end
    
    subgraph Data["Data Layer"]
        G[JsonAddressValidationRepository]
    end
    
    subgraph External["External Services"]
        H[Azure Maps API]
        I[Entra ID]
    end
    
    A --> B
    C --> B
    D --> E
    E --> F
    E --> H
    F --> I
    D --> G
    
    Program --> UI
    Program --> Services
    Program --> Data
```

### Address Validation Process

```mermaid
sequenceDiagram
    participant User
    participant UI as AddressFormUI
    participant Service as AddressValidationService
    participant Maps as AzureMapsService
    participant Storage as Repository
    
    User->>UI: Enter address details
    UI->>Service: ValidateAddressAsync(address)
    Service->>Maps: SearchAddressAsync(address)
    Maps->>Maps: GetAccessTokenAsync()
    Maps->>Azure: HTTP Request
    Azure->>Maps: JSON Response
    Maps->>Service: AddressSearchResponse
    Service->>Service: Evaluate confidence
    Service->>Storage: SaveValidationResultAsync()
    Service->>UI: ValidationResult
    UI->>User: Display results
```

### Key Architecture Principles

- **Clean Layering**: UI, services, and data access are clearly separated
- **Dependency Injection**: All services are registered with the DI container
- **Interface-Based Design**: Components depend on abstractions, not implementations
- **Single Responsibility**: Each class has a focused purpose and clear responsibilities
- **Comprehensive Error Handling**: Structured exception handling throughout all layers

For complete architecture documentation with additional diagrams, see [Mermaid Architecture Diagrams](Mermaid-Architecture-Diagrams.md).