# SerialAssistant.Win Manual Test Checklist

This document provides a step-by-step manual testing guide for SerialAssistant.Win.

## Prerequisites

- .NET 8 SDK
- Windows 10 or later
- (Optional) Serial port hardware or null modem emulator for real send/receive tests

## Test Steps

### 1. Application Startup

- [ ] **Step 1.1** Run the application:
  ```powershell
  dotnet run --project .\src\SerialAssistant.App\SerialAssistant.App.csproj
  ```
- [ ] **Step 1.2** Verify main window displays correctly
- [ ] **Step 1.3** Verify initial state (Disconnected, parameters at defaults)

### 2. Serial Port Scanning

- [ ] **Step 2.1** Click "刷新" (Refresh) button
- [ ] **Step 2.2** Verify status message shows available ports
- [ ] **Step 2.3** If ports available, verify they appear in dropdown
- [ ] **Step 2.4** If no ports, verify application doesn't crash
- [ ] **Step 2.5** Verify dropdown shows "无可用端口" when no ports

### 3. Serial Port Configuration

- [ ] **Step 3.1** Select a port from dropdown (if available)
- [ ] **Step 3.2** Change baud rate to different value
- [ ] **Step 3.3** Change data bits to 7 or 8
- [ ] **Step 3.4** Change parity to None/Odd/Even
- [ ] **Step 3.5** Change stop bits to One/Two
- [ ] **Step 3.6** Verify all UI controls respond

### 4. Serial Port Open/Close (Hardware Required)

- [ ] **Step 4.1** Select valid port and parameters
- [ ] **Step 4.2** Click "打开" (Open) button
- [ ] **Step 4.3** Verify connection state changes to "已连接"
- [ ] **Step 4.4** Verify parameter controls are disabled
- [ ] **Step 4.5** Verify send controls are enabled
- [ ] **Step 4.6** Click "关闭" (Close) button
- [ ] **Step 4.7** Verify connection state changes to "未连接"
- [ ] **Step 4.8** Verify parameter controls are enabled
- [ ] **Step 4.9** Verify send controls are disabled

### 5. Serial Port Send (Hardware Required)

#### Text Mode Send
- [ ] **Step 5.1** Open serial port
- [ ] **Step 5.2** Select "文本" (Text) send mode
- [ ] **Step 5.3** Enter text (e.g., "Hello World")
- [ ] **Step 5.4** Click "发送" (Send) button
- [ ] **Step 5.5** Verify status shows success message
- [ ] **Step 5.6** Verify TX record appears in receive area

#### Send Line Ending (Feature A)
- [ ] **Step 5.7** Select "None" line ending
- [ ] **Step 5.8** Send "ABC", verify TX record shows exactly "ABC"
- [ ] **Step 5.9** Select "CR" line ending
- [ ] **Step 5.10** Send "ABC", verify TX record shows "ABC" + 0x0D
- [ ] **Step 5.11** Select "LF" line ending
- [ ] **Step 5.12** Send "ABC", verify TX record shows "ABC" + 0x0A
- [ ] **Step 5.13** Select "CRLF" line ending
- [ ] **Step 5.14** Send "ABC", verify TX record shows "ABC" + 0x0D 0x0A

#### HEX Mode Send
- [ ] **Step 5.15** Select "HEX" send mode
- [ ] **Step 5.16** Enter valid HEX (e.g., "41 42 43")
- [ ] **Step 5.17** Click "发送" (Send) button
- [ ] **Step 5.18** Verify status shows success message
- [ ] **Step 5.19** Verify TX record shows HEX bytes (not text)
- [ ] **Step 5.20** Set line ending to "CRLF" while in HEX mode
- [ ] **Step 5.21** Send "41 42 43"
- [ ] **Step 5.22** Verify TX record does NOT include 0x0D 0x0A (HEX mode ignores line ending)
- [ ] **Step 5.23** Try invalid HEX (e.g., "XX YY")
- [ ] **Step 5.24** Verify validation error shows

### 6. Serial Port Receive (Hardware Required)

#### Text Mode Receive
- [ ] **Step 6.1** Open serial port
- [ ] **Step 6.2** Set receive mode to "文本"
- [ ] **Step 6.3** Have remote end send text
- [ ] **Step 6.4** Verify RX record appears in receive area with "RX" prefix
- [ ] **Step 6.5** Verify receive count increases

