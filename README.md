# SerialAssistant.Win

A Windows serial port assistant application built with C# + .NET 8 + WPF.

## Project Overview

SerialAssistant.Win is a desktop application for serial port communication on Windows. It provides a user-friendly interface for scanning, opening, closing serial ports, and sending/receiving data with HEX conversion support and configuration persistence.

## Technology Stack

- **Language**: C# 12
- **Framework**: .NET 8
- **UI Framework**: WPF (Windows Presentation Foundation)
- **Target Platform**: Windows
- **Test Framework**: xUnit

## Current Features

1. **Serial Port Scanning**: Auto-detect available COM ports
2. **Serial Port Connection**: Open and close serial ports with configurable parameters
3. **Text and HEX Mode Send**: Send data in text or HEX format
4. **Send Line Ending**: Support None/CR/LF/CRLF line endings for text mode send
5. **TX/RX Direction Marking**: Display TX (transmit) and RX (receive) records in receive area
6. **Timestamp Display**: Optional timestamp display for communication records
7. **Text and HEX Mode Receive**: Display received data in text or HEX format
8. **Clear Receive Buffer**: Clear received data and reset receive count
9. **Configuration Persistence**: Save and load serial port settings, display preferences, and line ending options
10. **Receive Buffer Limit**: Configurable receive buffer size (64 KiB, 256 KiB, 1 MiB, 4 MiB) with automatic trimming of old records when limit is exceeded
11. **Single Large Record Preservation**: Single record larger than buffer limit is preserved
12. **Send History**: Automatic recording of sent messages with duplicate removal, dropdown selection for quick resend, and configuration persistence

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
│  ├─ FinalReview.md
│  ├─ ManualTestChecklist.md
│  └─ PhaseReports/
│
└─ src/
   ├─ SerialAssistant.App/          # WPF Application Layer
   │  ├─ Commands/                  # RelayCommand
   │  ├─ UI/                        # UI utilities
   │  ├─ ViewModels/                # ViewModels
   │  ├─ App.xaml                   # Application entry point
   │  ├─ App.xaml.cs                # Composition Root
   │  ├─ MainWindow.xaml            # Main window UI
   │  └─ MainWindow.xaml.cs         # Minimal code-behind
   │
   ├─ SerialAssistant.Core/         # Domain Layer
   │  ├─ Enums/                     # DisplayMode, SendMode, SerialConnectionState
   │  ├─ Models/                    # Domain models and DTOs
   │  ├─ Services/                  # Service interfaces
   │  └─ Utilities/                 # HexConverter, SerialSettingsValidator
   │
   ├─ SerialAssistant.Infrastructure/  # Infrastructure Layer
   │  ├─ Configuration/             # JsonAppSettingsService
   │  └─ Serial/                    # SerialPortService, SerialPortScanner
   │
   └─ SerialAssistant.Tests/        # Test Project
      ├─ Commands/                  # RelayCommand tests
      ├─ Infrastructure/            # Infrastructure tests with fakes
      ├─ Models/                    # OperationResult tests
      ├─ Utilities/                 # HexConverter, SerialSettingsValidator tests
      └─ ViewModels/                # ViewModel tests
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
- Windows 10 or later

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

## Configuration File Location

Application settings are stored at:

```
%AppData%\SerialAssistant.Win\settings.json
```

## Known Limitations

- No logging persistence
- No file export functionality
- No database integration
- No auto-reconnection
- No protocol parsing
- No cycle/scheduled transmission
- No complex settings page
- No send history buffer

## Development Phases

The project follows a phased development approach:

1. **Phase 0**: Repository and Project Skeleton Initialization
2. **Phase 1**: Core Models, Interfaces, and Basic Utilities
3. **Phase 2**: WPF UI and ViewModel Skeleton
4. **Phase 3**: Serial Port Scanning
5. **Phase 4**: Serial Port Open/Close
6. **Phase 5**: Data Transmission
7. **Phase 6**: Data Reception and Display
8. **Phase 7**: Basic Configuration Persistence
9. **Phase 8**: Full Quality Check and Final Review
10. **Feature A**: Send Line Ending Options (None/CR/LF/CRLF)
11. **Feature B**: TX/RX Direction Marking and Timestamp Display
12. **Feature C**: Receive Buffer Limit and Configuration
13. **Feature D**: Send History (recording, UI dropdown, persistence)

## Code Style

This project uses EditorConfig for consistent code style:
- UTF-8 encoding
- CRLF line endings
- 4-space indentation for C# files
- Opening braces on new lines (Allman style)
- No double-slash comments (use /* */ instead)
- No empty catch blocks

## License

MIT License
