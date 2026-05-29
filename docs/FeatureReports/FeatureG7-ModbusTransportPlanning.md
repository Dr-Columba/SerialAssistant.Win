# Feature G7 Report: Modbus Transport Integration Planning

## Phase Summary

- **Phase Name**: Feature G7: Modbus Transport Integration Planning
- **Status**: Ready for Verification
- **Implementation Date**: 2026-05-28
- **Previous Phase**: G6 - Modbus Manual Test and Documentation Closure
- **Next Phase**: G8 - Modbus Transport Interfaces and Fake Tests (recommended)

## Modified Files

| File | Action |
|------|--------|
| docs/ModbusTransportPlan.md | ✅ Created/Updated |
| docs/ModbusPlan.md | ✅ Updated |
| docs/PhasePlan.md | ✅ Updated |
| docs/Architecture.md | ✅ Updated |
| docs/UIInformationArchitecture.md | ✅ Updated |
| docs/ManualTestChecklist.md | ✅ Updated |
| docs/FinalReview.md | ✅ Updated |
| docs/FeatureReports/FeatureG7-ModbusTransportPlanning.md | ✅ Created |

## Scope Control

### In Scope

✅ Transport architecture planning
✅ Interface design (IModbusTransport, etc.)
✅ RTU transport strategy
✅ TCP transport strategy
✅ Serial port ownership model
✅ Error strategy
✅ Test strategy (fake transport)
✅ Phase breakdown (G8-G12)
✅ Documentation updates

### Out of Scope

❌ Code implementation
❌ Test implementation
❌ UI implementation
❌ Version number changes
❌ Third-party library integration
❌ Real hardware testing

## Transport Planning Summary

### Core Objectives

1. **Layer Boundary Preservation**: Maintain strict separation between Core, App, and Infrastructure
2. **Interface Abstraction**: Define clean interfaces to isolate App layer from IO details
3. **Ownership Management**: Prevent Terminal/Modbus serial port conflicts
4. **Testability**: Enable automated testing without real hardware
5. **Incremental Implementation**: Break work into manageable phases (G8-G12)

### Key Principles

| Principle | Description |
|-----------|-------------|
| **No App IO** | App layer never directly references System.IO.Ports or TcpClient |
| **Core Only Protocol** | CRC, framing, parsing stay in Core layer |
| **Infrastructure Only IO** | All serial/TCP operations in Infrastructure |
| **Single Ownership** | Terminal and Modbus RTU cannot share serial port |
| **Fake First** | Implement interfaces with fake transport before real hardware |

## RTU Planning Summary

### Strategy

- **Reuse SerialPortService**: Modbus RTU will use existing Infrastructure SerialPortService
- **Single Ownership Model**: Terminal and Modbus RTU cannot use serial port simultaneously
- **Ownership Coordination**: MainWindowViewModel tracks connection state

### Flow

1. User selects RTU mode and configures serial port
2. User clicks Connect (disabled if Terminal is connected)
3. ModbusRtuTransport opens port via SerialPortService
4. User builds and sends request
5. Response received and parsed by Core layer
6. Results displayed in UI

### Error Handling

- Port not selected → Validation error
- Port already in use → Friendly message
- Open failed → Status message
- Send failed → Status message
- Timeout → Status message
- CRC invalid → Core parser error

## TCP Planning Summary

### Strategy

- **New ModbusTcpTransport**: Create dedicated TCP transport in Infrastructure
- **TcpClient Usage**: Isolated to Infrastructure layer only
- **MBAP Validation**: TransactionId matching in transport

### Flow

1. User selects TCP mode and enters IP/Port
2. User clicks Connect
3. ModbusTcpTransport connects via TcpClient
4. User builds and sends request with TransactionId
5. Response received, TransactionId validated
6. Results displayed in UI

### Error Handling

- Invalid IP/Port → Validation error
- Connect failed → Status message
- TCP disconnected → Status message
- TransactionId mismatch → Protocol error
- Timeout → Status message

## Layer Boundary Review

### Current Architecture State

```
┌─────────────────────────────────────────────────────────────────┐
│  App Layer (SerialAssistant.App)                                │
│  ├── ModbusViewModel                                           │
│  │   └── Uses IModbusTransport interface                       │
│  └── ModbusPage                                                │
├─────────────────────────────────────────────────────────────────┤
│  Core Layer (SerialAssistant.Core)                              │
│  ├── Modbus/ (protocol only)                                  │
│  └── Services/ (interface definitions)                        │
├─────────────────────────────────────────────────────────────────┤
│  Infrastructure Layer (SerialAssistant.Infrastructure)           │
│  ├── SerialPortService (existing)                             │
│  └── Modbus transports (future: ModbusRtuTransport,           │
│                          ModbusTcpTransport)                   │
└─────────────────────────────────────────────────────────────────┘
```

### Boundary Compliance

