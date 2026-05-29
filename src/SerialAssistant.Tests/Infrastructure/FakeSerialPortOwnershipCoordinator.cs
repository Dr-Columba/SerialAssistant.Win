using SerialAssistant.Core.Services;

namespace SerialAssistant.Tests.Infrastructure;

public class FakeSerialPortOwnershipCoordinator : ISerialPortOwnershipCoordinator
{
    private readonly Dictionary<string, SerialPortOwner> _portOwners = new Dictionary<string, SerialPortOwner>(StringComparer.OrdinalIgnoreCase);

    public event EventHandler<SerialPortOwnershipChangedEventArgs>? OwnershipChanged;

    public SerialPortOwner GetCurrentOwner(string portName)
    {
        var normalizedPortName = NormalizePortName(portName);
        if (normalizedPortName == null)
            return SerialPortOwner.None;

        return _portOwners.TryGetValue(normalizedPortName, out var owner) ? owner : SerialPortOwner.None;
    }

    public bool IsOwned(string portName)
    {
        return GetCurrentOwner(portName) != SerialPortOwner.None;
    }

    public bool IsOwnedBy(string portName, SerialPortOwner owner)
    {
        return GetCurrentOwner(portName) == owner;
    }

    public bool TryClaimOwnership(string portName, SerialPortOwner owner)
    {
        var normalizedPortName = NormalizePortName(portName);
        if (normalizedPortName == null)
            return false;

        if (owner == SerialPortOwner.None)
            return false;

        var currentOwner = GetCurrentOwner(portName);
        if (currentOwner == owner)
            return true;

        if (currentOwner != SerialPortOwner.None)
            return false;

        _portOwners[normalizedPortName] = owner;
        OnOwnershipChanged(normalizedPortName, SerialPortOwner.None, owner);
        return true;
    }

    public bool TryReleaseOwnership(string portName, SerialPortOwner owner)
    {
        var normalizedPortName = NormalizePortName(portName);
        if (normalizedPortName == null)
            return false;

        var currentOwner = GetCurrentOwner(portName);
        if (currentOwner != owner)
            return false;

        _portOwners.Remove(normalizedPortName);
        OnOwnershipChanged(normalizedPortName, owner, SerialPortOwner.None);
        return true;
    }

    private string? NormalizePortName(string portName)
    {
        if (string.IsNullOrWhiteSpace(portName))
            return null;

        return portName.Trim();
    }

    private void OnOwnershipChanged(string portName, SerialPortOwner previousOwner, SerialPortOwner currentOwner)
    {
        OwnershipChanged?.Invoke(this, new SerialPortOwnershipChangedEventArgs(portName, previousOwner, currentOwner));
    }
}
