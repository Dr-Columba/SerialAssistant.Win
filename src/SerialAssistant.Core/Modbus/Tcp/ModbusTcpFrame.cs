namespace SerialAssistant.Core.Modbus.Tcp
{
    public sealed class ModbusTcpFrame
    {
        public MbapHeader Header { get; }
        public byte FunctionCode { get; }
        public byte[] Data { get; }

        public ModbusTcpFrame(MbapHeader header, byte functionCode, byte[] data)
        {
            Header = header;
            FunctionCode = functionCode;
            Data = (byte[])data.Clone();
        }

        public byte[] ToByteArray()
        {
            byte[] headerBytes = Header.ToByteArray();
            byte[] frame = new byte[headerBytes.Length + 1 + Data.Length];
            Buffer.BlockCopy(headerBytes, 0, frame, 0, headerBytes.Length);
            frame[headerBytes.Length] = FunctionCode;
            Buffer.BlockCopy(Data, 0, frame, headerBytes.Length + 1, Data.Length);
            return frame;
        }

        public override string ToString()
        {
            return $"Transaction: {Header.TransactionId}, Unit: {Header.UnitId}, Function: 0x{FunctionCode:X2}, Length: {Header.Length}";
        }
    }
}
