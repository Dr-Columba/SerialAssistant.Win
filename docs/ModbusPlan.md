# SerialAssistant.Win Modbus Planning

## 1. Purpose

This document defines the planning for Modbus functionality in SerialAssistant.Win. This is a **planning document, not an implementation document**.

**Goals:**
- Define supported Modbus features
- Establish layer boundaries
- Plan test strategy
- Break down implementation into manageable phases

**Non-Goals:**
- No code implementation
- No test implementation
- No UI implementation

---

## 2. Modbus Scope

### Supported Features

| Feature | Support | Notes |
|---------|---------|-------|
| Modbus RTU | Yes | Serial communication |
| Modbus TCP | Yes | Ethernet communication |
| Client/Master Mode | Yes | Debug tool capability |
| Common Function Codes | Yes | See Section 3 |
| Request Frame Building | Yes | Core layer |
| Response Frame Parsing | Yes | Core layer |
| Exception Response Parsing | Yes | Core layer |
| CRC16 | Yes | Core layer |
| MBAP Header | Yes | TCP only |
| Register Read/Write | Yes | UI layer |
| Minimal UI Loop | Yes | App layer |
| Testable Core Protocol | Yes | Core layer |

### Out of Scope (Current Phase)

| Feature | Status | Notes |
|---------|--------|-------|
| Server/Slave Simulation | Not now | May add in future |
| Gateway | Not now | Not required |
| Script System | Not now | Over-complicated |
| Bulk Engineering Files | Not now | Not required |
| Advanced Waveform Charts | Not now | Not required |
| Third-party Library Integration | Not now | Custom implementation |
| Plugin System | Not now | Over-complicated |
| Database | Not now | Not required |
| Cloud Sync | Not now | Not required |

---

## 3. Supported Function Codes Roadmap

### Priority 1 (G1/G2 - Core Foundation)

| Code | Name | Notes |
|------|------|-------|
| 03 | Read Holding Registers | Most common read |
| 04 | Read Input Registers | Most common read |
| 06 | Write Single Register | Most common write |
| 10 | Write Multiple Registers | Bulk write |

### Priority 2 (G3/G4 - Extended)

| Code | Name | Notes |
|------|------|-------|
| 01 | Read Coils | Bit access |
| 02 | Read Discrete Inputs | Bit access |
| 05 | Write Single Coil | Bit write |
| 0F | Write Multiple Coils | Bulk bit write |

### Implementation Order

```
Phase G1: CRC16, base models, function code enums
Phase G2: RTU frame builder/parser for 03/04/06/10
Phase G3: TCP/MBAP frame builder/parser for 03/04/06/10
Phase G4: ModbusViewModel (no UI)
Phase G5: ModbusPage minimal UI
Phase G6: Documentation and manual testing
```

---

## 4. Core Layer Design

### Core Layer Responsibilities

The Core layer is the **pure protocol implementation**. It contains no platform-specific code.

**DO:**
- Protocol models (frames, requests, responses)
- Pure algorithms (CRC16, byte packing)
- Frame building logic
- Frame parsing logic
- Function code enums
- Data type definitions

**DO NOT:**
- Reference WPF
- Reference System.IO.Ports
- Access file system
- Contain UI state
- Contain serial port open/close logic
- Contain TCP socket logic
- Reference Infrastructure layer

### Proposed Namespace Structure (For Reference Only)

```
src/SerialAssistant.Core/
├── Modbus/
│   ├── Enums/
│   │   ├── ModbusFunctionCode.cs
│   │   └── ModbusDataType.cs
│   ├── Models/
│   │   ├── ModbusRequest.cs
│   │   ├── ModbusResponse.cs
│   │   ├── ModbusExceptionResponse.cs
│   │   └── ModbusRegisterValue.cs
│   ├── Rtu/
│   │   ├── ModbusRtuFrame.cs
│   │   ├── ModbusRtuFrameBuilder.cs
│   │   └── ModbusRtuFrameParser.cs
│   ├── Tcp/
│   │   ├── ModbusTcpFrame.cs
│   │   ├── MbapHeader.cs
│   │   ├── ModbusTcpFrameBuilder.cs
│   │   └── ModbusTcpFrameParser.cs
│   └── Utils/
│       ├── ModbusCrc16.cs
│       └── ModbusByteUtils.cs
```

**Note:** This structure is for planning reference. G0 does not create these directories.

---

## 5. App Layer Design

### App Layer Responsibilities

The App layer contains UI pages and ViewModels for Modbus functionality.

**DO:**
- ModbusPage (UI)
- ModbusViewModel (state management)
- Form input handling
- Command triggering
- Result display
- Error messages

**DO NOT:**
- Reference System.IO.Ports directly
- Implement CRC calculations
- Directly concatenate low-level protocol bytes
- Bypass Core protocol layer

