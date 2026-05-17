using SerialAssistant.Core.Services;

namespace SerialAssistant.Tests.UI
{
    /*
     * Fake UI thread invoker for testing
     */
    public class FakeUiThreadInvoker : IUiThreadInvoker
    {
        public bool InvokeRequired => false;

        public void Invoke(Action action)
        {
            action?.Invoke();
        }
    }
}