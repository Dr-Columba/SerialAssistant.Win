namespace SerialAssistant.Infrastructure.Modbus.Transport;

public interface IModbusRtuSerialAdapter
{
    bool IsOpen { get; }

    Task<bool> OpenAsync(CancellationToken cancellationToken = default);

    Task CloseAsync(CancellationToken cancellationToken = default);

    Task<bool> WriteAsync(byte[] requestBytes, CancellationToken cancellationToken = default);

    Task<byte[]> ReadAsync(int maxBytes, TimeSpan timeout, CancellationToken cancellationToken = default);
}