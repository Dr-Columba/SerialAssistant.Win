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

## Send History (Feature D)

### SendHistoryItem Model

Located in `SerialAssistant.Core.Models.SendHistoryItem`:

```csharp
public class SendHistoryItem
{
    public string Content { get; set; }
    public SendMode SendMode { get; set; }

    public SendHistoryItem() { }
    public SendHistoryItem(string content, SendMode sendMode) { }
}
```

- **Content**: User input text before sending (not including line ending)
- **SendMode**: Text or Hex mode used for this history entry

### Key Properties in MainWindowViewModel

```csharp
public ObservableCollection<SendHistoryItem> SendHistory { get; }
public SendHistoryItem? SelectedSendHistoryItem { get; set; }
public int MaxSendHistoryCount { get; set; } = 20;
public ICommand ClearSendHistoryCommand { get; }
```

- **SendHistory**: ObservableCollection of send history items, index 0 = most recent
- **SelectedSendHistoryItem**: Currently selected history item (for backfill)
- **MaxSendHistoryCount**: Maximum number of history items (default: 20)
- **ClearSendHistoryCommand**: Command to clear all history

### AddToSendHistory Recording Strategy

After successful send:

1. Get original SendText input (not including line ending)
2. Get current SendMode (Text or Hex)
3. Check for duplicate: same Content AND same SendMode
4. If duplicate found, remove old entry
5. Insert new entry at index 0
6. Trim to MaxSendHistoryCount if exceeded

### Duplicate Removal Rules

- Duplicate condition: Content same AND SendMode same
- Same Content but different SendMode: NOT duplicate (kept separately)
- On duplicate send: old entry removed, new entry inserted at index 0

### Sort Order

- Index 0 = Most recent item
- Index N = N-th most recent item
- When MaxSendHistoryCount exceeded: oldest item (last index) is removed

### ClearSendHistoryCommand Behavior

When executed:
1. SendHistory.Clear()
2. SelectedSendHistoryItem = null
3. SendText remains unchanged
4. ReceiveDisplay remains unchanged
5. Connection state unchanged

### SelectedSendHistoryItem Backfill

When user selects a history item from dropdown:

1. SendText = SelectedSendHistoryItem.Content
2. SelectedSendMode = SelectedSendHistoryItem.SendMode
3. NO send triggered
4. NO new history entry added
5. NO ReceiveDisplay modification

### Configuration Persistence

**AppSettings fields:**
```csharp
public int MaxSendHistoryCount { get; set; } = 20;
public List<SendHistoryItem> SendHistory { get; set; }
```

**Load flow:**
```
Application starts
    ↓
JsonAppSettingsService.Load returns AppSettings
    ↓
MainWindowViewModel.ApplySettings
    ↓
MaxSendHistoryCount = settings.MaxSendHistoryCount
    ↓
RestoreSendHistory(settings.SendHistory)
    ↓
SelectedSendHistoryItem = null
```

**RestoreSendHistory safety rules:**
1. If settings.SendHistory is null, use empty list
2. Skip null history items
3. Skip empty/whitespace Content
4. Skip invalid SendMode values
5. Remove duplicates (same Content + SendMode)
6. Trim to MaxSendHistoryCount
7. SelectedSendHistoryItem set to null

**Save flow:**
```
Application closing
    ↓
MainWindowViewModel.SaveSettings creates AppSettings
    ↓
AppSettings.MaxSendHistoryCount = MaxSendHistoryCount
    ↓
AppSettings.SendHistory = SendHistory.ToList()
    ↓
JsonAppSettingsService.Save writes to settings.json
```

**NOT saved to configuration:**
- SelectedSendHistoryItem (always null after restore)
- SendText (current input)
- TX/RX communication records

---

## Modbus Architecture Planning

### Overview

This section defines the architecture for Modbus functionality in SerialAssistant.Win. The design follows the same layered architecture principles established for serial terminal functionality.

### Layer Responsibilities for Modbus

| Layer | Modbus Responsibility | Examples |
|-------|----------------------|----------|
| Core | Pure protocol implementation | ModbusCrc16, ModbusRtuFrame, ModbusTcpFrame |
| Infrastructure | Transport adapters | ISerialPortService, ITcpService (future) |
| App | UI state and user interaction | ModbusViewModel, ModbusPage |

### Core Layer Modbus Rules

**DO:**
- Protocol models (frames, requests, responses)
- CRC16 calculation
- Frame building and parsing algorithms
- Function code enums
- Data type definitions

**DO NOT:**
- Reference WPF
- Reference System.IO.Ports
- Access file system
- Contain UI state
- Implement transport logic

### Infrastructure Layer Modbus Rules (Future)

**DO:**
- Serial port transport adapter
- TCP socket transport adapter
- Connection management

