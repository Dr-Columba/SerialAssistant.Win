# Modbus Transport Integration Plan

## Purpose

本文档用于规划 Modbus RTU/TCP 真实通信接入方案，不实现任何代码。本规划仅用于明确设计边界、接口约定、职责划分，为后续 G8-G12 阶段提供指导。

## Current State

### What we have

- ✅ **Core Modbus RTU/TCP frame builder/parser** - 协议构建、解析已在 Core 层完成
- ✅ **ModbusViewModel workflow** - BuildRequest、ParseResponse、Clear 命令已实现
- ✅ **ModbusPage minimal UI** - 参数选择、HEX 显示、解析结果展示已可用
- ✅ **Terminal serial service exists** - Infrastructure.SerialPortService 已实现串口通信
- ✅ **Infrastructure layer exists** - 已有基础设施抽象和实现

### What we do NOT have

- ❌ **Modbus real communication** - 当前只能构建请求、解析响应，不执行真实发送/接收
- ❌ **Modbus Transport layer** - 尚无 RTU/TCP 具体 Transport 实现
- ❌ **ModbusViewModel transport integration** - ModbusViewModel 尚未与真实 IO 层集成
- ❌ **Serial port ownership model** - Terminal 和 Modbus 如何共享/隔离串口尚未规划

## Target Capability

### Minimum Viable Communication

After this planning phase and implementation in subsequent phases:

#### RTU Mode
1. User selects TransportMode = RTU
2. User configures serial port (reuse Terminal config or separate UI)
3. User builds Modbus request (UnitId, FunctionCode, Address, Quantity, etc.)
4. User clicks "Send Request"
5. RTU Transport sends frame via serial port
6. RTU Transport receives response
7. Response is parsed by Core layer
8. UI shows RequestHex, ResponseHex, ParsedSummary, StatusMessage

#### TCP Mode
1. User selects TransportMode = TCP
2. User enters IP Address, Port
3. User connects to Modbus TCP Server
4. User builds Modbus request
5. User clicks "Send Request"
6. TCP Transport sends MBAP frame
7. TCP Transport receives MBAP response
8. Response is parsed by Core layer
9. UI shows RequestHex, ResponseHex, ParsedSummary, StatusMessage

## Layer Boundary Design

### Strict Separation

**This is non-negotiable**

| Layer | Responsibility | What it MUST NOT have |
|-------|----------------|----------------------|
| **Core** | Protocol framing, CRC, parsing, validation | System.IO.Ports, TcpClient, Socket, WPF, File I/O |
| **App** | ViewModels, UI binding, command flow, error display | Direct System.IO.Ports, direct TcpClient/Socket |
| **Infrastructure** | Serial/TCP IO, Transport implementations | WPF, Window, Dispatcher, UI references |

### Call Flow

```
ModbusPage (UI)
    |
    | Binds to
    |
    v
ModbusViewModel
    |
    | Calls
    |
    v
IModbusTransport (Interface)
    |
    | Implemented by
    |
    +---> ModbusRtuTransport (Infrastructure)
    |       |
    |       +---> SerialPortService (Infrastructure)
    |
    +---> ModbusTcpTransport (Infrastructure)
            |
            +---> TcpClient (Infrastructure only)
```

**Key Rule:** ModbusViewModel NEVER sees SerialPort or TcpClient directly. It only uses IModbusTransport interface.

## Proposed Interfaces

### Interface Placement

| Interface | Layer | Location |
|-----------|-------|----------|
| IModbusTransport | Core | src/SerialAssistant.Core/Services/Modbus/ 或 src/SerialAssistant.Core/Modbus/Transport/ |
| IModbusRtuTransport | Core | Same as above |
| IModbusTcpTransport | Core | Same as above |
| ModbusTransportResult | Core | Same as above |
| ModbusTransportOptions | Core | Same as above |
| ModbusRequestContext | Core | Same as above |

