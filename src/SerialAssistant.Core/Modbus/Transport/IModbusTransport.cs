namespace SerialAssistant.Core.Modbus.Transport
{
    public interface IModbusTransport
    {
        bool IsConnected { get; }

        ModbusTransportOptions Options { get; }

        Task<bool> ConnectAsync(CancellationToken cancellationToken = default);

        Task DisconnectAsync(CancellationToken cancellationToken = default);

        Task<ModbusTransportResult> SendRequestAsync(
            ModbusRequestContext context,
            byte[] requestBytes,
            CancellationToken cancellationToken = default);
    }
}
