using Xunit;
using SerialAssistant.App.ViewModels;
using SerialAssistant.Core.Enums;
using SerialAssistant.Core.Models;
using SerialAssistant.Tests.Infrastructure;
using SerialAssistant.Tests.UI;
using System.Text;

namespace SerialAssistant.Tests.ViewModels
{
    /*
     * Tests for MainWindowViewModel
     */
    public class MainWindowViewModelTests
    {
        /*
         * Test default connection state is Disconnected
         */
        [Fact]
        public void DefaultConnectionState_IsDisconnected()
        {
            /* Arrange */
            var viewModel = new MainWindowViewModel();

            /* Act & Assert */
            Assert.Equal(SerialConnectionState.Disconnected, viewModel.ConnectionState);
        }

        /*
         * Test default status message is not empty
         */
        [Fact]
        public void DefaultStatusMessage_IsNotEmpty()
        {
            /* Arrange */
            var viewModel = new MainWindowViewModel();

            /* Act & Assert */
            Assert.False(string.IsNullOrEmpty(viewModel.StatusMessage));
        }

        /*
         * Test RefreshPortsCommand without scanner provides status message
         */
        [Fact]
        public void RefreshPortsCommand_WithoutScanner_ProvidesStatusMessage()
        {
            /* Arrange */
            var viewModel = new MainWindowViewModel();

            /* Act */
            viewModel.RefreshPortsCommand.Execute(null);

            /* Assert */
            Assert.Contains("尚未接入", viewModel.StatusMessage);
        }

        /*
         * Test RefreshPortsCommand with fake scanner returns two ports updates AvailablePorts
         */
        [Fact]
        public void RefreshPortsCommand_WithFakeScanner_ReturnsTwoPorts_UpdatesAvailablePorts()
        {
            /* Arrange */
            var fakeScanner = new FakeSerialPortScanner(new List<SerialPortInfo>
            {
                SerialPortInfo.Create("COM1"),
                SerialPortInfo.Create("COM2")
            });
            var fakeService = new FakeSerialPortService();
            var viewModel = new MainWindowViewModel(fakeScanner, fakeService);

            /* Act */
            viewModel.RefreshPortsCommand.Execute(null);

            /* Assert */
            Assert.Equal(2, viewModel.SerialSettings.AvailablePorts.Count);
        }

        /*
         * Test RefreshPortsCommand with fake scanner returns two ports selects first port
         */
        [Fact]
        public void RefreshPortsCommand_WithFakeScanner_ReturnsTwoPorts_SelectsFirstPort()
        {
            /* Arrange */
            var fakeScanner = new FakeSerialPortScanner(new List<SerialPortInfo>
            {
                SerialPortInfo.Create("COM1"),
                SerialPortInfo.Create("COM2")
            });
            var fakeService = new FakeSerialPortService();
            var viewModel = new MainWindowViewModel(fakeScanner, fakeService);

            /* Act */
            viewModel.RefreshPortsCommand.Execute(null);

            /* Assert */
            Assert.Equal("COM1", viewModel.SerialSettings.SelectedPortName);
        }

        /*
         * Test RefreshPortsCommand with fake scanner returns empty list sets SelectedPortName to null
         */
        [Fact]
        public void RefreshPortsCommand_WithFakeScanner_ReturnsEmptyList_SetsSelectedPortNameToNull()
        {
            /* Arrange */
            var fakeScanner = new FakeSerialPortScanner(new List<SerialPortInfo>());
            var fakeService = new FakeSerialPortService();
            var viewModel = new MainWindowViewModel(fakeScanner, fakeService);

            /* Act */
            viewModel.RefreshPortsCommand.Execute(null);

            /* Assert */
            Assert.Null(viewModel.SerialSettings.SelectedPortName);
        }

        /*
         * Test RefreshPortsCommand success updates status message
         */
        [Fact]
        public void RefreshPortsCommand_Success_UpdatesStatusMessage()
        {
            /* Arrange */
            var fakeScanner = new FakeSerialPortScanner(new List<SerialPortInfo>
            {
                SerialPortInfo.Create("COM1"),
                SerialPortInfo.Create("COM2")
            });
            var fakeService = new FakeSerialPortService();
            var viewModel = new MainWindowViewModel(fakeScanner, fakeService);

            /* Act */
            viewModel.RefreshPortsCommand.Execute(null);

            /* Assert */
            Assert.Contains("刷新", viewModel.StatusMessage);
        }

        /*
         * Test RefreshPortsCommand failure updates status message with error
         */
        [Fact]
        public void RefreshPortsCommand_Failure_UpdatesStatusMessageWithError()
        {
            /* Arrange */
            var fakeScanner = new FakeSerialPortScanner(null, true, "Test error");
            var fakeService = new FakeSerialPortService();
            var viewModel = new MainWindowViewModel(fakeScanner, fakeService);

            /* Act */
            viewModel.RefreshPortsCommand.Execute(null);

            /* Assert */
            Assert.Contains("扫描失败", viewModel.StatusMessage);
            Assert.Contains("Test error", viewModel.StatusMessage);
        }

        /*
         * Test ToggleConnectionCommand without service provides status message
         */
        [Fact]
        public void ToggleConnectionCommand_WithoutService_ProvidesStatusMessage()
        {
            /* Arrange */
            var viewModel = new MainWindowViewModel();

            /* Act */
            viewModel.ToggleConnectionCommand.Execute(null);

            /* Assert */
            Assert.Contains("尚未接入", viewModel.StatusMessage);
        }

        /*
         * Test Open with valid settings changes state to Connected
         */
        [Fact]
        public void ToggleConnectionCommand_WithValidSettings_ChangesStateToConnected()
        {
            /* Arrange */
            var fakeScanner = new FakeSerialPortScanner(new List<SerialPortInfo>
            {
                SerialPortInfo.Create("COM1")
            });
            var fakeService = new FakeSerialPortService();
            var viewModel = new MainWindowViewModel(fakeScanner, fakeService);
            viewModel.SerialSettings.SelectedPortName = "COM1";

            /* Act */
            viewModel.ToggleConnectionCommand.Execute(null);

            /* Assert */
            Assert.Equal(SerialConnectionState.Connected, viewModel.ConnectionState);
        }

