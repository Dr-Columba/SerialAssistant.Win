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
- Feature E2: UI Information Architecture Detailed Design (in progress)

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

**Goal:** Build the main application shell with navigation, placeholder pages, and basic navigation logic

**Scope:**
- Create MainWindow.xaml as Shell container with navigation structure
- Create MainWindowViewModel with navigation logic
- Create placeholder pages:
  - TerminalPage.xaml / TerminalViewModel
  - ModbusPage.xaml / ModbusViewModel
  - TemplatesPage.xaml / TemplatesViewModel
  - LogsPage.xaml / LogsViewModel
  - SettingsPage.xaml / SettingsViewModel
- Implement left navigation panel
- Implement main content frame
- Verify navigation between pages works

**Allowed Modifications:**
- src/SerialAssistant.App/MainWindow.xaml (shell structure)
- src/SerialAssistant.App/MainWindow.xaml.cs (minimal changes if needed)
- src/SerialAssistant.App/ViewModels/ (new ViewModels)
- src/SerialAssistant.App/Views/ (new page XAML files)
- Tests for navigation logic

**Forbidden:**
- Migrating existing serial terminal functionality
- Implementing Modbus protocol
- Changing existing Feature A-D behavior
- Final visual styling
- Introducing third-party UI libraries
- Adding complex animations
- Creating theme systems

**Expected New Files:**
- src/SerialAssistant.App/Views/TerminalPage.xaml
- src/SerialAssistant.App/Views/ModbusPage.xaml
- src/SerialAssistant.App/Views/TemplatesPage.xaml
- src/SerialAssistant.App/Views/LogsPage.xaml
- src/SerialAssistant.App/Views/SettingsPage.xaml
- src/SerialAssistant.App/ViewModels/TerminalViewModel.cs
- src/SerialAssistant.App/ViewModels/ModbusViewModel.cs
- src/SerialAssistant.App/ViewModels/TemplatesViewModel.cs
- src/SerialAssistant.App/ViewModels/LogsViewModel.cs
- src/SerialAssistant.App/ViewModels/SettingsViewModel.cs

**Acceptance Criteria:**
- Shell UI renders correctly with navigation
- Navigation between all placeholder pages works
- MainWindow.xaml.cs remains minimal (~20 lines)
- MainWindowViewModel handles only navigation and global state
- All placeholder pages are navigable
- No serial port functionality moved yet

**Code Changes Allowed:** Yes (Shell infrastructure only)

**Tests Required:** Yes (navigation logic tests)

**Manual UI Verification Required:** Yes

**ValidationGate Compliance:** Required

---

### Feature F2: Terminal Page Migration

**Goal:** Migrate existing serial terminal functionality from MainWindow to TerminalPage, preserve Feature A-D behavior

**Scope:**
- Migrate terminal logic from MainWindowViewModel to TerminalViewModel
- Migrate terminal XAML from MainWindow.xaml to TerminalPage.xaml
- Create or update ReceiveDisplayViewModel if needed
- Update MainWindowViewModel to coordinate with TerminalViewModel
- Update tests for migrated functionality
- Verify all Feature A-D behavior preserved

**Allowed Modifications:**
- src/SerialAssistant.App/MainWindow.xaml (remove terminal content)
- src/SerialAssistant.App/MainWindow.xaml.cs (minimal if needed)
- src/SerialAssistant.App/ViewModels/MainWindowViewModel.cs (refactor)
- src/SerialAssistant.App/ViewModels/TerminalViewModel.cs (add terminal logic)
- src/SerialAssistant.App/Views/TerminalPage.xaml (add terminal content)
- src/SerialAssistant.App/ViewModels/ReceiveDisplayViewModel.cs (if needed)
- Tests for TerminalViewModel

**Migration Principles:**
1. Copy existing logic to new location first
2. Test after each logical unit moved
3. Remove old code only after verification
4. Maintain all existing tests

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
- Feature A Send Line Ending works correctly
- Feature B TX/RX marking and timestamp works correctly
- Feature C Receive buffer limit works correctly
- Feature D Send history works correctly

**Code Changes Allowed:** Yes (refactoring for migration only)

**Tests Required:** Yes (existing tests + migration tests)

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
