# Phase Plan

## Overview

This document outlines the phased development plan for SerialAssistant.Win, organized into features with clear scope, boundaries, and acceptance criteria.

## Current Status

**Current Tag:** v0.2.1 (Features A-E1 Completed)

**Completed Features:**
- Feature A: Send Line Ending Options
- Feature B: TX/RX Direction Marking and Timestamp Display
- Feature C: Receive Buffer Limit and Configuration
- Feature D: Send History with UI and Persistence
- Feature E1: Product Vision and Phase Roadmap Realignment

**Current Phase:**
- Feature F1: Application Shell Skeleton (Completed)

**Completed Features:**
- Feature A: Send Line Ending Options
- Feature B: TX/RX Direction Marking and Timestamp Display
- Feature C: Receive Buffer Limit and Configuration
- Feature D: Send History with UI and Persistence
- Feature E1: Product Vision and Phase Roadmap Realignment
- Feature E2: UI Information Architecture Detailed Design
- Feature F1: Application Shell Skeleton

## Phase Roadmap

### Feature E1: Product Vision and Phase Roadmap Realignment

**Goal:** Define product positioning and restructure phase plan

**Scope:**
- Update README.md with project positioning
- Create ProductVision.md
- Create PhasePlan.md
- Create UIInformationArchitecture.md
- Create ReleaseStrategy.md
- Create FeatureE1 Report

**Forbidden:**
- Modifying source code
- Modifying test code
- Changing existing functionality

**Acceptance Criteria:**
- All documentation files created
- Project positioning clearly defined
- Phase plan documented
- No code changes

**Code Changes Allowed:** No

**Tests Required:** No

**Report Required:** Yes

---

### Feature E2: UI Information Architecture Detailed Design

**Goal:** Design detailed UI architecture before implementation, create implementation plan for F1 and F2

**Scope:**
- Define left navigation structure
- Define top connection status bar
- Define main workspace layout
- Define bottom status bar
- Define page structure (Terminal, Modbus, Templates, Logs, Settings)
- Create Shell Implementation Plan section in UIInformationArchitecture.md
- Update PhasePlan.md with refined F1 and F2 descriptions

**Forbidden:**
- Implementing UI code
- Modifying existing views
- Changing functionality

**Acceptance Criteria:**
- UI architecture fully documented
- Navigation structure defined
- Page responsibilities clear
- F1 and F2 implementation plans documented
- Shell Implementation Plan added to UIInformationArchitecture.md

**Code Changes Allowed:** No

**Tests Required:** No

**Manual UI Verification Required:** No (documentation-only phase)

**Report Required:** Yes

---

### Feature F1: Application Shell Skeleton

**Status:** ✅ Completed

**Goal:** Build the main application shell with navigation, placeholder pages, and basic navigation logic

**Scope:**
- Create MainWindow.xaml as Shell container with navigation structure
- Add left navigation panel (Terminal, Modbus, Templates, Logs, Settings)
- Add top status bar (app name, connection status, version)
- Add bottom status bar (ready status, phase indicator)
- Wrap existing terminal functionality in main workspace (non-intrusive)
- Keep existing terminal functionality in MainWindow (not migrated yet)

**Allowed Modifications:**
- src/SerialAssistant.App/MainWindow.xaml (shell structure only)
- src/SerialAssistant.App/MainWindow.xaml.cs (no changes allowed, must remain minimal)
- docs/UIInformationArchitecture.md (F1 implementation notes)
- docs/PhasePlan.md (status update)
- docs/ManualTestChecklist.md (F1 verification steps)
- docs/FeatureReports/FeatureF1-ApplicationShellSkeleton.md (new report)

**Forbidden:**
- Migrating existing serial terminal functionality to separate pages
- Creating separate page UserControls in this phase
- Implementing Modbus protocol
- Changing existing Feature A-D behavior
- Final visual styling
- Introducing third-party UI libraries
- Adding complex animations
- Creating theme systems
- Adding business logic to MainWindow.xaml.cs

**Implementation Notes:**
- F1 uses non-intrusive shell wrapping approach
- Existing terminal UI remains in MainWindow.xaml main workspace
- Navigation buttons are static (no command binding) to minimize risk
- Placeholder pages will be created in F2 when migrating terminal functionality
- MainWindow.xaml.cs remains unchanged (only InitializeComponent)

**Acceptance Criteria:**
- Shell UI renders correctly with navigation panel
- Top status bar visible with app name and version
- Bottom status bar visible with status text
- Existing terminal functionality fully preserved and visible
- MainWindow.xaml.cs remains minimal (~20 lines)
- All 291 existing tests pass
- No serial port functionality moved yet

**Code Changes Allowed:** Yes (Shell UI structure only)

**Tests Required:** No (if only XAML changes, no new logic)

**Manual UI Verification Required:** Yes

**ValidationGate Compliance:** Required

---

### Feature F2A: Terminal UI Extraction

**Status:** ✅ Completed

**Goal:** Extract terminal UI from MainWindow.xaml to TerminalPage.xaml without ViewModel migration

**Scope:**
- Create TerminalPage.xaml UserControl with terminal UI
- Create TerminalPage.xaml.cs with minimal code-behind
- Update MainWindow.xaml to use TerminalPage
- TerminalPage reuses MainWindowViewModel DataContext
- All bindings remain unchanged

**Allowed Modifications:**
- src/SerialAssistant.App/MainWindow.xaml (replace terminal UI with TerminalPage)
- src/SerialAssistant.App/Views/TerminalPage.xaml (new file)
- src/SerialAssistant.App/Views/TerminalPage.xaml.cs (new file)
- docs/UIInformationArchitecture.md (F2A implementation notes)
- docs/PhasePlan.md (split F2 into F2A/F2B)
- docs/ManualTestChecklist.md (F2A verification steps)
- docs/FeatureReports/FeatureF2A-TerminalPageExtraction.md (new report)

