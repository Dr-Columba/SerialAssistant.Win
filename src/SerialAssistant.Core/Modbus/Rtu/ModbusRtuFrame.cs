using SerialAssistant.Core.Modbus.Utilities;

namespace SerialAssistant.Core.Modbus.Rtu
{
    public sealed class ModbusRtuFrame
    {
        public byte SlaveAddress { get; }
        public byte FunctionCode { get; }
        public byte[] Data { get; }
        public ushort Crc { get; }

        public ModbusRtuFrame(byte slaveAddress, byte functionCode, byte[] data)
        {
            SlaveAddress = slaveAddress;
            FunctionCode = functionCode;
            Data = (byte[])data.Clone();

            byte[] frameWithoutCrc = new byte[2 + data.Length];
            frameWithoutCrc[0] = slaveAddress;
            frameWithoutCrc[1] = functionCode;
            Buffer.BlockCopy(data, 0, frameWithoutCrc, 2, data.Length);
            Crc = ModbusCrc16.Compute(frameWithoutCrc);
        }

        public byte[] ToByteArray()
        {
            byte[] frameWithoutCrc = new byte[2 + Data.Length];
            frameWithoutCrc[0] = SlaveAddress;
            frameWithoutCrc[1] = FunctionCode;
            Buffer.BlockCopy(Data, 0, frameWithoutCrc, 2, Data.Length);
            return ModbusCrc16.AppendCrc(frameWithoutCrc);
        }

        public override string ToString()
        {
            return $"Slave: {SlaveAddress}, Function: 0x{FunctionCode:X2}, Data: {Data.Length} bytes";
        }
    }
}
