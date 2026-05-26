namespace SerialAssistant.Core.Modbus.Tcp
{
    public static class ModbusTcpRequestBuilder
    {
        private const ushort DefaultProtocolId = 0x0000;
        private const byte ReadHoldingRegisters = 0x03;
        private const byte ReadInputRegisters = 0x04;
        private const byte WriteSingleRegister = 0x06;
        private const byte WriteMultipleRegisters = 0x10;

        public static ModbusTcpFrame BuildReadHoldingRegisters(
            ushort transactionId,
            byte unitId,
            ushort startAddress,
            ushort quantity)
        {
            ValidateUnitId(unitId);
            ValidateReadQuantity(quantity);

            byte[] data = new byte[4];
            data[0] = (byte)(startAddress >> 8);
            data[1] = (byte)(startAddress & 0xFF);
            data[2] = (byte)(quantity >> 8);
            data[3] = (byte)(quantity & 0xFF);

            ushort length = (ushort)(1 + 1 + data.Length);
            MbapHeader header = new MbapHeader(transactionId, DefaultProtocolId, length, unitId);
            return new ModbusTcpFrame(header, ReadHoldingRegisters, data);
        }

        public static ModbusTcpFrame BuildReadInputRegisters(
            ushort transactionId,
            byte unitId,
            ushort startAddress,
            ushort quantity)
        {
            ValidateUnitId(unitId);
            ValidateReadQuantity(quantity);

            byte[] data = new byte[4];
            data[0] = (byte)(startAddress >> 8);
            data[1] = (byte)(startAddress & 0xFF);
            data[2] = (byte)(quantity >> 8);
            data[3] = (byte)(quantity & 0xFF);

            ushort length = (ushort)(1 + 1 + data.Length);
            MbapHeader header = new MbapHeader(transactionId, DefaultProtocolId, length, unitId);
            return new ModbusTcpFrame(header, ReadInputRegisters, data);
        }

        public static ModbusTcpFrame BuildWriteSingleRegister(
            ushort transactionId,
            byte unitId,
            ushort address,
            ushort value)
        {
            ValidateUnitId(unitId);

            byte[] data = new byte[4];
            data[0] = (byte)(address >> 8);
            data[1] = (byte)(address & 0xFF);
            data[2] = (byte)(value >> 8);
            data[3] = (byte)(value & 0xFF);

            ushort length = (ushort)(1 + 1 + data.Length);
            MbapHeader header = new MbapHeader(transactionId, DefaultProtocolId, length, unitId);
            return new ModbusTcpFrame(header, WriteSingleRegister, data);
        }

        public static ModbusTcpFrame BuildWriteMultipleRegisters(
            ushort transactionId,
            byte unitId,
            ushort startAddress,
            IReadOnlyList<ushort> values)
        {
            ValidateUnitId(unitId);
            ValidateWriteMultipleValues(values);

            ushort quantity = (ushort)values.Count;
            byte byteCount = (byte)(quantity * 2);
            byte[] data = new byte[5 + byteCount];
            data[0] = (byte)(startAddress >> 8);
            data[1] = (byte)(startAddress & 0xFF);
            data[2] = (byte)(quantity >> 8);
            data[3] = (byte)(quantity & 0xFF);
            data[4] = byteCount;

            for (int i = 0; i < values.Count; i++)
            {
                data[5 + i * 2] = (byte)(values[i] >> 8);
                data[6 + i * 2] = (byte)(values[i] & 0xFF);
            }

            ushort length = (ushort)(1 + 1 + data.Length);
            MbapHeader header = new MbapHeader(transactionId, DefaultProtocolId, length, unitId);
            return new ModbusTcpFrame(header, WriteMultipleRegisters, data);
        }

        private static void ValidateUnitId(byte unitId)
        {
            if (unitId < 1 || unitId > 247)
            {
                throw new ArgumentOutOfRangeException(nameof(unitId), "UnitId must be between 1 and 247");
            }
        }

        private static void ValidateReadQuantity(ushort quantity)
        {
            if (quantity == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be greater than 0");
            }
            if (quantity > 125)
            {
                throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be 125 or less");
            }
        }

        private static void ValidateWriteMultipleValues(IReadOnlyList<ushort> values)
        {
            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }
            if (values.Count == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(values), "Values count must be greater than 0");
            }
            if (values.Count > 123)
            {
                throw new ArgumentOutOfRangeException(nameof(values), "Values count must be 123 or less");
            }
        }
    }
}