**DO NOT:**
- Reference WPF
- Implement Modbus protocol rules
- Contain UI ViewModel

### App Layer Modbus Rules

**DO:**
- ModbusPage (UI)
- ModbusViewModel (state management)
- Form input handling
- Command triggering
- Result display

**DO NOT:**
- Directly concatenate protocol bytes
- Implement CRC calculations
- Bypass Core protocol layer

### Critical Boundary Rules

1. **MainWindowViewModel does NOT host Modbus business logic**
   - MainWindowViewModel remains a pure Shell ViewModel
   - Modbus logic goes into ModbusViewModel

2. **ModbusViewModel does NOT reference System.IO.Ports**
   - Uses ISerialPortService interface
   - Protocol work delegated to Core layer

3. **Core Modbus does NOT access file system**
   - Pure protocol implementation
   - No configuration file I/O

4. **Core Modbus does NOT reference WPF**
   - Pure .NET library
   - Can be unit tested without UI dependencies

### Dependency Direction

```
UI (ModbusPage)
    ↓
ViewModel (ModbusViewModel)
    ↓
Core Protocol (SerialAssistant.Core.Modbus)
    ↓
Infrastructure Transport (ISerialPortService, ITcpService)
```

### Data Flow

```
User Input (Address, Quantity, Function Code)
    ↓
ModbusViewModel.CreateRequest()
    ↓
ModbusRtuFrameBuilder.Build() / ModbusTcpFrameBuilder.Build()
    ↓
ISerialPortService.Send() / ITcpService.Send()
    ↓
Transport sends bytes over wire
    ↓
Response received
    ↓
ModbusRtuFrameParser.Parse() / ModbusTcpFrameParser.Parse()
    ↓
ModbusResponse / ModbusExceptionResponse
    ↓
ModbusViewModel displays result
```

### Current Namespace Structure (G1, G2, G3 Implemented)

```
src/SerialAssistant.Core/Modbus/
├── Common/                              ✅ Implemented
│   ├── ModbusFunctionCode.cs            ✅ Implemented
│   └── ModbusDataType.cs                ✅ Implemented
├── Models/                              ✅ Implemented
│   └── ModbusRegisterValue.cs           ✅ Implemented
├── Utilities/                           ✅ Implemented
│   └── ModbusCrc16.cs                   ✅ Implemented
├── Rtu/                                 ✅ Implemented (G2)
│   ├── ModbusRtuErrorCode.cs            ✅ Implemented
│   ├── ModbusRtuFrame.cs                ✅ Implemented
│   ├── ModbusRtuParseResult.cs          ✅ Implemented
│   ├── ModbusRtuRequestBuilder.cs       ✅ Implemented
│   └── ModbusRtuResponseParser.cs       ✅ Implemented
├── Tcp/                                 ✅ Implemented (G3)
│   ├── ModbusTcpErrorCode.cs            ✅ Implemented
│   ├── MbapHeader.cs                    ✅ Implemented
│   ├── ModbusTcpParseResult.cs          ✅ Implemented
│   ├── ModbusTcpFrame.cs                ✅ Implemented
│   ├── ModbusTcpRequestBuilder.cs       ✅ Implemented
│   └── ModbusTcpResponseParser.cs       ✅ Implemented
└── Utils/
    └── ModbusByteUtils.cs               ⬜ Pending
```

### Implementation Order

```
G1: Core Foundation (Completed 2026-05-26)
├── ModbusCrc16 ✅
├── ModbusFunctionCode enum ✅
├── ModbusDataType enum ✅
└── ModbusRegisterValue ✅

G2: RTU Implementation (Completed 2026-05-26)
├── ModbusRtuFrame ✅
├── ModbusRtuRequestBuilder ✅
├── ModbusRtuResponseParser ✅
└── Function codes 03, 04, 06, 10 ✅

G3: TCP Implementation (Completed 2026-05-26)
├── MbapHeader ✅
├── ModbusTcpFrame ✅
├── ModbusTcpRequestBuilder ✅
├── ModbusTcpResponseParser ✅
└── Function codes 03, 04, 06, 10 ✅

G4: ModbusViewModel (Completed 2026-05-26)
├── ModbusTransportMode enum ✅
├── ModbusRequestKind enum ✅
├── Request building (RTU/TCP) ✅
├── Response parsing (RTU/TCP) ✅
├── Error handling ✅
└── HEX conversion ✅

G5: ModbusPage UI (Completed 2026-05-26)
├── ModbusPage.xaml ✅
├── ModbusPage.xaml.cs ✅
├── MainWindowViewModel navigation ✅
├── MainWindow.xaml navigation buttons ✅
├── Address input ✅
├── Function code selection ✅
├── Read/Write buttons ✅
└── Response display ✅
```

