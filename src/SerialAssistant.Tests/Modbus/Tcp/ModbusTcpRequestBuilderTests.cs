using Xunit;
using SerialAssistant.Core.Modbus.Tcp;

namespace SerialAssistant.Tests.Modbus.Tcp
{
    public class ModbusTcpRequestBuilderTests
    {
        [Fact]
        public void BuildReadHoldingRegisters_BuildsCorrectFrame()
        {
            var frame = ModbusTcpRequestBuilder.BuildReadHoldingRegisters(0x0001, 0x01, 0x0000, 0x000A);
            var bytes = frame.ToByteArray();
            Assert.Equal(12, bytes.Length);
            Assert.Equal((byte)0x00, bytes[0]);
            Assert.Equal((byte)0x01, bytes[1]);
            Assert.Equal((byte)0x00, bytes[2]);
            Assert.Equal((byte)0x00, bytes[3]);
            Assert.Equal((byte)0x00, bytes[4]);
            Assert.Equal((byte)0x06, bytes[5]);
            Assert.Equal((byte)0x01, bytes[6]);
            Assert.Equal((byte)0x03, bytes[7]);
            Assert.Equal((byte)0x00, bytes[8]);
            Assert.Equal((byte)0x00, bytes[9]);
            Assert.Equal((byte)0x00, bytes[10]);
            Assert.Equal((byte)0x0A, bytes[11]);
        }

        [Fact]
        public void BuildReadInputRegisters_BuildsCorrectFunctionCode()
        {
            var frame = ModbusTcpRequestBuilder.BuildReadInputRegisters(0x0001, 0x01, 0x0000, 0x000A);
            Assert.Equal((byte)0x04, frame.FunctionCode);
        }

        [Fact]
        public void BuildWriteSingleRegister_BuildsCorrectFunctionCode()
        {
            var frame = ModbusTcpRequestBuilder.BuildWriteSingleRegister(0x0001, 0x01, 0x1000, 0x0001);
            Assert.Equal((byte)0x06, frame.FunctionCode);
        }

        [Fact]
        public void BuildWriteMultipleRegisters_BuildsCorrectFunctionCode()
        {
            var values = new ushort[] { 0x0001, 0x0002 };
            var frame = ModbusTcpRequestBuilder.BuildWriteMultipleRegisters(0x0001, 0x01, 0x0000, values);
            Assert.Equal((byte)0x10, frame.FunctionCode);
        }

        [Fact]
        public void BuildReadHoldingRegisters_MbapLengthCorrect()
        {
            var frame = ModbusTcpRequestBuilder.BuildReadHoldingRegisters(0x0001, 0x01, 0x0000, 0x000A);
            Assert.Equal((ushort)0x0006, frame.Header.Length);
        }

        [Fact]
        public void BuildWriteMultipleRegisters_ByteCountCorrect()
        {
            var values = new ushort[] { 0x0001, 0x0002 };
            var frame = ModbusTcpRequestBuilder.BuildWriteMultipleRegisters(0x0001, 0x01, 0x0000, values);
            Assert.Equal((byte)0x04, frame.Data[4]);
        }

        [Fact]
        public void BuildWriteMultipleRegisters_HasHighByteFirst()
        {
            var values = new ushort[] { 0x1234, 0x5678 };
            var frame = ModbusTcpRequestBuilder.BuildWriteMultipleRegisters(0x0001, 0x01, 0x0000, values);
            Assert.Equal((byte)0x12, frame.Data[5]);
            Assert.Equal((byte)0x34, frame.Data[6]);
            Assert.Equal((byte)0x56, frame.Data[7]);
            Assert.Equal((byte)0x78, frame.Data[8]);
        }

        [Fact]
        public void UnitIdZero_ThrowsException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => ModbusTcpRequestBuilder.BuildReadHoldingRegisters(0x0001, 0, 0x0000, 0x000A));
        }

        [Fact]
        public void UnitIdTooHigh_ThrowsException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => ModbusTcpRequestBuilder.BuildReadHoldingRegisters(0x0001, 248, 0x0000, 0x000A));
        }

        [Fact]
        public void QuantityZero_ThrowsException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => ModbusTcpRequestBuilder.BuildReadHoldingRegisters(0x0001, 0x01, 0x0000, 0));
        }

        [Fact]
        public void QuantityTooHigh_ThrowsException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => ModbusTcpRequestBuilder.BuildReadHoldingRegisters(0x0001, 0x01, 0x0000, 126));
        }

        [Fact]
        public void WriteMultipleValuesNull_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => ModbusTcpRequestBuilder.BuildWriteMultipleRegisters(0x0001, 0x01, 0x0000, null!));
        }

        [Fact]
        public void WriteMultipleValuesEmpty_ThrowsException()
        {
            var values = new ushort[0];
            Assert.Throws<ArgumentOutOfRangeException>(() => ModbusTcpRequestBuilder.BuildWriteMultipleRegisters(0x0001, 0x01, 0x0000, values));
        }

        [Fact]
        public void WriteMultipleValuesTooMany_ThrowsException()
        {
            var values = new ushort[124];
            Assert.Throws<ArgumentOutOfRangeException>(() => ModbusTcpRequestBuilder.BuildWriteMultipleRegisters(0x0001, 0x01, 0x0000, values));
        }

        [Fact]
        public void BuildResult_DoesNotContainCrc()
        {
            var frame = ModbusTcpRequestBuilder.BuildReadHoldingRegisters(0x0001, 0x01, 0x0000, 0x000A);
            var bytes = frame.ToByteArray();
            Assert.Equal(12, bytes.Length);
        }
    }
}
