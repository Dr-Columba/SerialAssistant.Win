# Feature G9A Report: Modbus RTU Transport Existing Serial Service Capability Review

## Phase Summary

- **Phase Name**: Feature G9A: Modbus RTU Transport Existing Serial Service Capability Review
- **Status**: ✅ Completed
- **Implementation Date**: 2026-05-29
- **Previous Phase**: G8B - ModbusViewModel Transport Injection with Fake Tests
- **Next Phase**: G9B - Serial Port Ownership Coordinator Planning (Recommended)

## Scope Control

### In Scope

- ✅ Review existing ISerialPortService interface
- ✅ Review existing SerialPortService implementation
- ✅ Review TerminalViewModel serial port usage
- ✅ Gap analysis for Modbus RTU request-response
- ✅ Recommend RTU transport implementation strategy
- ✅ Plan G9B/G9C subsequent phases
- ✅ Documentation only, no code changes

### Out of Scope

- ❌ No src changes
- ❌ No tests changes
- ❌ No Infrastructure changes
- ❌ No App layer changes
- ❌ No UI changes
- ❌ No version changes

## Files Inspected

| File | Path | Purpose |
|------|------|---------|
| ISerialPortService.cs | src/SerialAssistant.Core/Services/ISerialPortService.cs | Serial port service interface |
| SerialPortService.cs | src/SerialAssistant.Infrastructure/Serial/SerialPortService.cs | Serial port service implementation |
| TerminalViewModel.cs | src/SerialAssistant.App/ViewModels/TerminalViewModel.cs | Terminal serial usage |
| SerialReceiveData.cs | src/SerialAssistant.Core/Models/SerialReceiveData.cs | Serial receive data model |
| SerialPortSettings.cs | src/SerialAssistant.Core/Models/SerialPortSettings.cs | Serial port settings model |
| SerialConnectionState.cs | src/SerialAssistant.Core/Enums/SerialConnectionState.cs | Connection state enum |

## Existing Serial Interface Review

### ISerialPortService Capabilities

| Capability | Status | Notes |
|------------|--------|-------|
| Connection State | ✅ Yes | SerialConnectionState property |
| Open() | ✅ Yes | Synchronous open |
| Close() | ✅ Yes | Synchronous close |
| Send() | ✅ Yes | Synchronous send |
| DataReceived Event | ✅ Yes | Event-based receive |
| ErrorOccurred Event | ✅ Yes | Error event |
| ConnectionStateChanged Event | ✅ Yes | State change event |
| SendAndReceiveAsync | ❌ No | Not supported |
| CancellationToken Support | ❌ No | Not supported |
| ReceiveTimeout Setting | ⚠️ Partial | Only in SerialPortSettings (passed to SerialPort) |
| Port Ownership Tracking | ❌ No | Not supported |
| Owner Identification | ❌ No | Not supported |

### ISerialPortService Signatures

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

## Existing SerialPortService Review

### Implementation Details

| Aspect | Details |
|--------|---------|
| **Receive Mechanism** | Fully event-based via SerialPort.DataReceived |
| **Internal Buffer** | No internal buffer in SerialPortService |
| **Frame Detection** | No frame boundary detection |
| **Request-Response** | No request-response matching |
| **Concurrency Control** | No concurrency control |
| **UI Thread Dependency** | Event handlers called on arbitrary thread |
| **Timeout** | SerialPort.ReadTimeout/WriteTimeout set in Open() |
| **State Tracking** | Only tracks connection state |

### SerialPortService Read Logic

```csharp
private void OnSerialPortDataReceived(object sender, SerialDataReceivedEventArgs e)
{
    // Reads whatever is available immediately
    // No frame matching, no request correlation
    // No timeout control beyond SerialPort.ReadTimeout
    // No request-response matching
}
```

### SerialPortService Limitations for Modbus RTU

| Limitation | Impact |
|------------|--------|
| **Event-Based Only** | Cannot wait for specific response frame |
| **No Request Correlation** | Cannot match response to request |
| **No Timeout Control** | Cannot implement per-request timeout |
| **No Cancellation** | Cannot cancel ongoing receive |
| **No Frame Boundary** | Cannot detect Modbus RTU frame boundaries |
| **No Ownership Tracking** | Cannot prevent Terminal/Modbus conflict |

## Terminal Usage Review

### TerminalViewModel Serial Port Usage

