using Xunit;
using SerialAssistant.App.ViewModels;
using SerialAssistant.Core.Enums;
using SerialAssistant.Core.Models;
using SerialAssistant.Tests.Infrastructure;

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
            var fakeService = new FakeSerialPortService(true, false, "Open failed");
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
            var fakeService = new FakeSerialPortService(true, false, "Open failed");
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
            var fakeService = new FakeSerialPortService(true, false, "Open failed");
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
            var fakeService = new FakeSerialPortService(false, true, "Close failed");
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
            var viewModel = new MainWindowViewModel();
            viewModel.ReceiveDisplay.ReceivedText = "some data";

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
            var viewModel = new MainWindowViewModel();
            viewModel.ReceiveDisplay.ReceivedText = "some data";

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
         * Test SendCommand can execute when SendText is not empty
         */
        [Fact]
        public void SendCommand_CanExecute_WhenSendTextNotEmpty()
        {
            /* Arrange */
            var viewModel = new MainWindowViewModel();
            viewModel.SendText = "test data";

            /* Act & Assert */
            Assert.True(viewModel.SendCommand.CanExecute(null));
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
    }
}