### ModbusViewModel Responsibilities

```
ModbusViewModel
├── Connection settings (RTU: port, TCP: IP:port)
├── Current register values
├── Function code selection
├── Address/quantity inputs
├── Response display
├── Error handling
└── Delegates protocol work to Core layer
```

### Dependency Direction

```
UI (ModbusPage)
    ↓
ViewModel (ModbusViewModel)
    ↓
Core Protocol (Modbus.*)
    ↓
Infrastructure (ISerialPortService, ITcpService) [Future]
```

---

## 6. Infrastructure Layer Design

### Infrastructure Layer Responsibilities

The Infrastructure layer provides communication transport services.

**DO (Future):**
- Serial port transport adapter
- TCP socket transport adapter
- Connection management

**DO NOT:**
- Reference WPF
- Contain UI ViewModel
- Implement Modbus protocol rules

### Transport Interface (Future Reference)

```csharp
public interface IModbusTransport
{
    Task<OperationResult<byte[]>> SendAndReceiveAsync(byte[] request);
}
```

---

## 7. Data Models

### ModbusFunctionCode

**Purpose:** Enum for Modbus function codes

**Values:**
```csharp
public enum ModbusFunctionCode : byte
{
    ReadCoils = 0x01,
    ReadDiscreteInputs = 0x02,
    ReadHoldingRegisters = 0x03,
    ReadInputRegisters = 0x04,
    WriteSingleCoil = 0x05,
    WriteSingleRegister = 0x06,
    WriteMultipleCoils = 0x0F,
    WriteMultipleRegisters = 0x10,
}
```

### ModbusDataType

**Purpose:** Enum for register data types

**Values:**
```csharp
public enum ModbusDataType
{
    Unsigned16,
    Signed16,
    Unsigned32,
    Signed32,
    Float32,
    Unsigned64,
    Signed64,
    Float64,
    String,
}
```

### ModbusRtuFrame

**Purpose:** Represents a complete RTU frame

**Structure:**
```
[Slave Address (1)] [Function Code (1)] [Data (N)] [CRC Low (1)] [CRC High (1)]
```

### ModbusTcpFrame

**Purpose:** Represents a complete Modbus TCP frame

**Structure:**
```
[MBAP Header (7)] [Function Code (1)] [Data (N)]
```

### ModbusRequest

**Purpose:** Abstract base for all Modbus requests

**Common Properties:**
- SlaveAddress (byte)
- FunctionCode (ModbusFunctionCode)
- StartingAddress (ushort)
- Quantity (ushort) - for read/write multiple

### ModbusResponse

**Purpose:** Abstract base for all Modbus responses

**Common Properties:**
- FunctionCode (byte)
- IsException (bool)
- ExceptionCode (byte?) - when IsException is true

### ModbusExceptionResponse

**Purpose:** Represents an exception response

**Properties:**
- FunctionCode (byte) - original function code + 0x80
- ExceptionCode (byte) - 01-04 or more

**Exception Codes:**
- 01: Illegal Function
- 02: Illegal Data Address
- 03: Illegal Data Value
- 04: Server Failure

### ModbusRegisterValue

**Purpose:** Represents a register value with metadata

**Properties:**
- Address (ushort)
- Value (uint/ulong/float/etc.)
- DataType (ModbusDataType)
- RawHigh (ushort)
- RawLow (ushort)

### ModbusCrc16

**Purpose:** CRC16 calculation utility

**Algorithm:** Modbus CRC16
- Polynomial: 0x8005
- Initial: 0xFFFF
- Reflected input/output

---

## 8. RTU Frame Rules

### Frame Structure

```
┌─────────────┬─────────────┬───────────┬─────────┬─────────┐
│ Slave       │ Function    │ Data      │ CRC     │ CRC     │
│ Address     │ Code        │           │ Low     │ High    │
│ (1 byte)   │ (1 byte)    │ (N bytes) │ (1 byte)│ (1 byte)│
└─────────────┴─────────────┴───────────┴─────────┴─────────┘
```

### Example: Read Holding Registers (03)

**Request:**
```
[Slave: 0x01] [Function: 0x03] [Start Hi: 0x00] [Start Lo: 0x00]
[Qty Hi: 0x00] [Qty Lo: 0x0A] [CRC Lo: 0xC5] [CRC Hi: 0xCD]
```

**Response:**
```
[Slave: 0x01] [Function: 0x03] [Byte Count: 0x14] [Register 0 Hi] [Register 0 Lo]
... [Register 9 Hi] [Register 9 Lo] [CRC Lo] [CRC Hi]
```

### CRC Calculation

- Modbus CRC16 uses low byte first, then high byte
- Polynomial: 0x8005
- Initial value: 0xFFFF
- Final XOR: 0x0000
- All bits reflected

### Inter-Frame Delay