**Key Rules (Non-Negotiable):**
1. **Interfaces in Core layer only** - Core defines service contracts for both App and Infrastructure
2. **Core DTOs never reference App layer** - ModbusRequestContext does NOT use App layer's ModbusTransportMode
3. **Core never references App** - Strict layer separation
4. **Infrastructure implements Core interfaces** - No Infrastructure references App
5. **App only consumes Core interfaces** - App never directly references System.IO.Ports or TcpClient

### IModbusTransport

```csharp
// Concept only - NOT for implementation in G7
public interface IModbusTransport
{
    Task<ModbusTransportResult> SendRequestAsync(
        ModbusRequestContext context,
        byte[] requestBytes,
        CancellationToken cancellationToken = default);
    
    Task<bool> ConnectAsync(CancellationToken cancellationToken = default);
    Task DisconnectAsync();
    bool IsConnected { get; }
    ModbusTransportOptions Options { get; }
}
```

**Responsibility:** Abstraction for sending Modbus requests and receiving responses, regardless of RTU/TCP.

### IModbusRtuTransport

```csharp
// Concept only - NOT for implementation in G7
public interface IModbusRtuTransport : IModbusTransport
{
    // RTU-specific methods or properties if needed
}
```

**Responsibility:** RTU-specific transport abstraction.

### IModbusTcpTransport

```csharp
// Concept only - NOT for implementation in G7
public interface IModbusTcpTransport : IModbusTransport
{
    string IpAddress { get; }
    int Port { get; }
}
```

**Responsibility:** TCP-specific transport abstraction.

### ModbusTransportResult

```csharp
// Concept only - NOT for implementation in G7
public class ModbusTransportResult
{
    public bool Success { get; set; }
    public byte[]? ResponseBytes { get; set; }
    public string? ErrorMessage { get; set; }
    public TimeSpan Duration { get; set; }
}
```

### ModbusTransportOptions

```csharp
// Concept only - NOT for implementation in G7
public class ModbusTransportOptions
{
    public TimeSpan SendTimeout { get; set; } = TimeSpan.FromSeconds(5);
    public TimeSpan ReceiveTimeout { get; set; } = TimeSpan.FromSeconds(5);
    public bool ValidateResponse { get; set; } = true;
}
```

### ModbusRequestContext

```csharp
// Concept only - NOT for implementation in G7
public class ModbusRequestContext
{
    // Mode NOT in Core DTO - use specific interfaces instead (IModbusRtuTransport, IModbusTcpTransport)
    public byte UnitId { get; set; }
    public ushort TransactionId { get; set; } // For TCP only, optional for RTU
}
```

**Note:** Core DTOs do NOT reference App layer's `ModbusTransportMode`. RTU/TCP mode is handled by using specific interfaces rather than a Mode property in the context.

## RTU Transport Plan

### Serial Port Service Strategy

> **⚠️ G9A Review Update (2026-05-29): G7 assumption superseded by G9A review**

**Option 1: Reuse Existing SerialPortService (Original G7 assumption - superseded)**

- Terminal uses SerialPortService
- Modbus RTU can also use SerialPortService
- But need ownership coordination
- **G9A finding: SerialPortService is event-based only, lacks SendAndReceiveAsync**
- **Direct reuse NOT recommended for Modbus request-response pattern**

**Option 2: ModbusRtuTransport Owns SerialPort (Original G7 assumption - superseded)**

- Modbus has its own SerialPort access
- Duplicates some Terminal logic
- More isolation
- **G9A finding: Extending SerialPortService risks breaking Terminal behavior**

**Option C: New ModbusRtuTransport + Serial Port Ownership Coordinator (G9A Recommendation)**

- New `ModbusRtuTransport` in Infrastructure layer
- Internal composition of low-level serial capabilities
- New `ISerialPortOwnershipCoordinator` for ownership coordination
- **Does NOT extend existing SerialPortService**
- **Does NOT modify Terminal behavior**
- **Supports fake serial adapter for testing**
- **Clear layer boundaries preserved**