        /*
         * Test Open成功后 IsSettingsEnabled 为 false
         */
        [Fact]
        public void Open_Success_IsSettingsEnabled_IsFalse()
        {
            /* Arrange */
            var fakeScanner = new FakeSerialPortScanner(new List<SerialPortInfo>
            {
                SerialPortInfo.Create("COM1")
            });
            var fakeService = new FakeSerialPortService();
            var viewModel = new MainWindowViewModel(fakeScanner, fakeService);
            viewModel.SerialSettings.SelectedPortName = "COM1";

            /* Act */
            viewModel.ToggleConnectionCommand.Execute(null);

            /* Assert */
            Assert.False(viewModel.SerialSettings.IsSettingsEnabled);
        }

        /*
         * Test Open成功时 StatusMessage 包含已打开
         */
        [Fact]
        public void Open_Success_StatusMessage_ContainsOpened()
        {
            /* Arrange */
            var fakeScanner = new FakeSerialPortScanner(new List<SerialPortInfo>
            {
                SerialPortInfo.Create("COM1")
            });
            var fakeService = new FakeSerialPortService();
            var viewModel = new MainWindowViewModel(fakeScanner, fakeService);
            viewModel.SerialSettings.SelectedPortName = "COM1";

            /* Act */
            viewModel.ToggleConnectionCommand.Execute(null);

            /* Assert */
            Assert.Contains("已打开", viewModel.StatusMessage);
        }

        /*
         * Test Open失败时状态不应为 Connected
         */
        [Fact]
        public void Open_Failure_State_ShouldNotBeConnected()
        {
            /* Arrange */
            var fakeScanner = new FakeSerialPortScanner(new List<SerialPortInfo>
            {
                SerialPortInfo.Create("COM1")
            });
            var fakeService = new FakeSerialPortService(true, false, false, "Open failed");
            var viewModel = new MainWindowViewModel(fakeScanner, fakeService);
            viewModel.SerialSettings.SelectedPortName = "COM1";

            /* Act */
            viewModel.ToggleConnectionCommand.Execute(null);

            /* Assert */
            Assert.NotEqual(SerialConnectionState.Connected, viewModel.ConnectionState);
        }

        /*
         * Test Open失败时 IsSettingsEnabled 仍为 true
         */
        [Fact]
        public void Open_Failure_IsSettingsEnabled_IsTrue()
        {
            /* Arrange */
            var fakeScanner = new FakeSerialPortScanner(new List<SerialPortInfo>
            {
                SerialPortInfo.Create("COM1")
            });
            var fakeService = new FakeSerialPortService(true, false, false, "Open failed");
            var viewModel = new MainWindowViewModel(fakeScanner, fakeService);
            viewModel.SerialSettings.SelectedPortName = "COM1";

            /* Act */
            viewModel.ToggleConnectionCommand.Execute(null);

            /* Assert */
            Assert.True(viewModel.SerialSettings.IsSettingsEnabled);
        }

        /*
         * Test Open失败时 StatusMessage 包含失败原因
         */
        [Fact]
        public void Open_Failure_StatusMessage_ContainsError()
        {
            /* Arrange */
            var fakeScanner = new FakeSerialPortScanner(new List<SerialPortInfo>
            {
                SerialPortInfo.Create("COM1")
            });
            var fakeService = new FakeSerialPortService(true, false, false, "Open failed");
            var viewModel = new MainWindowViewModel(fakeScanner, fakeService);
            viewModel.SerialSettings.SelectedPortName = "COM1";

            /* Act */
            viewModel.ToggleConnectionCommand.Execute(null);

            /* Assert */
            Assert.Contains("Open failed", viewModel.StatusMessage);
        }

        /*
         * Test Close成功时状态变为 Disconnected
         */
        [Fact]
        public void Close_Success_State_IsDisconnected()
        {
            /* Arrange */
            var fakeScanner = new FakeSerialPortScanner(new List<SerialPortInfo>
            {
                SerialPortInfo.Create("COM1")
            });
            var fakeService = new FakeSerialPortService();
            var viewModel = new MainWindowViewModel(fakeScanner, fakeService);
            viewModel.SerialSettings.SelectedPortName = "COM1";
            viewModel.ToggleConnectionCommand.Execute(null);

            /* Act */
            viewModel.ToggleConnectionCommand.Execute(null);

            /* Assert */
            Assert.Equal(SerialConnectionState.Disconnected, viewModel.ConnectionState);
        }

        /*
         * Test Close成功时 IsSettingsEnabled 为 true
         */
        [Fact]
        public void Close_Success_IsSettingsEnabled_IsTrue()
        {
            /* Arrange */
            var fakeScanner = new FakeSerialPortScanner(new List<SerialPortInfo>
            {
                SerialPortInfo.Create("COM1")
            });
            var fakeService = new FakeSerialPortService();
            var viewModel = new MainWindowViewModel(fakeScanner, fakeService);
            viewModel.SerialSettings.SelectedPortName = "COM1";
            viewModel.ToggleConnectionCommand.Execute(null);

            /* Act */
            viewModel.ToggleConnectionCommand.Execute(null);

            /* Assert */
            Assert.True(viewModel.SerialSettings.IsSettingsEnabled);
        }

        /*
         * Test Close失败时 StatusMessage 包含失败原因
         */
        [Fact]
        public void Close_Failure_StatusMessage_ContainsError()
        {
            /* Arrange */
            var fakeScanner = new FakeSerialPortScanner(new List<SerialPortInfo>
            {
                SerialPortInfo.Create("COM1")
            });
            var fakeService = new FakeSerialPortService(false, true, false, null, "Close failed");
            var viewModel = new MainWindowViewModel(fakeScanner, fakeService);
            viewModel.SerialSettings.SelectedPortName = "COM1";
            viewModel.ToggleConnectionCommand.Execute(null);

            /* Act */
            viewModel.ToggleConnectionCommand.Execute(null);

            /* Assert */
            Assert.Contains("关闭串口失败", viewModel.StatusMessage);
        }

