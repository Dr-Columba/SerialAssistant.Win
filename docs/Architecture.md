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
  "DisplayMode": 0,
  "MaxDisplayBytes": 262144
}
```

## Receive Buffer Limit (Feature C)

### Overview

The receive buffer limit feature prevents memory issues with large communication records. It includes configurable buffer size, automatic trimming of old records, and preservation of single large records.

### Key Properties in ReceiveDisplayViewModel

Located in `SerialAssistant.App.ViewModels.ReceiveDisplayViewModel`:

```csharp
public int MaxDisplayBytes { get; set; } = 262144;
public int CurrentDisplayBytes { get; }
public int TrimmedRecordCount { get; }
```

- **MaxDisplayBytes**: Configurable maximum display buffer size in bytes (default: 262144 = 256 KiB)
- **CurrentDisplayBytes**: Current total size of all records in display buffer (read-only)
- **TrimmedRecordCount**: Number of records trimmed due to buffer limit (read-only)

### Buffer Trimming Strategy

When adding records via `AddTxData` or `AddRxData`:

1. New record added to internal list
2. Total size checked against `MaxDisplayBytes`
3. If exceeded, oldest records trimmed from beginning of list
4. Single record larger than `MaxDisplayBytes` is preserved
5. `TrimmedRecordCount` incremented for each trimmed record
6. `CurrentDisplayBytes` updated
7. Display text updated

### Trimming on MaxDisplayBytes Change

When `MaxDisplayBytes` setter is called:

1. New value validated (if ≤ 0, defaults to 262144)
2. If value decreases, old records trimmed immediately
3. If value increases, no trimmed records restored
4. Display text updated

### ReceivedBytesCount Behavior

`ReceivedBytesCount` never decreases due to trimming. It counts total bytes received since last clear.

### Clear Behavior

When `Clear()` is called:

1. All communication records cleared
2. `CurrentDisplayBytes` reset to 0
3. `TrimmedRecordCount` reset to 0
4. `ReceivedBytesCount` reset to 0
5. Display text cleared

### Configuration Persistence

**AppSettings fields:**
```csharp
public int MaxDisplayBytes { get; set; } = 262144;
```

**Load flow:**
```
Application starts
    ↓
JsonAppSettingsService.Load returns AppSettings
    ↓
MainWindowViewModel.ApplySettings
    ↓
ReceiveDisplay.MaxDisplayBytes = settings.MaxDisplayBytes
```

**Save flow:**
```
Application closing
    ↓
MainWindowViewModel.SaveSettings creates AppSettings
    ↓
AppSettings.MaxDisplayBytes = ReceiveDisplay.MaxDisplayBytes
    ↓
JsonAppSettingsService.Save writes to settings.json
```

Note: Communication records (TX/RX history) are NOT saved to configuration. Only buffer size preference is persisted.

## Current Status

All Phases 0-7 complete. Full serial port functionality with configuration persistence.

## Communication Records (Feature B)

### CommunicationDirection Enum

Located in `SerialAssistant.Core.Enums.CommunicationDirection`, this enum represents the direction of communication:

- **Tx**: Transmit (data sent by the application)
- **Rx**: Receive (data received from serial port)

### CommunicationRecord Model

Located in `SerialAssistant.Core.Models.CommunicationRecord`, this model represents a single communication record:

```csharp
public class CommunicationRecord
{
    public CommunicationDirection Direction { get; }
    public byte[] Data { get; }
    public DateTime Timestamp { get; }
}
```

Key points:
- Direction indicates TX or RX
- Data contains the original byte array (cloned to avoid external reference)
- Timestamp records when the communication occurred

### ReceiveDisplayViewModel Communication Records

`ReceiveDisplayViewModel` maintains a `List<CommunicationRecord>` internally instead of a simple byte list. This enables:

- TX/RX direction tracking
- Timestamp recording
- Historical record reformatting on display mode change

Key properties:
- **ShowTimestamp**: Controls whether timestamps are displayed (default: true)
- **ShowDirection**: Controls whether TX/RX markers are displayed (default: true)
- **IsHexDisplay**: Controls whether data is displayed as text or HEX

Display format examples:
- ShowTimestamp=true, ShowDirection=true: `[12:34:56.123] TX ABC`
- ShowTimestamp=false, ShowDirection=true: `TX ABC`
- ShowTimestamp=true, ShowDirection=false: `[12:34:56.123] ABC`
- ShowTimestamp=false, ShowDirection=false: `ABC`

### TX Record Flow (Send Success)

```
User clicks "Send" with valid data
    ↓
