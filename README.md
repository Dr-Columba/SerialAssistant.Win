# SerialAssistant.Win

A Windows 10 serial port assistant application built with C# + .NET 8 + WPF.

## Project Overview

SerialAssistant.Win is a desktop application for serial port communication on Windows 10. It provides a user-friendly interface for scanning, opening, closing serial ports, and sending/receiving data with HEX conversion support.

## Technology Stack

- **Language**: C# 12
- **Framework**: .NET 8
- **UI Framework**: WPF (Windows Presentation Foundation)
- **Target Platform**: Windows 10
- **Test Framework**: xUnit

## Project Structure

```
SerialAssistant.Win/
│
├─ SerialAssistant.Win.sln
├─ README.md
├─ .gitignore
├─ .editorconfig
│
├─ docs/
│  ├─ Architecture.md
│  └─ PhaseReports/
│
└─ src/
   ├─ SerialAssistant.App/          # WPF Application Layer
   │  └─ MainWindow.xaml            # Main application window
   │
   ├─ SerialAssistant.Core/         # Domain Layer
   │  └─ Models/                    # Domain models
   │
   ├─ SerialAssistant.Infrastructure/  # Infrastructure Layer
   │  └─ Services/                  # Infrastructure services
   │
   └─ SerialAssistant.Tests/        # Test Project
      └─ ProjectStructureTests.cs   # Basic structure tests
```

## Project References

```
SerialAssistant.App
├── SerialAssistant.Core
└── SerialAssistant.Infrastructure

SerialAssistant.Infrastructure
└── SerialAssistant.Core

SerialAssistant.Tests
├── SerialAssistant.Core
└── SerialAssistant.Infrastructure
```

## Build Instructions

### Prerequisites

- .NET 8 SDK or later
- Windows 10

### Restore Dependencies

```powershell
dotnet restore
```

### Build Solution

```powershell
dotnet build .\SerialAssistant.Win.sln -c Debug
```

### Run Tests

```powershell
dotnet test .\SerialAssistant.Win.sln -c Debug
```

### Run Application

```powershell
dotnet run --project .\src\SerialAssistant.App\SerialAssistant.App.csproj
```

## Current Phase

**Phase 0: Repository and Project Skeleton Initialization**

This phase establishes the basic project structure with four projects:
- SerialAssistant.App (WPF Application)
- SerialAssistant.Core (Domain Layer)
- SerialAssistant.Infrastructure (Infrastructure Layer)
- SerialAssistant.Tests (Test Project)

No serial port functionality is implemented yet.

## Development Phases

The project follows a phased development approach:

1. **Phase 0**: Repository and Project Skeleton Initialization (Current)
2. **Phase 1**: Serial Port Scanning and Basic Connection
3. **Phase 2**: Data Transmission and Reception
4. **Phase 3**: HEX Conversion and Display
5. **Phase 4**: Configuration Persistence
6. **Phase 5**: UI Polish and Advanced Features

## Code Style

This project uses EditorConfig for consistent code style:
- UTF-8 encoding
- CRLF line endings
- 4-space indentation for C# files
- Opening braces on new lines (Allman style)

## License

MIT License