        /*
         * Test SendCommand without connection provides status message
         */
        [Fact]
        public void SendCommand_WithoutConnection_ProvidesStatusMessage()
        {
            /* Arrange */
            var viewModel = new MainWindowViewModel();
            viewModel.SendText = "test data";

            /* Act */
            viewModel.SendCommand.Execute(null);

            /* Assert */
            Assert.Contains("尚未接入", viewModel.StatusMessage);
        }

        /*
         * Test ClearReceiveCommand clears receive area
         */
        [Fact]
        public void ClearReceiveCommand_ClearsReceiveArea()
        {
            /* Arrange */
            var fakeScanner = new FakeSerialPortScanner();
            var fakeService = new FakeSerialPortService();
            var fakeUiInvoker = new FakeUiThreadInvoker();
            var viewModel = new MainWindowViewModel(fakeScanner, fakeService, fakeUiInvoker);
            byte[] data = System.Text.Encoding.UTF8.GetBytes("some data");
            viewModel.ReceiveDisplay.AddReceivedData(data);

            /* Act */
            viewModel.ClearReceiveCommand.Execute(null);

            /* Assert */
            Assert.Equal(string.Empty, viewModel.ReceiveDisplay.ReceivedText);
        }

        /*
         * Test ClearReceiveCommand clears receive count
         */
        [Fact]
        public void ClearReceiveCommand_ClearsReceiveCount()
        {
            /* Arrange */
            var fakeScanner = new FakeSerialPortScanner();
            var fakeService = new FakeSerialPortService();
            var fakeUiInvoker = new FakeUiThreadInvoker();
            var viewModel = new MainWindowViewModel(fakeScanner, fakeService, fakeUiInvoker);
            byte[] data = System.Text.Encoding.UTF8.GetBytes("some data");
            viewModel.ReceiveDisplay.AddReceivedData(data);

            /* Act */
            viewModel.ClearReceiveCommand.Execute(null);

            /* Assert */
            Assert.Equal(0, viewModel.ReceiveDisplay.ReceivedBytesCount);
        }

        /*
         * Test ViewModel does not depend on real serial port device
         */
        [Fact]
        public void ViewModel_DoesNotDependOnRealSerialPort()
        {
            /* Arrange */
            var viewModel = new MainWindowViewModel();

            /* Act & Assert */
            Assert.NotNull(viewModel.SerialSettings);
            Assert.NotNull(viewModel.ReceiveDisplay);
        }

        /*
         * Test default SendText is empty
         */
        [Fact]
        public void DefaultSendText_IsEmpty()
        {
            /* Arrange */
            var viewModel = new MainWindowViewModel();

            /* Act & Assert */
            Assert.Equal(string.Empty, viewModel.SendText);
        }

        /*
         * Test default SentBytesCount is 0
         */
        [Fact]
        public void DefaultSentBytesCount_Is0()
        {
            /* Arrange */
            var viewModel = new MainWindowViewModel();

            /* Act & Assert */
            Assert.Equal(0, viewModel.SentBytesCount);
        }

        /*
         * Test SendModes contains expected modes
         */
        [Fact]
        public void SendModes_ContainsExpectedModes()
        {
            /* Arrange */
            var viewModel = new MainWindowViewModel();

            /* Act & Assert */
            Assert.Contains(SendMode.Text, viewModel.SendModes);
            Assert.Contains(SendMode.Hex, viewModel.SendModes);
        }

        /*
         * Test default SelectedSendMode is Text
         */
        [Fact]
        public void DefaultSelectedSendMode_IsText()
        {
            /* Arrange */
            var viewModel = new MainWindowViewModel();

            /* Act & Assert */
            Assert.Equal(SendMode.Text, viewModel.SelectedSendMode);
        }

        /*
         * Test RefreshPortsCommand can execute
         */
        [Fact]
        public void RefreshPortsCommand_CanExecute()
        {
            /* Arrange */
            var viewModel = new MainWindowViewModel();

            /* Act & Assert */
            Assert.True(viewModel.RefreshPortsCommand.CanExecute(null));
        }

        /*
         * Test ToggleConnectionCommand can execute when service exists
         */
        [Fact]
        public void ToggleConnectionCommand_CanExecute_WhenServiceExists()
        {
            /* Arrange */
            var fakeScanner = new FakeSerialPortScanner();
            var fakeService = new FakeSerialPortService();
            var viewModel = new MainWindowViewModel(fakeScanner, fakeService);

            /* Act & Assert */
            Assert.True(viewModel.ToggleConnectionCommand.CanExecute(null));
        }

        /*
         * Test SendCommand cannot execute when SendText is empty
         */
        [Fact]
        public void SendCommand_CannotExecute_WhenSendTextEmpty()
        {
            /* Arrange */
            var viewModel = new MainWindowViewModel();
            viewModel.SendText = string.Empty;

            /* Act & Assert */
            Assert.False(viewModel.SendCommand.CanExecute(null));
        }

        /*
         * Test SendCommand can execute when SendText is not empty and connected
         */
        [Fact]
        public void SendCommand_CanExecute_WhenSendTextNotEmpty_AndConnected()
        {
            /* Arrange */
            var fakeScanner = new FakeSerialPortScanner(new List<SerialPortInfo> { SerialPortInfo.Create("COM1") });
            var fakeService = new FakeSerialPortService();
            var viewModel = new MainWindowViewModel(fakeScanner, fakeService);

            /* Connect first */
            viewModel.SerialSettings.SelectedPortName = "COM1";
            viewModel.ToggleConnectionCommand.Execute(null);

            viewModel.SendText = "test data";

            /* Act & Assert */
            Assert.True(viewModel.SendCommand.CanExecute(null));
        }