- RTU requires minimum 3.5 character times between frames
- Implemented in Infrastructure layer, not Core

---

## 9. TCP Frame Rules

### MBAP Header Structure

```
┌─────────────┬─────────────┬───────────┬──────────────┐
│ Transaction │ Protocol    │ Length    │ Unit         │
│ Identifier │ Identifier  │ (2 bytes)│ Identifier   │
│ (2 bytes)  │ (2 bytes)  │           │ (1 byte)    │
└─────────────┴─────────────┴───────────┴──────────────┘
```

| Field | Size | Description |
|-------|------|-------------|
| Transaction Identifier | 2 bytes | Sequence number (echoed in response) |
| Protocol Identifier | 2 bytes | Always 0x0000 for Modbus |
| Length | 2 bytes | Byte count of remaining bytes |
| Unit Identifier | 1 byte | Slave address (for Modbus RTU bridge) |

### TCP Frame Structure

```
[MBAP Header (7 bytes)] [Function Code (1)] [Data (N bytes)]
```

### Example: Read Holding Registers (03)

**Request:**
```
[Transaction ID: 0x0001] [Protocol: 0x0000] [Length: 0x0006]
[Unit: 0x01] [Function: 0x03] [Start Hi: 0x00] [Start Lo: 0x00]
[Qty Hi: 0x00] [Qty Lo: 0x0A]
```

**Response:**
```
[Transaction ID: 0x0001] [Protocol: 0x0000] [Length: 0x0015]
[Unit: 0x01] [Function: 0x03] [Byte Count: 0x14]
[Register 0 Hi] [Register 0 Lo] ... [Register 9 Hi] [Register 9 Lo]
```

### Key Differences from RTU

| Aspect | RTU | TCP |
|--------|-----|-----|
| Address in frame | Yes (1 byte) | No (in MBAP Unit ID) |
| CRC | Yes (2 bytes) | No |
| MBAP Header | No | Yes (7 bytes) |
| Connection oriented | No | Yes |

---

## 10. Parsing Strategy

### Parsing Pipeline

```
1. Length Check
   ├── Minimum length validation
   ├── Maximum length validation
   └── Structure validation (header present)
       │
       ▼
2. Function Code Validation
   ├── Supported function codes check
   └── Exception flag detection (code + 0x80)
       │
       ▼
3. Protocol-Specific Validation
   ├── RTU: CRC validation
   └── TCP: MBAP validation
       │
       ▼
4. Data Validation
   ├── Address range
   ├── Quantity range
   └── Data content
       │
       ▼
5. Exception Response Detection
   ├── Exception code extraction
   └── Exception meaning
       │
       ▼
6. Structured Result Output
   ├── Success: Parsed response object
   └── Failure: Error with reason
```

### Parsing Rules

1. **Never concatenate UI text in parsing layer**
2. **Return structured objects, not strings**
3. **Include raw bytes for debugging**
4. **Provide clear error codes**

---

## 11. Error Strategy

### Error Types

| Error Code | Description | Detection Point |
|------------|-------------|-----------------|
| InvalidLength | Frame too short or too long | Length check |
| InvalidCrc | CRC mismatch | RTU validation |
| InvalidMbapHeader | Malformed MBAP | TCP validation |
| UnsupportedFunctionCode | Unknown function code | Function code check |
| ExceptionResponse | Server returned error | Response parsing |
| AddressOutOfRange | Starting address invalid | Data validation |
| QuantityOutOfRange | Quantity invalid | Data validation |
| PayloadMismatch | Data length mismatch | Data validation |

### Error Response Structure

```csharp
public class ModbusParseError
{
    public ModbusErrorCode ErrorCode { get; }
    public string Description { get; }
    public byte[] RawBytes { get; }
    public int? ErrorPosition { get; }
}
```

### Exception Response Codes

| Code | Name | Meaning |
|------|------|---------|
| 01 | ILLEGAL FUNCTION | Function code not supported |
| 02 | ILLEGAL DATA ADDRESS | Address out of range |
| 03 | ILLEGAL DATA VALUE | Invalid data value |
| 04 | SERVER FAILURE | Server error |

---

## 12. Test Strategy

### CRC16 Tests

**Standard Test Vectors:**
```csharp
// Modbus CRC16 test vectors
// Input: { 0x01, 0x03, 0x00, 0x00, 0x00, 0x0A }
// Expected: 0xC5CD

// Additional vectors needed
- Empty input
- Single byte
- Long payload
```

### RTU Frame Tests

**Request Building:**
- Valid 03 request
- Valid 04 request
- Valid 06 request
- Valid 10 request
- Boundary address (0x0000, 0xFFFF)
- Boundary quantity (1, 125 for registers)

**Response Parsing:**
- Valid 03 response
- Valid 04 response
- Valid 06 response
- Valid 10 response
- Exception response (01, 02, 03, 04)

