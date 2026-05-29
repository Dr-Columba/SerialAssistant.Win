namespace SerialAssistant.Core.Modbus.Transport
{
    public interface IModbusTcpTransport : IModbusTransport
    {
        string Host { get; }

        int Port { get; }
    }
}
