# UI Information Architecture

## Overview

This document defines the UI structure for SerialAssistant.Win, following the principle of "UI skeleton first, features second."

## Architecture Overview

```
┌─────────────────────────────────────────────────────────────────┐
│                    Top Status Bar                              │
│  ┌──────────┐ ┌──────────────────────┐ ┌──────────────────┐    │
│  │ Nav Menu │ │ Connection Status    │ │ App Controls     │    │
│  └──────────┘ │ (Port, Baud, Status) │ │ (Min, Max, Close)│    │
│               └──────────────────────┘ └──────────────────┘    │
├──────────────┬──────────────────────────────────────────────────┤
│              │                                                 │
│  Left        │              Main Workspace                      │
│  Navigation  │                                                 │
│  Panel       │  ┌──────────────────────────────────────────┐    │
│              │  │                                        │    │
│  ┌────────┐  │  │     Current Page Content               │    │
│  │Terminal│  │  │  (TerminalPage / ModbusPage / etc.)    │    │
│  ├────────┤  │  │                                        │    │
│  │Modbus  │  │  └──────────────────────────────────────────┘    │
│  ├────────┤  │                                                 │
│  │Templates│ │                                                 │
│  ├────────┤  │                                                 │
│  │Logs    │  │                                                 │
│  ├────────┤  │                                                 │
│  │Settings│  │                                                 │
│  └────────┘  │                                                 │
│              │                                                 │
├──────────────┴──────────────────────────────────────────────────┤
│                    Bottom Status Bar                            │
│  TX: 12345 bytes          RX: 67890 bytes          Status: OK │
└─────────────────────────────────────────────────────────────────┘
```

## 1. MainWindow.xaml - The Shell

**Responsibility:**
- Act as the container for all UI elements
- Provide layout structure
- Host navigation frame
- Manage window-level events

**Constraints:**
- No business logic
- No direct data binding to serial/Modbus logic
- Only contains layout controls

**Structure:**
```xml
<Window>
    <Grid>
        <!-- Row Definitions: TopBar, MainContent, BottomBar -->
        <!-- Column Definitions: NavPanel, Workspace -->

        <!-- Top Status Bar -->
        <Grid Grid.Row="0">
            <!-- Connection Status, App Controls -->
        </Grid>

        <!-- Main Content -->
        <Grid Grid.Row="1">
            <!-- Left Navigation Panel -->
            <NavigationView Grid.Column="0" />

            <!-- Main Workspace -->
            <Frame Grid.Column="1" />
        </Grid>

        <!-- Bottom Status Bar -->
        <Grid Grid.Row="2">
            <!-- Statistics, Status -->
        </Grid>
    </Grid>
</Window>
```

## 2. MainWindow.xaml.cs - Minimal Code-behind

**Responsibility:**
- Initialize component
- Handle window lifecycle events
- Pass navigation to ViewModel

**Constraints:**
- No business logic
- No serial communication
- No data binding setup

**Max Lines:** ~20 lines

## 3. MainWindowViewModel - Navigation and Global State

**Responsibility:**
- Manage navigation between pages
- Track global connection status
- Handle window-level commands
- Coordinate between pages

**Properties:**
- CurrentPage (string/enum)
- IsConnected (bool)
- ConnectionStatus (string)
- CurrentPortName (string)
- CurrentBaudRate (int)

**Commands:**
- NavigateCommand
- ConnectCommand
- DisconnectCommand
- ExitCommand

**Forbidden:**
- Serial communication logic
- Terminal display logic
- Modbus protocol logic

## 4. Left Navigation Panel

**Items:**

| Item | Icon | Page | Description |
|------|------|------|-------------|
| Terminal | Terminal | TerminalPage | Serial communication terminal |
| Modbus | Cpu | ModbusPage | Modbus protocol debugging |
| Templates | FileCode | TemplatesPage | Message templates |
| Logs | FileText | LogsPage | Communication logs |
| Settings | Settings | SettingsPage | Application settings |

**Behavior:**
- Single selection
- Highlight active page
- Collapsible (optional)

## 5. Top Status Bar

**Components:**

