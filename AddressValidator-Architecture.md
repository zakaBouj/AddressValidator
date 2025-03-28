# AddressValidator Architecture Diagrams

## 1. High-Level Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                    AddressValidator Console App              │
└───────────────────────────────┬─────────────────────────────┘
                                │
                                ▼
┌─────────────────────────────────────────────────────────────┐
│                           UI Layer                           │
│                                                             │
│  ┌───────────────┐   ┌───────────────┐   ┌───────────────┐  │
│  │ AddressFormUI │   │ConsoleUIService│   │ValidationHist.│  │
│  └───────┬───────┘   └───────┬───────┘   └───────┬───────┘  │
└──────────┼───────────────────┼───────────────────┼──────────┘
           │                   │                   │
           ▼                   ▼                   ▼
┌─────────────────────────────────────────────────────────────┐
│                        Service Layer                         │
│                                                             │
│  ┌───────────────┐   ┌───────────────┐   ┌───────────────┐  │
│  │AddressValid.  │──▶│  AzureMaps    │◀──┤ AzureMapsToken│  │
│  │Service        │   │  Service      │   │ Service       │  │
│  └───────┬───────┘   └───────┬───────┘   └───────────────┘  │
└──────────┼───────────────────┼───────────────────────────────┘
           │                   │
           ▼                   ▼
┌─────────────────────┐    ┌─────────────────────┐
│  Repository Layer   │    │  External Services  │
│                     │    │                     │
│  ┌───────────────┐  │    │  ┌───────────────┐  │
│  │JsonAddressVal.│  │    │  │  Azure Maps   │  │
│  │Repository     │  │    │  │     API       │  │
│  └───────────────┘  │    │  └───────────────┘  │
└─────────────────────┘    └─────────────────────┘
```

## 2. Component Diagram with Dependencies

```
                       ┌───────────────────┐
                       │   Program.cs      │
                       │ (Entry Point)     │
                       └─────────┬─────────┘
                                 │
                                 │ creates/configures
                                 ▼
┌────────────────┐     ┌─────────────────────┐     ┌──────────────────┐
│                │     │                     │     │                  │
│  ConsoleUI     │◀────│  DI Container      │────▶│ Repository       │
│  Service       │     │  (ServiceProvider) │     │ (JSON Storage)   │
│                │     │                    │     │                  │
└────────┬───────┘     └──────────┬─────────┘     └──────────────────┘
         │                        │
         │                        │ creates
         │                        ▼
         │              ┌─────────────────────┐
         │              │AddressValidation    │
         │              │Service              │
         │              └─────────┬───────────┘
         │                        │
         │                        │ uses
         │                        ▼
         │              ┌─────────────────────┐     ┌──────────────────┐
         │              │                     │     │                  │
         │              │  AzureMapsService   │────▶│ AzureMapsToken   │
         │              │                     │     │ Service          │
         │              └─────────┬───────────┘     │                  │
         │                        │                 └──────────────────┘
         │                        │ calls
         │                        ▼
         │              ┌─────────────────────┐
         │              │   Azure Maps API    │
         │              │  (External Service) │
         │              └─────────────────────┘
         │
         │ used by
         ▼
┌────────────────┐     ┌─────────────────────┐
│                │     │                     │
│  AddressFormUI │     │ ValidationHistoryUI │
│                │     │                     │
└────────────────┘     └─────────────────────┘
```

## 3. Data Flow Diagram - Address Validation Process

```
┌────────────────┐     ┌─────────────────────┐     ┌────────────────┐
│  User          │     │                     │     │  Azure Maps    │
│  Input         │────▶│  AddressFormUI      │     │  API           │
│                │     │                     │     │                │
└────────────────┘     └─────────┬───────────┘     └───────┬────────┘
                                 │                         │
                                 │                         │
                                 ▼                         │
┌────────────────┐     ┌─────────────────────┐            │
│                │     │                     │            │
│  Validation    │◀────│  AddressValidation  │            │
│  Result        │     │  Service            │            │
│                │     │                     │            │
└────────────────┘     └─────────┬───────────┘            │
                                 │                         │
                                 │ calls                   │
                                 ▼                         │
