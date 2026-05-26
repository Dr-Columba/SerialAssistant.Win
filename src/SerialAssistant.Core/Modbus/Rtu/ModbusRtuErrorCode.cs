namespace SerialAssistant.Core.Modbus.Rtu
{
    public enum ModbusRtuErrorCode
    {
        None,
        NullFrame,
        InvalidLength,
        InvalidCrc,
        UnsupportedFunctionCode,
        InvalidByteCount,
        PayloadMismatch,
        ExceptionResponse
    }
}