**CRC Tests:**
- Valid CRC
- Invalid CRC
- Corrupted data

### TCP Frame Tests

**MBAP Header:**
- Valid header
- Invalid protocol identifier
- Invalid length
- Transaction ID echo

**TCP Frame Building:**
- Valid 03 request
- Valid response parsing
- Exception response

### Boundary Tests

**Address:**
- Minimum: 0x0000
- Maximum: 0xFFFF
- Address + Quantity overflow

**Quantity:**
- Minimum: 1
- Maximum: 125 (registers) / 2000 (coils)
- Quantity = 0 (invalid)

### Error Cases

- Empty input
- Single byte
- Truncated frame
- Oversized frame
- Garbage data
- Wrong slave address
- Wrong function code

### Byte Order Tests

**Big Endian vs Little Endian:**
- 16-bit register: high byte first, low byte second
- 32-bit value: high register first, low register second
- Float32: IEEE 754 big endian

---

## 13. Phase Breakdown

### Feature G1: Modbus Core Foundation

**Goal:** Implement Core layer base models, enums, and CRC16

**Scope:**
- ModbusFunctionCode enum
- ModbusDataType enum
- ModbusCrc16 utility
- ModbusRegisterValue model
- Base request/response models
- Unit tests for CRC16

**Forbidden:**
- No UI implementation
- No Infrastructure implementation
- No frame builder/parser

### Feature G2: Modbus RTU Frame Builder and Parser

**Goal:** Implement RTU frame building and parsing for core function codes

**Scope:**
- ModbusRtuFrame model
- ModbusRtuFrameBuilder
- ModbusRtuFrameParser
- Support function codes: 03, 04, 06, 10
- Unit tests for RTU frames

**Forbidden:**
- No UI implementation
- No TCP implementation

### Feature G3: Modbus TCP Frame Builder and Parser

**Goal:** Implement TCP/MBAP frame building and parsing

**Scope:**
- MbapHeader model
- ModbusTcpFrame model
- ModbusTcpFrameBuilder
- ModbusTcpFrameParser
- Support function codes: 03, 04, 06, 10
- Unit tests for TCP frames

**Forbidden:**
- No UI implementation

### Feature G4: ModbusViewModel Minimal Workflow

**Goal:** Establish ModbusViewModel without complex UI

**Scope:**
- ModbusViewModel
- Connection state management
- Register value management
- Error handling
- Unit tests

**Forbidden:**
- No XAML implementation
- No frame building/parsing (delegates to Core)

### Feature G5: ModbusPage Minimal UI

**Goal:** Implement minimal UI for register read/write

**Scope:**
- ModbusPage.xaml
- Address input
- Quantity input
- Function code selection
- Read/Write buttons
- Response display

**Forbidden:**
- Complex charting
- Multiple simultaneous operations

### Feature G6: Modbus Manual Test and Documentation Closure

**Goal:** Complete manual testing and documentation

**Scope:**
- Manual test checklist for Modbus
- Documentation updates
- Final verification

**Forbidden:**
- No new features

---

## 14. Non-Goals

### Not Implemented (Current Scope)

| Feature | Reason |
|---------|--------|
| Third-party Modbus Libraries | Custom implementation for learning |
| Plugin System | Over-complicated |
| Script System | Over-complicated |
| Server/Slave Simulation | Client/master tool only |
| Gateway | Not required |
| Database | Not required |
| Cloud Sync | Not required |
| Advanced Theme UI | Focus on functionality |
| Advanced Charts | Not required |

---

## 15. References

### Modbus Specifications

- Modbus Application Protocol Specification V1.1b3
- Modbus over Serial Line Specification V1.02
- Modbus TCP/IP Specification

### External Resources

- modbus.org
- Various open-source Modbus implementations for reference

---

## 16. G1 Implementation Notes

### G1 Completed (2026-05-26)

**Status:** Implemented

**Files Created:**
- `src/SerialAssistant.Core/Modbus/Common/ModbusFunctionCode.cs`
- `src/SerialAssistant.Core/Modbus/Common/ModbusDataType.cs`
- `src/SerialAssistant.Core/Modbus/Models/ModbusRegisterValue.cs`
- `src/SerialAssistant.Core/Modbus/Utilities/ModbusCrc16.cs`
- `src/SerialAssistant.Tests/Modbus/Common/ModbusFunctionCodeTests.cs`
- `src/SerialAssistant.Tests/Modbus/Common/ModbusDataTypeTests.cs`
- `src/SerialAssistant.Tests/Modbus/Models/ModbusRegisterValueTests.cs`
- `src/SerialAssistant.Tests/Modbus/Utilities/ModbusCrc16Tests.cs`

