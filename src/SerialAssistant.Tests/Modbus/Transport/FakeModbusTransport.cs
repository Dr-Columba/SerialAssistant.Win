using System.Collections.Generic;
using SerialAssistant.Core.Modbus.Transport;

namespace SerialAssistant.Tests.Modbus.Transport
{
    public class FakeModbusTransport : IModbusTransport
    {
        private readonly Queue<byte[]> _responseQueue;
        private readonly Queue<(ModbusTransportErrorCode ErrorCode, string Message)> _failureQueue;
        private readonly List<byte[]> _sentRequests;
        private readonly List<ModbusRequestContext> _sentContexts;

        public bool IsConnected { get; private set; }

        public ModbusTransportOptions Options { get; set; }

        public IReadOnlyList<byte[]> SentRequests => _sentRequests.AsReadOnly();

        public IReadOnlyList<ModbusRequestContext> SentContexts => _sentContexts.AsReadOnly();

        public FakeModbusTransport()
        {
            _responseQueue = new Queue<byte[]>();
            _failureQueue = new Queue<(ModbusTransportErrorCode, string)>();
            _sentRequests = new List<byte[]>();
            _sentContexts = new List<ModbusRequestContext>();
            IsConnected = false;
            Options = new ModbusTransportOptions();
        }

        public void QueueResponse(byte[] responseBytes)
        {
            if (responseBytes == null)
            {
                throw new ArgumentNullException(nameof(responseBytes));
            }

            var copy = new byte[responseBytes.Length];
            Array.Copy(responseBytes, copy, responseBytes.Length);
            _responseQueue.Enqueue(copy);
        }

        public void QueueFailure(ModbusTransportErrorCode errorCode, string message)
        {
            _failureQueue.Enqueue((errorCode, message ?? string.Empty));
        }

        public void ClearQueues()
        {
            _responseQueue.Clear();
            _failureQueue.Clear();
            _sentRequests.Clear();
            _sentContexts.Clear();
        }

        public Task<bool> ConnectAsync(CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return Task.FromResult(false);
            }

            IsConnected = true;
            return Task.FromResult(true);
        }

        public Task DisconnectAsync(CancellationToken cancellationToken = default)
        {
            IsConnected = false;
            return Task.CompletedTask;
        }

        public Task<ModbusTransportResult> SendRequestAsync(
            ModbusRequestContext context,
            byte[] requestBytes,
            CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return Task.FromResult(ModbusTransportResult.Failure(
                    ModbusTransportErrorCode.Unknown,
                    "Operation cancelled",
                    TimeSpan.Zero));
            }

            if (!IsConnected)
            {
                return Task.FromResult(ModbusTransportResult.Failure(
                    ModbusTransportErrorCode.NotConnected,
                    "Not connected",
                    TimeSpan.Zero));
            }

            var requestCopy = new byte[requestBytes.Length];
            Array.Copy(requestBytes, requestCopy, requestBytes.Length);
            _sentRequests.Add(requestCopy);

            _sentContexts.Add(context);

            if (_failureQueue.Count > 0)
            {
                var failure = _failureQueue.Dequeue();
                return Task.FromResult(ModbusTransportResult.Failure(
                    failure.ErrorCode,
                    failure.Message,
                    TimeSpan.FromMilliseconds(10)));
            }

            if (_responseQueue.Count > 0)
            {
                var response = _responseQueue.Dequeue();
                return Task.FromResult(ModbusTransportResult.SuccessResult(
                    response,
                    TimeSpan.FromMilliseconds(10)));
            }

            return Task.FromResult(ModbusTransportResult.Failure(
                ModbusTransportErrorCode.Timeout,
                "No response queued",
                TimeSpan.FromMilliseconds(10)));
        }
    }
}
