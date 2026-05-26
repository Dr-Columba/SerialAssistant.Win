using Xunit;
using SerialAssistant.Core.Modbus.Tcp;

namespace SerialAssistant.Tests.Modbus.Tcp
{
    public class ModbusTcpResponseParserTests
    {
        [Fact]
        public void Parse_NullInput_ReturnsNullFrame()
        {
            var result = ModbusTcpResponseParser.Parse(null);
            Assert.False(result.IsSuccess);
            Assert.Equal(ModbusTcpErrorCode.NullFrame, result.ErrorCode);
        }

        [Fact]
        public void Parse_LengthLessThan9_ReturnsInvalidLength()
        {
            byte[] frame = new byte[8];
            var result = ModbusTcpResponseParser.Parse(frame);
            Assert.False(result.IsSuccess);
            Assert.Equal(ModbusTcpErrorCode.InvalidLength, result.ErrorCode);
        }

        [Fact]
        public void Parse_ProtocolIdNotZero_ReturnsInvalidProtocolId()
        {
            byte[] frame = new byte[9];
            frame[0] = 0x00;
            frame[1] = 0x01;
            frame[2] = 0x00;
            frame[3] = 0x01;
            frame[4] = 0x00;
            frame[5] = 0x02;
            frame[6] = 0x01;
            frame[7] = 0x03;
            frame[8] = 0x02;
            var result = ModbusTcpResponseParser.Parse(frame);
            Assert.False(result.IsSuccess);
            Assert.Equal(ModbusTcpErrorCode.InvalidProtocolId, result.ErrorCode);
        }

        [Fact]
        public void Parse_MbapLengthMismatch_ReturnsLengthMismatch()
        {
            byte[] frame = new byte[9];
            frame[0] = 0x00;
            frame[1] = 0x01;
            frame[2] = 0x00;
            frame[3] = 0x00;
            frame[4] = 0x00;
            frame[5] = 0x05;
            frame[6] = 0x01;
            frame[7] = 0x03;
            frame[8] = 0x02;
            var result = ModbusTcpResponseParser.Parse(frame);
            Assert.False(result.IsSuccess);
            Assert.Equal(ModbusTcpErrorCode.LengthMismatch, result.ErrorCode);
        }

        [Fact]
        public void Parse_03ReadHoldingRegisters_Success()
        {
            byte[] frame = new byte[13];
            frame[0] = 0x00;
            frame[1] = 0x01;
            frame[2] = 0x00;
            frame[3] = 0x00;
            frame[4] = 0x00;
            frame[5] = 0x06;
            frame[6] = 0x01;
            frame[7] = 0x03;
            frame[8] = 0x04;
            frame[9] = 0x00;
            frame[10] = 0x01;
            frame[11] = 0x00;
            frame[12] = 0x02;
            var result = ModbusTcpResponseParser.Parse(frame);
            Assert.True(result.IsSuccess);
            Assert.Equal(2, result.Registers!.Count);
        }

        [Fact]
        public void Parse_04ReadInputRegisters_Success()
        {
            byte[] frame = new byte[13];
            frame[0] = 0x00;
            frame[1] = 0x01;
            frame[2] = 0x00;
            frame[3] = 0x00;
            frame[4] = 0x00;
            frame[5] = 0x06;
            frame[6] = 0x01;
            frame[7] = 0x04;
            frame[8] = 0x04;
            frame[9] = 0x00;
            frame[10] = 0x01;
            frame[11] = 0x00;
            frame[12] = 0x02;
            var result = ModbusTcpResponseParser.Parse(frame);
            Assert.True(result.IsSuccess);
            Assert.Equal(2, result.Registers!.Count);
        }

        [Fact]
        public void Parse_0304OddByteCount_ReturnsInvalidByteCount()
        {
            byte[] frame = new byte[12];
            frame[0] = 0x00;
            frame[1] = 0x01;
            frame[2] = 0x00;
            frame[3] = 0x00;
            frame[4] = 0x00;
            frame[5] = 0x05;
            frame[6] = 0x01;
            frame[7] = 0x03;
            frame[8] = 0x03;
            frame[9] = 0x00;
            frame[10] = 0x01;
            frame[11] = 0x00;
            var result = ModbusTcpResponseParser.Parse(frame);
            Assert.False(result.IsSuccess);
            Assert.Equal(ModbusTcpErrorCode.InvalidByteCount, result.ErrorCode);
        }

        [Fact]
        public void Parse_0304ByteCountMismatch_ReturnsPayloadMismatch()
        {
            byte[] frame = new byte[12];
            frame[0] = 0x00;
            frame[1] = 0x01;
            frame[2] = 0x00;
            frame[3] = 0x00;
            frame[4] = 0x00;
            frame[5] = 0x05;
            frame[6] = 0x01;
            frame[7] = 0x03;
            frame[8] = 0x06;
            frame[9] = 0x00;
            frame[10] = 0x01;
            frame[11] = 0x00;
            var result = ModbusTcpResponseParser.Parse(frame);
            Assert.False(result.IsSuccess);
            Assert.Equal(ModbusTcpErrorCode.PayloadMismatch, result.ErrorCode);
        }

        [Fact]
        public void Parse_06WriteSingleRegister_AddressIsCorrect()
        {
            byte[] frame = new byte[13];
            frame[0] = 0x00;
            frame[1] = 0x01;
            frame[2] = 0x00;
            frame[3] = 0x00;
            frame[4] = 0x00;
            frame[5] = 0x06;
            frame[6] = 0x01;
            frame[7] = 0x06;
            frame[8] = 0x10;
            frame[9] = 0x00;
            frame[10] = 0x00;
            frame[11] = 0x01;
            var result = ModbusTcpResponseParser.Parse(frame);
            Assert.True(result.IsSuccess);
            Assert.Equal((ushort)0x1000, result.Address);
        }

