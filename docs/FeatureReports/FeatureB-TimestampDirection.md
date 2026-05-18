# Feature B: TX/RX Direction Marking and Timestamp Display

## Overview

Feature B adds communication direction marking and optional timestamp display to the serial assistant application. This allows users to clearly distinguish between transmitted (TX) and received (RX) data in the display area.

## Feature Phases

| Phase | Description | Status |
|-------|-------------|--------|
| B1 | Core models and ReceiveDisplayViewModel refactoring | Complete |
| B2 | MainWindowViewModel integration | Complete |
| B3 | UI controls and configuration persistence | Complete |
| B4 | Documentation and verification | Complete |

## B1: Communication Record Models

### CommunicationDirection Enum

```csharp
public enum CommunicationDirection
{
    Tx,
    Rx
}
```

### CommunicationRecord Model

```csharp
public class CommunicationRecord
{
    public CommunicationDirection Direction { get; }
    public byte[] Data { get; }
    public DateTime Timestamp { get; }
}
```

### ReceiveDisplayViewModel Changes

- Internal storage changed from `List<byte>` to `List<CommunicationRecord>`
- New methods: `AddTxData(byte[])`, `AddRxData(byte[])`
- Existing `AddReceivedData` delegates to `AddRxData`
- New properties: `ShowTimestamp` (default: true), `ShowDirection` (default: true)
- Display text regenerated when ShowTimestamp/ShowDirection/IsHexDisplay changes

## B2: MainWindowViewModel Integration

### TX Record Flow

```
SendCommand.Execute
    -> Validate input
    -> Convert to bytes (apply SendLineEnding for text mode)
    -> ISerialPortService.Send(data)
    -> If success:
        -> SentBytesCount += data.Length
        -> ReceiveDisplay.AddTxData(data)  // TX record added
        -> StatusMessage updated
```

### RX Record Flow

```
SerialPort.DataReceived event
    -> SerialPortService reads bytes
    -> OnDataReceived in ViewModel
    -> IUiThreadInvoker.Invoke() to UI thread
    -> ReceiveDisplay.AddRxData(data)  // RX record added
    -> ReceivedBytesCount increased
    -> Display text regenerated
```

### Key Behaviors

- TX records only added on successful send
- TX records include line ending bytes (for text mode)
- RX records added via IUiThreadInvoker for thread safety
- ReceivedBytesCount only counts RX bytes (not TX)

## B3: UI and Configuration

### UI Controls

Two CheckBox controls added to receive display area:

- **显示时间戳** (Show Timestamp): Controls timestamp display
- **显示方向** (Show Direction): Controls TX/RX direction markers

### Configuration Fields

```csharp
public bool ShowTimestamp { get; set; } = true;
public bool ShowDirection { get; set; } = true;
```

### Display Format Examples

| ShowTimestamp | ShowDirection | Example Output |
|---------------|---------------|----------------|
| true | true | `[12:34:56.123] TX ABC` |
| false | true | `TX ABC` |
| true | false | `[12:34:56.123] ABC` |
| false | false | `ABC` |

### Configuration Persistence

- ShowTimestamp and ShowDirection saved to settings.json
- Settings restored on application startup
- Missing fields in old config use defaults (true)
- Corrupted config falls back to defaults

## B4: Documentation

This phase includes:

- README.md: Feature list updated
- Architecture.md: Communication record flow documented
- ManualTestChecklist.md: TX/RX and timestamp test steps added
- FinalReview.md: Feature B status added
- FeatureReports: This document

## Automatic Tests

### Test Coverage

| Component | Tests |
|-----------|-------|
| ReceiveDisplayViewModel | ~35 tests |
| MainWindowViewModel | ~50 tests |
| JsonAppSettingsService | ~15 tests |
| Total | 214+ tests |

### Key Test Scenarios

1. Default ShowTimestamp/ShowDirection values
2. TX record appending on successful send
3. TX record with CRLF includes 0x0D 0x0A
4. HEX send does not append line ending
5. TX does not increase ReceivedBytesCount
6. RX increases ReceivedBytesCount
7. ShowTimestamp/ShowDirection changes trigger redraw
8. Text/HEX mode switch redraws all records
9. Configuration save/load of ShowTimestamp/ShowDirection
10. Old config missing fields use defaults

## Manual Testing Recommendations

### Without Serial Hardware

1. Launch application
2. Verify UI shows "显示时间戳" and "显示方向" checkboxes
3. Verify both checkboxes are checked by default
4. Check/uncheck checkboxes and observe display changes
5. Test configuration persistence by closing and reopening
6. Test old config fallback by manually editing settings.json

### With Serial Hardware

1. Send data and verify TX records appear
2. Receive data and verify RX records appear
3. Enable/disable timestamps and verify format [HH:mm:ss.fff]
4. Enable/disable direction markers and verify TX/RX shown/hidden
5. Switch between text/HEX display and verify historical redraw
6. Test CRLF/CR/LF line endings in text mode
7. Verify HEX mode ignores line ending setting

## Known Limitations

1. Communication records (TX/RX history) are not persisted across sessions
2. No send history buffer
3. No logging persistence

## Next Steps

### Suggested Future Improvements

1. Add send history buffer
2. Add logging persistence for debugging
3. Add export of receive buffer to file
4. Consider auto-reconnection option

### Before Merging

1. Ensure all tests pass (214+ tests)
2. Run manual test checklist
3. Verify no code style violations
4. Check for any regression in existing features
5. Review documentation accuracy

## Branch Information

- **Main branch**: main (v0.1.0-mvp baseline)
- **Feature A**: feature/send-line-ending (completed)
- **Feature B**: feature/timestamp-direction-b4 (current, ready for PR)

## Conclusion

Feature B is complete and ready for integration. The implementation follows all architectural constraints, includes comprehensive test coverage, and provides clear documentation for future maintenance.
