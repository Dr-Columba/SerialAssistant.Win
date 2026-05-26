using Xunit;
using SerialAssistant.App.ViewModels;
using SerialAssistant.Core.Enums;
using SerialAssistant.Core.Models;
using SerialAssistant.Tests.Infrastructure;
using SerialAssistant.Tests.UI;
using System.Text;

namespace SerialAssistant.Tests.ViewModels
{
    public class TerminalViewModelTests
    {
        [Fact]
        public void Constructor_Initializes_NotNull()
        {
            var viewModel = new TerminalViewModel();
            Assert.NotNull(viewModel);
        }

        [Fact]
        public void SerialSettings_NotNull()
        {
            var viewModel = new TerminalViewModel();
            Assert.NotNull(viewModel.SerialSettings);
        }

        [Fact]
        public void ReceiveDisplay_NotNull()
        {
            var viewModel = new TerminalViewModel();
            Assert.NotNull(viewModel.ReceiveDisplay);
        }

        [Fact]
        public void SendLineEndings_ContainsAllOptions()
        {
            var viewModel = new TerminalViewModel();
            Assert.Contains(SendLineEnding.None, viewModel.SendLineEndings);
            Assert.Contains(SendLineEnding.CR, viewModel.SendLineEndings);
            Assert.Contains(SendLineEnding.LF, viewModel.SendLineEndings);
            Assert.Contains(SendLineEnding.CRLF, viewModel.SendLineEndings);
        }

        [Fact]
        public void SendModes_ContainsAllOptions()
        {
            var viewModel = new TerminalViewModel();
            Assert.Contains(SendMode.Text, viewModel.SendModes);
            Assert.Contains(SendMode.Hex, viewModel.SendModes);
        }

        [Fact]
        public void SendHistory_InitializedAsEmptyCollection()
        {
            var viewModel = new TerminalViewModel();
            Assert.NotNull(viewModel.SendHistory);
            Assert.Empty(viewModel.SendHistory);
        }

        [Fact]
        public void MaxSendHistoryCount_DefaultIs20()
        {
            var viewModel = new TerminalViewModel();
            Assert.Equal(20, viewModel.MaxSendHistoryCount);
        }

        [Fact]
        public void ReceiveBufferSizeOptions_ContainsExpectedValues()
        {
            var viewModel = new TerminalViewModel();
            Assert.Contains(65536, viewModel.ReceiveBufferSizeOptions);
            Assert.Contains(262144, viewModel.ReceiveBufferSizeOptions);
            Assert.Contains(1048576, viewModel.ReceiveBufferSizeOptions);
            Assert.Contains(4194304, viewModel.ReceiveBufferSizeOptions);
        }

        [Fact]
        public void SelectedSendLineEnding_DefaultIsNone()
        {
            var viewModel = new TerminalViewModel();
            Assert.Equal(SendLineEnding.None, viewModel.SelectedSendLineEnding);
        }

        [Fact]
        public void SelectedSendMode_DefaultIsText()
        {
            var viewModel = new TerminalViewModel();
            Assert.Equal(SendMode.Text, viewModel.SelectedSendMode);
        }

        [Fact]
        public void Commands_NotNull()
        {
            var viewModel = new TerminalViewModel();
            Assert.NotNull(viewModel.RefreshPortsCommand);
            Assert.NotNull(viewModel.ToggleConnectionCommand);
            Assert.NotNull(viewModel.SendCommand);
            Assert.NotNull(viewModel.ClearReceiveCommand);
            Assert.NotNull(viewModel.ClearSendHistoryCommand);
        }

        [Fact]
        public void DefaultConnectionState_IsDisconnected()
        {
            var viewModel = new TerminalViewModel();
            Assert.Equal(SerialConnectionState.Disconnected, viewModel.ConnectionState);
        }

        [Fact]
        public void DefaultStatusMessage_IsNotEmpty()
        {
            var viewModel = new TerminalViewModel();
            Assert.False(string.IsNullOrEmpty(viewModel.StatusMessage));
        }

        [Fact]
        public void DefaultSendText_IsEmpty()
        {
            var viewModel = new TerminalViewModel();
            Assert.Equal(string.Empty, viewModel.SendText);
        }

        [Fact]
        public void DefaultSentBytesCount_Is0()
        {
            var viewModel = new TerminalViewModel();
            Assert.Equal(0, viewModel.SentBytesCount);
        }

        [Fact]
        public void DefaultSelectedSendHistoryItem_IsNull()
        {
            var viewModel = new TerminalViewModel();
            Assert.Null(viewModel.SelectedSendHistoryItem);
        }

        [Fact]
        public void MaxSendHistoryCount_InvalidValue_ClampedTo20()
        {
            var viewModel = new TerminalViewModel();
            viewModel.MaxSendHistoryCount = -5;
            Assert.Equal(20, viewModel.MaxSendHistoryCount);
        }

        [Fact]
        public void RefreshPortsCommand_WithoutScanner_ProvidesStatusMessage()
        {
            var viewModel = new TerminalViewModel();
            viewModel.RefreshPortsCommand.Execute(null);
            Assert.Contains("尚未接入", viewModel.StatusMessage);
        }

        [Fact]
        public void RefreshPortsCommand_WithFakeScanner_ReturnsTwoPorts_UpdatesAvailablePorts()
        {
            var fakeScanner = new FakeSerialPortScanner(new List<SerialPortInfo>
            {
                SerialPortInfo.Create("COM1"),
                SerialPortInfo.Create("COM2")
            });
            var fakeService = new FakeSerialPortService();
            var viewModel = new TerminalViewModel(fakeScanner, fakeService);

            viewModel.RefreshPortsCommand.Execute(null);

            Assert.Equal(2, viewModel.SerialSettings.AvailablePorts.Count);
        }

        [Fact]
        public void RefreshPortsCommand_WithFakeScanner_ReturnsTwoPorts_SelectsFirstPort()
        {
            var fakeScanner = new FakeSerialPortScanner(new List<SerialPortInfo>
            {
                SerialPortInfo.Create("COM1"),
                SerialPortInfo.Create("COM2")
            });
            var fakeService = new FakeSerialPortService();
            var viewModel = new TerminalViewModel(fakeScanner, fakeService);

            viewModel.RefreshPortsCommand.Execute(null);

            Assert.Equal("COM1", viewModel.SerialSettings.SelectedPortName);
        }

        [Fact]
        public void RefreshPortsCommand_WithFakeScanner_ReturnsEmptyList_SetsSelectedPortNameToNull()
        {
            var fakeScanner = new FakeSerialPortScanner(new List<SerialPortInfo>());
            var fakeService = new FakeSerialPortService();
            var viewModel = new TerminalViewModel(fakeScanner, fakeService);

            viewModel.RefreshPortsCommand.Execute(null);

            Assert.Null(viewModel.SerialSettings.SelectedPortName);
        }

        [Fact]
        public void RefreshPortsCommand_Success_UpdatesStatusMessage()
        {
            var fakeScanner = new FakeSerialPortScanner(new List<SerialPortInfo>
            {
                SerialPortInfo.Create("COM1"),
                SerialPortInfo.Create("COM2")
            });
            var fakeService = new FakeSerialPortService();
            var viewModel = new TerminalViewModel(fakeScanner, fakeService);

            viewModel.RefreshPortsCommand.Execute(null);

            Assert.Contains("刷新", viewModel.StatusMessage);
        }

        [Fact]
        public void RefreshPortsCommand_Failure_UpdatesStatusMessageWithError()
        {
            var fakeScanner = new FakeSerialPortScanner(null, true, "Test error");
            var fakeService = new FakeSerialPortService();
            var viewModel = new TerminalViewModel(fakeScanner, fakeService);

            viewModel.RefreshPortsCommand.Execute(null);

            Assert.Contains("扫描失败", viewModel.StatusMessage);
            Assert.Contains("Test error", viewModel.StatusMessage);
        }

        [Fact]
        public void ToggleConnectionCommand_WithoutService_ProvidesStatusMessage()
        {
            var viewModel = new TerminalViewModel();
            viewModel.ToggleConnectionCommand.Execute(null);
            Assert.Contains("尚未接入", viewModel.StatusMessage);
        }

        [Fact]
        public void ToggleConnectionCommand_WithValidSettings_ChangesStateToConnected()
        {
            var fakeScanner = new FakeSerialPortScanner(new List<SerialPortInfo>
            {
                SerialPortInfo.Create("COM1")
            });
            var fakeService = new FakeSerialPortService();
            var viewModel = new TerminalViewModel(fakeScanner, fakeService);
            viewModel.SerialSettings.SelectedPortName = "COM1";

            viewModel.ToggleConnectionCommand.Execute(null);

            Assert.Equal(SerialConnectionState.Connected, viewModel.ConnectionState);
        }

        [Fact]
        public void Open_Success_IsSettingsEnabled_IsFalse()
        {
            var fakeScanner = new FakeSerialPortScanner(new List<SerialPortInfo>
            {
                SerialPortInfo.Create("COM1")
            });
            var fakeService = new FakeSerialPortService();
            var viewModel = new TerminalViewModel(fakeScanner, fakeService);
            viewModel.SerialSettings.SelectedPortName = "COM1";

            viewModel.ToggleConnectionCommand.Execute(null);

            Assert.False(viewModel.SerialSettings.IsSettingsEnabled);
        }

        [Fact]
        public void Open_Success_StatusMessage_ContainsOpened()
        {
            var fakeScanner = new FakeSerialPortScanner(new List<SerialPortInfo>
            {
                SerialPortInfo.Create("COM1")
            });
            var fakeService = new FakeSerialPortService();
            var viewModel = new TerminalViewModel(fakeScanner, fakeService);
            viewModel.SerialSettings.SelectedPortName = "COM1";

            viewModel.ToggleConnectionCommand.Execute(null);

            Assert.Contains("已打开", viewModel.StatusMessage);
        }

