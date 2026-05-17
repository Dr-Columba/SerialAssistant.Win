namespace SerialAssistant.Core.Services
{
    /*
     * Interface for UI thread invocation
     */
    public interface IUiThreadInvoker
    {
        bool InvokeRequired
        {
            get;
        }

        void Invoke(Action action);
    }
}
