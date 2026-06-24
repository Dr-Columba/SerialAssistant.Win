# Feature G9E: RTU Transport Composition and UI Integration Planning

## Phase Summary

**Phase**: G9E - RTU Transport Composition and UI Integration Planning

**Status**: ✅ Completed

**Planning Date**: 2026-05-29

**Branch**: `feature/modbus-rtu-composition-planning-g9e`

**Parent Branch**: `main` (tag v0.4.9)

**Phase Type**: Documentation Only (No Code Changes)

## Modified Files

### New Files Created

| File | Location | Purpose |
|------|----------|---------|
| `FeatureG9E-RtuCompositionPlanning.md` | `docs/FeatureReports/` | This planning report |

### Modified Files

| File | Location | Change |
|------|----------|--------|
| `ModbusTransportPlan.md` | `docs/` | Added G9E RTU Composition Plan |
| `ModbusPlan.md` | `docs/` | Added G9E status |
| `PhasePlan.md` | `docs/` | Added G9E Completed, planned G9F-G9J |
| `Architecture.md` | `docs/` | Added G9E architecture planning |
| `ManualTestChecklist.md` | `docs/` | Added G9E verification checklist |
| `FinalReview.md` | `docs/` | Added G9E review |

## Scope Control

### In Scope ✅

- ✅ Document current transport assets
- ✅ Plan composition strategy
- ✅ Plan ownership strategy
- ✅ Plan UI integration strategy
- ✅ Define recommended next phases (G9F-G9J)
- ✅ Update all required documentation

### Out of Scope ❌

- ❌ No src/ modifications
- ❌ No test modifications
- ❌ No version number changes
- ❌ No UI implementation
- ❌ No factory/service/DI implementation
- ❌ No ViewModel modifications
- ❌ No real serial port integration

## Current Transport Assets

### Core Layer Assets

| Asset | Location | Purpose |
|-------|----------|---------|
| `IModbusRtuTransport` | `Core/Modbus/Transport/` | RTU transport interface |
| `ModbusTransportOptions` | `Core/Modbus/Transport/` | Transport configuration |
| `ModbusRequestContext` | `Core/Modbus/Transport/` | Request context |
| `ModbusTransportResult` | `Core/Modbus/Transport/` | Transport result |
| `ModbusTransportErrorCode` | `Core/Modbus/Transport/` | Error codes |
| `ISerialPortOwnershipCoordinator` | `Core/Services/` | Ownership coordinator interface |
| `SerialPortOwner` | `Core/Services/` | Owner enum (None, Terminal, ModbusRtu) |

### Infrastructure Layer Assets

| Asset | Location | Purpose |
|-------|----------|---------|
| `IModbusRtuSerialAdapter` | `Infrastructure/Modbus/Transport/` | Serial adapter interface |
| `ModbusRtuTransport` | `Infrastructure/Modbus/Transport/` | RTU transport implementation |
| `SystemIoPortsModbusRtuSerialAdapter` | `Infrastructure/Modbus/Transport/` | Real serial adapter |
| `FakeModbusRtuSerialAdapter` | `Tests/Infrastructure/Modbus/` | Test fake adapter |

### App Layer Assets

| Asset | Location | Purpose |
|-------|----------|---------|
| `ModbusViewModel` | `App/ViewModels/` | Modbus UI logic (unchanged) |
| `ModbusPage` | `App/Views/` | Modbus UI (unchanged) |
| `TerminalViewModel` | `App/ViewModels/` | Terminal logic (unchanged) |

### Current Composition Gap

```
┌─────────────────────────────────────────────────────────────────┐
│                        App Layer                                │
│  ┌─────────────────┐                                            │
│  │ ModbusViewModel │                                            │
│  │   - Has IModbusTransport? ❌ NO                             │
│  │   - Can create adapter? ❌ NO (App cannot use System.IO.Ports)│
│  └─────────────────┘                                            │
│                                                                 │
│  ❌ Missing: Composition Root / Factory                         │
│  ❌ Missing: Ownership Coordinator Implementation               │
└─────────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────────┐
│                    Infrastructure Layer                         │
│  ┌─────────────────────────────────────────────────────────┐   │
│  │ ModbusRtuTransport                                       │   │
│  │   - Accepts IModbusRtuSerialAdapter ✅                   │   │
│  │   - Accepts ISerialPortOwnershipCoordinator ✅           │   │
│  └─────────────────────────────────────────────────────────┘   │
│  ┌─────────────────────────────────────────────────────────┐   │
│  │ SystemIoPortsModbusRtuSerialAdapter                      │   │
│  │   - Implements IModbusRtuSerialAdapter ✅                │   │
│  │   - Uses System.IO.Ports ✅                              │   │
│  └─────────────────────────────────────────────────────────┘   │
│                                                                 │
│  ❌ Missing: Who creates the adapter?                           │
│  ❌ Missing: Who creates the transport?                         │
│  ❌ Missing: Who implements ownership coordinator?              │
└─────────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────────┐
│                        Core Layer                               │
│  ┌─────────────────┐  ┌─────────────────┐                      │
│  │ IModbusRtu      │  │ ISerialPort     │                      │
│  │ SerialAdapter   │  │ Ownership       │                      │
│  │ ✅              │  │ Coordinator ✅  │                      │
│  └─────────────────┘  └─────────────────┘                      │
│                                                                 │
│  ✅ Complete: All interfaces defined                            │
└─────────────────────────────────────────────────────────────────┘
```

