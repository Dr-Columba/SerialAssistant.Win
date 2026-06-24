namespace SerialAssistant.Core.Modbus.Transport;

/* Factory contract for creating Modbus RTU transport.
 * Defined in Core layer to allow ViewModel to depend on contract, not concrete factory.
 * Does NOT reference Infrastructure, System.IO.Ports, WPF, or disk I/O APIs.
 */
public interface IModbusRtuTransportFactory
{
    /* Creates a new Modbus RTU transport instance.
     * Does NOT open the serial port.
     * Does NOT perform any I/O operations.
     * Returns IModbusRtuTransport interface to hide implementation details.
     */
    IModbusRtuTransport Create(ModbusRtuTransportFactoryOptions options);
}