### App Layer Modbus ViewModel Implementation

**ModbusViewModel Location:** `src/SerialAssistant.App/ViewModels/ModbusViewModel.cs`

**Key Characteristics:**
- Inherits from `BaseViewModel` (INotifyPropertyChanged)
- Uses `RelayCommand` for commands
- Delegates protocol work to Core layer
- No System.IO.Ports references
- No file system access
- No WPF references

**ModbusViewModel Dependencies:**
```
ModbusViewModel
    ↓
Core.Modbus.Rtu.ModbusRtuRequestBuilder
Core.Modbus.Rtu.ModbusRtuResponseParser
Core.Modbus.Tcp.ModbusTcpRequestBuilder
Core.Modbus.Tcp.ModbusTcpResponseParser
Core.Utilities.HexConverter
```

**ModbusViewModel Properties:**
- `SelectedTransportMode`: Rtu/Tcp selection
- `SelectedRequestKind`: Function code selection (03/04/06/10)
- `UnitId`: Slave/Unit address (1-247)
- `TransactionId`: TCP transaction ID
- `StartAddress`: Register address
- `Quantity`: Number of registers
- `SingleWriteValue`: Value for single register write
- `MultipleWriteValuesText`: Comma/space separated hex values
- `RequestHex`: Built request as hex string
- `ResponseHex`: Input response hex string
- `ParsedSummary`: Human-readable parse result
- `StatusMessage`: Operation status
- `IsRtu`, `IsTcp`: Computed flags
- `HasRequest`, `HasParsedResponse`: Computed flags

**ModbusViewModel Commands:**
- `BuildRequestCommand`: Builds request frame via Core
- `ParseResponseCommand`: Parses response via Core
- `ClearCommand`: Clears all inputs and results

### Testing Strategy

| Component | Test Type | Dependencies |
|-----------|-----------|--------------|
| ModbusCrc16 | Unit | None |
| ModbusRtuFrameBuilder | Unit | None |
| ModbusRtuFrameParser | Unit | None |
| ModbusTcpFrameBuilder | Unit | None |
| ModbusTcpFrameParser | Unit | None |
| ModbusViewModel | Unit | Fake services |

---

## ModbusPage UI Implementation

### Overview

This section describes the ModbusPage minimal UI implementation (G5).

### ModbusPage Location

- `src/SerialAssistant.App/Views/ModbusPage.xaml` - UI layout
- `src/SerialAssistant.App/Views/ModbusPage.xaml.cs` - Minimal code-behind (only InitializeComponent)

### ModbusPage UI Structure

```
ModbusPage (UserControl)
├── Header: "Modbus"
├── Left Panel: Parameters
│   ├── TransportMode selection (ComboBox)
│   ├── RequestKind selection (ComboBox)
│   ├── UnitId input (TextBox)
│   ├── TransactionId input (TextBox)
│   ├── StartAddress input (TextBox)
│   ├── Quantity input (TextBox)
│   └── SingleWriteValue input (TextBox)
├── Right Panel: Multiple Write Values
│   ├── MultipleWriteValuesText (TextBox with multiline)
│   ├── Build Request button
│   ├── Parse Response button
│   └── Clear button
├── Bottom Left: Request Display
│   └── RequestHex (ReadOnly TextBox)
└── Bottom Right: Response and Parsing
    ├── ResponseHex (Input TextBox)
    └── ParsedSummary (ReadOnly TextBox)
```

### Data Binding

ModbusPage directly binds to ModbusViewModel properties:

- `ItemsSource="{Binding TransportModes}"`
- `SelectedItem="{Binding SelectedTransportMode}"`
- `ItemsSource="{Binding RequestKinds}"`
- `SelectedItem="{Binding SelectedRequestKind}"`
- `Text="{Binding UnitId, UpdateSourceTrigger=PropertyChanged}"`
- `Command="{Binding BuildRequestCommand}"`
- `Command="{Binding ParseResponseCommand}"`
- `Command="{Binding ClearCommand}"`

### ModbusViewModel UI Collection Additions

**New Properties:**
- `TransportModes`: `IReadOnlyList<ModbusTransportMode>` (Rtu, Tcp)
- `RequestKinds`: `IReadOnlyList<ModbusRequestKind>` (03, 04, 06, 10)

These are static collections for UI binding purposes only.

### MainWindowViewModel Navigation Additions

**Navigation Properties:**
- `IsTerminalSelected`: `bool` (default: true)
- `IsModbusSelected`: `bool` (default: false)
- `IsTerminalPageVisible`: `bool` (computed from IsTerminalSelected)
- `IsModbusPageVisible`: `bool` (computed from IsModbusSelected)

**Navigation Commands:**
- `ShowTerminalCommand`: Sets IsTerminalSelected = true
- `ShowModbusCommand`: Sets IsModbusSelected = true

