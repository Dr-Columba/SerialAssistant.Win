using SerialAssistant.Core.Modbus.Transport;
using SerialAssistant.Core.Services;

namespace SerialAssistant.Infrastructure.Modbus.Transport;

/* Factory for creating Modbus RTU transport with real serial adapter.
 * Implements IModbusRtuTransportFactory from Core layer.
 * Composes SystemIoPortsModbusRtuSerialAdapter, ModbusRtuTransport, and ownership coordinator.
 * Does NOT open serial port during creation.
 * Does NOT expose System.IO.Ports to App layer.
 * Does not depend on UI framework, network client APIs, or disk I/O APIs.
 */
public sealed class ModbusRtuTransportFactory : IModbusRtuTransportFactory
{
    private readonly ISerialPortOwnershipCoordinator _ownershipCoordinator;

    public ModbusRtuTransportFactory(ISerialPortOwnershipCoordinator ownershipCoordinator)
    {
        _ownershipCoordinator = ownershipCoordinator ?? throw new ArgumentNullException(nameof(ownershipCoordinator));
    }

    /* Creates a new Modbus RTU transport instance.
     * Does NOT open the serial port.
     * Does NOT perform any I/O operations.
     * Returns IModbusRtuTransport interface to hide implementation details.
     */
    public IModbusRtuTransport Create(ModbusRtuTransportFactoryOptions options)
    {
        if (options == null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        /* Validate options */
        var validationError = options.Validate();
        if (validationError != null)
        {
            throw new ArgumentException(validationError, nameof(options));
        }

        /* Create serial adapter with factory options */
        var adapter = new SystemIoPortsModbusRtuSerialAdapter(
            options.PortName,
            options.BaudRate,
            options.DataBits,
            options.Parity,
            options.StopBits,
            options.ReadTimeoutMilliseconds,
            options.WriteTimeoutMilliseconds);

        /* Create transport options from factory options */
        var transportOptions = new ModbusTransportOptions
        {
            ConnectTimeout = TimeSpan.FromMilliseconds(1000),
            SendTimeout = TimeSpan.FromMilliseconds(options.SendTimeoutMilliseconds),
            ReceiveTimeout = TimeSpan.FromMilliseconds(options.ReceiveTimeoutMilliseconds),
            ValidateResponse = true,
            MaxResponseBytes = 260
        };

        /* Create and return transport */
        var transport = new ModbusRtuTransport(
            options.PortName,
            adapter,
            _ownershipCoordinator,
            transportOptions);

        return transport;
    }
}