| Component | Location | Data Source |
|-----------|----------|-------------|
| Connection Status | Center | MainWindowViewModel |
| Port Info | Left-Center | MainWindowViewModel |
| Baud Rate | Left-Center | MainWindowViewModel |
| Connection Indicator | Left-Center | MainWindowViewModel |
| Minimize Button | Right | System |
| Maximize Button | Right | System |
| Close Button | Right | System |

**Connection Status States:**
- Disconnected (Gray)
- Connecting (Yellow)
- Connected (Green)
- Error (Red)

## 6. Bottom Status Bar

**Components:**

| Component | Location | Data Source |
|-----------|----------|-------------|
| TX Counter | Left | TerminalViewModel |
| RX Counter | Center-Left | TerminalViewModel |
| Status Text | Center | MainWindowViewModel |
| Protocol | Center-Right | MainWindowViewModel |
| Timestamp | Right | System |

## 7. TerminalPage / TerminalViewModel

**Responsibility:**
- Serial port communication
- Send text/HEX data
- Display received data
- Handle send history
- Manage receive buffer

**Properties:**
- SendText
- SendMode (Text/Hex)
- SendLineEnding
- ReceivedText
- SendHistory
- SelectedSendHistoryItem
- CurrentDisplayBytes
- ReceivedBytesCount

**Commands:**
- SendCommand
- ClearCommand
- ClearSendHistoryCommand

**Forbidden:**
- Navigation logic
- Modbus protocol
- Global state management

## 8. ModbusPage / ModbusViewModel

**Responsibility:**
- Modbus RTU/TCP communication
- Register read/write operations
- Protocol-specific display
- Connection management for Modbus

**Properties:**
- SlaveAddress
- FunctionCode
- StartingAddress
- Quantity
- RegisterValue
- ResponseData
- IsConnected

**Commands:**
- ReadCommand
- WriteCommand
- ConnectCommand
- DisconnectCommand

**Forbidden:**
- Navigation logic
- Serial terminal logic
- Global state management

## 9. TemplatesPage / TemplatesViewModel

**Responsibility:**
- Manage message templates
- Create/edit/delete templates
- Template variables
- Apply templates

**Properties:**
- Templates (ObservableCollection)
- SelectedTemplate
- TemplateContent
- TemplateName

**Commands:**
- NewTemplateCommand
- SaveTemplateCommand
- DeleteTemplateCommand
- ApplyTemplateCommand

## 10. LogsPage / LogsViewModel

**Responsibility:**
- Display communication logs
- Filter logs
- Export logs
- Show statistics

**Properties:**
- LogEntries
- FilterText
- FilterType
- Statistics

**Commands:**
- ExportCommand
- ClearCommand
- FilterCommand

## 11. SettingsPage / SettingsViewModel

**Responsibility:**
- Application settings management
- Connection defaults
- Display preferences
- Save/load settings

**Properties:**
- DefaultBaudRate
- DefaultDataBits
- DefaultParity
- DefaultStopBits
- ShowTimestamp
- ShowDirection
- MaxDisplayBytes
- MaxSendHistoryCount

**Commands:**
- SaveCommand
- ResetCommand
- ImportCommand
- ExportCommand

## 12. Page Navigation Flow

```
User clicks Navigation Item
    ↓
MainWindowViewModel.NavigateCommand executed
    ↓
CurrentPage property updated
    ↓
Frame navigates to corresponding Page
    ↓
Page loads with ViewModel
    ↓
ViewModel connects to services
    ↓
UI updates with data
```

## 13. Data Flow Principles

### From User Input to Action
```
User input → Page ViewModel → Service Interface → Implementation
```

### From Service to UI
```
Service event → Page ViewModel → Property changed → UI update
```

### Global State
```
Global event → MainWindowViewModel → Property changed → All interested ViewModels
```

## 14. UI Design Principles

### Layout
- Responsive to window size
- Consistent spacing (8px grid)
- Proper padding and margins

### Typography
- Sans-serif font (Segoe UI)
- Clear hierarchy
- Readable font sizes

### Color
- Neutral background
- Clear status indicators
- Good contrast ratio

