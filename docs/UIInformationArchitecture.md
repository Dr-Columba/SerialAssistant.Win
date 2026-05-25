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

## 15. Future Expansion

**Additional Pages (Future):**
- DashboardPage - Overview dashboard
- ScriptsPage - Automation scripts
- DevicesPage - Device management

**Additional Navigation Items:**
- Dashboard
- Scripts
- Devices

---

*Last updated: May 2026*