        [Fact]
        public void Open_Failure_State_ShouldNotBeConnected()
        {
            var fakeScanner = new FakeSerialPortScanner(new List<SerialPortInfo>
            {
                SerialPortInfo.Create("COM1")
            });
            var fakeService = new FakeSerialPortService(true, false, false, "Open failed");
            var viewModel = new TerminalViewModel(fakeScanner, fakeService);
            viewModel.SerialSettings.SelectedPortName = "COM1";

            viewModel.ToggleConnectionCommand.Execute(null);

            Assert.NotEqual(SerialConnectionState.Connected, viewModel.ConnectionState);
        }

        [Fact]
        public void Open_Failure_IsSettingsEnabled_IsTrue()
        {
            var fakeScanner = new FakeSerialPortScanner(new List<SerialPortInfo>
            {
                SerialPortInfo.Create("COM1")
            });
            var fakeService = new FakeSerialPortService(true, false, false, "Open failed");
            var viewModel = new TerminalViewModel(fakeScanner, fakeService);
            viewModel.SerialSettings.SelectedPortName = "COM1";

            viewModel.ToggleConnectionCommand.Execute(null);

            Assert.True(viewModel.SerialSettings.IsSettingsEnabled);
        }

        [Fact]
        public void Open_Failure_StatusMessage_ContainsError()
        {
            var fakeScanner = new FakeSerialPortScanner(new List<SerialPortInfo>
            {
                SerialPortInfo.Create("COM1")
            });
            var fakeService = new FakeSerialPortService(true, false, false, "Open failed");
            var viewModel = new TerminalViewModel(fakeScanner, fakeService);
            viewModel.SerialSettings.SelectedPortName = "COM1";

            viewModel.ToggleConnectionCommand.Execute(null);

            Assert.Contains("Open failed", viewModel.StatusMessage);
        }

        [Fact]
        public void Close_Success_State_IsDisconnected()
        {
            var fakeScanner = new FakeSerialPortScanner(new List<SerialPortInfo>
            {
                SerialPortInfo.Create("COM1")
            });
            var fakeService = new FakeSerialPortService();
            var viewModel = new TerminalViewModel(fakeScanner, fakeService);
            viewModel.SerialSettings.SelectedPortName = "COM1";
            viewModel.ToggleConnectionCommand.Execute(null);

            viewModel.ToggleConnectionCommand.Execute(null);

            Assert.Equal(SerialConnectionState.Disconnected, viewModel.ConnectionState);
        }

        [Fact]
        public void Close_Success_IsSettingsEnabled_IsTrue()
        {
            var fakeScanner = new FakeSerialPortScanner(new List<SerialPortInfo>
            {
                SerialPortInfo.Create("COM1")
            });
            var fakeService = new FakeSerialPortService();
            var viewModel = new TerminalViewModel(fakeScanner, fakeService);
            viewModel.SerialSettings.SelectedPortName = "COM1";
            viewModel.ToggleConnectionCommand.Execute(null);

            viewModel.ToggleConnectionCommand.Execute(null);

            Assert.True(viewModel.SerialSettings.IsSettingsEnabled);
        }

        [Fact]
        public void Close_Failure_StatusMessage_ContainsError()
        {
            var fakeScanner = new FakeSerialPortScanner(new List<SerialPortInfo>
            {
                SerialPortInfo.Create("COM1")
            });
            var fakeService = new FakeSerialPortService(false, true, false, null, "Close failed");
            var viewModel = new TerminalViewModel(fakeScanner, fakeService);
            viewModel.SerialSettings.SelectedPortName = "COM1";
            viewModel.ToggleConnectionCommand.Execute(null);

            viewModel.ToggleConnectionCommand.Execute(null);

            Assert.Contains("关闭串口失败", viewModel.StatusMessage);
        }

        [Fact]
        public void SendCommand_WithoutConnection_ProvidesStatusMessage()
        {
            var viewModel = new TerminalViewModel();
            viewModel.SendText = "test data";

            viewModel.SendCommand.Execute(null);

            Assert.Contains("尚未接入", viewModel.StatusMessage);
        }

        [Fact]
        public void SendCommand_CannotExecute_WhenSendTextEmpty()
        {
            var viewModel = new TerminalViewModel();
            viewModel.SendText = string.Empty;

            Assert.False(viewModel.SendCommand.CanExecute(null));
        }

        [Fact]
        public void SendCommand_CannotExecute_WhenNotConnected_EvenIfSendTextNotEmpty()
        {
            var fakeScanner = new FakeSerialPortScanner();
            var fakeService = new FakeSerialPortService();
            var viewModel = new TerminalViewModel(fakeScanner, fakeService);
            viewModel.SendText = "test data";

            Assert.False(viewModel.SendCommand.CanExecute(null));
        }

        [Fact]
        public void SendCommand_TextMode_SendsCorrectData()
        {
            var fakeScanner = new FakeSerialPortScanner();
            var fakeService = new FakeSerialPortService();
            var viewModel = new TerminalViewModel(fakeScanner, fakeService);

            viewModel.SerialSettings.SelectedPortName = "COM1";
            viewModel.ToggleConnectionCommand.Execute(null);

            viewModel.SelectedSendMode = SendMode.Text;
            viewModel.SendText = "ABC";

            viewModel.SendCommand.Execute(null);

            Assert.Single(fakeService.SentData);
            Assert.Equal(Encoding.UTF8.GetBytes("ABC"), fakeService.SentData[0]);
            Assert.Equal(3, viewModel.SentBytesCount);
            Assert.Contains("已发送", viewModel.StatusMessage);
        }

        [Fact]
        public void SendCommand_TextMode_None_SendsCorrectData()
        {
            var fakeScanner = new FakeSerialPortScanner();
            var fakeService = new FakeSerialPortService();
            var viewModel = new TerminalViewModel(fakeScanner, fakeService);

            viewModel.SerialSettings.SelectedPortName = "COM1";
            viewModel.ToggleConnectionCommand.Execute(null);
            viewModel.SelectedSendMode = SendMode.Text;
            viewModel.SelectedSendLineEnding = SendLineEnding.None;
            viewModel.SendText = "ABC";

            viewModel.SendCommand.Execute(null);

            Assert.Single(fakeService.SentData);
            Assert.Equal(Encoding.UTF8.GetBytes("ABC"), fakeService.SentData[0]);
            Assert.Equal(3, viewModel.SentBytesCount);
        }

        [Fact]
        public void SendCommand_TextMode_CR_SendsCorrectData()
        {
            var fakeScanner = new FakeSerialPortScanner();
            var fakeService = new FakeSerialPortService();
            var viewModel = new TerminalViewModel(fakeScanner, fakeService);

            viewModel.SerialSettings.SelectedPortName = "COM1";
            viewModel.ToggleConnectionCommand.Execute(null);
            viewModel.SelectedSendMode = SendMode.Text;
            viewModel.SelectedSendLineEnding = SendLineEnding.CR;
            viewModel.SendText = "ABC";

            viewModel.SendCommand.Execute(null);

            Assert.Single(fakeService.SentData);
            Assert.Equal(Encoding.UTF8.GetBytes("ABC\r"), fakeService.SentData[0]);
            Assert.Equal(4, viewModel.SentBytesCount);
        }

        [Fact]
        public void SendCommand_TextMode_LF_SendsCorrectData()
        {
            var fakeScanner = new FakeSerialPortScanner();
            var fakeService = new FakeSerialPortService();
            var viewModel = new TerminalViewModel(fakeScanner, fakeService);

            viewModel.SerialSettings.SelectedPortName = "COM1";
            viewModel.ToggleConnectionCommand.Execute(null);
            viewModel.SelectedSendMode = SendMode.Text;
            viewModel.SelectedSendLineEnding = SendLineEnding.LF;
            viewModel.SendText = "ABC";

            viewModel.SendCommand.Execute(null);

            Assert.Single(fakeService.SentData);
            Assert.Equal(Encoding.UTF8.GetBytes("ABC\n"), fakeService.SentData[0]);
            Assert.Equal(4, viewModel.SentBytesCount);
        }

        [Fact]
        public void SendCommand_TextMode_CRLF_SendsCorrectData()
        {
            var fakeScanner = new FakeSerialPortScanner();
            var fakeService = new FakeSerialPortService();
            var viewModel = new TerminalViewModel(fakeScanner, fakeService);

            viewModel.SerialSettings.SelectedPortName = "COM1";
            viewModel.ToggleConnectionCommand.Execute(null);
            viewModel.SelectedSendMode = SendMode.Text;
            viewModel.SelectedSendLineEnding = SendLineEnding.CRLF;
            viewModel.SendText = "ABC";

            viewModel.SendCommand.Execute(null);

            Assert.Single(fakeService.SentData);
            Assert.Equal(Encoding.UTF8.GetBytes("ABC\r\n"), fakeService.SentData[0]);
            Assert.Equal(5, viewModel.SentBytesCount);
        }

        [Fact]
        public void SendCommand_HexMode_WithSpaces_SendsCorrectData()
        {
            var fakeScanner = new FakeSerialPortScanner();
            var fakeService = new FakeSerialPortService();
            var viewModel = new TerminalViewModel(fakeScanner, fakeService);

            viewModel.SerialSettings.SelectedPortName = "COM1";
            viewModel.ToggleConnectionCommand.Execute(null);
            viewModel.SelectedSendMode = SendMode.Hex;
            viewModel.SendText = "41 42 43";

            viewModel.SendCommand.Execute(null);

            Assert.Single(fakeService.SentData);
            Assert.Equal(new byte[] { 0x41, 0x42, 0x43 }, fakeService.SentData[0]);
            Assert.Equal(3, viewModel.SentBytesCount);
        }

        [Fact]
        public void SendCommand_HexMode_WithoutSpaces_SendsCorrectData()
        {
            var fakeScanner = new FakeSerialPortScanner();
            var fakeService = new FakeSerialPortService();
            var viewModel = new TerminalViewModel(fakeScanner, fakeService);

            viewModel.SerialSettings.SelectedPortName = "COM1";
            viewModel.ToggleConnectionCommand.Execute(null);
            viewModel.SelectedSendMode = SendMode.Hex;
            viewModel.SendText = "414243";

            viewModel.SendCommand.Execute(null);

            Assert.Single(fakeService.SentData);
            Assert.Equal(new byte[] { 0x41, 0x42, 0x43 }, fakeService.SentData[0]);
            Assert.Equal(3, viewModel.SentBytesCount);
        }

