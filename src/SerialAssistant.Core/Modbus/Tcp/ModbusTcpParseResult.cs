namespace SerialAssistant.Core.Modbus.Tcp
{
    public sealed class ModbusTcpParseResult
    {
        public bool IsSuccess { get; }
        public bool IsExceptionResponse { get; }
        public ModbusTcpErrorCode ErrorCode { get; }
        public string ErrorMessage { get; }
        public ushort TransactionId { get; }
        public byte UnitId { get; }
        public byte FunctionCode { get; }
        public byte? ExceptionCode { get; }
        public ushort? Address { get; }
        public ushort? Quantity { get; }
        public ushort? Value { get; }
        public IReadOnlyList<ushort>? Registers { get; }
        public byte[] RawFrame { get; }

        private ModbusTcpParseResult(
            bool isSuccess,
            bool isExceptionResponse,
            ModbusTcpErrorCode errorCode,
            string errorMessage,
            ushort transactionId,
            byte unitId,
            byte functionCode,
            byte? exceptionCode,
            ushort? address,
            ushort? quantity,
            ushort? value,
            IReadOnlyList<ushort>? registers,
            byte[] rawFrame)
        {
            IsSuccess = isSuccess;
            IsExceptionResponse = isExceptionResponse;
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
            TransactionId = transactionId;
            UnitId = unitId;
            FunctionCode = functionCode;
            ExceptionCode = exceptionCode;
            Address = address;
            Quantity = quantity;
            Value = value;
            Registers = registers;
            RawFrame = (byte[])rawFrame.Clone();
        }

        public static ModbusTcpParseResult Success(
            ushort transactionId,
            byte unitId,
            byte functionCode,
            ushort? address,
            ushort? quantity,
            ushort? value,
            IReadOnlyList<ushort>? registers,
            byte[] rawFrame)
        {
            return new ModbusTcpParseResult(
                true,
                false,
                ModbusTcpErrorCode.None,
                string.Empty,
                transactionId,
                unitId,
                functionCode,
                null,
                address,
                quantity,
                value,
                registers,
                rawFrame);
        }

        public static ModbusTcpParseResult ExceptionResponse(
            ushort transactionId,
            byte unitId,
            byte functionCode,
            byte exceptionCode,
            byte[] rawFrame)
        {
            return new ModbusTcpParseResult(
                false,
                true,
                ModbusTcpErrorCode.ExceptionResponse,
                $"Exception response: {exceptionCode}",
                transactionId,
                unitId,
                functionCode,
                exceptionCode,
                null,
                null,
                null,
                null,
                rawFrame);
        }

        public static ModbusTcpParseResult Failure(
            ModbusTcpErrorCode errorCode,
            string errorMessage,
            byte[]? rawFrame)
        {
            return new ModbusTcpParseResult(
                false,
                false,
                errorCode,
                errorMessage,
                0,
                0,
                0,
                null,
                null,
                null,
                null,
                null,
                rawFrame ?? Array.Empty<byte>());
        }
    }
}