| Operation | How Terminal Does It |
|-----------|---------------------|
| **Open Serial** | Calls ISerialPortService.Open(settings) |
| **Send Data** | Calls ISerialPortService.Send(byte[]) |
| **Receive Data** | Subscribes to ISerialPortService.DataReceived event |
| **Close Serial** | Calls ISerialPortService.Close() |
| **Ownership Control** | None - no tracking |

### Terminal/Modbus Conflict Scenario

If both Terminal and Modbus try to use same COM port:

1. **Current Behavior**: No prevention mechanism
2. **Problem**: DataReceived events received by both
3. **Risk**: Modbus responses mixed with Terminal data
4. **Risk**: Unpredictable behavior
5. **Current Protection**: SerialPortService checks "串口已打开" only for itself

### Current Ownership Status

| Feature | Status |
|---------|--------|
| **Port Ownership Tracking** | ❌ Not exists |
| **Owner Identification** | ❌ Not exists |
| **Conflict Prevention** | ❌ Not exists |
| **Exclusive Access** | ❌ Not exists |

## Gap Analysis

### Critical Gaps for Modbus RTU

| Gap | Severity | Description |
|-----|----------|-------------|
| **No SendAndReceiveAsync** | 🔴 High | Cannot implement request-response pattern |
| **No Timeout Per Request** | 🔴 High | Cannot wait for response with timeout |
| **No Cancellation** | 🔴 High | Cannot cancel ongoing operation |
| **No Frame Matching** | 🟡 Medium | Cannot match response to request |
| **No Port Ownership** | 🔴 High | Cannot prevent Terminal/Modbus conflict |
| **No Async Support** | 🟡 Medium | All methods synchronous only |

### ISerialPortService vs Modbus RTU Requirements

| Requirement | Current ISerialPortService | Need for Modbus RTU |
|-------------|---------------------------|--------------------|
| Open/Close | ✅ Yes | ✅ Yes |
| Send | ✅ Yes | ✅ Yes |
| Event Receive | ✅ Yes | ❌ No (need request-response) |
| SendAndReceiveAsync | ❌ No | ✅ Required |
| Timeout Control | ⚠️ Partial | ✅ Required per request |
| Cancellation | ❌ No | ✅ Required |
| Port Ownership | ❌ No | ✅ Required |

## Recommended RTU Transport Strategy

### Recommendation: Option C - New ModbusRtuTransport with Ownership Coordinator

**Primary Recommendation**: Option C

> **C. 新增 ModbusRtuTransport，内部组合现有低层串口能力 + 新增串口所有权协调服务**

### Why Option C?

| Reason | Explanation |
|--------|-------------|
| **No Terminal Breakage** | Existing Terminal continues to use ISerialPortService unchanged |
| **Clean Architecture** | Modbus-specific logic isolated to ModbusRtuTransport |
| **Testability** | Fake serial adapter possible without changing Infrastructure |
| **Layer Boundaries** | App only references Core transport interfaces |
| **Ownership Control** | Explicit ownership coordination possible |

### Why Not Other Options?

| Option | Why Not Recommended |
|--------|-------------------|
| A. Reuse SerialPortService | ❌ Cannot easily add request-response without breaking Terminal |
| B. Extend SerialPortService | ❌ Risks breaking existing Terminal behavior |
| D. Only Ownership Coordinator | ❌ Still need Modbus-specific transport logic |

## Ownership Strategy

### Recommended Ownership Model

1. **Single Port Ownership**: Only one owner (Terminal OR Modbus) at a time
2. **Explicit Ownership**: SerialPortOwner enum identifies the owner, and ISerialPortOwnershipCoordinator manages ownership
3. **Ownership Coordinator**: New ISerialPortOwnershipCoordinator in Core
4. **Owner Types**: "Terminal" and "ModbusRtu"

### Ownership Flow

```
User wants to use Modbus RTU
    ↓
Check if port is owned
    ↓
If owned by Terminal:
    - Prompt user to close Terminal
    - Or auto-close Terminal
    ↓
If not owned:
    - Claim ownership for Modbus
    - Open port
    - Do Modbus communication
    - Release ownership when done
```

### Recommended Interfaces (Core Layer)