        [Fact]
        public void SendCommand_HexMode_DoesNotAppendLineEnding()
        {
            var fakeScanner = new FakeSerialPortScanner();
            var fakeService = new FakeSerialPortService();
            var viewModel = new TerminalViewModel(fakeScanner, fakeService);

            viewModel.SerialSettings.SelectedPortName = "COM1";
            viewModel.ToggleConnectionCommand.Execute(null);
            viewModel.SelectedSendMode = SendMode.Hex;
            viewModel.SelectedSendLineEnding = SendLineEnding.CRLF;
            viewModel.SendText = "41 42 43";

            viewModel.SendCommand.Execute(null);

            Assert.Single(fakeService.SentData);
            Assert.Equal(new byte[] { 0x41, 0x42, 0x43 }, fakeService.SentData[0]);
            Assert.Equal(3, viewModel.SentBytesCount);
        }

        [Fact]
        public void SendCommand_InvalidHex_DoesNotSend()
        {
            var fakeScanner = new FakeSerialPortScanner();
            var fakeService = new FakeSerialPortService();
            var viewModel = new TerminalViewModel(fakeScanner, fakeService);

            viewModel.SerialSettings.SelectedPortName = "COM1";
            viewModel.ToggleConnectionCommand.Execute(null);
            viewModel.SelectedSendMode = SendMode.Hex;
            viewModel.SendText = "41 4G";

            viewModel.SendCommand.Execute(null);

            Assert.Empty(fakeService.SentData);
            Assert.Contains("HEX 格式错误", viewModel.StatusMessage);
        }

        [Fact]
        public void SendCommand_OddLengthHex_DoesNotSend()
        {
            var fakeScanner = new FakeSerialPortScanner();
            var fakeService = new FakeSerialPortService();
            var viewModel = new TerminalViewModel(fakeScanner, fakeService);

            viewModel.SerialSettings.SelectedPortName = "COM1";
            viewModel.ToggleConnectionCommand.Execute(null);
            viewModel.SelectedSendMode = SendMode.Hex;
            viewModel.SendText = "414";

            viewModel.SendCommand.Execute(null);

            Assert.Empty(fakeService.SentData);
            Assert.Contains("HEX 格式错误", viewModel.StatusMessage);
        }

        [Fact]
        public void SendCommand_EmptyText_DoesNotSend()
        {
            var fakeScanner = new FakeSerialPortScanner();
            var fakeService = new FakeSerialPortService();
            var viewModel = new TerminalViewModel(fakeScanner, fakeService);

            viewModel.SerialSettings.SelectedPortName = "COM1";
            viewModel.ToggleConnectionCommand.Execute(null);
            viewModel.SendText = string.Empty;

            viewModel.SendCommand.Execute(null);

            Assert.Empty(fakeService.SentData);
            Assert.Contains("发送内容不能为空", viewModel.StatusMessage);
        }

        [Fact]
        public void SendCommand_WhenNotConnected_DoesNotSend()
        {
            var fakeScanner = new FakeSerialPortScanner();
            var fakeService = new FakeSerialPortService();
            var viewModel = new TerminalViewModel(fakeScanner, fakeService);
            viewModel.SendText = "Hello";

            viewModel.SendCommand.Execute(null);

            Assert.Empty(fakeService.SentData);
            Assert.Contains("串口未打开", viewModel.StatusMessage);
        }

        [Fact]
        public void SendCommand_Failure_UpdatesStatusMessage()
        {
            var fakeScanner = new FakeSerialPortScanner();
            var fakeService = new FakeSerialPortService(shouldFailSend: true, sendErrorMessage: "Fake send failure");
            var viewModel = new TerminalViewModel(fakeScanner, fakeService);

            viewModel.SerialSettings.SelectedPortName = "COM1";
            viewModel.ToggleConnectionCommand.Execute(null);
            viewModel.SelectedSendMode = SendMode.Text;
            viewModel.SendText = "Hello";

            viewModel.SendCommand.Execute(null);

            Assert.Empty(fakeService.SentData);
            Assert.Contains("发送失败", viewModel.StatusMessage);
            Assert.Contains("Fake send failure", viewModel.StatusMessage);
        }

        [Fact]
        public void SendCommand_TextMode_Success_AppendsTxRecord()
        {
            var fakeScanner = new FakeSerialPortScanner();
            var fakeService = new FakeSerialPortService();
            var viewModel = new TerminalViewModel(fakeScanner, fakeService);
            viewModel.SerialSettings.SelectedPortName = "COM1";
            viewModel.ToggleConnectionCommand.Execute(null);
            viewModel.SelectedSendMode = SendMode.Text;
            viewModel.SendText = "ABC";
            viewModel.ReceiveDisplay.ShowTimestamp = false;

            viewModel.SendCommand.Execute(null);

            Assert.Contains("TX ", viewModel.ReceiveDisplay.ReceivedText);
            Assert.Contains("ABC", viewModel.ReceiveDisplay.ReceivedText);
        }

        [Fact]
        public void SendCommand_HexMode_Success_AppendsTxRecord()
        {
            var fakeScanner = new FakeSerialPortScanner();
            var fakeService = new FakeSerialPortService();
            var viewModel = new TerminalViewModel(fakeScanner, fakeService);
            viewModel.SerialSettings.SelectedPortName = "COM1";
            viewModel.ToggleConnectionCommand.Execute(null);
            viewModel.SelectedSendMode = SendMode.Hex;
            viewModel.SendText = "41 42 43";
            viewModel.ReceiveDisplay.ShowTimestamp = false;

            viewModel.SendCommand.Execute(null);

            Assert.Contains("TX ", viewModel.ReceiveDisplay.ReceivedText);
            Assert.Contains("ABC", viewModel.ReceiveDisplay.ReceivedText);
        }

        [Fact]
        public void SendCommand_Failure_DoesNotAppendTxRecord()
        {
            var fakeScanner = new FakeSerialPortScanner();
            var fakeService = new FakeSerialPortService(shouldFailSend: true, sendErrorMessage: "Fake send failure");
            var viewModel = new TerminalViewModel(fakeScanner, fakeService);
            viewModel.SerialSettings.SelectedPortName = "COM1";
            viewModel.ToggleConnectionCommand.Execute(null);
            viewModel.SelectedSendMode = SendMode.Text;
            viewModel.SendText = "ABC";
            viewModel.ReceiveDisplay.ShowTimestamp = false;

            viewModel.SendCommand.Execute(null);

            Assert.DoesNotContain("TX ", viewModel.ReceiveDisplay.ReceivedText);
            Assert.Equal(string.Empty, viewModel.ReceiveDisplay.ReceivedText);
        }

        [Fact]
        public void SendCommand_NotConnected_DoesNotAppendTxRecord()
        {
            var fakeScanner = new FakeSerialPortScanner();
            var fakeService = new FakeSerialPortService();
            var viewModel = new TerminalViewModel(fakeScanner, fakeService);
            viewModel.SelectedSendMode = SendMode.Text;
            viewModel.SendText = "ABC";
            viewModel.ReceiveDisplay.ShowTimestamp = false;

            viewModel.SendCommand.Execute(null);

            Assert.DoesNotContain("TX ", viewModel.ReceiveDisplay.ReceivedText);
            Assert.Equal(string.Empty, viewModel.ReceiveDisplay.ReceivedText);
        }

        [Fact]
        public void SendCommand_InvalidHex_DoesNotAppendTxRecord()
        {
            var fakeScanner = new FakeSerialPortScanner();
            var fakeService = new FakeSerialPortService();
            var viewModel = new TerminalViewModel(fakeScanner, fakeService);
            viewModel.SerialSettings.SelectedPortName = "COM1";
            viewModel.ToggleConnectionCommand.Execute(null);
            viewModel.SelectedSendMode = SendMode.Hex;
            viewModel.SendText = "41 4G";
            viewModel.ReceiveDisplay.ShowTimestamp = false;

            viewModel.SendCommand.Execute(null);

            Assert.DoesNotContain("TX ", viewModel.ReceiveDisplay.ReceivedText);
            Assert.Equal(string.Empty, viewModel.ReceiveDisplay.ReceivedText);
        }

        [Fact]
        public void SendCommand_TxRecord_DoesNotIncreaseReceivedBytesCount()
        {
            var fakeScanner = new FakeSerialPortScanner();
            var fakeService = new FakeSerialPortService();
            var viewModel = new TerminalViewModel(fakeScanner, fakeService);
            viewModel.SerialSettings.SelectedPortName = "COM1";
            viewModel.ToggleConnectionCommand.Execute(null);
            viewModel.SelectedSendMode = SendMode.Text;
            viewModel.SendText = "ABC";

            viewModel.SendCommand.Execute(null);

            Assert.Equal(0, viewModel.ReceiveDisplay.ReceivedBytesCount);
        }

        [Fact]
        public void DataReceived_UpdatesReceivedBytesCount()
        {
            var fakeScanner = new FakeSerialPortScanner();
            var fakeService = new FakeSerialPortService();
            var fakeUiInvoker = new FakeUiThreadInvoker();
            var viewModel = new TerminalViewModel(fakeScanner, fakeService, fakeUiInvoker);

            viewModel.SerialSettings.SelectedPortName = "COM1";
            viewModel.ToggleConnectionCommand.Execute(null);

            byte[] data = new byte[] { 0x41, 0x42, 0x43 };

            fakeService.SimulateDataReceived(data);

            Assert.Equal(3, viewModel.ReceiveDisplay.ReceivedBytesCount);
        }