        /*
         * Test SendCommand cannot execute when not connected, even if SendText is not empty
         */
        [Fact]
        public void SendCommand_CannotExecute_WhenNotConnected_EvenIfSendTextNotEmpty()
        {
            /* Arrange */
            var fakeScanner = new FakeSerialPortScanner();
            var fakeService = new FakeSerialPortService();
            var viewModel = new MainWindowViewModel(fakeScanner, fakeService);
            viewModel.SendText = "test data";

            /* Act & Assert */
            Assert.False(viewModel.SendCommand.CanExecute(null));
        }

        /*
         * Test ClearReceiveCommand can execute
         */
        [Fact]
        public void ClearReceiveCommand_CanExecute()
        {
            /* Arrange */
            var viewModel = new MainWindowViewModel();

            /* Act & Assert */
            Assert.True(viewModel.ClearReceiveCommand.CanExecute(null));
        }

        /*
         * Test IsHexDisplay binds to ReceiveDisplay.IsHexDisplay
         */
        [Fact]
        public void IsHexDisplay_BindsToReceiveDisplayIsHexDisplay()
        {
            /* Arrange */
            var viewModel = new MainWindowViewModel();

            /* Act */
            viewModel.IsHexDisplay = true;

            /* Assert */
            Assert.True(viewModel.ReceiveDisplay.IsHexDisplay);
        }

        /*
         * Test 参数非法时 ToggleConnectionCommand 不调用 Open
         */
        [Fact]
        public void ToggleConnectionCommand_WithInvalidSettings_DoesNotCallOpen()
        {
            /* Arrange */
            var fakeScanner = new FakeSerialPortScanner(new List<SerialPortInfo>());
            var fakeService = new FakeSerialPortService();
            var viewModel = new MainWindowViewModel(fakeScanner, fakeService);
            viewModel.SerialSettings.SelectedPortName = null;

            /* Act */
            viewModel.ToggleConnectionCommand.Execute(null);

            /* Assert */
            Assert.Equal(SerialConnectionState.Disconnected, viewModel.ConnectionState);
            Assert.Contains("参数无效", viewModel.StatusMessage);
        }

        /*
         * Test 未连接时 SendCommand 不发送
         */
        [Fact]
        public void SendCommand_WhenNotConnected_DoesNotSend()
        {
            /* Arrange */
            var fakeScanner = new FakeSerialPortScanner();
            var fakeService = new FakeSerialPortService();
            var viewModel = new MainWindowViewModel(fakeScanner, fakeService);
            viewModel.SendText = "Hello";

            /* Act */
            viewModel.SendCommand.Execute(null);

            /* Assert */
            Assert.Empty(fakeService.SentData);
            Assert.Contains("串口未打开", viewModel.StatusMessage);
        }

        /*
         * Test 文本模式发送正确数据
         */
        [Fact]
        public void SendCommand_TextMode_SendsCorrectData()
        {
            /* Arrange */
            var fakeScanner = new FakeSerialPortScanner();
            var fakeService = new FakeSerialPortService();
            var viewModel = new MainWindowViewModel(fakeScanner, fakeService);

            /* Open port first */
            viewModel.SerialSettings.SelectedPortName = "COM1";
            viewModel.ToggleConnectionCommand.Execute(null);

            viewModel.SelectedSendMode = SendMode.Text;
            viewModel.SendText = "ABC";

            /* Act */
            viewModel.SendCommand.Execute(null);

            /* Assert */
            Assert.Single(fakeService.SentData);
            Assert.Equal(Encoding.UTF8.GetBytes("ABC"), fakeService.SentData[0]);
            Assert.Equal(3, viewModel.SentBytesCount);
            Assert.Contains("已发送", viewModel.StatusMessage);
        }

        /*
         * Test HEX 模式发送正确数据（带空格）
         */
        [Fact]
        public void SendCommand_HexMode_WithSpaces_SendsCorrectData()
        {
            /* Arrange */
            var fakeScanner = new FakeSerialPortScanner();
            var fakeService = new FakeSerialPortService();
            var viewModel = new MainWindowViewModel(fakeScanner, fakeService);

            /* Open port first */
            viewModel.SerialSettings.SelectedPortName = "COM1";
            viewModel.ToggleConnectionCommand.Execute(null);

            viewModel.SelectedSendMode = SendMode.Hex;
            viewModel.SendText = "41 42 43";

            /* Act */
            viewModel.SendCommand.Execute(null);

            /* Assert */
            Assert.Single(fakeService.SentData);
            Assert.Equal(new byte[] { 0x41, 0x42, 0x43 }, fakeService.SentData[0]);
            Assert.Equal(3, viewModel.SentBytesCount);
            Assert.Contains("已发送", viewModel.StatusMessage);
        }

        /*
         * Test HEX 模式发送正确数据（不带空格）
         */
        [Fact]
        public void SendCommand_HexMode_WithoutSpaces_SendsCorrectData()
        {
            /* Arrange */
            var fakeScanner = new FakeSerialPortScanner();
            var fakeService = new FakeSerialPortService();
            var viewModel = new MainWindowViewModel(fakeScanner, fakeService);

            /* Open port first */
            viewModel.SerialSettings.SelectedPortName = "COM1";
            viewModel.ToggleConnectionCommand.Execute(null);

            viewModel.SelectedSendMode = SendMode.Hex;
            viewModel.SendText = "414243";

            /* Act */
            viewModel.SendCommand.Execute(null);

            /* Assert */
            Assert.Single(fakeService.SentData);
            Assert.Equal(new byte[] { 0x41, 0x42, 0x43 }, fakeService.SentData[0]);
            Assert.Equal(3, viewModel.SentBytesCount);
            Assert.Contains("已发送", viewModel.StatusMessage);
        }