**What Was Implemented:**
1. ModbusFunctionCode enum with all 8 function codes (0x01-0x10)
2. ModbusDataType enum with 7 data types
3. ModbusRegisterValue model with Address, RawValue, DataType, HighByte, LowByte, SignedValue
4. ModbusCrc16 utility with Compute, LowByte, HighByte, AppendCrc, Validate methods

**What Was NOT Implemented:**
- RTU frame builder (G2)
- RTU frame parser (G2)
- TCP frame builder (G3)
- TCP frame parser (G3)
- Request/Response models (future phases)
- UI (G5)

**Test Coverage:**
- 8 tests for ModbusFunctionCode
- 7 tests for ModbusDataType
- 8 tests for ModbusRegisterValue
- 11 tests for ModbusCrc16
- Total: 34 new tests added

**Version Update:**
- UI display updated from v0.3.3 to v0.4.0

**Next Phase (G2):**
- Implement ModbusRtuFrame model
- Implement ModbusRtuFrameBuilder
- Implement ModbusRtuFrameParser
- Support function codes 03, 04, 06, 10

---

## 17. G2 Implementation Notes

### G2 Completed (2026-05-26)

**Status:** Implemented

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

**What Was Implemented:**
1. ModbusRtuErrorCode enum with error types
2. ModbusRtuFrame class with SlaveAddress, FunctionCode, Data, Crc
3. ModbusRtuParseResult class for parsing results
4. ModbusRtuRequestBuilder with methods for 03, 04, 06, 10 function codes
5. ModbusRtuResponseParser with support for 03, 04, 06, 10 and exception responses

**What Was NOT Implemented:**
- TCP frame builder (G3)
- TCP frame parser (G3)
- MBAP header (G3)
- UI (G5)
- ModbusViewModel (G4)

**Test Coverage:**
- 8 tests for ModbusRtuFrame
- 14 tests for ModbusRtuRequestBuilder
- 16 tests for ModbusRtuResponseParser
- Total: 38 new tests added

**Version Update:**
- UI display updated from v0.4.0 to v0.4.1

**Next Phase (G3):**
- Implement ModbusTcpFrame model
- Implement ModbusTcpFrameBuilder
- Implement ModbusTcpFrameParser
- Support function codes 03, 04, 06, 10
- Implement MBAP header

---

---

## 18. G3 Implementation Notes

### G3 Completed (2026-05-26)

**Status:** Implemented

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

**What Was Implemented:**
1. ModbusTcpErrorCode enum with error types
2. MbapHeader class with TransactionId, ProtocolId, Length, UnitId
3. ModbusTcpParseResult class for parsing results
4. ModbusTcpFrame class with Header, FunctionCode, Data
5. ModbusTcpRequestBuilder with methods for 03, 04, 06, 10 function codes
6. ModbusTcpResponseParser with support for 03, 04, 06, 10 and exception responses

**What Was NOT Implemented:**
- TCP Socket communication (G3 - Infrastructure)
- UI (G5)
- ModbusViewModel (G4)

**Test Coverage:**
- 8 tests for MbapHeader
- 8 tests for ModbusTcpFrame
- 15 tests for ModbusTcpRequestBuilder
- 18 tests for ModbusTcpResponseParser
- Total: 49 new tests added

**Version Update:**
- UI display updated from v0.4.1 to v0.4.2

**Next Phase (G4):**
- Implement ModbusViewModel
- Connection state management
- Register value management
- Error handling

---

## 19. G4 Implementation Notes

### G4 Completed (2026-05-26)

**Status:** Implemented

**Files Created:**
- `src/SerialAssistant.App/ViewModels/ModbusTransportMode.cs`
- `src/SerialAssistant.App/ViewModels/ModbusRequestKind.cs`
- `src/SerialAssistant.App/ViewModels/ModbusViewModel.cs`
- `src/SerialAssistant.Tests/ViewModels/ModbusViewModelTests.cs`
- `src/SerialAssistant.App/MainWindow.xaml` (version update to v0.4.3)

**What Was Implemented:**
1. ModbusTransportMode enum (Rtu, Tcp)
2. ModbusRequestKind enum (ReadHoldingRegisters, ReadInputRegisters, WriteSingleRegister, WriteMultipleRegisters)
3. ModbusViewModel with:
   - Transport mode selection (RTU/TCP)
   - Request kind selection
   - Request building via Core RTU/TCP builders
   - Response parsing via Core RTU/TCP parsers
   - HEX conversion using existing HexConverter
   - Status management
4. Comprehensive unit tests (54 tests)

**What Was NOT Implemented:**
- ModbusPage.xaml (G5)
- Real serial port sending
- TCP Socket communication
- Infrastructure modifications

**Test Coverage:**
- 54 new tests added to ModbusViewModelTests
- Total tests: 494 (was 440)

**Version Update:**
- UI display updated from v0.4.2 to v0.4.3