        [Fact]
        public void DataReceived_UpdatesReceivedText()
        {
            var fakeScanner = new FakeSerialPortScanner();
            var fakeService = new FakeSerialPortService();
            var fakeUiInvoker = new FakeUiThreadInvoker();
            var viewModel = new TerminalViewModel(fakeScanner, fakeService, fakeUiInvoker);

            viewModel.SerialSettings.SelectedPortName = "COM1";
            viewModel.ToggleConnectionCommand.Execute(null);

            viewModel.ReceiveDisplay.IsHexDisplay = false;
            viewModel.ReceiveDisplay.ShowTimestamp = false;
            viewModel.ReceiveDisplay.ShowDirection = false;
            byte[] data = Encoding.UTF8.GetBytes("ABC");

            fakeService.SimulateDataReceived(data);

            Assert.Equal("ABC", viewModel.ReceiveDisplay.ReceivedText);
        }

        [Fact]
        public void DataReceived_TextMode_DisplaysABC()
        {
            var fakeScanner = new FakeSerialPortScanner();
            var fakeService = new FakeSerialPortService();
            var fakeUiInvoker = new FakeUiThreadInvoker();
            var viewModel = new TerminalViewModel(fakeScanner, fakeService, fakeUiInvoker);

            viewModel.SerialSettings.SelectedPortName = "COM1";
            viewModel.ToggleConnectionCommand.Execute(null);

            viewModel.ReceiveDisplay.IsHexDisplay = false;
            viewModel.ReceiveDisplay.ShowTimestamp = false;
            viewModel.ReceiveDisplay.ShowDirection = false;
            byte[] data = Encoding.UTF8.GetBytes("ABC");

            fakeService.SimulateDataReceived(data);

            Assert.Equal("ABC", viewModel.ReceiveDisplay.ReceivedText);
        }

        [Fact]
        public void DataReceived_HexMode_Displays414243()
        {
            var fakeScanner = new FakeSerialPortScanner();
            var fakeService = new FakeSerialPortService();
            var fakeUiInvoker = new FakeUiThreadInvoker();
            var viewModel = new TerminalViewModel(fakeScanner, fakeService, fakeUiInvoker);

            viewModel.SerialSettings.SelectedPortName = "COM1";
            viewModel.ToggleConnectionCommand.Execute(null);

            viewModel.ReceiveDisplay.IsHexDisplay = true;
            byte[] data = new byte[] { 0x41, 0x42, 0x43 };

            fakeService.SimulateDataReceived(data);

            Assert.Contains("41 42 43", viewModel.ReceiveDisplay.ReceivedText);
        }

        [Fact]
        public void DataReceived_Multiple_CountAccumulates()
        {
            var fakeScanner = new FakeSerialPortScanner();
            var fakeService = new FakeSerialPortService();
            var fakeUiInvoker = new FakeUiThreadInvoker();
            var viewModel = new TerminalViewModel(fakeScanner, fakeService, fakeUiInvoker);

            viewModel.SerialSettings.SelectedPortName = "COM1";
            viewModel.ToggleConnectionCommand.Execute(null);

            byte[] data1 = new byte[] { 0x41, 0x42 };
            byte[] data2 = new byte[] { 0x43, 0x44 };

            fakeService.SimulateDataReceived(data1);
            fakeService.SimulateDataReceived(data2);

            Assert.Equal(4, viewModel.ReceiveDisplay.ReceivedBytesCount);
        }

        [Fact]
        public void DataReceived_AppendsRxRecord()
        {
            var fakeScanner = new FakeSerialPortScanner();
            var fakeService = new FakeSerialPortService();
            var fakeUiInvoker = new FakeUiThreadInvoker();
            var viewModel = new TerminalViewModel(fakeScanner, fakeService, fakeUiInvoker);
            viewModel.SerialSettings.SelectedPortName = "COM1";
            viewModel.ToggleConnectionCommand.Execute(null);
            viewModel.ReceiveDisplay.ShowTimestamp = false;
            byte[] data = new byte[] { 0x4F, 0x4B };

            fakeService.SimulateDataReceived(data);

            Assert.Contains("RX ", viewModel.ReceiveDisplay.ReceivedText);
            Assert.Contains("OK", viewModel.ReceiveDisplay.ReceivedText);
        }

        [Fact]
        public void DataReceived_ViaUiThreadInvoker()
        {
            var fakeScanner = new FakeSerialPortScanner();
            var fakeService = new FakeSerialPortService();
            var fakeUiInvoker = new FakeUiThreadInvoker();
            var viewModel = new TerminalViewModel(fakeScanner, fakeService, fakeUiInvoker);
            viewModel.SerialSettings.SelectedPortName = "COM1";
            viewModel.ToggleConnectionCommand.Execute(null);
            byte[] data = new byte[] { 0x41 };

            fakeService.SimulateDataReceived(data);

            Assert.Equal(1, fakeUiInvoker.InvokeCount);
        }

        [Fact]
        public void ClearReceiveCommand_ClearsTxAndRxRecords()
        {
            var fakeScanner = new FakeSerialPortScanner();
            var fakeService = new FakeSerialPortService();
            var fakeUiInvoker = new FakeUiThreadInvoker();
            var viewModel = new TerminalViewModel(fakeScanner, fakeService, fakeUiInvoker);
            viewModel.SerialSettings.SelectedPortName = "COM1";
            viewModel.ToggleConnectionCommand.Execute(null);
            viewModel.ReceiveDisplay.ShowTimestamp = false;

            viewModel.SelectedSendMode = SendMode.Text;
            viewModel.SendText = "TXDATA";
            viewModel.SendCommand.Execute(null);

            byte[] rxData = Encoding.UTF8.GetBytes("RXDATA");
            fakeService.SimulateDataReceived(rxData);

            Assert.False(string.IsNullOrEmpty(viewModel.ReceiveDisplay.ReceivedText));

            viewModel.ClearReceiveCommand.Execute(null);

            Assert.Equal(string.Empty, viewModel.ReceiveDisplay.ReceivedText);
            Assert.Equal(0, viewModel.ReceiveDisplay.ReceivedBytesCount);
        }

        [Fact]
        public void ErrorOccurred_UpdatesStatusMessage()
        {
            var fakeScanner = new FakeSerialPortScanner();
            var fakeService = new FakeSerialPortService();
            var fakeUiInvoker = new FakeUiThreadInvoker();
            var viewModel = new TerminalViewModel(fakeScanner, fakeService, fakeUiInvoker);

            viewModel.SerialSettings.SelectedPortName = "COM1";
            viewModel.ToggleConnectionCommand.Execute(null);

            Exception ex = new Exception("Test receive error");

            fakeService.SimulateErrorOccurred(ex);

            Assert.Contains("接收串口数据失败", viewModel.StatusMessage);
            Assert.Contains("Test receive error", viewModel.StatusMessage);
        }

        [Fact]
        public void RefreshPortsCommand_CanExecute()
        {
            var viewModel = new TerminalViewModel();
            Assert.True(viewModel.RefreshPortsCommand.CanExecute(null));
        }

        [Fact]
        public void ToggleConnectionCommand_CanExecute_WhenServiceExists()
        {
            var fakeScanner = new FakeSerialPortScanner();
            var fakeService = new FakeSerialPortService();
            var viewModel = new TerminalViewModel(fakeScanner, fakeService);

            Assert.True(viewModel.ToggleConnectionCommand.CanExecute(null));
        }

        [Fact]
        public void ClearReceiveCommand_CanExecute()
        {
            var viewModel = new TerminalViewModel();
            Assert.True(viewModel.ClearReceiveCommand.CanExecute(null));
        }

        [Fact]
        public void ToggleConnectionCommand_WithInvalidSettings_DoesNotCallOpen()
        {
            var fakeScanner = new FakeSerialPortScanner(new List<SerialPortInfo>());
            var fakeService = new FakeSerialPortService();
            var viewModel = new TerminalViewModel(fakeScanner, fakeService);
            viewModel.SerialSettings.SelectedPortName = null;

            viewModel.ToggleConnectionCommand.Execute(null);

            Assert.Equal(SerialConnectionState.Disconnected, viewModel.ConnectionState);
            Assert.Contains("参数无效", viewModel.StatusMessage);
        }

        [Fact]
        public void DefaultConfig_MaxDisplayBytes_Is262144()
        {
            var viewModel = new TerminalViewModel();
            Assert.Equal(262144, viewModel.ReceiveDisplay.MaxDisplayBytes);
        }

        [Fact]
        public void LoadSettings_RestoresBaudRate()
        {
            var fakeScanner = new FakeSerialPortScanner();
            var fakeService = new FakeSerialPortService();
            var fakeUiInvoker = new FakeUiThreadInvoker();
            var fakeSettingsService = new FakeAppSettingsService();
            var customSettings = new AppSettings
            {
                BaudRate = 115200,
                DataBits = 7,
                Parity = "Odd",
                StopBits = "Two",
                SendMode = SendMode.Hex,
                DisplayMode = DisplayMode.Hex
            };
            fakeSettingsService.Save(customSettings);

            var viewModel = new TerminalViewModel(fakeScanner, fakeService, fakeUiInvoker, fakeSettingsService);

            Assert.Equal(115200, viewModel.SerialSettings.SelectedBaudRate);
            Assert.Equal(7, viewModel.SerialSettings.SelectedDataBits);
            Assert.Equal("Odd", viewModel.SerialSettings.SelectedParity);
            Assert.Equal("Two", viewModel.SerialSettings.SelectedStopBits);
            Assert.Equal(SendMode.Hex, viewModel.SelectedSendMode);
            Assert.True(viewModel.ReceiveDisplay.IsHexDisplay);
        }

        [Fact]
        public void SaveSettings_CallsSettingsService()
        {
            var fakeScanner = new FakeSerialPortScanner();
            var fakeService = new FakeSerialPortService();
            var fakeUiInvoker = new FakeUiThreadInvoker();
            var fakeSettingsService = new FakeAppSettingsService();
            var viewModel = new TerminalViewModel(fakeScanner, fakeService, fakeUiInvoker, fakeSettingsService);

            viewModel.SerialSettings.SelectedPortName = "COM3";
            viewModel.SerialSettings.SelectedBaudRate = 9600;
            viewModel.SelectedSendMode = SendMode.Text;
            viewModel.ReceiveDisplay.IsHexDisplay = false;

            var result = viewModel.SaveSettings();

            Assert.True(result.IsSuccess);
            Assert.Equal("COM3", fakeSettingsService.GetSavedSettings().LastPortName);
            Assert.Equal(9600, fakeSettingsService.GetSavedSettings().BaudRate);
            Assert.Equal(SendMode.Text, fakeSettingsService.GetSavedSettings().SendMode);
            Assert.Equal(DisplayMode.Text, fakeSettingsService.GetSavedSettings().DisplayMode);
        }

