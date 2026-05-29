# SerialAssistant.Win Manual Test Checklist

This document provides a step-by-step manual testing guide for SerialAssistant.Win.

## Prerequisites

- .NET 8 SDK
- Windows 10 or later
- (Optional) Serial port hardware or null modem emulator for real send/receive tests

## Test Steps

### 1. Application Startup

- [ ] **Step 1.1** Run the application:
  ```powershell
  dotnet run --project .\src\SerialAssistant.App\SerialAssistant.App.csproj
  ```
- [ ] **Step 1.2** Verify main window displays correctly
- [ ] **Step 1.3** Verify initial state (Disconnected, parameters at defaults)

### Feature F1: Application Shell Skeleton Verification

#### F1.1 Shell Structure Visibility

- [ ] **Step F1.1** Verify left navigation panel is visible
- [ ] **Step F1.2** Verify navigation items exist: Terminal, Modbus, Templates, Logs, Settings
- [ ] **Step F1.3** Verify Terminal button is highlighted/enabled (current active)
- [ ] **Step F1.4** Verify Modbus, Templates, Logs, Settings buttons are visible (may be disabled as placeholders)
- [ ] **Step F1.5** Verify top status bar is visible
- [ ] **Step F1.6** Verify top status bar shows "SerialAssistant.Win" text
- [ ] **Step F1.7** Verify top status bar shows version "v0.2.1"
- [ ] **Step F1.8** Verify bottom status bar is visible
- [ ] **Step F1.9** Verify bottom status bar shows "Ready" or similar status text
- [ ] **Step F1.10** Verify bottom status bar shows "Feature F1: Application Shell Skeleton"

#### F1.2 Existing Terminal Functionality Preservation

- [ ] **Step F1.11** Verify serial port settings panel is visible (COM port, baud rate, etc.)
- [ ] **Step F1.12** Verify receive area is visible with TX/RX display
- [ ] **Step F1.13** Verify send area is visible with text input and send button
- [ ] **Step F1.14** Verify "发送结尾" (Send Line Ending) dropdown is visible (Feature A)
- [ ] **Step F1.15** Verify "显示时间戳" checkbox is visible (Feature B)
- [ ] **Step F1.16** Verify "显示方向" checkbox is visible (Feature B)
- [ ] **Step F1.17** Verify "接收缓存" dropdown is visible (Feature C)
- [ ] **Step F1.18** Verify "发送历史" dropdown is visible (Feature D)
- [ ] **Step F1.19** Verify "清空历史" button is visible (Feature D)

#### F1.3 MainWindow.xaml.cs Boundary Check

- [ ] **Step F1.20** Open `src/SerialAssistant.App/MainWindow.xaml.cs`
- [ ] **Step F1.21** Verify file contains only `InitializeComponent()` call
- [ ] **Step F1.22** Verify no button click handlers exist
- [ ] **Step F1.23** Verify no navigation logic exists
- [ ] **Step F1.24** Verify no serial port logic exists
- [ ] **Step F1.25** Verify file length is minimal (~20 lines or less)

### Feature F2A: Terminal UI Extraction Verification

#### F2A.1 TerminalPage Visibility and Structure

- [ ] **Step F2A.1** Run application, verify Terminal page displays by default
- [ ] **Step F2A.2** Verify serial port settings panel is visible
- [ ] **Step F2A.3** Verify receive area is visible
- [ ] **Step F2A.4** Verify send area is visible
- [ ] **Step F2A.5** Verify status bar at bottom of Terminal page is visible

#### F2A.2 Feature A-D Controls Visibility

- [ ] **Step F2A.6** Verify "发送结尾" (Send Line Ending) dropdown is visible (Feature A)
- [ ] **Step F2A.7** Verify "显示时间戳" checkbox is visible (Feature B)
- [ ] **Step F2A.8** Verify "显示方向" checkbox is visible (Feature B)
- [ ] **Step F2A.9** Verify "接收缓存" dropdown is visible (Feature C)
- [ ] **Step F2A.10** Verify "发送历史" dropdown is visible (Feature D)
- [ ] **Step F2A.11** Verify "清空历史" button is visible (Feature D)
- [ ] **Step F2A.12** Verify "清空接收区" button is visible

#### F2A.3 Shell Structure Preservation

- [ ] **Step F2A.13** Verify left navigation panel is still visible
- [ ] **Step F2A.14** Verify top status bar is still visible
- [ ] **Step F2A.15** Verify bottom status bar shows "Feature F2A: Terminal Page Extraction"

#### F2A.4 Code-behind Boundary Check

- [ ] **Step F2A.16** Open `src/SerialAssistant.App/MainWindow.xaml.cs`
- [ ] **Step F2A.17** Verify file contains only `InitializeComponent()` call
- [ ] **Step F2A.18** Open `src/SerialAssistant.App/Views/TerminalPage.xaml.cs`
- [ ] **Step F2A.19** Verify file contains only `InitializeComponent()` call
- [ ] **Step F2A.20** Verify no business logic in either code-behind file

### Feature F2B1: TerminalViewModel Verification

#### F2B1.1 Terminal Functionality

- [ ] **Step F2B1.1** Run application, verify Terminal page displays by default
- [ ] **Step F2B1.2** Verify serial port settings panel is visible
- [ ] **Step F2B1.3** Verify refresh button is enabled
- [ ] **Step F2B1.4** Verify receive area is visible
- [ ] **Step F2B1.5** Verify send area is visible

#### F2B1.2 Feature A-D Controls

- [ ] **Step F2B1.6** Verify "发送结尾" dropdown is visible (Feature A)
- [ ] **Step F2B1.7** Verify "发送模式" dropdown is visible
- [ ] **Step F2B1.8** Verify "发送历史" dropdown is visible (Feature D)
- [ ] **Step F2B1.9** Verify "清空历史" button is visible (Feature D)
- [ ] **Step F2B1.10** Verify "接收缓存" dropdown is visible (Feature C)
- [ ] **Step F2B1.11** Verify "清空接收区" button is visible
- [ ] **Step F2B1.12** Verify connection button is visible

#### F2B1.3 Shell Structure

- [ ] **Step F2B1.13** Verify left navigation panel is visible
- [ ] **Step F2B1.14** Verify top status bar is visible
- [ ] **Step F2B1.15** Verify bottom status bar shows "Feature F2B1: TerminalViewModel"

#### F2B1.4 Code-behind Boundary

- [ ] **Step F2B1.16** Open `src/SerialAssistant.App/MainWindow.xaml.cs`
- [ ] **Step F2B1.17** Verify only `InitializeComponent()` is present
- [ ] **Step F2B1.18** Open `src/SerialAssistant.App/Views/TerminalPage.xaml.cs`
- [ ] **Step F2B1.19** Verify only `InitializeComponent()` is present

### Feature F2B2: MainWindowViewModel Terminal Cleanup Verification

#### F2B2.1 Shell Structure Verification

- [ ] **Step F2B2.1** Run application, verify Terminal page displays by default
- [ ] **Step F2B2.2** Verify top status bar shows connection state via Terminal.*
- [ ] **Step F2B2.3** Verify top status bar shows port name via Terminal.SerialSettings
- [ ] **Step F2B2.4** Verify top status bar shows baud rate via Terminal.SerialSettings
- [ ] **Step F2B2.5** Verify bottom status bar shows "Feature F2B2: MainWindowViewModel Terminal Cleanup"
- [ ] **Step F2B2.6** Verify version display shows "v0.3.3"

#### F2B2.2 Feature A-D Controls

- [ ] **Step F2B2.7** Verify "发送结尾" dropdown is visible (Feature A)
- [ ] **Step F2B2.8** Verify "发送模式" dropdown is visible
- [ ] **Step F2B2.9** Verify "发送历史" dropdown is visible (Feature D)
- [ ] **Step F2B2.10** Verify "清空历史" button is visible (Feature D)
- [ ] **Step F2B2.11** Verify "接收缓存" dropdown is visible (Feature C)
- [ ] **Step F2B2.12** Verify connection button is visible
- [ ] **Step F2B2.13** Verify serial port settings are visible

#### F2B2.3 Code-behind Boundary

