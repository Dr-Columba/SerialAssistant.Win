namespace SerialAssistant.Core.Modbus.Utilities
{
    public static class ModbusCrc16
    {
        private const ushort InitialValue = 0xFFFF;
        private const ushort Polynomial = 0xA001;

        public static ushort Compute(byte[] data)
        {
            if (data == null)
            {
                throw new System.ArgumentNullException(nameof(data));
            }
            return ComputeInternal(data);
        }

        public static ushort Compute(ReadOnlySpan<byte> data)
        {
            return ComputeInternal(data);
        }

        private static ushort ComputeInternal(ReadOnlySpan<byte> data)
        {
            ushort crc = InitialValue;
            foreach (byte b in data)
            {
                crc ^= b;
                for (int i = 0; i < 8; i++)
                {
                    if ((crc & 0x0001) != 0)
                    {
                        crc = (ushort)((crc >> 1) ^ Polynomial);
                    }
                    else
                    {
                        crc >>= 1;
                    }
                }
            }
            return crc;
        }

        public static byte LowByte(ushort value)
        {
            return (byte)(value & 0xFF);
        }

        public static byte HighByte(ushort value)
        {
            return (byte)(value >> 8);
        }

        public static byte[] AppendCrc(byte[] data)
        {
            if (data == null)
            {
                throw new System.ArgumentNullException(nameof(data));
            }
            ushort crc = Compute(data);
            byte[] result = new byte[data.Length + 2];
            for (int i = 0; i < data.Length; i++)
            {
                result[i] = data[i];
            }
            result[data.Length] = LowByte(crc);
            result[data.Length + 1] = HighByte(crc);
            return result;
        }

        public static bool Validate(byte[] data)
        {
            if (data == null)
            {
                throw new System.ArgumentNullException(nameof(data));
            }
            if (data.Length < 3)
            {
                return false;
            }
            ushort expectedCrc = Compute(data.AsSpan(0, data.Length - 2));
            ushort receivedCrc = (ushort)(data[data.Length - 1] << 8 | data[data.Length - 2]);
            return expectedCrc == receivedCrc;
        }
    }
}