        [Fact]
        public void SaveSettings_Failure_ReturnsFailure()
        {
            var fakeScanner = new FakeSerialPortScanner();
            var fakeService = new FakeSerialPortService();
            var fakeUiInvoker = new FakeUiThreadInvoker();
            var fakeSettingsService = new FakeAppSettingsService();
            fakeSettingsService.SetFailSave(true);
            var viewModel = new TerminalViewModel(fakeScanner, fakeService, fakeUiInvoker, fakeSettingsService);

            var result = viewModel.SaveSettings();

            Assert.False(result.IsSuccess);
            Assert.Equal("Save failed.", result.ErrorMessage);
        }

        [Fact]
        public void RefreshPorts_LastPortNameExists_SelectsIt()
        {
            var fakeScanner = new FakeSerialPortScanner(new List<SerialPortInfo>
            {
                SerialPortInfo.Create("COM1"),
                SerialPortInfo.Create("COM2"),
                SerialPortInfo.Create("COM3")
            });
            var fakeService = new FakeSerialPortService();
            var fakeUiInvoker = new FakeUiThreadInvoker();
            var fakeSettingsService = new FakeAppSettingsService();
            var customSettings = new AppSettings { LastPortName = "COM2" };
            fakeSettingsService.Save(customSettings);
            var viewModel = new TerminalViewModel(fakeScanner, fakeService, fakeUiInvoker, fakeSettingsService);

            viewModel.RefreshPortsCommand.Execute(null);

            Assert.Equal("COM2", viewModel.SerialSettings.SelectedPortName);
        }

        [Fact]
        public void RefreshPorts_LastPortNameNotExists_SelectsFirst()
        {
            var fakeScanner = new FakeSerialPortScanner(new List<SerialPortInfo>
            {
                SerialPortInfo.Create("COM1"),
                SerialPortInfo.Create("COM2")
            });
            var fakeUiInvoker = new FakeUiThreadInvoker();
            var fakeSettingsService = new FakeAppSettingsService();
            var customSettings = new AppSettings { LastPortName = "COM999" };
            fakeSettingsService.Save(customSettings);
            var viewModel = new TerminalViewModel(fakeScanner, null, fakeUiInvoker, fakeSettingsService);

            viewModel.RefreshPortsCommand.Execute(null);

            Assert.Equal("COM1", viewModel.SerialSettings.SelectedPortName);
        }

        [Fact]
        public void LoadSettings_RestoresSendLineEnding()
        {
            var fakeScanner = new FakeSerialPortScanner();
            var fakeService = new FakeSerialPortService();
            var fakeUiInvoker = new FakeUiThreadInvoker();
            var fakeSettingsService = new FakeAppSettingsService();
            var customSettings = new AppSettings
            {
                SendLineEnding = SendLineEnding.CRLF
            };
            fakeSettingsService.Save(customSettings);

            var viewModel = new TerminalViewModel(fakeScanner, fakeService, fakeUiInvoker, fakeSettingsService);

            Assert.Equal(SendLineEnding.CRLF, viewModel.SelectedSendLineEnding);
        }

        [Fact]
        public void SaveSettings_SavesSendLineEnding()
        {
            var fakeScanner = new FakeSerialPortScanner();
            var fakeService = new FakeSerialPortService();
            var fakeUiInvoker = new FakeUiThreadInvoker();
            var fakeSettingsService = new FakeAppSettingsService();
            var viewModel = new TerminalViewModel(fakeScanner, fakeService, fakeUiInvoker, fakeSettingsService);
            viewModel.SelectedSendLineEnding = SendLineEnding.CRLF;

            var result = viewModel.SaveSettings();

            Assert.True(result.IsSuccess);
            Assert.Equal(SendLineEnding.CRLF, fakeSettingsService.GetSavedSettings().SendLineEnding);
        }

        [Fact]
        public void LoadSettings_RestoresShowTimestamp()
        {
            var fakeScanner = new FakeSerialPortScanner();
            var fakeService = new FakeSerialPortService();
            var fakeUiInvoker = new FakeUiThreadInvoker();
            var fakeSettingsService = new FakeAppSettingsService();
            var customSettings = new AppSettings
            {
                ShowTimestamp = false
            };
            fakeSettingsService.Save(customSettings);

            var viewModel = new TerminalViewModel(fakeScanner, fakeService, fakeUiInvoker, fakeSettingsService);

            Assert.False(viewModel.ReceiveDisplay.ShowTimestamp);
        }

        [Fact]
        public void LoadSettings_RestoresShowDirection()
        {
            var fakeScanner = new FakeSerialPortScanner();
            var fakeService = new FakeSerialPortService();
            var fakeUiInvoker = new FakeUiThreadInvoker();
            var fakeSettingsService = new FakeAppSettingsService();
            var customSettings = new AppSettings
            {
                ShowDirection = false
            };
            fakeSettingsService.Save(customSettings);

            var viewModel = new TerminalViewModel(fakeScanner, fakeService, fakeUiInvoker, fakeSettingsService);

            Assert.False(viewModel.ReceiveDisplay.ShowDirection);
        }

        [Fact]
        public void SaveSettings_SavesShowTimestamp()
        {
            var fakeScanner = new FakeSerialPortScanner();
            var fakeService = new FakeSerialPortService();
            var fakeUiInvoker = new FakeUiThreadInvoker();
            var fakeSettingsService = new FakeAppSettingsService();
            var viewModel = new TerminalViewModel(fakeScanner, fakeService, fakeUiInvoker, fakeSettingsService);
            viewModel.ReceiveDisplay.ShowTimestamp = false;

            var result = viewModel.SaveSettings();

            Assert.True(result.IsSuccess);
            Assert.False(fakeSettingsService.GetSavedSettings().ShowTimestamp);
        }

        [Fact]
        public void SaveSettings_SavesShowDirection()
        {
            var fakeScanner = new FakeSerialPortScanner();
            var fakeService = new FakeSerialPortService();
            var fakeUiInvoker = new FakeUiThreadInvoker();
            var fakeSettingsService = new FakeAppSettingsService();
            var viewModel = new TerminalViewModel(fakeScanner, fakeService, fakeUiInvoker, fakeSettingsService);
            viewModel.ReceiveDisplay.ShowDirection = false;

            var result = viewModel.SaveSettings();

            Assert.True(result.IsSuccess);
            Assert.False(fakeSettingsService.GetSavedSettings().ShowDirection);
        }

        [Fact]
        public void LoadSettings_RestoresMaxDisplayBytes()
        {
            var fakeScanner = new FakeSerialPortScanner();
            var fakeService = new FakeSerialPortService();
            var fakeUiInvoker = new FakeUiThreadInvoker();
            var fakeSettingsService = new FakeAppSettingsService();
            var customSettings = new AppSettings { MaxDisplayBytes = 65536 };
            fakeSettingsService.Save(customSettings);

            var viewModel = new TerminalViewModel(fakeScanner, fakeService, fakeUiInvoker, fakeSettingsService);

            Assert.Equal(65536, viewModel.ReceiveDisplay.MaxDisplayBytes);
        }

        [Fact]
        public void SaveSettings_SavesMaxDisplayBytes()
        {
            var fakeScanner = new FakeSerialPortScanner();
            var fakeService = new FakeSerialPortService();
            var fakeUiInvoker = new FakeUiThreadInvoker();
            var fakeSettingsService = new FakeAppSettingsService();
            var viewModel = new TerminalViewModel(fakeScanner, fakeService, fakeUiInvoker, fakeSettingsService);
            viewModel.ReceiveDisplay.MaxDisplayBytes = 1048576;

            var result = viewModel.SaveSettings();

            Assert.True(result.IsSuccess);
            Assert.Equal(1048576, fakeSettingsService.GetSavedSettings().MaxDisplayBytes);
        }

        [Fact]
        public void SendCommand_TextMode_Success_AddsHistory()
        {
            var fakeScanner = new FakeSerialPortScanner();
            var fakeService = new FakeSerialPortService();
            var viewModel = new TerminalViewModel(fakeScanner, fakeService);
            viewModel.SerialSettings.SelectedPortName = "COM1";
            viewModel.ToggleConnectionCommand.Execute(null);
            viewModel.SelectedSendMode = SendMode.Text;
            viewModel.SendText = "ABC";

            viewModel.SendCommand.Execute(null);

            Assert.Single(viewModel.SendHistory);
            Assert.Equal("ABC", viewModel.SendHistory[0].Content);
            Assert.Equal(SendMode.Text, viewModel.SendHistory[0].SendMode);
        }

        [Fact]
        public void SendCommand_HexMode_Success_AddsHistory()
        {
            var fakeScanner = new FakeSerialPortScanner();
            var fakeService = new FakeSerialPortService();
            var viewModel = new TerminalViewModel(fakeScanner, fakeService);
            viewModel.SerialSettings.SelectedPortName = "COM1";
            viewModel.ToggleConnectionCommand.Execute(null);
            viewModel.SelectedSendMode = SendMode.Hex;
            viewModel.SendText = "41 42 43";

            viewModel.SendCommand.Execute(null);

            Assert.Single(viewModel.SendHistory);
            Assert.Equal("41 42 43", viewModel.SendHistory[0].Content);
            Assert.Equal(SendMode.Hex, viewModel.SendHistory[0].SendMode);
        }

