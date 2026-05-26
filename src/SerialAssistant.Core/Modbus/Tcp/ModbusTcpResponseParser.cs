namespace SerialAssistant.Core.Modbus.Tcp
{
    public static class ModbusTcpResponseParser
    {
        private const byte ReadHoldingRegisters = 0x03;
        private const byte ReadInputRegisters = 0x04;
        private const byte WriteSingleRegister = 0x06;
        private const byte WriteMultipleRegisters = 0x10;
        private const byte ExceptionBit = 0x80;

        public static ModbusTcpParseResult Parse(byte[]? frame)
        {
            if (frame == null)
            {
                return ModbusTcpParseResult.Failure(
                    ModbusTcpErrorCode.NullFrame,
                    "Frame cannot be null",
                    null);
            }

            if (frame.Length < 9)
            {
                return ModbusTcpParseResult.Failure(
                    ModbusTcpErrorCode.InvalidLength,
                    $"Frame length {frame.Length} is less than minimum 9",
                    frame);
            }

            ushort transactionId = (ushort)((frame[0] << 8) | frame[1]);
            ushort protocolId = (ushort)((frame[2] << 8) | frame[3]);
            ushort length = (ushort)((frame[4] << 8) | frame[5]);
            byte unitId = frame[6];

            if (protocolId != 0)
            {
                return ModbusTcpParseResult.Failure(
                    ModbusTcpErrorCode.InvalidProtocolId,
                    $"ProtocolId must be 0, got {protocolId}",
                    frame);
            }

            int actualRemaining = frame.Length - 7;
            if (length != actualRemaining)
            {
                return ModbusTcpParseResult.Failure(
                    ModbusTcpErrorCode.LengthMismatch,
                    $"MBAP Length {length} does not match actual remaining length {actualRemaining}",
                    frame);
            }

            byte functionCode = frame[7];

            if ((functionCode & ExceptionBit) != 0)
            {
                byte exceptionCode = frame[8];
                return ModbusTcpParseResult.ExceptionResponse(
                    transactionId,
                    unitId,
                    (byte)(functionCode & 0x7F),
                    exceptionCode,
                    frame);
            }

            switch (functionCode)
            {
                case ReadHoldingRegisters:
                case ReadInputRegisters:
                    return ParseReadResponse(frame, transactionId, unitId, functionCode);

                case WriteSingleRegister:
                    return ParseWriteSingleRegisterResponse(frame, transactionId, unitId, functionCode);

                case WriteMultipleRegisters:
                    return ParseWriteMultipleRegistersResponse(frame, transactionId, unitId, functionCode);

                default:
                    return ModbusTcpParseResult.Failure(
                        ModbusTcpErrorCode.UnsupportedFunctionCode,
                        $"Unsupported function code: 0x{functionCode:X2}",
                        frame);
            }
        }

        private static ModbusTcpParseResult ParseReadResponse(
            byte[] frame,
            ushort transactionId,
            byte unitId,
            byte functionCode)
        {
            if (frame.Length < 9)
            {
                return ModbusTcpParseResult.Failure(
                    ModbusTcpErrorCode.InvalidLength,
                    "Frame too short for read response",
                    frame);
            }

            byte byteCount = frame[8];
            int expectedByteCount = frame.Length - 9;

            if (byteCount % 2 != 0)
            {
                return ModbusTcpParseResult.Failure(
                    ModbusTcpErrorCode.InvalidByteCount,
                    $"Byte count {byteCount} must be even",
                    frame);
            }

            if (byteCount != expectedByteCount)
            {
                return ModbusTcpParseResult.Failure(
                    ModbusTcpErrorCode.PayloadMismatch,
                    $"Byte count {byteCount} does not match payload length {expectedByteCount}",
                    frame);
            }

            int registerCount = byteCount / 2;
            ushort[] registers = new ushort[registerCount];
            for (int i = 0; i < registerCount; i++)
            {
                registers[i] = (ushort)((frame[9 + i * 2] << 8) | frame[10 + i * 2]);
            }

            return ModbusTcpParseResult.Success(
                transactionId,
                unitId,
                functionCode,
                null,
                (ushort)registerCount,
                null,
                registers,
                frame);
        }

        private static ModbusTcpParseResult ParseWriteSingleRegisterResponse(
            byte[] frame,
            ushort transactionId,
            byte unitId,
            byte functionCode)
        {
            if (frame.Length < 12)
            {
                return ModbusTcpParseResult.Failure(
                    ModbusTcpErrorCode.InvalidLength,
                    "Frame too short for write single register response",
                    frame);
            }

            ushort address = (ushort)((frame[8] << 8) | frame[9]);
            ushort value = (ushort)((frame[10] << 8) | frame[11]);

            return ModbusTcpParseResult.Success(
                transactionId,
                unitId,
                functionCode,
                address,
                1,
                value,
                null,
                frame);
        }

        private static ModbusTcpParseResult ParseWriteMultipleRegistersResponse(
            byte[] frame,
            ushort transactionId,
            byte unitId,
            byte functionCode)
        {
            if (frame.Length < 12)
            {
                return ModbusTcpParseResult.Failure(
                    ModbusTcpErrorCode.InvalidLength,
                    "Frame too short for write multiple registers response",
                    frame);
            }

            ushort startAddress = (ushort)((frame[8] << 8) | frame[9]);
            ushort quantity = (ushort)((frame[10] << 8) | frame[11]);

            return ModbusTcpParseResult.Success(
                transactionId,
                unitId,
                functionCode,
                startAddress,
                quantity,
                null,
                null,
                frame);
        }
    }
}