### Icons
- Consistent style
- Meaningful representation
- Appropriate size

### Interactions
- Clear feedback on actions
- Smooth transitions
- No unnecessary animations

## 16. Shell Implementation Plan

This section defines the detailed plan for implementing the UI Shell in Feature F1 and migrating existing functionality in Feature F2.

### 16.1 MainWindow.xaml Future Responsibilities

**Will Become:**
- Shell container for all UI elements
- Contains left navigation panel area
- Contains top connection status bar area
- Contains main content area (Frame for pages)
- Contains bottom status bar area

**Will NOT Contain:**
- Serial port specific business logic
- Modbus protocol logic
- Log management logic
- Template management logic
- Any page-specific data binding

### 16.2 MainWindow.xaml.cs Boundaries

**Current State:**
- Minimal code-behind with InitializeComponent
- No business logic

**Future State:**
- Remains minimal (~20 lines)
- No serial port, file, navigation, communication logic
- No new code-behind events unless explicitly documented in future phases

**Forbidden:**
```csharp
// The following patterns must NOT appear in MainWindow.xaml.cs:
void OnSendClicked(object sender, RoutedEventArgs e) { }     // FORBIDDEN
void OnPortChanged(object sender, SelectionChangedEventArgs e) { } // FORBIDDEN
void OnDataReceived(object sender, SerialDataReceivedEventArgs e) { } // FORBIDDEN
```

### 16.3 MainWindowViewModel Future Responsibilities

**Will ONLY Handle:**
- Global navigation state (CurrentPage)
- Current page selection
- Global connection status summary
- Navigation commands between pages

**Will NOT Handle:**
- Terminal display logic
- Modbus protocol operations
- Log management
- Settings persistence
- Send/Receive operations

**Page-Specific Logic Must Be In:**
- TerminalViewModel (for terminal operations)
- ModbusViewModel (for Modbus operations)
- LogsViewModel (for log operations)
- TemplatesViewModel (for template operations)
- SettingsViewModel (for settings operations)

### 16.4 Future Page Split

| Current Location | Future Location | Phase |
|------------------|-----------------|-------|
| MainWindowViewModel | TerminalViewModel | F2 |
| MainWindow.xaml | TerminalPage.xaml | F2 |
| Serial port logic | SerialPortService | F2 |
| Receive display | ReceiveDisplayViewModel | F2 |

### 16.5 Feature F1 Coding Boundaries

**Allowed:**
- Create shell layout (MainWindow.xaml with navigation)
- Create placeholder pages (TerminalPage.xaml, etc.)
- Create page ViewModels (empty shells)
- Implement navigation logic in MainWindowViewModel
- Add basic navigation commands

**Forbidden:**
- Migrating existing serial port functionality
- Implementing Modbus protocol
- Changing existing Feature A-D behavior
- Final visual styling
- Introducing third-party UI libraries
- Adding complex animations
- Creating theme systems

### 16.6 Feature F2 Coding Boundaries

**Allowed:**
- Migrate existing terminal logic to TerminalViewModel
- Move existing terminal XAML to TerminalPage.xaml
- Create ReceiveDisplayViewModel as needed
- Update MainWindowViewModel to coordinate with TerminalViewModel
- Preserve all Feature A-D behavior

**Migration Principles:**
1. Copy existing logic to new location
2. Test after each logical unit moved
3. Remove old code only after verification
4. Maintain all existing tests

**Feature A-D Behavior Requirements:**
| Feature | Required Behavior |
|---------|-------------------|
| A | Send line ending (None/CR/LF/CRLF) |
| B | TX/RX direction marking, timestamp |
| C | Receive buffer limit, MaxDisplayBytes |
| D | Send history, duplicate removal |

### 16.7 UI Style Boundaries

**Reference:** MQTTX modern workbench direction

**Current Phase (F1) Boundaries:**
- Structure only, no final styling
- No complex animations
- No theme system
- No advanced icon system
- No third-party control libraries

**Future Phase (N1) Boundaries:**
- Color scheme optimization
- Spacing and typography
- Corner radius consistency
- Icon system
- Light/dark theme support
- Status indicators