**Next Phase (G5):**
- Implement ModbusPage.xaml
- Bind to ModbusViewModel
- Implement register read/write UI

---

## G5 Implementation Notes

**Completed:**
- ✅ ModbusPage.xaml created with minimal UI
- ✅ ModbusPage.xaml.cs created with InitializeComponent only
- ✅ MainWindowViewModel updated with navigation properties
- ✅ MainWindow.xaml updated with navigation buttons
- ✅ ModbusPage binds directly to ModbusViewModel
- ✅ ModbusViewModel updated with TransportModes and RequestKinds collections
- ✅ Version updated from v0.4.3 to v0.4.4
- ✅ Shell navigation between Terminal and Modbus pages works

**Not Implemented (Future Phase):**
- ❌ Real serial port sending
- ❌ TCP socket communication
- ❌ Infrastructure layer changes

**Version Update:**
- UI display updated from v0.4.3 to v0.4.4

**Next Phase (G6):**
- Manual testing and documentation

---

## G6 Closure Notes

**Phase**: G6 - Modbus Manual Test and Documentation Closure

**Status**: ✅ Completed

**Completion Date**: 2026-05-27

### Summary

G1-G5 phases have been completed, establishing the Modbus foundation:

- **G1**: Core foundation (CRC16, models, enums)
- **G2**: RTU frame builder and parser
- **G3**: TCP frame builder and parser
- **G4**: ModbusViewModel minimal workflow
- **G5**: ModbusPage minimal UI with shell navigation

### Current State

- **Test Count**: 520 passed (26 new tests added in G5)
- **Version**: v0.4.4
- **Tag**: v0.4.4 (on main branch)
- **UI**: Minimal functional UI, not final visual style

### What's Complete

- ✅ Modbus RTU frame building and parsing
- ✅ Modbus TCP frame building and parsing
- ✅ Request/Response HEX display
- ✅ Shell navigation between Terminal and Modbus pages
- ✅ ModbusPage UI binding to ModbusViewModel

### What's Not Complete

- ❌ Real RTU serial port communication
- ❌ Real TCP socket communication
- ❌ Auto-polling functionality
- ❌ Register table editing
- ❌ Batch templates
- ❌ Final UI styling

### Next Phase Recommendation

**Suggested**: G7 - Modbus Transport Integration Planning

- Plan how to connect Modbus to real communication channels
- Design RTU transport via Infrastructure SerialPortService
- Design TCP transport via new ModbusTcpTransportService
- Do NOT continue UI styling until communication is working

---

## G7 Transport Planning Notes

**Phase**: G7 - Modbus Transport Integration Planning

**Status**: ✅ Completed

**Completion Date**: 2026-05-27

### Summary

G7 is a pure documentation planning phase with NO code changes. This phase defines the architecture for integrating real Modbus RTU/TCP communication:

### What G7 Accomplishes

- ✅ **Architecture Planning**: Defines layer boundaries for transport integration
- ✅ **Interface Design**: Proposes IModbusTransport, IModbusRtuTransport, IModbusTcpTransport interfaces
- ✅ **RTU Strategy**: Plans RTU transport via existing SerialPortService with ownership management
- ✅ **TCP Strategy**: Plans TCP transport via new Infrastructure service with TcpClient
- ✅ **Error Strategy**: Defines comprehensive error handling approach
- ✅ **Test Strategy**: Plans fake transport for automated testing without hardware
- ✅ **Phase Breakdown**: Defines G8-G12 phases for incremental implementation

### Key Decisions from G7

1. **G8 First**: Start with interfaces and fake transport, NOT real hardware
2. **Single Ownership**: Terminal and Modbus RTU cannot share serial port simultaneously
3. **Core Only Protocol**: CRC, frame building/parsing stays in Core layer
4. **Infrastructure Only IO**: App layer never directly references System.IO.Ports or TcpClient
5. **Defer UI Styling**: No final MQTTX-style UI until functional communication is complete

### Current State After G7

- **Test Count**: 520 passed (unchanged from G6)
- **Version**: v0.4.4 (unchanged)
- **No Code Changes**: 100% documentation-only phase
- **No src/ modifications**: All layers preserved as-is
- **No tests/ modifications**: Test suite untouched

### Next Phase Recommendation

**Recommended**: G8 - Modbus Transport Interfaces and Fake Tests

**Why G8 Next**:
- Lock down interface contracts before real implementation
- Prove ViewModel can work with transport via fakes
- Reduce risk by validating architecture first
- Prevent App layer pollution with IO references

---

## G8A Transport Contracts Implementation Notes

**Phase**: G8A - Modbus Transport Contracts and Fake Transport Foundation

**Status**: ✅ Completed

**Completion Date**: 2026-05-29

### Summary

G8A is a code implementation phase that creates the Core transport contracts and FakeModbusTransport for testing:

### What G8A Accomplishes