        [Fact]
        public void Parse_06WriteSingleRegister_ValueIsCorrect()
        {
            byte[] frame = new byte[13];
            frame[0] = 0x00;
            frame[1] = 0x01;
            frame[2] = 0x00;
            frame[3] = 0x00;
            frame[4] = 0x00;
            frame[5] = 0x06;
            frame[6] = 0x01;
            frame[7] = 0x06;
            frame[8] = 0x00;
            frame[9] = 0x00;
            frame[10] = 0x00;
            frame[11] = 0x01;
            var result = ModbusTcpResponseParser.Parse(frame);
            Assert.True(result.IsSuccess);
            Assert.Equal((ushort)0x0001, result.Value);
        }

        [Fact]
        public void Parse_10WriteMultipleRegisters_AddressIsCorrect()
        {
            byte[] frame = new byte[13];
            frame[0] = 0x00;
            frame[1] = 0x01;
            frame[2] = 0x00;
            frame[3] = 0x00;
            frame[4] = 0x00;
            frame[5] = 0x06;
            frame[6] = 0x01;
            frame[7] = 0x10;
            frame[8] = 0x00;
            frame[9] = 0x00;
            frame[10] = 0x00;
            frame[11] = 0x02;
            var result = ModbusTcpResponseParser.Parse(frame);
            Assert.True(result.IsSuccess);
            Assert.Equal((ushort)0x0000, result.Address);
        }

        [Fact]
        public void Parse_10WriteMultipleRegisters_QuantityIsCorrect()
        {
            byte[] frame = new byte[13];
            frame[0] = 0x00;
            frame[1] = 0x01;
            frame[2] = 0x00;
            frame[3] = 0x00;
            frame[4] = 0x00;
            frame[5] = 0x06;
            frame[6] = 0x01;
            frame[7] = 0x10;
            frame[8] = 0x00;
            frame[9] = 0x00;
            frame[10] = 0x00;
            frame[11] = 0x02;
            var result = ModbusTcpResponseParser.Parse(frame);
            Assert.True(result.IsSuccess);
            Assert.Equal((ushort)0x0002, result.Quantity);
        }

        [Fact]
        public void Parse_ExceptionResponse0x83_IdentifiesException()
        {
            byte[] frame = new byte[10];
            frame[0] = 0x00;
            frame[1] = 0x01;
            frame[2] = 0x00;
            frame[3] = 0x00;
            frame[4] = 0x00;
            frame[5] = 0x03;
            frame[6] = 0x01;
            frame[7] = 0x83;
            frame[8] = 0x02;
            frame[9] = 0x00;
            var result = ModbusTcpResponseParser.Parse(frame);
            Assert.False(result.IsSuccess);
            Assert.True(result.IsExceptionResponse);
        }

        [Fact]
        public void Parse_ExceptionResponse_ExtractsExceptionCode()
        {
            byte[] frame = new byte[10];
            frame[0] = 0x00;
            frame[1] = 0x01;
            frame[2] = 0x00;
            frame[3] = 0x00;
            frame[4] = 0x00;
            frame[5] = 0x03;
            frame[6] = 0x01;
            frame[7] = 0x83;
            frame[8] = 0x02;
            frame[9] = 0x00;
            var result = ModbusTcpResponseParser.Parse(frame);
            Assert.Equal((byte)0x02, result.ExceptionCode);
        }

        [Fact]
        public void Parse_UnsupportedFunctionCode_ReturnsUnsupportedFunctionCode()
        {
            byte[] frame = new byte[10];
            frame[0] = 0x00;
            frame[1] = 0x01;
            frame[2] = 0x00;
            frame[3] = 0x00;
            frame[4] = 0x00;
            frame[5] = 0x03;
            frame[6] = 0x01;
            frame[7] = 0x01;
            frame[8] = 0x00;
            frame[9] = 0x00;
            var result = ModbusTcpResponseParser.Parse(frame);
            Assert.False(result.IsSuccess);
            Assert.Equal(ModbusTcpErrorCode.UnsupportedFunctionCode, result.ErrorCode);
        }

        [Fact]
        public void Parse_SuccessResult_RawFrameCopied()
        {
            byte[] frame = new byte[13];
            frame[0] = 0x00;
            frame[1] = 0x01;
            frame[2] = 0x00;
            frame[3] = 0x00;
            frame[4] = 0x00;
            frame[5] = 0x06;
            frame[6] = 0x01;
            frame[7] = 0x06;
            frame[8] = 0x10;
            frame[9] = 0x00;
            frame[10] = 0x00;
            frame[11] = 0x01;
            var result = ModbusTcpResponseParser.Parse(frame);
            Assert.Equal(13, result.RawFrame.Length);
            frame[0] = 0xFF;
            Assert.Equal((byte)0x00, result.RawFrame[0]);
        }

        [Fact]
        public void Parse_Registers_HighByteFirst()
        {
            byte[] frame = new byte[13];
            frame[0] = 0x00;
            frame[1] = 0x01;
            frame[2] = 0x00;
            frame[3] = 0x00;
            frame[4] = 0x00;
            frame[5] = 0x06;
            frame[6] = 0x01;
            frame[7] = 0x03;
            frame[8] = 0x04;
            frame[9] = 0x12;
            frame[10] = 0x34;
            frame[11] = 0x56;
            frame[12] = 0x78;
            var result = ModbusTcpResponseParser.Parse(frame);
            Assert.True(result.IsSuccess);
            Assert.Equal(2, result.Registers!.Count);
            Assert.Equal((ushort)0x1234, result.Registers[0]);
            Assert.Equal((ushort)0x5678, result.Registers[1]);
        }
    }
}
