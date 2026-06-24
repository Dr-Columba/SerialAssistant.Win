# Feature G9H: RTU Transport Factory Core Contract Alignment

## Phase Summary

**Phase**: G9H - RTU Transport Factory Core Contract Alignment

**Status**: ✅ Completed

**Branch**: `feature/modbus-rtu-factory-contract-g9h`

**Parent Branch**: `main` (tag v0.4.9)

**Implementation Date**: 2026-06-24

**Test Baseline**: 742 → 750 (8 new tests)

**Version**: v0.4.9 (unchanged)

---

## Modified Files

### Files Created

| File | Purpose |
|------|---------|
| `src/SerialAssistant.Core/Modbus/Transport/IModbusRtuTransportFactory.cs` | Factory interface in Core layer |
| `src/SerialAssistant.Core/Modbus/Transport/ModbusRtuTransportFactoryOptions.cs` | Options DTO in Core layer |
| `docs/FeatureReports/FeatureG9H-RtuFactoryContract.md` | This report |

### Files Modified

| File | Changes |
|------|---------|
| `src/SerialAssistant.Infrastructure/Modbus/Transport/ModbusRtuTransportFactory.cs` | Implements Core interface |
| `src/SerialAssistant.Tests/Infrastructure/Modbus/ModbusRtuTransportFactoryTests.cs` | Added 8 new tests |
| `docs/ModbusTransportPlan.md` | Added G9H Contract Alignment Notes |
| `docs/ModbusPlan.md` | Added G9H status and test count |
| `docs/PhasePlan.md` | Updated G9H status to Completed |
| `docs/Architecture.md` | Added G9H contract architecture |
| `docs/ManualTestChecklist.md` | Added G9H verification checklist |
| `docs/FinalReview.md` | Added G9H review |

### Files Deleted

| File | Reason |
|------|--------|
| `src/SerialAssistant.Infrastructure/Modbus/Transport/ModbusRtuTransportFactoryOptions.cs` | Moved to Core layer to avoid dual DTO confusion |

---

## Scope Control

### In Scope

- ✅ `IModbusRtuTransportFactory` interface in Core
- ✅ `ModbusRtuTransportFactoryOptions` DTO in Core
- ✅ Infrastructure factory implements Core interface
- ✅ Delete Infrastructure version of options
- ✅ 8 unit tests
- ✅ Documentation updates

### Out of Scope (Forbidden)

- ❌ No `src/SerialAssistant.App` modifications
- ❌ No `src/SerialAssistant.App/MainWindow.xaml` modifications
- ❌ No `src/SerialAssistant.App/ViewModels/ModbusViewModel.cs` modifications
- ❌ No `src/SerialAssistant.Infrastructure/SerialPortService.cs` modifications
- ❌ No `src/SerialAssistant.Infrastructure/Services/SerialPortOwnershipCoordinator.cs` modifications
- ❌ No `src/SerialAssistant.Infrastructure/Modbus/Transport/ModbusRtuTransport.cs` modifications
- ❌ No `src/SerialAssistant.Infrastructure/Modbus/Transport/SystemIoPortsModbusRtuSerialAdapter.cs` modifications
- ❌ No version number changes
- ❌ No csproj/sln changes
- ❌ No README.md changes
- ❌ No UI file changes

---

## Contract Alignment Summary

### Before G9H

| Component | Location |
|-----------|----------|
| Factory Interface | None (Infrastructure concrete only) |
| Options DTO | Infrastructure layer |
| Factory Implementation | Infrastructure layer |
| ViewModel Dependency | Would depend on Infrastructure |

### After G9H

| Component | Location |
|-----------|----------|
| Factory Interface | Core layer (`IModbusRtuTransportFactory`) |
| Options DTO | Core layer (`ModbusRtuTransportFactoryOptions`) |
| Factory Implementation | Infrastructure layer (implements Core) |
| ViewModel Dependency | Can depend on Core only |

### Key Decision: Delete Infrastructure Options

**Reason**: Having two `ModbusRtuTransportFactoryOptions` classes (one in Core, one in Infrastructure) would cause:
- Namespace confusion
- Import ambiguity
- Dual DTO maintenance burden
- Potential for divergence

