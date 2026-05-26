using Xunit;
using SerialAssistant.Core.Modbus.Common;

namespace SerialAssistant.Tests.Modbus.Common
{
    public class ModbusFunctionCodeTests
    {
        [Fact]
        public void ReadCoils_HasCorrectValue()
        {
            Assert.Equal((byte)0x01, (byte)ModbusFunctionCode.ReadCoils);
        }

        [Fact]
        public void ReadDiscreteInputs_HasCorrectValue()
        {
            Assert.Equal((byte)0x02, (byte)ModbusFunctionCode.ReadDiscreteInputs);
        }

        [Fact]
        public void ReadHoldingRegisters_HasCorrectValue()
        {
            Assert.Equal((byte)0x03, (byte)ModbusFunctionCode.ReadHoldingRegisters);
        }

        [Fact]
        public void ReadInputRegisters_HasCorrectValue()
        {
            Assert.Equal((byte)0x04, (byte)ModbusFunctionCode.ReadInputRegisters);
        }

        [Fact]
        public void WriteSingleCoil_HasCorrectValue()
        {
            Assert.Equal((byte)0x05, (byte)ModbusFunctionCode.WriteSingleCoil);
        }

        [Fact]
        public void WriteSingleRegister_HasCorrectValue()
        {
            Assert.Equal((byte)0x06, (byte)ModbusFunctionCode.WriteSingleRegister);
        }

        [Fact]
        public void WriteMultipleCoils_HasCorrectValue()
        {
            Assert.Equal((byte)0x0F, (byte)ModbusFunctionCode.WriteMultipleCoils);
        }

        [Fact]
        public void WriteMultipleRegisters_HasCorrectValue()
        {
            Assert.Equal((byte)0x10, (byte)ModbusFunctionCode.WriteMultipleRegisters);
        }
    }
}
