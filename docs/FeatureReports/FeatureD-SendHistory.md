# Feature D: Send History

## Overview

Feature D adds send history functionality to SerialAssistant.Win, allowing users to quickly resend previously sent messages through a dropdown selection.

## Feature D Phases

| Phase | Status | Description |
|-------|--------|-------------|
| D1 | ✅ Complete | SendHistoryItem model, SendHistory ObservableCollection, MaxSendHistoryCount, AddToSendHistory, ClearSendHistoryCommand |
| D2 | ✅ Complete | UI dropdown for send history selection, SelectedSendHistoryItem binding, backfill SendText/SendMode |
| D3 | ✅ Complete | AppSettings integration for SendHistory and MaxSendHistoryCount, configuration persistence |
| D4 | ✅ Complete | Documentation update and final verification |

## Key Features

### 1. Send History Data Structure

**SendHistoryItem Model** (`SerialAssistant.Core.Models.SendHistoryItem`):

```csharp
public class SendHistoryItem
{
    public string Content { get; set; }
    public SendMode SendMode { get; set; }

    public SendHistoryItem() { }
    public SendHistoryItem(string content, SendMode sendMode) { }
}
```

- **Content**: User input text before sending (not including line ending)
- **SendMode**: Text or Hex mode used for this history entry

### 2. Recording Strategy

After successful send:

1. Get original SendText input (not including line ending)
2. Get current SendMode (Text or Hex)
3. Check for duplicate: same Content AND same SendMode
4. If duplicate found, remove old entry
5. Insert new entry at index 0 (most recent)
6. Trim to MaxSendHistoryCount if exceeded

**Conditions for NOT recording:**
- Send failure
- Not connected
- Invalid HEX input
- Empty content

### 3. Duplicate Removal Rules

- **Duplicate condition**: Content same AND SendMode same
- **Not duplicate**: Same Content but different SendMode (kept separately)
- **On duplicate send**: Old entry removed, new entry inserted at index 0

### 4. Sort Order

- **Index 0**: Most recent item
- **Index N**: N-th most recent item
- **Overflow**: When MaxSendHistoryCount exceeded, oldest item (last index) is removed

### 5. History Selection Backfill

When user selects a history item from dropdown:

1. SendText = SelectedSendHistoryItem.Content
2. SelectedSendMode = SelectedSendHistoryItem.SendMode
3. NO send triggered
4. NO new history entry added
5. NO ReceiveDisplay modification

### 6. Clear History Behavior

When ClearSendHistoryCommand is executed:

1. SendHistory.Clear()
2. SelectedSendHistoryItem = null
3. SendText remains unchanged
4. ReceiveDisplay remains unchanged
5. Connection state unchanged

### 7. Configuration Persistence

**AppSettings fields:**

```csharp
public int MaxSendHistoryCount { get; set; } = 20;
public List<SendHistoryItem> SendHistory { get; set; }
```

**NOT saved to configuration:**
- SelectedSendHistoryItem (always null after restore)
- SendText (current input)
- TX/RX communication records

**RestoreSendHistory safety rules:**
1. If settings.SendHistory is null, use empty list
2. Skip null history items
3. Skip empty/whitespace Content
4. Skip invalid SendMode values
5. Remove duplicates (same Content + SendMode)
6. Trim to MaxSendHistoryCount
7. SelectedSendHistoryItem set to null

## Key Implementation Details

### MainWindowViewModel Properties

```csharp
public ObservableCollection<SendHistoryItem> SendHistory { get; }
public SendHistoryItem? SelectedSendHistoryItem { get; set; }
public int MaxSendHistoryCount { get; set; } = 20;
public ICommand ClearSendHistoryCommand { get; }
```

### AddToSendHistory Method

```csharp
private void AddToSendHistory(string content, SendMode sendMode)
{
    if (string.IsNullOrEmpty(content))
    {
        return;
    }

    var existingIndex = -1;
    for (int i = 0; i < _sendHistory.Count; i++)
    {
        if (_sendHistory[i].Content == content && _sendHistory[i].SendMode == sendMode)
        {
            existingIndex = i;
            break;
        }
    }

    if (existingIndex >= 0)
    {
        _sendHistory.RemoveAt(existingIndex);
    }

    _sendHistory.Insert(0, new SendHistoryItem(content, sendMode));
    TrimSendHistory();
}
```

### SelectedSendHistoryItem Setter

```csharp
public SendHistoryItem? SelectedSendHistoryItem
{
    get => _selectedSendHistoryItem;
    set
    {
        if (SetProperty(ref _selectedSendHistoryItem, value))
        {
            if (value != null)
            {
                SendText = value.Content;
                SelectedSendMode = value.SendMode;
            }
        }
    }
}
```

## Test Coverage

### D1 Tests (Send History Core)
- Default SendHistory is empty
- Default MaxSendHistoryCount is 20
- Send success adds history
- Send failure does not add history
- Duplicate removal
- Overflow trimming
- ClearSendHistoryCommand behavior

### D2 Tests (Send History UI)
- SelectedSendHistoryItem default is null
- Selection backfills SendText and SendMode
- Selection does not trigger send
- Selection does not add history
- ClearSendHistoryCommand clears selection

### D3 Tests (Send History Persistence)
- Load/Save SendHistory
- Load/Save MaxSendHistoryCount
- Duplicate removal on restore
- Overflow trimming on restore
- Empty/whitespace content skipped
- SelectedSendHistoryItem not saved
- Old config fallback

**Total Tests**: 291+

## Manual Verification Steps

1. Start the application
2. Open a serial port connection
3. Send text "Hello" in Text mode
4. Send "41 42 43" in HEX mode
5. Verify history dropdown shows both records
6. Verify latest record is at top (index 0)
7. Select "Hello" from history
8. Verify SendText is filled with "Hello"
9. Verify send mode is Text
10. Click "清空历史"
11. Verify history is empty
12. Verify SendText is NOT cleared
13. Close and reopen application
14. Verify history is restored
15. Verify Features A/B/C still work

## Current Limitations

- No send history search/filter
- No history item editing
- No send history export
- No logging persistence
- No cycle/scheduled transmission

## Recommendations for Next Steps

1. Consider adding logging persistence
2. Consider adding receive buffer export
3. Consider adding auto-reconnection
4. Consider adding theme support (dark/light mode)

## Conclusion

Feature D has been successfully completed in all phases (D1-D4). The implementation provides a complete send history system with recording, UI selection, and configuration persistence.

**Recommendation**: Merge into main branch.
