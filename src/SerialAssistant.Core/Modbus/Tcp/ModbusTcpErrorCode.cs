namespace SerialAssistant.Core.Modbus.Tcp
{
    public enum ModbusTcpErrorCode
    {
        None,
        NullFrame,
        InvalidLength,
        InvalidProtocolId,
        LengthMismatch,
        UnsupportedFunctionCode,
        InvalidByteCount,
        PayloadMismatch,
        ExceptionResponse
    }
}