## Composition Strategy

### Principle 1: App Does NOT Create Real Adapter

**Rule**: `ModbusViewModel` must NOT directly create `SystemIoPortsModbusRtuSerialAdapter`

**Reason**:
- App layer is forbidden from using `System.IO.Ports`
- Direct instantiation would violate layer boundaries
- ViewModel should only consume interfaces

**Solution**:
- Infrastructure provides factory or composition root
- App receives pre-configured `IModbusRtuTransport` via DI or service locator
- Factory creates adapter internally within Infrastructure

### Principle 2: ViewModel Only Consumes Interfaces

**Rule**: `ModbusViewModel` only knows about `IModbusTransport` / `IModbusRtuTransport`

**Reason**:
- ViewModel should not know about adapter implementation details
- ViewModel should not know about serial port specifics
- ViewModel should not know about ownership coordinator implementation

**Solution**:
- ViewModel receives `IModbusRtuTransport` as dependency
- ViewModel calls `ConnectAsync()`, `SendRequestAsync()`, `DisconnectAsync()`
- Transport handles adapter and ownership internally

### Principle 3: Core Does NOT Reference Infrastructure

**Rule**: Core interfaces remain pure, no Infrastructure references

**Reason**:
- Core is the contract layer
- Infrastructure implements Core contracts
- Core should not depend on implementation details

**Current State**: ✅ Compliant - Core has no Infrastructure references

### Principle 4: Infrastructure Does NOT Reference App

**Rule**: Infrastructure does not know about ViewModel or UI

**Reason**:
- Infrastructure provides services, not UI logic
- Infrastructure should not depend on WPF or App types
- Infrastructure is testable without App layer

**Current State**: ✅ Compliant - Infrastructure has no App references

### Recommended Composition Pattern

```
┌─────────────────────────────────────────────────────────────────┐
│                        App Layer                                │
│  ┌─────────────────┐                                            │
│  │ ModbusViewModel │                                            │
│  │   - IModbusRtuTransport (injected) ✅                       │
│  │   - Calls ConnectAsync/SendRequestAsync/DisconnectAsync     │
│  └─────────────────┘                                            │
│                                                                 │
│  Future: G9H - ViewModel receives transport via DI             │
└─────────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────────┐
│                    Infrastructure Layer                         │
│  ┌─────────────────────────────────────────────────────────┐   │
│  │ ModbusRtuTransportFactory (G9G)                          │   │
│  │   - Creates SystemIoPortsModbusRtuSerialAdapter          │   │
│  │   - Creates ModbusRtuTransport                            │   │
│  │   - Injects ownership coordinator                         │   │
│  └─────────────────────────────────────────────────────────┘   │
│  ┌─────────────────────────────────────────────────────────┐   │
│  │ SerialPortOwnershipCoordinatorImpl (G9F)                 │   │
│  │   - Implements ISerialPortOwnershipCoordinator           │   │
│  │   - Tracks current owner per port                         │   │
│  │   - Raises ownership changed events                       │   │
│  └─────────────────────────────────────────────────────────┘   │
│                                                                 │
│  Future: G9F - Ownership coordinator implementation            │
│  Future: G9G - Factory/composition root                        │
└─────────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────────┐
│                        Core Layer                               │
│  ✅ All interfaces defined (unchanged)                          │
└─────────────────────────────────────────────────────────────────┘
```

## Ownership Strategy

### Problem: Terminal vs Modbus RTU Port Conflict

**Scenario**:
1. User opens Terminal, connects to COM3
2. User switches to Modbus page, tries to connect to COM3 for RTU
3. **Conflict**: Both Terminal and Modbus want the same port

### Current State

- `ISerialPortOwnershipCoordinator` interface defined in Core ✅
- `SerialPortOwner` enum defines Terminal and ModbusRtu ✅
- `FakeSerialPortOwnershipCoordinator` exists in Tests ✅
- **Missing**: Real implementation in Infrastructure ❌

### Ownership Rules