```csharp
// Core layer only - no Infrastructure reference
public enum SerialPortOwner
{
    None,
    Terminal,
    ModbusRtu
}

public interface ISerialPortOwnershipCoordinator
{
    SerialPortOwner GetCurrentOwner(string portName);
    bool TryClaimOwnership(string portName, SerialPortOwner owner);
    void ReleaseOwnership(string portName, SerialPortOwner owner);
    event EventHandler<SerialPortOwner>? OwnershipChanged;
}
```

## Risks

| Risk | Severity | Mitigation |
|------|----------|------------|
| **Terminal Behavior Change** | 🟡 Medium | Keep ISerialPortService unchanged for Terminal |
| **Complexity Increase** | 🟡 Medium | Phase-based implementation (G9B → G9C → G9D) |
| **Real Hardware Testing** | 🟡 Medium | Fake-based tests first, then manual verification |
| **Regression Risk** | 🟢 Low | Existing Terminal tests should pass |

## Next Phase Recommendation

### Recommended Next Phase: G9B

**G9B: Serial Port Ownership Coordinator Contracts**

| Phase | Purpose |
|-------|---------|
| G9B | Define ISerialPortOwnershipCoordinator in Core |
| G9C | Implement ModbusRtuTransport with fake serial adapter |
| G9D | Modbus RTU Transport Manual Verification |

### Why G9B First?

1. **Ownership is Critical**: Must prevent Terminal/Modbus conflict
2. **Core First**: Define contracts before implementation
3. **Testability**: Fake coordinator possible
4. **Layer Boundaries**: Core contracts = clean architecture

## Validation Result

### Automated Validation

| Check | Command | Result |
|-------|---------|--------|
| Git diff check | `git diff --check` | ✅ Passed |
| Build | `dotnet build .\SerialAssistant.Win.sln -c Debug` | ✅ Passed (0 warnings, 0 errors) |
| Tests | `dotnet test .\SerialAssistant.Win.sln -c Debug` | ✅ Passed (586 tests) |
| No src changes | `git diff --name-only -- src/` | ✅ None |
| No tests changes | `git diff --name-only -- src/SerialAssistant.Tests/` | ✅ None |

## User Verification Commands

```powershell
git branch --show-current
git status --short

git diff --check
echo $LASTEXITCODE

dotnet build .\SerialAssistant.Win.sln -c Debug
dotnet test .\SerialAssistant.Win.sln -c Debug

git diff --name-status main..feature/modbus-rtu-transport-review-g9a
git diff --stat main..feature/modbus-rtu-transport-review-g9a

git diff --name-only -- src/
git diff --name-only -- src/SerialAssistant.Tests/

Select-String -Path .\src\SerialAssistant.Core\**\*.cs,.\src\SerialAssistant.Infrastructure\**\*.cs,.\src\SerialAssistant.App\ViewModels\TerminalViewModel.cs -Pattern "ISerialPortService","SerialPortService","DataReceived","Send","Open","Close","SerialPort","BytesToRead","Read","Write","CancellationToken","Timeout"

Select-String -Path .\docs\ModbusTransportPlan.md,.\docs\Architecture.md,.\docs\PhasePlan.md,.\docs\FinalReview.md,.\docs\FeatureReports\FeatureG9A-ModbusRtuTransportReview.md -Pattern "G9A","G9B","G9C","RTU","SerialPortService","ISerialPortService","ownership","SendAndReceiveAsync","request-response","586"
```

## Final Recommendation

### Phase Status

✅ G9A Complete - Ready for User Verification

### Key Findings

1. **ISerialPortService Limitations**: Event-based only, no request-response
2. **No Ownership Control**: Cannot prevent Terminal/Modbus conflict
3. **Need New Strategy**: Recommend Option C - new ModbusRtuTransport + ownership coordinator
4. **Phase G9B First**: Define ownership contracts in Core

### Recommendations for User

1. **Review Report**: Read FeatureG9A report carefully
2. **Accept Recommendation**: Confirm Option C approach
3. **Proceed to G9B**: Implement ownership coordinator contracts
4. **No Code Changes Yet**: G9A is documentation only
5. **Maintain 586 Tests**: No test count change in G9A

### Next: G9B

**G9B: Serial Port Ownership Coordinator Contracts**

- Define ISerialPortOwnershipCoordinator in Core
- Define SerialPortOwner enum
- No Infrastructure changes yet
- Fake-based tests for coordinator

---

## Fix Notes (Documentation Alignment Fix)

### Fix Date: 2026-05-29

### Issues Fixed