- ✅ **Core Transport Namespace**: Created `src/SerialAssistant.Core/Modbus/Transport/`
- ✅ **Interface Definitions**: IModbusTransport, IModbusRtuTransport, IModbusTcpTransport
- ✅ **DTO Models**: ModbusTransportResult, ModbusTransportOptions, ModbusRequestContext
- ✅ **Error Codes**: ModbusTransportErrorCode enum with 16 error types
- ✅ **Fake Implementation**: FakeModbusTransport for test-only usage
- ✅ **Comprehensive Tests**: 40 new tests for all transport types
- ✅ **Version Update**: Updated to v0.4.5

### Key Architecture Decisions from G8A

1. **Interfaces in Core**: All transport interfaces and DTOs placed in Core layer
2. **No IO Dependencies**: Core transport files contain no System.IO.Ports, TcpClient, or Socket
3. **Defensive Copies**: ResponseBytes are always copied to prevent external mutation
4. **Async Pattern**: ConnectAsync, DisconnectAsync, SendRequestAsync all return Task
5. **Fake Only in Tests**: FakeModbusTransport exists only in test project

### Files Created

| File | Purpose |
|------|---------|
| IModbusTransport.cs | Core transport interface |
| IModbusRtuTransport.cs | RTU-specific transport interface |
| IModbusTcpTransport.cs | TCP-specific transport interface |
| ModbusTransportResult.cs | Result DTO with factory methods |
| ModbusTransportOptions.cs | Configuration options |
| ModbusRequestContext.cs | Request context data |
| ModbusTransportErrorCode.cs | Error enumeration |
| FakeModbusTransport.cs | Test fake implementation |

### Test Coverage

| Test Class | Tests | Purpose |
|-----------|-------|---------|
| ModbusTransportOptionsTests | 7 | Options validation |
| ModbusRequestContextTests | 9 | Context validation |
| ModbusTransportResultTests | 10 | Result factory and copy |
| FakeModbusTransportTests | 14 | Fake transport behavior |

### Current State After G8A

- **Test Count**: 560 passed (was 520, +40 new tests)
- **Version**: v0.4.5 (updated from v0.4.4)
- **No Real IO**: Still no actual SerialPort or TcpClient usage
- **Core Transport Complete**: Ready for Infrastructure implementation
- **Fake Available**: Tests can use FakeModbusTransport

### What G8A Does NOT Include

- ❌ **Real SerialPort**: No System.IO.Ports usage
- ❌ **Real TcpClient**: No System.Net.Sockets usage
- ❌ **ModbusViewModel Changes**: Send workflow not yet integrated
- ❌ **ModbusPage UI**: Transport UI not yet added

### Next Phase Recommendation

**Recommended**: G8B - ModbusViewModel Transport Injection with Fake Tests

**Why G8B Next**:
- G8A provides clean interface contracts
- G8B integrates contracts into ViewModel
- Fake transport enables testing without hardware
- ViewModel can be tested in isolation
- Maintains layer separation

---

## G8B ViewModel Transport Injection Implementation Notes

**Phase**: G8B - ModbusViewModel Transport Injection with Fake Tests

**Status**: ✅ Completed

**Completion Date**: 2026-05-29

### Summary

G8B integrates IModbusTransport into ModbusViewModel with async methods and comprehensive tests.

### What G8B Accomplishes

- ✅ **Transport Injection**: ModbusViewModel can now receive IModbusTransport via constructor
- ✅ **Async Methods**: ConnectTransportAsync, DisconnectTransportAsync, SendRequestAsync
- ✅ **State Management**: IsConnected, IsBusy, ConnectionStatus, LastTransportError
- ✅ **Error Handling**: Comprehensive error capture and status reporting
- ✅ **Test Coverage**: 26 new tests for transport integration
- ✅ **Version Update**: Updated to v0.4.6

### Key Changes to ModbusViewModel

```csharp
public ModbusViewModel() { /* No transport */ }
public ModbusViewModel(IModbusTransport transport) { /* With transport */ }

public async Task ConnectTransportAsync() { ... }
public async Task DisconnectTransportAsync() { ... }
public async Task SendRequestAsync() { ... }
```

### New Properties

| Property | Type | Purpose |
|---------|------|---------|
| IsTransportAvailable | bool | Indicates if transport is injected |
| IsConnected | bool | Connection state from transport |
| ConnectionStatus | string | Human-readable status |
| CanSendRequest | bool | Determines if SendRequest can execute |
| IsBusy | bool | Indicates async operation in progress |
| LastTransportError | string | Stores last transport error |

### Test Coverage

| Test Class | Tests | Purpose |
|-----------|-------|---------|
| ModbusViewModelTransportTests | 26 | Transport integration tests |

### Current State After G8B