### 16.8 Migration Sequence

```
Phase F1: Build Shell
├── Create MainWindow.xaml with navigation shell
├── Create MainWindowViewModel with navigation logic
├── Create placeholder pages (TerminalPage, ModbusPage, etc.)
└── Verify navigation works

Phase F2: Migrate Terminal
├── Copy terminal logic from MainWindowViewModel to TerminalViewModel
├── Copy terminal XAML from MainWindow.xaml to TerminalPage.xaml
├── Update MainWindowViewModel to coordinate
├── Update tests
└── Verify Feature A-D still works

Future Phases: Add Features
├── G1-H1: Add Modbus functionality
├── J1: Add Templates functionality
├── K1: Add Logs functionality
├── L1: Add Settings page
└── N1: Add final styling
```

## 17. Feature F1 Implementation Notes

### 17.1 Shell Structure Implemented

**Status:** Shell skeleton established in MainWindow.xaml

**Current Structure:**
```
MainWindow.xaml
├── Top Status Bar (App title, Connection status, Version)
├── Main Content Area
│   ├── Left Navigation Panel (Terminal, Modbus, Templates, Logs, Settings)
│   └── Main Workspace (Current terminal functionality)
└── Bottom Status Bar (Ready status, Phase indicator, Tool description)
```

### 17.2 Terminal Still in MainWindow

**Important:** Terminal functionality remains in MainWindow for F1.

**Current State:**
- Serial port settings area: Present in MainWindow
- Receive area: Present in MainWindow
- Send area: Present in MainWindow
- Status bar: Present in MainWindow

**F2 Migration Plan:**
- Terminal functionality will be moved to TerminalPage.xaml
- Terminal logic will be moved to TerminalViewModel
- MainWindow will become pure Shell container

### 17.3 Navigation Panel State

**Current Implementation:**
- Static buttons (no command binding)
- Terminal button highlighted (current page)
- Other buttons disabled (placeholder)
- No navigation logic yet

**F2+ Enhancement:**
- Add navigation command binding
- Enable page switching
- Implement placeholder content for Modbus/Templates/Logs/Settings

### 17.4 Visual Style Notes

**Current Phase (F1):**
- Structural UI only
- Simple borders and backgrounds
- No complex styling
- No theme system
- No advanced icons

**Future Phase (N1):**
- Modern visual design
- Consistent color scheme
- Professional appearance
- Reference: MQTTX modern workbench direction

### 17.5 MainWindow.xaml.cs Status

**Maintained:**
- Minimal code-behind
- Only InitializeComponent
- No business logic added
- No event handlers added

---

## 18. Feature F2A Implementation Notes

### 18.1 Terminal UI Extracted to TerminalPage

**Status:** Terminal UI successfully extracted to TerminalPage.xaml

**New Files Created:**
- `src/SerialAssistant.App/Views/TerminalPage.xaml` - Terminal UI UserControl
- `src/SerialAssistant.App/Views/TerminalPage.xaml.cs` - Minimal code-behind

### 18.2 TerminalPage Reuses MainWindowViewModel

**Important:** TerminalPage does not have its own ViewModel yet.

**Current State:**
- TerminalPage inherits DataContext from MainWindow
- All Bindings remain unchanged
- SerialSettings, ReceiveDisplay, SendHistory still bound to MainWindowViewModel properties

**Benefits:**
- No ViewModel migration risk
- All Feature A-D behavior preserved
- No breaking changes to tests

### 18.3 MainWindow.xaml Simplified

**Changes:**
- Shell structure preserved (top bar, navigation, bottom bar)
- Main workspace now contains `<views:TerminalPage />`
- TerminalPage fills remaining space in column 1

**MainWindow.xaml Now Contains:**
```xml
<Window>
    <Grid>
        <!-- Top Status Bar -->
        <Border Grid.Row="0" />

        <!-- Main Content -->
        <Grid Grid.Row="1">
            <!-- Left Navigation Panel -->
            <Border Grid.Column="0" />

            <!-- Terminal Page -->
            <views:TerminalPage Grid.Column="1" />
        </Grid>

        <!-- Bottom Status Bar -->
        <Border Grid.Row="2" />
    </Grid>
</Window>
```