**G9A Review Summary:**
- Existing ISerialPortService is event-based (DataReceived only)
- No SendAndReceiveAsync method
- No per-request timeout control
- No request-response matching
- No port ownership tracking
- Option C avoids breaking Terminal while enabling Modbus RTU

**Recommendation: Option C for G9B-G9D**

### Serial Port Ownership

**Single Ownership Model (Recommended for G9-G10)**

- Only one component can use serial port at a time
- If Terminal is connected, Modbus RTU "Connect" button shows "Terminal in use - please disconnect Terminal first"
- If Modbus RTU is connected, Terminal "Open Port" button shows "Modbus in use - please disconnect Modbus first"
- Simple, clear, avoids conflicts

**Future Shared Model (Post-G12)**

- Design a SerialPortManager that coordinates access
- Possibly share port with mutual exclusion
- Out of scope for G7-G12

### RTU Flow

1. User selects TransportMode = RTU
2. User selects/configures serial port
3. User clicks "Connect"
4. ModbusRtuTransport opens port
5. IsConnected = true
6. User enters parameters, clicks "Build Request"
7. RequestHex shows frame
8. User clicks "Send Request"
9. ModbusRtuTransport writes bytes to port
10. ModbusRtuTransport reads response from port
11. CRC validation happens where?
    - Option A: Core CRC after read
    - Option B: Transport layer validates before returning
    - **Recommendation: Option A** - CRC is protocol, keep in Core
12. Transport returns ModbusTransportResult
13. ViewModel calls Core parser
14. UI displays ResponseHex and ParsedSummary

### RTU Error Handling

| Error | How to handle |
|-------|--------------|
| Port not selected | Validate before connect, show error |
| Port already in use | Detect, show friendly message |
| Open failed | Catch exception, show StatusMessage |
| Send failed | Catch exception, show StatusMessage |
| Timeout (no response) | Return error with timeout info |
| Invalid response format | Return parse error to ViewModel |
| CRC invalid | Return CRC error to ViewModel |
| Unsupported function | Return to UI |

### Manual Testing Strategy

- Use loopback (TX-RX shorted)
- Use virtual serial port
- Use real Modbus device if available
- Document expected behavior

## TCP Transport Plan

### Infrastructure Layer Additions

```
SerialAssistant.Infrastructure/
├── Modbus/
│   ├── ModbusRtuTransport
│   └── ModbusTcpTransport
```

Transport interfaces and DTOs are defined in Core. Infrastructure contains only concrete transport implementations.

### TCP Connection

```csharp
// Concept only - NOT for implementation in G7
public class ModbusTcpTransport : IModbusTcpTransport
{
    private TcpClient? _client;
    private NetworkStream? _stream;
    
    public string IpAddress { get; }
    public int Port { get; }
    
    public async Task<bool> ConnectAsync(CancellationToken ct)
    {
        // TCP connect logic here (in Infrastructure only)
    }
}
```

### MBAP TransactionId Matching

- Every Modbus TCP request has TransactionId
- Response must have same TransactionId
- If mismatch, consider invalid
- TCP Transport should validate this before returning

### Response Length Strategy

1. Read 6 bytes MBAP header first
2. Get Length field from header
3. Read exactly Length bytes remaining
4. Handle partial reads
5. Timeout if incomplete

### TCP Flow

1. User selects TransportMode = TCP
2. User enters IP, Port
3. User clicks "Connect"
4. ModbusTcpTransport connects
5. IsConnected = true
6. User enters parameters, clicks "Build Request"
7. RequestHex shows MBAP frame
8. User clicks "Send Request"
9. ModbusTcpTransport writes to TCP stream
10. ModbusTcpTransport reads response
11. Validate TransactionId
12. Return ModbusTransportResult
13. ViewModel calls Core parser
14. UI displays

### TCP Error Handling