#### HEX Mode Receive
- [ ] **Step 6.6** Set receive mode to "HEX"
- [ ] **Step 6.7** Have remote end send data
- [ ] **Step 6.8** Verify HEX appears in receive area with "RX" prefix
- [ ] **Step 6.9** Verify receive count increases

### 7. TX/RX Direction and Timestamp Display (Feature B)

#### Verify UI Controls Exist
- [ ] **Step 7.1** Verify "显示时间戳" (Show Timestamp) checkbox exists
- [ ] **Step 7.2** Verify "显示方向" (Show Direction) checkbox exists
- [ ] **Step 7.3** Verify both checkboxes are checked by default

#### Direction Marking
- [ ] **Step 7.4** Send some data
- [ ] **Step 7.5** Verify TX record shows "TX" prefix
- [ ] **Step 7.6** Receive some data
- [ ] **Step 7.7** Verify RX record shows "RX" prefix

#### Timestamp Display
- [ ] **Step 7.8** Verify timestamp format is [HH:mm:ss.fff]
- [ ] **Step 7.9** Uncheck "显示时间戳"
- [ ] **Step 7.10** Verify TX/RX records no longer show timestamps
- [ ] **Step 7.11** Check "显示时间戳" again
- [ ] **Step 7.12** Verify timestamps reappear

#### Hide Direction Marking
- [ ] **Step 7.13** Uncheck "显示方向"
- [ ] **Step 7.14** Verify TX/RX records show only data, no "TX" or "RX" prefix
- [ ] **Step 7.15** Send data, verify no "TX" prefix
- [ ] **Step 7.16** Receive data, verify no "RX" prefix

#### Display Format Examples
- [ ] **Step 7.17** Enable both "显示时间戳" and "显示方向"
- [ ] **Step 7.18** Send "ABC", verify display shows: `[HH:mm:ss.fff] TX ABC`
- [ ] **Step 7.19** Receive "OK", verify display shows: `[HH:mm:ss.fff] RX OK`

#### HEX Mode Display
- [ ] **Step 7.20** Switch to HEX display mode
- [ ] **Step 7.21** Verify TX/RX records reformatted to show HEX bytes
- [ ] **Step 7.22** Verify TX "ABC" shows as "TX 41 42 43"
- [ ] **Step 7.23** Verify RX "OK" shows as "RX 4F 4B"

### 8. Clear Receive Buffer

- [ ] **Step 8.1** Send and receive some data first
- [ ] **Step 8.2** Verify TX and RX records exist in receive area
- [ ] **Step 8.3** Click "清空接收区" button
- [ ] **Step 8.4** Verify TX/RX records are cleared
- [ ] **Step 8.5** Verify receive count is reset to 0

### 9. Configuration Persistence (Feature A & B)

#### Save Configuration
- [ ] **Step 9.1** Change serial parameters (baud rate, etc.)
- [ ] **Step 9.2** Change send mode
- [ ] **Step 9.3** Change receive mode
- [ ] **Step 9.4** Change send line ending (None/CR/LF/CRLF)
- [ ] **Step 9.5** Modify ShowTimestamp setting
- [ ] **Step 9.6** Modify ShowDirection setting
- [ ] **Step 9.7** Close application normally
- [ ] **Step 9.8** Verify %AppData%\SerialAssistant.Win\settings.json created
- [ ] **Step 9.9** Verify file contains valid JSON
- [ ] **Step 9.10** Verify JSON includes SendLineEnding, ShowTimestamp, ShowDirection

#### Load Configuration
- [ ] **Step 9.11** Re-open application
- [ ] **Step 9.12** Verify serial parameters loaded correctly
- [ ] **Step 9.13** Verify send mode restored
- [ ] **Step 9.14** Verify receive mode restored
- [ ] **Step 9.15** Verify SendLineEnding restored
- [ ] **Step 9.16** Verify ShowTimestamp setting restored
- [ ] **Step 9.17** Verify ShowDirection setting restored
- [ ] **Step 9.18** If port exists, verify it's selected

#### Old Config Missing Fields
- [ ] **Step 9.19** Close application
- [ ] **Step 9.20** Edit settings.json to remove SendLineEnding, ShowTimestamp, ShowDirection
- [ ] **Step 9.21** Re-open application
- [ ] **Step 9.22** Verify defaults used: SendLineEnding=None, ShowTimestamp=true, ShowDirection=true

#### Corrupted Config Fallback
- [ ] **Step 9.23** Close application
- [ ] **Step 9.24** Manually corrupt settings.json (e.g., just "{")
- [ ] **Step 9.25** Re-open application
- [ ] **Step 9.26** Verify application doesn't crash
- [ ] **Step 9.27** Verify default settings loaded