### MainWindow.xaml Navigation Updates

- Added BooleanToVisibilityConverter to Window.Resources
- TerminalPage bound to `IsTerminalPageVisible`
- ModbusPage bound to `IsModbusPageVisible`
- Navigation buttons in left panel bound to commands
- ModbusPage.DataContext set to `{Binding Modbus}`

### Layer Boundary Compliance (G5)

✅ ModbusPage.xaml.cs contains only InitializeComponent
✅ No business logic in code-behind
✅ All UI state stored in ModbusViewModel
✅ No System.IO.Ports references in ModbusViewModel
✅ No file system access in ModbusViewModel
✅ No Infrastructure layer changes
✅ No TerminalViewModel changes
✅ No MainWindow.xaml.cs changes
✅ ModbusPage binds directly to ModbusViewModel
✅ MainWindowViewModel only handles navigation, not Modbus business logic

### Version Update

UI display version updated from v0.4.3 to v0.4.4 in MainWindow.xaml.

---

## Modbus Closure Architecture Review

**Phase**: G6 - Modbus Manual Test and Documentation Closure

**Status**: ✅ Completed

### Architecture State

#### Core Layer

- ✅ Modbus RTU protocol (Frame, Builder, Parser)
- ✅ Modbus TCP protocol (Frame, Builder, Parser, MbapHeader)
- ✅ Common types (DataType, FunctionCode)
- ✅ Models (RegisterValue)
- ✅ Utilities (ModbusCrc16)

#### App Layer

- ✅ ModbusViewModel (BuildRequest, ParseResponse, Clear)
- ✅ ModbusPage.xaml / ModbusPage.xaml.cs
- ✅ MainWindowViewModel (navigation only)
- ✅ Shell navigation (Terminal ↔ Modbus)

#### Infrastructure Layer

- ⏳ No Modbus transport yet
- ⏳ SerialPortService exists for Terminal
- ⏳ No ModbusTcpTransportService yet

### Boundary Compliance

| Rule | Status | Notes |
|------|--------|-------|
| Core: No WPF | ✅ | No System.Windows references |
| Core: No file I/O | ✅ | No File/Directory operations |
| App: No System.IO.Ports | ✅ | No direct serial port access |
| App: No TcpClient/Socket | ✅ | No direct network access |
| App: No protocol in UI | ✅ | All protocol work in Core |
| Infrastructure: No WPF | ✅ | No UI framework references |

### Future Communication Path

When real Modbus communication is implemented:

```
┌─────────────────────────────────────────────────────────────────┐
│  App Layer                                                      │
│  ModbusViewModel → calls Infrastructure services                │
├─────────────────────────────────────────────────────────────────┤
│  Infrastructure Layer                                           │
│  ├── SerialPortService (existing) → for RTU                     │
│  └── ModbusTcpTransportService (new) → for TCP                  │
├─────────────────────────────────────────────────────────────────┤
│  External                                                       │
│  ├── Serial Port (RTU)                                          │
│  └── TCP Socket (TCP)                                           │
└─────────────────────────────────────────────────────────────────┘
```

**Key Rule**: App layer must NEVER directly reference System.IO.Ports or TcpClient. All communication must go through Infrastructure services.

---

## Modbus Transport Architecture Planning (G7)

### Overview

This section defines the detailed architecture for Modbus RTU/TCP transport integration, planned in G7.

### Current State (Before G7)
- ✅ Modbus RTU/TCP frame building/parsing in Core layer
- ✅ ModbusViewModel with BuildRequest/ParseResponse
- ✅ ModbusPage minimal UI
- ✅ SerialPortService exists for Terminal
- ❌ No real Modbus communication yet

### Future State (After G12)

Real Modbus RTU/TCP communication implemented, with proper interface separation and single port ownership model.

### Layer Boundary Rules (Non-Negotiable)

| Layer | Allowed | Forbidden |
|-------|---------|-----------|
| **Core** | Protocol framing, CRC, parsing | System.IO.Ports, TcpClient, Socket, WPF, File I/O |
| **App** | ViewModels, UI binding | Direct IO references |
| **Infrastructure** | Serial/TCP IO, transport | WPF, Dispatcher, Window |

### Proposed Interface Design

#### Core Layer Interfaces Only

All interfaces and DTOs are placed in Core layer. Core does NOT reference App layer.

**Interface Location:** `src/SerialAssistant.Core/Modbus/Transport/` or `src/SerialAssistant.Core/Services/Modbus/`