| Error | How to handle |
|-------|--------------|
| Invalid IP/Port | Validate input |
| Connect failed | Show connection error |
| TCP disconnected | Detect and update IsConnected |
| Send failed | Show error |
| Timeout | Show timeout |
| TransactionId mismatch | Show error |
| Invalid MBAP | Show parse error |
| TCP half-open | Handle gracefully |

### Manual Testing Strategy

- Use Modbus TCP simulator (e.g., Modbus Poll, Modbus Slave)
- Use localhost testing
- Use real Modbus TCP device if available

## ModbusViewModel Integration Plan

### Current vs Future

**Current (G5-G6):**
- BuildRequestCommand: only builds hex, no send
- ParseResponseCommand: parses user-provided hex
- No network/serial IO

**Future (G11+):**
- Keep BuildRequestCommand for offline use
- **NEW: SendRequestCommand** - sends to transport
- Keep ParseResponseCommand for debugging
- No IO directly in ViewModel

### ViewModel-Transport Contract

```
ModbusViewModel receives:
- RequestContext (UnitId, TransactionId, etc.)
- RequestBytes (from Core builder)

ModbusViewModel calls:
- IModbusTransport.SendRequestAsync(context, bytes, ct)

ModbusViewModel receives:
- ModbusTransportResult (Success, ResponseBytes, Error, etc.)

ModbusViewModel then:
- If success: calls Core parser
- If error: shows in StatusMessage
- Updates UI properties
```

### ViewModel Changes (Conceptual)

```csharp
// Concept only - NOT for G7 implementation
public class ModbusViewModel : BaseViewModel
{
    // Existing properties remain
    
    // NEW properties
    private IModbusTransport? _transport;
    public bool IsConnected { get => _transport?.IsConnected ?? false; }
    
    // NEW commands
    public ICommand ConnectCommand { get; }
    public ICommand DisconnectCommand { get; }
    public ICommand SendRequestCommand { get; }
    
    private async Task SendRequestAsync()
    {
        // Build request first
        BuildRequest();
        
        if (_transport == null || !_transport.IsConnected)
        {
            StatusMessage = "Not connected";
            return;
        }
        
        // Use transport, NO direct SerialPort/TcpClient!
        var context = new ModbusRequestContext
        {
            UnitId = UnitId,
            TransactionId = TransactionId
        };
        
        var result = await _transport.SendRequestAsync(
            context, 
            RequestBytes!, 
            CancellationToken.None);
        
        if (result.Success && result.ResponseBytes != null)
        {
            ResponseHex = Convert.ToHexString(result.ResponseBytes);
            ParseResponse();
            StatusMessage = $"Success in {result.Duration.TotalMilliseconds:F0}ms";
        }
        else
        {
            StatusMessage = result.ErrorMessage ?? "Unknown error";
        }
    }
}
```

## UI Change Plan

### Minimum UI Additions

**Connection Area (new):**
- Connect/Disconnect button
- IsConnected indicator
- Timeout input (optional)

**RTU Parameters (conditional):**
- Port selector (reuse Terminal UI or similar)
- Baud rate selector
- Data bits, parity, stop bits (if different from Terminal)

**TCP Parameters (conditional):**
- IP Address input
- Port input

**Send Button (new):**
- "Send Request" button (separate from Build Request)
- Disabled when not connected
- Shows busy/loading state

### Style Plan

- **NO final MQTTX style in G8-G11**
- Keep UI minimal and functional
- UI polish saved for potential H phase
- Focus on correctness first

## Concurrency and Ownership

### Single Ownership (Phase 1 Recommendation)

**The Simplest Thing That Could Possibly Work:**

- Terminal and Modbus RTU cannot share port at same time
- UI state machine:

| Current State | Terminal Action | Modbus RTU Action |
|--------------|----------------|-------------------|
| Terminal Connected | Can use Terminal | Modbus Connect disabled (shows message) |
| Modbus Connected | Terminal Open disabled (shows message) | Can use Modbus |
| Both Disconnected | Can open Terminal | Can connect Modbus |

**Message Examples:**
- Modbus: "Terminal is using the serial port. Please disconnect Terminal first."
- Terminal: "Modbus is using the serial port. Please disconnect Modbus first."

