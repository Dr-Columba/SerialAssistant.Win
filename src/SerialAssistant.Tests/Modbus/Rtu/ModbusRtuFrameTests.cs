using Xunit;
using SerialAssistant.Core.Modbus.Rtu;
using SerialAssistant.Core.Modbus.Utilities;

namespace SerialAssistant.Tests.Modbus.Rtu
{
    public class ModbusRtuFrameTests
    {
        [Fact]
        public void Constructor_SetsSlaveAddressCorrectly()
        {
            byte[] data = new byte[4];
            var frame = new ModbusRtuFrame(0x01, 0x03, data);
            Assert.Equal((byte)0x01, frame.SlaveAddress);
        }

        [Fact]
        public void Constructor_SetsFunctionCodeCorrectly()
        {
            byte[] data = new byte[4];
            var frame = new ModbusRtuFrame(0x01, 0x03, data);
            Assert.Equal((byte)0x03, frame.FunctionCode);
        }

        [Fact]
        public void Constructor_CopiesDataArray()
        {
            byte[] originalData = new byte[4];
            originalData[0] = 0x01;
            originalData[1] = 0x02;
            originalData[2] = 0x03;
            originalData[3] = 0x04;

            var frame = new ModbusRtuFrame(0x01, 0x03, originalData);

            originalData[0] = 0xFF;

            Assert.Equal((byte)0x01, frame.Data[0]);
            Assert.Equal((byte)0x02, frame.Data[1]);
            Assert.Equal((byte)0x03, frame.Data[2]);
            Assert.Equal((byte)0x04, frame.Data[3]);
        }

        [Fact]
        public void ToByteArray_ContainsSlaveAddress()
        {
            byte[] data = new byte[4];
            var frame = new ModbusRtuFrame(0x01, 0x03, data);
            var frameBytes = frame.ToByteArray();
            Assert.Equal((byte)0x01, frameBytes[0]);
        }

        [Fact]
        public void ToByteArray_ContainsFunctionCode()
        {
            byte[] data = new byte[4];
            var frame = new ModbusRtuFrame(0x01, 0x03, data);
            var frameBytes = frame.ToByteArray();
            Assert.Equal((byte)0x03, frameBytes[1]);
        }

        [Fact]
        public void ToByteArray_ContainsData()
        {
            byte[] data = new byte[4];
            data[0] = 0x01;
            data[1] = 0x02;
            data[2] = 0x03;
            data[3] = 0x04;
            var frame = new ModbusRtuFrame(0x01, 0x03, data);
            var frameBytes = frame.ToByteArray();
            Assert.Equal((byte)0x01, frameBytes[2]);
            Assert.Equal((byte)0x02, frameBytes[3]);
            Assert.Equal((byte)0x03, frameBytes[4]);
            Assert.Equal((byte)0x04, frameBytes[5]);
        }

        [Fact]
        public void ToByteArray_AppendsCrcLowHigh()
        {
            byte[] data = new byte[4];
            data[0] = 0x00;
            data[1] = 0x00;
            data[2] = 0x00;
            data[3] = 0x0A;
            var frame = new ModbusRtuFrame(0x01, 0x03, data);
            var frameBytes = frame.ToByteArray();
            Assert.Equal(8, frameBytes.Length);
            Assert.Equal((byte)0xC5, frameBytes[6]);
            Assert.Equal((byte)0xCD, frameBytes[7]);
        }

        [Fact]
        public void ToString_ReturnsNonEmpty()
        {
            byte[] data = new byte[4];
            var frame = new ModbusRtuFrame(0x01, 0x03, data);
            var str = frame.ToString();
            Assert.False(string.IsNullOrEmpty(str));
        }
    }
}