**Forbidden:**
- Creating TerminalViewModel
- Modifying MainWindowViewModel business logic
- Changing any binding paths
- Changing Feature A-D behavior
- Implementing navigation logic
- Implementing other page logic (Modbus/Templates/Logs/Settings)

**Implementation Notes:**
- F2A only extracts XAML structure
- TerminalPage inherits DataContext from MainWindow
- No ViewModel changes required
- Zero risk to existing functionality
- All 291 tests remain passing

**Acceptance Criteria:**
- TerminalPage.xaml created with all terminal UI
- TerminalPage.xaml.cs contains only InitializeComponent
- MainWindow.xaml uses TerminalPage correctly
- Shell structure preserved (navigation, top/bottom bars)
- All 291 existing tests pass
- Feature A-D behavior unchanged

**Code Changes Allowed:** Yes (XAML extraction only)

**Tests Required:** No (no logic changes)

**Manual UI Verification Required:** Yes

**ValidationGate Compliance:** Required

---

### Feature F2B1: TerminalViewModel Introduction

**Status:** ✅ Completed

**Goal:** Introduce TerminalViewModel and migrate terminal logic, maintain compatibility with existing tests

**Scope:**
- Create TerminalViewModel with all terminal-specific logic
- Update TerminalPage to bind to TerminalViewModel
- Update MainWindowViewModel to contain Terminal property
- Add compatibility forwarding properties in MainWindowViewModel
- Add TerminalViewModelTests
- Verify all Feature A-D behavior preserved

**Allowed Modifications:**
- src/SerialAssistant.App/ViewModels/TerminalViewModel.cs (new file)
- src/SerialAssistant.App/MainWindowViewModel.cs (add Terminal property and forwarding)
- src/SerialAssistant.App/MainWindow.xaml (update TerminalPage DataContext)
- src/SerialAssistant.Tests/ViewModels/TerminalViewModelTests.cs (new file)
- docs/UIInformationArchitecture.md (F2B1 implementation notes)
- docs/PhasePlan.md (split F2B into F2B1/F2B2)
- docs/FeatureReports/FeatureF2B1-TerminalViewModel.md (new report)

**Implementation Strategy:**
1. Create TerminalViewModel with full terminal logic
2. MainWindowViewModel creates and owns TerminalViewModel
3. MainWindowViewModel provides forwarding properties for test compatibility
4. TerminalPage binds to MainWindowViewModel.Terminal
5. All existing tests pass via forwarding

**Feature A-D Behavior Requirements:**
| Feature | Required Behavior | Implementation Location |
|---------|-------------------|-------------------|
| A | Send line ending (None/CR/LF/CRLF) | TerminalViewModel |
| B | TX/RX direction marking, timestamp | TerminalViewModel/ReceiveDisplayViewModel |
| C | Receive buffer limit, MaxDisplayBytes | ReceiveDisplayViewModel |
| D | Send history, duplicate removal | TerminalViewModel |

**Forbidden:**
- Changing existing Feature A-D behavior
- Removing features
- Breaking existing tests
- Adding new features not in Feature A-D
- Implementing Modbus protocol

**Acceptance Criteria:**
- All 291+ existing tests pass
- Terminal functionality identical to before migration
- TerminalViewModel created with all terminal logic
- TerminalPage bound to TerminalViewModel
- MainWindowViewModel contains Terminal property with forwarding

**Code Changes Allowed:** Yes (refactoring for migration only)

**Tests Required:** Yes (TerminalViewModelTests added)

**Manual UI Verification Required:** Yes

---

### Feature F2B2: MainWindowViewModel Terminal Cleanup

**Status:** ✅ Completed

**Goal:** Remove Terminal compatibility forwarding properties from MainWindowViewModel, making it a pure Shell ViewModel

**Scope:**
- Remove all compatibility forwarding properties from MainWindowViewModel
- Update MainWindow.xaml status bar bindings to use Terminal.*
- Update MainWindowViewModelTests to use Terminal property
- Verify all tests pass
- Update version display to v0.3.3

**Implementation:**
1. MainWindow.xaml status bar now binds to Terminal.ConnectionState, Terminal.SerialSettings.*
2. MainWindowViewModel only contains Terminal property and SaveSettings method
3. MainWindowViewModelTests updated to use viewModel.Terminal.*
4. All terminal logic remains in TerminalViewModel

**Feature A-D Behavior Requirements:**
| Feature | Required Behavior | Implementation Location |
|---------|-------------------|-------------------|
| A | Send line ending (None/CR/LF/CRLF) | TerminalViewModel |
| B | TX/RX direction marking, timestamp | TerminalViewModel/ReceiveDisplayViewModel |
| C | Receive buffer limit, MaxDisplayBytes | ReceiveDisplayViewModel |
| D | Send history, duplicate removal | TerminalViewModel |

**Forbidden:**
- Changing existing Feature A-D behavior
- Removing features
- Breaking existing tests
- Adding new features not in Feature A-D

**Acceptance Criteria:**
- All 291+ existing tests pass
- MainWindowViewModel contains only Terminal property and SaveSettings
- MainWindow.xaml status bar uses Terminal.* bindings
- Version display updated to v0.3.3

**Code Changes Allowed:** Yes (cleanup refactoring only)

**Tests Required:** Yes (update existing tests)

**Manual UI Verification Required:** Yes

---

### Feature F2B: TerminalViewModel Migration - COMPLETED