**How to Track Ownership:**

> **⚠️ G9A Update (2026-05-29): Earlier App-layer ownership idea superseded**

- **Old Option 1 (superseded):** MainWindowViewModel tracks active port user
- **Old Option 2 (superseded):** Shared service in Infrastructure tracks ownership
- **Current Recommendation:** Core ownership coordinator contract first, then Infrastructure implementation

**G9A Ownership Plan:**
- **G9B:** Define `ISerialPortOwnershipCoordinator` and `SerialPortOwner` enum in Core
- **Core layer** owns the ownership coordination contract (no Infrastructure reference)
- **Infrastructure layer** will later provide concrete ownership implementation
- **App layer** may display ownership state and bind UI enablement, but **must not become the ownership authority**
- MainWindowViewModel must NOT be the ownership authority

### Future Shared Model (Optional, Post-G12)

If we really want to share:
- Design SerialPortManager
- Queue requests
- But probably not needed for initial version

### No TCP Ownership Conflict

TCP connects to IP:Port, not physical port
- Multiple TCP connections possible (to different targets)
- Terminal and Modbus TCP can coexist
- No conflict to handle

## Error Strategy

### Error Classification

| Category | Examples |
|----------|---------|
| Input validation | No port selected, invalid IP, timeout <= 0 |
| Connection | Open failed, connect failed, disconnected |
| Communication | Send failed, receive failed, timeout |
| Protocol | CRC invalid, TransactionId mismatch, invalid response |
| Business | Unsupported function, permission denied |

### Error Display

- All errors shown in ModbusPage.StatusMessage
- Distinguish transient vs permanent errors
- Keep messages user-friendly
- Don't show raw exceptions to user

### Error Recovery

- Timeout: User can retry
- Disconnect: User can reconnect
- Protocol error: User can adjust parameters
- App should NOT crash

## Test Strategy

### Test Layers

| Test Type | What it does | Real Hardware? |
|----------|-------------|---------------|
| Unit Tests | ViewModels, Core protocol | No |
| Fake Transport Tests | ViewModel with FakeTransport | No |
| Integration-like | Fake Serial/TCP simulators | No |
| Manual Tests | Real device/real port | Yes |

### Fake Transport Pattern

```csharp
// Concept for G8
public class FakeModbusTransport : IModbusTransport
{
    public List<byte[]> SentRequests { get; } = new();
    public Queue<byte[]> ResponseQueue { get; } = new();
    public bool IsConnected { get; set; }
    
    public Task<ModbusTransportResult> SendRequestAsync(...)
    {
        SentRequests.Add(requestBytes);
        if (ResponseQueue.TryDequeue(out var resp))
        {
            return Task.FromResult(ModbusTransportResult.Success(resp));
        }
        return Task.FromResult(ModbusTransportResult.Fail("No queued response"));
    }
}
```

**Key Benefit:** All automated tests can run without hardware.

### Test Count Expectation

- G8: +20-30 tests (interface contracts, fake tests)
- G9: +15-25 tests (RTU transport)
- G10: +15-25 tests (TCP transport)
- G11: +10-20 tests (ViewModel integration)

## Phase Breakdown

### G8: Modbus Transport Interfaces and Fake Tests

**Scope:**
- Define interfaces (IModbusTransport, etc.)
- Create Fake implementations
- Write tests with FakeTransport
- Ensure ViewModel can call transport without real IO
- NO real serial/TCP code yet

**Allowed:**
- src/SerialAssistant.Core/Services (interfaces)
- src/SerialAssistant.App/Services (if needed)
- src/SerialAssistant.Tests (fake transport tests)
- NO real IO

**Forbidden:**
- SerialPort
- TcpClient
- Socket

**Validation:**
- Tests pass with fake transport
- ViewModel doesn't see real IO
- No App layer IO references

### G9: Modbus RTU Transport Integration

