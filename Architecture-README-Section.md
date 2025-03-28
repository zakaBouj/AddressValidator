## Architecture Overview

The AddressValidator application follows a clean, layered architecture focusing on separation of concerns and maintainability:

### Layered Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                           UI Layer                           │
│  ┌───────────────┐   ┌───────────────┐   ┌───────────────┐  │
│  │ AddressFormUI │   │ConsoleUIService│   │ValidationHist.│  │
│  └───────────────┘   └───────────────┘   └───────────────┘  │
└──────────────────────────────────────────────────────────────┘
                           │
                           ▼
┌─────────────────────────────────────────────────────────────┐
│                        Service Layer                         │
│  ┌───────────────┐   ┌───────────────┐   ┌───────────────┐  │
│  │AddressValid.  │──▶│  AzureMaps    │◀──┤ AzureMapsToken│  │
│  │Service        │   │  Service      │   │ Service       │  │
│  └───────────────┘   └───────────────┘   └───────────────┘  │
└──────────────────────────────────────────────────────────────┘
                           │
                           ▼
┌─────────────────────┐    ┌─────────────────────┐
│  Repository Layer   │    │  External Services  │
│  ┌───────────────┐  │    │  ┌───────────────┐  │
│  │JsonAddressVal.│  │    │  │  Azure Maps   │  │
│  │Repository     │  │    │  │     API       │  │
│  └───────────────┘  │    │  └───────────────┘  │
└─────────────────────┘    └─────────────────────┘
```

For detailed architecture diagrams including component interactions, data flow, class diagrams, and more, see the [Architecture Documentation](AddressValidator-Architecture.md).

### Key Architecture Principles

- **Clean Layering**: UI, services, and data access are clearly separated
- **Dependency Injection**: All services are registered with the DI container
- **Interface-Based Design**: Components depend on abstractions, not implementations
- **Single Responsibility**: Each class has a focused purpose and clear responsibilities
- **Comprehensive Error Handling**: Structured exception handling throughout all layers