| Layer | Allowed | Forbidden |
|-------|---------|-----------|
| Core | Protocol framing, CRC, parsing | System.IO.Ports, TcpClient, WPF |
| App | ViewModels, UI binding, commands | Direct IO references |
| Infrastructure | Serial/TCP IO | WPF, Dispatcher |

## Risk Review

### Identified Risks

| Risk | Severity | Mitigation |
|------|----------|-----------|
| Serial port ownership conflict | High | Single ownership model |
| RTU response length detection | Medium | Test with multiple devices |
| TCP half-open state | Medium | Timeout and heartbeat |
| UI blocking during send | Medium | Proper async/await |
| Tests requiring hardware | Low | Fake transport pattern |
| App layer IO pollution | High | Code reviews, validation gates |
| Premature UI polish | Medium | Defer to H phase |

### Key Decisions

| Decision | Rationale |
|----------|-----------|
| G8 First | Lock down interfaces before real implementation |
| Single Ownership | Simple and predictable |
| Fake Transport | Enable testing without hardware |
| Defer UI Styling | Function first, aesthetics later |

## Next Phase Recommendation

**Recommended**: G8 - Modbus Transport Interfaces and Fake Tests

**Why G8 First**:
1. Lock down interface contracts before real implementation
2. Prove ViewModel can work with transport via fakes
3. Validate architecture before committing to hardware
4. Reduce risk of App layer pollution
5. Enable automated testing without hardware dependency

**Phase Sequence**:
- G8 → G9 → G10 → G11 → G12

## Validation Result

### Automated Validation

| Check | Command | Result |
|-------|---------|--------|
| Git diff check | `git diff --check` | ✅ Passed (no trailing whitespace) |
| Build | `dotnet build .\SerialAssistant.Win.sln -c Debug` | ✅ Passed (0 warnings, 0 errors) |
| Tests | `dotnet test .\SerialAssistant.Win.sln -c Debug` | ✅ Passed (520 tests) |
| No src changes | `git diff --name-only -- src/` | ✅ Passed (empty) |
| No tests changes | `git diff --name-only -- src/SerialAssistant.Tests/` | ✅ Passed (empty) |

### Current State

- **Test Count**: 520 passed (unchanged from G6)
- **Version**: v0.4.4 (unchanged)
- **Branch**: feature/modbus-transport-plan-g7
- **Code Changes**: None (documentation only)

### Fix Notes (May 2026)

1. **Clarified Interface Ownership**: All transport interfaces and DTOs placed in Core layer only
2. **Removed App Layer Reference from Core**: ModbusRequestContext no longer references App's ModbusTransportMode
3. **Fixed Markdown Format**: Corrected missing closing tags, parentheses, and table formatting in Architecture.md
4. **Updated Validation Commands**: Changed tests directory check from `tests/` to `src/SerialAssistant.Tests/`
5. **Removed remaining "Core/App" ambiguity**: Removed all "Core/App", "Core or App", "if not in Core/App" wording
6. **Replaced "App layer sees ..." with explicit "App layer must NOT see ..."**: Updated Forbidden sections for clarity
7. **Confirmed Infrastructure contains concrete implementations only**: Removed interface references from Infrastructure structure
8. **No Code Changes**: Only documentation updates, no src/tests modifications
9. **Test count remains 520 passed**: No changes to test code

## User Verification Commands

```powershell
git branch --show-current
git status --short

git diff --check
echo $LASTEXITCODE

dotnet build .\SerialAssistant.Win.sln -c Debug
dotnet test .\SerialAssistant.Win.sln -c Debug

git diff --name-only -- src/
git diff --name-only -- src/SerialAssistant.Tests/

git diff --name-status main..feature/modbus-transport-plan-g7
git diff --stat main..feature/modbus-transport-plan-g7

Select-String -Path .\docs\ModbusTransportPlan.md,.\docs\ModbusPlan.md,.\docs\PhasePlan.md,.\docs\Architecture.md,.\docs\FinalReview.md -Pattern "G7","G8","RTU","TCP","Transport","System.IO.Ports","TcpClient","Infrastructure","ownership","timeout"
```

## Final Recommendation

### Phase Status

✅ G7 Complete - Ready for User Verification

### Recommendations

1. **User Verification Required**: Run the verification commands above before proceeding
2. **No Code Changes**: This is a documentation-only phase
3. **Proceed to G8**: After verification, move to G8 - Modbus Transport Interfaces and Fake Tests
4. **Do NOT Skip G8**: Critical to validate architecture before real hardware implementation

### Key Success Metrics

- ✅ All planning documentation complete
- ✅ G8-G12 phases clearly defined
- ✅ Layer boundaries preserved
- ✅ Serial port ownership strategy defined
- ✅ Error strategy defined
- ✅ Test strategy defined
- ✅ No code changes (pure documentation)
- ✅ Test count unchanged at 520 passed

---

**Report Created**: 2026-05-28
**Report Author**: AI Assistant
**Phase Lead**: User
**Next Phase**: G8 - Modbus Transport Interfaces and Fake Tests