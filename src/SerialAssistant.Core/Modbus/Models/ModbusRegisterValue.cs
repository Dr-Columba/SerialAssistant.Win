using SerialAssistant.Core.Modbus.Common;

namespace SerialAssistant.Core.Modbus.Models
{
    public sealed class ModbusRegisterValue
    {
        public ushort Address { get; }
        public ushort RawValue { get; }
        public ModbusDataType DataType { get; }

        public ModbusRegisterValue(ushort address, ushort rawValue, ModbusDataType dataType = ModbusDataType.UInt16)
        {
            Address = address;
            RawValue = rawValue;
            DataType = dataType;
        }

        public byte HighByte => (byte)(RawValue >> 8);

        public byte LowByte => (byte)(RawValue & 0xFF);

        public short SignedValue => (short)RawValue;

        public override string ToString()
        {
            return $"[{Address:X4}] = {RawValue:X4}";
        }
    }
}