**Solution**: Delete Infrastructure version, use Core version everywhere.

---

## Factory Interface Design

### IModbusRtuTransportFactory

**Location**: `src/SerialAssistant.Core/Modbus/Transport/IModbusRtuTransportFactory.cs`

**Namespace**: `SerialAssistant.Core.Modbus.Transport`

**Purpose**: Factory contract for creating RTU transport, owned by Core layer

**Interface Definition**:

```csharp
public interface IModbusRtuTransportFactory
{
    IModbusRtuTransport Create(ModbusRtuTransportFactoryOptions options);
}
```

**Key Features**:

1. **Core Layer**: Interface in Core, not Infrastructure
2. **Returns Interface**: Returns `IModbusRtuTransport`, not concrete type
3. **Uses Core Options**: Parameter is Core options DTO
4. **No Infrastructure Reference**: Core does NOT reference Infrastructure
5. **No System.IO.Ports**: Core does NOT reference serial port APIs

---

## Options DTO Design

### ModbusRtuTransportFactoryOptions

**Location**: `src/SerialAssistant.Core/Modbus/Transport/ModbusRtuTransportFactoryOptions.cs`

**Namespace**: `SerialAssistant.Core.Modbus.Transport`

**Purpose**: Cross-layer DTO for factory input, owned by Core layer

**Properties**:

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| PortName | string | "" | Serial port name |
| BaudRate | int | 9600 | Baud rate |
| DataBits | int | 8 | Data bits (5-8) |
| Parity | string | "None" | Parity setting |
| StopBits | string | "One" | Stop bits setting |
| ReadTimeoutMilliseconds | int | 1000 | Read timeout |
| WriteTimeoutMilliseconds | int | 1000 | Write timeout |
| SendTimeoutMilliseconds | int | 1000 | Send timeout for transport |
| ReceiveTimeoutMilliseconds | int | 1000 | Receive timeout for transport |

**Validation**: `Validate()` method returns error message if invalid, null if valid.

---

## Test Summary

### Test File

**Location**: `src/SerialAssistant.Tests/Infrastructure/Modbus/ModbusRtuTransportFactoryTests.cs`

**Test Count**: 33 tests (25 existing + 8 new)

### New Tests Added

| Test | Purpose |
|------|---------|
| Factory_ImplementsCoreInterface | Verify factory implements Core interface |
| Interface_Create_ValidOptions_ReturnsTransport | Verify interface method works |
| Interface_Create_DoesNotOpenTransport | Verify interface method behavior |
| Interface_Create_DoesNotClaimOwnership | Verify interface method behavior |
| Options_DataBits_DefaultsTo8 | Verify default value |
| Options_Validate_ValidOptions_ReturnsTrue | Verify validation |
| Options_IsInCoreNamespace | Verify options is in Core namespace |
| Interface_IsInCoreNamespace | Verify interface is in Core namespace |

### Test Results

| Metric | Value |
|--------|-------|
| Total Tests | 750 passed |
| New Tests | 8 |
| Previous Tests | 742 |
| Failed Tests | 0 |

---

## Layer Boundary Compliance

### Verification Results

| Check | Status | Notes |
|-------|--------|-------|
| Core does NOT reference Infrastructure | ✅ Pass | Core interface and options have no Infrastructure reference |
| Core does NOT reference System.IO.Ports | ✅ Pass | Core has no serial port reference |
| No TcpClient/Socket in Core | ✅ Pass | No network references |
| No System.Windows in Core | ✅ Pass | No WPF references |
| No File./Directory. in Core | ✅ Pass | No file system access |
| No Registry in Core | ✅ Pass | No registry access |
| App layer unchanged | ✅ Pass | No App modifications |
| ModbusViewModel unchanged | ✅ Pass | No ViewModel modifications |
| SerialPortService unchanged | ✅ Pass | No SerialPortService modifications |
| SerialPortOwnershipCoordinator unchanged | ✅ Pass | No coordinator modifications |
| ModbusRtuTransport unchanged | ✅ Pass | No transport modifications |
| SystemIoPortsModbusRtuSerialAdapter unchanged | ✅ Pass | No adapter modifications |
| MainWindow.xaml unchanged | ✅ Pass | No UI modifications |

---

## Validation Result