- [ ] **Step F2B2.14** Open `src/SerialAssistant.App/MainWindow.xaml.cs`
- [ ] **Step F2B2.15** Verify only `InitializeComponent()` is present
- [ ] **Step F2B2.16** Open `src/SerialAssistant.App/Views/TerminalPage.xaml.cs`
- [ ] **Step F2B2.17** Verify only `InitializeComponent()` is present
- [ ] **Step F2B2.18** Open `src/SerialAssistant.App/ViewModels/MainWindowViewModel.cs`
- [ ] **Step F2B2.19** Verify only contains Terminal property and SaveSettings method
- [ ] **Step F2B2.20** Verify no forwarding properties exist

### Feature F2C: Shell and Terminal Migration Closure Verification

#### F2C.1 Application Startup

- [ ] **Step F2C.1** Run application
- [ ] **Step F2C.2** Verify application starts without errors
- [ ] **Step F2C.3** Verify shell layout displays correctly

#### F2C.2 Shell Structure Visibility

- [ ] **Step F2C.4** Verify left navigation panel is visible
- [ ] **Step F2C.5** Verify Terminal is the default displayed page
- [ ] **Step F2C.6** Verify top status bar is visible
- [ ] **Step F2C.7** Verify bottom status bar is visible

#### F2C.3 Version Display

- [ ] **Step F2C.8** Verify UI displays version "v0.3.3"

#### F2C.4 Feature A-D Controls Visibility

- [ ] **Step F2C.9** Verify serial port settings area is visible
- [ ] **Step F2C.10** Verify receive area is visible
- [ ] **Step F2C.11** Verify send area is visible
- [ ] **Step F2C.12** Verify "发送结尾" dropdown is visible (Feature A)
- [ ] **Step F2C.13** Verify "发送历史" dropdown is visible (Feature D)
- [ ] **Step F2C.14** Verify "接收缓存" dropdown is visible (Feature C)

#### F2C.5 Code-behind Boundary Check

- [ ] **Step F2C.15** Open `src/SerialAssistant.App/MainWindow.xaml.cs`
- [ ] **Step F2C.16** Verify only `InitializeComponent()` is present
- [ ] **Step F2C.17** Verify no business logic exists
- [ ] **Step F2C.18** Open `src/SerialAssistant.App/Views/TerminalPage.xaml.cs`
- [ ] **Step F2C.19** Verify only `InitializeComponent()` is present
- [ ] **Step F2C.20** Verify no business logic exists

#### F2C.6 Build and Test Verification

- [ ] **Step F2C.21** Run `dotnet build .\SerialAssistant.Win.sln -c Debug`
- [ ] **Step F2C.22** Verify build succeeds with 0 warnings, 0 errors
- [ ] **Step F2C.23** Run `dotnet test .\SerialAssistant.Win.sln -c Debug`
- [ ] **Step F2C.24** Verify test count shows 320 passed
- [ ] **Step F2C.25** Verify all tests pass

### Feature G0: Modbus Planning Verification

#### G0.1 Documentation Files

- [ ] **Step G0.1** Verify `docs/ModbusPlan.md` exists
- [ ] **Step G0.2** Verify ModbusPlan.md contains Purpose section
- [ ] **Step G0.3** Verify ModbusPlan.md contains Modbus Scope section
- [ ] **Step G0.4** Verify ModbusPlan.md contains Supported Function Codes Roadmap
- [ ] **Step G0.5** Verify ModbusPlan.md contains Core Layer Design
- [ ] **Step G0.6** Verify ModbusPlan.md contains RTU Frame Rules
- [ ] **Step G0.7** Verify ModbusPlan.md contains TCP Frame Rules
- [ ] **Step G0.8** Verify ModbusPlan.md contains Test Strategy section

#### G0.2 PhasePlan Updates

- [ ] **Step G0.9** Verify `docs/PhasePlan.md` contains G0 section
- [ ] **Step G0.10** Verify PhasePlan.md contains G1 section (Modbus Core Foundation)
- [ ] **Step G0.11** Verify PhasePlan.md contains G2 section (RTU Frames)
- [ ] **Step G0.12** Verify PhasePlan.md contains G3 section (TCP Frames)
- [ ] **Step G0.13** Verify PhasePlan.md contains G4 section (ModbusViewModel)
- [ ] **Step G0.14** Verify PhasePlan.md contains G5 section (ModbusPage UI)
- [ ] **Step G0.15** Verify PhasePlan.md contains G6 section (Documentation Closure)
- [ ] **Step G0.16** Verify each phase has scope, allowed/forbidden modifications

#### G0.3 Architecture Updates

- [ ] **Step G0.17** Verify `docs/Architecture.md` contains Modbus Architecture Planning section
- [ ] **Step G0.18** Verify Architecture.md specifies Core layer rules
- [ ] **Step G0.19** Verify Architecture.md specifies Infrastructure layer rules
- [ ] **Step G0.20** Verify Architecture.md specifies App layer rules
- [ ] **Step G0.21** Verify Architecture.md defines dependency direction

#### G0.4 UIInformationArchitecture Updates

- [ ] **Step G0.22** Verify `docs/UIInformationArchitecture.md` contains Modbus UI Planning section
- [ ] **Step G0.23** Verify UIInformationArchitecture.md defines ModbusPage position
- [ ] **Step G0.24** Verify UIInformationArchitecture.md defines ModbusViewModel boundary
- [ ] **Step G0.25** Verify UIInformationArchitecture.md specifies ModbusPage is independent

#### G0.5 Code Restriction Compliance

- [ ] **Step G0.26** Verify no changes to `src/` directory
- [ ] **Step G0.27** Verify no changes to `tests/` directory
- [ ] **Step G0.28** Verify no changes to `*.csproj` files
- [ ] **Step G0.29** Verify no changes to `*.sln` files
- [ ] **Step G0.30** Verify no changes to UI XAML files
- [ ] **Step G0.31** Verify no changes to version numbers

#### G0.6 Build and Test Baseline

- [ ] **Step G0.32** Run `dotnet build .\SerialAssistant.Win.sln -c Debug`
- [ ] **Step G0.33** Verify build succeeds with 0 warnings, 0 errors
- [ ] **Step G0.34** Run `dotnet test .\SerialAssistant.Win.sln -c Debug`
- [ ] **Step G0.35** Verify test count is still 320 passed
- [ ] **Step G0.36** Verify no new tests were added (G0 is documentation only)

### Feature G1: Modbus Core Foundation Verification

#### G1.1 Core Modbus Directory Structure

- [ ] **Step G1.1** Verify `src/SerialAssistant.Core/Modbus/` directory exists
- [ ] **Step G1.2** Verify `src/SerialAssistant.Core/Modbus/Common/` directory exists
- [ ] **Step G1.3** Verify `src/SerialAssistant.Core/Modbus/Models/` directory exists
- [ ] **Step G1.4** Verify `src/SerialAssistant.Core/Modbus/Utilities/` directory exists

#### G1.2 Core Modbus Files

- [ ] **Step G1.5** Verify `ModbusFunctionCode.cs` exists
- [ ] **Step G1.6** Verify `ModbusDataType.cs` exists
- [ ] **Step G1.7** Verify `ModbusRegisterValue.cs` exists
- [ ] **Step G1.8** Verify `ModbusCrc16.cs` exists

#### G1.3 Core Modbus Tests Directory

- [ ] **Step G1.9** Verify `src/SerialAssistant.Tests/Modbus/` directory exists
- [ ] **Step G1.10** Verify `src/SerialAssistant.Tests/Modbus/Common/` exists
- [ ] **Step G1.11** Verify `src/SerialAssistant.Tests/Modbus/Models/` exists
- [ ] **Step G1.12** Verify `src/SerialAssistant.Tests/Modbus/Utilities/` exists

#### G1.4 Core Modbus Tests Files

- [ ] **Step G1.13** Verify `ModbusFunctionCodeTests.cs` exists
- [ ] **Step G1.14** Verify `ModbusDataTypeTests.cs` exists
- [ ] **Step G1.15** Verify `ModbusRegisterValueTests.cs` exists
- [ ] **Step G1.16** Verify `ModbusCrc16Tests.cs` exists

#### G1.5 Layer Boundary Compliance