MainWindowViewModel.SendCommand.Execute
    ↓
Validates input (text/HEX format)
    ↓
If text mode, applies SendLineEnding (None/CR/LF/CRLF)
    ↓
ISerialPortService.Send(byte[] data)
    ↓
SerialPortService calls SerialPort.Write
    ↓
Returns OperationResult
    ↓
If success:
    - SentBytesCount increases
    - ReceiveDisplay.AddTxData(data) is called
    - StatusMessage updated
```

### RX Record Flow (Data Received)

```
SerialPort.DataReceived event fires
    ↓
SerialPortService handler reads BytesToRead
    ↓
Reads bytes into buffer
    ↓
Invokes DataReceived event
    ↓
MainWindowViewModel.OnDataReceived receives data
    ↓
Creates updateAction for updating ReceiveDisplayViewModel
    ↓
Calls IUiThreadInvoker.Invoke(updateAction) to switch to UI thread
    ↓
ReceiveDisplayViewModel.AddRxData(data) is called
    ↓
CommunicationRecord created with Rx direction
    ↓
Record added to internal list
    ↓
ReceivedBytesCount increases
    ↓
Display text reformatted with current ShowTimestamp/ShowDirection settings
```

### Display Reformatting on Settings Change

When ShowTimestamp, ShowDirection, or IsHexDisplay changes:
1. Property setter in ReceiveDisplayViewModel triggers UpdateDisplayText()
2. All CommunicationRecords are re-iterated
3. Display format applied according to current settings
4. ReceivedText property updated
5. UI binding updates automatically

### Configuration for Display Settings

**AppSettings fields:**
```csharp
public bool ShowTimestamp { get; set; } = true;
public bool ShowDirection { get; set; } = true;
```

**Load flow:**
```
Application starts
    ↓
JsonAppSettingsService.Load returns AppSettings
    ↓
MainWindowViewModel.ApplySettings
    ↓
ReceiveDisplay.ShowTimestamp = settings.ShowTimestamp
    ↓
ReceiveDisplay.ShowDirection = settings.ShowDirection
```

**Save flow:**
```
Application closing
    ↓
MainWindowViewModel.SaveSettings creates AppSettings
    ↓
AppSettings.ShowTimestamp = ReceiveDisplay.ShowTimestamp
    ↓
AppSettings.ShowDirection = ReceiveDisplay.ShowDirection
    ↓
JsonAppSettingsService.Save writes to settings.json
```

Note: Communication records (TX/RX history) are NOT saved to configuration. Only display preferences are persisted.

## Send Line Ending (Feature A)

### SendLineEnding Enum

Located in `SerialAssistant.Core.Enums.SendLineEnding`, this enum represents line ending options:

- **None**: No line ending appended
- **CR**: Carriage Return (0x0D)
- **LF**: Line Feed (0x0A)
- **CRLF**: Carriage Return + Line Feed (0x0D 0x0A)

### Text Mode Send with Line Ending

```
User selects Text mode, enters "ABC", selects CRLF
    ↓
MainWindowViewModel.SendCommand.Execute
    ↓
Converts "ABC" to UTF-8 bytes: [0x41, 0x42, 0x43]
    ↓
Appends line ending based on SelectedSendLineEnding:
    - None: [0x41, 0x42, 0x43]
    - CR: [0x41, 0x42, 0x43, 0x0D]
    - LF: [0x41, 0x42, 0x43, 0x0A]
    - CRLF: [0x41, 0x42, 0x43, 0x0D, 0x0A]
    ↓
ISerialPortService.Send(byte[] data)
    ↓
TX record added with actual bytes sent (including line ending)
```

### HEX Mode Send Behavior

HEX mode does NOT append line endings, even if SendLineEnding is set. This maintains data precision for binary protocols.

```
User selects HEX mode, enters "41 42 43"
    ↓
HexConverter.FromHexString validates and converts
    ↓
Byte array: [0x41, 0x42, 0x43]
    ↓
No line ending appended regardless of SendLineEnding setting
    ↓
ISerialPortService.Send([0x41, 0x42, 0x43])
```