- **Test Count**: 586 passed (was 560, +26 new tests)
- **Version**: v0.4.6 (updated from v0.4.5)
- **Transport Integration**: ModbusViewModel can use IModbusTransport
- **Fake Available**: All tests use FakeModbusTransport

### What G8B Does NOT Include

- ❌ **Real SerialPort**: No System.IO.Ports usage
- ❌ **Real TcpClient**: No System.Net.Sockets usage
- ❌ **ModbusPage UI**: Transport UI not yet added
- ❌ **Infrastructure**: Still no transport implementation

---

## G9A RTU Transport Capability Review Notes

**Phase**: G9A - Modbus RTU Transport Existing Serial Service Capability Review

**Status**: ✅ Completed - 2026-05-29

G9A is a **documentation-only phase** that reviews existing serial port service capabilities before implementing Modbus RTU transport.

### What G9A Accomplishes

1. **Serial Service Capability Review**
   - Reviewed ISerialPortService interface capabilities
   - Reviewed SerialPortService implementation details
   - Reviewed TerminalViewModel serial usage patterns

2. **Gap Analysis**
   - No SendAndReceiveAsync method
   - No per-request timeout control
   - No CancellationToken support
   - No port ownership tracking

3. **Strategy Resolution**
   - Resolved G7 Option 1 vs G9A Option C conflict
   - Updated ModbusTransportPlan.md with Option C recommendation

### Key Findings from G9A

#### ISerialPortService Capabilities

| Capability | Status | Notes |
|------------|--------|-------|
| Open/Close | ✅ Yes | Synchronous operations |
| Send | ✅ Yes | Synchronous send |
| DataReceived Event | ✅ Yes | Event-based receive |
| SendAndReceiveAsync | ❌ No | Not available |
| CancellationToken | ❌ No | Not supported |
| Port Ownership | ❌ No | Not tracked |

#### SerialPortService Gaps for Modbus RTU

| Gap | Severity | Impact |
|-----|----------|--------|
| No SendAndReceiveAsync | 🔴 High | Cannot implement request-response |
| No Per-Request Timeout | 🔴 High | Cannot wait with timeout |
| No Cancellation | 🔴 High | Cannot cancel ongoing operation |
| No Port Ownership | 🔴 High | Cannot prevent Terminal/Modbus conflict |
| Event-Based Only | 🔴 High | Incompatible with request-response pattern |

### Recommended Strategy: Option C

**C. 新增 ModbusRtuTransport，内部组合现有低层串口能力 + 新增串口所有权协调服务**

| Reason | Explanation |
|--------|-------------|
| No Terminal Breakage | Existing Terminal continues unchanged |
| Clean Architecture | Modbus-specific logic isolated |
| Testability | Fake serial adapter possible |
| Layer Boundaries | App only references Core interfaces |
| Ownership Control | Explicit ownership coordination |

### Why Option C Instead of Option 1?

| Concern | Option 1 | Option C |
|---------|----------|----------|
| Event-based only | ❌ Incompatible | ✅ Full control |
| Terminal risk | ❌ High | ✅ None |
| Timeout per request | ❌ Not supported | ✅ Full support |

### G9B-G9D Phase Plan

| Phase | Focus | Key Deliverables |
|-------|-------|------------------|
| **G9B** | Ownership Coordinator Contracts | ISerialPortOwnershipCoordinator in Core |
| **G9C** | ModbusRtuTransport with Fake | ModbusRtuTransport + FakeSerialAdapter |
| **G9D** | Manual Verification | Real hardware testing |

### Current State After G9A

- ✅ Test count: 586 passed (unchanged)
- ✅ Version: v0.4.6 (unchanged)
- ✅ No code changes
- ✅ Serial service capability review complete
- ✅ Strategy conflict resolved
- ❌ Still no real Modbus RTU communication

### Critical Boundary Rules (G9A Reinforcement)

1. **App NEVER references System.IO.Ports**
2. **Infrastructure CAN reference System.IO.Ports**
3. **Core NEVER references Infrastructure**
4. **Terminal and Modbus RTU port ownership MUST be explicitly managed**

### What G9A Does NOT Include

- ❌ No code implementation (documentation only)
- ❌ No ModbusRtuTransport (deferred to G9C)
- ❌ No Ownership Coordinator (deferred to G9B)

### Next Phase Recommendation

**Recommended**: G9B - Serial Port Ownership Coordinator Contracts

**Why G9B Next**:
- Ownership is critical for preventing Terminal/Modbus conflicts
- Core contracts should be defined before Infrastructure implementation
- Fake coordinator possible for testing

**Do NOT Skip to G9C**:
- ❌ Do NOT implement RTU without ownership coordinator
- ❌ Do NOT modify Terminal serial behavior

---

*Document created: 2026-05-26*
*Last updated: 2026-05-29*
*Phase: G9A - Modbus RTU Transport Capability Review Complete*
