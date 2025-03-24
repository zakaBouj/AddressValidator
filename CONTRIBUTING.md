# Contributing to AddressValidator

Thank you for considering contributing to AddressValidator! This document provides guidelines and instructions for contributing to this project.

## Code of Conduct

By participating in this project, you agree to maintain a respectful and inclusive environment for everyone.

## How Can I Contribute?

### Reporting Bugs

When reporting bugs, please include:

- A clear, descriptive title
- Steps to reproduce the issue
- Expected behavior vs. actual behavior
- Screenshots if applicable
- Your environment details (OS, .NET version, etc.)

### Suggesting Enhancements

Enhancement suggestions are welcome! Please include:

- A clear description of the enhancement
- The motivation behind the suggestion
- Possible implementation approaches if you have ideas

### Pull Requests

1. Fork the repository
2. Create a new branch for your feature/fix
3. Write your code following the coding conventions
4. Add or update tests as needed
5. Ensure all tests pass
6. Submit your pull request with a clear description

## Development Setup

1. Clone the repository
2. Copy the example configuration:
   ```bash
   cp AddressValidator.Console/appsettings.example.json AddressValidator.Console/appsettings.json
   ```
3. Obtain Azure Maps credentials and update the configuration
4. Build the solution: `dotnet build`
5. Run tests: `dotnet test`

## Coding Conventions

- Follow standard C# coding conventions
- Use meaningful variable and method names
- Write method and class documentation using XML comments
- Keep methods focused and concise

## Testing

- Add tests for new features
- Ensure existing tests pass before submitting pull requests
- Strive for high test coverage

## Documentation

- Update documentation to reflect your changes
- Add comments to explain complex code sections
- Keep the README updated with new features or changed behavior

Thank you for contributing to AddressValidator!