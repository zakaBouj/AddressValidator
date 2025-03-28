## Architecture

AddressValidator uses a clean, layered architecture with Azure Maps integration:

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

The application follows clean architecture principles with:
- UI components built with Spectre.Console for rich terminal experiences
- Service layer handling address validation logic and API integration
- Data layer managing validation history persistence
- External integration with Azure Maps secured via Entra ID