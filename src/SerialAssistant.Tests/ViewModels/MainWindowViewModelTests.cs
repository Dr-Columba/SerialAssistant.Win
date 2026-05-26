using Xunit;
using SerialAssistant.App.ViewModels;
using SerialAssistant.Core.Enums;
using SerialAssistant.Core.Models;
using SerialAssistant.Tests.Infrastructure;
using SerialAssistant.Tests.UI;
using System.Text;

namespace SerialAssistant.Tests.ViewModels
{
    public class MainWindowViewModelTests
    {
        [Fact]
        public void Constructor_InitializesTerminal()
        {
            var viewModel = new MainWindowViewModel();
            Assert.NotNull(viewModel.Terminal);
        }

        [Fact]
        public void Terminal_IsOfTypeTerminalViewModel()
        {
            var viewModel = new MainWindowViewModel();
            Assert.IsType<TerminalViewModel>(viewModel.Terminal);
        }

        [Fact]
        public void Terminal_NotNull()
        {
            var viewModel = new MainWindowViewModel();
            Assert.NotNull(viewModel.Terminal);
        }

        [Fact]
        public void SaveSettings_DelegatesToTerminal()
        {
            var fakeScanner = new FakeSerialPortScanner();
            var fakeService = new FakeSerialPortService();
            var fakeSettingsService = new FakeAppSettingsService();
            var viewModel = new MainWindowViewModel(fakeScanner, fakeService, null, fakeSettingsService);
            var result = viewModel.SaveSettings();
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public void DefaultConnectionState_IsDisconnected()
        {
            var viewModel = new MainWindowViewModel();
            Assert.Equal(SerialConnectionState.Disconnected, viewModel.Terminal.ConnectionState);
        }

        [Fact]
        public void DefaultStatusMessage_IsNotEmpty()
        {
            var viewModel = new MainWindowViewModel();
            Assert.False(string.IsNullOrEmpty(viewModel.Terminal.StatusMessage));
        }

        [Fact]
        public void RefreshPortsCommand_WithoutScanner_ProvidesStatusMessage()
        {
            var viewModel = new MainWindowViewModel();
            viewModel.Terminal.RefreshPortsCommand.Execute(null);
            Assert.Contains("尚未接入", viewModel.Terminal.StatusMessage);
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
            var viewModel = new MainWindowViewModel(fakeScanner, fakeService);

            viewModel.Terminal.RefreshPortsCommand.Execute(null);

            Assert.Equal(2, viewModel.Terminal.SerialSettings.AvailablePorts.Count);
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
            var viewModel = new MainWindowViewModel(fakeScanner, fakeService);

            viewModel.Terminal.RefreshPortsCommand.Execute(null);

            Assert.Equal("COM1", viewModel.Terminal.SerialSettings.SelectedPortName);
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
            var viewModel = new MainWindowViewModel(fakeScanner, fakeService);

            viewModel.Terminal.RefreshPortsCommand.Execute(null);

            Assert.Contains("刷新", viewModel.Terminal.StatusMessage);
        }

        [Fact]
        public void ToggleConnectionCommand_WithValidSettings_ChangesStateToConnected()
        {
            var fakeScanner = new FakeSerialPortScanner(new List<SerialPortInfo>
            {
                SerialPortInfo.Create("COM1")
            });
            var fakeService = new FakeSerialPortService();
            var viewModel = new MainWindowViewModel(fakeScanner, fakeService);
            viewModel.Terminal.SerialSettings.SelectedPortName = "COM1";

            viewModel.Terminal.ToggleConnectionCommand.Execute(null);

            Assert.Equal(SerialConnectionState.Connected, viewModel.Terminal.ConnectionState);
        }

        [Fact]
        public void Open_Success_IsSettingsEnabled_IsFalse()
        {
            var fakeScanner = new FakeSerialPortScanner(new List<SerialPortInfo>
            {
                SerialPortInfo.Create("COM1")
            });
            var fakeService = new FakeSerialPortService();
            var viewModel = new MainWindowViewModel(fakeScanner, fakeService);
            viewModel.Terminal.SerialSettings.SelectedPortName = "COM1";

            viewModel.Terminal.ToggleConnectionCommand.Execute(null);

            Assert.False(viewModel.Terminal.SerialSettings.IsSettingsEnabled);
        }

        [Fact]
        public void Open_Success_StatusMessage_ContainsOpened()
        {
            var fakeScanner = new FakeSerialPortScanner(new List<SerialPortInfo>
            {
                SerialPortInfo.Create("COM1")
            });
            var fakeService = new FakeSerialPortService();
            var viewModel = new MainWindowViewModel(fakeScanner, fakeService);
            viewModel.Terminal.SerialSettings.SelectedPortName = "COM1";

            viewModel.Terminal.ToggleConnectionCommand.Execute(null);

            Assert.Contains("已打开", viewModel.Terminal.StatusMessage);
        }

        [Fact]
        public void SendCommand_TextMode_SendsCorrectData()
        {
            var fakeScanner = new FakeSerialPortScanner();
            var fakeService = new FakeSerialPortService();
            var viewModel = new MainWindowViewModel(fakeScanner, fakeService);

            viewModel.Terminal.SerialSettings.SelectedPortName = "COM1";
            viewModel.Terminal.ToggleConnectionCommand.Execute(null);

            viewModel.Terminal.SelectedSendMode = SendMode.Text;
            viewModel.Terminal.SendText = "ABC";

            viewModel.Terminal.SendCommand.Execute(null);

            Assert.Single(fakeService.SentData);
            Assert.Equal(Encoding.UTF8.GetBytes("ABC"), fakeService.SentData[0]);
            Assert.Equal(3, viewModel.Terminal.SentBytesCount);
        }

        [Fact]
        public void ClearReceiveCommand_ClearsReceiveArea()
        {
            var fakeScanner = new FakeSerialPortScanner();
            var fakeService = new FakeSerialPortService();
            var fakeUiInvoker = new FakeUiThreadInvoker();
            var viewModel = new MainWindowViewModel(fakeScanner, fakeService, fakeUiInvoker);
            byte[] data = Encoding.UTF8.GetBytes("some data");
            viewModel.Terminal.ReceiveDisplay.AddReceivedData(data);

            viewModel.Terminal.ClearReceiveCommand.Execute(null);

            Assert.Equal(string.Empty, viewModel.Terminal.ReceiveDisplay.ReceivedText);
        }

        [Fact]
        public void DefaultSendText_IsEmpty()
        {
            var viewModel = new MainWindowViewModel();
            Assert.Equal(string.Empty, viewModel.Terminal.SendText);
        }

        [Fact]
        public void DefaultSentBytesCount_Is0()
        {
            var viewModel = new MainWindowViewModel();
            Assert.Equal(0, viewModel.Terminal.SentBytesCount);
        }

        [Fact]
        public void SendModes_ContainsExpectedModes()
        {
            var viewModel = new MainWindowViewModel();
            Assert.Contains(SendMode.Text, viewModel.Terminal.SendModes);
            Assert.Contains(SendMode.Hex, viewModel.Terminal.SendModes);
        }

        [Fact]
        public void DefaultSelectedSendMode_IsText()
        {
            var viewModel = new MainWindowViewModel();
            Assert.Equal(SendMode.Text, viewModel.Terminal.SelectedSendMode);
        }

        [Fact]
        public void RefreshPortsCommand_CanExecute()
        {
            var viewModel = new MainWindowViewModel();
            Assert.True(viewModel.Terminal.RefreshPortsCommand.CanExecute(null));
        }

        [Fact]
        public void ToggleConnectionCommand_CanExecute_WhenServiceExists()
        {
            var fakeScanner = new FakeSerialPortScanner();
            var fakeService = new FakeSerialPortService();
            var viewModel = new MainWindowViewModel(fakeScanner, fakeService);

            Assert.True(viewModel.Terminal.ToggleConnectionCommand.CanExecute(null));
        }

        [Fact]
        public void ClearReceiveCommand_CanExecute()
        {
            var viewModel = new MainWindowViewModel();
            Assert.True(viewModel.Terminal.ClearReceiveCommand.CanExecute(null));
        }

        [Fact]
        public void SendLineEndings_ContainsExpectedOptions()
        {
            var viewModel = new MainWindowViewModel();
            Assert.Contains(SendLineEnding.None, viewModel.Terminal.SendLineEndings);
            Assert.Contains(SendLineEnding.CR, viewModel.Terminal.SendLineEndings);
            Assert.Contains(SendLineEnding.LF, viewModel.Terminal.SendLineEndings);
            Assert.Contains(SendLineEnding.CRLF, viewModel.Terminal.SendLineEndings);
        }

        [Fact]
        public void DefaultSelectedSendLineEnding_IsNone()
        {
            var viewModel = new MainWindowViewModel();
            Assert.Equal(SendLineEnding.None, viewModel.Terminal.SelectedSendLineEnding);
        }

        [Fact]
        public void SendCommand_TextMode_CRLF_SendsCorrectData()
        {
            var fakeScanner = new FakeSerialPortScanner();
            var fakeService = new FakeSerialPortService();
            var viewModel = new MainWindowViewModel(fakeScanner, fakeService);

            viewModel.Terminal.SerialSettings.SelectedPortName = "COM1";
            viewModel.Terminal.ToggleConnectionCommand.Execute(null);

            viewModel.Terminal.SelectedSendMode = SendMode.Text;
            viewModel.Terminal.SelectedSendLineEnding = SendLineEnding.CRLF;
            viewModel.Terminal.SendText = "ABC";

            viewModel.Terminal.SendCommand.Execute(null);

            Assert.Single(fakeService.SentData);
            Assert.Equal(Encoding.UTF8.GetBytes("ABC\r\n"), fakeService.SentData[0]);
            Assert.Equal(5, viewModel.Terminal.SentBytesCount);
        }

        [Fact]
        public void SendCommand_HexMode_WithSpaces_SendsCorrectData()
        {
            var fakeScanner = new FakeSerialPortScanner();
            var fakeService = new FakeSerialPortService();
            var viewModel = new MainWindowViewModel(fakeScanner, fakeService);

            viewModel.Terminal.SerialSettings.SelectedPortName = "COM1";
            viewModel.Terminal.ToggleConnectionCommand.Execute(null);

            viewModel.Terminal.SelectedSendMode = SendMode.Hex;
            viewModel.Terminal.SendText = "41 42 43";

            viewModel.Terminal.SendCommand.Execute(null);

            Assert.Single(fakeService.SentData);
            Assert.Equal(new byte[] { 0x41, 0x42, 0x43 }, fakeService.SentData[0]);
            Assert.Equal(3, viewModel.Terminal.SentBytesCount);
        }

        [Fact]
        public void ReceiveBufferSizeOptions_ContainsExpectedOptions()
        {
            var viewModel = new MainWindowViewModel();
            Assert.Contains(65536, viewModel.Terminal.ReceiveBufferSizeOptions);
            Assert.Contains(262144, viewModel.Terminal.ReceiveBufferSizeOptions);
            Assert.Contains(1048576, viewModel.Terminal.ReceiveBufferSizeOptions);
            Assert.Contains(4194304, viewModel.Terminal.ReceiveBufferSizeOptions);
        }

        [Fact]
        public void DefaultSendHistory_IsEmpty()
        {
            var viewModel = new MainWindowViewModel();
            Assert.Empty(viewModel.Terminal.SendHistory);
        }

        [Fact]
        public void DefaultMaxSendHistoryCount_Is20()
        {
            var viewModel = new MainWindowViewModel();
            Assert.Equal(20, viewModel.Terminal.MaxSendHistoryCount);
        }

        [Fact]
        public void SendCommand_TextMode_Success_AddsHistory()
        {
            var fakeScanner = new FakeSerialPortScanner();
            var fakeService = new FakeSerialPortService();
            var viewModel = new MainWindowViewModel(fakeScanner, fakeService);
            viewModel.Terminal.SerialSettings.SelectedPortName = "COM1";
            viewModel.Terminal.ToggleConnectionCommand.Execute(null);
            viewModel.Terminal.SelectedSendMode = SendMode.Text;
            viewModel.Terminal.SendText = "ABC";

            viewModel.Terminal.SendCommand.Execute(null);

            Assert.Single(viewModel.Terminal.SendHistory);
            Assert.Equal("ABC", viewModel.Terminal.SendHistory[0].Content);
            Assert.Equal(SendMode.Text, viewModel.Terminal.SendHistory[0].SendMode);
        }

        [Fact]
        public void ClearSendHistoryCommand_ClearsHistory()
        {
            var fakeScanner = new FakeSerialPortScanner();
            var fakeService = new FakeSerialPortService();
            var viewModel = new MainWindowViewModel(fakeScanner, fakeService);
            viewModel.Terminal.SerialSettings.SelectedPortName = "COM1";
            viewModel.Terminal.ToggleConnectionCommand.Execute(null);
            viewModel.Terminal.SelectedSendMode = SendMode.Text;
            viewModel.Terminal.SendText = "ABC";
            viewModel.Terminal.SendCommand.Execute(null);

            Assert.NotEmpty(viewModel.Terminal.SendHistory);

            viewModel.Terminal.ClearSendHistoryCommand.Execute(null);

            Assert.Empty(viewModel.Terminal.SendHistory);
        }

        [Fact]
        public void SelectedSendHistoryItem_Set_UpdatesSendText()
        {
            var viewModel = new MainWindowViewModel();
            var historyItem = new SendHistoryItem("Hello World", SendMode.Text);

            viewModel.Terminal.SelectedSendHistoryItem = historyItem;

            Assert.Equal("Hello World", viewModel.Terminal.SendText);
        }

        [Fact]
        public void SelectedSendHistoryItem_Set_UpdatesSendMode()
        {
            var viewModel = new MainWindowViewModel();
            viewModel.Terminal.SelectedSendMode = SendMode.Text;

            var historyItem = new SendHistoryItem("41 42 43", SendMode.Hex);

            viewModel.Terminal.SelectedSendHistoryItem = historyItem;

            Assert.Equal(SendMode.Hex, viewModel.Terminal.SelectedSendMode);
        }

        [Fact]
        public void SaveSettings_CallsSettingsService()
        {
            var fakeScanner = new FakeSerialPortScanner();
            var fakeService = new FakeSerialPortService();
            var fakeUiInvoker = new FakeUiThreadInvoker();
            var fakeSettingsService = new FakeAppSettingsService();
            var viewModel = new MainWindowViewModel(fakeScanner, fakeService, fakeUiInvoker, fakeSettingsService);

            viewModel.Terminal.SerialSettings.SelectedPortName = "COM3";
            viewModel.Terminal.SerialSettings.SelectedBaudRate = 9600;

            var result = viewModel.SaveSettings();

            Assert.True(result.IsSuccess);
            Assert.Equal("COM3", fakeSettingsService.GetSavedSettings().LastPortName);
            Assert.Equal(9600, fakeSettingsService.GetSavedSettings().BaudRate);
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
                StopBits = "Two"
            };
            fakeSettingsService.Save(customSettings);

            var viewModel = new MainWindowViewModel(fakeScanner, fakeService, fakeUiInvoker, fakeSettingsService);

            Assert.Equal(115200, viewModel.Terminal.SerialSettings.SelectedBaudRate);
            Assert.Equal(7, viewModel.Terminal.SerialSettings.SelectedDataBits);
            Assert.Equal("Odd", viewModel.Terminal.SerialSettings.SelectedParity);
            Assert.Equal("Two", viewModel.Terminal.SerialSettings.SelectedStopBits);
        }

