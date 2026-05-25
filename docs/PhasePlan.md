# Phase Plan

## Overview

This document outlines the phased development plan for SerialAssistant.Win, organized into features with clear scope, boundaries, and acceptance criteria.

## Current Status

**Current Tag:** v0.2.0 (Features A-D Completed)

**Completed Features:**
- Feature A: Send Line Ending Options
- Feature B: TX/RX Direction Marking and Timestamp Display
- Feature C: Receive Buffer Limit and Configuration
- Feature D: Send History with UI and Persistence

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

**Goal:** Design detailed UI architecture before implementation

**Scope:**
- Define left navigation structure
- Define top connection status bar
- Define main workspace layout
- Define bottom status bar
- Define page structure (Terminal, Modbus, Templates, Logs, Settings)
- Update UIInformationArchitecture.md

**Forbidden:**
- Implementing UI code
- Modifying existing views
- Changing functionality

**Acceptance Criteria:**
- UI architecture fully documented
- Navigation structure defined
- Page responsibilities clear

**Code Changes Allowed:** No

**Tests Required:** No

**Report Required:** Yes

---

### Feature F1: Application Shell Skeleton

**Goal:** Build the main application shell with navigation

**Scope:**
- Implement left navigation panel
- Implement top status bar
- Implement main workspace frame
- Implement bottom status bar
- Create base page infrastructure
- Update MainWindow.xaml as Shell container

**Forbidden:**
- Modifying serial terminal logic
- Moving existing functionality

**Acceptance Criteria:**
- Shell UI renders correctly
- Navigation between placeholder pages works
- MainWindow.xaml.cs remains minimal
- MainWindowViewModel handles only navigation

**Code Changes Allowed:** Yes (Shell infrastructure only)

**Tests Required:** Yes (Navigation logic)

**Report Required:** Yes

---

### Feature F2: Terminal Page Migration

**Goal:** Migrate existing serial terminal to TerminalPage

**Scope:**
- Create TerminalPage.xaml
- Create TerminalViewModel
- Move existing serial communication logic
- Preserve Feature A-D behavior
- Update Shell to include Terminal in navigation

**Forbidden:**
- Changing existing functionality behavior
- Removing features
- Breaking existing tests

**Acceptance Criteria:**
- All existing tests pass
- Terminal functionality identical to before
- MainWindowViewModel no longer contains terminal logic

**Code Changes Allowed:** Yes (refactoring only)

**Tests Required:** Yes (existing tests + new migration tests)

**Report Required:** Yes

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
     └──→ G1 (can start after E2)
```

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

---

*Last updated: May 2026*