- [ ] **Step G1.17** Verify Core Modbus files have no `System.Windows` reference
- [ ] **Step G1.18** Verify Core Modbus files have no `System.IO.Ports` reference
- [ ] **Step G1.19** Verify Core Modbus files have no `File.` operations
- [ ] **Step G1.20** Verify Core Modbus files have no `Directory.` operations

#### G1.6 CRC16 Standard Test Vector

- [ ] **Step G1.21** Run `dotnet test .\SerialAssistant.Win.sln -c Debug --filter ModbusCrc16Tests`
- [ ] **Step G1.22** Verify CRC for `{ 0x01, 0x03, 0x00, 0x00, 0x00, 0x0A }` is `0xCDC5`
- [ ] **Step G1.23** Verify `AppendCrc` appends C5 CD (low byte first, high byte second)

#### G1.7 Build and Test Verification

- [ ] **Step G1.24** Run `dotnet build .\SerialAssistant.Win.sln -c Debug`
- [ ] **Step G1.25** Verify build succeeds with 0 warnings, 0 errors
- [ ] **Step G1.26** Run `dotnet test .\SerialAssistant.Win.sln -c Debug`
- [ ] **Step G1.27** Verify test count shows 354 passed (320 + 34 new)
- [ ] **Step G1.28** Verify all existing Terminal tests still pass

#### G1.8 Version Update Verification

- [ ] **Step G1.29** Open `src/SerialAssistant.App/MainWindow.xaml`
- [ ] **Step G1.30** Verify version text shows "v0.4.0"

#### G1.9 Application Startup

- [ ] **Step G1.31** Run application with `dotnet run --project src/SerialAssistant.App`
- [ ] **Step G1.32** Verify application starts without errors
- [ ] **Step G1.33** Verify UI displays version "v0.4.0"
- [ ] **Step G1.34** Verify Terminal page still works correctly

### Feature G3: Modbus TCP Frame Builder and Parser Verification

#### G3.1 Core Modbus TCP Files

- [ ] **Step G3.1** Verify `src/SerialAssistant.Core/Modbus/Tcp/MbapHeader.cs` exists
- [ ] **Step G3.2** Verify `src/SerialAssistant.Core/Modbus/Tcp/ModbusTcpFrame.cs` exists
- [ ] **Step G3.3** Verify `src/SerialAssistant.Core/Modbus/Tcp/ModbusTcpRequestBuilder.cs` exists
- [ ] **Step G3.4** Verify `src/SerialAssistant.Core/Modbus/Tcp/ModbusTcpResponseParser.cs` exists
- [ ] **Step G3.5** Verify `src/SerialAssistant.Core/Modbus/Tcp/ModbusTcpErrorCode.cs` exists
- [ ] **Step G3.6** Verify `src/SerialAssistant.Core/Modbus/Tcp/ModbusTcpParseResult.cs` exists

#### G3.2 Core Modbus TCP Tests

- [ ] **Step G3.7** Verify `src/SerialAssistant.Tests/Modbus/Tcp/MbapHeaderTests.cs` exists
- [ ] **Step G3.8** Verify `src/SerialAssistant.Tests/Modbus/Tcp/ModbusTcpFrameTests.cs` exists
- [ ] **Step G3.9** Verify `src/SerialAssistant.Tests/Modbus/Tcp/ModbusTcpRequestBuilderTests.cs` exists
- [ ] **Step G3.10** Verify `src/SerialAssistant.Tests/Modbus/Tcp/ModbusTcpResponseParserTests.cs` exists

#### G3.3 Request Builder Tests

- [ ] **Step G3.11** Run `dotnet test .\SerialAssistant.Win.sln -c Debug --filter ModbusTcpRequestBuilderTests`
- [ ] **Step G3.12** Verify 0x03 Read Holding Registers request builds correctly
- [ ] **Step G3.13** Verify 0x04 Read Input Registers request builds correctly
- [ ] **Step G3.14** Verify 0x06 Write Single Register request builds correctly
- [ ] **Step G3.15** Verify 0x10 Write Multiple Registers request builds correctly

#### G3.4 Response Parser Tests

- [ ] **Step G3.16** Run `dotnet test .\SerialAssistant.Win.sln -c Debug --filter ModbusTcpResponseParserTests`
- [ ] **Step G3.17** Verify MBAP Length validation works correctly
- [ ] **Step G3.18** Verify exception response parsing works correctly

#### G3.5 Layer Boundary Compliance

- [ ] **Step G3.19** Verify Core Modbus TCP files have no `System.Windows` reference
- [ ] **Step G3.20** Verify Core Modbus TCP files have no `System.IO.Ports` reference
- [ ] **Step G3.21** Verify Core Modbus TCP files have no `File.` operations
- [ ] **Step G3.22** Verify Core Modbus TCP files have no `Directory.` operations

#### G3.6 Build and Test Verification

- [ ] **Step G3.23** Run `dotnet build .\SerialAssistant.Win.sln -c Debug`
- [ ] **Step G3.24** Verify build succeeds with 0 warnings, 0 errors
- [ ] **Step G3.25** Run `dotnet test .\SerialAssistant.Win.sln -c Debug`
- [ ] **Step G3.26** Verify test count shows 440 passed
- [ ] **Step G3.27** Verify all tests pass

#### G3.7 Version Update Verification

- [ ] **Step G3.28** Open `src/SerialAssistant.App/MainWindow.xaml`
- [ ] **Step G3.29** Verify version text shows "v0.4.2"

#### G3.8 Application Startup

- [ ] **Step G3.30** Run application with `dotnet run --project src/SerialAssistant.App`
- [ ] **Step G3.31** Verify application starts without errors
- [ ] **Step G3.32** Verify UI displays version "v0.4.2"
- [ ] **Step G3.33** Verify Terminal page still works correctly

#### G3.9 Scope Control Verification

- [ ] **Step G3.34** Verify no changes to Infrastructure layer
- [ ] **Step G3.35** Verify `src/SerialAssistant.App/MainWindow.xaml.cs` unchanged
- [ ] **Step G3.36** Verify `src/SerialAssistant.App/Views/TerminalPage.xaml.cs` unchanged

### Feature G4: ModbusViewModel Minimal Workflow Verification

#### G4.1 Core ModbusViewModel Files

- [ ] **Step G4.1** Verify `src/SerialAssistant.App/ViewModels/ModbusTransportMode.cs` exists
- [ ] **Step G4.2** Verify `src/SerialAssistant.App/ViewModels/ModbusRequestKind.cs` exists
- [ ] **Step G4.3** Verify `src/SerialAssistant.App/ViewModels/ModbusViewModel.cs` exists
- [ ] **Step G4.4** Verify `src/SerialAssistant.Tests/ViewModels/ModbusViewModelTests.cs` exists

#### G4.2 Build and Test Verification

- [ ] **Step G4.5** Run `dotnet build .\SerialAssistant.Win.sln -c Debug`
- [ ] **Step G4.6** Verify build succeeds with 0 warnings, 0 errors
- [ ] **Step G4.7** Run `dotnet test .\SerialAssistant.Win.sln -c Debug`
- [ ] **Step G4.8** Verify test count shows 494 passed
- [ ] **Step G4.9** Verify all tests pass

#### G4.3 Request Building Tests

- [ ] **Step G4.10** Run `dotnet test .\SerialAssistant.Win.sln -c Debug --filter ModbusViewModelTests`
- [ ] **Step G4.11** Verify RTU request building tests pass
- [ ] **Step G4.12** Verify TCP request building tests pass

#### G4.4 Response Parsing Tests

- [ ] **Step G4.13** Verify RTU response parsing tests pass
- [ ] **Step G4.14** Verify TCP response parsing tests pass

#### G4.5 ClearCommand Tests

- [ ] **Step G4.15** Verify ClearCommand tests pass

#### G4.6 Version Update Verification

- [ ] **Step G4.16** Open `src/SerialAssistant.App/MainWindow.xaml`
- [ ] **Step G4.17** Verify version text shows "v0.4.3"

#### G4.7 Application Startup

- [ ] **Step G4.18** Run application with `dotnet run --project src/SerialAssistant.App`
- [ ] **Step G4.19** Verify application starts without errors
- [ ] **Step G4.20** Verify UI displays version "v0.4.3"
- [ ] **Step G4.21** Verify Terminal page still works correctly

