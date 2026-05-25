using System.Collections.ObjectModel;
using System.Windows.Input;
using SerialAssistant.App.Commands;
using SerialAssistant.Core.Enums;
using SerialAssistant.Core.Services;
using SerialAssistant.Core.Utilities;
using SerialAssistant.Core.Models;
using System.Text;

namespace SerialAssistant.App.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        private readonly ISerialPortScanner? _scanner;
        private readonly ISerialPortService? _serialPortService;
        private readonly IUiThreadInvoker? _uiThreadInvoker;
        private readonly IAppSettingsService? _appSettingsService;

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
        }

        public TerminalViewModel Terminal
        {
            get;
            private set;
        }

        #region Compatibility Forwarding Properties (for existing tests)

        public SerialSettingsViewModel SerialSettings => Terminal.SerialSettings;

        public ReceiveDisplayViewModel ReceiveDisplay => Terminal.ReceiveDisplay;

        public bool IsHexDisplay
        {
            get => Terminal.ReceiveDisplay.IsHexDisplay;
            set => Terminal.ReceiveDisplay.IsHexDisplay = value;
        }

        public string SendText
        {
            get => Terminal.SendText;
            set => Terminal.SendText = value;
        }

        public ObservableCollection<SendMode> SendModes => Terminal.SendModes;

        public SendMode SelectedSendMode
        {
            get => Terminal.SelectedSendMode;
            set => Terminal.SelectedSendMode = value;
        }

        public ObservableCollection<SendLineEnding> SendLineEndings => Terminal.SendLineEndings;

        public ObservableCollection<int> ReceiveBufferSizeOptions => Terminal.ReceiveBufferSizeOptions;

        public SendLineEnding SelectedSendLineEnding
        {
            get => Terminal.SelectedSendLineEnding;
            set => Terminal.SelectedSendLineEnding = value;
        }

        public SerialConnectionState ConnectionState => Terminal.ConnectionState;

        public string StatusMessage => Terminal.StatusMessage;

        public int SentBytesCount => Terminal.SentBytesCount;

        public string ConnectionButtonText => Terminal.ConnectionButtonText;

        public ICommand RefreshPortsCommand => Terminal.RefreshPortsCommand;

        public ICommand ToggleConnectionCommand => Terminal.ToggleConnectionCommand;

        public ICommand SendCommand => Terminal.SendCommand;

        public ICommand ClearReceiveCommand => Terminal.ClearReceiveCommand;

        public ICommand ClearSendHistoryCommand => Terminal.ClearSendHistoryCommand;

        public ObservableCollection<SendHistoryItem> SendHistory => Terminal.SendHistory;

        public int MaxSendHistoryCount
        {
            get => Terminal.MaxSendHistoryCount;
            set => Terminal.MaxSendHistoryCount = value;
        }

        public SendHistoryItem? SelectedSendHistoryItem
        {
            get => Terminal.SelectedSendHistoryItem;
            set => Terminal.SelectedSendHistoryItem = value;
        }

        #endregion

        public OperationResult SaveSettings()
        {
            return Terminal.SaveSettings();
        }
    }
}
