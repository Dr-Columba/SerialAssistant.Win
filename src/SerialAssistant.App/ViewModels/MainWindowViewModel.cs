using System.Collections.ObjectModel;
using System.Windows.Input;
using SerialAssistant.App.Commands;
using SerialAssistant.Core.Enums;
using SerialAssistant.Core.Services;

namespace SerialAssistant.App.ViewModels
{
    /*
     * Main ViewModel for the application window
     */
    public class MainWindowViewModel : BaseViewModel
    {
        private readonly ISerialPortScanner? _scanner;
        private readonly ISerialPortService? _serialPortService;
        private SerialConnectionState _connectionState;
        private string _sendText;
        private SendMode _selectedSendMode;
        private string _statusMessage;
        private int _sentBytesCount;
        private bool _isHexDisplay;
        private string _connectionButtonText;

        public MainWindowViewModel()
            : this(null, null)
        {
        }

        public MainWindowViewModel(ISerialPortScanner? scanner, ISerialPortService? serialPortService)
        {
            _scanner = scanner;
            _serialPortService = serialPortService;
            SerialSettings = new SerialSettingsViewModel();
            ReceiveDisplay = new ReceiveDisplayViewModel();
            SendModes = new ObservableCollection<SendMode>();

            _sendText = string.Empty;
            _statusMessage = "就绪。请点击刷新按钮获取可用串口。";
            _sentBytesCount = 0;
            _selectedSendMode = SendMode.Text;
            _connectionButtonText = "打开串口";

            foreach (SendMode mode in Enum.GetValues<SendMode>())
            {
                SendModes.Add(mode);
            }

            if (_serialPortService != null)
            {
                _serialPortService.ConnectionStateChanged += OnConnectionStateChanged;
                _connectionState = _serialPortService.ConnectionState;
                UpdateConnectionButtonText(_connectionState);
            }
            else
            {
                _connectionState = SerialConnectionState.Disconnected;
            }

            RefreshPortsCommand = new RelayCommand(RefreshPorts);
            ToggleConnectionCommand = new RelayCommand(ToggleConnection, CanToggleConnection);
            SendCommand = new RelayCommand(Send, CanSend);
            ClearReceiveCommand = new RelayCommand(ClearReceive);
        }

        public SerialSettingsViewModel SerialSettings
        {
            get;
            private set;
        }

        public ReceiveDisplayViewModel ReceiveDisplay
        {
            get;
            private set;
        }

        public string SendText
        {
            get => _sendText;
            set => SetProperty(ref _sendText, value);
        }

        public ObservableCollection<SendMode> SendModes
        {
            get;
            private set;
        }

        public SendMode SelectedSendMode
        {
            get => _selectedSendMode;
            set => SetProperty(ref _selectedSendMode, value);
        }

        public SerialConnectionState ConnectionState
        {
            get => _connectionState;
            private set => SetProperty(ref _connectionState, value);
        }

        public string StatusMessage
        {
            get => _statusMessage;
            private set => SetProperty(ref _statusMessage, value);
        }

        public int SentBytesCount
        {
            get => _sentBytesCount;
            private set => SetProperty(ref _sentBytesCount, value);
        }

        public bool IsHexDisplay
        {
            get => _isHexDisplay;
            set
            {
                if (SetProperty(ref _isHexDisplay, value))
                {
                    ReceiveDisplay.IsHexDisplay = value;
                }
            }
        }

        public string ConnectionButtonText
        {
            get => _connectionButtonText;
            private set => SetProperty(ref _connectionButtonText, value);
        }

        public ICommand RefreshPortsCommand
        {
            get;
            private set;
        }

        public ICommand ToggleConnectionCommand
        {
            get;
            private set;
        }

        public ICommand SendCommand
        {
            get;
            private set;
        }

        public ICommand ClearReceiveCommand
        {
            get;
            private set;
        }

        private void UpdateConnectionButtonText(SerialConnectionState state)
        {
            ConnectionButtonText = state == SerialConnectionState.Connected ? "关闭串口" : "打开串口";
        }

        private void OnConnectionStateChanged(object? sender, SerialConnectionState state)
        {
            ConnectionState = state;
            UpdateConnectionButtonText(state);

            if (state == SerialConnectionState.Connected)
            {
                SerialSettings.IsSettingsEnabled = false;
            }
            else if (state == SerialConnectionState.Disconnected)
            {
                SerialSettings.IsSettingsEnabled = true;
            }
        }

        private void RefreshPorts(object? parameter)
        {
            if (_scanner == null)
            {
                StatusMessage = "当前阶段尚未接入串口扫描功能。";
                return;
            }

            try
            {
                var result = _scanner.GetAvailablePorts();

                if (result.IsSuccess)
                {
                    SerialSettings.UpdateAvailablePorts(result.Value!);
                    int count = result.Value!.Count;

                    if (count > 0)
                    {
                        StatusMessage = $"已刷新串口列表，共 {count} 个可用串口。";
                    }
                    else
                    {
                        StatusMessage = "未发现可用串口。";
                    }
                }
                else
                {
                    StatusMessage = $"串口扫描失败：{result.ErrorMessage}";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"串口扫描异常：{ex.Message}";
            }
        }

        private bool CanToggleConnection(object? parameter)
        {
            return _serialPortService != null;
        }

        private void ToggleConnection(object? parameter)
        {
            if (_serialPortService == null)
            {
                StatusMessage = "当前阶段尚未接入串口服务。";
                return;
            }

            if (ConnectionState == SerialConnectionState.Disconnected)
            {
                OpenPort();
            }
            else
            {
                ClosePort();
            }
        }

        private void OpenPort()
        {
            var settings = SerialSettings.CreateSettings();
            var validationResult = SerialSettings.ValidateCurrentSettings();

            if (!validationResult.IsSuccess)
            {
                StatusMessage = $"串口参数无效：{validationResult.ErrorMessage}";
                return;
            }

            var openResult = _serialPortService!.Open(settings);

            if (openResult.IsSuccess)
            {
                StatusMessage = $"串口 {settings.PortName} 已打开。";
            }
            else
            {
                StatusMessage = $"打开串口失败：{openResult.ErrorMessage}";
            }
        }

        private void ClosePort()
        {
            var closeResult = _serialPortService!.Close();

            if (closeResult.IsSuccess)
            {
                StatusMessage = "串口已关闭。";
            }
            else
            {
                StatusMessage = $"关闭串口失败：{closeResult.ErrorMessage}";
            }
        }

        private bool CanSend(object? parameter)
        {
            return !string.IsNullOrEmpty(SendText);
        }

        private void Send(object? parameter)
        {
            if (ConnectionState != SerialConnectionState.Connected)
            {
                StatusMessage = "当前阶段尚未接入串口发送功能。";
                return;
            }

            SentBytesCount += System.Text.Encoding.UTF8.GetByteCount(SendText);
            StatusMessage = "数据已发送（占位）。";
        }

        private void ClearReceive(object? parameter)
        {
            ReceiveDisplay.ClearCommand.Execute(null);
            StatusMessage = "接收区已清空。";
        }
    }
}