### 18.4 TerminalPage.xaml.cs Status

**Maintained:**
- Minimal code-behind (only InitializeComponent)
- No business logic
- No event handlers
- No serial port operations
- No file operations
- No navigation logic

### 18.5 F2B Migration Plan

**Phase F2B will:**
- Create TerminalViewModel
- Migrate terminal logic from MainWindowViewModel to TerminalViewModel
- Update TerminalPage to use TerminalViewModel
- Update MainWindowViewModel to manage navigation only
- Update tests accordingly

### 18.6 Feature A-D Behavior Preservation

| Feature | Status | Notes |
|---------|--------|-------|
| A | ✅ Preserved | Send line ending options (None/CR/LF/CRLF) |
| B | ✅ Preserved | TX/RX direction, timestamp, text/HEX display |
| C | ✅ Preserved | Receive buffer limit, MaxDisplayBytes |
| D | ✅ Preserved | Send history, duplicate removal, max count |

### 18.7 Visual Style Notes

**Current Phase (F2A):**
- No visual changes
- TerminalPage uses same styling as original
- No new styling introduced
- Focus on structural extraction only

---

## 19. Feature F2B1 Implementation Notes

### 19.1 TerminalViewModel Introduced

**Status:** TerminalViewModel successfully created and integrated

**New Files Created:**
- `src/SerialAssistant.App/ViewModels/TerminalViewModel.cs` - Terminal-specific ViewModel
- `src/SerialAssistant.Tests/ViewModels/TerminalViewModelTests.cs` - Unit tests

### 19.2 TerminalPage Now Binds to TerminalViewModel

**Binding Configuration:**
```xml
<views:TerminalPage Grid.Column="1" DataContext="{Binding Terminal}" />
```

TerminalPage now directly binds to MainWindowViewModel.Terminal property.

### 19.3 MainWindowViewModel Shell Transition

**Current State:**
- MainWindowViewModel now contains Terminal property
- All terminal logic migrated to TerminalViewModel
- MainWindowViewModel retains compatibility forwarding properties

**Compatibility Forwarding Properties:**
| Property | Forwarded To |
|----------|-------------|
| SerialSettings | Terminal.SerialSettings |
| ReceiveDisplay | Terminal.ReceiveDisplay |
| SendText | Terminal.SendText |
| SendModes | Terminal.SendModes |
| SelectedSendMode | Terminal.SelectedSendMode |
| SendLineEndings | Terminal.SendLineEndings |
| SelectedSendLineEnding | Terminal.SelectedSendLineEnding |
| ConnectionState | Terminal.ConnectionState |
| StatusMessage | Terminal.StatusMessage |
| SentBytesCount | Terminal.SentBytesCount |
| ConnectionButtonText | Terminal.ConnectionButtonText |
| All Commands | Terminal.*Command |
| SendHistory | Terminal.SendHistory |
| MaxSendHistoryCount | Terminal.MaxSendHistoryCount |
| SelectedSendHistoryItem | Terminal.SelectedSendHistoryItem |

### 19.4 F2B2 Cleanup Implemented

**Status:** Completed

**Changes Made:**
- Removed all compatibility forwarding properties from MainWindowViewModel
- MainWindowViewModel now only contains Terminal property and SaveSettings method
- MainWindow.xaml status bar bindings updated to use Terminal.*
- MainWindowViewModel is now a pure Shell ViewModel

**Updated MainWindow.xaml Status Bar Bindings:**
```xml
<TextBlock Text="{Binding Terminal.ConnectionState}" />
<TextBlock Text="{Binding Terminal.SerialSettings.SelectedPortName}" />
<TextBlock Text="{Binding Terminal.SerialSettings.SelectedBaudRate}" />
```

### 19.5 MainWindowViewModel Current State

**MainWindowViewModel.cs now contains only:**
```csharp
public TerminalViewModel Terminal { get; }
public OperationResult SaveSettings()
```

**No longer contains:**
- Terminal forwarding properties
- Serial settings forwarding
- Send/receive forwarding
- Command forwarding