┌────────────────┐     ┌─────────────────────┐            │
│                │     │                     │            │
│  Token         │────▶│  AzureMapsService   │────────────┘
│  Service       │     │                     │ HTTP Request
│                │     │                     │
└────────────────┘     └─────────┬───────────┘
                                 │
                                 │ stores result
                                 ▼
┌────────────────┐     ┌─────────────────────┐
│                │     │                     │
│  JSON          │◀────│  Repository         │
│  Storage       │     │                     │
│                │     │                     │
└────────────────┘     └─────────────────────┘
```

## 4. Class Diagram

```
┌───────────────────────────┐      ┌────────────────────────────┐
│ AddressInput              │      │ AddressSearchResponse      │
├───────────────────────────┤      ├────────────────────────────┤
│ + AddressLine1: string    │      │ + Results: AddressSearch[] │
│ + AddressLine2: string?   │      └────────────────────────────┘
│ + AddressLine3: string?   │                 ▲
│ + PostalCode: string      │                 │
│ + City: string            │                 │
│ + Country: string         │      ┌────────────────────────────┐
├───────────────────────────┤      │ AddressSearchResult        │
│ + ToSingleLineString()    │      ├────────────────────────────┤
└───────────────────────────┘      │ + Score: double            │
             │                     │ + Address: Address         │
             │                     │ + Position: Position       │
             ▼                     └────────────────────────────┘
┌───────────────────────────┐                 │
│ AddressValidationResult   │                 │
├───────────────────────────┤                 │
│ + IsValid: bool           │                 │
│ + ConfidencePercentage: d │                 │
│ + ValidationMessage: str  │                 │
│ + FreeformAddress: str?   │                 │
│ + MatchedAddress: Address?│◀────────────────┘
│ + Position: Position?     │
│ + OriginalInput: AddressI?│
└───────────────────────────┘


┌─────────────────────────────────┐
│ «interface»                     │
│ IAddressValidationService       │
├─────────────────────────────────┤
│ + ValidateAddressAsync(         │
│   AddressInput): Task<Result>   │
│ + ValidateAddressAsync(         │
│   string): Task<Result>         │
└─────────────────────────────────┘
             ▲
             │
             │
┌─────────────────────────────────┐       ┌─────────────────────────┐
│ AddressValidationService        │       │ «interface»             │
├─────────────────────────────────┤       │ IAzureMapsService       │
│ - _azureMapsService             │──────▶├─────────────────────────┤
│ - _minimumConfidenceThreshold   │       │ + SearchAddressAsync()  │
├─────────────────────────────────┤       └─────────────────────────┘
│ + ValidateAddressAsync(         │                  ▲
│   AddressInput): Task<Result>   │                  │
│ + ValidateAddressAsync(         │                  │
│   string): Task<Result>         │       ┌─────────────────────────┐
└─────────────────────────────────┘       │ AzureMapsService        │
                                          ├─────────────────────────┤
                                          │ - _httpClient           │
                                          │ - _tokenService         │
                                          │ - _config               │
                                          ├─────────────────────────┤
                                          │ + SearchAddressAsync()  │
                                          └─────────────────────────┘
```

## 5. UI Component Interactions

```
┌────────────────────────────────────────────────┐
│                   User                         │
└─────────────────────┬──────────────────────────┘
                      │
                      │ interacts with
                      ▼
┌────────────────────────────────────────────────┐
│              ConsoleUIService                  │
├────────────────────────────────────────────────┤
│ + Initialize()                                 │
│ + ShowMainMenu()                               │
│ + ShowSpinnerAsync<T>(message, Func<Task<T>>)  │
│ + ShowError(message)                           │
│ + ShowGoodbye()                                │
│ + PressAnyKeyToContinue()                      │
└────────────────────┬───────────────────────────┘
                     │
                     │ provides base UI
                     │ capabilities to
                     ▼
