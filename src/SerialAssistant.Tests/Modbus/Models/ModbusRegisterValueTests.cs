using Xunit;
using SerialAssistant.Core.Modbus.Models;
using SerialAssistant.Core.Modbus.Common;

namespace SerialAssistant.Tests.Modbus.Models
{
    public class ModbusRegisterValueTests
    {
        [Fact]
        public void Constructor_SetsAddressCorrectly()
        {
            var register = new ModbusRegisterValue(0x1234, 0xABCD);
            Assert.Equal((ushort)0x1234, register.Address);
        }

        [Fact]
        public void Constructor_SetsRawValueCorrectly()
        {
            var register = new ModbusRegisterValue(0x1234, 0xABCD);
            Assert.Equal((ushort)0xABCD, register.RawValue);
        }

        [Fact]
        public void Constructor_DefaultDataType_IsUInt16()
        {
            var register = new ModbusRegisterValue(0x0000, 0x0000);
            Assert.Equal(ModbusDataType.UInt16, register.DataType);
        }

        [Fact]
        public void Constructor_ExplicitDataType_IsSet()
        {
            var register = new ModbusRegisterValue(0x0000, 0x0000, ModbusDataType.Int16);
            Assert.Equal(ModbusDataType.Int16, register.DataType);
        }

        [Fact]
        public void HighByte_ReturnsHighByte()
        {
            var register = new ModbusRegisterValue(0x0000, 0xABCD);
            Assert.Equal((byte)0xAB, register.HighByte);
        }

        [Fact]
        public void LowByte_ReturnsLowByte()
        {
            var register = new ModbusRegisterValue(0x0000, 0xABCD);
            Assert.Equal((byte)0xCD, register.LowByte);
        }

        [Fact]
        public void SignedValue_ForFFFF_ReturnsNegativeOne()
        {
            var register = new ModbusRegisterValue(0x0000, 0xFFFF);
            Assert.Equal((short)(-1), register.SignedValue);
        }

        [Fact]
        public void ToString_ReturnsNonEmptyString()
        {
            var register = new ModbusRegisterValue(0x1234, 0xABCD);
            var result = register.ToString();
            Assert.False(string.IsNullOrEmpty(result));
            Assert.Contains("1234", result);
            Assert.Contains("ABCD", result);
        }
    }
}
