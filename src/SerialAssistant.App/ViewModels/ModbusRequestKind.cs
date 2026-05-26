namespace SerialAssistant.App.ViewModels
{
    public enum ModbusRequestKind
    {
        ReadHoldingRegisters,
        ReadInputRegisters,
        WriteSingleRegister,
        WriteMultipleRegisters
    }
}