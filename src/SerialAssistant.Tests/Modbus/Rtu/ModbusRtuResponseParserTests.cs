using Xunit;
using SerialAssistant.Core.Modbus.Rtu;
using SerialAssistant.Core.Modbus.Utilities;

namespace SerialAssistant.Tests.Modbus.Rtu
{
    public class ModbusRtuResponseParserTests
    {
        [Fact]
        public void Parse_NullInput_ReturnsNullFrame()
        {
            var result = ModbusRtuResponseParser.Parse(null);
            Assert.False(result.IsSuccess);
            Assert.Equal(ModbusRtuErrorCode.NullFrame, result.ErrorCode);
        }

        [Fact]
        public void Parse_LengthLessThan5_ReturnsInvalidLength()
        {
            byte[] frame = new byte[4];
            var result = ModbusRtuResponseParser.Parse(frame);
            Assert.False(result.IsSuccess);
            Assert.Equal(ModbusRtuErrorCode.InvalidLength, result.ErrorCode);
        }

        [Fact]
        public void Parse_WrongCrc_ReturnsInvalidCrc()
        {
            byte[] frame = new byte[8];
            frame[0] = 0x01;
            frame[1] = 0x03;
            frame[2] = 0x02;
            frame[3] = 0x00;
            frame[4] = 0x01;
            frame[5] = 0x00;
            frame[6] = 0xFF;
            frame[7] = 0xFF;
            var result = ModbusRtuResponseParser.Parse(frame);
            Assert.False(result.IsSuccess);
            Assert.Equal(ModbusRtuErrorCode.InvalidCrc, result.ErrorCode);
        }

        [Fact]
        public void Parse_03ReadHoldingRegisters_Success()
        {
            byte[] frame = new byte[9];
            frame[0] = 0x01;
            frame[1] = 0x03;
            frame[2] = 0x04;
            frame[3] = 0x00;
            frame[4] = 0x01;
            frame[5] = 0x00;
            frame[6] = 0x02;
            ushort crc = ModbusCrc16.Compute(frame.AsSpan(0, 7));
            frame[7] = ModbusCrc16.LowByte(crc);
            frame[8] = ModbusCrc16.HighByte(crc);
            var result = ModbusRtuResponseParser.Parse(frame);
            Assert.True(result.IsSuccess);
            Assert.Equal(2, result.Registers!.Count);
        }

        [Fact]
        public void Parse_04ReadInputRegisters_Success()
        {
            byte[] frame = new byte[9];
            frame[0] = 0x01;
            frame[1] = 0x04;
            frame[2] = 0x04;
            frame[3] = 0x00;
            frame[4] = 0x01;
            frame[5] = 0x00;
            frame[6] = 0x02;
            ushort crc = ModbusCrc16.Compute(frame.AsSpan(0, 7));
            frame[7] = ModbusCrc16.LowByte(crc);
            frame[8] = ModbusCrc16.HighByte(crc);
            var result = ModbusRtuResponseParser.Parse(frame);
            Assert.True(result.IsSuccess);
            Assert.Equal(2, result.Registers!.Count);
        }

        [Fact]
        public void Parse_0304OddByteCount_ReturnsInvalidByteCount()
        {
            byte[] frame = new byte[8];
            frame[0] = 0x01;
            frame[1] = 0x03;
            frame[2] = 0x03;
            frame[3] = 0x00;
            frame[4] = 0x01;
            frame[5] = 0x00;
            frame[6] = 0x00;
            ushort crc = ModbusCrc16.Compute(frame.AsSpan(0, 6));
            frame[6] = ModbusCrc16.LowByte(crc);
            frame[7] = ModbusCrc16.HighByte(crc);
            var result = ModbusRtuResponseParser.Parse(frame);
            Assert.False(result.IsSuccess);
            Assert.Equal(ModbusRtuErrorCode.InvalidByteCount, result.ErrorCode);
        }

        [Fact]
        public void Parse_0304ByteCountMismatch_ReturnsPayloadMismatch()
        {
            byte[] frame = new byte[8];
            frame[0] = 0x01;
            frame[1] = 0x03;
            frame[2] = 0x04;
            frame[3] = 0x00;
            frame[4] = 0x01;
            frame[5] = 0x00;
            frame[6] = 0x00;
            ushort crc = ModbusCrc16.Compute(frame.AsSpan(0, 6));
            frame[6] = ModbusCrc16.LowByte(crc);
            frame[7] = ModbusCrc16.HighByte(crc);
            var result = ModbusRtuResponseParser.Parse(frame);
            Assert.False(result.IsSuccess);
            Assert.Equal(ModbusRtuErrorCode.PayloadMismatch, result.ErrorCode);
        }

        [Fact]
        public void Parse_06WriteSingleRegister_AddressIsCorrect()
        {
            byte[] frame = new byte[8];
            frame[0] = 0x01;
            frame[1] = 0x06;
            frame[2] = 0x10;
            frame[3] = 0x00;
            frame[4] = 0x00;
            frame[5] = 0x01;
            ushort crc = ModbusCrc16.Compute(frame.AsSpan(0, 6));
            frame[6] = ModbusCrc16.LowByte(crc);
            frame[7] = ModbusCrc16.HighByte(crc);
            var result = ModbusRtuResponseParser.Parse(frame);
            Assert.True(result.IsSuccess);
            Assert.Equal((ushort)0x1000, result.Address);
        }

