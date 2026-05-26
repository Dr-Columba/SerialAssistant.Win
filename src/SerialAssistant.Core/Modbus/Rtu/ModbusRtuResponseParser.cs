using SerialAssistant.Core.Modbus.Utilities;

namespace SerialAssistant.Core.Modbus.Rtu
{
    public static class ModbusRtuResponseParser
    {
        private const byte ReadHoldingRegisters = 0x03;
        private const byte ReadInputRegisters = 0x04;
        private const byte WriteSingleRegister = 0x06;
        private const byte WriteMultipleRegisters = 0x10;
        private const byte ExceptionBit = 0x80;

        public static ModbusRtuParseResult Parse(byte[]? frame)
        {
            if (frame == null)
            {
                return ModbusRtuParseResult.Failure(
                    ModbusRtuErrorCode.NullFrame,
                    "Frame cannot be null",
                    null);
            }

            if (frame.Length < 5)
            {
                return ModbusRtuParseResult.Failure(
                    ModbusRtuErrorCode.InvalidLength,
                    $"Frame length {frame.Length} is less than minimum 5",
                    frame);
            }

            if (!ModbusCrc16.Validate(frame))
            {
                return ModbusRtuParseResult.Failure(
                    ModbusRtuErrorCode.InvalidCrc,
                    "CRC validation failed",
                    frame);
            }

            byte slaveAddress = frame[0];
            byte functionCode = frame[1];

            if ((functionCode & ExceptionBit) != 0)
            {
                byte exceptionCode = frame[2];
                return ModbusRtuParseResult.ExceptionResponse(
                    slaveAddress,
                    (byte)(functionCode & 0x7F),
                    exceptionCode,
                    frame);
            }

            switch (functionCode)
            {
                case ReadHoldingRegisters:
                case ReadInputRegisters:
                    return ParseReadResponse(frame, slaveAddress, functionCode);

                case WriteSingleRegister:
                    return ParseWriteSingleRegisterResponse(frame, slaveAddress, functionCode);

                case WriteMultipleRegisters:
                    return ParseWriteMultipleRegistersResponse(frame, slaveAddress, functionCode);

                default:
                    return ModbusRtuParseResult.Failure(
                        ModbusRtuErrorCode.UnsupportedFunctionCode,
                        $"Unsupported function code: 0x{functionCode:X2}",
                        frame);
            }
        }

        private static ModbusRtuParseResult ParseReadResponse(byte[] frame, byte slaveAddress, byte functionCode)
        {
            byte byteCount = frame[2];
            int expectedByteCount = frame.Length - 5;

            if (byteCount % 2 != 0)
            {
                return ModbusRtuParseResult.Failure(
                    ModbusRtuErrorCode.InvalidByteCount,
                    $"Byte count {byteCount} must be even",
                    frame);
            }

            if (byteCount != expectedByteCount)
            {
                return ModbusRtuParseResult.Failure(
                    ModbusRtuErrorCode.PayloadMismatch,
                    $"Byte count {byteCount} does not match payload length {expectedByteCount}",
                    frame);
            }

            int registerCount = byteCount / 2;
            ushort[] registers = new ushort[registerCount];
            for (int i = 0; i < registerCount; i++)
            {
                registers[i] = (ushort)((frame[3 + i * 2] << 8) | frame[4 + i * 2]);
            }

            return ModbusRtuParseResult.Success(
                slaveAddress,
                functionCode,
                null,
                (ushort)registerCount,
                null,
                registers,
                frame);
        }

        private static ModbusRtuParseResult ParseWriteSingleRegisterResponse(byte[] frame, byte slaveAddress, byte functionCode)
        {
            if (frame.Length != 8)
            {
                return ModbusRtuParseResult.Failure(
                    ModbusRtuErrorCode.InvalidLength,
                    $"Write single register response must be 8 bytes, got {frame.Length}",
                    frame);
            }

            ushort address = (ushort)((frame[2] << 8) | frame[3]);
            ushort value = (ushort)((frame[4] << 8) | frame[5]);

            return ModbusRtuParseResult.Success(
                slaveAddress,
                functionCode,
                address,
                1,
                value,
                null,
                frame);
        }

        private static ModbusRtuParseResult ParseWriteMultipleRegistersResponse(byte[] frame, byte slaveAddress, byte functionCode)
        {
            if (frame.Length != 8)
            {
                return ModbusRtuParseResult.Failure(
                    ModbusRtuErrorCode.InvalidLength,
                    $"Write multiple registers response must be 8 bytes, got {frame.Length}",
                    frame);
            }

            ushort startAddress = (ushort)((frame[2] << 8) | frame[3]);
            ushort quantity = (ushort)((frame[4] << 8) | frame[5]);

            return ModbusRtuParseResult.Success(
                slaveAddress,
                functionCode,
                startAddress,
                quantity,
                null,
                null,
                frame);
        }
    }
}
