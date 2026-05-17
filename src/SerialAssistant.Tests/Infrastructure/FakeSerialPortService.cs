using SerialAssistant.Core.Enums;
using SerialAssistant.Core.Models;
using SerialAssistant.Core.Services;

namespace SerialAssistant.Tests.Infrastructure
{
    /*
     * Fake implementation of ISerialPortService for testing
     */
    public class FakeSerialPortService : ISerialPortService
    {
        private SerialConnectionState _connectionState;
        private readonly bool _shouldFailOpen;
        private readonly bool _shouldFailClose;
        private readonly string? _openErrorMessage;
        private readonly string? _closeErrorMessage;

        public FakeSerialPortService(
            bool shouldFailOpen = false,
            bool shouldFailClose = false,
            string? openErrorMessage = null,
            string? closeErrorMessage = null)
        {
            _connectionState = SerialConnectionState.Disconnected;
            _shouldFailOpen = shouldFailOpen;
            _shouldFailClose = shouldFailClose;
            _openErrorMessage = openErrorMessage ?? "Fake open error";
            _closeErrorMessage = closeErrorMessage ?? "Fake close error";
        }

        public SerialConnectionState ConnectionState
        {
            get => _connectionState;
            private set
            {
                if (_connectionState != value)
                {
                    _connectionState = value;
                    ConnectionStateChanged?.Invoke(this, value);
                }
            }
        }

        public OperationResult Open(SerialPortSettings settings)
        {
            if (_shouldFailOpen)
            {
                ConnectionState = SerialConnectionState.Faulted;
                return OperationResult.Failure(_openErrorMessage!);
            }

            ConnectionState = SerialConnectionState.Connected;
            return OperationResult.Success();
        }

        public OperationResult Close()
        {
            if (_shouldFailClose)
            {
                ConnectionState = SerialConnectionState.Faulted;
                return OperationResult.Failure(_closeErrorMessage!);
            }

            ConnectionState = SerialConnectionState.Disconnected;
            return OperationResult.Success();
        }

        public OperationResult Send(byte[] data)
        {
            return OperationResult.Failure("Send not implemented in fake");
        }

        public event EventHandler<SerialReceiveData>? DataReceived;

        public event EventHandler<Exception>? ErrorOccurred;

        public event EventHandler<SerialConnectionState>? ConnectionStateChanged;
    }
}
