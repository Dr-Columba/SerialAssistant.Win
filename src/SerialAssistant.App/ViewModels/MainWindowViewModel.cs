using SerialAssistant.Core.Services;
using SerialAssistant.Core.Utilities;
using SerialAssistant.Core.Models;
using SerialAssistant.App.Commands;
using System.Windows.Input;

namespace SerialAssistant.App.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        private readonly ISerialPortScanner? _scanner;
        private readonly ISerialPortService? _serialPortService;
        private readonly IUiThreadInvoker? _uiThreadInvoker;
        private readonly IAppSettingsService? _appSettingsService;

        private bool _isTerminalSelected = true;
        private bool _isModbusSelected = false;

        public MainWindowViewModel()
            : this(null, null, null, null)
        {
        }

        public MainWindowViewModel(ISerialPortScanner? scanner, ISerialPortService? serialPortService)
            : this(scanner, serialPortService, null, null)
        {
        }

        public MainWindowViewModel(ISerialPortScanner? scanner, ISerialPortService? serialPortService, IUiThreadInvoker? uiThreadInvoker)
            : this(scanner, serialPortService, uiThreadInvoker, null)
        {
        }

        public MainWindowViewModel(ISerialPortScanner? scanner, ISerialPortService? serialPortService, IUiThreadInvoker? uiThreadInvoker, IAppSettingsService? appSettingsService)
        {
            _scanner = scanner;
            _serialPortService = serialPortService;
            _uiThreadInvoker = uiThreadInvoker;
            _appSettingsService = appSettingsService;

            Terminal = new TerminalViewModel(scanner, serialPortService, uiThreadInvoker, appSettingsService);
            Modbus = new ModbusViewModel();
            ShowTerminalCommand = new RelayCommand(ShowTerminal);
            ShowModbusCommand = new RelayCommand(ShowModbus);
        }

        public TerminalViewModel Terminal
        {
            get;
            private set;
        }

        public ModbusViewModel Modbus
        {
            get;
            private set;
        }

        public bool IsTerminalSelected
        {
            get => _isTerminalSelected;
            private set => SetProperty(ref _isTerminalSelected, value);
        }

        public bool IsModbusSelected
        {
            get => _isModbusSelected;
            private set => SetProperty(ref _isModbusSelected, value);
        }

        public bool IsTerminalPageVisible => IsTerminalSelected;

        public bool IsModbusPageVisible => IsModbusSelected;

        public ICommand ShowTerminalCommand { get; }

        public ICommand ShowModbusCommand { get; }

        public OperationResult SaveSettings()
        {
            return Terminal.SaveSettings();
        }

        private void ShowTerminal()
        {
            IsTerminalSelected = true;
            IsModbusSelected = false;
            OnPropertyChanged(nameof(IsTerminalPageVisible));
            OnPropertyChanged(nameof(IsModbusPageVisible));
        }

        private void ShowModbus()
        {
            IsTerminalSelected = false;
            IsModbusSelected = true;
            OnPropertyChanged(nameof(IsTerminalPageVisible));
            OnPropertyChanged(nameof(IsModbusPageVisible));
        }
    }
}