### 10. Receive Buffer Limit (Feature C)

#### Verify UI Control Exists
- [ ] **Step 10.1** Verify "接收缓存" (Receive Buffer) dropdown exists
- [ ] **Step 10.2** Verify options include 64 KiB, 256 KiB, 1 MiB, 4 MiB
- [ ] **Step 10.3** Verify default is 256 KiB

#### Buffer Trimming on Data Add
- [ ] **Step 10.4** Set buffer to 64 KiB
- [ ] **Step 10.5** Send/receive data until buffer exceeds 64 KiB
- [ ] **Step 10.6** Verify oldest records are trimmed
- [ ] **Step 10.7** Verify TX/RX direction and timestamps still work on remaining records
- [ ] **Step 10.8** Switch between text/HEX modes, verify only remaining records are redrawn

#### Single Large Record Preservation
- [ ] **Step 10.9** Clear receive buffer
- [ ] **Step 10.10** Set buffer to 64 KiB
- [ ] **Step 10.11** Send/receive single record larger than 64 KiB
- [ ] **Step 10.12** Verify record is preserved (not trimmed)

#### MaxDisplayBytes Change Behavior
- [ ] **Step 10.13** Fill buffer with multiple records at 256 KiB setting
- [ ] **Step 10.14** Change buffer to 64 KiB
- [ ] **Step 10.15** Verify old records are trimmed immediately
- [ ] **Step 10.16** Change buffer back to 256 KiB
- [ ] **Step 10.17** Verify trimmed records are NOT restored

#### Configuration Persistence
- [ ] **Step 10.18** Set buffer to 64 KiB
- [ ] **Step 10.19** Close application normally
- [ ] **Step 10.20** Re-open application
- [ ] **Step 10.21** Verify buffer size is restored to 64 KiB
- [ ] **Step 10.22** Set buffer to 1 MiB
- [ ] **Step 10.23** Close and re-open, verify restored to 1 MiB

#### Clear Behavior
- [ ] **Step 10.24** Send/receive data to fill buffer with some trimmed records
- [ ] **Step 10.25** Click "清空接收区" button
- [ ] **Step 10.26** Verify display is cleared
- [ ] **Step 10.27** Verify receive count is reset to 0

#### Old Config Missing MaxDisplayBytes
- [ ] **Step 10.28** Close application
- [ ] **Step 10.29** Edit settings.json to remove MaxDisplayBytes
- [ ] **Step 10.30** Re-open application
- [ ] **Step 10.31** Verify default 256 KiB is used

### 11. Edge Cases

- [ ] **Step 11.1** Open non-existent port (should show error)
- [ ] **Step 11.2** Open already-open port (should show error)
- [ ] **Step 11.3** Close already-closed port (should show error)
- [ ] **Step 11.4** Send without opening port (should show error)
- [ ] **Step 11.5** Empty text send (should show error)
- [ ] **Step 11.6** Empty HEX send (should show error)

### 12. Cleanup

- [ ] **Step 11.1** Close any open serial ports
- [ ] **Step 11.2** Exit application
- [ ] **Step 11.3** (Optional) Delete test config file at %AppData%\SerialAssistant.Win\settings.json

## Test Results Summary

| Category | Result | Notes |
|----------|--------|-------|
| Application Startup | ☐ Pass / ☐ Fail | |
| Serial Port Scanning | ☐ Pass / ☐ Fail | |
| TX/RX Direction Display | ☐ Pass / ☐ Fail | |
| Timestamp Display | ☐ Pass / ☐ Fail | |
| Configuration | ☐ Pass / ☐ Fail | |
| Open/Close | ☐ Pass / ☐ N/A (No HW) | |
| Text Send | ☐ Pass / ☐ N/A (No HW) | |
| HEX Send | ☐ Pass / ☐ N/A (No HW) | |
| Text Receive | ☐ Pass / ☐ N/A (No HW) | |
| HEX Receive | ☐ Pass / ☐ N/A (No HW) | |
| Clear Buffer | ☐ Pass / ☐ Fail | |
| Config Persistence | ☐ Pass / ☐ Fail | |
| Config Corruption | ☐ Pass / ☐ Fail | |
| Edge Cases | ☐ Pass / ☐ Fail | |

## Tester Information

- **Tester Name**: _________________________
- **Test Date**: _________________________
- **Additional Notes**:
