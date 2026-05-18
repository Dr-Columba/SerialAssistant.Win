using SerialAssistant.Core.Services;

namespace SerialAssistant.Tests.UI
{
    /*
     * Fake UI thread invoker for testing
     */
    public class FakeUiThreadInvoker : IUiThreadInvoker
    {
        public bool InvokeRequired => false;

        public int InvokeCount
        {
            get;
            private set;
        }

        public FakeUiThreadInvoker()
        {
            InvokeCount = 0;
        }

        public void Invoke(Action action)
        {
            InvokeCount++;
            action?.Invoke();
        }
    }
}