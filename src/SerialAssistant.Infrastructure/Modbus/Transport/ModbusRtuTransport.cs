using SerialAssistant.Core.Modbus.Transport;
using SerialAssistant.Core.Modbus.Utilities;
using SerialAssistant.Core.Services;

namespace SerialAssistant.Infrastructure.Modbus.Transport;

public class ModbusRtuTransport : IModbusRtuTransport
{
    private readonly string _portName;
    private readonly IModbusRtuSerialAdapter _adapter;
    private readonly ISerialPortOwnershipCoordinator _ownershipCoordinator;
    private readonly ModbusTransportOptions _options;
    private bool _isConnected;

    public bool IsConnected => _isConnected;

    public ModbusTransportOptions Options => _options;

    public ModbusRtuTransport(
        string portName,
        IModbusRtuSerialAdapter adapter,
        ISerialPortOwnershipCoordinator ownershipCoordinator,
        ModbusTransportOptions? options = null)
    {
        _portName = portName;
        _adapter = adapter ?? throw new ArgumentNullException(nameof(adapter));
        _ownershipCoordinator = ownershipCoordinator ?? throw new ArgumentNullException(nameof(ownershipCoordinator));
        _options = options ?? new ModbusTransportOptions();
        _isConnected = false;
    }

    public async Task<bool> ConnectAsync(CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_portName))
        {
            return false;
        }

        if (!_ownershipCoordinator.TryClaimOwnership(_portName, SerialPortOwner.ModbusRtu))
        {
            return false;
        }

        try
        {
            if (!await _adapter.OpenAsync(cancellationToken))
            {
                _ownershipCoordinator.TryReleaseOwnership(_portName, SerialPortOwner.ModbusRtu);
                return false;
            }

            _isConnected = true;
            return true;
        }
        catch
        {
            _ownershipCoordinator.TryReleaseOwnership(_portName, SerialPortOwner.ModbusRtu);
            return false;
        }
    }

    public async Task DisconnectAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await _adapter.CloseAsync(cancellationToken);
        }
        catch
        {
            // Ignore adapter close exceptions, we're disconnecting anyway
        }
        finally
        {
            _ownershipCoordinator.TryReleaseOwnership(_portName, SerialPortOwner.ModbusRtu);
            _isConnected = false;
        }
    }

    public async Task<ModbusTransportResult> SendRequestAsync(
        ModbusRequestContext context,
        byte[] requestBytes,
        CancellationToken cancellationToken = default)
    {
        var startTime = DateTimeOffset.UtcNow;

        if (!_isConnected)
        {
            return ModbusTransportResult.Failure(
                ModbusTransportErrorCode.NotConnected,
                "Not connected",
                DateTimeOffset.UtcNow - startTime);
        }

        if (requestBytes == null || requestBytes.Length == 0)
        {
            return ModbusTransportResult.Failure(
                ModbusTransportErrorCode.InvalidOptions,
                "Request bytes is empty",
                DateTimeOffset.UtcNow - startTime);
        }

        if (!context.IsValid())
        {
            return ModbusTransportResult.Failure(
                ModbusTransportErrorCode.InvalidOptions,
                "Invalid request context",
                DateTimeOffset.UtcNow - startTime);
        }

        try
        {
            if (!await _adapter.WriteAsync(requestBytes, cancellationToken))
            {
                return ModbusTransportResult.Failure(
                    ModbusTransportErrorCode.SendFailed,
                    "Write failed",
                    DateTimeOffset.UtcNow - startTime);
            }

            byte[] responseBytes = await _adapter.ReadAsync(
                _options.MaxResponseBytes,
                _options.ReceiveTimeout,
                cancellationToken);

            var duration = DateTimeOffset.UtcNow - startTime;

            if (responseBytes == null || responseBytes.Length == 0)
            {
                return ModbusTransportResult.Failure(
                    ModbusTransportErrorCode.ReceiveFailed,
                    "Empty response",
                    duration);
            }

            if (_options.ValidateResponse)
            {
                if (responseBytes.Length < 3)
                {
                    return ModbusTransportResult.Failure(
                        ModbusTransportErrorCode.InvalidResponse,
                        "Response too short",
                        duration);
                }

                if (!ModbusCrc16.Validate(responseBytes))
                {
                    return ModbusTransportResult.Failure(
                        ModbusTransportErrorCode.CrcInvalid,
                        "CRC validation failed",
                        duration);
                }
            }

            return ModbusTransportResult.SuccessResult(responseBytes, duration);
        }
        catch (OperationCanceledException)
        {
            return ModbusTransportResult.Failure(
                ModbusTransportErrorCode.Timeout,
                "Operation canceled",
                DateTimeOffset.UtcNow - startTime);
        }
        catch (TimeoutException)
        {
            return ModbusTransportResult.Failure(
                ModbusTransportErrorCode.Timeout,
                "Receive timeout",
                DateTimeOffset.UtcNow - startTime);
        }
        catch
        {
            return ModbusTransportResult.Failure(
                ModbusTransportErrorCode.ReceiveFailed,
                "Receive failed",
                DateTimeOffset.UtcNow - startTime);
        }
    }
}