        [Fact]
        public void ViewModel_Terminal_NotNull()
        {
            var viewModel = new MainWindowViewModel();
            Assert.NotNull(viewModel.Terminal);
            Assert.NotNull(viewModel.Terminal.SerialSettings);
            Assert.NotNull(viewModel.Terminal.ReceiveDisplay);
        }

        [Fact]
        public void Default_IsTerminalSelected_IsTrue()
        {
            var viewModel = new MainWindowViewModel();
            Assert.True(viewModel.IsTerminalSelected);
        }

        [Fact]
        public void Default_IsModbusSelected_IsFalse()
        {
            var viewModel = new MainWindowViewModel();
            Assert.False(viewModel.IsModbusSelected);
        }

        [Fact]
        public void Default_IsTerminalPageVisible_IsTrue()
        {
            var viewModel = new MainWindowViewModel();
            Assert.True(viewModel.IsTerminalPageVisible);
        }

        [Fact]
        public void Default_IsModbusPageVisible_IsFalse()
        {
            var viewModel = new MainWindowViewModel();
            Assert.False(viewModel.IsModbusPageVisible);
        }

        [Fact]
        public void Modbus_NotNull()
        {
            var viewModel = new MainWindowViewModel();
            Assert.NotNull(viewModel.Modbus);
        }