        [Fact]
        public void SendCommand_RecordsOriginalSendText()
        {
            var fakeScanner = new FakeSerialPortScanner();
            var fakeService = new FakeSerialPortService();
            var viewModel = new TerminalViewModel(fakeScanner, fakeService);
            viewModel.SerialSettings.SelectedPortName = "COM1";
            viewModel.ToggleConnectionCommand.Execute(null);
            viewModel.SelectedSendMode = SendMode.Text;
            viewModel.SelectedSendLineEnding = SendLineEnding.CRLF;
            viewModel.SendText = "Hello";

            viewModel.SendCommand.Execute(null);

            Assert.Single(viewModel.SendHistory);
            Assert.Equal("Hello", viewModel.SendHistory[0].Content);
            Assert.DoesNotContain("\r\n", viewModel.SendHistory[0].Content);
        }

        [Fact]
        public void SendCommand_TextMode_CRLF_HistoryDoesNotContainCRLF()
        {
            var fakeScanner = new FakeSerialPortScanner();
            var fakeService = new FakeSerialPortService();
            var viewModel = new TerminalViewModel(fakeScanner, fakeService);
            viewModel.SerialSettings.SelectedPortName = "COM1";
            viewModel.ToggleConnectionCommand.Execute(null);
            viewModel.SelectedSendMode = SendMode.Text;
            viewModel.SelectedSendLineEnding = SendLineEnding.CRLF;
            viewModel.SendText = "ABC";

            viewModel.SendCommand.Execute(null);

            Assert.Single(viewModel.SendHistory);
            Assert.Equal("ABC", viewModel.SendHistory[0].Content);
            Assert.DoesNotContain("\r", viewModel.SendHistory[0].Content);
            Assert.DoesNotContain("\n", viewModel.SendHistory[0].Content);
        }

        [Fact]
        public void SendCommand_Failure_DoesNotAddHistory()
        {
            var fakeScanner = new FakeSerialPortScanner();
            var fakeService = new FakeSerialPortService(shouldFailSend: true, sendErrorMessage: "Fake failure");
            var viewModel = new TerminalViewModel(fakeScanner, fakeService);
            viewModel.SerialSettings.SelectedPortName = "COM1";
            viewModel.ToggleConnectionCommand.Execute(null);
            viewModel.SelectedSendMode = SendMode.Text;
            viewModel.SendText = "ABC";

            viewModel.SendCommand.Execute(null);

            Assert.Empty(viewModel.SendHistory);
        }

        [Fact]
        public void SendCommand_NotConnected_DoesNotAddHistory()
        {
            var fakeScanner = new FakeSerialPortScanner();
            var fakeService = new FakeSerialPortService();
            var viewModel = new TerminalViewModel(fakeScanner, fakeService);
            viewModel.SelectedSendMode = SendMode.Text;
            viewModel.SendText = "ABC";

            viewModel.SendCommand.Execute(null);

            Assert.Empty(viewModel.SendHistory);
        }

        [Fact]
        public void SendCommand_InvalidHex_DoesNotAddHistory()
        {
            var fakeScanner = new FakeSerialPortScanner();
            var fakeService = new FakeSerialPortService();
            var viewModel = new TerminalViewModel(fakeScanner, fakeService);
            viewModel.SerialSettings.SelectedPortName = "COM1";
            viewModel.ToggleConnectionCommand.Execute(null);
            viewModel.SelectedSendMode = SendMode.Hex;
            viewModel.SendText = "41 4G";

            viewModel.SendCommand.Execute(null);

            Assert.Empty(viewModel.SendHistory);
        }

        [Fact]
        public void SendCommand_EmptyText_DoesNotAddHistory()
        {
            var fakeScanner = new FakeSerialPortScanner();
            var fakeService = new FakeSerialPortService();
            var viewModel = new TerminalViewModel(fakeScanner, fakeService);
            viewModel.SerialSettings.SelectedPortName = "COM1";
            viewModel.ToggleConnectionCommand.Execute(null);
            viewModel.SelectedSendMode = SendMode.Text;
            viewModel.SendText = string.Empty;

            viewModel.SendCommand.Execute(null);

            Assert.Empty(viewModel.SendHistory);
        }

        [Fact]
        public void SendCommand_DuplicateText_RemovesDuplicates()
        {
            var fakeScanner = new FakeSerialPortScanner();
            var fakeService = new FakeSerialPortService();
            var viewModel = new TerminalViewModel(fakeScanner, fakeService);
            viewModel.SerialSettings.SelectedPortName = "COM1";
            viewModel.ToggleConnectionCommand.Execute(null);
            viewModel.SelectedSendMode = SendMode.Text;
            viewModel.SendText = "ABC";
            viewModel.SendCommand.Execute(null);

            viewModel.SendText = "DEF";
            viewModel.SendCommand.Execute(null);

            viewModel.SendText = "ABC";
            viewModel.SendCommand.Execute(null);

            Assert.Equal(2, viewModel.SendHistory.Count);
            Assert.Equal("ABC", viewModel.SendHistory[0].Content);
            Assert.Equal("DEF", viewModel.SendHistory[1].Content);
        }

        [Fact]
        public void SendCommand_DuplicateHex_RemovesDuplicates()
        {
            var fakeScanner = new FakeSerialPortScanner();
            var fakeService = new FakeSerialPortService();
            var viewModel = new TerminalViewModel(fakeScanner, fakeService);
            viewModel.SerialSettings.SelectedPortName = "COM1";
            viewModel.ToggleConnectionCommand.Execute(null);
            viewModel.SelectedSendMode = SendMode.Hex;
            viewModel.SendText = "41 42 43";
            viewModel.SendCommand.Execute(null);

            viewModel.SendText = "44 45 46";
            viewModel.SendCommand.Execute(null);

            viewModel.SendText = "41 42 43";
            viewModel.SendCommand.Execute(null);

            Assert.Equal(2, viewModel.SendHistory.Count);
            Assert.Equal("41 42 43", viewModel.SendHistory[0].Content);
            Assert.Equal("44 45 46", viewModel.SendHistory[1].Content);
        }

        [Fact]
        public void SendCommand_SameContent_DifferentMode_KeepsBoth()
        {
            var fakeScanner = new FakeSerialPortScanner();
            var fakeService = new FakeSerialPortService();
            var viewModel = new TerminalViewModel(fakeScanner, fakeService);
            viewModel.SerialSettings.SelectedPortName = "COM1";
            viewModel.ToggleConnectionCommand.Execute(null);

            viewModel.SelectedSendMode = SendMode.Text;
            viewModel.SendText = "41 42 43";
            viewModel.SendCommand.Execute(null);

            viewModel.SelectedSendMode = SendMode.Hex;
            viewModel.SendCommand.Execute(null);

            Assert.Equal(2, viewModel.SendHistory.Count);
        }

        [Fact]
        public void SendCommand_Duplicate_MovesToLatest()
        {
            var fakeScanner = new FakeSerialPortScanner();
            var fakeService = new FakeSerialPortService();
            var viewModel = new TerminalViewModel(fakeScanner, fakeService);
            viewModel.SerialSettings.SelectedPortName = "COM1";
            viewModel.ToggleConnectionCommand.Execute(null);
            viewModel.SelectedSendMode = SendMode.Text;

            viewModel.SendText = "A";
            viewModel.SendCommand.Execute(null);

            viewModel.SendText = "B";
            viewModel.SendCommand.Execute(null);

            viewModel.SendText = "C";
            viewModel.SendCommand.Execute(null);

            viewModel.SendText = "A";
            viewModel.SendCommand.Execute(null);

            Assert.Equal("A", viewModel.SendHistory[0].Content);
            Assert.Equal("C", viewModel.SendHistory[1].Content);
            Assert.Equal("B", viewModel.SendHistory[2].Content);
        }

        [Fact]
        public void SendCommand_ExceedsMaxCount_DeletesOldest()
        {
            var fakeScanner = new FakeSerialPortScanner();
            var fakeService = new FakeSerialPortService();
            var viewModel = new TerminalViewModel(fakeScanner, fakeService);
            viewModel.SerialSettings.SelectedPortName = "COM1";
            viewModel.ToggleConnectionCommand.Execute(null);
            viewModel.SelectedSendMode = SendMode.Text;

            for (int i = 0; i < 25; i++)
            {
                viewModel.SendText = $"Item{i}";
                viewModel.SendCommand.Execute(null);
            }

            Assert.Equal(20, viewModel.SendHistory.Count);
            Assert.Equal("Item24", viewModel.SendHistory[0].Content);
            Assert.Equal("Item5", viewModel.SendHistory[19].Content);
            Assert.DoesNotContain(viewModel.SendHistory, h => h.Content == "Item0");
        }

        [Fact]
        public void MaxSendHistoryCount_SetTo0_UsesDefault()
        {
            var fakeScanner = new FakeSerialPortScanner();
            var fakeService = new FakeSerialPortService();
            var viewModel = new TerminalViewModel(fakeScanner, fakeService);
            viewModel.SerialSettings.SelectedPortName = "COM1";
            viewModel.ToggleConnectionCommand.Execute(null);
            viewModel.SelectedSendMode = SendMode.Text;

            viewModel.SendText = "A";
            viewModel.SendCommand.Execute(null);

            viewModel.MaxSendHistoryCount = 0;

            Assert.Equal(20, viewModel.MaxSendHistoryCount);
            Assert.Single(viewModel.SendHistory);
        }

        [Fact]
        public void MaxSendHistoryCount_SetToNegative_NoCrash()
        {
            var fakeScanner = new FakeSerialPortScanner();
            var fakeService = new FakeSerialPortService();
            var viewModel = new TerminalViewModel(fakeScanner, fakeService);
            viewModel.SerialSettings.SelectedPortName = "COM1";
            viewModel.ToggleConnectionCommand.Execute(null);
            viewModel.SelectedSendMode = SendMode.Text;

            viewModel.SendText = "A";
            viewModel.SendCommand.Execute(null);

            viewModel.MaxSendHistoryCount = -5;

            Assert.Equal(20, viewModel.MaxSendHistoryCount);
            Assert.Single(viewModel.SendHistory);
        }

