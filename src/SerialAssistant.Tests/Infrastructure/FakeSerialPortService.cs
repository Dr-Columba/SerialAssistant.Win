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
        private readonly bool _shouldFailSend;
        private readonly string? _openErrorMessage;
        private readonly string? _closeErrorMessage;
        private readonly string? _sendErrorMessage;

        public List<byte[]> SentData { get; private set; } = new List<byte[]>();

        public FakeSerialPortService(
            bool shouldFailOpen = false,
            bool shouldFailClose = false,
            bool shouldFailSend = false,
            string? openErrorMessage = null,
            string? closeErrorMessage = null,
            string? sendErrorMessage = null)
        {
            _connectionState = SerialConnectionState.Disconnected;
            _shouldFailOpen = shouldFailOpen;
            _shouldFailClose = shouldFailClose;
            _shouldFailSend = shouldFailSend;
            _openErrorMessage = openErrorMessage ?? "Fake open error";
            _closeErrorMessage = closeErrorMessage ?? "Fake close error";
            _sendErrorMessage = sendErrorMessage ?? "Fake send error";
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
            if (_shouldFailSend)
            {
                return OperationResult.Failure(_sendErrorMessage!);
            }

            if (ConnectionState != SerialConnectionState.Connected)
            {
                return OperationResult.Failure("串口未打开，无法发送。");
            }

            SentData.Add((byte[])data.Clone());
            return OperationResult.Success();
        }

        public void SimulateDataReceived(byte[] data)
        {
            if (ConnectionState == SerialConnectionState.Connected)
            {
                SerialReceiveData receiveData = SerialReceiveData.Create(data);
                DataReceived?.Invoke(this, receiveData);
            }
        }

        public void SimulateErrorOccurred(Exception ex)
        {
            ErrorOccurred?.Invoke(this, ex);
        }

        public event EventHandler<SerialReceiveData>? DataReceived;

        public event EventHandler<Exception>? ErrorOccurred;

        public event EventHandler<SerialConnectionState>? ConnectionStateChanged;
    }
}