**Summary:** TerminalViewModel Migration phase is complete.

**Achievements:**
- F2B1: Introduced TerminalViewModel
- F2B2: Cleaned MainWindowViewModel terminal forwarding

**Next Steps:**
- Proceed to Modbus planning phase
- Consider implementing Modbus functionality
- Consider implementing Templates, Logs, Settings pages

**ValidationGate Compliance:** Required

---

### Feature F2C: Shell and Terminal Migration Closure

**Status:** ✅ Completed

**Goal:** Documentation closure for Shell/Terminal migration phases, no code changes

**Scope:**
- Update UIInformationArchitecture.md with closure review
- Update PhasePlan.md with F2C status and future roadmap
- Update ManualTestChecklist.md with F2C verification items
- Update FinalReview.md with Shell/Terminal Migration Review
- Create FeatureF2C report

**Allowed Modifications:**
- docs/UIInformationArchitecture.md
- docs/PhasePlan.md
- docs/ManualTestChecklist.md
- docs/FinalReview.md
- docs/FeatureReports/FeatureF2C-TerminalMigrationClosure.md

**Forbidden:**
- No src modifications
- No tests modifications
- No csproj/sln modifications
- No UI modifications
- No version number changes
- No tag creation

**Version/Tag Policy:**
- Current UI display: v0.3.3
- F2C is documentation-only phase
- No new tag recommended for F2C
- Next actual feature phase should update version appropriately

**Acceptance Criteria:**
- All documentation files updated
- F2B2 test coverage restoration acknowledged
- Future page boundary rules documented
- No code changes made
- Test count: 320 passed

**Code Changes Allowed:** No

**Tests Required:** No

**Manual UI Verification Required:** Yes

**ValidationGate Compliance:** Required

---

### Feature G0: Modbus Planning and Test Strategy

**Status:** ✅ Completed

**Goal:** Define Modbus implementation approach and test strategy before coding

**Scope:**
- Document Modbus RTU/TCP requirements
- Define protocol implementation boundaries
- Create test strategy for Modbus functionality
- Identify dependencies on Core/Infrastructure layers
- Create docs/ModbusPlan.md

**Allowed Modifications:**
- docs/PhasePlan.md
- docs/UIInformationArchitecture.md
- docs/Architecture.md
- docs/ManualTestChecklist.md
- docs/FinalReview.md
- docs/FeatureReports/FeatureG0-ModbusPlanning.md
- docs/ModbusPlan.md

**Forbidden:**
- No code implementation
- No test implementation
- No src modifications
- No csproj/sln modifications
- No UI modifications
- No version number changes

**Version/Tag Policy:**
- Current UI display: v0.3.3
- G0 is documentation-only phase
- No new tag recommended for G0
- Next actual code phase (G1) should update version appropriately

**Acceptance Criteria:**
- Modbus implementation plan documented
- Test strategy defined
- Dependencies identified
- No code changes made
- Test count: 320 passed

**Code Changes Allowed:** No

**Tests Required:** No

**Manual UI Verification Required:** Yes

**ValidationGate Compliance:** Required

---

### Feature G1: Modbus Core Foundation

**Status:** ✅ Completed

**Goal:** Implement Core layer base models, enums, and CRC16 utility

**Implementation Date:** 2026-05-26

**Files Created:**
- `src/SerialAssistant.Core/Modbus/Common/ModbusFunctionCode.cs`
- `src/SerialAssistant.Core/Modbus/Common/ModbusDataType.cs`
- `src/SerialAssistant.Core/Modbus/Models/ModbusRegisterValue.cs`
- `src/SerialAssistant.Core/Modbus/Utilities/ModbusCrc16.cs`
- `src/SerialAssistant.Tests/Modbus/Common/ModbusFunctionCodeTests.cs`
- `src/SerialAssistant.Tests/Modbus/Common/ModbusDataTypeTests.cs`
- `src/SerialAssistant.Tests/Modbus/Models/ModbusRegisterValueTests.cs`
- `src/SerialAssistant.Tests/Modbus/Utilities/ModbusCrc16Tests.cs`
- `src/SerialAssistant.App/MainWindow.xaml` (version update to v0.4.0)

**Scope:**
- ModbusFunctionCode enum
- ModbusDataType enum
- ModbusCrc16 utility
- ModbusRegisterValue model
- Unit tests for all components
- Reference: docs/ModbusPlan.md Section 4

