namespace SerialAssistant.Core.Modbus.Rtu
{
    public static class ModbusRtuRequestBuilder
    {
        private const byte ReadHoldingRegisters = 0x03;
        private const byte ReadInputRegisters = 0x04;
        private const byte WriteSingleRegister = 0x06;
        private const byte WriteMultipleRegisters = 0x10;

        public static ModbusRtuFrame BuildReadHoldingRegisters(byte slaveAddress, ushort startAddress, ushort quantity)
        {
            ValidateSlaveAddress(slaveAddress);
            ValidateReadQuantity(quantity);

            byte[] data = new byte[4];
            data[0] = (byte)(startAddress >> 8);
            data[1] = (byte)(startAddress & 0xFF);
            data[2] = (byte)(quantity >> 8);
            data[3] = (byte)(quantity & 0xFF);

            return new ModbusRtuFrame(slaveAddress, ReadHoldingRegisters, data);
        }

        public static ModbusRtuFrame BuildReadInputRegisters(byte slaveAddress, ushort startAddress, ushort quantity)
        {
            ValidateSlaveAddress(slaveAddress);
            ValidateReadQuantity(quantity);

            byte[] data = new byte[4];
            data[0] = (byte)(startAddress >> 8);
            data[1] = (byte)(startAddress & 0xFF);
            data[2] = (byte)(quantity >> 8);
            data[3] = (byte)(quantity & 0xFF);

            return new ModbusRtuFrame(slaveAddress, ReadInputRegisters, data);
        }

        public static ModbusRtuFrame BuildWriteSingleRegister(byte slaveAddress, ushort address, ushort value)
        {
            ValidateSlaveAddress(slaveAddress);

            byte[] data = new byte[4];
            data[0] = (byte)(address >> 8);
            data[1] = (byte)(address & 0xFF);
            data[2] = (byte)(value >> 8);
            data[3] = (byte)(value & 0xFF);

            return new ModbusRtuFrame(slaveAddress, WriteSingleRegister, data);
        }

        public static ModbusRtuFrame BuildWriteMultipleRegisters(byte slaveAddress, ushort startAddress, IReadOnlyList<ushort> values)
        {
            ValidateSlaveAddress(slaveAddress);
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

            return new ModbusRtuFrame(slaveAddress, WriteMultipleRegisters, data);
        }

        private static void ValidateSlaveAddress(byte slaveAddress)
        {
            if (slaveAddress < 1 || slaveAddress > 247)
            {
                throw new ArgumentOutOfRangeException(nameof(slaveAddress), "Slave address must be between 1 and 247");
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