#### G4.8 Scope Control Verification

- [ ] **Step G4.22** Verify no ModbusPage.xaml was added
- [ ] **Step G4.23** Verify no changes to Infrastructure layer
- [ ] **Step G4.24** Verify `src/SerialAssistant.App/MainWindow.xaml.cs` unchanged
- [ ] **Step G4.25** Verify `src/SerialAssistant.App/Views/TerminalPage.xaml.cs` unchanged

### 2. Serial Port Scanning

- [ ] **Step 2.1** Click "刷新" (Refresh) button
- [ ] **Step 2.2** Verify status message shows available ports
- [ ] **Step 2.3** If ports available, verify they appear in dropdown
- [ ] **Step 2.4** If no ports, verify application doesn't crash
- [ ] **Step 2.5** Verify dropdown shows "无可用端口" when no ports

### 3. Serial Port Configuration

- [ ] **Step 3.1** Select a port from dropdown (if available)
- [ ] **Step 3.2** Change baud rate to different value
- [ ] **Step 3.3** Change data bits to 7 or 8
- [ ] **Step 3.4** Change parity to None/Odd/Even
- [ ] **Step 3.5** Change stop bits to One/Two
- [ ] **Step 3.6** Verify all UI controls respond

### 4. Serial Port Open/Close (Hardware Required)

- [ ] **Step 4.1** Select valid port and parameters
- [ ] **Step 4.2** Click "打开" (Open) button
- [ ] **Step 4.3** Verify connection state changes to "已连接"
- [ ] **Step 4.4** Verify parameter controls are disabled
- [ ] **Step 4.5** Verify send controls are enabled
- [ ] **Step 4.6** Click "关闭" (Close) button
- [ ] **Step 4.7** Verify connection state changes to "未连接"
- [ ] **Step 4.8** Verify parameter controls are enabled
- [ ] **Step 4.9** Verify send controls are disabled

### 5. Serial Port Send (Hardware Required)

#### Text Mode Send
- [ ] **Step 5.1** Open serial port
- [ ] **Step 5.2** Select "文本" (Text) send mode
- [ ] **Step 5.3** Enter text (e.g., "Hello World")
- [ ] **Step 5.4** Click "发送" (Send) button
- [ ] **Step 5.5** Verify status shows success message
- [ ] **Step 5.6** Verify TX record appears in receive area

#### Send Line Ending (Feature A)
- [ ] **Step 5.7** Select "None" line ending
- [ ] **Step 5.8** Send "ABC", verify TX record shows exactly "ABC"
- [ ] **Step 5.9** Select "CR" line ending
- [ ] **Step 5.10** Send "ABC", verify TX record shows "ABC" + 0x0D
- [ ] **Step 5.11** Select "LF" line ending
- [ ] **Step 5.12** Send "ABC", verify TX record shows "ABC" + 0x0A
- [ ] **Step 5.13** Select "CRLF" line ending
- [ ] **Step 5.14** Send "ABC", verify TX record shows "ABC" + 0x0D 0x0A

#### HEX Mode Send
- [ ] **Step 5.15** Select "HEX" send mode
- [ ] **Step 5.16** Enter valid HEX (e.g., "41 42 43")
- [ ] **Step 5.17** Click "发送" (Send) button
- [ ] **Step 5.18** Verify status shows success message
- [ ] **Step 5.19** Verify TX record shows HEX bytes (not text)
- [ ] **Step 5.20** Set line ending to "CRLF" while in HEX mode
- [ ] **Step 5.21** Send "41 42 43"
- [ ] **Step 5.22** Verify TX record does NOT include 0x0D 0x0A (HEX mode ignores line ending)
- [ ] **Step 5.23** Try invalid HEX (e.g., "XX YY")
- [ ] **Step 5.24** Verify validation error shows

### 6. Serial Port Receive (Hardware Required)

#### Text Mode Receive
- [ ] **Step 6.1** Open serial port
- [ ] **Step 6.2** Set receive mode to "文本"
- [ ] **Step 6.3** Have remote end send text
- [ ] **Step 6.4** Verify RX record appears in receive area with "RX" prefix
- [ ] **Step 6.5** Verify receive count increases

#### HEX Mode Receive
- [ ] **Step 6.6** Set receive mode to "HEX"
- [ ] **Step 6.7** Have remote end send data
- [ ] **Step 6.8** Verify HEX appears in receive area with "RX" prefix
- [ ] **Step 6.9** Verify receive count increases

### 7. TX/RX Direction and Timestamp Display (Feature B)

#### Verify UI Controls Exist
- [ ] **Step 7.1** Verify "显示时间戳" (Show Timestamp) checkbox exists
- [ ] **Step 7.2** Verify "显示方向" (Show Direction) checkbox exists
- [ ] **Step 7.3** Verify both checkboxes are checked by default

#### Direction Marking
- [ ] **Step 7.4** Send some data
- [ ] **Step 7.5** Verify TX record shows "TX" prefix
- [ ] **Step 7.6** Receive some data
- [ ] **Step 7.7** Verify RX record shows "RX" prefix

#### Timestamp Display
- [ ] **Step 7.8** Verify timestamp format is [HH:mm:ss.fff]
- [ ] **Step 7.9** Uncheck "显示时间戳"
- [ ] **Step 7.10** Verify TX/RX records no longer show timestamps
- [ ] **Step 7.11** Check "显示时间戳" again
- [ ] **Step 7.12** Verify timestamps reappear

#### Hide Direction Marking
- [ ] **Step 7.13** Uncheck "显示方向"
- [ ] **Step 7.14** Verify TX/RX records show only data, no "TX" or "RX" prefix
- [ ] **Step 7.15** Send data, verify no "TX" prefix
- [ ] **Step 7.16** Receive data, verify no "RX" prefix

#### Display Format Examples
- [ ] **Step 7.17** Enable both "显示时间戳" and "显示方向"
- [ ] **Step 7.18** Send "ABC", verify display shows: `[HH:mm:ss.fff] TX ABC`
- [ ] **Step 7.19** Receive "OK", verify display shows: `[HH:mm:ss.fff] RX OK`

#### HEX Mode Display
- [ ] **Step 7.20** Switch to HEX display mode
- [ ] **Step 7.21** Verify TX/RX records reformatted to show HEX bytes
- [ ] **Step 7.22** Verify TX "ABC" shows as "TX 41 42 43"
- [ ] **Step 7.23** Verify RX "OK" shows as "RX 4F 4B"

### 8. Clear Receive Buffer

- [ ] **Step 8.1** Send and receive some data first
- [ ] **Step 8.2** Verify TX and RX records exist in receive area
- [ ] **Step 8.3** Click "清空接收区" button
- [ ] **Step 8.4** Verify TX/RX records are cleared
- [ ] **Step 8.5** Verify receive count is reset to 0

### 9. Configuration Persistence (Feature A & B)

#### Save Configuration
- [ ] **Step 9.1** Change serial parameters (baud rate, etc.)
- [ ] **Step 9.2** Change send mode
- [ ] **Step 9.3** Change receive mode
- [ ] **Step 9.4** Change send line ending (None/CR/LF/CRLF)
- [ ] **Step 9.5** Modify ShowTimestamp setting
- [ ] **Step 9.6** Modify ShowDirection setting
- [ ] **Step 9.7** Close application normally
- [ ] **Step 9.8** Verify %AppData%\SerialAssistant.Win\settings.json created
- [ ] **Step 9.9** Verify file contains valid JSON
- [ ] **Step 9.10** Verify JSON includes SendLineEnding, ShowTimestamp, ShowDirection

#### Load Configuration
- [ ] **Step 9.11** Re-open application
- [ ] **Step 9.12** Verify serial parameters loaded correctly
- [ ] **Step 9.13** Verify send mode restored
- [ ] **Step 9.14** Verify receive mode restored
- [ ] **Step 9.15** Verify SendLineEnding restored
- [ ] **Step 9.16** Verify ShowTimestamp setting restored
- [ ] **Step 9.17** Verify ShowDirection setting restored
- [ ] **Step 9.18** If port exists, verify it's selected