**Scope:**
- Implement ModbusRtuTransport
- Use existing SerialPortService
- Handle port ownership
- Implement Connect/Disconnect/SendRequestAsync
- RTU error handling

**Allowed:**
- Infrastructure layer only for real serial
- App layer uses IModbusRtuTransport

**Forbidden:**
- App layer must NOT see SerialPort
- UI logic in Infrastructure

**Validation:**
- Manual test with loopback
- Manual test with real device (optional)

### G10: Modbus TCP Transport Integration

**Scope:**
- Implement ModbusTcpTransport
- Handle TCP connect/disconnect
- MBAP TransactionId matching
- TCP error handling

**Allowed:**
- Infrastructure layer only for TCP
- App layer uses IModbusTcpTransport

**Forbidden:**
- App layer must NOT see TcpClient
- App layer must NOT see Socket

**Validation:**
- Manual test with Modbus simulator
- Manual test with real device (optional)

### G11: Modbus Send Workflow UI Integration

**Scope:**
- Update ModbusViewModel for SendRequestCommand
- Update ModbusPage with Connect/Disconnect/Send
- RTU/TCP parameter UI
- Connection state binding
- Error display binding

**Allowed:**
- Minimal UI additions only
- No complex styling

**Forbidden:**
- Final UI polish (save for H phase)

**Validation:**
- End-to-end manual test
- UI flow works
- Errors display correctly

### G12: Modbus Communication Manual Verification

**Scope:**
- Full manual testing
- Edge case testing
- Error condition testing
- Create final manual test checklist

**Allowed:**
- Documentation only

**Forbidden:**
- New features

**Validation:**
- Manual checklist all passed
- Ready for potential H phase

## Risks and Decisions

### Known Risks

| Risk | Severity | Mitigation |
|------|----------|-----------|
| Serial port ownership conflict | High | Single-ownership model first |
| RTU response length detection | Medium | Test with multiple devices, document behavior |
| TCP half-open state | Medium | Add heartbeat or timeout |
| UI blocking during send | Medium | Use async/await properly |
| Timeout configuration too short/long | Medium | Make configurable, sensible default |
| Tests require real hardware | Low | FakeTransport pattern |
| App layer accidentally gets IO references | High | Code reviews, validation gates |
| Premature UI polish | Medium | Explicitly defer to H phase |

### Key Decisions

| Decision | Rationale |
|----------|-----------|
| **G8 first: Interfaces + FakeTests** | Lock down architecture before real IO |
| **Single port ownership** | Simple, predictable, low risk |
| **Core-only CRC/validation** | Keep protocol in Core layer |
| **No App IO references** | Strict layer boundary |
| **Defer UI polish** | Function first, aesthetics later |

## Final Recommendation

**Historical Recommendation from G7 (superseded):**
> NEXT PHASE: G8 - Modbus Transport Interfaces and Fake Tests
>
> G8 was previously recommended and has now been completed through G8A/G8B.

**Current Recommendation after G9A:**
> NEXT PHASE: G9B - Serial Port Ownership Coordinator Contracts

**G9B Scope:**
- Define SerialPortOwner enum in Core
- Define ISerialPortOwnershipCoordinator in Core
- Add fake-based tests for coordinator
- **No Infrastructure changes in G9B**
- **No ModbusRtuTransport in G9B**

**DO NOT SKIP G9B:**
- ❌ Do NOT implement Infrastructure ownership coordination in G9B
- ❌ Do NOT implement real ModbusRtuTransport in G9B
- ❌ Do NOT modify Terminal serial behavior

**WHY G9B NEXT:**
- Ownership is critical for preventing Terminal/Modbus conflicts
- Core contracts should be defined before Infrastructure implementation
- Clean architecture requires Core-first approach

**WHAT G9B DELIVERS:**
- SerialPortOwner enum (None, Terminal, ModbusRtu)
- ISerialPortOwnershipCoordinator interface
- FakeSerialPortOwnershipCoordinator for testing
- Tests proving coordinator works

