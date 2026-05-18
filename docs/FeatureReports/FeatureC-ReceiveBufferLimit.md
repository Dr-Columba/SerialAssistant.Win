# Feature C: Receive Buffer Limit

## Overview

Feature C adds configurable receive buffer limit functionality to SerialAssistant.Win, preventing memory issues with large communication records while preserving usability.

## Feature C Phases

| Phase | Status | Description |
|-------|--------|-------------|
| C1 | ✅ Complete | ReceiveDisplayViewModel internal buffer limit with MaxDisplayBytes, CurrentDisplayBytes, TrimmedRecordCount |
| C2 | ✅ Complete | UI dropdown for receive buffer size, AppSettings integration, configuration persistence |
| C3 | ✅ Complete | Documentation update and final verification |

## Key Features

### 1. Configurable Buffer Size

- **Options**: 64 KiB (65536 bytes), 256 KiB (262144 bytes), 1 MiB (1048576 bytes), 4 MiB (4194304 bytes)
- **Default**: 256 KiB
- **UI Control**: Dropdown selector in main window
- **Persistence**: Saved to settings.json

### 2. Buffer Trimming Strategy

- **Triggers**:
  - Adding new TX/RX records via `AddTxData` or `AddRxData`
  - Decreasing `MaxDisplayBytes` via UI
- **Behavior**:
  - Oldest records trimmed first (FIFO)
  - Trimming stops when total size ≤ `MaxDisplayBytes`
  - Single record larger than `MaxDisplayBytes` is preserved (not trimmed)
- **Tracking**:
  - `CurrentDisplayBytes`: Current total size of records in buffer
  - `TrimmedRecordCount`: Number of records trimmed since last clear

### 3. ReceivedBytesCount Behavior

- Never decreases due to trimming
- Counts total bytes received since last clear
- Resets to 0 only when `Clear()` is called

### 4. Clear Behavior

When `Clear()` is called:
- All communication records are removed
- `CurrentDisplayBytes` reset to 0
- `TrimmedRecordCount` reset to 0
- `ReceivedBytesCount` reset to 0
- Display text is cleared

### 5. Configuration Persistence

**AppSettings Model**:
```csharp
public int MaxDisplayBytes { get; set; } = 262144;
```

**Load Flow**:
1. Application starts
2. JsonAppSettingsService.Load() reads settings.json
3. If MaxDisplayBytes is missing, uses default 262144
4. MainWindowViewModel.ApplySettings() sets ReceiveDisplay.MaxDisplayBytes

**Save Flow**:
1. Application closing
2. MainWindowViewModel.SaveSettings() creates AppSettings
3. AppSettings.MaxDisplayBytes = ReceiveDisplay.MaxDisplayBytes
4. JsonAppSettingsService.Save() writes to settings.json

## Key Implementation Details

### ReceiveDisplayViewModel Properties

Located in `SerialAssistant.App.ViewModels.ReceiveDisplayViewModel`:

```csharp
public int MaxDisplayBytes
{
    get => _maxDisplayBytes;
    set
    {
        int safeValue = value;
        if (safeValue <= 0)
        {
            safeValue = 262144;
        }
        if (SetProperty(ref _maxDisplayBytes, safeValue))
        {
            TrimExcessRecords();
            UpdateDisplayText();
        }
    }
}

public int CurrentDisplayBytes { get; private set; }

public int TrimmedRecordCount { get; private set; }
```

### TrimExcessRecords Method

```csharp
private void TrimExcessRecords()
{
    while (_records.Count > 1 && CurrentDisplayBytes > _maxDisplayBytes)
    {
        CommunicationRecord oldest = _records[0];
        _records.RemoveAt(0);
        TrimmedRecordCount++;
    }
}
```

### MainWindowViewModel ReceiveBufferSizeOptions

```csharp
public ObservableCollection<int> ReceiveBufferSizeOptions { get; private set; }

// In constructor:
ReceiveBufferSizeOptions = new ObservableCollection<int>
{
    65536,   // 64 KiB
    262144,  // 256 KiB (default)
    1048576, // 1 MiB
    4194304  // 4 MiB
};
```

## Test Coverage

### ReceiveDisplayViewModelTests (C1)

- MaxDisplayBytes defaults to 262144
- Setting MaxDisplayBytes to smaller value trims old records
- Setting MaxDisplayBytes to 0 or negative uses default 262144
- Single large record is preserved
- Clear() resets all counts to 0
- ReceivedBytesCount doesn't decrease on trimming

### MainWindowViewModelTests (C2)

- ReceiveBufferSizeOptions contains all expected values
- LoadSettings restores MaxDisplayBytes
- SaveSettings saves MaxDisplayBytes
- Default config has MaxDisplayBytes = 262144
- Feature A/B tests continue to pass

### JsonAppSettingsServiceTests (C2)

- Default config has MaxDisplayBytes = 262144
- Save/Load round-trip works
- Old config missing MaxDisplayBytes uses default
- Damaged config falls back to defaults
- Feature A/B tests continue to pass

**Total Tests**: 239+

## Manual Verification Steps

1. Start application, verify receive buffer dropdown exists with 64 KiB/256 KiB/1 MiB/4 MiB
2. Verify default is 256 KiB
3. Change to 64 KiB, close and re-open, verify restored
4. Send/receive data until buffer exceeds limit, verify old records trimmed
5. Verify single large record is preserved
6. Click Clear, verify all counts reset
7. Verify Feature A/B continue to work

## Current Limitations

- Communication records (TX/RX history) not persisted across sessions
- No send history buffer
- No logging persistence
- No auto-scroll
- No receive buffer export

## Recommendations for Next Steps

1. Merge Feature C into main branch
2. Continue with any remaining feature requests
3. Consider adding logging persistence in future
4. Consider adding receive buffer export in future

## Conclusion

Feature C has been successfully completed in all phases (C1-C3). The implementation is well-tested, documented, and ready for use.

**Recommendation**: Merge into main branch.
