# SerialAssistant.Win Architecture

## Overview

SerialAssistant.Win follows a layered architecture pattern with clear separation of concerns.

## Layers

### 1. SerialAssistant.App (Presentation Layer)

**Responsibilities:**
- WPF UI implementation
- Data binding
- ViewModel composition
- User interaction handling via Commands
- Window management
- UI thread dispatch

**Dependencies:**
- SerialAssistant.Core
- SerialAssistant.Infrastructure

**Constraints:**
- No direct System.IO.Ports usage
- No direct file system access
- No direct JsonSerializer usage
- No complex logic in MainWindow.xaml.cs

### 2. SerialAssistant.Core (Domain Layer)

**Responsibilities:**
- Domain models and entities
- Business logic interfaces
- Enumerations (SendMode, DisplayMode, etc.)
- Value objects (AppSettings, SerialPortSettings, etc.)
- Operation result wrappers
- Utility functions (HexConverter, SerialSettingsValidator)

**Dependencies:**
- None (pure .NET)

**Constraints:**
- No WPF references
- No System.Windows references
- No System.IO.Ports references
- No File/Directory/Registry access

### 3. SerialAssistant.Infrastructure (Infrastructure Layer)

**Responsibilities:**
- Serial port service implementation
- Serial port scanner implementation
- Application configuration persistence via JSON
- File system operations

**Dependencies:**
- SerialAssistant.Core

**Constraints:**
- No WPF references
- No System.Windows references
- No Dispatcher usage
- No Registry access
- No database usage
- No third-party JSON libraries (use System.Text.Json)

### 4. SerialAssistant.Tests (Test Layer)

**Responsibilities:**
- Unit tests for all layers
- Fake implementations of services for testing
- Test fixtures and helpers

**Dependencies:**
- SerialAssistant.Core
- SerialAssistant.Infrastructure
- xUnit
- coverlet.collector

**Constraints:**
- No real serial port device dependency
- No real AppData file pollution
- Use temporary directories for file-based tests

## Project References

```
┌─────────────────────────────────────┐
│      SerialAssistant.App            │
│      (WPF Application)              │
└──────────────┬──────────────────────┘
               │
       ┌───────┴───────┐
       │               │
       ▼               ▼
┌──────────────┐ ┌─────────────────────┐
│   Serial     │ │     Serial          │
│ Assistant.   │ │  Assistant.         │
│ Core         │ │  Infrastructure     │
│ (Domain)     │ │  (Infrastructure)   │
└──────────────┘ └──────────┬──────────┘
                            │
                            ▼
                   ┌─────────────────┐
                   │   Serial        │
                   │ Assistant.      │
                   │ Core            │
                   └─────────────────┘

┌─────────────────────────────────────┐
│      SerialAssistant.Tests          │
│      (Test Project)                 │
└──────────────┬──────────────────────┘
               │
       ┌───────┴───────┐
       │               │
       ▼               ▼
┌──────────────┐ ┌─────────────────────┐
│   Serial     │ │     Serial          │
│ Assistant.   │ │  Assistant.         │
│ Core         │ │  Infrastructure     │
└──────────────┘ └─────────────────────┘
```

## Workflows

### Serial Port Scanning Flow

```
User clicks "Refresh"
    ↓
MainWindowViewModel.RefreshPortsCommand.Execute
    ↓
ISerialPortScanner.GetAvailablePorts
    ↓
SerialPortScanner (Infrastructure) calls SerialPort.GetPortNames
    ↓
Returns List<SerialPortInfo>
    ↓
ViewModel updates AvailablePorts
    ↓
UI updates via data binding
```

### Serial Port Open/Close Flow

**Open:**
```
User selects port, sets params, clicks "Open"
    ↓
MainWindowViewModel.ToggleConnectionCommand.Execute
    ↓
Validates parameters via SerialSettingsValidator
    ↓
ISerialPortService.Open
    ↓
SerialPortService creates SerialPort instance, opens
    ↓
Subscribes to DataReceived
    ↓
Returns OperationResult
    ↓
ViewModel updates ConnectionState
    ↓
Disables parameter controls, enables send controls
    ↓
StatusMessage updated
```

