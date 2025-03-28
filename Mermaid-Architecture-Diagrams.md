# AddressValidator Architecture Diagrams (Mermaid)

GitHub natively renders these Mermaid diagrams when included in markdown files. Simply include them in your README.md or documentation files.

## 1. Component Architecture

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

## 2. Class Diagram

```mermaid
classDiagram
    class AddressInput {
        +string AddressLine1
        +string? AddressLine2
        +string? AddressLine3
        +string PostalCode
        +string City
        +string Country
        +ToSingleLineString()
    }
    
    class AddressValidationResult {
        +bool IsValid
        +double ConfidencePercentage
        +string ValidationMessage
        +string? FreeformAddress
        +Address? MatchedAddress
        +Position? Position
        +AddressInput? OriginalInput
    }
    
    class IAddressValidationService {
        <<interface>>
        +ValidateAddressAsync(AddressInput) Task~AddressValidationResult~
        +ValidateAddressAsync(string) Task~AddressValidationResult~
    }
    
    class AddressValidationService {
        -IAzureMapsService _azureMapsService
        -double _minimumConfidenceThreshold
        +ValidateAddressAsync(AddressInput) Task~AddressValidationResult~
        +ValidateAddressAsync(string) Task~AddressValidationResult~
    }
    
    class IAzureMapsService {
        <<interface>>
        +SearchAddressAsync(string) Task~AddressSearchResponse~
    }
    
    class AzureMapsService {
        -HttpClient _httpClient
        -IAzureMapsTokenService _tokenService
        -AzureMapsConfig _config
        +SearchAddressAsync(string) Task~AddressSearchResponse~
    }
    
    class IAzureMapsTokenService {
        <<interface>>
        +GetAccessTokenAsync() Task~string~
    }
    
    class AzureMapsTokenService {
        -AzureMapsConfig _config
        -HttpClient _httpClient
        +GetAccessTokenAsync() Task~string~
    }
    
    class IAddressValidationRepository {
        <<interface>>
        +SaveValidationResultAsync(string, AddressInput?, AddressValidationResult) Task
        +GetValidationHistoryAsync() Task~List~ValidationRecord~~
        +ClearHistoryAsync() Task~bool~
    }
    
    class JsonAddressValidationRepository {
        -string _filePath
        -int _maxHistorySize
        +SaveValidationResultAsync(string, AddressInput?, AddressValidationResult) Task
        +GetValidationHistoryAsync() Task~List~ValidationRecord~~
        +ClearHistoryAsync() Task~bool~
    }
    
    AddressInput <-- AddressValidationResult
    IAddressValidationService <|.. AddressValidationService
    AddressValidationService --> IAzureMapsService
    IAzureMapsService <|.. AzureMapsService
    AzureMapsService --> IAzureMapsTokenService
    IAzureMapsTokenService <|.. AzureMapsTokenService
    AddressValidationService --> IAddressValidationRepository
    IAddressValidationRepository <|.. JsonAddressValidationRepository
```

## 3. Sequence Diagram - Address Validation

```mermaid
sequenceDiagram
    participant User
    participant AddressFormUI
    participant ValidationService as AddressValidationService
    participant AzureMapsService
    participant Repository
    
    User->>AddressFormUI: Enter address details
    AddressFormUI->>ValidationService: ValidateAddressAsync(addressInput)
    ValidationService->>AzureMapsService: SearchAddressAsync(address)
    AzureMapsService->>AzureMapsService: GetAccessTokenAsync()
    AzureMapsService->>External: HTTP Request to Azure Maps API
    External->>AzureMapsService: JSON Response
    AzureMapsService->>ValidationService: AddressSearchResponse
    ValidationService->>ValidationService: Evaluate confidence score
    ValidationService->>Repository: SaveValidationResultAsync()
    ValidationService->>AddressFormUI: AddressValidationResult
    AddressFormUI->>User: Display validation result
```

## 4. Infrastructure Diagram

```mermaid
flowchart TD
    subgraph Client["Client Machine"]
        App[AddressValidator.Console]
        Config[appsettings.json]
        Storage[JSON Storage]
    end
    
    subgraph Azure["Azure Cloud"]
        AzureMaps[Azure Maps Service]
        EntraID[Entra ID]
    end
    
    App -->|Reads| Config
    App -->|Writes to| Storage
    App -->|HTTPS Requests| AzureMaps
    App -->|Authentication| EntraID
    EntraID -->|Token| App
    AzureMaps -->|Validates Addresses| App
```

## 5. Data Flow Diagram

```mermaid
flowchart LR
    User([User]) -->|1. Enters address| UI[Address Form UI]
    UI -->|2. Submit for validation| Service[AddressValidation Service]
    Service -->|3. Request validation| Azure[Azure Maps API]
    Azure -->|4. Return matches| Service
    Service -->|5. Store result| Storage[(JSON Storage)]
    Service -->|6. Return validation result| UI
    UI -->|7. Display results| User
```

## 6. Main Menu Navigation Flow

```mermaid
stateDiagram-v2
    [*] --> MainMenu
    
    MainMenu --> ValidateAddress: Option 1
    MainMenu --> ViewHistory: Option 2
    MainMenu --> RevalidateAddress: Option 3
    MainMenu --> ClearHistory: Option 4
    MainMenu --> Exit: Option 5
    
    ValidateAddress --> CollectAddress
    CollectAddress --> ReviewAddress
    ReviewAddress --> SubmitAddress
    SubmitAddress --> DisplayResults
    DisplayResults --> MainMenu
    
    ViewHistory --> DisplayHistory
    DisplayHistory --> MainMenu
    
    RevalidateAddress --> SelectHistoryRecord
    SelectHistoryRecord --> SubmitRevalidation
    SubmitRevalidation --> DisplayResults
    
    ClearHistory --> ConfirmClear
    ConfirmClear --> ClearConfirmed
    ClearConfirmed --> MainMenu
    ConfirmClear --> MainMenu: Cancel
    
    Exit --> [*]
```

## How to Use in GitHub Markdown

Simply copy and paste these code blocks (including the triple backticks and "mermaid") into your GitHub markdown files. GitHub will automatically render them as diagrams.

Example usage in README.md:

````markdown
## Application Architecture

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