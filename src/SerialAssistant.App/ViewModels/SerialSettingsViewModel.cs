using System.Collections.ObjectModel;
using System.Windows.Input;
using SerialAssistant.App.Commands;
using SerialAssistant.Core.Enums;
using SerialAssistant.Core.Models;
using SerialAssistant.Core.Utilities;

namespace SerialAssistant.App.ViewModels
{
    /*
     * ViewModel for serial port settings UI
     */
    public class SerialSettingsViewModel : BaseViewModel
    {
        private string? _selectedPortName;
        private int _selectedBaudRate;
        private int _selectedDataBits;
        private string? _selectedParity;
        private string? _selectedStopBits;
        private bool _isSettingsEnabled;

        public SerialSettingsViewModel()
        {
            AvailablePorts = new ObservableCollection<string>();
            BaudRates = new ObservableCollection<int>();
            DataBitsOptions = new ObservableCollection<int>();
            ParityOptions = new ObservableCollection<string>();
            StopBitsOptions = new ObservableCollection<string>();

            RefreshPortsCommand = new RelayCommand(RefreshPorts);

            InitializeDefaults();
        }

        public ObservableCollection<string> AvailablePorts
        {
            get;
            private set;
        }

        public string? SelectedPortName
        {
            get => _selectedPortName;
            set => SetProperty(ref _selectedPortName, value);
        }

        public ObservableCollection<int> BaudRates
        {
            get;
            private set;
        }

        public int SelectedBaudRate
        {
            get => _selectedBaudRate;
            set => SetProperty(ref _selectedBaudRate, value);
        }

        public ObservableCollection<int> DataBitsOptions
        {
            get;
            private set;
        }

        public int SelectedDataBits
        {
            get => _selectedDataBits;
            set => SetProperty(ref _selectedDataBits, value);
        }

        public ObservableCollection<string> ParityOptions
        {
            get;
            private set;
        }

        public string? SelectedParity
        {
            get => _selectedParity;
            set => SetProperty(ref _selectedParity, value);
        }

        public ObservableCollection<string> StopBitsOptions
        {
            get;
            private set;
        }

        public string? SelectedStopBits
        {
            get => _selectedStopBits;
            set => SetProperty(ref _selectedStopBits, value);
        }

        public bool IsSettingsEnabled
        {
            get => _isSettingsEnabled;
            set => SetProperty(ref _isSettingsEnabled, value);
        }

        public ICommand RefreshPortsCommand
        {
            get;
            private set;
        }

        public void UpdateAvailablePorts(IReadOnlyList<SerialPortInfo> ports)
        {
            AvailablePorts.Clear();
            foreach (var port in ports)
            {
                AvailablePorts.Add(port.PortName);
            }

            if (AvailablePorts.Count > 0 && string.IsNullOrEmpty(SelectedPortName))
            {
                SelectedPortName = AvailablePorts[0];
            }
            else if (AvailablePorts.Count == 0)
            {
                SelectedPortName = null;
            }
        }

        public SerialPortSettings CreateSettings()
        {
            return new SerialPortSettings
            {
                PortName = SelectedPortName ?? string.Empty,
                BaudRate = SelectedBaudRate,
                DataBits = SelectedDataBits,
                Parity = SelectedParity ?? "None",
                StopBits = SelectedStopBits ?? "One",
                ReadTimeout = 1000,
                WriteTimeout = 1000
            };
        }

        public OperationResult ValidateCurrentSettings()
        {
            return SerialSettingsValidator.Validate(CreateSettings());
        }

        private void InitializeDefaults()
        {
            SelectedBaudRate = 9600;
            SelectedDataBits = 8;
            SelectedParity = "None";
            SelectedStopBits = "One";
            IsSettingsEnabled = true;

            foreach (int rate in new[] { 300, 1200, 2400, 4800, 9600, 19200, 38400, 57600, 115200, 230400 })
            {
                BaudRates.Add(rate);
            }

            foreach (int bits in new[] { 5, 6, 7, 8 })
            {
                DataBitsOptions.Add(bits);
            }

            foreach (string parity in new[] { "None", "Odd", "Even", "Mark", "Space" })
            {
                ParityOptions.Add(parity);
            }

            foreach (string stopBits in new[] { "None", "One", "Two", "OnePointFive" })
            {
                StopBitsOptions.Add(stopBits);
            }
        }

        private void RefreshPorts(object? parameter)
        {
        }
    }
}