```csharp
// Concept only - NOT implemented in G7
public interface IModbusTransport
{
    Task<ModbusTransportResult> SendRequestAsync(
        ModbusRequestContext context,
        byte[] requestBytes,
        CancellationToken cancellationToken = default);

    Task<bool> ConnectAsync(CancellationToken cancellationToken = default);
    Task DisconnectAsync();
    bool IsConnected { get; }
    ModbusTransportOptions Options { get; }
}

public interface IModbusRtuTransport : IModbusTransport
{
    // RTU-specific properties/methods
}

public interface IModbusTcpTransport : IModbusTransport
{
    string IpAddress { get; }
    int Port { get; }
}

public class ModbusTransportResult
{
    public bool Success { get; set; }
    public byte[]? ResponseBytes { get; set; }
    public string? ErrorMessage { get; set; }
    public TimeSpan Duration { get; set; }
}

public class ModbusTransportOptions
{
    public TimeSpan SendTimeout { get; set; } = TimeSpan.FromSeconds(5);
    public TimeSpan ReceiveTimeout { get; set; } = TimeSpan.FromSeconds(5);
}

public class ModbusRequestContext
{
    // Mode NOT in Core DTO - use specific interfaces instead
    public byte UnitId { get; set; }
    public ushort TransactionId { get; set; } // For TCP only, optional for RTU
}
```

### Infrastructure Layer Implementation

#### RTU Transport

```
ModbusRtuTransport (Infrastructure)
    ↓
SerialPortService (existing Infrastructure)
    ↓
System.IO.Ports (Infrastructure only)
```

**Ownership Strategy**: Single Ownership Model
- Terminal and Modbus RTU cannot use serial port exclusively
- If Terminal is connected, Modbus RTU Connect is disabled
- If Modbus RTU is connected, Terminal Open is disabled
> **⚠️ G9A Update**: Previous G7 sketch had MainWindowViewModel coordinating ownership state, but this is superseded by G9A.
> Current plan: Core defines ISerialPortOwnershipCoordinator and SerialPortOwner in G9B; Infrastructure implements ownership coordination in G9C or later; App displays ownership state only.

#### TCP Transport

```
ModbusTcpTransport (Infrastructure - NEW)
    ↓
TcpClient (Infrastructure only)
    ↓
NetworkStream (Infrastructure only)
```

**TCP Features**:
- MBAP TransactionId matching
- Length field reading strategy
- Timeout handling
- Half-open connection detection

### ModbusViewModel Integration

```
ModbusViewModel (App)
    ↓
IModbusTransport interface (Core)
    ↓
ModbusRtuTransport/ModbusTcpTransport (Infrastructure)
```

**Key Rule**: ModbusViewModel NEVER sees SerialPort or TcpClient directly!

### Data Flow (RTU Example)

1. User configures RTU parameters → ModbusViewModel
2. User clicks Connect → ModbusViewModel.ConnectAsync()
3. ModbusViewModel calls IModbusRtuTransport.ConnectAsync()
4. ModbusRtuTransport uses SerialPortService to open port
5. User builds request → BuildRequest (Core layer)
6. User clicks Send Request → ModbusViewModel.SendRequestAsync()
7. ModbusRtuTransport sends bytes via SerialPortService
8. ModbusRtuTransport receives response bytes
9. ModbusViewModel calls Core parser to parse response
10. UI displays RequestHex, ResponseHex, ParsedSummary

### TCP Example

1. User configures TCP IP/Port → ModbusViewModel
2. User clicks Connect → ModbusViewModel.ConnectAsync()
3. ModbusTcpTransport connects via TcpClient
4. User builds request with TransactionId → BuildRequest (Core)
5. User clicks Send Request → ModbusViewModel.SendRequestAsync()
6. ModbusTcpTransport sends MBAP frame
7. ModbusTcpTransport receives response, verifies TransactionId
8. ModbusViewModel parses response via Core
9. UI displays results

### Serial Port Ownership Coordination

> **⚠️ G9A Update (2026-05-29): Earlier ownership sketch superseded**

**G9A ownership plan:**
- Core defines `ISerialPortOwnershipCoordinator` and `SerialPortOwner` enum
- Infrastructure implements concrete ownership coordination
- App displays ownership state and disables conflicting actions through bindings
- MainWindowViewModel must NOT be the ownership authority

```
MainWindowViewModel (G7 sketch - superseded by G9A):
- IsTerminalConnected: bool (superseded)
- IsModbusRtuConnected: bool (superseded)

When Terminal opens port:
- Modbus RTU Connect button disabled (superseded - now via ownership coordinator)

When Modbus RTU connects:
- Terminal Open button disabled (superseded - now via ownership coordinator)

Message: "Terminal is using the serial port. Please disconnect Terminal first."
```

### Error Strategy

| Error Category | Examples |
|---------------|----------|
| Input Validation | No port selected, invalid IP, timeout ≤ 0 |
| Connection | Open failed, connect failed, disconnected |
| Communication | Send failed, receive failed, timeout |
| Protocol | CRC invalid, TransactionId mismatch, invalid response |
| Business | Unsupported function, permission denied |