**Close:**
```
User clicks "Close"
    ↓
MainWindowViewModel.ToggleConnectionCommand.Execute
    ↓
ISerialPortService.Close
    ↓
SerialPortService unsubscribes DataReceived
    ↓
Closes and Disposes SerialPort
    ↓
Returns OperationResult
    ↓
ViewModel updates ConnectionState
    ↓
Enables parameter controls, disables send controls
    ↓
StatusMessage updated
```

### Data Send Flow

**Text Mode:**
```
User enters text, selects "Text", clicks "Send"
    ↓
MainWindowViewModel.SendCommand.Execute
    ↓
Gets text from SendTextInput
    ↓
Converts to UTF-8 bytes
    ↓
ISerialPortService.Send(byte[] data)
    ↓
SerialPortService calls SerialPort.Write
    ↓
Returns OperationResult
    ↓
StatusMessage updated (success or error)
```

**HEX Mode:**
```
User enters HEX string, selects "HEX", clicks "Send"
    ↓
MainWindowViewModel.SendCommand.Execute
    ↓
Validates HEX format via HexConverter.FromHexString
    ↓
If valid, gets byte[]
    ↓
ISerialPortService.Send(byte[] data)
    ↓
SerialPortService calls SerialPort.Write
    ↓
Returns OperationResult
    ↓
StatusMessage updated (success or error)
```

### Data Receive Flow

```
SerialPort.DataReceived event fires
    ↓
SerialPortService handler reads BytesToRead
    ↓
Reads bytes into buffer via SerialPort.Read
    ↓
Creates SerialReceiveData
    ↓
Invokes DataReceived event
    ↓
MainWindowViewModel.OnDataReceived receives data
    ↓
Creates updateAction for updating ReceiveDisplayViewModel
    ↓
Calls IUiThreadInvoker.Invoke(updateAction) to switch to UI thread
    ↓
DispatcherUiThreadInvoker uses Application.Current.Dispatcher
    ↓
ReceiveDisplayViewModel updates DisplayText and ReceiveCount
    ↓
UI updates via data binding
```

### Configuration Persistence Flow

**Load (App startup):**
```
Application starts (App.xaml.cs)
    ↓
Creates JsonAppSettingsService
    ↓
Creates MainWindowViewModel with services
    ↓
ViewModel constructor calls LoadSettings
    ↓
IAppSettingsService.Load
    ↓
JsonAppSettingsService checks if %AppData%\SerialAssistant.Win\settings.json exists
    ↓
If exists, reads and deserializes
    ↓
If missing or damaged, returns AppSettings with defaults
    ↓
ViewModel applies settings (LastPortName, BaudRate, DataBits, etc.)
    ↓
Refreshes serial ports and attempts to select LastPortName
```

**Save (App exit):**
```
Application closing (App.xaml.cs OnExit)
    ↓
Calls MainWindowViewModel.SaveSettings
    ↓
ViewModel creates AppSettings from current state
    ↓
IAppSettingsService.Save(AppSettings)
    ↓
JsonAppSettingsService ensures directory exists
    ↓
Serializes to JSON with WriteIndented = true
    ↓
Writes to %AppData%\SerialAssistant.Win\settings.json
    ↓
Returns OperationResult
```

## UI Thread Switching

All UI updates from non-UI thread go through IUiThreadInvoker:

- **SerialPort.DataReceived** fires on thread pool
- **ViewModel callback** receives data
- **IUiThreadInvoker.Invoke(Action)** marshals to UI thread
- **DispatcherUiThreadInvoker** uses Application.Current.Dispatcher.BeginInvoke

This ensures thread safety for WPF data binding.

## Configuration File

**Location:** %AppData%\SerialAssistant.Win\settings.json

**Format:**
```json
{
  "LastPortName": "COM3",
  "BaudRate": 9600,
  "DataBits": 8,
  "Parity": "None",
  "StopBits": "One",
  "SendMode": 0,
  "DisplayMode": 0
}
```

## Current Status

All Phases 0-7 complete. Full serial port functionality with configuration persistence.
