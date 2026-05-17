using Xunit;
using SerialAssistant.App.ViewModels;

namespace SerialAssistant.Tests.ViewModels
{
    /*
     * Tests for SerialSettingsViewModel
     */
    public class SerialSettingsViewModelTests
    {
        /*
         * Test default baud rate is 9600
         */
        [Fact]
        public void DefaultBaudRate_Is9600()
        {
            /* Arrange */
            var viewModel = new SerialSettingsViewModel();

            /* Act & Assert */
            Assert.Equal(9600, viewModel.SelectedBaudRate);
        }

        /*
         * Test default data bits is 8
         */
        [Fact]
        public void DefaultDataBits_Is8()
        {
            /* Arrange */
            var viewModel = new SerialSettingsViewModel();

            /* Act & Assert */
            Assert.Equal(8, viewModel.SelectedDataBits);
        }

        /*
         * Test default parity is None
         */
        [Fact]
        public void DefaultParity_IsNone()
        {
            /* Arrange */
            var viewModel = new SerialSettingsViewModel();

            /* Act & Assert */
            Assert.Equal("None", viewModel.SelectedParity);
        }

        /*
         * Test default stop bits is One
         */
        [Fact]
        public void DefaultStopBits_IsOne()
        {
            /* Arrange */
            var viewModel = new SerialSettingsViewModel();

            /* Act & Assert */
            Assert.Equal("One", viewModel.SelectedStopBits);
        }

        /*
         * Test settings controls are enabled by default
         */
        [Fact]
        public void SettingsControls_EnabledByDefault()
        {
            /* Arrange */
            var viewModel = new SerialSettingsViewModel();

            /* Act & Assert */
            Assert.True(viewModel.IsSettingsEnabled);
        }

        /*
         * Test SelectedPortName can be set
         */
        [Fact]
        public void SelectedPortName_CanBeSet()
        {
            /* Arrange */
            var viewModel = new SerialSettingsViewModel();

            /* Act */
            viewModel.SelectedPortName = "COM1";

            /* Assert */
            Assert.Equal("COM1", viewModel.SelectedPortName);
        }

        /*
         * Test property change notification is raised on port name change
         */
        [Fact]
        public void SelectedPortName_ChangeRaisesPropertyChanged()
        {
            /* Arrange */
            var viewModel = new SerialSettingsViewModel();
            bool propertyChangedRaised = false;
            viewModel.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(SerialSettingsViewModel.SelectedPortName))
                {
                    propertyChangedRaised = true;
                }
            };

            /* Act */
            viewModel.SelectedPortName = "COM2";