┌──────────────────────────┐     ┌─────────────────────────────┐
│     AddressFormUI         │     │     ValidationHistoryUI     │
├──────────────────────────┤     ├─────────────────────────────┤
│                          │     │                             │
│ + CollectAddressInput()  │     │ + DisplayValidationHistory()│
│ + DisplayValidationResult│     │ + SelectRecordFromHistory() │
│   (result, input?)       │     │ + ConfirmClearHistory()     │
│                          │     │ + ShowClearSuccessMessage() │
└──────────────────────────┘     │ + ShowNoHistoryToClear()    │
                                 └─────────────────────────────┘
```

## 6. Sequence Diagram - Address Validation

```
┌─────┐          ┌──────────┐         ┌─────────┐         ┌───────┐        ┌─────────┐
│User │          │AddressUI │         │Validation│         │AzureMaps│       │Repository│
│     │          │          │         │Service   │         │Service  │       │          │
└──┬──┘          └────┬─────┘         └────┬─────┘         └────┬───┘       └────┬─────┘
   │                  │                    │                    │                 │
   │ Enter Address    │                    │                    │                 │
   │ ─────────────────>                    │                    │                 │
   │                  │                    │                    │                 │
   │                  │ ValidateAddress    │                    │                 │
   │                  │ ───────────────────>                    │                 │
   │                  │                    │                    │                 │
   │                  │                    │ SearchAddressAsync │                 │
   │                  │                    │ ───────────────────>                 │
   │                  │                    │                    │                 │
   │                  │                    │                    │ API Request     │
   │                  │                    │                    │ ──────────────> │
   │                  │                    │                    │                 │
   │                  │                    │                    │ API Response    │
   │                  │                    │                    │ <─────────────── │
   │                  │                    │                    │                 │
   │                  │                    │ Result             │                 │
   │                  │                    │ <────────────────── │                 │
   │                  │                    │                    │                 │
   │                  │                    │ SaveValidationResult                 │
   │                  │                    │ ──────────────────────────────────> │
   │                  │                    │                    │                 │
   │                  │ Result             │                    │                 │
   │                  │ <───────────────── │                    │                 │
   │                  │                    │                    │                 │
   │ Display Result   │                    │                    │                 │
   │ <─────────────── │                    │                    │                 │
   │                  │                    │                    │                 │
┌──┴──┐          ┌────┴─────┐         ┌────┴─────┐         ┌────┴───┐       ┌────┴─────┐
│User │          │AddressUI │         │Validation│         │AzureMaps│       │Repository│
│     │          │          │         │Service   │         │Service  │       │          │
└─────┘          └──────────┘         └──────────┘         └─────────┘       └──────────┘
```

## 7. Infrastructure and Deployment

```
┌───────────────────────────────────────────────────────────────┐
│                     Client Machine                            │
│                                                               │
│   ┌───────────────────────────────────────────────────────┐   │
│   │              AddressValidator.Console.exe             │   │
│   │                                                       │   │
│   │  ┌─────────────┐  ┌──────────────┐  ┌─────────────┐   │   │
│   │  │   UI        │  │ Services     │  │ Repository  │   │   │
│   │  │ Components  │  │              │  │             │   │   │
│   │  └─────────────┘  └──────────────┘  └─────────────┘   │   │
│   │                                                       │   │
│   └───────────────────────────┬───────────────────────────┘   │
│                               │                               │
└───────────────────────────────┼───────────────────────────────┘
                                │
                                │ HTTPS
                                ▼
┌───────────────────────────────────────────────────────────────┐
│                        Azure Cloud                            │
│                                                               │
│   ┌─────────────────────┐       ┌─────────────────────────┐   │
│   │                     │       │                         │   │
│   │   Azure Maps        │◀─────▶│   Entra ID              │   │
│   │   Service           │ Auth  │   (Authentication)      │   │
│   │                     │       │                         │   │
│   └─────────────────────┘       └─────────────────────────┘   │
│                                                               │
└───────────────────────────────────────────────────────────────┘