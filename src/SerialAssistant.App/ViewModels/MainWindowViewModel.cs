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
    /*
     * Main ViewModel for the application window
     */
    public class MainWindowViewModel : BaseViewModel
    {
        private readonly ISerialPortScanner? _scanner;
        private readonly ISerialPortService? _serialPortService;
        private readonly IUiThreadInvoker? _uiThreadInvoker;
        private readonly IAppSettingsService? _appSettingsService;
        private AppSettings _lastLoadedSettings;
        private SerialConnectionState _connectionState;
        private string _sendText;
        private SendMode _selectedSendMode;
        private string _statusMessage;
        private int _sentBytesCount;
        private bool _isHexDisplay;
        private string _connectionButtonText;

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
            _lastLoadedSettings = AppSettings.CreateDefault();
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
                _serialPortService.DataReceived += OnDataReceived;
                _serialPortService.ErrorOccurred += OnErrorOccurred;
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

            LoadSettings();
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
            set
            {
                if (SetProperty(ref _sendText, value))
                {
                    (SendCommand as RelayCommand)?.RaiseCanExecuteChanged();
                }
            }
        }

        public ObservableCollection<SendMode> SendModes
        {
            get;
            private set;
        }

        public SendMode SelectedSendMode
        {
            get => _selectedSendMode;
            set
            {
                if (SetProperty(ref _selectedSendMode, value))
                {
                    (SendCommand as RelayCommand)?.RaiseCanExecuteChanged();
                }
            }
        }

        public SerialConnectionState ConnectionState
        {
            get => _connectionState;
            private set
            {
                if (SetProperty(ref _connectionState, value))
                {
                    (SendCommand as RelayCommand)?.RaiseCanExecuteChanged();
                }
            }
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

                    if (count > 0 && !string.IsNullOrEmpty(_lastLoadedSettings.LastPortName))
                    {
                        bool found = false;
                        foreach (var port in result.Value)
                        {
                            if (port.PortName == _lastLoadedSettings.LastPortName)
                            {
                                SerialSettings.SelectedPortName = _lastLoadedSettings.LastPortName;
                                found = true;
                                break;
                            }
                        }
                        if (!found)
                        {
                            SerialSettings.SelectedPortName = result.Value[0].PortName;
                        }
                    }

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
            if (_serialPortService == null)
            {
                return false;
            }

            if (ConnectionState != SerialConnectionState.Connected)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(SendText))
            {
                return false;
            }

            return true;
        }

        private void Send(object? parameter)
        {
            if (_serialPortService == null)
            {
                StatusMessage = "当前阶段尚未接入串口服务。";
                return;
            }

            if (ConnectionState != SerialConnectionState.Connected)
            {
                StatusMessage = "串口未打开，无法发送。";
                return;
            }

            if (string.IsNullOrWhiteSpace(SendText))
            {
                StatusMessage = "发送内容不能为空。";
                return;
            }

            byte[] data;

            if (SelectedSendMode == SendMode.Text)
            {
                data = Encoding.UTF8.GetBytes(SendText);
            }
            else
            {
                var hexResult = HexConverter.FromHexString(SendText);
                if (!hexResult.IsSuccess)
                {
                    StatusMessage = $"HEX 格式错误：{hexResult.ErrorMessage}";
                    return;
                }

                data = hexResult.Value!;

                if (data.Length == 0)
                {
                    StatusMessage = "发送内容不能为空。";
                    return;
                }
            }

            var sendResult = _serialPortService.Send(data);

            if (sendResult.IsSuccess)
            {
                SentBytesCount += data.Length;
                StatusMessage = $"已发送 {data.Length} 字节。";
            }
            else
            {
                StatusMessage = $"发送失败：{sendResult.ErrorMessage}";
            }
        }

        private void ClearReceive(object? parameter)
        {
            ReceiveDisplay.ClearCommand.Execute(null);
            StatusMessage = "接收区已清空。";
        }

        private void OnDataReceived(object? sender, Core.Models.SerialReceiveData e)
        {
            Action updateAction = () =>
            {
                ReceiveDisplay.AddReceivedData(e.Data);
                StatusMessage = $"已接收 {e.Data.Length} 字节。";
            };

            if (_uiThreadInvoker != null)
            {
                _uiThreadInvoker.Invoke(updateAction);
            }
            else
            {
                updateAction();
            }
        }

        private void OnErrorOccurred(object? sender, Exception e)
        {
            Action updateAction = () =>
            {
                StatusMessage = $"接收串口数据失败：{e.Message}";
            };

            if (_uiThreadInvoker != null)
            {
                _uiThreadInvoker.Invoke(updateAction);
            }
            else
            {
                updateAction();
            }
        }

        private void LoadSettings()
        {
            if (_appSettingsService == null)
            {
                return;
            }

            OperationResult<AppSettings> loadResult = _appSettingsService.Load();
            if (loadResult.IsSuccess && loadResult.Value != null)
            {
                _lastLoadedSettings = loadResult.Value;
                ApplySettings(_lastLoadedSettings);
            }
        }

        private void ApplySettings(AppSettings settings)
        {
            if (settings == null)
            {
                return;
            }

            SerialSettings.SelectedBaudRate = settings.BaudRate;
            SerialSettings.SelectedDataBits = settings.DataBits;
            SerialSettings.SelectedParity = settings.Parity;
            SerialSettings.SelectedStopBits = settings.StopBits;
            SelectedSendMode = settings.SendMode;
            ReceiveDisplay.IsHexDisplay = (settings.DisplayMode == DisplayMode.Hex);
            _lastLoadedSettings = settings;
        }

        public OperationResult SaveSettings()
        {
            if (_appSettingsService == null)
            {
                return OperationResult.Failure("配置服务不可用。");
            }

            AppSettings settings = new AppSettings
            {
                LastPortName = SerialSettings.SelectedPortName ?? string.Empty,
                BaudRate = SerialSettings.SelectedBaudRate,
                DataBits = SerialSettings.SelectedDataBits,
                Parity = SerialSettings.SelectedParity ?? "None",
                StopBits = SerialSettings.SelectedStopBits ?? "One",
                SendMode = SelectedSendMode,
                DisplayMode = ReceiveDisplay.IsHexDisplay ? DisplayMode.Hex : DisplayMode.Text
            };

            return _appSettingsService.Save(settings);
        }
    }
}