            /* Assert */
            Assert.True(propertyChangedRaised);
        }

        /*
         * Test property change notification is raised on baud rate change
         */
        [Fact]
        public void SelectedBaudRate_ChangeRaisesPropertyChanged()
        {
            /* Arrange */
            var viewModel = new SerialSettingsViewModel();
            bool propertyChangedRaised = false;
            viewModel.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(SerialSettingsViewModel.SelectedBaudRate))
                {
                    propertyChangedRaised = true;
                }
            };

            /* Act */
            viewModel.SelectedBaudRate = 115200;

            /* Assert */
            Assert.True(propertyChangedRaised);
        }

        /*
         * Test baud rate options contain expected values
         */
        [Fact]
        public void BaudRates_ContainsExpectedValues()
        {
            /* Arrange */
            var viewModel = new SerialSettingsViewModel();

            /* Act & Assert */
            Assert.Contains(9600, viewModel.BaudRates);
            Assert.Contains(115200, viewModel.BaudRates);
        }

        /*
         * Test data bits options contain expected values
         */
        [Fact]
        public void DataBitsOptions_ContainsExpectedValues()
        {
            /* Arrange */
            var viewModel = new SerialSettingsViewModel();

            /* Act & Assert */
            Assert.Contains(8, viewModel.DataBitsOptions);
            Assert.Contains(7, viewModel.DataBitsOptions);
        }

        /*
         * Test parity options contain expected values
         */
        [Fact]
        public void ParityOptions_ContainsExpectedValues()
        {
            /* Arrange */
            var viewModel = new SerialSettingsViewModel();

            /* Act & Assert */
            Assert.Contains("None", viewModel.ParityOptions);
            Assert.Contains("Even", viewModel.ParityOptions);
        }

        /*
         * Test stop bits options contain expected values
         */
        [Fact]
        public void StopBitsOptions_ContainsExpectedValues()
        {
            /* Arrange */
            var viewModel = new SerialSettingsViewModel();

            /* Act & Assert */
            Assert.Contains("One", viewModel.StopBitsOptions);
            Assert.Contains("Two", viewModel.StopBitsOptions);
        }

        /*
         * Test RefreshPortsCommand can be executed
         */
        [Fact]
        public void RefreshPortsCommand_CanExecute()
        {
            /* Arrange */
            var viewModel = new SerialSettingsViewModel();

            /* Act & Assert */
            Assert.NotNull(viewModel.RefreshPortsCommand);
            Assert.True(viewModel.RefreshPortsCommand.CanExecute(null));
        }

        /*
         * Test UpdateAvailablePorts updates collection count
         */
        [Fact]
        public void UpdateAvailablePorts_UpdatesCollectionCount()
        {
            /* Arrange */
            var viewModel = new SerialSettingsViewModel();
            var ports = new List<SerialAssistant.Core.Models.SerialPortInfo>
            {
                SerialAssistant.Core.Models.SerialPortInfo.Create("COM1"),
                SerialAssistant.Core.Models.SerialPortInfo.Create("COM2"),
                SerialAssistant.Core.Models.SerialPortInfo.Create("COM3")
            };

            /* Act */
            viewModel.UpdateAvailablePorts(ports);

            /* Assert */
            Assert.Equal(3, viewModel.AvailablePorts.Count);
        }

        /*
         * Test UpdateAvailablePorts selects first port when list is not empty
         */
        [Fact]
        public void UpdateAvailablePorts_SelectsFirstPort()
        {
            /* Arrange */
            var viewModel = new SerialSettingsViewModel();
            var ports = new List<SerialAssistant.Core.Models.SerialPortInfo>
            {
                SerialAssistant.Core.Models.SerialPortInfo.Create("COM1"),
                SerialAssistant.Core.Models.SerialPortInfo.Create("COM2")
            };

            /* Act */
            viewModel.UpdateAvailablePorts(ports);

            /* Assert */
            Assert.Equal("COM1", viewModel.SelectedPortName);
        }

        /*
         * Test UpdateAvailablePorts sets SelectedPortName to null when list is empty
         */
        [Fact]
        public void UpdateAvailablePorts_SetsSelectedPortNameToNull_WhenEmpty()
        {
            /* Arrange */
            var viewModel = new SerialSettingsViewModel();
            viewModel.SelectedPortName = "COM1";
            var ports = new List<SerialAssistant.Core.Models.SerialPortInfo>();

            /* Act */
            viewModel.UpdateAvailablePorts(ports);

            /* Assert */
            Assert.Null(viewModel.SelectedPortName);
        }

        /*
         * Test CreateSettings generates correct SerialPortSettings
         */
        [Fact]
        public void CreateSettings_GeneratesCorrectSettings()
        {
            /* Arrange */
            var viewModel = new SerialSettingsViewModel();
            viewModel.SelectedPortName = "COM3";
            viewModel.SelectedBaudRate = 115200;
            viewModel.SelectedDataBits = 7;
            viewModel.SelectedParity = "Even";
            viewModel.SelectedStopBits = "Two";

            /* Act */
            var settings = viewModel.CreateSettings();

            /* Assert */
            Assert.Equal("COM3", settings.PortName);
            Assert.Equal(115200, settings.BaudRate);
            Assert.Equal(7, settings.DataBits);
            Assert.Equal("Even", settings.Parity);
            Assert.Equal("Two", settings.StopBits);
            Assert.Equal(1000, settings.ReadTimeout);
            Assert.Equal(1000, settings.WriteTimeout);
        }

        /*
         * Test ValidateCurrentSettings passes with valid settings
         */
        [Fact]
        public void ValidateCurrentSettings_PassesWithValidSettings()
        {
            /* Arrange */
            var viewModel = new SerialSettingsViewModel();
            viewModel.SelectedPortName = "COM1";

            /* Act */
            var result = viewModel.ValidateCurrentSettings();

            /* Assert */
            Assert.True(result.IsSuccess);
        }

        /*
         * Test ValidateCurrentSettings fails when no port selected
         */
        [Fact]
        public void ValidateCurrentSettings_FailsWhenNoPortSelected()
        {
            /* Arrange */
            var viewModel = new SerialSettingsViewModel();
            viewModel.SelectedPortName = null;

            /* Act */
            var result = viewModel.ValidateCurrentSettings();

            /* Assert */
            Assert.False(result.IsSuccess);
            Assert.Contains("Port name", result.ErrorMessage);
        }

        /*
         * Test ValidateCurrentSettings reflects new baud rate
         */
        [Fact]
        public void ValidateCurrentSettings_ReflectsNewBaudRate()
        {
            /* Arrange */
            var viewModel = new SerialSettingsViewModel();
            viewModel.SelectedPortName = "COM1";
            viewModel.SelectedBaudRate = 115200;

            /* Act */
            var settings = viewModel.CreateSettings();

            /* Assert */
            Assert.Equal(115200, settings.BaudRate);
        }

        /*
         * Test ValidateCurrentSettings reflects new parity
         */
        [Fact]
        public void ValidateCurrentSettings_ReflectsNewParity()
        {
            /* Arrange */
            var viewModel = new SerialSettingsViewModel();
            viewModel.SelectedPortName = "COM1";
            viewModel.SelectedParity = "Odd";

            /* Act */
            var settings = viewModel.CreateSettings();

            /* Assert */
            Assert.Equal("Odd", settings.Parity);
        }

        /*
         * Test ValidateCurrentSettings reflects new stop bits
         */
        [Fact]
        public void ValidateCurrentSettings_ReflectsNewStopBits()
        {
            /* Arrange */
            var viewModel = new SerialSettingsViewModel();
            viewModel.SelectedPortName = "COM1";
            viewModel.SelectedStopBits = "Two";

            /* Act */
            var settings = viewModel.CreateSettings();

            /* Assert */
            Assert.Equal("Two", settings.StopBits);
        }

        /*
         * Test CreateSettings uses default timeout values
         */
        [Fact]
        public void CreateSettings_UsesDefaultTimeoutValues()
        {
            /* Arrange */
            var viewModel = new SerialSettingsViewModel();
            viewModel.SelectedPortName = "COM1";

            /* Act */
            var settings = viewModel.CreateSettings();

            /* Assert */
            Assert.Equal(1000, settings.ReadTimeout);
            Assert.Equal(1000, settings.WriteTimeout);
        }
    }
}