        [Fact]
        public void ClearSendHistoryCommand_ClearsHistory()
        {
            var fakeScanner = new FakeSerialPortScanner();
            var fakeService = new FakeSerialPortService();
            var viewModel = new TerminalViewModel(fakeScanner, fakeService);
            viewModel.SerialSettings.SelectedPortName = "COM1";
            viewModel.ToggleConnectionCommand.Execute(null);
            viewModel.SelectedSendMode = SendMode.Text;

            viewModel.SendText = "ABC";
            viewModel.SendCommand.Execute(null);

            Assert.NotEmpty(viewModel.SendHistory);

            viewModel.ClearSendHistoryCommand.Execute(null);

            Assert.Empty(viewModel.SendHistory);
        }

        [Fact]
        public void ClearSendHistoryCommand_DoesNotClearSendText()
        {
            var fakeScanner = new FakeSerialPortScanner();
            var fakeService = new FakeSerialPortService();
            var viewModel = new TerminalViewModel(fakeScanner, fakeService);
            viewModel.SendText = "ABC";

            viewModel.ClearSendHistoryCommand.Execute(null);

            Assert.Equal("ABC", viewModel.SendText);
        }

        [Fact]
        public void ClearSendHistoryCommand_DoesNotClearReceiveDisplay()
        {
            var fakeScanner = new FakeSerialPortScanner();
            var fakeService = new FakeSerialPortService();
            var fakeUiInvoker = new FakeUiThreadInvoker();
            var viewModel = new TerminalViewModel(fakeScanner, fakeService, fakeUiInvoker);
            viewModel.SerialSettings.SelectedPortName = "COM1";
            viewModel.ToggleConnectionCommand.Execute(null);
            byte[] data = Encoding.UTF8.GetBytes("RXDATA");
            fakeService.SimulateDataReceived(data);

            viewModel.SendText = "TXDATA";
            viewModel.SendCommand.Execute(null);

            viewModel.ClearSendHistoryCommand.Execute(null);

            Assert.False(string.IsNullOrEmpty(viewModel.ReceiveDisplay.ReceivedText));
        }

        [Fact]
        public void ClearSendHistoryCommand_DoesNotChangeConnectionState()
        {
            var fakeScanner = new FakeSerialPortScanner();
            var fakeService = new FakeSerialPortService();
            var viewModel = new TerminalViewModel(fakeScanner, fakeService);
            viewModel.SerialSettings.SelectedPortName = "COM1";
            viewModel.ToggleConnectionCommand.Execute(null);

            var originalState = viewModel.ConnectionState;

            viewModel.ClearSendHistoryCommand.Execute(null);

            Assert.Equal(originalState, viewModel.ConnectionState);
        }

        [Fact]
        public void SelectedSendHistoryItem_Set_UpdatesSendText()
        {
            var viewModel = new TerminalViewModel();
            var historyItem = new SendHistoryItem("Hello World", SendMode.Text);

            viewModel.SelectedSendHistoryItem = historyItem;

            Assert.Equal("Hello World", viewModel.SendText);
        }

        [Fact]
        public void SelectedSendHistoryItem_Set_UpdatesSendMode()
        {
            var viewModel = new TerminalViewModel();
            viewModel.SelectedSendMode = SendMode.Text;

            var historyItem = new SendHistoryItem("41 42 43", SendMode.Hex);

            viewModel.SelectedSendHistoryItem = historyItem;

            Assert.Equal(SendMode.Hex, viewModel.SelectedSendMode);
        }

        [Fact]
        public void SelectedSendHistoryItem_TextMode_SetsTextMode()
        {
            var viewModel = new TerminalViewModel();
            viewModel.SelectedSendMode = SendMode.Hex;

            var historyItem = new SendHistoryItem("Test", SendMode.Text);

            viewModel.SelectedSendHistoryItem = historyItem;

            Assert.Equal(SendMode.Text, viewModel.SelectedSendMode);
        }

        [Fact]
        public void SelectedSendHistoryItem_HexMode_SetsHexMode()
        {
            var viewModel = new TerminalViewModel();
            viewModel.SelectedSendMode = SendMode.Text;

            var historyItem = new SendHistoryItem("41 42", SendMode.Hex);

            viewModel.SelectedSendHistoryItem = historyItem;

            Assert.Equal(SendMode.Hex, viewModel.SelectedSendMode);
        }

        [Fact]
        public void SelectedSendHistoryItem_Set_DoesNotAddHistory()
        {
            var fakeScanner = new FakeSerialPortScanner();
            var fakeService = new FakeSerialPortService();
            var viewModel = new TerminalViewModel(fakeScanner, fakeService);
            viewModel.SerialSettings.SelectedPortName = "COM1";
            viewModel.ToggleConnectionCommand.Execute(null);

            viewModel.SendText = "ABC";
            viewModel.SendCommand.Execute(null);

            int initialCount = viewModel.SendHistory.Count;

            viewModel.SelectedSendHistoryItem = viewModel.SendHistory[0];

            Assert.Equal(initialCount, viewModel.SendHistory.Count);
        }

        [Fact]
        public void SelectedSendHistoryItem_Set_DoesNotSend()
        {
            var fakeScanner = new FakeSerialPortScanner();
            var fakeService = new FakeSerialPortService();
            var viewModel = new TerminalViewModel(fakeScanner, fakeService);
            viewModel.SerialSettings.SelectedPortName = "COM1";
            viewModel.ToggleConnectionCommand.Execute(null);

            int initialCount = fakeService.SentData.Count;

            var historyItem = new SendHistoryItem("Test", SendMode.Text);

            viewModel.SelectedSendHistoryItem = historyItem;

            Assert.Equal(initialCount, fakeService.SentData.Count);
        }

        [Fact]
        public void SelectedSendHistoryItem_Set_DoesNotChangeSentBytesCount()
        {
            var viewModel = new TerminalViewModel();
            int initialCount = viewModel.SentBytesCount;

            var historyItem = new SendHistoryItem("Test", SendMode.Text);

            viewModel.SelectedSendHistoryItem = historyItem;

            Assert.Equal(initialCount, viewModel.SentBytesCount);
        }

        [Fact]
        public void SelectedSendHistoryItem_Set_DoesNotClearReceiveDisplay()
        {
            var fakeScanner = new FakeSerialPortScanner();
            var fakeService = new FakeSerialPortService();
            var fakeUiInvoker = new FakeUiThreadInvoker();
            var viewModel = new TerminalViewModel(fakeScanner, fakeService, fakeUiInvoker);
            viewModel.SerialSettings.SelectedPortName = "COM1";
            viewModel.ToggleConnectionCommand.Execute(null);
            byte[] data = Encoding.UTF8.GetBytes("RXDATA");
            fakeService.SimulateDataReceived(data);

            string originalText = viewModel.ReceiveDisplay.ReceivedText;

            var historyItem = new SendHistoryItem("Test", SendMode.Text);

            viewModel.SelectedSendHistoryItem = historyItem;

            Assert.Equal(originalText, viewModel.ReceiveDisplay.ReceivedText);
        }

        [Fact]
        public void ClearSendHistoryCommand_SetsSelectedSendHistoryItemToNull()
        {
            var fakeScanner = new FakeSerialPortScanner();
            var fakeService = new FakeSerialPortService();
            var viewModel = new TerminalViewModel(fakeScanner, fakeService);
            viewModel.SerialSettings.SelectedPortName = "COM1";
            viewModel.ToggleConnectionCommand.Execute(null);
            viewModel.SendText = "ABC";
            viewModel.SendCommand.Execute(null);

            viewModel.SelectedSendHistoryItem = viewModel.SendHistory[0];
            Assert.NotNull(viewModel.SelectedSendHistoryItem);

            viewModel.ClearSendHistoryCommand.Execute(null);

            Assert.Null(viewModel.SelectedSendHistoryItem);
        }

        [Fact]
        public void ClearSendHistoryCommand_DoesNotChangeSendMode()
        {
            var viewModel = new TerminalViewModel();
            viewModel.SelectedSendMode = SendMode.Hex;

            viewModel.ClearSendHistoryCommand.Execute(null);

            Assert.Equal(SendMode.Hex, viewModel.SelectedSendMode);
        }

        [Fact]
        public void LoadSettings_RestoresMaxSendHistoryCount()
        {
            var fakeScanner = new FakeSerialPortScanner();
            var fakeService = new FakeSerialPortService();
            var fakeSettingsService = new FakeAppSettingsService();
            fakeSettingsService.SetSavedSettings(new AppSettings
            {
                MaxSendHistoryCount = 50
            });

            var viewModel = new TerminalViewModel(fakeScanner, fakeService, null, fakeSettingsService);

            Assert.Equal(50, viewModel.MaxSendHistoryCount);
        }

        [Fact]
        public void LoadSettings_RestoresSendHistory()
        {
            var fakeScanner = new FakeSerialPortScanner();
            var fakeService = new FakeSerialPortService();
            var fakeSettingsService = new FakeAppSettingsService();
            var settingsHistory = new List<SendHistoryItem>
            {
                new SendHistoryItem("Hello", SendMode.Text),
                new SendHistoryItem("41 42", SendMode.Hex)
            };
            fakeSettingsService.SetSavedSettings(new AppSettings
            {
                SendHistory = settingsHistory
            });

            var viewModel = new TerminalViewModel(fakeScanner, fakeService, null, fakeSettingsService);

            Assert.Equal(2, viewModel.SendHistory.Count);
            Assert.Equal("Hello", viewModel.SendHistory[0].Content);
            Assert.Equal(SendMode.Text, viewModel.SendHistory[0].SendMode);
            Assert.Equal("41 42", viewModel.SendHistory[1].Content);
            Assert.Equal(SendMode.Hex, viewModel.SendHistory[1].SendMode);
        }

