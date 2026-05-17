using System.Windows;
using SerialAssistant.App.ViewModels;
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
            MainWindowViewModel viewModel = new MainWindowViewModel(scanner);

            MainWindow mainWindow = new MainWindow();
            mainWindow.DataContext = viewModel;
            mainWindow.Show();
        }
    }
}