#### Old Config Missing Fields
- [ ] **Step 9.19** Close application
- [ ] **Step 9.20** Edit settings.json to remove SendLineEnding, ShowTimestamp, ShowDirection
- [ ] **Step 9.21** Re-open application
- [ ] **Step 9.22** Verify defaults used: SendLineEnding=None, ShowTimestamp=true, ShowDirection=true

#### Corrupted Config Fallback
- [ ] **Step 9.23** Close application
- [ ] **Step 9.24** Manually corrupt settings.json (e.g., just "{")
- [ ] **Step 9.25** Re-open application
- [ ] **Step 9.26** Verify application doesn't crash
- [ ] **Step 9.27** Verify default settings loaded

### 10. Receive Buffer Limit (Feature C)

#### Verify UI Control Exists
- [ ] **Step 10.1** Verify "接收缓存" (Receive Buffer) dropdown exists
- [ ] **Step 10.2** Verify options include 64 KiB, 256 KiB, 1 MiB, 4 MiB
- [ ] **Step 10.3** Verify default is 256 KiB

#### Buffer Trimming on Data Add
- [ ] **Step 10.4** Set buffer to 64 KiB
- [ ] **Step 10.5** Send/receive data until buffer exceeds 64 KiB
- [ ] **Step 10.6** Verify oldest records are trimmed
- [ ] **Step 10.7** Verify TX/RX direction and timestamps still work on remaining records
- [ ] **Step 10.8** Switch between text/HEX modes, verify only remaining records are redrawn

#### Single Large Record Preservation
- [ ] **Step 10.9** Clear receive buffer
- [ ] **Step 10.10** Set buffer to 64 KiB
- [ ] **Step 10.11** Send/receive single record larger than 64 KiB
- [ ] **Step 10.12** Verify record is preserved (not trimmed)

#### MaxDisplayBytes Change Behavior
- [ ] **Step 10.13** Fill buffer with multiple records at 256 KiB setting
- [ ] **Step 10.14** Change buffer to 64 KiB
- [ ] **Step 10.15** Verify old records are trimmed immediately
- [ ] **Step 10.16** Change buffer back to 256 KiB
- [ ] **Step 10.17** Verify trimmed records are NOT restored

#### Configuration Persistence
- [ ] **Step 10.18** Set buffer to 64 KiB
- [ ] **Step 10.19** Close application normally
- [ ] **Step 10.20** Re-open application
- [ ] **Step 10.21** Verify buffer size is restored to 64 KiB
- [ ] **Step 10.22** Set buffer to 1 MiB
- [ ] **Step 10.23** Close and re-open, verify restored to 1 MiB

#### Clear Behavior
- [ ] **Step 10.24** Send/receive data to fill buffer with some trimmed records
- [ ] **Step 10.25** Click "清空接收区" button
- [ ] **Step 10.26** Verify display is cleared
- [ ] **Step 10.27** Verify receive count is reset to 0

#### Old Config Missing MaxDisplayBytes
- [ ] **Step 10.28** Close application
- [ ] **Step 10.29** Edit settings.json to remove MaxDisplayBytes
- [ ] **Step 10.30** Re-open application
- [ ] **Step 10.31** Verify default 256 KiB is used

### 11. Send History (Feature D)

#### Verify UI Controls Exist
- [ ] **Step 11.1** Verify "发送历史" (Send History) dropdown exists
- [ ] **Step 11.2** Verify "清空历史" button exists
- [ ] **Step 11.3** Verify history dropdown is empty by default

#### Send History Recording
- [ ] **Step 11.4** Open serial port
- [ ] **Step 11.5** Send text "Hello" in Text mode
- [ ] **Step 11.6** Send "41 42 43" in HEX mode
- [ ] **Step 11.7** Verify history dropdown shows both records
- [ ] **Step 11.8** Verify latest record ("41 42 43") is at top (index 0)

#### Duplicate Detection
- [ ] **Step 11.9** Send "Hello" again in Text mode
- [ ] **Step 11.10** Verify only one "Hello" in history
- [ ] **Step 11.11** Verify "Hello" moved to top (most recent)
- [ ] **Step 11.12** Send "41 42 43" in Text mode
- [ ] **Step 11.13** Verify both "41 42 43" entries exist (different modes)

#### History Selection Backfill
- [ ] **Step 11.14** Select "Hello" from history dropdown
- [ ] **Step 11.15** Verify SendText is filled with "Hello"
- [ ] **Step 11.16** Verify send mode is restored to Text
- [ ] **Step 11.17** Select "41 42 43" HEX entry from dropdown
- [ ] **Step 11.18** Verify SendText is filled with "41 42 43"
- [ ] **Step 11.19** Verify send mode is restored to HEX

#### Selection Does Not Auto-Send
- [ ] **Step 11.20** Select any history item
- [ ] **Step 11.21** Verify no data is sent automatically
- [ ] **Step 11.22** Verify SelectedSendHistoryItem dropdown shows selection

#### Clear History
- [ ] **Step 11.23** Click "清空历史" button
- [ ] **Step 11.24** Verify history dropdown is empty
- [ ] **Step 11.25** Verify SendText is NOT cleared
- [ ] **Step 11.26** Verify receive area is NOT cleared

#### Configuration Persistence
- [ ] **Step 11.27** Send several history items
- [ ] **Step 11.28** Close application normally
- [ ] **Step 11.29** Re-open application
- [ ] **Step 11.30** Verify send history is restored
- [ ] **Step 11.31** Verify latest item is at top
- [ ] **Step 11.32** Click "清空历史"
- [ ] **Step 11.33** Close and re-open application
- [ ] **Step 11.34** Verify history is empty

#### Old Config Missing SendHistory
- [ ] **Step 11.35** Close application
- [ ] **Step 11.36** Edit settings.json to remove SendHistory and MaxSendHistoryCount
- [ ] **Step 11.37** Re-open application
- [ ] **Step 11.38** Verify default MaxSendHistoryCount (20) is used
- [ ] **Step 11.39** Verify empty history

#### Feature A/B/C Compatibility
- [ ] **Step 11.40** Verify Feature A (Send Line Ending) still works
- [ ] **Step 11.41** Verify Feature B (TX/RX Direction) still works
- [ ] **Step 11.42** Verify Feature C (Receive Buffer Limit) still works

### 12. Edge Cases

- [ ] **Step 12.1** Open non-existent port (should show error)
- [ ] **Step 12.2** Open already-open port (should show error)
- [ ] **Step 12.3** Close already-closed port (should show error)
- [ ] **Step 12.4** Send without opening port (should show error)
- [ ] **Step 12.5** Empty text send (should show error)
- [ ] **Step 12.6** Empty HEX send (should show error)

### 13. Cleanup

- [ ] **Step 13.1** Close any open serial ports
- [ ] **Step 13.2** Exit application
- [ ] **Step 13.3** (Optional) Delete test config file at %AppData%\SerialAssistant.Win\settings.json

### Feature G5: ModbusPage Minimal UI Verification

#### G5.1 ModbusPage Files

- [ ] **Step G5.1** Verify `src/SerialAssistant.App/Views/ModbusPage.xaml` exists
- [ ] **Step G5.2** Verify `src/SerialAssistant.App/Views/ModbusPage.xaml.cs` exists
- [ ] **Step G5.3** Verify `src/SerialAssistant.App/ViewModels/MainWindowViewModel.cs` has navigation properties
- [ ] **Step G5.4** Verify `src/SerialAssistant.App/ViewModels/ModbusViewModel.cs` has TransportModes and RequestKinds

#### G5.2 Build and Test Verification

- [ ] **Step G5.5** Run `dotnet build .\SerialAssistant.Win.sln -c Debug`
- [ ] **Step G5.6** Verify build succeeds with 0 warnings, 0 errors
- [ ] **Step G5.7** Run `dotnet test .\SerialAssistant.Win.sln -c Debug`
- [ ] **Step G5.8** Verify test count shows 520 passed (494 + 26 new)
- [ ] **Step G5.9** Verify all tests pass

#### G5.3 Application Startup and Navigation