### Test Strategy

| Test Type | Uses Real Hardware? |
|-----------|---------------------|
| Unit Tests | ❌ No |
| Fake Transport Tests | ❌ No |
| Integration-like Tests | ❌ No (fake serial/TCP) |
| Manual Tests | ✅ Yes (optional) |

### Phase Implementation Order (G8A-G12)

```
G8A: Transport Contracts + Fake Transport (COMPLETED)
G8B: ViewModel Transport Injection
G9:  RTU Transport (real serial)
G10: TCP Transport (real socket)
G11: UI Integration
G12: Manual Verification
```

### Key Architecture Decisions from G7/G8A

1. **G8A First**: Transport contracts before real hardware
2. **Single Ownership**: Simple and safe for initial implementation
3. **Core Only Protocol**: CRC, framing stays in Core
4. **Core Only Interfaces**: All IModbusTransport interfaces in Core layer
5. **No App IO**: App layer NEVER sees System.IO.Ports or TcpClient
6. **Defer UI Styling**: Function first, polish later

### G8A Implementation Summary (May 2026)

**Completed**:
- Core/Modbus/Transport namespace created
- IModbusTransport, IModbusRtuTransport, IModbusTcpTransport interfaces
- ModbusTransportResult, ModbusTransportOptions, ModbusRequestContext, ModbusTransportErrorCode
- FakeModbusTransport in Tests project
- 40 new tests added (560 total)

**Not Included** (deferred to G8B/G9/G10):
- No real SerialPort usage
- No real TcpClient usage
- No ModbusViewModel integration
- No ModbusPage UI changes

### G8B Implementation Summary (May 2026)

**Completed**:
- ModbusViewModel transport injection
- ConnectTransportAsync, DisconnectTransportAsync, SendRequestAsync methods
- Connection state properties (IsConnected, IsBusy, ConnectionStatus)
- Error handling (LastTransportError)
- 26 new tests added (586 total)
- Version updated to v0.4.6

**Architecture**:
```
ModbusViewModel (App)
    │
    ├── IModbusTransport (Core interface)
    │
    └── FakeModbusTransport (Tests only)
```

**Not Included** (deferred to G9/G10):
- No real SerialPort usage
- No real TcpClient usage
- No Infrastructure layer changes
- No ModbusPage UI changes

---

## G9A: Modbus RTU Transport Capability Review (May 2026)

### Overview

This section documents the G9A review of existing serial port service capabilities for Modbus RTU transport implementation.

### G9A Status
✅ Completed - May 2026

### Existing Serial Service Structure

#### ISerialPortService Interface (Core Layer)

```csharp
public interface ISerialPortService
{
    SerialConnectionState ConnectionState { get; }
    OperationResult Open(SerialPortSettings settings);
    OperationResult Close();
    OperationResult Send(byte[] data);
    event EventHandler<SerialReceiveData>? DataReceived;
    event EventHandler<Exception>? ErrorOccurred;
    event EventHandler<SerialConnectionState>? ConnectionStateChanged;
}
```

**Key Capabilities:**
| Capability | Status |
|------------|--------|
| Open/Close | ✅ Yes |
| Send | ✅ Yes |
| DataReceived Event | ✅ Yes |
| SendAndReceiveAsync | ❌ No |
| CancellationToken | ❌ No |
| Per-Request Timeout | ❌ No |
| Port Ownership Tracking | ❌ No |

#### SerialPortService Implementation (Infrastructure Layer)

| Aspect | Status |
|--------|--------|
| **Receive Mechanism** | Fully event-based (DataReceived) |
| **Internal Buffer** | ❌ No |
| **Frame Boundary Detection** | ❌ No |
| **Request-Response Matching** | ❌ No |
| **Concurrency Control** | ❌ No |
| **Timeout Per Request** | ❌ No |

### Current Terminal Serial Data Flow

```
TerminalViewModel
    │
    ├── Open() → ISerialPortService.Open()
    ├── Send() → ISerialPortService.Send()
    │
    └── DataReceived ← Event-based callback
            │
            └── All received bytes forwarded to UI
```

**Terminal Behavior:**
- Opens serial port via ISerialPortService
- Sends data via ISerialPortService.Send()
- Receives data via DataReceived event
- No request-response pattern
- No timeout control
- No ownership awareness

### Modbus RTU Request-Response Requirements

| Requirement | Current Support | Gap |
|-------------|----------------|-----|
| Open/Close | ✅ Yes | - |
| Send Request | ✅ Yes | - |
| Wait for Response | ❌ No | Event-based only |
| Per-Request Timeout | ❌ No | No timeout per operation |
| Cancellation | ❌ No | No CancellationToken |
| Frame Boundary Detection | ❌ No | No frame matching |
| Request-Response Correlation | ❌ No | Cannot match response to request |

