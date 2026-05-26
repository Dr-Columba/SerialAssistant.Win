using Xunit;
using SerialAssistant.Core.Modbus.Rtu;
using SerialAssistant.Core.Modbus.Utilities;

namespace SerialAssistant.Tests.Modbus.Rtu
{
    public class ModbusRtuRequestBuilderTests
    {
        [Fact]
        public void BuildReadHoldingRegisters_BuildsCorrectFrame()
        {
            var frame = ModbusRtuRequestBuilder.BuildReadHoldingRegisters(0x01, 0x0000, 0x000A);
            var frameBytes = frame.ToByteArray();
            Assert.Equal(8, frameBytes.Length);
            Assert.Equal((byte)0x01, frameBytes[0]);
            Assert.Equal((byte)0x03, frameBytes[1]);
            Assert.Equal((byte)0x00, frameBytes[2]);
            Assert.Equal((byte)0x00, frameBytes[3]);
            Assert.Equal((byte)0x00, frameBytes[4]);
            Assert.Equal((byte)0x0A, frameBytes[5]);
            Assert.Equal((byte)0xC5, frameBytes[6]);
            Assert.Equal((byte)0xCD, frameBytes[7]);
        }

        [Fact]
        public void BuildReadInputRegisters_BuildsCorrectFunctionCode()
        {
            var frame = ModbusRtuRequestBuilder.BuildReadInputRegisters(0x01, 0x0000, 0x000A);
            Assert.Equal((byte)0x04, frame.FunctionCode);
        }

        [Fact]
        public void BuildWriteSingleRegister_BuildsCorrectFunctionCode()
        {
            var frame = ModbusRtuRequestBuilder.BuildWriteSingleRegister(0x01, 0x1000, 0x0001);
            Assert.Equal((byte)0x06, frame.FunctionCode);
        }

        [Fact]
        public void BuildWriteMultipleRegisters_BuildsCorrectFunctionCode()
        {
            var values = new ushort[] { 0x0001, 0x0002 };
            var frame = ModbusRtuRequestBuilder.BuildWriteMultipleRegisters(0x01, 0x0000, values);
            Assert.Equal((byte)0x10, frame.FunctionCode);
        }

        [Fact]
        public void BuildWriteMultipleRegisters_HasCorrectByteCount()
        {
            var values = new ushort[] { 0x0001, 0x0002 };
            var frame = ModbusRtuRequestBuilder.BuildWriteMultipleRegisters(0x01, 0x0000, values);
            Assert.Equal((byte)0x04, frame.Data[4]);
        }

        [Fact]
        public void BuildWriteMultipleRegisters_HasHighByteFirst()
        {
            var values = new ushort[] { 0x1234, 0x5678 };
            var frame = ModbusRtuRequestBuilder.BuildWriteMultipleRegisters(0x01, 0x0000, values);
            Assert.Equal((byte)0x12, frame.Data[5]);
            Assert.Equal((byte)0x34, frame.Data[6]);
            Assert.Equal((byte)0x56, frame.Data[7]);
            Assert.Equal((byte)0x78, frame.Data[8]);
        }

        [Fact]
        public void SlaveAddressZero_ThrowsException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => ModbusRtuRequestBuilder.BuildReadHoldingRegisters(0, 0x0000, 0x000A));
        }

        [Fact]
        public void SlaveAddressTooHigh_ThrowsException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => ModbusRtuRequestBuilder.BuildReadHoldingRegisters(248, 0x0000, 0x000A));
        }

        [Fact]
        public void QuantityZero_ThrowsException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => ModbusRtuRequestBuilder.BuildReadHoldingRegisters(0x01, 0x0000, 0));
        }

        [Fact]
        public void QuantityTooHigh_ThrowsException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => ModbusRtuRequestBuilder.BuildReadHoldingRegisters(0x01, 0x0000, 126));
        }

        [Fact]
        public void WriteMultipleValuesNull_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => ModbusRtuRequestBuilder.BuildWriteMultipleRegisters(0x01, 0x0000, null!));
        }

        [Fact]
        public void WriteMultipleValuesEmpty_ThrowsException()
        {
            var values = new ushort[0];
            Assert.Throws<ArgumentOutOfRangeException>(() => ModbusRtuRequestBuilder.BuildWriteMultipleRegisters(0x01, 0x0000, values));
        }

        [Fact]
        public void WriteMultipleValuesTooMany_ThrowsException()
        {
            var values = new ushort[124];
            Assert.Throws<ArgumentOutOfRangeException>(() => ModbusRtuRequestBuilder.BuildWriteMultipleRegisters(0x01, 0x0000, values));
        }

        [Fact]
        public void BuildResult_ValidateCrcReturnsTrue()
        {
            var frame = ModbusRtuRequestBuilder.BuildReadHoldingRegisters(0x01, 0x0000, 0x000A);
            var frameBytes = frame.ToByteArray();
            Assert.True(ModbusCrc16.Validate(frameBytes));
        }
    }
}