### 19.6 F2B2 Further Cleanup Plan

**Status:** Not Required

**Rationale:**
- MainWindowViewModel is now clean Shell ViewModel
- Terminal logic is fully in TerminalViewModel
- No remaining forwarding properties to remove
- Terminal ViewModel Migration phase is complete

**Next Steps:**
- Proceed to Modbus planning or next feature phase
- Terminal ViewModel Migration (F2B) is complete

### 19.5 Feature A-D Behavior Preservation

| Feature | Status | Notes |
|---------|--------|-------|
| A | ✅ Preserved | Send line ending options via TerminalViewModel |
| B | ✅ Preserved | TX/RX direction, timestamp via TerminalViewModel |
| C | ✅ Preserved | Receive buffer limit via TerminalViewModel.ReceiveDisplay |
| D | ✅ Preserved | Send history via TerminalViewModel |

### 19.6 Test Coverage

**New Tests Added:**
- TerminalViewModelTests - 12 tests covering initialization, properties, commands

**Existing Tests:**
- All 291+ tests pass via compatibility forwarding

---

## 20. Terminal Migration Closure Review

### 20.1 Current Shell Structure

**MainWindow.xaml as Shell:**
```
MainWindow.xaml
├── Top Status Bar (App title, Connection status via Terminal.*, Version v0.3.3)
├── Main Content Area
│   ├── Left Navigation Panel (Terminal, Modbus, Templates, Logs, Settings)
│   └── Main Workspace
│       └── TerminalPage (current single real business page)
└── Bottom Status Bar (Ready status, Phase indicator)
```

**Key Characteristics:**
- MainWindow.xaml is purely a Shell container
- No terminal-specific bindings in MainWindow itself
- All terminal bindings go through TerminalPage → TerminalViewModel
- Status bar now binds to Terminal.ConnectionState, Terminal.SerialSettings.*

### 20.2 Current ViewModel Boundaries

| ViewModel | Responsibility | Location |
|-----------|----------------|----------|
| MainWindowViewModel | Shell state, navigation, global status summary | ViewModels/MainWindowViewModel.cs |
| TerminalViewModel | Terminal business logic (Feature A-D) | ViewModels/TerminalViewModel.cs |
| ReceiveDisplayViewModel | Receive display, buffer management | ViewModels/ReceiveDisplayViewModel.cs |
| SerialSettingsViewModel | Serial port settings | ViewModels/SerialSettingsViewModel.cs |

**MainWindowViewModel Current State:**
```csharp
public class MainWindowViewModel : BaseViewModel
{
    public TerminalViewModel Terminal { get; }
    public OperationResult SaveSettings() => Terminal.SaveSettings();
}
```

**Boundary Rule:**
Subsequent pages (Modbus, Templates, Logs, Settings) **MUST NOT** place business logic back into MainWindowViewModel.

### 20.3 Current Code-behind Boundaries

| File | Constraint | Current Status |
|------|------------|----------------|
| MainWindow.xaml.cs | Only InitializeComponent | ✅ Compliant |
| TerminalPage.xaml.cs | Only InitializeComponent | ✅ Compliant |

**All code-behind files must remain minimal.** Business logic belongs in ViewModels.

### 20.4 Feature A-D Migration Status

| Feature | Location | Status |
|---------|----------|--------|
| Feature A (Send Line Ending) | TerminalViewModel | ✅ Complete |
| Feature B (TX/RX Direction & Timestamp) | TerminalViewModel + ReceiveDisplayViewModel | ✅ Complete |
| Feature C (Receive Buffer Limit) | ReceiveDisplayViewModel | ✅ Complete |
| Feature D (Send History) | TerminalViewModel | ✅ Complete |

**All Feature A-D behavior is preserved in TerminalViewModel.**

### 20.5 Test Coverage Distribution

| Test Class | Test Count | Responsibility |
|------------|------------|----------------|
| TerminalViewModelTests | 120 | Feature A-D behavior, serial basics |
| MainWindowViewModelTests | 44 | Shell responsibilities only |
| Other tests | 156 | Infrastructure, Models, Commands |
| **Total** | **320** | |