### Current Gaps for Modbus RTU

| Gap | Severity | Impact |
|-----|----------|--------|
| No SendAndReceiveAsync | 🔴 High | Cannot implement request-response |
| No Timeout Control | 🔴 High | Cannot wait with timeout |
| No Cancellation | 🔴 High | Cannot cancel ongoing operation |
| No Port Ownership | 🔴 High | Cannot prevent Terminal/Modbus conflict |
| No Frame Matching | 🟡 Medium | Cannot detect frame boundaries |

### Recommended Architecture (Option C)

#### Layer Boundaries (Unchanged)

| Layer | Allowed | Forbidden |
|-------|---------|-----------|
| **Core** | Protocol, interfaces | System.IO.Ports, TcpClient, WPF |
| **App** | ViewModels, UI binding | Direct IO references |
| **Infrastructure** | Serial/TCP IO, transport | WPF, Dispatcher |

#### New Components for Option C

```
┌─────────────────────────────────────────────────────────────────┐
│  App Layer                                                      │
│  ModbusViewModel → calls IModbusRtuTransport                   │
├─────────────────────────────────────────────────────────────────┤
│  Core Layer                                                     │
│  IModbusRtuTransport (existing)                                 │
│  ISerialPortOwnershipCoordinator (NEW - G9B)                    │
│  SerialPortOwner enum (NEW - G9B)                               │
├─────────────────────────────────────────────────────────────────┤
│  Infrastructure Layer                                           │
│  ModbusRtuTransport (future - G9C or later)                   │
│  SerialPortOwnershipCoordinator (future - G9C or later)        │
│  SerialPortService (existing, NOT modified)                     │
├─────────────────────────────────────────────────────────────────┤
│  External                                                       │
│  System.IO.Ports.SerialPort (Infrastructure only)              │
└─────────────────────────────────────────────────────────────────┘
```

**Important Notes:**
- G9B only adds Core contracts and tests (no Infrastructure changes)
- ISerialPortOwnershipCoordinator belongs to Core
- Infrastructure ownership implementation is deferred to G9C or later
- App displays ownership state but is not ownership authority

**Key Design Decisions:**

1. **Do NOT extend SerialPortService** - Event-based model incompatible with request-response
2. **Do NOT modify Terminal behavior** - Keep ISerialPortService unchanged for Terminal
3. **New ModbusRtuTransport** - Owns serial handling logic including timeout/cancellation
4. **New Ownership Coordinator** - Prevents Terminal/Modbus port conflicts
5. **App still forbidden System.IO.Ports** - All serial access via Infrastructure services

### G9B-G9D Phase Plan

| Phase | Focus | Key Deliverables |
|-------|-------|------------------|
| **G9B** | Ownership Coordinator Contracts | ISerialPortOwnershipCoordinator in Core |
| **G9C** | ModbusRtuTransport with Fake | ModbusRtuTransport + FakeSerialAdapter |
| **G9D** | Manual Verification | Real hardware testing |

### Why Option C Instead of Extending SerialPortService?

| Concern | Option A/B | Option C |
|---------|-----------|----------|
| Terminal behavior risk | ❌ High | ✅ None |
| Modbus request-response | ⚠️ Complex | ✅ Clean |
| Layer boundary | ⚠️ Violated | ✅ Preserved |
| Testability | ⚠️ Difficult | ✅ Easy (fake) |
| Code complexity | ⚠️ Mixed concerns | ✅ Separated |

### Critical Boundary Rules (G9A Reinforcement)

1. **App NEVER references System.IO.Ports**
2. **Infrastructure CAN reference System.IO.Ports**
3. **Core NEVER references Infrastructure**
4. **Terminal and Modbus RTU port ownership MUST be explicitly managed**

---

## G9B: Serial Port Ownership Coordinator Contracts (May 2026)

### G9B Status: ✅ Completed

### What G9B Added

1. **Core Layer Additions**:
   - `SerialPortOwner` enum: None, Terminal, ModbusRtu
   - `ISerialPortOwnershipCoordinator` interface
   - `SerialPortOwnershipChangedEventArgs`

2. **Test Layer Additions**:
   - `FakeSerialPortOwnershipCoordinator`
   - 32 new tests

3. **Version Update**:
   - Updated from v0.4.6 to v0.4.7

### G9B Architecture

#### SerialPortOwner Enum

```csharp
namespace SerialAssistant.Core.Services;

public enum SerialPortOwner
{
    None,
    Terminal,
    ModbusRtu
}
```

#### ISerialPortOwnershipCoordinator Interface

