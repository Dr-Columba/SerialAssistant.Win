using System.Windows.Threading;
using SerialAssistant.Core.Services;

namespace SerialAssistant.App.UI
{
    /*
     * WPF Dispatcher-based UI thread invoker
     */
    public class DispatcherUiThreadInvoker : IUiThreadInvoker
    {
        private readonly Dispatcher _dispatcher;

        public DispatcherUiThreadInvoker(Dispatcher dispatcher)
        {
            _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
        }

        public bool InvokeRequired => !_dispatcher.CheckAccess();

        public void Invoke(Action action)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            if (InvokeRequired)
            {
                _dispatcher.Invoke(action);
            }
            else
            {
                action();
            }
        }
    }
}