- [ ] **Step G5.10** Run application with `dotnet run --project src/SerialAssistant.App`
- [ ] **Step G5.11** Verify application starts without errors
- [ ] **Step G5.12** Verify UI displays version "v0.4.4"
- [ ] **Step G5.13** Verify Terminal page is visible by default
- [ ] **Step G5.14** Verify "Modbus" button exists in left navigation
- [ ] **Step G5.15** Click "Modbus" button
- [ ] **Step G5.16** Verify ModbusPage becomes visible
- [ ] **Step G5.17** Click "Terminal" button
- [ ] **Step G5.18** Verify TerminalPage becomes visible again

#### G5.4 ModbusPage UI Elements

- [ ] **Step G5.19** Navigate to Modbus page
- [ ] **Step G5.20** Verify "Modbus" header is visible
- [ ] **Step G5.21** Verify TransportMode ComboBox exists (Rtu, Tcp)
- [ ] **Step G5.22** Verify RequestKind ComboBox exists (03, 04, 06, 10)
- [ ] **Step G5.23** Verify UnitId input exists
- [ ] **Step G5.24** Verify TransactionId input exists
- [ ] **Step G5.25** Verify StartAddress input exists
- [ ] **Step G5.26** Verify Quantity input exists
- [ ] **Step G5.27** Verify SingleWriteValue input exists
- [ ] **Step G5.28** Verify MultipleWriteValuesText TextBox exists (multiline)
- [ ] **Step G5.29** Verify "Build Request" button exists
- [ ] **Step G5.30** Verify "Parse Response" button exists
- [ ] **Step G5.31** Verify "Clear" button exists
- [ ] **Step G5.32** Verify RequestHex TextBox exists (readonly)
- [ ] **Step G5.33** Verify ResponseHex TextBox exists (input)
- [ ] **Step G5.34** Verify ParsedSummary TextBox exists (readonly)

#### G5.5 ModbusPage Functionality

- [ ] **Step G5.35** Set UnitId=1, StartAddress=0, Quantity=1
- [ ] **Step G5.36** Click "Build Request"
- [ ] **Step G5.37** Verify RequestHex shows HEX string (RTU)
- [ ] **Step G5.38** Enter valid response HEX in ResponseHex
- [ ] **Step G5.39** Click "Parse Response"
- [ ] **Step G5.40** Verify ParsedSummary shows result
- [ ] **Step G5.41** Click "Clear"
- [ ] **Step G5.42** Verify RequestHex, ResponseHex, ParsedSummary are cleared
- [ ] **Step G5.43** Switch to Tcp mode
- [ ] **Step G5.44** Click "Build Request"
- [ ] **Step G5.45** Verify TCP request HEX is generated

#### G5.6 Terminal Page Still Works

- [ ] **Step G5.46** Navigate back to Terminal page
- [ ] **Step G5.47** Verify Terminal page still loads correctly
- [ ] **Step G5.48** Verify all Terminal UI elements are present and work

#### G5.7 Scope Control Verification

- [ ] **Step G5.49** Verify no changes to Infrastructure layer
- [ ] **Step G5.50** Verify `src/SerialAssistant.App/MainWindow.xaml.cs` unchanged
- [ ] **Step G5.51** Verify `src/SerialAssistant.App/Views/TerminalPage.xaml.cs` unchanged
- [ ] **Step G5.52** Verify `src/SerialAssistant.App/ViewModels/TerminalViewModel.cs` unchanged
- [ ] **Step G5.53** Verify ModbusPage.xaml.cs contains only InitializeComponent
- [ ] **Step G5.54** Verify no business logic in ModbusPage.xaml.cs
- [ ] **Step G5.55** Verify MainWindowViewModel only contains navigation, no Modbus business logic

## Test Results Summary

| Category | Result | Notes |
|----------|--------|-------|
| Application Startup | ☐ Pass / ☐ Fail | |
| Serial Port Scanning | ☐ Pass / ☐ Fail | |
| TX/RX Direction Display | ☐ Pass / ☐ Fail | |
| Timestamp Display | ☐ Pass / ☐ Fail | |
| Configuration | ☐ Pass / ☐ Fail | |
| Open/Close | ☐ Pass / ☐ N/A (No HW) | |
| Text Send | ☐ Pass / ☐ N/A (No HW) | |
| HEX Send | ☐ Pass / ☐ N/A (No HW) | |
| Text Receive | ☐ Pass / ☐ N/A (No HW) | |
| HEX Receive | ☐ Pass / ☐ N/A (No HW) | |
| Clear Buffer | ☐ Pass / ☐ Fail | |
| Config Persistence | ☐ Pass / ☐ Fail | |
| Config Corruption | ☐ Pass / ☐ Fail | |
| Send History | ☐ Pass / ☐ Fail | |
| Edge Cases | ☐ Pass / ☐ Fail | |
| ModbusPage UI | ☐ Pass / ☐ Fail | |
| ModbusPage Navigation | ☐ Pass / ☐ Fail | |
| ModbusPage Functionality | ☐ Pass / ☐ Fail | |
| Terminal Still Works | ☐ Pass / ☐ Fail | |

---

## G6: Modbus Closure Manual Verification

### G6.1 Automated Verification

- [ ] **Step G6.1** Run `git diff --check` - Verify no trailing whitespace
- [ ] **Step G6.2** Run `dotnet build .\SerialAssistant.Win.sln -c Debug` - Verify build passes with 0 warnings, 0 errors
- [ ] **Step G6.3** Run `dotnet test .\SerialAssistant.Win.sln -c Debug` - Verify test count shows 520 passed (494 + 26 new)
- [ ] **Step G6.4** Check `git status` - Verify working tree is clean
- [ ] **Step G6.5** Verify `g5-navigation-debug.txt` does not exist

### G6.2 Terminal Page Verification

- [ ] **Step G6.6** Start app with `dotnet run --project src/SerialAssistant.App`
- [ ] **Step G6.7** Verify app starts without errors
- [ ] **Step G6.8** Verify Terminal page is visible by default
- [ ] **Step G6.9** Verify Terminal page shows serial port settings area
- [ ] **Step G6.10** Verify Terminal page shows receive area
- [ ] **Step G6.11** Verify Terminal page shows send area
- [ ] **Step G6.12** Verify Terminal page has no layout overlapping

### G6.3 Modbus Page Verification

- [ ] **Step G6.13** Click Modbus button in left navigation
- [ ] **Step G6.14** Verify ModbusPage becomes visible
- [ ] **Step G6.15** Click Terminal button in left navigation
- [ ] **Step G6.16** Verify TerminalPage becomes visible again
- [ ] **Step G6.17** Repeat Terminal → Modbus → Terminal multiple times - Verify all work correctly
- [ ] **Step G6.18** Verify ModbusPage has no Terminal background leaking through
- [ ] **Step G6.19** Verify ModbusPage parameter area displays correctly
- [ ] **Step G6.20** Click Build Request - Verify RequestHex shows HEX string
- [ ] **Step G6.21** Click Clear - Verify RequestHex, ResponseHex, ParsedSummary are cleared

### G6.4 Boundary Check Verification

- [ ] **Step G6.22** Verify `MainWindow.xaml.cs` contains only `InitializeComponent()` only
- [ ] **Step G6.23** Verify `TerminalPage.xaml.cs` contains only `InitializeComponent()` only
- [ ] **Step G6.24** Verify `ModbusPage.xaml.cs` contains only `InitializeComponent()` only
- [ ] **Step G6.25** Verify App ViewModels do not reference System.IO.Ports, TcpClient, Socket, Registry, File, Directory
- [ ] **Step G6.26** Verify Infrastructure does not reference System.Windows, Window, Dispatcher
- [ ] **Step G6.27** Verify Core does not reference System.Windows, Window, Dispatcher, System.IO.Ports

---

## G7: Modbus Transport Planning Verification

### G7.1 Documentation Verification

- [ ] **Step G7.1** Verify `docs/ModbusTransportPlan.md` exists and is complete
- [ ] **Step G7.2** Verify `docs/ModbusPlan.md` contains G7 Transport Planning Notes
- [ ] **Step G7.3** Verify `docs/PhasePlan.md` contains G7 status and G8-G12 phases
- [ ] **Step G7.4** Verify `docs/Architecture.md` contains Modbus Transport Architecture Planning
- [ ] **Step G7.5** Verify `docs/UIInformationArchitecture.md` contains Modbus Transport UI Planning
- [ ] **Step G7.6** Verify `docs/FinalReview.md` contains G7 review
- [ ] **Step G7.7** Verify `docs/FeatureReports/FeatureG7-ModbusTransportPlanning.md` exists

