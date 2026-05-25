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

### Feature F2B: TerminalViewModel Migration

**Status:** Pending

**Goal:** Migrate terminal logic from MainWindowViewModel to TerminalViewModel

**Scope:**
- Create TerminalViewModel with terminal-specific logic
- Migrate serial port logic, send/receive logic, history management
- Update TerminalPage to use TerminalViewModel
- Update MainWindowViewModel to manage navigation only
- Update tests for migrated functionality
- Verify all Feature A-D behavior preserved

**Allowed Modifications:**
- src/SerialAssistant.App/ViewModels/TerminalViewModel.cs (new file)
- src/SerialAssistant.App/MainWindowViewModel.cs (remove terminal logic)
- src/SerialAssistant.App/Views/TerminalPage.xaml (update bindings if needed)
- Tests for TerminalViewModel
- docs/UIInformationArchitecture.md (F2B implementation notes)
- docs/PhasePlan.md (update F2B status)
- docs/FeatureReports/FeatureF2B-TerminalViewModelMigration.md (new report)

**Migration Principles:**
1. Copy existing logic to TerminalViewModel first
2. Test after each logical unit moved
3. Remove old code from MainWindowViewModel only after verification
4. Maintain all existing tests
5. Preserve all Feature A-D behavior

**Feature A-D Behavior Requirements:**
| Feature | Required Behavior | Migration Location |
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
- MainWindowViewModel no longer contains terminal-specific logic
- TerminalViewModel handles all terminal operations
- Feature A-D behavior preserved

**Code Changes Allowed:** Yes (refactoring for migration only)

**Tests Required:** Yes (existing tests + TerminalViewModel tests)

**Manual UI Verification Required:** Yes

**ValidationGate Compliance:** Required

---

### Feature G1: Modbus Core Protocol Layer

**Goal:** Implement core Modbus protocol objects and algorithms

**Scope:**
- Implement Modbus function codes (01-16)
- Implement CRC16 calculation
- Implement RTU frame structure
- Create base request/response models
- Implement protocol exceptions

**Forbidden:**
- Referencing WPF
- Referencing System.IO.Ports
- Accessing file system

**Acceptance Criteria:**
- Core protocol objects work correctly
- CRC16 validation passes
- All unit tests pass
- No UI dependencies

**Code Changes Allowed:** Yes (Core layer only)

**Tests Required:** Yes (comprehensive protocol tests)

**Report Required:** Yes

---

### Feature G2: Modbus RTU Request/Response Handling

**Goal:** Implement Modbus RTU request building and response parsing

**Scope:**
- Read Coils (01)
- Read Discrete Inputs (02)
- Read Holding Registers (03)
- Read Input Registers (04)
- Write Single Coil (05)
- Write Single Register (06)
- Write Multiple Coils (15)
- Write Multiple Registers (16)

**Forbidden:**
- UI implementation
- Serial port communication

**Acceptance Criteria:**
- All function codes implemented
- Request/response round-trip works
- Error handling for invalid responses

**Code Changes Allowed:** Yes (Core layer only)

**Tests Required:** Yes

**Report Required:** Yes

---

### Feature H1: Modbus RTU Minimal UI Loop

**Goal:** Basic Modbus RTU UI for register read/write

**Scope:**
- Create ModbusPage.xaml
- Create ModbusViewModel
- Basic register read functionality
- Basic register write functionality
- Connection status display

**Forbidden:**
- Complex UI features
- Full protocol implementation

**Acceptance Criteria:**
- Can connect to Modbus RTU device
- Can read holding registers
- Can write holding registers
- Basic error handling

**Code Changes Allowed:** Yes

**Tests Required:** Yes

**Report Required:** Yes

---

### Feature I1: Modbus TCP Support

**Goal:** Add Modbus TCP protocol support

**Scope:**
- Create TCP communication abstraction
- Implement Modbus TCP frame format
- Handle TCP-specific considerations
- Integrate with existing Modbus core

**Forbidden:**
- Breaking existing RTU functionality

**Acceptance Criteria:**
- Modbus TCP communication works
- Both RTU and TCP available
- Protocol selection in UI

**Code Changes Allowed:** Yes

**Tests Required:** Yes

**Report Required:** Yes

---

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
