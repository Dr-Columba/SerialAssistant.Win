namespace SerialAssistant.Core.Modbus.Tcp
{
    public sealed class MbapHeader
    {
        public ushort TransactionId { get; }
        public ushort ProtocolId { get; }
        public ushort Length { get; }
        public byte UnitId { get; }

        public MbapHeader(ushort transactionId, ushort protocolId, ushort length, byte unitId)
        {
            TransactionId = transactionId;
            ProtocolId = protocolId;
            Length = length;
            UnitId = unitId;
        }

        public byte[] ToByteArray()
        {
            byte[] header = new byte[7];
            header[0] = (byte)(TransactionId >> 8);
            header[1] = (byte)(TransactionId & 0xFF);
            header[2] = (byte)(ProtocolId >> 8);
            header[3] = (byte)(ProtocolId & 0xFF);
            header[4] = (byte)(Length >> 8);
            header[5] = (byte)(Length & 0xFF);
            header[6] = UnitId;
            return header;
        }

        public static MbapHeader Parse(byte[] data)
        {
            if (data == null || data.Length < 7)
            {
                throw new ArgumentException("Data must contain at least 7 bytes", nameof(data));
            }
            ushort transactionId = (ushort)((data[0] << 8) | data[1]);
            ushort protocolId = (ushort)((data[2] << 8) | data[3]);
            ushort length = (ushort)((data[4] << 8) | data[5]);
            byte unitId = data[6];
            return new MbapHeader(transactionId, protocolId, length, unitId);
        }

        public override string ToString()
        {
            return $"TransactionId: {TransactionId}, ProtocolId: {ProtocolId}, Length: {Length}, UnitId: {UnitId}";
        }
    }
}