        /*
         * Test 非法 HEX 不发送
         */
        [Fact]
        public void SendCommand_InvalidHex_DoesNotSend()
        {
            /* Arrange */
            var fakeScanner = new FakeSerialPortScanner();
            var fakeService = new FakeSerialPortService();
            var viewModel = new MainWindowViewModel(fakeScanner, fakeService);

            /* Open port first */
            viewModel.SerialSettings.SelectedPortName = "COM1";
            viewModel.ToggleConnectionCommand.Execute(null);

            viewModel.SelectedSendMode = SendMode.Hex;
            viewModel.SendText = "41 4G";

            /* Act */
            viewModel.SendCommand.Execute(null);

            /* Assert */
            Assert.Empty(fakeService.SentData);
            Assert.Contains("HEX 格式错误", viewModel.StatusMessage);
        }

        /*
         * Test 奇数长度 HEX 不发送
         */
        [Fact]
        public void SendCommand_OddLengthHex_DoesNotSend()
        {
            /* Arrange */
            var fakeScanner = new FakeSerialPortScanner();
            var fakeService = new FakeSerialPortService();
            var viewModel = new MainWindowViewModel(fakeScanner, fakeService);

            /* Open port first */
            viewModel.SerialSettings.SelectedPortName = "COM1";
            viewModel.ToggleConnectionCommand.Execute(null);

            viewModel.SelectedSendMode = SendMode.Hex;
            viewModel.SendText = "414";

            /* Act */
            viewModel.SendCommand.Execute(null);

            /* Assert */
            Assert.Empty(fakeService.SentData);
            Assert.Contains("HEX 格式错误", viewModel.StatusMessage);
        }

        /*
         * Test 发送失败时状态更新正确
         */
        [Fact]
        public void SendCommand_Failure_UpdatesStatusMessage()
        {
            /* Arrange */
            var fakeScanner = new FakeSerialPortScanner();
            var fakeService = new FakeSerialPortService(shouldFailSend: true, sendErrorMessage: "Fake send failure");
            var viewModel = new MainWindowViewModel(fakeScanner, fakeService);

            /* Open port first */
            viewModel.SerialSettings.SelectedPortName = "COM1";
            viewModel.ToggleConnectionCommand.Execute(null);

            viewModel.SelectedSendMode = SendMode.Text;
            viewModel.SendText = "Hello";

            /* Act */
            viewModel.SendCommand.Execute(null);

            /* Assert */
            Assert.Empty(fakeService.SentData);
            Assert.Contains("发送失败", viewModel.StatusMessage);
            Assert.Contains("Fake send failure", viewModel.StatusMessage);
        }

        /*
         * Test 空内容不发送
         */
        [Fact]
        public void SendCommand_EmptyText_DoesNotSend()
        {
            /* Arrange */
            var fakeScanner = new FakeSerialPortScanner();
            var fakeService = new FakeSerialPortService();
            var viewModel = new MainWindowViewModel(fakeScanner, fakeService);

            /* Open port first */
            viewModel.SerialSettings.SelectedPortName = "COM1";
            viewModel.ToggleConnectionCommand.Execute(null);

            viewModel.SendText = string.Empty;

            /* Act */
            viewModel.SendCommand.Execute(null);

            /* Assert */
            Assert.Empty(fakeService.SentData);
            Assert.Contains("发送内容不能为空", viewModel.StatusMessage);
        }

        /*
         * Test 接收事件触发后 ReceivedBytesCount 增加
         */
        [Fact]
        public void DataReceived_UpdatesReceivedBytesCount()
        {
            /* Arrange */
            var fakeScanner = new FakeSerialPortScanner();
            var fakeService = new FakeSerialPortService();
            var fakeUiInvoker = new FakeUiThreadInvoker();
            var viewModel = new MainWindowViewModel(fakeScanner, fakeService, fakeUiInvoker);

            /* Open port first */
            viewModel.SerialSettings.SelectedPortName = "COM1";
            viewModel.ToggleConnectionCommand.Execute(null);

            byte[] data = new byte[] { 0x41, 0x42, 0x43 };

            /* Act */
            fakeService.SimulateDataReceived(data);

            /* Assert */
            Assert.Equal(3, viewModel.ReceiveDisplay.ReceivedBytesCount);
        }

        /*
         * Test 接收事件触发后 ReceivedText 更新
         */
        [Fact]
        public void DataReceived_UpdatesReceivedText()
        {
            /* Arrange */
            var fakeScanner = new FakeSerialPortScanner();
            var fakeService = new FakeSerialPortService();
            var fakeUiInvoker = new FakeUiThreadInvoker();
            var viewModel = new MainWindowViewModel(fakeScanner, fakeService, fakeUiInvoker);

            /* Open port first */
            viewModel.SerialSettings.SelectedPortName = "COM1";
            viewModel.ToggleConnectionCommand.Execute(null);

            viewModel.IsHexDisplay = false;
            byte[] data = System.Text.Encoding.UTF8.GetBytes("ABC");

            /* Act */
            fakeService.SimulateDataReceived(data);

            /* Assert */
            Assert.Equal("ABC", viewModel.ReceiveDisplay.ReceivedText);
        }

        /*
         * Test 文本模式接收 ABC 显示 ABC
         */
        [Fact]
        public void DataReceived_TextMode_DisplaysABC()
        {
            /* Arrange */
            var fakeScanner = new FakeSerialPortScanner();
            var fakeService = new FakeSerialPortService();
            var fakeUiInvoker = new FakeUiThreadInvoker();
            var viewModel = new MainWindowViewModel(fakeScanner, fakeService, fakeUiInvoker);

            /* Open port first */
            viewModel.SerialSettings.SelectedPortName = "COM1";
            viewModel.ToggleConnectionCommand.Execute(null);

            viewModel.IsHexDisplay = false;
            byte[] data = System.Text.Encoding.UTF8.GetBytes("ABC");

            /* Act */
            fakeService.SimulateDataReceived(data);

            /* Assert */
            Assert.Equal("ABC", viewModel.ReceiveDisplay.ReceivedText);
        }

