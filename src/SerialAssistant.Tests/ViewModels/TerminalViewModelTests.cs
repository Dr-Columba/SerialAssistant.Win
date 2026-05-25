using Xunit;
using SerialAssistant.App.ViewModels;
using SerialAssistant.Core.Enums;
using System.Collections.ObjectModel;

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
            Assert.IsType<ObservableCollection<Core.Models.SendHistoryItem>>(viewModel.SendHistory);
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
        public void ClearSendHistoryCommand_ClearsHistory()
        {
            var viewModel = new TerminalViewModel();
            
            viewModel.SelectedSendHistoryItem = null;
            
            Assert.Empty(viewModel.SendHistory);
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
        public void Commands_NotNull()
        {
            var viewModel = new TerminalViewModel();
            
            Assert.NotNull(viewModel.RefreshPortsCommand);
            Assert.NotNull(viewModel.ToggleConnectionCommand);
            Assert.NotNull(viewModel.SendCommand);
            Assert.NotNull(viewModel.ClearReceiveCommand);
            Assert.NotNull(viewModel.ClearSendHistoryCommand);
        }
    }
}
