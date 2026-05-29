namespace SerialAssistant.Core.Services;

public class SerialPortOwnershipChangedEventArgs : EventArgs
{
    public string PortName { get; }

    public SerialPortOwner PreviousOwner { get; }

    public SerialPortOwner CurrentOwner { get; }

    public DateTimeOffset ChangedAt { get; }

    public SerialPortOwnershipChangedEventArgs(string portName, SerialPortOwner previousOwner, SerialPortOwner currentOwner)
        : this(portName, previousOwner, currentOwner, DateTimeOffset.UtcNow)
    {
    }

    public SerialPortOwnershipChangedEventArgs(string portName, SerialPortOwner previousOwner, SerialPortOwner currentOwner, DateTimeOffset changedAt)
    {
        PortName = (portName ?? string.Empty).Trim();
        PreviousOwner = previousOwner;
        CurrentOwner = currentOwner;
        ChangedAt = changedAt;
    }
}