        /*
         * Test HEX 模式接收 ABC 显示 41 42 43
         */
        [Fact]
        public void DataReceived_HexMode_Displays414243()
        {
            /* Arrange */
            var fakeScanner = new FakeSerialPortScanner();
            var fakeService = new FakeSerialPortService();
            var fakeUiInvoker = new FakeUiThreadInvoker();
            var viewModel = new MainWindowViewModel(fakeScanner, fakeService, fakeUiInvoker);

            /* Open port first */
            viewModel.SerialSettings.SelectedPortName = "COM1";
            viewModel.ToggleConnectionCommand.Execute(null);

            viewModel.IsHexDisplay = true;
            byte[] data = new byte[] { 0x41, 0x42, 0x43 };

            /* Act */
            fakeService.SimulateDataReceived(data);

            /* Assert */
            Assert.Contains("41 42 43", viewModel.ReceiveDisplay.ReceivedText);
        }

        /*
         * Test 多次接收后计数累计
         */
        [Fact]
        public void DataReceived_Multiple_CountAccumulates()
        {
            /* Arrange */
            var fakeScanner = new FakeSerialPortScanner();
            var fakeService = new FakeSerialPortService();
            var fakeUiInvoker = new FakeUiThreadInvoker();
            var viewModel = new MainWindowViewModel(fakeScanner, fakeService, fakeUiInvoker);

            /* Open port first */
            viewModel.SerialSettings.SelectedPortName = "COM1";
            viewModel.ToggleConnectionCommand.Execute(null);

            byte[] data1 = new byte[] { 0x41, 0x42 };
            byte[] data2 = new byte[] { 0x43, 0x44 };

            /* Act */
            fakeService.SimulateDataReceived(data1);
            fakeService.SimulateDataReceived(data2);

            /* Assert */
            Assert.Equal(4, viewModel.ReceiveDisplay.ReceivedBytesCount);
        }

        /*
         * Test ClearReceiveCommand 清空接收文本
         */
        [Fact]
        public void ClearReceiveCommand_ClearsReceivedText()
        {
            /* Arrange */
            var fakeScanner = new FakeSerialPortScanner();
            var fakeService = new FakeSerialPortService();
            var fakeUiInvoker = new FakeUiThreadInvoker();
            var viewModel = new MainWindowViewModel(fakeScanner, fakeService, fakeUiInvoker);

            /* Open port and receive data */
            viewModel.SerialSettings.SelectedPortName = "COM1";
            viewModel.ToggleConnectionCommand.Execute(null);
            byte[] data = System.Text.Encoding.UTF8.GetBytes("ABC");
            fakeService.SimulateDataReceived(data);

            /* Act */
            viewModel.ClearReceiveCommand.Execute(null);

            /* Assert */
            Assert.Equal(string.Empty, viewModel.ReceiveDisplay.ReceivedText);
            Assert.Contains("接收区已清空", viewModel.StatusMessage);
        }

        /*
         * Test ClearReceiveCommand 清空接收计数
         */
        [Fact]
        public void ClearReceiveCommand_ClearsReceivedCount()
        {
            /* Arrange */
            var fakeScanner = new FakeSerialPortScanner();
            var fakeService = new FakeSerialPortService();
            var fakeUiInvoker = new FakeUiThreadInvoker();
            var viewModel = new MainWindowViewModel(fakeScanner, fakeService, fakeUiInvoker);

            /* Open port and receive data */
            viewModel.SerialSettings.SelectedPortName = "COM1";
            viewModel.ToggleConnectionCommand.Execute(null);
            byte[] data = System.Text.Encoding.UTF8.GetBytes("ABC");
            fakeService.SimulateDataReceived(data);

            /* Act */
            viewModel.ClearReceiveCommand.Execute(null);

            /* Assert */
            Assert.Equal(0, viewModel.ReceiveDisplay.ReceivedBytesCount);
        }

        /*
         * Test 接收后 StatusMessage 更新
         */
        [Fact]
        public void DataReceived_UpdatesStatusMessage()
        {
            /* Arrange */
            var fakeScanner = new FakeSerialPortScanner();
            var fakeService = new FakeSerialPortService();
            var fakeUiInvoker = new FakeUiThreadInvoker();
            var viewModel = new MainWindowViewModel(fakeScanner, fakeService, fakeUiInvoker);

            /* Open port first */
            viewModel.SerialSettings.SelectedPortName = "COM1";
            viewModel.ToggleConnectionCommand.Execute(null);

            byte[] data = new byte[] { 0x41, 0x42, 0x43 };

            /* Act */
            fakeService.SimulateDataReceived(data);

            /* Assert */
            Assert.Contains("已接收", viewModel.StatusMessage);
        }

        /*
         * Test 错误发生后 StatusMessage 更新
         */
        [Fact]
        public void ErrorOccurred_UpdatesStatusMessage()
        {
            /* Arrange */
            var fakeScanner = new FakeSerialPortScanner();
            var fakeService = new FakeSerialPortService();
            var fakeUiInvoker = new FakeUiThreadInvoker();
            var viewModel = new MainWindowViewModel(fakeScanner, fakeService, fakeUiInvoker);

            /* Open port first */
            viewModel.SerialSettings.SelectedPortName = "COM1";
            viewModel.ToggleConnectionCommand.Execute(null);

            Exception ex = new Exception("Test receive error");

            /* Act */
            fakeService.SimulateErrorOccurred(ex);

            /* Assert */
            Assert.Contains("接收串口数据失败", viewModel.StatusMessage);
            Assert.Contains("Test receive error", viewModel.StatusMessage);
        }

