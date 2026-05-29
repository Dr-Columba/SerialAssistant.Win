namespace SerialAssistant.Core.Services;

public interface ISerialPortOwnershipCoordinator
{
    SerialPortOwner GetCurrentOwner(string portName);

    bool IsOwned(string portName);

    bool IsOwnedBy(string portName, SerialPortOwner owner);

    bool TryClaimOwnership(string portName, SerialPortOwner owner);

    bool TryReleaseOwnership(string portName, SerialPortOwner owner);

    event EventHandler<SerialPortOwnershipChangedEventArgs>? OwnershipChanged;
}