```csharp
namespace SerialAssistant.Core.Services;

public interface ISerialPortOwnershipCoordinator
{
    SerialPortOwner GetCurrentOwner(string portName);
    bool IsOwned(string portName);
    bool IsOwnedBy(string portName, SerialPortOwner owner);
    bool TryClaimOwnership(string portName, SerialPortOwner owner);
    bool TryReleaseOwnership(string portName, SerialPortOwner owner);
    event EventHandler<SerialPortOwnershipChangedEventArgs>? OwnershipChanged;
}
```

#### Current State After G9B

| Layer | Status |
|-------|--------|
| **Core** | New ownership contracts added |
| **App** | Unchanged |
| **Infrastructure** | Unchanged |
| **Tests** | Fake coordinator + tests added |

### Key G9B Decisions

1. **Core Only**: All ownership contracts in Core
2. **No App Logic**: App doesn't have ownership authority
3. **No Infrastructure Changes**: Still no real ownership coordinator
4. **No RTU Yet**: ModbusRtuTransport deferred to G9C

---

## G9C: Modbus RTU Transport with Fake Serial Adapter (May 2026)

### G9C Status: ✅ Completed

### What G9C Added

1. **Infrastructure Layer Additions**:
   - `IModbusRtuSerialAdapter` interface - serial adapter abstraction
   - `ModbusRtuTransport` class - implements `IModbusRtuTransport`
   - Location: `src/SerialAssistant.Infrastructure/Modbus/Transport/`

2. **Test Layer Additions**:
   - `FakeModbusRtuSerialAdapter` - test fake serial adapter
   - `ModbusRtuTransportTests` - 30 comprehensive tests
   - Location: `src/SerialAssistant.Tests/Infrastructure/Modbus/`

3. **Version Update**:
   - Updated from v0.4.7 to v0.4.8

### G9C Architecture

#### IModbusRtuSerialAdapter Interface

```csharp
namespace SerialAssistant.Infrastructure.Modbus.Transport;

public interface IModbusRtuSerialAdapter
{
    bool IsOpen { get; }
    Task<bool> OpenAsync(CancellationToken cancellationToken = default);
    Task CloseAsync(CancellationToken cancellationToken = default);
    Task<bool> WriteAsync(byte[] requestBytes, CancellationToken cancellationToken = default);
    Task<byte[]> ReadAsync(int maxBytes, TimeSpan timeout, CancellationToken cancellationToken = default);
}
```

#### ModbusRtuTransport Structure

```
ModbusRtuTransport (Infrastructure)
    │
    ├── IModbusRtuSerialAdapter (dependency)
    │       └── FakeModbusRtuSerialAdapter (tests)
    │
    ├── ISerialPortOwnershipCoordinator (dependency)
    │       └── FakeSerialPortOwnershipCoordinator (tests)
    │
    └── ModbusTransportOptions (configuration)
```

#### G9C Layer Boundary Compliance

| Rule | Status | Notes |
|------|--------|-------|
| App: No System.IO.Ports | ✅ | No direct serial access |
| App: No TcpClient/Socket | ✅ | No network references |
| Infrastructure: No WPF | ✅ | Pure .NET code |
| Core: No Infrastructure refs | ✅ | Contracts only |
| No real hardware | ✅ | Fake adapter only |

#### Current State After G9C

| Layer | Status |
|-------|--------|
| **Core** | Ownership contracts + transport contracts |
| **App** | Unchanged |
| **Infrastructure** | ModbusRtuTransport + IModbusRtuSerialAdapter |
| **Tests** | Fake adapters + comprehensive tests |

### G9C Test Coverage

| Test Class | Tests |
|-----------|-------|
| ModbusRtuTransportTests | 29 |

**Total Tests**: 647 (was 618 after G9B)

### Key G9C Decisions

1. **Adapter Pattern**: Serial access abstracted via `IModbusRtuSerialAdapter`
2. **Ownership Integration**: Transport claims/releases port ownership
3. **CRC Validation**: Optional CRC validation in transport layer
4. **Fake First**: All tests use fake adapter, no hardware required
5. **No Real IO**: Defer real serial to G9D

### Next Phase: G9D

G9D will add real System.IO.Ports serial adapter and perform manual hardware verification.

---

*Last updated: May 2026*
*Modbus Architecture Planning: May 2026*
*G5 ModbusPage UI Complete: May 2026*
*G6 Modbus Closure Complete: May 2026*
*G7 Modbus Transport Planning Complete: May 2026*
*G8A Modbus Transport Contracts Complete: May 2026*
*G8B ModbusViewModel Transport Injection Complete: May 2026*
*G9A Modbus RTU Transport Capability Review: May 2026*
*G9B Serial Port Ownership Coordinator Contracts: May 2026*
*G9C Modbus RTU Transport with Fake Serial: May 2026*
