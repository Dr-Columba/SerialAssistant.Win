using System.Windows;
using SerialAssistant.App.ViewModels;
using SerialAssistant.App.UI;
using SerialAssistant.Core.Services;
using SerialAssistant.Infrastructure.Serial;
using SerialAssistant.Infrastructure.Configuration;

namespace SerialAssistant.App
{
    /*
     * Application entry point for SerialAssistant.Win
     */
    public partial class App : Application
    {
        private MainWindowViewModel? _viewModel;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            ISerialPortScanner scanner = new SerialPortScanner();
            ISerialPortService serialPortService = new SerialPortService();
            IUiThreadInvoker uiThreadInvoker = new DispatcherUiThreadInvoker(Dispatcher);
            IAppSettingsService appSettingsService = new JsonAppSettingsService();
            _viewModel = new MainWindowViewModel(scanner, serialPortService, uiThreadInvoker, appSettingsService);

            MainWindow mainWindow = new MainWindow();
            mainWindow.DataContext = _viewModel;
            mainWindow.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            if (_viewModel != null)
            {
                _viewModel.SaveSettings();
            }
        }
    }
}
