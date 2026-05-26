using Xunit;
using SerialAssistant.Core.Modbus.Common;

namespace SerialAssistant.Tests.Modbus.Common
{
    public class ModbusDataTypeTests
    {
        [Fact]
        public void UInt16_Exists()
        {
            Assert.True(Enum.IsDefined(typeof(ModbusDataType), ModbusDataType.UInt16));
        }

        [Fact]
        public void Int16_Exists()
        {
            Assert.True(Enum.IsDefined(typeof(ModbusDataType), ModbusDataType.Int16));
        }

        [Fact]
        public void UInt32_Exists()
        {
            Assert.True(Enum.IsDefined(typeof(ModbusDataType), ModbusDataType.UInt32));
        }

        [Fact]
        public void Int32_Exists()
        {
            Assert.True(Enum.IsDefined(typeof(ModbusDataType), ModbusDataType.Int32));
        }

        [Fact]
        public void Float32_Exists()
        {
            Assert.True(Enum.IsDefined(typeof(ModbusDataType), ModbusDataType.Float32));
        }

        [Fact]
        public void Boolean_Exists()
        {
            Assert.True(Enum.IsDefined(typeof(ModbusDataType), ModbusDataType.Boolean));
        }

        [Fact]
        public void RawBytes_Exists()
        {
            Assert.True(Enum.IsDefined(typeof(ModbusDataType), ModbusDataType.RawBytes));
        }
    }
}