**DO NOT SKIP G9C:**
- ❌ Do NOT implement real ModbusRtuTransport without ownership coordinator
- ❌ Do NOT skip G9B and go directly to real hardware

---

## G9A Review Notes

### G9A Status: ✅ Completed

**Review Date**: 2026-05-29

### Key Findings

1. **ISerialPortService Limitations**:
   - Fully event-based receive only
   - No SendAndReceiveAsync
   - No CancellationToken support
   - No per-request timeout control
   - No port ownership tracking

2. **SerialPortService Implementation**:
   - No internal buffer
   - No frame boundary detection
   - No request-response matching
   - No concurrency control

3. **Terminal Usage**:
   - Open/Close/Send synchronous
   - DataReceived event for receive
   - No ownership control

### Recommended Strategy: Option C

> **C. 新增 ModbusRtuTransport，内部组合现有低层串口能力 + 新增串口所有权协调服务**

### Why Option C?

| Reason | Explanation |
|--------|-------------|
| No Terminal Breakage | Existing Terminal continues unchanged |
| Clean Architecture | Modbus-specific logic isolated |
| Testability | Fake serial adapter possible |
| Layer Boundaries | App only references Core interfaces |

### Subsequent Phases Plan

- G9B: Serial Port Ownership Coordinator Contracts
- G9C: Modbus RTU Transport Implementation with Fake Serial Adapter
- G9D: Modbus RTU Transport Manual Verification

### Critical Decisions

1. **Ownership First**: Must implement port ownership before real RTU
2. **Core Contracts First**: Define ISerialPortOwnershipCoordinator in Core
3. **No Terminal Changes**: Keep ISerialPortService as-is for Terminal
4. **New ModbusRtuTransport**: Implement IModbusRtuTransport, own serial handling
5. **Fake Adapter First**: Test with fake serial before real hardware
- Do NOT skip fake tests

---

## G9B Implementation Notes

### G9B Status: ✅ Completed

**Implementation Date**: 2026-05-29

### What G9B Delivered

1. **Core Contracts**:
   - `SerialPortOwner` enum in Core (None, Terminal, ModbusRtu)
   - `ISerialPortOwnershipCoordinator` interface in Core
   - `SerialPortOwnershipChangedEventArgs` in Core

2. **Testing Infrastructure**:
   - `FakeSerialPortOwnershipCoordinator` in Tests
   - Comprehensive test coverage for coordinator

3. **Version Update**:
   - Updated MainWindow.xaml version from v0.4.6 to v0.4.7

### What G9B Did NOT Deliver

1. **No Infrastructure Changes**:
   - No real SerialPortOwnershipCoordinator implementation
   - No ModbusRtuTransport implementation
   - No changes to SerialPortService

2. **No App Logic Changes**:
   - No changes to MainWindowViewModel
   - No changes to TerminalViewModel
   - No changes to ModbusViewModel

### G9B Test Coverage

- 2 tests for SerialPortOwner
- 8 tests for SerialPortOwnershipChangedEventArgs
- 22 tests for FakeSerialPortOwnershipCoordinator
- Total: 32 new tests added
- Total tests: 618 (586 + 32)

### Key G9B Decisions

1. **Core Only**: All ownership contracts in Core, no Infrastructure changes
2. **No App Logic**: App doesn't have ownership authority, just displays state
3. **Fake First**: Test with FakeSerialPortOwnershipCoordinator before real implementation
4. **No RTU Yet**: ModbusRtuTransport deferred to G9C

### Next Phase: G9C

**G9C Scope**:
- Implement Infrastructure ownership coordinator
- Implement ModbusRtuTransport with fake serial adapter
- Keep Core contracts unchanged
- Still no real hardware communication

---

## G9C Implementation Notes

### G9C Status: ✅ Completed

**Implementation Date**: 2026-05-29

### What G9C Delivered

1. **Infrastructure Interfaces**:
   - `IModbusRtuSerialAdapter` in Infrastructure layer
   - Abstract serial adapter for RTU transport