        /*
         * Test 加载配置后波特率恢复
         */
        [Fact]
        public void LoadSettings_RestoresBaudRate()
        {
            /* Arrange */
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

            /* Act */
            var viewModel = new MainWindowViewModel(fakeScanner, fakeService, fakeUiInvoker, fakeSettingsService);

            /* Assert */
            Assert.Equal(115200, viewModel.SerialSettings.SelectedBaudRate);
            Assert.Equal(7, viewModel.SerialSettings.SelectedDataBits);
            Assert.Equal("Odd", viewModel.SerialSettings.SelectedParity);
            Assert.Equal("Two", viewModel.SerialSettings.SelectedStopBits);
            Assert.Equal(SendMode.Hex, viewModel.SelectedSendMode);
            Assert.True(viewModel.ReceiveDisplay.IsHexDisplay);
        }

        /*
         * Test 保存配置时调用设置服务
         */
        [Fact]
        public void SaveSettings_CallsSettingsService()
        {
            /* Arrange */
            var fakeScanner = new FakeSerialPortScanner();
            var fakeService = new FakeSerialPortService();
            var fakeUiInvoker = new FakeUiThreadInvoker();
            var fakeSettingsService = new FakeAppSettingsService();
            var viewModel = new MainWindowViewModel(fakeScanner, fakeService, fakeUiInvoker, fakeSettingsService);

            /* Change settings */
            viewModel.SerialSettings.SelectedPortName = "COM3";
            viewModel.SerialSettings.SelectedBaudRate = 9600;
            viewModel.SelectedSendMode = SendMode.Text;
            viewModel.ReceiveDisplay.IsHexDisplay = false;

            /* Act */
            var result = viewModel.SaveSettings();

            /* Assert */
            Assert.True(result.IsSuccess);
            Assert.Equal("COM3", fakeSettingsService.GetSavedSettings().LastPortName);
            Assert.Equal(9600, fakeSettingsService.GetSavedSettings().BaudRate);
            Assert.Equal(SendMode.Text, fakeSettingsService.GetSavedSettings().SendMode);
            Assert.Equal(DisplayMode.Text, fakeSettingsService.GetSavedSettings().DisplayMode);
        }

        /*
         * Test 保存失败时状态更新正确
         */
        [Fact]
        public void SaveSettings_Failure_ReturnsFailure()
        {
            /* Arrange */
            var fakeScanner = new FakeSerialPortScanner();
            var fakeService = new FakeSerialPortService();
            var fakeUiInvoker = new FakeUiThreadInvoker();
            var fakeSettingsService = new FakeAppSettingsService();
            fakeSettingsService.SetFailSave(true);
            var viewModel = new MainWindowViewModel(fakeScanner, fakeService, fakeUiInvoker, fakeSettingsService);

            /* Act */
            var result = viewModel.SaveSettings();

            /* Assert */
            Assert.False(result.IsSuccess);
            Assert.Equal("Save failed.", result.ErrorMessage);
        }

        /*
         * Test LastPortName 存在于刷新结果时自动选中
         */
        [Fact]
        public void RefreshPorts_LastPortNameExists_SelectsIt()
        {
            /* Arrange */
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
            var viewModel = new MainWindowViewModel(fakeScanner, fakeService, fakeUiInvoker, fakeSettingsService);

            /* Act */
            viewModel.RefreshPortsCommand.Execute(null);

            /* Assert */
            Assert.Equal("COM2", viewModel.SerialSettings.SelectedPortName);
        }

        /*
         * Test LastPortName 不存在于刷新结果时选择第一个
         */
        [Fact]
        public void RefreshPorts_LastPortNameNotExists_SelectsFirst()
        {
            /* Arrange */
            var fakeScanner = new FakeSerialPortScanner(new List<SerialPortInfo>
            {
                SerialPortInfo.Create("COM1"),
                SerialPortInfo.Create("COM2")
            });
            var fakeService = new FakeSerialPortService();
            var fakeUiInvoker = new FakeUiThreadInvoker();
            var fakeSettingsService = new FakeAppSettingsService();
            var customSettings = new AppSettings { LastPortName = "COM999" };
            fakeSettingsService.Save(customSettings);
            var viewModel = new MainWindowViewModel(fakeScanner, fakeService, fakeUiInvoker, fakeSettingsService);

            /* Act */
            viewModel.RefreshPortsCommand.Execute(null);

            /* Assert */
            Assert.Equal("COM1", viewModel.SerialSettings.SelectedPortName);
        }

        /*
         * Test SendLineEndings 包含预期的选项
         */
        [Fact]
        public void SendLineEndings_ContainsExpectedOptions()
        {
            /* Arrange */
            var viewModel = new MainWindowViewModel();

            /* Act & Assert */
            Assert.Contains(SendLineEnding.None, viewModel.SendLineEndings);
            Assert.Contains(SendLineEnding.CR, viewModel.SendLineEndings);
            Assert.Contains(SendLineEnding.LF, viewModel.SendLineEndings);
            Assert.Contains(SendLineEnding.CRLF, viewModel.SendLineEndings);
        }

        /*
         * Test 默认 SelectedSendLineEnding 是 None
         */
        [Fact]
        public void DefaultSelectedSendLineEnding_IsNone()
        {
            /* Arrange */
            var viewModel = new MainWindowViewModel();

            /* Act & Assert */
            Assert.Equal(SendLineEnding.None, viewModel.SelectedSendLineEnding);
        }

        /*
         * Test 文本模式 + None 发送正确数据
         */
        [Fact]
        public void SendCommand_TextMode_None_SendsCorrectData()
        {
            /* Arrange */
            var fakeScanner = new FakeSerialPortScanner();
            var fakeService = new FakeSerialPortService();
            var viewModel = new MainWindowViewModel(fakeScanner, fakeService);

            viewModel.SerialSettings.SelectedPortName = "COM1";
            viewModel.ToggleConnectionCommand.Execute(null);
            viewModel.SelectedSendMode = SendMode.Text;
            viewModel.SelectedSendLineEnding = SendLineEnding.None;
            viewModel.SendText = "ABC";

            /* Act */
            viewModel.SendCommand.Execute(null);

            /* Assert */
            Assert.Single(fakeService.SentData);
            Assert.Equal(Encoding.UTF8.GetBytes("ABC"), fakeService.SentData[0]);
            Assert.Equal(3, viewModel.SentBytesCount);
        }

