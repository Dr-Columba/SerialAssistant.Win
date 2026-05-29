namespace SerialAssistant.Core.Modbus.Transport
{
    public enum ModbusTransportErrorCode
    {
        None = 0,
        NotConnected = 1,
        PortNotSelected = 2,
        PortAlreadyInUse = 3,
        OpenFailed = 4,
        ConnectFailed = 5,
        SendFailed = 6,
        ReceiveFailed = 7,
        Timeout = 8,
        Disconnected = 9,
        InvalidResponse = 10,
        CrcInvalid = 11,
        TransactionMismatch = 12,
        UnsupportedFunction = 13,
        InvalidOptions = 14,
        Unknown = 99
    }
}