2. **Infrastructure Implementation**:
   - `ModbusRtuTransport` implementing `IModbusRtuTransport`
   - Uses `IModbusRtuSerialAdapter` for serial abstraction
   - Integrates with `ISerialPortOwnershipCoordinator`
   - Implements ConnectAsync, DisconnectAsync, SendRequestAsync
   - Supports CRC validation when enabled

3. **Testing Infrastructure**:
   - `FakeModbusRtuSerialAdapter` in Tests
   - `ModbusRtuTransportTests` with comprehensive test coverage
   - Uses `FakeSerialPortOwnershipCoordinator` from G9B

4. **Version Update**:
   - Updated MainWindow.xaml version from v0.4.7 to v0.4.8

### What G9C Did NOT Deliver

1. **No Real Hardware**:
   - No System.IO.Ports usage
   - No real serial port implementation
   - No TcpClient/Socket usage

2. **No App Logic Changes**:
   - No changes to ModbusViewModel
   - No changes to TerminalViewModel
   - No changes to MainWindowViewModel

3. **No UI Changes**:
   - No changes to ModbusPage
   - No changes to TerminalPage
   - Only version display updated

### G9C Test Coverage

- 29 new tests added to `ModbusRtuTransportTests`
- Total tests: 647 (618 + 29)

### Key G9C Decisions

1. **Adapter Pattern**: Use `IModbusRtuSerialAdapter` to abstract serial access
2. **Fake First**: Test with fake adapter before real hardware
3. **Ownership Integration**: ModbusRtuTransport uses ISerialPortOwnershipCoordinator
4. **CRC Validation**: CRC validation in transport layer when enabled
5. **No Real IO**: Defer real serial to G9D

## G9D Implementation Notes

### G9D Status: ✅ Completed

### What G9D Delivered

1. **Infrastructure Layer Additions**:
   - `SystemIoPortsModbusRtuSerialAdapter` - real serial port adapter
   - Implements `IModbusRtuSerialAdapter` interface
   - Location: `src/SerialAssistant.Infrastructure/Modbus/Transport/`

2. **Test Layer Additions**:
   - `SystemIoPortsModbusRtuSerialAdapterTests` - 39 unit tests
   - No hardware required for tests
   - Location: `src/SerialAssistant.Tests/Infrastructure/Modbus/`

3. **Version Update**:
   - Updated from v0.4.8 to v0.4.9

### G9D Key Principles

1. **System.IO.Ports Only in Infrastructure Adapter**: The adapter is the only G9D code allowed to reference System.IO.Ports
2. **No App/ViewModels Integration**: G9D did not modify App layer or inject adapter into ModbusViewModel
3. **No UI Changes**: Only version display updated
4. **No Hardware Tests**: All 39 tests run without real serial ports
5. **String-based Parameters**: Parity and StopBits use strings to avoid exposing System.IO.Ports types

### G9D Test Coverage

- 39 new tests added to `SystemIoPortsModbusRtuSerialAdapterTests`
- Total tests: 686 (647 + 39)
- All tests pass without hardware

### Key G9D Decisions

1. **Adapter Pattern**: Real serial access via `SystemIoPortsModbusRtuSerialAdapter`
2. **Parameter Validation**: Adapter validates port name, baud rate, data bits before opening
3. **Defensive Copy**: WriteAsync copies input bytes before writing
4. **Inter-byte Idle**: ReadAsync uses 20ms idle for Modbus RTU frame detection
5. **Exception Isolation**: Exceptions caught and converted to return values

### Next Phase: G9E

**G9E Scope**:
- RTU Transport Composition (combine adapter with ModbusRtuTransport)
- UI Integration Planning
- Manual Hardware Verification Checklist
- Ownership Coordinator Integration with UI

**Do NOT Skip G9E**:
- ❌ Do NOT directly modify UI without planning
- ❌ Do NOT inject adapter into ModbusViewModel without composition
- ❌ Do NOT skip manual verification planning