        [Fact]
        public void Parse_06WriteSingleRegister_ValueIsCorrect()
        {
            byte[] frame = new byte[8];
            frame[0] = 0x01;
            frame[1] = 0x06;
            frame[2] = 0x10;
            frame[3] = 0x00;
            frame[4] = 0x00;
            frame[5] = 0x01;
            ushort crc = ModbusCrc16.Compute(frame.AsSpan(0, 6));
            frame[6] = ModbusCrc16.LowByte(crc);
            frame[7] = ModbusCrc16.HighByte(crc);
            var result = ModbusRtuResponseParser.Parse(frame);
            Assert.True(result.IsSuccess);
            Assert.Equal((ushort)0x0001, result.Value);
        }

        [Fact]
        public void Parse_10WriteMultipleRegisters_AddressIsCorrect()
        {
            byte[] frame = new byte[8];
            frame[0] = 0x01;
            frame[1] = 0x10;
            frame[2] = 0x00;
            frame[3] = 0x00;
            frame[4] = 0x00;
            frame[5] = 0x02;
            ushort crc = ModbusCrc16.Compute(frame.AsSpan(0, 6));
            frame[6] = ModbusCrc16.LowByte(crc);
            frame[7] = ModbusCrc16.HighByte(crc);
            var result = ModbusRtuResponseParser.Parse(frame);
            Assert.True(result.IsSuccess);
            Assert.Equal((ushort)0x0000, result.Address);
        }

        [Fact]
        public void Parse_10WriteMultipleRegisters_QuantityIsCorrect()
        {
            byte[] frame = new byte[8];
            frame[0] = 0x01;
            frame[1] = 0x10;
            frame[2] = 0x00;
            frame[3] = 0x00;
            frame[4] = 0x00;
            frame[5] = 0x02;
            ushort crc = ModbusCrc16.Compute(frame.AsSpan(0, 6));
            frame[6] = ModbusCrc16.LowByte(crc);
            frame[7] = ModbusCrc16.HighByte(crc);
            var result = ModbusRtuResponseParser.Parse(frame);
            Assert.True(result.IsSuccess);
            Assert.Equal((ushort)0x0002, result.Quantity);
        }

        [Fact]
        public void Parse_ExceptionResponse0x83_IdentifiesException()
        {
            byte[] frame = new byte[5];
            frame[0] = 0x01;
            frame[1] = 0x83;
            frame[2] = 0x02;
            ushort crc = ModbusCrc16.Compute(frame.AsSpan(0, 3));
            frame[3] = ModbusCrc16.LowByte(crc);
            frame[4] = ModbusCrc16.HighByte(crc);
            var result = ModbusRtuResponseParser.Parse(frame);
            Assert.False(result.IsSuccess);
            Assert.True(result.IsExceptionResponse);
        }

        [Fact]
        public void Parse_ExceptionResponse_ExtractsExceptionCode()
        {
            byte[] frame = new byte[5];
            frame[0] = 0x01;
            frame[1] = 0x83;
            frame[2] = 0x02;
            ushort crc = ModbusCrc16.Compute(frame.AsSpan(0, 3));
            frame[3] = ModbusCrc16.LowByte(crc);
            frame[4] = ModbusCrc16.HighByte(crc);
            var result = ModbusRtuResponseParser.Parse(frame);
            Assert.Equal((byte)0x02, result.ExceptionCode);
        }

        [Fact]
        public void Parse_UnsupportedFunctionCode_ReturnsUnsupportedFunctionCode()
        {
            byte[] frame = new byte[5];
            frame[0] = 0x01;
            frame[1] = 0x01;
            frame[2] = 0x00;
            frame[3] = 0x00;
            frame[4] = 0x00;
            ushort crc = ModbusCrc16.Compute(frame.AsSpan(0, 3));
            frame[3] = ModbusCrc16.LowByte(crc);
            frame[4] = ModbusCrc16.HighByte(crc);
            var result = ModbusRtuResponseParser.Parse(frame);
            Assert.False(result.IsSuccess);
            Assert.Equal(ModbusRtuErrorCode.UnsupportedFunctionCode, result.ErrorCode);
        }

        [Fact]
        public void Parse_SuccessResult_RawFrameCopied()
        {
            byte[] frame = new byte[8];
            frame[0] = 0x01;
            frame[1] = 0x06;
            frame[2] = 0x10;
            frame[3] = 0x00;
            frame[4] = 0x00;
            frame[5] = 0x01;
            ushort crc = ModbusCrc16.Compute(frame.AsSpan(0, 6));
            frame[6] = ModbusCrc16.LowByte(crc);
            frame[7] = ModbusCrc16.HighByte(crc);
            var result = ModbusRtuResponseParser.Parse(frame);
            Assert.Equal(8, result.RawFrame.Length);
            frame[0] = 0xFF;
            Assert.Equal((byte)0x01, result.RawFrame[0]);
        }

        [Fact]
        public void Parse_Registers_HighByteFirst()
        {
            byte[] frame = new byte[9];
            frame[0] = 0x01;
            frame[1] = 0x03;
            frame[2] = 0x04;
            frame[3] = 0x12;
            frame[4] = 0x34;
            frame[5] = 0x56;
            frame[6] = 0x78;
            ushort crc = ModbusCrc16.Compute(frame.AsSpan(0, 7));
            frame[7] = ModbusCrc16.LowByte(crc);
            frame[8] = ModbusCrc16.HighByte(crc);
            var result = ModbusRtuResponseParser.Parse(frame);
            Assert.True(result.IsSuccess);
            Assert.Equal(2, result.Registers!.Count);
            Assert.Equal((ushort)0x1234, result.Registers[0]);
            Assert.Equal((ushort)0x5678, result.Registers[1]);
        }
    }
}
