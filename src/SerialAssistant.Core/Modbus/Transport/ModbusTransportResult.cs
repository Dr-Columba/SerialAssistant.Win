namespace SerialAssistant.Core.Modbus.Transport
{
    public sealed class ModbusTransportResult
    {
        public bool Success { get; private set; }

        public byte[]? ResponseBytes { get; private set; }

        public ModbusTransportErrorCode ErrorCode { get; private set; }

        public string ErrorMessage { get; private set; }

        public TimeSpan Duration { get; private set; }

        public DateTimeOffset CompletedAt { get; private set; }

        private ModbusTransportResult()
        {
            ErrorMessage = string.Empty;
        }

        public static ModbusTransportResult SuccessResult(byte[] responseBytes, TimeSpan duration)
        {
            if (responseBytes == null)
            {
                throw new ArgumentNullException(nameof(responseBytes));
            }

            var result = new ModbusTransportResult
            {
                Success = true,
                ErrorCode = ModbusTransportErrorCode.None,
                ErrorMessage = string.Empty,
                Duration = duration,
                CompletedAt = DateTimeOffset.UtcNow
            };

            result.ResponseBytes = new byte[responseBytes.Length];
            Array.Copy(responseBytes, result.ResponseBytes, responseBytes.Length);

            return result;
        }

        public static ModbusTransportResult Failure(ModbusTransportErrorCode errorCode, string errorMessage, TimeSpan duration)
        {
            if (string.IsNullOrEmpty(errorMessage))
            {
                errorMessage = string.Empty;
            }

            return new ModbusTransportResult
            {
                Success = false,
                ErrorCode = errorCode,
                ErrorMessage = errorMessage,
                Duration = duration,
                CompletedAt = DateTimeOffset.UtcNow
            };
        }

        public byte[]? GetResponseBytesCopy()
        {
            if (ResponseBytes == null || ResponseBytes.Length == 0)
            {
                return null;
            }

            var copy = new byte[ResponseBytes.Length];
            Array.Copy(ResponseBytes, copy, ResponseBytes.Length);
            return copy;
        }
    }
}