        /*
         * Test 文本模式 + CR 发送正确数据
         */
        [Fact]
        public void SendCommand_TextMode_CR_SendsCorrectData()
        {
            /* Arrange */
            var fakeScanner = new FakeSerialPortScanner();
            var fakeService = new FakeSerialPortService();
            var viewModel = new MainWindowViewModel(fakeScanner, fakeService);

            viewModel.SerialSettings.SelectedPortName = "COM1";
            viewModel.ToggleConnectionCommand.Execute(null);
            viewModel.SelectedSendMode = SendMode.Text;
            viewModel.SelectedSendLineEnding = SendLineEnding.CR;
            viewModel.SendText = "ABC";

            /* Act */
            viewModel.SendCommand.Execute(null);

            /* Assert */
            Assert.Single(fakeService.SentData);
            Assert.Equal(Encoding.UTF8.GetBytes("ABC\r"), fakeService.SentData[0]);
            Assert.Equal(4, viewModel.SentBytesCount);
        }

        /*
         * Test 文本模式 + LF 发送正确数据
         */
        [Fact]
        public void SendCommand_TextMode_LF_SendsCorrectData()
        {
            /* Arrange */
            var fakeScanner = new FakeSerialPortScanner();
            var fakeService = new FakeSerialPortService();
            var viewModel = new MainWindowViewModel(fakeScanner, fakeService);

            viewModel.SerialSettings.SelectedPortName = "COM1";
            viewModel.ToggleConnectionCommand.Execute(null);
            viewModel.SelectedSendMode = SendMode.Text;
            viewModel.SelectedSendLineEnding = SendLineEnding.LF;
            viewModel.SendText = "ABC";

            /* Act */
            viewModel.SendCommand.Execute(null);

            /* Assert */
            Assert.Single(fakeService.SentData);
            Assert.Equal(Encoding.UTF8.GetBytes("ABC\n"), fakeService.SentData[0]);
            Assert.Equal(4, viewModel.SentBytesCount);
        }

        /*
         * Test 文本模式 + CRLF 发送正确数据
         */
        [Fact]
        public void SendCommand_TextMode_CRLF_SendsCorrectData()
        {
            /* Arrange */
            var fakeScanner = new FakeSerialPortScanner();
            var fakeService = new FakeSerialPortService();
            var viewModel = new MainWindowViewModel(fakeScanner, fakeService);

            viewModel.SerialSettings.SelectedPortName = "COM1";
            viewModel.ToggleConnectionCommand.Execute(null);
            viewModel.SelectedSendMode = SendMode.Text;
            viewModel.SelectedSendLineEnding = SendLineEnding.CRLF;
            viewModel.SendText = "ABC";

            /* Act */
            viewModel.SendCommand.Execute(null);

            /* Assert */
            Assert.Single(fakeService.SentData);
            Assert.Equal(Encoding.UTF8.GetBytes("ABC\r\n"), fakeService.SentData[0]);
            Assert.Equal(5, viewModel.SentBytesCount);
        }

        /*
         * Test HEX 模式即使选中 CRLF 也不追加结尾
         */
        [Fact]
        public void SendCommand_HexMode_DoesNotAppendLineEnding()
        {
            /* Arrange */
            var fakeScanner = new FakeSerialPortScanner();
            var fakeService = new FakeSerialPortService();
            var viewModel = new MainWindowViewModel(fakeScanner, fakeService);

            viewModel.SerialSettings.SelectedPortName = "COM1";
            viewModel.ToggleConnectionCommand.Execute(null);
            viewModel.SelectedSendMode = SendMode.Hex;
            viewModel.SelectedSendLineEnding = SendLineEnding.CRLF;
            viewModel.SendText = "41 42 43";

            /* Act */
            viewModel.SendCommand.Execute(null);

            /* Assert */
            Assert.Single(fakeService.SentData);
            Assert.Equal(new byte[] { 0x41, 0x42, 0x43 }, fakeService.SentData[0]);
            Assert.Equal(3, viewModel.SentBytesCount);
        }

        /*
         * Test 加载配置时恢复 SendLineEnding
         */
        [Fact]
        public void LoadSettings_RestoresSendLineEnding()
        {
            /* Arrange */
            var fakeScanner = new FakeSerialPortScanner();
            var fakeService = new FakeSerialPortService();
            var fakeUiInvoker = new FakeUiThreadInvoker();
            var fakeSettingsService = new FakeAppSettingsService();
            var customSettings = new AppSettings
            {
                SendLineEnding = SendLineEnding.CRLF
            };
            fakeSettingsService.Save(customSettings);

            /* Act */
            var viewModel = new MainWindowViewModel(fakeScanner, fakeService, fakeUiInvoker, fakeSettingsService);

            /* Assert */
            Assert.Equal(SendLineEnding.CRLF, viewModel.SelectedSendLineEnding);
        }

        /*
         * Test 保存配置时保存 SendLineEnding
         */
        [Fact]
        public void SaveSettings_SavesSendLineEnding()
        {
            /* Arrange */
            var fakeScanner = new FakeSerialPortScanner();
            var fakeService = new FakeSerialPortService();
            var fakeUiInvoker = new FakeUiThreadInvoker();
            var fakeSettingsService = new FakeAppSettingsService();
            var viewModel = new MainWindowViewModel(fakeScanner, fakeService, fakeUiInvoker, fakeSettingsService);
            viewModel.SelectedSendLineEnding = SendLineEnding.CRLF;

            /* Act */
            var result = viewModel.SaveSettings();

            /* Assert */
            Assert.True(result.IsSuccess);
            Assert.Equal(SendLineEnding.CRLF, fakeSettingsService.GetSavedSettings().SendLineEnding);
        }
    }
}
