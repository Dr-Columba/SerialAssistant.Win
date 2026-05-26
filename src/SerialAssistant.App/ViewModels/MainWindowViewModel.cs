using SerialAssistant.Core.Services;
using SerialAssistant.Core.Utilities;
using SerialAssistant.Core.Models;

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

        public OperationResult SaveSettings()
        {
            return Terminal.SaveSettings();
        }
    }
}