### G7.2 Scope Control Verification

- [ ] **Step G7.8** Verify NO modifications to `src/` directory
- [ ] **Step G7.9** Verify NO modifications to `tests/` directory
- [ ] **Step G7.10** Verify NO modifications to `*.csproj` or `*.sln`
- [ ] **Step G7.11** Verify NO modifications to UI files
- [ ] **Step G7.12** Verify NO version number changes
- [ ] **Step G7.13** Verify NO third-party libraries added

### G7.3 Build and Test Verification

- [ ] **Step G7.14** Run `git diff --check` - Verify no trailing whitespace
- [ ] **Step G7.15** Run `dotnet build .\SerialAssistant.Win.sln -c Debug` - Verify build passes with 0 errors, 0 warnings
- [ ] **Step G7.16** Run `dotnet test .\SerialAssistant.Win.sln -c Debug` - Verify test count shows 520 passed (unchanged)

### G7.4 Planning Content Verification

- [ ] **Step G7.17** Verify ModbusTransportPlan.md contains Purpose section
- [ ] **Step G7.18** Verify ModbusTransportPlan.md contains Current State section
- [ ] **Step G7.19** Verify ModbusTransportPlan.md contains Target Capability section
- [ ] **Step G7.20** Verify ModbusTransportPlan.md contains Layer Boundary Design section
- [ ] **Step G7.21** Verify ModbusTransportPlan.md contains Proposed Interfaces section
- [ ] **Step G7.22** Verify ModbusTransportPlan.md contains RTU Transport Plan section
- [ ] **Step G7.23** Verify ModbusTransportPlan.md contains TCP Transport Plan section
- [ ] **Step G7.24** Verify ModbusTransportPlan.md contains ModbusViewModel Integration Plan section
- [ ] **Step G7.25** Verify ModbusTransportPlan.md contains UI Change Plan section
- [ ] **Step G7.26** Verify ModbusTransportPlan.md contains Concurrency and Ownership section
- [ ] **Step G7.27** Verify ModbusTransportPlan.md contains Error Strategy section
- [ ] **Step G7.28** Verify ModbusTransportPlan.md contains Test Strategy section
- [ ] **Step G7.29** Verify ModbusTransportPlan.md contains Phase Breakdown section (G8-G12)
- [ ] **Step G7.30** Verify ModbusTransportPlan.md contains Risks and Decisions section
- [ ] **Step G7.31** Verify ModbusTransportPlan.md contains Final Recommendation section (recommend G8 first)

### G7.5 Architecture Boundary Verification

- [ ] **Step G7.32** Verify planning clearly states App layer must NOT reference System.IO.Ports
- [ ] **Step G7.33** Verify planning clearly states App layer must NOT reference TcpClient/Socket
- [ ] **Step G7.34** Verify planning clearly states Core layer must NOT reference Infrastructure
- [ ] **Step G7.35** Verify planning clearly states Infrastructure layer must NOT reference WPF
- [ ] **Step G7.36** Verify planning defines serial port ownership strategy
- [ ] **Step G7.37** Verify planning recommends G8 first (Interfaces + Fake Tests) before real hardware

---

## G8A: Modbus Transport Contracts Verification

### G8A.1 Core Transport Files Verification

- [ ] **Step G8A.1** Verify `src/SerialAssistant.Core/Modbus/Transport/IModbusTransport.cs` exists
- [ ] **Step G8A.2** Verify `src/SerialAssistant.Core/Modbus/Transport/IModbusRtuTransport.cs` exists
- [ ] **Step G8A.3** Verify `src/SerialAssistant.Core/Modbus/Transport/IModbusTcpTransport.cs` exists
- [ ] **Step G8A.4** Verify `src/SerialAssistant.Core/Modbus/Transport/ModbusTransportResult.cs` exists
- [ ] **Step G8A.5** Verify `src/SerialAssistant.Core/Modbus/Transport/ModbusTransportOptions.cs` exists
- [ ] **Step G8A.6** Verify `src/SerialAssistant.Core/Modbus/Transport/ModbusRequestContext.cs` exists
- [ ] **Step G8A.7** Verify `src/SerialAssistant.Core/Modbus/Transport/ModbusTransportErrorCode.cs` exists

### G8A.2 Test Files Verification

- [ ] **Step G8A.8** Verify `src/SerialAssistant.Tests/Modbus/Transport/FakeModbusTransport.cs` exists
- [ ] **Step G8A.9** Verify `src/SerialAssistant.Tests/Modbus/Transport/ModbusTransportOptionsTests.cs` exists
- [ ] **Step G8A.10** Verify `src/SerialAssistant.Tests/Modbus/Transport/ModbusRequestContextTests.cs` exists
- [ ] **Step G8A.11** Verify `src/SerialAssistant.Tests/Modbus/Transport/ModbusTransportResultTests.cs` exists
- [ ] **Step G8A.12** Verify `src/SerialAssistant.Tests/Modbus/Transport/FakeModbusTransportTests.cs` exists

### G8A.3 Scope Control Verification

- [ ] **Step G8A.13** Verify NO modifications to `src/SerialAssistant.Infrastructure/`
- [ ] **Step G8A.14** Verify NO modifications to `src/SerialAssistant.App/ViewModels/ModbusViewModel.cs`
- [ ] **Step G8A.15** Verify NO modifications to `src/SerialAssistant.App/Views/ModbusPage.xaml`
- [ ] **Step G8A.16** Verify MainWindow.xaml version updated to v0.4.5

### G8A.4 Layer Boundary Verification

- [ ] **Step G8A.17** Verify Core transport files do NOT contain System.IO.Ports
- [ ] **Step G8A.18** Verify Core transport files do NOT contain TcpClient
- [ ] **Step G8A.19** Verify Core transport files do NOT contain Socket
- [ ] **Step G8A.20** Verify Core transport files do NOT contain System.Windows
- [ ] **Step G8A.21** Verify App ViewModels do NOT contain System.IO.Ports
- [ ] **Step G8A.22** Verify App ViewModels do NOT contain TcpClient/Socket

### G8A.5 Build and Test Verification

- [ ] **Step G8A.23** Run `git diff --check` - Verify no trailing whitespace
- [ ] **Step G8A.24** Run `dotnet build .\SerialAssistant.Win.sln -c Debug` - Verify build passes
- [ ] **Step G8A.25** Run `dotnet test .\SerialAssistant.Win.sln -c Debug` - Verify test count shows 560 passed (40 new tests)

### G8A.6 Application Verification

- [ ] **Step G8A.26** Run application - Verify app starts normally
- [ ] **Step G8A.27** Verify version display shows v0.4.5 in title bar

---

## G8B: ModbusViewModel Transport Injection Verification

### G8B.1 ViewModel Changes Verification

- [ ] **Step G8B.1** Verify ModbusViewModel.cs has constructor with IModbusTransport parameter
- [ ] **Step G8B.2** Verify ModbusViewModel.cs has ConnectTransportAsync method
- [ ] **Step G8B.3** Verify ModbusViewModel.cs has DisconnectTransportAsync method
- [ ] **Step G8B.4** Verify ModbusViewModel.cs has SendRequestAsync method
- [ ] **Step G8B.5** Verify ModbusViewModel.cs has IsConnected property
- [ ] **Step G8B.6** Verify ModbusViewModel.cs has IsBusy property
- [ ] **Step G8B.7** Verify ModbusViewModel.cs has LastTransportError property

### G8B.2 Test Files Verification

- [ ] **Step G8B.8** Verify `src/SerialAssistant.Tests/ViewModels/ModbusViewModelTransportTests.cs` exists
- [ ] **Step G8B.9** Verify test file contains Connect/Disconnect/SendRequest tests

### G8B.3 Scope Control Verification

- [ ] **Step G8B.10** Verify NO modifications to `src/SerialAssistant.Infrastructure/`
- [ ] **Step G8B.11** Verify NO modifications to ModbusPage.xaml
- [ ] **Step G8B.12** Verify MainWindow.xaml version updated to v0.4.6