### 20.6 Future Page Boundary Suggestions

| Future Page | ViewModel | Rule |
|-------------|-----------|------|
| ModbusPage | ModbusViewModel | Business logic in ModbusViewModel, not MainWindowViewModel |
| TemplatesPage | TemplatesViewModel | Business logic in TemplatesViewModel |
| LogsPage | LogsViewModel | Business logic in LogsViewModel |
| SettingsPage | SettingsViewModel | Business logic in SettingsViewModel |

**Principle:** Each page owns its ViewModel. MainWindowViewModel only coordinates navigation and global state.

### 20.7 Version and Tag Policy

- **Current UI Display:** v0.3.3
- **F2C Phase:** Documentation closure only, no code changes
- **No new tag recommended for F2C**
- **Next actual code/feature phase should update version appropriately

---

## 21. Modbus UI Planning

### 21.1 ModbusPage Position

**ModbusPage is a standalone page, NOT a part of TerminalPage.**

```
MainWindow.xaml
├── Top Status Bar
├── Main Content Area
│   ├── Left Navigation Panel
│   │   ├── Terminal (current)
│   │   ├── Modbus (future)
│   │   ├── Templates
│   │   ├── Logs
│   │   └── Settings
│   └── Main Workspace
│       ├── TerminalPage (current)
│       └── ModbusPage (future - G5)
└── Bottom Status Bar
```

### 21.2 ModbusViewModel Boundary

**ModbusViewModel is independent from MainWindowViewModel.**

```
MainWindowViewModel
├── Terminal (TerminalViewModel) - Shell-owned
└── Navigation state

ModbusViewModel (standalone)
├── Connection settings (RTU: port, TCP: IP:port)
├── Function code selection
├── Address/quantity inputs
├── Register values
├── Response display
└── Error handling
```

**CRITICAL:** Do NOT add Modbus controls to TerminalPage.
**CRITICAL:** Do NOT add Modbus business logic to MainWindowViewModel.

### 21.3 ModbusPage UI Structure

**Minimal UI (G5 scope):**

```
┌─────────────────────────────────────────────────────────┐
│ Connection Settings                                      │
│ ┌─────────────────┐ ┌───────────────┐ ┌────────────┐ │
│ │ Protocol (RTU)  │ │ Port / IP     │ │ Connect    │ │
│ └─────────────────┘ └───────────────┘ └────────────┘ │
├─────────────────────────────────────────────────────────┤
│ Request Parameters                                       │
│ ┌─────────────────┐ ┌───────────────┐ ┌────────────┐ │
│ │ Function Code   │ │ Starting Addr │ │ Quantity   │ │
│ └─────────────────┘ └───────────────┘ └────────────┘ │
│ ┌──────────────┐ ┌──────────────┐ ┌──────────────┐ │
│ │   Read       │ │   Write      │ │   Write...   │ │
│ └──────────────┘ └──────────────┘ └──────────────┘ │
├─────────────────────────────────────────────────────────┤
│ Response Display                                         │
│ ┌─────────────────────────────────────────────────────┐ │
│ │                                                     │ │
│ │ [Response data or error message]                   │ │
│ │                                                     │ │
│ └─────────────────────────────────────────────────────┘ │
└─────────────────────────────────────────────────────────┘
```

### 21.4 G0 Does NOT Implement UI Switch

**Current state (G0):**
- Terminal page is default and only active page
- Left navigation shows Modbus button (placeholder)
- No Modbus functionality implemented yet

**Future state (G5):**
- Left navigation switches between pages
- ModbusPage becomes active when Modbus selected
- TerminalPage becomes active when Terminal selected

### 21.5 Modbus UI Principles

**Do:**
- Create dedicated ModbusPage.xaml
- Create dedicated ModbusViewModel
- Keep Modbus UI minimal for G5
- Delegate protocol work to Core layer

**Don't:**
- Don't add Modbus controls to TerminalPage
- Don't add Modbus business logic to MainWindowViewModel
- Don't add Modbus controls to MainWindow.xaml
- Don't implement protocol in UI layer

### 21.6 Future Enhancements (Post-G6)

