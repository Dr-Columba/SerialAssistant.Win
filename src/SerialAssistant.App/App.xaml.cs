using System.Windows;
using SerialAssistant.App.ViewModels;
using SerialAssistant.App.UI;
using SerialAssistant.Core.Services;
using SerialAssistant.Infrastructure.Serial;

namespace SerialAssistant.App
{
    /*
     * Application entry point for SerialAssistant.Win
     */
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            ISerialPortScanner scanner = new SerialPortScanner();
            ISerialPortService serialPortService = new SerialPortService();
            IUiThreadInvoker uiThreadInvoker = new DispatcherUiThreadInvoker(Dispatcher);
            MainWindowViewModel viewModel = new MainWindowViewModel(scanner, serialPortService, uiThreadInvoker);

            MainWindow mainWindow = new MainWindow();
            mainWindow.DataContext = viewModel;
            mainWindow.Show();
        }
    }
}