        [Fact]
        public void ShowModbusCommand_SetsIsModbusSelected_True()
        {
            var viewModel = new MainWindowViewModel();
            viewModel.ShowModbusCommand.Execute(null);
            Assert.True(viewModel.IsModbusSelected);
        }

        [Fact]
        public void ShowModbusCommand_SetsIsTerminalSelected_False()
        {
            var viewModel = new MainWindowViewModel();
            viewModel.ShowModbusCommand.Execute(null);
            Assert.False(viewModel.IsTerminalSelected);
        }

        [Fact]
        public void ShowTerminalCommand_SetsIsTerminalSelected_True()
        {
            var viewModel = new MainWindowViewModel();
            viewModel.ShowModbusCommand.Execute(null);
            viewModel.ShowTerminalCommand.Execute(null);
            Assert.True(viewModel.IsTerminalSelected);
        }

        [Fact]
        public void ShowTerminalCommand_SetsIsModbusSelected_False()
        {
            var viewModel = new MainWindowViewModel();
            viewModel.ShowModbusCommand.Execute(null);
            viewModel.ShowTerminalCommand.Execute(null);
            Assert.False(viewModel.IsModbusSelected);
        }

        [Fact]
        public void ShowModbusCommand_UpdatesIsModbusPageVisible_True()
        {
            var viewModel = new MainWindowViewModel();
            viewModel.ShowModbusCommand.Execute(null);
            Assert.True(viewModel.IsModbusPageVisible);
        }