Future UI enhancements may include:
- Register table view
- Multiple simultaneous operations
- Coil/Input bit toggle
- Response charting
- Batch operations

These are out of scope for G5 minimal UI.

---

## 22. ModbusPage Current State (G6 Closure)

### 22.1 Current UI State

**Status:** ✅ G5 ModbusPage minimal UI complete

**Current UI Elements:**
- ✅ Terminal / Modbus page switching works
- ✅ ModbusPage with parameter area (Transport Mode, Request Kind, UnitId, TransactionId, StartAddress, Quantity)
- ✅ ModbusPage with request area (Build Request button, RequestHex display)
- ✅ ModbusPage with response area (ResponseHex input, Parse Response button, ParsedSummary display)
- ✅ ModbusPage with clear button
- ✅ Minimal, functional UI (not final visual style)

**Key Characteristics:**
1. **Minimal UI:** Just enough to be usable, no complex styling
2. **Direct Binding:** ModbusPage.xaml binds directly to ModbusViewModel
3. **Shell Navigation:** Switching between Terminal and Modbus works correctly
4. **No Communication:** Builds requests and parses responses only, no real serial/TCP communication
5. **Version v0.4.4:** UI displays v0.4.4 in status bar

### 22.2 ModbusPage UI Structure (Current)

```
ModbusPage.xaml
├── Title ("Modbus")
├── Parameter Area
│   ├── Transport Mode (ComboBox: RTU/TCP)
│   ├── Request Kind (ComboBox: Read Holding Registers, Read Input Registers, Write Single Register, Write Multiple Registers)
│   ├── UnitId Input
│   ├── TransactionId Input
│   ├── StartAddress Input
│   ├── Quantity Input
│   ├── SingleWriteValue Input
│   └── MultipleWriteValuesText TextBox
├── Action Buttons
│   ├── Build Request
│   ├── Parse Response
│   └── Clear
├── Request Area
│   └── RequestHex (read-only TextBox)
├── Response Area
│   ├── ResponseHex (input TextBox)
│   └── ParsedSummary (read-only TextBox)
└── Status Area
    └── StatusMessage display
```

### 22.3 Visual Style Status

**Current State:** Minimal, functional UI
- No final MQTTX-style visual design
- Basic Grid layout with spacing
- Simple GroupBoxes for organization
- Standard WPF controls (TextBox, Button, ComboBox)
- White background on ModbusPage to prevent Terminal from showing through

**Future Style Planning (H Phase):**
- Should be done after functional completion
- Should unify Terminal and Modbus page styles
- Should follow MQTTX modern workbench direction
- NOT recommended for G6 or immediate next phase

### 22.4 Navigation Behavior

**Verified:**
- ✅ App starts with TerminalPage visible by default
- ✅ Clicking Modbus button in left navigation shows ModbusPage
- ✅ Clicking Terminal button returns to TerminalPage
- ✅ Multiple round-trip switches work correctly
- ✅ No TerminalPage background leaking through ModbusPage

**MainWindowViewModel Navigation State:**
```csharp
public bool IsTerminalSelected { get; private set; } // Default: true
public bool IsModbusSelected { get; private set; } // Default: false
public ICommand ShowTerminalCommand { get; }
public ICommand ShowModbusCommand { get; }
```

### 22.5 Page Visibility Implementation

**MainWindow.xaml Current Structure:**
```xml
<Grid Grid.Column="1">
    <Grid Visibility="{Binding IsTerminalPageVisible, Converter={StaticResource BoolToVis}}">
        <views:TerminalPage DataContext="{Binding Terminal}" />
    </Grid>
    <Grid Visibility="{Binding IsModbusPageVisible, Converter={StaticResource BoolToVis}}">
        <views:ModbusPage DataContext="{Binding Modbus}" />
    </Grid>
</Grid>
```

**Key Design:**
- Outer Grid containers control Visibility
- Outer Grid uses MainWindowViewModel DataContext
- Inner UserControls set their own DataContext
- Terminal and Modbus pages are mutually exclusive

---

*Last updated: May 2026*
*Modbus UI Planning: May 2026*
*G6 Modbus Closure: May 2026*