### G8B.4 Layer Boundary Verification

- [ ] **Step G8B.13** Verify ModbusViewModel.cs does NOT contain System.IO.Ports
- [ ] **Step G8B.14** Verify ModbusViewModel.cs does NOT contain TcpClient
- [ ] **Step G8B.15** Verify ModbusViewModel.cs does NOT contain Socket

### G8B.5 Build and Test Verification

- [ ] **Step G8B.16** Run `git diff --check` - Verify no trailing whitespace
- [ ] **Step G8B.17** Run `dotnet build .\SerialAssistant.Win.sln -c Debug` - Verify build passes
- [ ] **Step G8B.18** Run `dotnet test .\SerialAssistant.Win.sln -c Debug` - Verify test count shows 586 passed

### G8B.6 Application Verification

- [ ] **Step G8B.19** Run application - Verify app starts normally
- [ ] **Step G8B.20** Verify version display shows v0.4.6 in title bar

---

## G9A: Modbus RTU Transport Capability Review Verification

### G9A Overview

G9A is a **documentation-only phase** for reviewing existing serial port service capabilities for Modbus RTU transport implementation. No code changes are made in this phase.

### G9A.1 Phase Type Verification

- [ ] **Step G9A.1** Verify G9A is documented as a documentation-only phase
- [ ] **Step G9A.2** Verify no `src/` modifications occurred
- [ ] **Step G9A.3** Verify no `src/SerialAssistant.Tests/` modifications occurred

### G9A.2 Build and Test Verification

- [ ] **Step G9A.4** Run `git diff --check` - Verify no trailing whitespace
- [ ] **Step G9A.5** Run `dotnet build .\SerialAssistant.Win.sln -c Debug` - Verify build passes with 0 errors
- [ ] **Step G9A.6** Run `dotnet test .\SerialAssistant.Win.sln -c Debug` - Verify test count shows **586 passed** (unchanged)
- [ ] **Step G9A.7** Verify no new tests were added (G9A is documentation only)

### G9A.3 Source Code Diff Verification

- [ ] **Step G9A.8** Run `git diff --name-only -- src/` - Verify result is empty
- [ ] **Step G9A.9** Run `git diff --name-only -- src/SerialAssistant.Tests/` - Verify result is empty

### G9A.4 G9A Report Verification

- [ ] **Step G9A.10** Verify `docs/FeatureReports/FeatureG9A-ModbusRtuTransportReview.md` exists
- [ ] **Step G9A.11** Verify report documents existing ISerialPortService capabilities
- [ ] **Step G9A.12** Verify report documents SerialPortService gaps:
  - No SendAndReceiveAsync
  - No per-request timeout control
  - No CancellationToken support
  - No port ownership tracking
- [ ] **Step G9A.13** Verify report **explicitly recommends Option C** (not Option 1)
- [ ] **Step G9A.14** Verify report plans G9B/G9C/G9D phases

### G9A.5 Strategy Conflict Resolution Verification

- [ ] **Step G9A.15** Verify `docs/ModbusTransportPlan.md` no longer recommends Option 1
- [ ] **Step G9A.16** Verify ModbusTransportPlan.md marks Option 1 as "superseded"
- [ ] **Step G9A.17** Verify ModbusTransportPlan.md documents Option C as current recommendation
- [ ] **Step G9A.18** Verify ModbusTransportPlan.md does NOT contain "Start with Option 1 for G9"

### G9A.6 Architecture Documentation Verification

- [ ] **Step G9A.19** Verify `docs/Architecture.md` contains G9A capability review
- [ ] **Step G9A.20** Verify Architecture.md documents:
  - Current ISerialPortService limitations
  - Why direct reuse is NOT recommended
  - Option C architecture
  - G9B/G9C/G9D phase plan
  - Layer boundary rules (App forbidden System.IO.Ports)

### G9A.7 PhasePlan Update Verification

- [ ] **Step G9A.21** Verify `docs/PhasePlan.md` contains G9A section
- [ ] **Step G9A.22** Verify PhasePlan.md marks G9A as "Completed"
- [ ] **Step G9A.23** Verify PhasePlan.md contains G9B/G9C/G9D sections

### G9A.8 FinalReview Update Verification

- [ ] **Step G9A.24** Verify `docs/FinalReview.md` contains G9A review
- [ ] **Step G9A.25** Verify FinalReview.md documents:
  - Current still no real RTU communication
  - SerialPortService review completed
  - Option C recommended
  - G9B recommended as next phase

### G9A.9 Key Findings Documentation

- [ ] **Step G9A.26** Verify G9A report documents ISerialPortService limitations:
  - Event-based receive only (DataReceived)
  - No SendAndReceiveAsync method
  - No request-response pattern support
  - No per-request timeout
  - No CancellationToken support
  - No port ownership tracking
- [ ] **Step G9A.27** Verify G9A report documents recommendation rationale:
  - Option C avoids breaking Terminal behavior
  - New ModbusRtuTransport isolates Modbus logic
  - New Ownership Coordinator enables conflict prevention
  - Fake serial adapter enables testing

### G9A.10 Boundary Rules Verification

- [ ] **Step G9A.28** Verify all documentation states App layer is forbidden System.IO.Ports
- [ ] **Step G9A.29** Verify all documentation states Infrastructure CAN use System.IO.Ports
- [ ] **Step G9A.30** Verify all documentation states Core MUST NOT reference Infrastructure
- [ ] **Step G9A.31** Verify all documentation states Terminal and Modbus ownership must be managed

---

## G9B: Serial Port Ownership Coordinator Contracts

### G9B Overview

G9B is a code phase that adds Core ownership contracts and tests, with no Infrastructure or App logic changes.

### G9B.1 Core Contracts Verification

- [ ] **Step G9B.1** Verify `SerialPortOwner` enum exists in Core/Services
- [ ] **Step G9B.2** Verify `SerialPortOwner` has values: None, Terminal, ModbusRtu
- [ ] **Step G9B.3** Verify `ISerialPortOwnershipCoordinator` interface exists in Core/Services
- [ ] **Step G9B.4** Verify `SerialPortOwnershipChangedEventArgs` exists in Core/Services
- [ ] **Step G9B.5** Verify Core files have NO references to System.IO.Ports
- [ ] **Step G9B.6** Verify Core files have NO references to Infrastructure
- [ ] **Step G9B.7** Verify Core files have NO references to WPF

### G9B.2 Tests Verification

- [ ] **Step G9B.8** Verify `FakeSerialPortOwnershipCoordinator` exists in Tests/Infrastructure
- [ ] **Step G9B.9** Verify `SerialPortOwnerTests` exists in Tests/Services
- [ ] **Step G9B.10** Verify `SerialPortOwnershipChangedEventArgsTests` exists in Tests/Services
- [ ] **Step G9B.11** Verify `FakeSerialPortOwnershipCoordinatorTests` exists in Tests/Services
- [ ] **Step G9B.12** Verify test count increased from 586 to 618

### G9B.3 No Infrastructure Changes

- [ ] **Step G9B.13** Verify `src/SerialAssistant.Infrastructure/` has NO changes
- [ ] **Step G9B.14** Verify NO real ownership coordinator implemented
- [ ] **Step G9B.15** Verify NO ModbusRtuTransport implemented

### G9B.4 No App Logic Changes

- [ ] **Step G9B.16** Verify `MainWindowViewModel` has NO changes
- [ ] **Step G9B.17** Verify `TerminalViewModel` has NO changes
- [ ] **Step G9B.18** Verify `ModbusViewModel` has NO changes
- [ ] **Step G9B.19** Verify NO ownership logic added to App layer

### G9B.5 Version Verification

- [ ] **Step G9B.20** Verify MainWindow.xaml version shows v0.4.7

### G9B.6 Build and Test

- [ ] **Step G9B.21** Run `git diff --check` - Verify no trailing whitespace
- [ ] **Step G9B.22** Run `dotnet build` - Verify build passes with 0 warnings
- [ ] **Step G9B.23** Run `dotnet test` - Verify all 618 tests pass

---

## Tester Information

- **Tester Name**: _________________________
- **Test Date**: _________________________
- **Additional Notes**: _________________________
