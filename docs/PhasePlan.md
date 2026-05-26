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

**Status:** Planned

**Goal:** Implement RTU frame building and parsing for core function codes

**Scope:**
- ModbusRtuFrame model
- ModbusRtuFrameBuilder
- ModbusRtuFrameParser
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

**Status:** Planned

**Goal:** Implement TCP/MBAP frame building and parsing

**Scope:**
- MbapHeader model
- ModbusTcpFrame model
- ModbusTcpFrameBuilder
- ModbusTcpFrameParser
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

**Status:** Planned

**Goal:** Establish ModbusViewModel without complex UI

**Scope:**
- ModbusViewModel
- Connection state management
- Register value management
- Error handling
- Unit tests
- Reference: docs/ModbusPlan.md Section 5

**Allowed Modifications:**
- src/SerialAssistant.App/ViewModels/ModbusViewModel.cs (new file)
- src/SerialAssistant.Tests/ViewModels/ModbusViewModelTests.cs (new file)

**Forbidden:**
- No XAML implementation
- No frame building/parsing (delegates to Core)
- No System.IO.Ports references in ViewModel

**Acceptance Criteria:**
- ModbusViewModel created
- Connection state tracking works
- Register read/write operations work
- Error handling works
- All unit tests pass

**Code Changes Allowed:** Yes (App and Tests only)

**Tests Required:** Yes

**Manual UI Verification Required:** No

**Report Required:** Yes

---

### Feature G5: ModbusPage Minimal UI

**Status:** Planned

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

**Code Changes Allowed:** Yes (UI and minimal code-behind)

**Tests Required:** No (manual verification only)

**Manual UI Verification Required:** Yes

**Report Required:** Yes

---

### Feature G6: Modbus Manual Test and Documentation Closure

**Status:** Planned

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

### Feature G0-G6 Summary

| Phase | Type | Focus | Code Allowed |
|-------|------|-------|--------------|
| G0 | Planning | Documentation only | No |
| G1 | Core | CRC16, base models | Core + Tests |
| G2 | Core | RTU frames | Core + Tests |
| G3 | Core | TCP frames | Core + Tests |
| G4 | App | ModbusViewModel | App + Tests |
| G5 | UI | ModbusPage | UI + minimal |
| G6 | Closure | Testing + Docs | No |

---

## Future Phases

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