        [Fact]
        public void ShowTerminalCommand_UpdatesIsTerminalPageVisible_True()
        {
            var viewModel = new MainWindowViewModel();
            viewModel.ShowModbusCommand.Execute(null);
            viewModel.ShowTerminalCommand.Execute(null);
            Assert.True(viewModel.IsTerminalPageVisible);
        }

        [Fact]
        public void Terminal_Still_NotNull_AfterShowModbus()
        {
            var viewModel = new MainWindowViewModel();
            viewModel.ShowModbusCommand.Execute(null);
            Assert.NotNull(viewModel.Terminal);
        }

        [Fact]
        public void SaveSettings_Still_Delegates_AfterShowModbus()
        {
            var fakeScanner = new FakeSerialPortScanner();
            var fakeService = new FakeSerialPortService();
            var fakeSettingsService = new FakeAppSettingsService();
            var viewModel = new MainWindowViewModel(fakeScanner, fakeService, null, fakeSettingsService);
            viewModel.ShowModbusCommand.Execute(null);
            var result = viewModel.SaveSettings();
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public void ShowModbusCommand_SetsIsTerminalPageVisible_False()
        {
            var viewModel = new MainWindowViewModel();
            viewModel.ShowModbusCommand.Execute(null);
            Assert.False(viewModel.IsTerminalPageVisible);
        }

        [Fact]
        public void ShowTerminalCommand_SetsIsModbusPageVisible_False()
        {
            var viewModel = new MainWindowViewModel();
            viewModel.ShowModbusCommand.Execute(null);
            viewModel.ShowTerminalCommand.Execute(null);
            Assert.False(viewModel.IsModbusPageVisible);
        }

        [Fact]
        public void RepeatedPageSwitching_WorksCorrectly()
        {
            var viewModel = new MainWindowViewModel();

            // Round 1
            viewModel.ShowModbusCommand.Execute(null);
            Assert.True(viewModel.IsModbusPageVisible);
            Assert.False(viewModel.IsTerminalPageVisible);

            viewModel.ShowTerminalCommand.Execute(null);
            Assert.False(viewModel.IsModbusPageVisible);
            Assert.True(viewModel.IsTerminalPageVisible);

            // Round 2
            viewModel.ShowModbusCommand.Execute(null);
            Assert.True(viewModel.IsModbusPageVisible);
            Assert.False(viewModel.IsTerminalPageVisible);

            viewModel.ShowTerminalCommand.Execute(null);
            Assert.False(viewModel.IsModbusPageVisible);
            Assert.True(viewModel.IsTerminalPageVisible);

            // Round 3
            viewModel.ShowModbusCommand.Execute(null);
            Assert.True(viewModel.IsModbusPageVisible);
            Assert.False(viewModel.IsTerminalPageVisible);

            viewModel.ShowTerminalCommand.Execute(null);
            Assert.False(viewModel.IsModbusPageVisible);
            Assert.True(viewModel.IsTerminalPageVisible);
        }

        [Fact]
        public void ShowTerminalCommand_WhenAlreadyTerminal_NoSideEffects()
        {
            var viewModel = new MainWindowViewModel();
            bool wasTerminalSelected = viewModel.IsTerminalSelected;
            bool wasModbusSelected = viewModel.IsModbusSelected;
            bool wasTerminalPageVisible = viewModel.IsTerminalPageVisible;
            bool wasModbusPageVisible = viewModel.IsModbusPageVisible;

            viewModel.ShowTerminalCommand.Execute(null);

            Assert.Equal(wasTerminalSelected, viewModel.IsTerminalSelected);
            Assert.Equal(wasModbusSelected, viewModel.IsModbusSelected);
            Assert.Equal(wasTerminalPageVisible, viewModel.IsTerminalPageVisible);
            Assert.Equal(wasModbusPageVisible, viewModel.IsModbusPageVisible);
        }

        [Fact]
        public void ShowModbusCommand_WhenAlreadyModbus_NoSideEffects()
        {
            var viewModel = new MainWindowViewModel();
            viewModel.ShowModbusCommand.Execute(null);
            bool wasTerminalSelected = viewModel.IsTerminalSelected;
            bool wasModbusSelected = viewModel.IsModbusSelected;
            bool wasTerminalPageVisible = viewModel.IsTerminalPageVisible;
            bool wasModbusPageVisible = viewModel.IsModbusPageVisible;

            viewModel.ShowModbusCommand.Execute(null);

            Assert.Equal(wasTerminalSelected, viewModel.IsTerminalSelected);
            Assert.Equal(wasModbusSelected, viewModel.IsModbusSelected);
            Assert.Equal(wasTerminalPageVisible, viewModel.IsTerminalPageVisible);
            Assert.Equal(wasModbusPageVisible, viewModel.IsModbusPageVisible);
        }
    }
}
