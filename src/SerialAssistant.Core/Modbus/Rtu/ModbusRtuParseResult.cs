namespace SerialAssistant.Core.Modbus.Rtu
{
    public sealed class ModbusRtuParseResult
    {
        public bool IsSuccess { get; }
        public bool IsExceptionResponse { get; }
        public ModbusRtuErrorCode ErrorCode { get; }
        public string ErrorMessage { get; }
        public byte SlaveAddress { get; }
        public byte FunctionCode { get; }
        public byte? ExceptionCode { get; }
        public ushort? Address { get; }
        public ushort? Quantity { get; }
        public ushort? Value { get; }
        public IReadOnlyList<ushort>? Registers { get; }
        public byte[] RawFrame { get; }

        private ModbusRtuParseResult(
            bool isSuccess,
            bool isExceptionResponse,
            ModbusRtuErrorCode errorCode,
            string errorMessage,
            byte slaveAddress,
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
            SlaveAddress = slaveAddress;
            FunctionCode = functionCode;
            ExceptionCode = exceptionCode;
            Address = address;
            Quantity = quantity;
            Value = value;
            Registers = registers;
            RawFrame = (byte[])rawFrame.Clone();
        }

        public static ModbusRtuParseResult Success(
            byte slaveAddress,
            byte functionCode,
            ushort? address,
            ushort? quantity,
            ushort? value,
            IReadOnlyList<ushort>? registers,
            byte[] rawFrame)
        {
            return new ModbusRtuParseResult(
                true,
                false,
                ModbusRtuErrorCode.None,
                string.Empty,
                slaveAddress,
                functionCode,
                null,
                address,
                quantity,
                value,
                registers,
                rawFrame);
        }

        public static ModbusRtuParseResult ExceptionResponse(
            byte slaveAddress,
            byte functionCode,
            byte exceptionCode,
            byte[] rawFrame)
        {
            return new ModbusRtuParseResult(
                false,
                true,
                ModbusRtuErrorCode.ExceptionResponse,
                $"Exception response: {exceptionCode}",
                slaveAddress,
                functionCode,
                exceptionCode,
                null,
                null,
                null,
                null,
                rawFrame);
        }

        public static ModbusRtuParseResult Failure(
            ModbusRtuErrorCode errorCode,
            string errorMessage,
            byte[]? rawFrame)
        {
            return new ModbusRtuParseResult(
                false,
                false,
                errorCode,
                errorMessage,
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