**Allowed Modifications:**
- src/SerialAssistant.Core/Modbus/* (new directory)
- src/SerialAssistant.Tests/Modbus/* (new directory)
- src/SerialAssistant.App/MainWindow.xaml (version only)

**Forbidden:**
- No UI implementation
- No Infrastructure implementation
- No frame builder/parser
- No WPF references
- No System.IO.Ports references

**Acceptance Criteria:**
- CRC16 passes all test vectors ✅
- All enums defined ✅
- Base models created ✅
- All unit tests pass ✅ (34 new tests)
- No UI dependencies ✅
- Test count: 354 total (was 320)

**Code Changes Allowed:** Yes (Core and Tests only)

**Tests Required:** Yes (CRC16 tests)

**Manual UI Verification Required:** No

**Report Required:** Yes

---

### Feature G2: Modbus RTU Frame Builder and Parser

**Status:** ✅ Completed

**Goal:** Implement RTU frame building and parsing for core function codes

**Implementation Date:** 2026-05-26

**Files Created:**
- `src/SerialAssistant.Core/Modbus/Rtu/ModbusRtuErrorCode.cs`
- `src/SerialAssistant.Core/Modbus/Rtu/ModbusRtuFrame.cs`
- `src/SerialAssistant.Core/Modbus/Rtu/ModbusRtuParseResult.cs`
- `src/SerialAssistant.Core/Modbus/Rtu/ModbusRtuRequestBuilder.cs`
- `src/SerialAssistant.Core/Modbus/Rtu/ModbusRtuResponseParser.cs`
- `src/SerialAssistant.Tests/Modbus/Rtu/ModbusRtuFrameTests.cs`
- `src/SerialAssistant.Tests/Modbus/Rtu/ModbusRtuRequestBuilderTests.cs`
- `src/SerialAssistant.Tests/Modbus/Rtu/ModbusRtuResponseParserTests.cs`
- `src/SerialAssistant.App/MainWindow.xaml` (version update to v0.4.1)

**Scope:**
- ModbusRtuFrame model
- ModbusRtuRequestBuilder
- ModbusRtuResponseParser
- Support function codes: 03, 04, 06, 10
- Unit tests for RTU frames
- Reference: docs/ModbusPlan.md Section 8

**Allowed Modifications:**
- src/SerialAssistant.Core/Modbus/Rtu/* (new directory)
- src/SerialAssistant.Tests/Modbus/Rtu/* (new directory)

**Forbidden:**
- No UI implementation
- No TCP implementation
- No Infrastructure implementation

**Acceptance Criteria:**
- 03 Read Holding Registers: frame building and parsing works
- 04 Read Input Registers: frame building and parsing works
- 06 Write Single Register: frame building and parsing works
- 10 Write Multiple Registers: frame building and parsing works
- CRC validation works
- Exception response parsing works
- All unit tests pass

**Code Changes Allowed:** Yes (Core and Tests only)

**Tests Required:** Yes (RTU frame tests)

**Manual UI Verification Required:** No

**Report Required:** Yes

---

### Feature G3: Modbus TCP Frame Builder and Parser

**Status:** ✅ Completed

**Goal:** Implement TCP/MBAP frame building and parsing

**Implementation Date:** 2026-05-26

**Files Created:**
- `src/SerialAssistant.Core/Modbus/Tcp/ModbusTcpErrorCode.cs`
- `src/SerialAssistant.Core/Modbus/Tcp/MbapHeader.cs`
- `src/SerialAssistant.Core/Modbus/Tcp/ModbusTcpParseResult.cs`
- `src/SerialAssistant.Core/Modbus/Tcp/ModbusTcpFrame.cs`
- `src/SerialAssistant.Core/Modbus/Tcp/ModbusTcpRequestBuilder.cs`
- `src/SerialAssistant.Core/Modbus/Tcp/ModbusTcpResponseParser.cs`
- `src/SerialAssistant.Tests/Modbus/Tcp/MbapHeaderTests.cs`
- `src/SerialAssistant.Tests/Modbus/Tcp/ModbusTcpFrameTests.cs`
- `src/SerialAssistant.Tests/Modbus/Tcp/ModbusTcpRequestBuilderTests.cs`
- `src/SerialAssistant.Tests/Modbus/Tcp/ModbusTcpResponseParserTests.cs`
- `src/SerialAssistant.App/MainWindow.xaml` (version update to v0.4.2)

**Scope:**
- MbapHeader model
- ModbusTcpFrame model
- ModbusTcpRequestBuilder
- ModbusTcpResponseParser
- Support function codes: 03, 04, 06, 10
- Unit tests for TCP frames
- Reference: docs/ModbusPlan.md Section 9

**Allowed Modifications:**
- src/SerialAssistant.Core/Modbus/Tcp/* (new directory)
- src/SerialAssistant.Tests/Modbus/Tcp/* (new directory)

**Forbidden:**
- No UI implementation
- No Infrastructure implementation

**Acceptance Criteria:**
- 03 Read Holding Registers: frame building and parsing works
- 04 Read Input Registers: frame building and parsing works
- 06 Write Single Register: frame building and parsing works
- 10 Write Multiple Registers: frame building and parsing works
- MBAP header validation works
- All unit tests pass

**Code Changes Allowed:** Yes (Core and Tests only)

**Tests Required:** Yes (TCP frame tests)

**Manual UI Verification Required:** No

**Report Required:** Yes

---

### Feature G4: ModbusViewModel Minimal Workflow

**Status:** ✅ Completed

**Implementation Date:** 2026-05-26

**Goal:** Establish ModbusViewModel without complex UI

**Files Created:**
- `src/SerialAssistant.App/ViewModels/ModbusTransportMode.cs`
- `src/SerialAssistant.App/ViewModels/ModbusRequestKind.cs`
- `src/SerialAssistant.App/ViewModels/ModbusViewModel.cs`
- `src/SerialAssistant.Tests/ViewModels/ModbusViewModelTests.cs`
- `src/SerialAssistant.App/MainWindow.xaml` (version update to v0.4.3)

**Scope:**
- ModbusTransportMode enum (Rtu, Tcp)
- ModbusRequestKind enum (ReadHoldingRegisters, ReadInputRegisters, WriteSingleRegister, WriteMultipleRegisters)
- ModbusViewModel with RTU/TCP request building and response parsing
- Unit tests (54 tests)
- Reference: docs/ModbusPlan.md Section 5

**Allowed Modifications:**
- src/SerialAssistant.App/ViewModels/*.cs (new files)
- src/SerialAssistant.Tests/ViewModels/*.cs (new files)

**Forbidden:**
- No XAML implementation
- No frame building/parsing (delegates to Core)
- No System.IO.Ports references in ViewModel
- No Infrastructure modifications

**Acceptance Criteria:**
- ModbusViewModel created ✅
- RTU/TCP request building works ✅
- RTU/TCP response parsing works ✅
- Error handling works ✅
- All unit tests pass ✅ (54 new tests)
- Test count: 494 total (was 440)

**Code Changes Allowed:** Yes (App and Tests only)

**Tests Required:** Yes

**Manual UI Verification Required:** No

**Report Required:** Yes

---

### Feature G5: ModbusPage Minimal UI

**Status:** Completed

**Goal:** Implement minimal UI for register read/write

**Scope:**
- ModbusPage.xaml (new file)
- ModbusPage.xaml.cs (new file)
- Address input
- Quantity input
- Function code selection
- Read/Write buttons
- Response display
- Reference: docs/ModbusPlan.md Section 5

**Allowed Modifications:**
- src/SerialAssistant.App/Views/ModbusPage.xaml (new file)
- src/SerialAssistant.App/Views/ModbusPage.xaml.cs (new file)
- src/SerialAssistant.App/MainWindow.xaml (navigation binding update)
- src/SerialAssistant.App/ViewModels/MainWindowViewModel.cs
- src/SerialAssistant.App/ViewModels/ModbusViewModel.cs (add UI collections)

**Forbidden:**
- Complex charting
- Multiple simultaneous operations
- Advanced styling

**Acceptance Criteria:**
- Address input works
- Quantity input works
- Function code selection works
- Read operation works
- Write operation works
- Response displays correctly
- Navigation to ModbusPage works
- All unit tests pass (26 new tests added to MainWindowViewModelTests and ModbusViewModelTests)
- Test count: 520 total (was 494)

**Code Changes Allowed:** Yes (UI and minimal code-behind)

**Tests Required:** Yes

**Manual UI Verification Required:** Yes

**Report Required:** Yes

---

### Feature G6: Modbus Manual Test and Documentation Closure

**Status:** Completed

**Goal:** Complete manual testing and documentation

**Scope:**
- Manual test checklist for Modbus
- Documentation updates
- Final verification
- Reference: docs/ModbusPlan.md Section 12

**Allowed Modifications:**
- docs/ManualTestChecklist.md
- docs/ModbusPlan.md (if updates needed)
- docs/FinalReview.md

**Forbidden:**
- No new features
- No code implementation
- No test implementation

**Acceptance Criteria:**
- Manual test checklist complete
- All Modbus functionality verified
- Documentation up to date
- G1-G5 test counts documented

**Code Changes Allowed:** No

**Tests Required:** No

**Manual UI Verification Required:** Yes

**Report Required:** Yes

---

### Feature G0-G7 Summary

| Phase | Type | Focus | Code Allowed |
|-------|------|-------|--------------|
| G0 | Planning | Documentation only | No |
| G1 | Core | CRC16, base models | Core + Tests |
| G2 | Core | RTU frames | Core + Tests |
| G3 | Core | TCP frames | Core + Tests |
| G4 | App | ModbusViewModel | App + Tests |
| G5 | UI | ModbusPage | UI + minimal |
| G6 | Closure | Testing + Docs | No |
| G7 | Planning | Transport Integration | No |

---

### Feature G7: Modbus Transport Integration Planning

**Status**: ✅ Completed

**Goal**: Plan Modbus RTU/TCP real communication integration architecture

**Scope**:
- Define layer boundaries for transport integration
- Propose IModbusTransport, IModbusRtuTransport, IModbusTcpTransport interfaces
- Plan RTU transport via existing SerialPortService with ownership management
- Plan TCP transport via new Infrastructure service with TcpClient
- Define error strategy for communication failures
- Plan fake transport for automated testing without hardware
- Break down G8-G12 phases for incremental implementation

**Allowed Modifications**:
- docs/ModbusTransportPlan.md (new)
- docs/ModbusPlan.md (update)
- docs/PhasePlan.md (update)
- docs/Architecture.md (update)
- docs/UIInformationArchitecture.md (update)
- docs/ManualTestChecklist.md (update)
- docs/FinalReview.md (update)
- docs/FeatureReports/FeatureG7-ModbusTransportPlanning.md (new)

**Forbidden**:
- No src/ modifications
- No tests/ modifications
- No UI changes
- No version changes
- No third-party libraries

**Acceptance Criteria**:
- All planning documentation complete
- G8-G12 phases clearly defined
- Layer boundaries preserved
- No code changes made
- Test count remains 520 passed

**Code Changes Allowed**: No

**Tests Required**: No (only verification that existing tests still pass)

**Report Required**: Yes

---

### Next Phase Recommendation

**Recommended**: G8 - Modbus Transport Interfaces and Fake Tests

**Rationale:**
- G7 planning complete, architecture defined
- Lock down interfaces before real implementation
- Prove ViewModel can work with transport via fakes
- Reduce risk by validating architecture first
- NO real hardware implementation yet

**Why NOT Skip to G9/G10**:
- Risk of App layer pollution with IO references
- Risk of incorrect architecture decisions
- Fake transport allows testing without hardware
- Interface contract validation is critical

**Note**: Do NOT continue UI styling until communication is working. Functional completeness should precede visual polish.

---

## Future Phases (G8A-G12)

### Feature G8A: Modbus Transport Contracts and Fake Transport Foundation

**Status**: ✅ Completed

**Implementation Date**: 2026-05-29

**Files Created**:
- `src/SerialAssistant.Core/Modbus/Transport/IModbusTransport.cs`
- `src/SerialAssistant.Core/Modbus/Transport/IModbusRtuTransport.cs`
- `src/SerialAssistant.Core/Modbus/Transport/IModbusTcpTransport.cs`
- `src/SerialAssistant.Core/Modbus/Transport/ModbusTransportResult.cs`
- `src/SerialAssistant.Core/Modbus/Transport/ModbusTransportOptions.cs`
- `src/SerialAssistant.Core/Modbus/Transport/ModbusRequestContext.cs`
- `src/SerialAssistant.Core/Modbus/Transport/ModbusTransportErrorCode.cs`
- `src/SerialAssistant.Tests/Modbus/Transport/FakeModbusTransport.cs`
- `src/SerialAssistant.Tests/Modbus/Transport/ModbusTransportOptionsTests.cs`
- `src/SerialAssistant.Tests/Modbus/Transport/ModbusRequestContextTests.cs`
- `src/SerialAssistant.Tests/Modbus/Transport/ModbusTransportResultTests.cs`
- `src/SerialAssistant.Tests/Modbus/Transport/FakeModbusTransportTests.cs`

**Scope**:
- Create IModbusTransport interface in Core layer
- Create IModbusRtuTransport interface
- Create IModbusTcpTransport interface
- Create ModbusTransportResult model
- Create ModbusTransportOptions model
- Create ModbusRequestContext model
- Create ModbusTransportErrorCode enum
- Implement FakeModbusTransport for testing
- Add comprehensive tests for all new types
- Update version display to v0.4.5

**Allowed Modifications**:
- src/SerialAssistant.Core/Modbus/Transport/ (interfaces and models)
- src/SerialAssistant.Tests/Modbus/Transport/ (fake transport and tests)
- src/SerialAssistant.App/MainWindow.xaml (version only)

**Forbidden**:
- No System.IO.Ports reference in App layer
- No TcpClient/Socket reference in App layer
- No real serial/TCP implementation
- No ModbusViewModel send workflow changes
- No ModbusPage UI changes

**Acceptance Criteria**:
- All transport contracts defined in Core
- FakeModbusTransport implemented
- 40 new tests added
- Test count: 560 total (was 520)
- Layer boundaries preserved
- Version updated to v0.4.5

**Code Changes Allowed**: Yes (Core + Tests + version update)

**Tests Required**: Yes (40 new tests)

**Report Required**: Yes

---

### Feature G8B: ModbusViewModel Transport Injection with Fake Tests

**Status**: ✅ Completed

**Implementation Date**: 2026-05-29

**Files Created/Modified**:
- `src/SerialAssistant.App/ViewModels/ModbusViewModel.cs` (updated)
- `src/SerialAssistant.Tests/ViewModels/ModbusViewModelTransportTests.cs` (new)
- `src/SerialAssistant.App/MainWindow.xaml` (version updated to v0.4.6)

**Scope**:
- Inject IModbusTransport into ModbusViewModel
- Add Connect/Disconnect/SendRequest async methods
- Add connection state management (IsConnected, IsBusy, etc.)
- Add transport error handling
- Test with FakeModbusTransport

**Allowed Modifications**:
- src/SerialAssistant.App/ViewModels/ModbusViewModel.cs
- src/SerialAssistant.Tests/ViewModels/ModbusViewModelTransportTests.cs
- src/SerialAssistant.App/MainWindow.xaml (version only)

**Forbidden**:
- No real SerialPort/TcpClient/Socket implementation
- No Infrastructure layer changes
- No ModbusPage UI changes

**Acceptance Criteria**:
- ViewModel can use IModbusTransport
- Tests pass with FakeModbusTransport (26 new tests)
- No IO references in App layer
- Test count: 586 passed (was 560)

**Code Changes Allowed**: Yes (App layer integration + Tests)

**Tests Required**: Yes (26 new tests)

**Report Required**: Yes

---

### Feature G9A: Modbus RTU Transport Existing Serial Service Capability Review

**Status**: ✅ Completed

**Implementation Date**: 2026-05-29

**Scope**:
- Review existing ISerialPortService
- Review existing SerialPortService
- Review TerminalViewModel serial usage
- Gap analysis for Modbus RTU request-response
- Recommend RTU transport implementation strategy
- Plan G9B/G9C subsequent phases

**Allowed Modifications**:
- Documentation only
- No src changes
- No tests changes

**Forbidden**:
- Any code changes

**Acceptance Criteria**:
- G9A report created
- Gaps documented
- Recommendation made
- G9B/G9C planned

**Code Changes Allowed**: No

**Tests Required**: No (existing tests pass, 586)

**Report Required**: Yes

---

### Feature G9B: Serial Port Ownership Coordinator Contracts

**Status**: ⏳ Pending

**Goal**: Define port ownership coordination interfaces

**Scope**:
- Define ISerialPortOwnershipCoordinator in Core
- Define SerialPortOwner enum
- No Infrastructure changes yet
- Fake-based tests for coordinator

**Allowed Modifications**:
- Core layer only (new interfaces/models)
- Tests layer (fake coordinator tests)

**Forbidden**:
- No Infrastructure changes
- No App layer changes
- No Terminal changes

**Acceptance Criteria**:
- Interfaces defined in Core
- No Infrastructure references in Core
- Fake coordinator tests pass

**Code Changes Allowed**: Yes (Core layer only)

**Tests Required**: Yes (fake-based tests)

**Report Required**: Yes

---

### Feature G9C: Modbus RTU Transport Implementation with Fake Serial Adapter

**Status**: ⏳ Pending

**Goal**: Implement ModbusRtuTransport with fake serial adapter

**Scope**:
- Implement ModbusRtuTransport in Infrastructure
- Use fake serial adapter for testing
- Implement Connect/Disconnect/SendRequestAsync
- Integrate with ownership coordinator
- No real System.IO.Ports yet

**Allowed Modifications**:
- Infrastructure layer (ModbusRtuTransport)
- Tests layer (fake serial adapter tests)

**Forbidden**:
- No real System.IO.Ports usage yet
- No App layer changes
- No Terminal changes

**Acceptance Criteria**:
- ModbusRtuTransport implements IModbusRtuTransport
- Ownership coordinator integration works
- Fake-based tests pass
- No real hardware required

**Code Changes Allowed**: Yes (Infrastructure layer)

**Tests Required**: Yes (fake-based tests)

**Report Required**: Yes

---

### Feature G9D: Modbus RTU Transport Manual Verification

**Status**: ⏳ Pending

**Goal**: Manual verification with real hardware (optional)

**Scope**:
- Implement real System.IO.Ports in ModbusRtuTransport
- Manual testing with real Modbus RTU device
- Manual testing with Modbus RTU simulator
- Verify Terminal/Modbus conflict prevention

**Allowed Modifications**:
- Infrastructure layer (add real serial)
- Documentation only (manual test checklist)

**Forbidden**:
- No App layer changes
- No Terminal changes

**Acceptance Criteria**:
- Real RTU communication works
- Ownership conflict prevention verified
- Manual tests pass

**Code Changes Allowed**: Yes (Infrastructure layer only)

**Tests Required**: Manual tests only

**Report Required**: Yes

---

### Feature G10: Modbus TCP Transport Integration

**Status**: ⏳ Pending

**Goal**: Implement real TCP transport via TcpClient

**Scope**:
- Implement ModbusTcpTransport in Infrastructure layer
- Use TcpClient (Infrastructure only)
- Implement MBAP TransactionId matching
- Handle TCP connect/disconnect
- Handle TCP half-open connections
- Add timeout handling
- Manual testing with Modbus TCP simulator/device

**Allowed Modifications**:
- Infrastructure layer (TCP transport)
- No App layer IO references

**Forbidden**:
- App layer directly references TcpClient/Socket
- Core layer references Infrastructure

**Acceptance Criteria**:
- TCP transport works with Modbus TCP server
- TransactionId matching works
- Error handling implemented
- Manual verification passes

**Code Changes Allowed**: Yes (Infrastructure only)

**Tests Required**: Yes (fake-based tests)

**Report Required**: Yes

---

### Feature G11: Modbus Send Workflow UI Integration

**Status**: ⏳ Pending

**Goal**: Integrate transport with ModbusViewModel and update UI

**Scope**:
- Update ModbusViewModel with SendRequestCommand
- Add Connect/Disconnect commands
- Add RTU parameters UI (port, baud rate, etc.)
- Add TCP parameters UI (IP, port)
- Add Send Request button
- Add connection status display
- Add timeout configuration
- NO final UI styling

**Allowed Modifications**:
- src/SerialAssistant.App/ViewModels/ModbusViewModel.cs
- src/SerialAssistant.App/Views/ModbusPage.xaml (minimal additions)

**Forbidden**:
- Final MQTTX-style UI

**Goal**: Implement real RTU transport via SerialPortService

**Scope**:
- Implement ModbusRtuTransport in Infrastructure layer
- Integrate with existing SerialPortService
- Implement serial port ownership management
- Implement Connect/Disconnect/SendRequestAsync
- Handle RTU-specific error cases
- Add timeout handling
- Manual testing with real hardware (optional)

**Allowed Modifications**:
- src/SerialAssistant.Infrastructure/ (RTU transport)
- No App layer IO references
- No Core layer changes

**Forbidden**:
- App layer directly references System.IO.Ports
- Core layer references Infrastructure
- UI changes (except minimal status)

**Acceptance Criteria**:
- RTU transport works with serial port
- Ownership model prevents Terminal/Modbus conflicts
- Error handling implemented
- Manual verification passes

**Code Changes Allowed**: Yes (Infrastructure only)

**Tests Required**: Yes (fake-based tests, no hardware required)

**Report Required**: Yes

---

### Feature G10: Modbus TCP Transport Integration

**Status**: ⏳ Pending

**Goal**: Implement real TCP transport via TcpClient

**Scope**:
- Implement ModbusTcpTransport in Infrastructure layer
- Use TcpClient (Infrastructure only)
- Implement MBAP TransactionId matching
- Handle TCP connect/disconnect
- Handle TCP half-open connections
- Add timeout handling
- Manual testing with Modbus TCP simulator/device

**Allowed Modifications**:
- src/SerialAssistant.Infrastructure/ (TCP transport)
- No App layer IO references

**Forbidden**:
- App layer directly references TcpClient/Socket
- Core layer references Infrastructure

**Acceptance Criteria**:
- TCP transport works with Modbus TCP server
- TransactionId matching works
- Error handling implemented
- Manual verification passes

**Code Changes Allowed**: Yes (Infrastructure only)

**Tests Required**: Yes (fake-based tests)

**Report Required**: Yes

---

### Feature G11: Modbus Send Workflow UI Integration

**Status**: ⏳ Pending

**Goal**: Integrate transport with ModbusViewModel and update UI

**Scope**:
- Update ModbusViewModel with SendRequestCommand
- Add Connect/Disconnect commands
- Add RTU parameters UI (port, baud rate, etc.)
- Add TCP parameters UI (IP, port)
- Add Send Request button
- Add connection status display
- Add timeout configuration
- NO final UI styling

**Allowed Modifications**:
- src/SerialAssistant.App/ViewModels/ModbusViewModel.cs
- src/SerialAssistant.App/Views/ModbusPage.xaml (minimal additions)

**Forbidden**:
- Final MQTTX-style UI
- Complex UI animations
- Breaking existing functionality

**Acceptance Criteria**:
- User can connect RTU/TCP
- User can send Modbus requests
- Responses display correctly
- Errors display in UI
- All existing tests still pass

**Code Changes Allowed**: Yes (App layer only)

**Tests Required**: Yes

**Report Required**: Yes

---

### Feature G12: Modbus Communication Manual Verification

**Status**: ⏳ Pending

**Goal**: Full manual testing and documentation closure

**Scope**:
- Full manual testing of RTU communication
- Full manual testing of TCP communication
- Edge case testing (timeouts, errors, disconnects)
- Update manual test checklist
- Final documentation review

**Allowed Modifications**:
- Documentation only

**Forbidden**:
- No new features
- No code changes

**Acceptance Criteria**:
- All manual test checklist items pass
- Documentation complete and consistent
- Ready for next phase (H0 UI styling)

**Code Changes Allowed**: No

**Tests Required**: No (only manual verification)

**Report Required**: Yes

---

## Future Phases (J1-N1)

### Feature J1: Message Templates and Periodic Sending

**Goal:** Support reusable message templates and cyclic transmission

**Scope:**
- Template management (create/edit/delete)
- Template variables
- Periodic sending configuration
- Send interval control
- Start/stop periodic sending

**Forbidden:**
- Complex scripting
- External file dependencies

**Acceptance Criteria:**
- Templates can be saved and reused
- Periodic sending works correctly
- UI controls for interval adjustment

**Code Changes Allowed:** Yes

**Tests Required:** Yes

**Report Required:** Yes

---

### Feature K1: Communication Logs Export and Statistics

**Goal:** Implement comprehensive logging and statistics

**Scope:**
- Session logging
- Log filtering
- Log export (CSV/JSON)
- Communication statistics (bytes sent/received, errors)
- Real-time metrics display

**Forbidden:**
- Database dependency
- Cloud storage

**Acceptance Criteria:**
- Logs can be viewed and filtered
- Logs can be exported
- Statistics displayed in UI
- No external dependencies

**Code Changes Allowed:** Yes

**Tests Required:** Yes

**Report Required:** Yes

---

### Feature L1: Connection Configuration Profiles

**Goal:** Support saving and switching connection configurations

**Scope:**
- Profile management (create/edit/delete)
- Serial port profiles
- TCP connection profiles
- Modbus settings profiles
- Default profile selection

**Forbidden:**
- External configuration storage
- Complex profile inheritance

**Acceptance Criteria:**
- Profiles can be saved and loaded
- Connection settings restored correctly
- Profile switching works

**Code Changes Allowed:** Yes

**Tests Required:** Yes

**Report Required:** Yes

---

### Feature M1: Release Strategy Finalization

**Goal:** Define and implement release process

**Scope:**
- Framework-dependent deployment
- Documentation of .NET Runtime requirements
- Build script optimization
- Versioning strategy
- Optional Native Launcher planning

**Forbidden:**
- Self-contained deployment
- Third-party packaging tools

**Acceptance Criteria:**
- Release process documented
- Build commands tested
- Versioning consistent

**Code Changes Allowed:** Yes (build configuration)

**Tests Required:** Yes (build verification)

**Report Required:** Yes

---

### Feature N1: Unified UI Style Optimization

**Goal:** Modernize UI with consistent styling

**Scope:**
- Color scheme optimization
- Spacing and typography
- Corner radius consistency
- Icon system
- Light/dark theme support
- Status indicators

**Forbidden:**
- Third-party UI libraries
- Flashy animations
- Breaking existing functionality

**Acceptance Criteria:**
- Modern, clean appearance
- Consistent visual language
- Both light and dark themes work
- No functionality regressions

**Code Changes Allowed:** Yes (styling only)

**Tests Required:** Yes (UI tests if applicable)

**Report Required:** Yes

---

## Phase Dependencies

```
E1 → E2 → F1 → F2 → G1 → G2 → H1 → I1 → J1 → K1 → L1 → M1 → N1
                 ↓
                 └──→ G1 (can start after F1)
```

**Dependency Notes:**
- E2 requires E1 to be complete
- F1 requires E2 to be complete
- F2 requires F1 to be complete
- G1 can be started after F1 (parallel track with F2)

## Phase Duration Estimates

| Feature | Estimated Duration |
|---------|-------------------|
| E1 | 1 day |
| E2 | 2 days |
| F1 | 3 days |
| F2 | 3 days |
| G1 | 3 days |
| G2 | 3 days |
| H1 | 2 days |
| I1 | 2 days |
| J1 | 2 days |
| K1 | 2 days |
| L1 | 2 days |
| M1 | 1 day |
| N1 | 3 days |

**Total Estimate:** 28 days (approximately 6 weeks)

## Validation Requirement

All phases must follow the **Validation Gate** defined in [docs/ValidationGate.md](ValidationGate.md).

### Required Verification
Before declaring any phase complete, the following must pass:

1. **Branch Check**: Current branch matches expected feature branch
2. **Build Check**: `dotnet build -c Debug` passes with 0 errors
3. **Test Check**: `dotnet test -c Debug` passes with all tests green
4. **Diff Check**: `git diff --check` passes with no trailing whitespace
5. **Scope Check**: Changed files match phase constraints
6. **Report Check**: Phase report created/updated in docs/FeatureReports/

### Phase Prompt Reference
Each phase prompt should reference ValidationGate rather than repeating all commands:

```
Before completing this phase, verify:
- git diff --check passes (no trailing whitespace)
- Build and tests pass
- Changed files match phase scope
- Phase report created

See docs/ValidationGate.md for complete validation requirements.
```

### Common Rejection Reasons
- Trailing whitespace in any file (including .md)
- Build errors or warnings
- Test failures
- Out-of-scope file changes
- Missing Phase Report

---

*Last updated: May 2026*