### Build Status

| Check | Result |
|-------|--------|
| `dotnet build` | ✅ 0 warnings, 0 errors |
| `dotnet test` | ✅ 750 passed |

### Git Status

| Check | Result |
|-------|--------|
| `git diff --check` | ✅ Exit code 0 |
| `src/` modifications | ✅ Core (new), Infrastructure (modified), Tests (modified) |
| `src/SerialAssistant.App/` | ✅ No changes |
| `src/SerialAssistant.App/MainWindow.xaml` | ✅ No changes |
| `src/SerialAssistant.App/ViewModels/ModbusViewModel.cs` | ✅ No changes |
| Version | ✅ Still v0.4.9 |

---

## User Verification Commands

```powershell
# Branch verification
git branch --show-current
git status --short

# Build and test
git diff --check
echo $LASTEXITCODE

dotnet build .\SerialAssistant.Win.sln -c Debug
dotnet test .\SerialAssistant.Win.sln -c Debug

# Diff verification
git diff --name-status main..feature/modbus-rtu-factory-contract-g9h
git diff --stat main..feature/modbus-rtu-factory-contract-g9h

# Scope control verification
git diff --name-only -- src/SerialAssistant.App/
git diff --name-only -- src/SerialAssistant.App/MainWindow.xaml
git diff --name-only -- src/SerialAssistant.App/ViewModels/ModbusViewModel.cs
git diff --name-only -- src/SerialAssistant.Infrastructure/SerialPortService.cs
git diff --name-only -- src/SerialAssistant.Infrastructure/Services/SerialPortOwnershipCoordinator.cs
git diff --name-only -- src/SerialAssistant.Infrastructure/Modbus/Transport/ModbusRtuTransport.cs
git diff --name-only -- src/SerialAssistant.Infrastructure/Modbus/Transport/SystemIoPortsModbusRtuSerialAdapter.cs

# Layer boundary verification
Select-String -Path .\src\SerialAssistant.Core\Modbus\Transport\IModbusRtuTransportFactory.cs,.\src\SerialAssistant.Core\Modbus\Transport\ModbusRtuTransportFactoryOptions.cs -Pattern "System.IO.Ports","System.Windows","TcpClient","Socket","File.","Directory.","Registry","SerialAssistant.Infrastructure"

Select-String -Path .\src\SerialAssistant.App\**\*.cs -Pattern "System.IO.Ports","TcpClient","Socket","SystemIoPortsModbusRtuSerialAdapter","ModbusRtuTransportFactory"
```

---

## Final Recommendation

### Phase Status

**G9H**: ✅ Completed - Ready for User Verification

### What Was Accomplished

1. Core factory interface (`IModbusRtuTransportFactory`) created
2. Core options DTO (`ModbusRtuTransportFactoryOptions`) created
3. Infrastructure factory implements Core interface
4. Infrastructure options deleted (avoid dual DTO)
5. 8 comprehensive unit tests added
6. All layer boundaries respected
7. No forbidden modifications

### What Was NOT Accomplished (Deferred)

1. ModbusViewModel integration (G9I)
2. ModbusPage integration (G9J)
3. App startup factory registration (G9I)
4. UI integration (G9J)
5. Real serial port testing (G9K)

### Next Phase Recommendation

**Recommended**: G9I - ModbusViewModel RTU Factory Injection

**Why G9I Next**:
- G9H provides Core factory contract
- G9I can inject Core contract into ModbusViewModel
- G9I will add Connect/Send commands using factory
- G9I will NOT modify SerialPortService

**Do NOT Skip G9I**:
- ❌ Do NOT inject Infrastructure factory directly
- ❌ Do NOT bypass Core contract
- ❌ Do NOT create factory in ViewModel

---

## Report Status

- **Report File Path**: `docs/FeatureReports/FeatureG9H-RtuFactoryContract.md`
- **Code Modification Result**: Completed
- **Agent Auto Verification**: Passed
- **Test Count**: 750 passed
- **Requires User Verification**: Yes
- **Recommendation for Next Phase**: No, must be verified by user and accepted before proceeding to G9I

---

*Report created: 2026-06-24*
*Phase: G9H - RTU Transport Factory Core Contract Alignment Complete*