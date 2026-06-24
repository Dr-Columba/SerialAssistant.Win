using SerialAssistant.Core.Services;

namespace SerialAssistant.Infrastructure.Services;

/* Real implementation of serial port ownership coordinator.
 * Tracks ownership per port name with thread-safe dictionary.
 * Does NOT reference System.IO.Ports, WPF, or file system.
 * Does NOT modify existing SerialPortService or ModbusRtuTransport.
 */
public sealed class SerialPortOwnershipCoordinator : ISerialPortOwnershipCoordinator
{
    private readonly Dictionary<string, SerialPortOwner> _ownershipMap;
    private readonly object _lock;

    public event EventHandler<SerialPortOwnershipChangedEventArgs>? OwnershipChanged;

    public SerialPortOwnershipCoordinator()
    {
        _ownershipMap = new Dictionary<string, SerialPortOwner>(StringComparer.OrdinalIgnoreCase);
        _lock = new object();
    }

    public SerialPortOwner GetCurrentOwner(string portName)
    {
        if (IsInvalidPortName(portName))
        {
            return SerialPortOwner.None;
        }

        lock (_lock)
        {
            var normalizedName = NormalizePortName(portName);
            if (_ownershipMap.TryGetValue(normalizedName, out var owner))
            {
                return owner;
            }

            return SerialPortOwner.None;
        }
    }

    public bool IsOwned(string portName)
    {
        if (IsInvalidPortName(portName))
        {
            return false;
        }

        lock (_lock)
        {
            var normalizedName = NormalizePortName(portName);
            if (_ownershipMap.TryGetValue(normalizedName, out var owner))
            {
                return owner != SerialPortOwner.None;
            }

            return false;
        }
    }

    public bool IsOwnedBy(string portName, SerialPortOwner owner)
    {
        if (IsInvalidPortName(portName))
        {
            return false;
        }

        if (owner == SerialPortOwner.None)
        {
            return false;
        }

        lock (_lock)
        {
            var normalizedName = NormalizePortName(portName);
            if (_ownershipMap.TryGetValue(normalizedName, out var currentOwner))
            {
                return currentOwner == owner;
            }

            return false;
        }
    }

    public bool TryClaimOwnership(string portName, SerialPortOwner owner)
    {
        if (IsInvalidPortName(portName))
        {
            return false;
        }

        if (owner == SerialPortOwner.None)
        {
            return false;
        }

        lock (_lock)
        {
            var normalizedName = NormalizePortName(portName);
            var previousOwner = GetCurrentOwnerInternal(normalizedName);

            /* Idempotent: same owner can claim again */
            if (previousOwner == owner)
            {
                return true;
            }

            /* Port is owned by different owner */
            if (previousOwner != SerialPortOwner.None)
            {
                return false;
            }

            /* Claim ownership */
            _ownershipMap[normalizedName] = owner;

            /* Raise event only when owner actually changes */
            if (previousOwner != owner)
            {
                RaiseOwnershipChanged(normalizedName, previousOwner, owner);
            }

            return true;
        }
    }

    public bool TryReleaseOwnership(string portName, SerialPortOwner owner)
    {
        if (IsInvalidPortName(portName))
        {
            return false;
        }

        if (owner == SerialPortOwner.None)
        {
            return false;
        }

        lock (_lock)
        {
            var normalizedName = NormalizePortName(portName);
            var currentOwner = GetCurrentOwnerInternal(normalizedName);

            /* Port is not owned */
            if (currentOwner == SerialPortOwner.None)
            {
                return false;
            }

            /* Owner mismatch */
            if (currentOwner != owner)
            {
                return false;
            }

            /* Release ownership */
            _ownershipMap[normalizedName] = SerialPortOwner.None;

            /* Raise event */
            RaiseOwnershipChanged(normalizedName, currentOwner, SerialPortOwner.None);

            return true;
        }
    }

    private static bool IsInvalidPortName(string portName)
    {
        return string.IsNullOrWhiteSpace(portName);
    }

    private static string NormalizePortName(string portName)
    {
        return portName.Trim().ToUpperInvariant();
    }

    private SerialPortOwner GetCurrentOwnerInternal(string normalizedName)
    {
        if (_ownershipMap.TryGetValue(normalizedName, out var owner))
        {
            return owner;
        }

        return SerialPortOwner.None;
    }

    private void RaiseOwnershipChanged(string portName, SerialPortOwner previousOwner, SerialPortOwner currentOwner)
    {
        var eventArgs = new SerialPortOwnershipChangedEventArgs(portName, previousOwner, currentOwner);
        OwnershipChanged?.Invoke(this, eventArgs);
    }
}