| Rule | Description |
|------|-------------|
| Rule 1 | Terminal using port → Modbus RTU cannot claim |
| Rule 2 | Modbus RTU using port → Terminal cannot claim |
| Rule 3 | Owner must release before another can claim |
| Rule 4 | Ownership authority NOT in MainWindowViewModel |
| Rule 5 | Ownership coordinator in Infrastructure |

### Ownership Coordinator Location

**Forbidden Locations**:
- ❌ NOT in MainWindowViewModel (App layer should not manage ownership)
- ❌ NOT in Core (Core only defines interface)
- ❌ NOT in ViewModel base classes

**Required Location**:
- ✅ Infrastructure layer implements `ISerialPortOwnershipCoordinator`
- ✅ Singleton or scoped service
- ✅ Injected into both Terminal and Modbus transport

### Ownership Integration Flow

```
TerminalViewModel                    ModbusViewModel
     │                                    │
     │ Uses SerialPortService             │ Uses IModbusRtuTransport
     │                                    │
     ▼                                    ▼
SerialPortService                  ModbusRtuTransport
     │                                    │
     │ Should check ownership             │ Should check ownership
     │                                    │
     ▼                                    ▼
     └────────────┬───────────────────────┘
                  │
                  ▼
    SerialPortOwnershipCoordinatorImpl (Infrastructure)
                  │
                  ├─ TryClaimOwnership(port, owner)
                  ├─ TryReleaseOwnership(port, owner)
                  ├─ GetCurrentOwner(port)
                  └─ OwnershipChanged event
```

### Future: G9F Implementation

**G9F Scope**:
- Implement `SerialPortOwnershipCoordinatorImpl` in Infrastructure
- Inject into `SerialPortService` (Terminal)
- Inject into `ModbusRtuTransport` (Modbus)
- Add tests for real coordinator

## UI Integration Strategy

### Current UI State

- ModbusPage has minimal UI for request building ✅
- ModbusPage shows RequestHex, ResponseHex, ParsedSummary ✅
- **Missing**: RTU connection parameters ❌
- **Missing**: Connect/Disconnect buttons ❌
- **Missing**: Port selection for RTU ❌

### UI Integration Phases

**Phase G9I: Minimal RTU Parameter Binding**

Add minimal RTU parameters to ModbusPage:
- Port name selection (combo or text)
- Baud rate selection
- Data bits selection
- Parity selection (None/Odd/Even)
- Stop bits selection (One/Two)
- Connect/Disconnect button
- Connection status indicator

**NOT in G9I**:
- ❌ No complex state machine
- ❌ No UI beautification
- ❌ No advanced error display
- ❌ No transaction history

### UI Parameter Flow

```
┌─────────────────────────────────────────────────────────────────┐
│                        ModbusPage UI                            │
│  ┌─────────────────────────────────────────────────────────┐   │
│  │ RTU Connection Parameters                                │   │
│  │   - PortName: [COM3]                                     │   │
│  │   - BaudRate: [9600]                                     │   │
│  │   - DataBits: [8]                                        │   │
│  │   - Parity: [None]                                       │   │
│  │   - StopBits: [One]                                      │   │
│  │   - [Connect] [Disconnect]                               │   │
│  │   - Status: Connected / Disconnected                     │   │
│  └─────────────────────────────────────────────────────────┘   │
│  ┌─────────────────────────────────────────────────────────┐   │
│  │ Existing: Request Builder                                │   │
│  │   - UnitId, FunctionCode, Address, Quantity              │   │
│  │   - [Build Request]                                      │   │
│  │   - RequestHex display                                   │   │
│  └─────────────────────────────────────────────────────────┘   │
│  ┌─────────────────────────────────────────────────────────┐   │
│  │ Existing: Response Display                               │   │
│  │   - ResponseHex, ParsedSummary, StatusMessage            │   │
│  └─────────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────────┘

Binding Flow:
UI Parameters → ModbusViewModel → IModbusRtuTransport.ConnectAsync(portName, ...)
```

### UI Integration Order

| Step | Phase | Description |
|------|-------|-------------|
| 1 | G9F | Implement ownership coordinator |
| 2 | G9G | Create transport factory |
| 3 | G9H | Inject transport into ViewModel |
| 4 | G9I | Add minimal UI parameters |
| 5 | G9J | Manual hardware verification |

## Recommended Next Phases

### G9F: Infrastructure Serial Ownership Coordinator Implementation

**Goal**: Implement real ownership coordinator in Infrastructure

**Scope**:
- Create `SerialPortOwnershipCoordinatorImpl` in Infrastructure
- Implement `ISerialPortOwnershipCoordinator` interface
- Track ownership per port name
- Raise `OwnershipChanged` events
- Add unit tests

**Forbidden**:
- No App layer changes
- No UI changes
- No ViewModel changes

