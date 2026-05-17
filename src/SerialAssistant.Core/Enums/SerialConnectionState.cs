namespace SerialAssistant.Core.Enums
{
    /*
     * Represents the connection state of a serial port
     */
    public enum SerialConnectionState
    {
        Disconnected = 0,
        Connecting = 1,
        Connected = 2,
        Disconnecting = 3,
        Faulted = 4
    }
}
