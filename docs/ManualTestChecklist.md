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

#### HEX Mode Send
- [ ] **Step 5.6** Select "HEX" send mode
- [ ] **Step 5.7** Enter valid HEX (e.g., "48 65 6C 6C 6F")
- [ ] **Step 5.8** Click "发送" (Send) button
- [ ] **Step 5.9** Verify status shows success message
- [ ] **Step 5.10** Try invalid HEX (e.g., "XX YY")
- [ ] **Step 5.11** Verify validation error shows

### 6. Serial Port Receive (Hardware Required)

#### Text Mode Receive
- [ ] **Step 6.1** Open serial port
- [ ] **Step 6.2** Set receive mode to "文本"
- [ ] **Step 6.3** Have remote end send text
- [ ] **Step 6.4** Verify text appears in receive area
- [ ] **Step 6.5** Verify receive count increases

#### HEX Mode Receive
- [ ] **Step 6.6** Set receive mode to "HEX"
- [ ] **Step 6.7** Have remote end send data
- [ ] **Step 6.8** Verify HEX appears in receive area
- [ ] **Step 6.9** Verify receive count increases

### 7. Clear Receive Buffer

- [ ] **Step 7.1** Receive some data first
- [ ] **Step 7.2** Click "清空接收区" button
- [ ] **Step 7.3** Verify receive text area is cleared
- [ ] **Step 7.4** Verify receive count is reset to 0

### 8. Configuration Persistence

#### Save Configuration
- [ ] **Step 8.1** Change serial parameters (baud rate, etc.)
- [ ] **Step 8.2** Change send mode
- [ ] **Step 8.3** Change receive mode
- [ ] **Step 8.4** Close application normally
- [ ] **Step 8.5** Verify %AppData%\SerialAssistant.Win\settings.json created
- [ ] **Step 8.6** Verify file contains valid JSON

#### Load Configuration
- [ ] **Step 8.7** Re-open application
- [ ] **Step 8.8** Verify parameters loaded correctly
- [ ] **Step 8.9** Verify send mode restored
- [ ] **Step 8.10** Verify receive mode restored
- [ ] **Step 8.11** If port exists, verify it's selected

#### Corrupted Config Fallback
- [ ] **Step 8.12** Close application
- [ ] **Step 8.13** Manually corrupt settings.json (e.g., just "{")
- [ ] **Step 8.14** Re-open application
- [ ] **Step 8.15** Verify application doesn't crash
- [ ] **Step 8.16** Verify default settings loaded

### 9. Edge Cases

- [ ] **Step 9.1** Open non-existent port (should show error)
- [ ] **Step 9.2** Open already-open port (should show error)
- [ ] **Step 9.3** Close already-closed port (should show error)
- [ ] **Step 9.4** Send without opening port (should show error)
- [ ] **Step 9.5** Empty text send (should show error)
- [ ] **Step 9.6** Empty HEX send (should show error)

### 10. Cleanup

- [ ] **Step 10.1** Close any open serial ports
- [ ] **Step 10.2** Exit application
- [ ] **Step 10.3** (Optional) Delete test config file at %AppData%\SerialAssistant.Win\settings.json

## Test Results Summary

| Category | Result | Notes |
|----------|--------|-------|
| Application Startup | ☐ Pass / ☐ Fail | |
| Serial Port Scanning | ☐ Pass / ☐ Fail | |
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