#### Issue 1: ModbusTransportPlan.md Strategy Conflict

**Problem**: 
- ModbusTransportPlan.md previously recommended "Option 1: Reuse Existing SerialPortService"
- But G9A Review Notes recommended "Option C: New ModbusRtuTransport + Ownership Coordinator"
- These conclusions conflicted, would mislead G9B/G9C implementation

**Resolution**:
- Marked Option 1 as "Original G7 assumption - superseded"
- Added G9A finding: "SerialPortService is event-based only, lacks SendAndReceiveAsync"
- Added "Direct reuse NOT recommended for Modbus request-response pattern"
- Added full Option C documentation with rationale
- Removed "Start with Option 1 for G9" recommendation

#### Issue 2: Missing Documentation Updates

**Problem**:
- G9A required updates to multiple documentation files
- Initial implementation only partially completed

**Resolution**:
- Updated Architecture.md with G9A capability review section
- Updated ManualTestChecklist.md with G9A verification steps
- Updated FinalReview.md with G9A review section
- Updated ModbusPlan.md with G9A status section
- Updated FeatureG9A report with Fix Notes

### Files Modified in Fix

| File | Change |
|------|--------|
| docs/ModbusTransportPlan.md | Option 1 marked as superseded, Option C documented |
| docs/Architecture.md | Added G9A capability review section |
| docs/ManualTestChecklist.md | Added G9A verification section (31 steps) |
| docs/FinalReview.md | Added G9A review section |
| docs/ModbusPlan.md | Added G9A status section |
| docs/FeatureReports/FeatureG9A-ModbusRtuTransportReview.md | Added Fix Notes |

### Files NOT Modified (No Code Changes)

| Path | Status |
|------|--------|
| src/ | ✅ No changes |
| src/SerialAssistant.Tests/ | ✅ No changes |
| *.csproj | ✅ No changes |
| *.sln | ✅ No changes |
| README.md | ✅ No changes |
| MainWindow.xaml | ✅ No changes |
| Version numbers | ✅ No changes |

### Validation Results

| Check | Command | Expected | Result |
|-------|---------|----------|--------|
| Git diff check | `git diff --check` | 0 errors | ✅ Passed |
| Build | `dotnet build` | 0 errors | ✅ Passed |
| Tests | `dotnet test` | 586 passed | ✅ Passed |
| src/ diff | `git diff --name-only -- src/` | Empty | ✅ Empty |
| tests/ diff | `git diff --name-only -- src/SerialAssistant.Tests/` | Empty | ✅ Empty |

### Strategy Summary After Fix

| Item | Status | Notes |
|------|--------|-------|
| Option 1 (Reuse SerialPortService) | ❌ Superseded | Event-based model incompatible |
| Option 2 (Extend SerialPortService) | ❌ Superseded | Risks breaking Terminal |
| Option C (New ModbusRtuTransport) | ✅ Recommended | Clean architecture, no Terminal risk |
| G7 assumption | ✅ Superseded | G9A review provides updated guidance |
| G9B | ✅ Next | Ownership coordinator contracts |
| G9C | ⏳ Pending | ModbusRtuTransport with fake |
| G9D | ⏳ Pending | Manual verification |

### Key Messages After Fix

1. **Do NOT reuse SerialPortService directly for Modbus RTU**
2. **Do NOT extend SerialPortService**
3. **Do NOT modify Terminal behavior**
4. **Option C is the only recommended approach**
5. **G9B must come before G9C**
6. **App layer is still forbidden System.IO.Ports**
7. **MainWindowViewModel must NOT be the ownership authority**
8. **Core ownership coordinator contract first (G9B), then Infrastructure implementation**

### Additional Fix Notes

- Removed remaining App-layer ownership recommendation from ModbusTransportPlan.md
- Marked MainWindowViewModel ownership tracking as superseded in Architecture.md
- Updated Validation Results table to show ✅ Passed instead of ⏳ Pending
- Confirmed ownership coordinator contract belongs to Core in G9B
- Confirmed Infrastructure provides concrete ownership implementation later
- Test count remains 586 passed
- No src changes
- No tests changes

---

**Report Created**: 2026-05-29
**Report Updated**: 2026-05-29 (Fix Notes Added)
**Report Author**: AI Assistant
**Phase Lead**: User
**Next Phase**: G9B - Serial Port Ownership Coordinator Contracts (Recommended)