**Acceptance**:
- Coordinator implementation exists
- Tests pass
- No layer boundary violations

### G9G: RTU Transport Factory / Composition Root

**Goal**: Create factory that composes transport with adapter

**Scope**:
- Create `ModbusRtuTransportFactory` in Infrastructure
- Factory creates `SystemIoPortsModbusRtuSerialAdapter`
- Factory creates `ModbusRtuTransport` with adapter and coordinator
- Factory returns `IModbusRtuTransport`
- Add unit tests

**Forbidden**:
- No App layer changes
- No UI changes
- No ViewModel changes

**Acceptance**:
- Factory implementation exists
- Tests pass
- Factory does not expose System.IO.Ports types

### G9H: ModbusViewModel RTU Connect/Send Integration

**Goal**: Inject transport into ViewModel, add Connect/Send commands

**Scope**:
- Add `IModbusRtuTransport` dependency to ModbusViewModel
- Add `ConnectCommand` using transport
- Add `SendRequestCommand` using transport
- Add `DisconnectCommand` using transport
- Update DI registration

**Forbidden**:
- No direct adapter creation in ViewModel
- No System.IO.Ports in ViewModel
- No ownership logic in ViewModel

**Acceptance**:
- ViewModel uses injected transport
- Commands work with fake adapter in tests
- No layer boundary violations

### G9I: Minimal UI RTU Parameter Binding

**Goal**: Add minimal RTU connection parameters to UI

**Scope**:
- Add port name selection
- Add baud rate selection
- Add data bits selection
- Add parity selection
- Add stop bits selection
- Add Connect/Disconnect buttons
- Add connection status indicator

**Forbidden**:
- No complex state machine
- No UI beautification
- No advanced error display

**Acceptance**:
- UI parameters bind to ViewModel
- Connect/Disconnect buttons work
- Connection status displays correctly

### G9J: Manual RTU Hardware Verification

**Goal**: Verify RTU communication with real hardware

**Scope**:
- Manual test with real Modbus RTU device
- Manual test with virtual COM port pair
- Document verification results
- Update ManualTestChecklist

**Forbidden**:
- No automated hardware tests
- No changes to unit tests

**Acceptance**:
- Manual verification documented
- Real communication works
- Ownership conflict prevention verified

## Validation Result

| Check | Result |
|-------|--------|
| `git diff --check` | ✅ Pass (no trailing whitespace) |
| `dotnet build -c Debug` | ✅ Pass (0 errors, 0 warnings) |
| `dotnet test -c Debug` | ✅ Pass (686 tests) |
| No src changes | ✅ Verified |
| No test changes | ✅ Verified |
| Version unchanged | ✅ Still v0.4.9 |

## User Verification Commands

```powershell
git branch --show-current
git status --short

git diff --check
echo $LASTEXITCODE

dotnet build .\SerialAssistant.Win.sln -c Debug
dotnet test .\SerialAssistant.Win.sln -c Debug

git diff --name-status main..feature/modbus-rtu-composition-planning-g9e
git diff --stat main..feature/modbus-rtu-composition-planning-g9e

git diff --name-only -- src/
git diff --name-only -- src/SerialAssistant.Tests/

Select-String -Path .\docs\FeatureReports\FeatureG9E-RtuCompositionPlanning.md,.\docs\ModbusTransportPlan.md,.\docs\ModbusPlan.md,.\docs\PhasePlan.md,.\docs\Architecture.md,.\docs\ManualTestChecklist.md,.\docs\FinalReview.md -Pattern "G9E","G9F","G9G","G9H","G9I","G9J","686","v0.4.9","SystemIoPortsModbusRtuSerialAdapter","ISerialPortOwnershipCoordinator"
```

## Final Recommendation

**G9E Status**: ✅ Completed (Documentation Only)

**Build Status**: ✅ 0 warnings, 0 errors

**Test Status**: ✅ 686 tests passed (unchanged)

**Version Status**: ✅ Still v0.4.9 (no changes)

**Scope Compliance**: ✅ All constraints met

**Ready for Merge**: Yes - pending user local verification

**Next Phase**: G9F - Infrastructure Serial Ownership Coordinator Implementation

**Why G9F First**:
- Ownership coordinator is foundational for conflict prevention
- Must be implemented before factory can inject it
- Must be implemented before ViewModel can use transport
- Must be implemented before UI can show ownership state

**Do NOT Skip G9F**:
- ❌ Do NOT proceed to UI without ownership coordinator
- ❌ Do NOT proceed to factory without ownership coordinator
- ❌ Do NOT skip conflict prevention infrastructure

---

*Report Generated: 2026-05-29*
*Feature G9E: RTU Transport Composition and UI Integration Planning*