        [Fact]
        public void LoadSettings_PreservesHistoryOrder()
        {
            var fakeScanner = new FakeSerialPortScanner();
            var fakeService = new FakeSerialPortService();
            var fakeSettingsService = new FakeAppSettingsService();
            var settingsHistory = new List<SendHistoryItem>
            {
                new SendHistoryItem("First", SendMode.Text),
                new SendHistoryItem("Second", SendMode.Text),
                new SendHistoryItem("Third", SendMode.Text)
            };
            fakeSettingsService.SetSavedSettings(new AppSettings
            {
                SendHistory = settingsHistory
            });

            var viewModel = new TerminalViewModel(fakeScanner, fakeService, null, fakeSettingsService);

            Assert.Equal("First", viewModel.SendHistory[0].Content);
            Assert.Equal("Second", viewModel.SendHistory[1].Content);
            Assert.Equal("Third", viewModel.SendHistory[2].Content);
        }

        [Fact]
        public void LoadSettings_SelectedSendHistoryItem_IsNull()
        {
            var fakeScanner = new FakeSerialPortScanner();
            var fakeService = new FakeSerialPortService();
            var fakeSettingsService = new FakeAppSettingsService();
            fakeSettingsService.SetSavedSettings(new AppSettings
            {
                SendHistory = new List<SendHistoryItem>
                {
                    new SendHistoryItem("Test", SendMode.Text)
                }
            });

            var viewModel = new TerminalViewModel(fakeScanner, fakeService, null, fakeSettingsService);

            Assert.Null(viewModel.SelectedSendHistoryItem);
        }

        [Fact]
        public void LoadSettings_DoesNotModifySendText()
        {
            var fakeScanner = new FakeSerialPortScanner();
            var fakeService = new FakeSerialPortService();
            var fakeSettingsService = new FakeAppSettingsService();
            fakeSettingsService.SetSavedSettings(new AppSettings
            {
                SendHistory = new List<SendHistoryItem>
                {
                    new SendHistoryItem("FromHistory", SendMode.Text)
                }
            });

            var viewModel = new TerminalViewModel(fakeScanner, fakeService, null, fakeSettingsService);

            Assert.Equal(string.Empty, viewModel.SendText);
        }

        [Fact]
        public void LoadSettings_ExceedsMaxCount_TrimsOldest()
        {
            var fakeScanner = new FakeSerialPortScanner();
            var fakeService = new FakeSerialPortService();
            var fakeSettingsService = new FakeAppSettingsService();
            var settingsHistory = new List<SendHistoryItem>();
            for (int i = 0; i < 25; i++)
            {
                settingsHistory.Add(new SendHistoryItem($"Item{i}", SendMode.Text));
            }
            fakeSettingsService.SetSavedSettings(new AppSettings
            {
                SendHistory = settingsHistory,
                MaxSendHistoryCount = 20
            });

            var viewModel = new TerminalViewModel(fakeScanner, fakeService, null, fakeSettingsService);

            Assert.Equal(20, viewModel.SendHistory.Count);
            Assert.Equal("Item0", viewModel.SendHistory[0].Content);
            Assert.Equal("Item19", viewModel.SendHistory[19].Content);
            Assert.DoesNotContain(viewModel.SendHistory, h => h.Content == "Item20");
        }

        [Fact]
        public void LoadSettings_DuplicateHistory_Deduplicates()
        {
            var fakeScanner = new FakeSerialPortScanner();
            var fakeService = new FakeSerialPortService();
            var fakeSettingsService = new FakeAppSettingsService();
            var settingsHistory = new List<SendHistoryItem>
            {
                new SendHistoryItem("Hello", SendMode.Text),
                new SendHistoryItem("World", SendMode.Text),
                new SendHistoryItem("Hello", SendMode.Text)
            };
            fakeSettingsService.SetSavedSettings(new AppSettings
            {
                SendHistory = settingsHistory
            });

            var viewModel = new TerminalViewModel(fakeScanner, fakeService, null, fakeSettingsService);

            Assert.Equal(2, viewModel.SendHistory.Count);
            Assert.Equal("Hello", viewModel.SendHistory[0].Content);
            Assert.Equal("World", viewModel.SendHistory[1].Content);
        }

        [Fact]
        public void LoadSettings_EmptyContent_IsSkipped()
        {
            var fakeScanner = new FakeSerialPortScanner();
            var fakeService = new FakeSerialPortService();
            var fakeSettingsService = new FakeAppSettingsService();
            var settingsHistory = new List<SendHistoryItem>
            {
                new SendHistoryItem("Valid", SendMode.Text),
                new SendHistoryItem("", SendMode.Text),
                new SendHistoryItem("   ", SendMode.Text),
                new SendHistoryItem("AlsoValid", SendMode.Text)
            };
            fakeSettingsService.SetSavedSettings(new AppSettings
            {
                SendHistory = settingsHistory
            });

            var viewModel = new TerminalViewModel(fakeScanner, fakeService, null, fakeSettingsService);

            Assert.Equal(2, viewModel.SendHistory.Count);
            Assert.Equal("Valid", viewModel.SendHistory[0].Content);
            Assert.Equal("AlsoValid", viewModel.SendHistory[1].Content);
        }

        [Fact]
        public void SaveSettings_SavesMaxSendHistoryCount()
        {
            var fakeScanner = new FakeSerialPortScanner();
            var fakeService = new FakeSerialPortService();
            var fakeSettingsService = new FakeAppSettingsService();
            var viewModel = new TerminalViewModel(fakeScanner, fakeService, null, fakeSettingsService);

            viewModel.MaxSendHistoryCount = 30;
            viewModel.SerialSettings.SelectedPortName = "COM1";
            viewModel.ToggleConnectionCommand.Execute(null);
            viewModel.SendText = "Test";
            viewModel.SendCommand.Execute(null);

            viewModel.SaveSettings();

            Assert.Equal(30, fakeSettingsService.LastSavedSettings!.MaxSendHistoryCount);
        }

        [Fact]
        public void SaveSettings_SavesSendHistory()
        {
            var fakeScanner = new FakeSerialPortScanner();
            var fakeService = new FakeSerialPortService();
            var fakeSettingsService = new FakeAppSettingsService();
            var viewModel = new TerminalViewModel(fakeScanner, fakeService, null, fakeSettingsService);

            viewModel.SerialSettings.SelectedPortName = "COM1";
            viewModel.ToggleConnectionCommand.Execute(null);
            viewModel.SendText = "Hello";
            viewModel.SendCommand.Execute(null);

            viewModel.SendText = "41 42";
            viewModel.SelectedSendMode = SendMode.Hex;
            viewModel.SendCommand.Execute(null);

            viewModel.SaveSettings();

            Assert.NotNull(fakeSettingsService.LastSavedSettings!.SendHistory);
            Assert.Equal(2, fakeSettingsService.LastSavedSettings.SendHistory.Count);
            Assert.Equal("41 42", fakeSettingsService.LastSavedSettings.SendHistory[0].Content);
            Assert.Equal(SendMode.Hex, fakeSettingsService.LastSavedSettings.SendHistory[0].SendMode);
        }

        [Fact]
        public void SaveSettings_DoesNotSaveSelectedSendHistoryItem()
        {
            var fakeScanner = new FakeSerialPortScanner();
            var fakeService = new FakeSerialPortService();
            var fakeSettingsService = new FakeAppSettingsService();
            var viewModel = new TerminalViewModel(fakeScanner, fakeService, null, fakeSettingsService);

            viewModel.SerialSettings.SelectedPortName = "COM1";
            viewModel.ToggleConnectionCommand.Execute(null);
            viewModel.SendText = "Hello";
            viewModel.SendCommand.Execute(null);

            viewModel.SelectedSendHistoryItem = viewModel.SendHistory[0];

            viewModel.SaveSettings();

            Assert.Single(fakeSettingsService.LastSavedSettings!.SendHistory);
        }

        [Fact]
        public void SaveSettings_PreservesHistoryOrder()
        {
            var fakeScanner = new FakeSerialPortScanner();
            var fakeService = new FakeSerialPortService();
            var fakeSettingsService = new FakeAppSettingsService();
            var viewModel = new TerminalViewModel(fakeScanner, fakeService, null, fakeSettingsService);

            viewModel.SerialSettings.SelectedPortName = "COM1";
            viewModel.ToggleConnectionCommand.Execute(null);

            viewModel.SendText = "First";
            viewModel.SendCommand.Execute(null);

            viewModel.SendText = "Second";
            viewModel.SendCommand.Execute(null);

            viewModel.SendText = "Third";
            viewModel.SendCommand.Execute(null);

            viewModel.SaveSettings();

            Assert.Equal("Third", fakeSettingsService.LastSavedSettings!.SendHistory[0].Content);
            Assert.Equal("Second", fakeSettingsService.LastSavedSettings.SendHistory[1].Content);
            Assert.Equal("First", fakeSettingsService.LastSavedSettings.SendHistory[2].Content);
        }

        [Fact]
        public void SaveSettings_AfterClearHistory_IsEmpty()
        {
            var fakeScanner = new FakeSerialPortScanner();
            var fakeService = new FakeSerialPortService();
            var fakeSettingsService = new FakeAppSettingsService();
            var viewModel = new TerminalViewModel(fakeScanner, fakeService, null, fakeSettingsService);

            viewModel.SerialSettings.SelectedPortName = "COM1";
            viewModel.ToggleConnectionCommand.Execute(null);
            viewModel.SendText = "Hello";
            viewModel.SendCommand.Execute(null);

            viewModel.ClearSendHistoryCommand.Execute(null);

            viewModel.SaveSettings();

            Assert.NotNull(fakeSettingsService.LastSavedSettings!.SendHistory);
            Assert.Empty(fakeSettingsService.LastSavedSettings.SendHistory);
        }

        [Fact]
        public void SendCommand_CanExecute_WhenSendTextNotEmpty_AndConnected()
        {
            var fakeScanner = new FakeSerialPortScanner(new List<SerialPortInfo> { SerialPortInfo.Create("COM1") });
            var fakeService = new FakeSerialPortService();
            var viewModel = new TerminalViewModel(fakeScanner, fakeService);

            viewModel.SerialSettings.SelectedPortName = "COM1";
            viewModel.ToggleConnectionCommand.Execute(null);

            viewModel.SendText = "test data";

            Assert.True(viewModel.SendCommand.CanExecute(null));
        }
    }
}
