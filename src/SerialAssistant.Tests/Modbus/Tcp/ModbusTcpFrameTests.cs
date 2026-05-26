using Xunit;
using SerialAssistant.Core.Modbus.Tcp;

namespace SerialAssistant.Tests.Modbus.Tcp
{
    public class ModbusTcpFrameTests
    {
        [Fact]
        public void Constructor_SetsHeaderCorrectly()
        {
            var header = new MbapHeader(0x0001, 0x0000, 0x0006, 0x01);
            var frame = new ModbusTcpFrame(header, 0x03, new byte[] { 0x00, 0x00, 0x00, 0x0A });
            Assert.Same(header, frame.Header);
        }

        [Fact]
        public void Constructor_SetsFunctionCodeCorrectly()
        {
            var header = new MbapHeader(0x0001, 0x0000, 0x0006, 0x01);
            var frame = new ModbusTcpFrame(header, 0x03, new byte[] { 0x00, 0x00, 0x00, 0x0A });
            Assert.Equal((byte)0x03, frame.FunctionCode);
        }

        [Fact]
        public void Constructor_CopiesDataArray()
        {
            var header = new MbapHeader(0x0001, 0x0000, 0x0006, 0x01);
            byte[] originalData = new byte[] { 0x00, 0x00, 0x00, 0x0A };
            var frame = new ModbusTcpFrame(header, 0x03, originalData);
            originalData[0] = 0xFF;
            Assert.Equal((byte)0x00, frame.Data[0]);
        }

        [Fact]
        public void ToByteArray_ContainsMbap()
        {
            var header = new MbapHeader(0x0001, 0x0000, 0x0006, 0x01);
            var frame = new ModbusTcpFrame(header, 0x03, new byte[] { 0x00, 0x00, 0x00, 0x0A });
            var bytes = frame.ToByteArray();
            Assert.Equal((byte)0x00, bytes[0]);
            Assert.Equal((byte)0x01, bytes[1]);
            Assert.Equal((byte)0x00, bytes[2]);
            Assert.Equal((byte)0x00, bytes[3]);
            Assert.Equal((byte)0x00, bytes[4]);
            Assert.Equal((byte)0x06, bytes[5]);
            Assert.Equal((byte)0x01, bytes[6]);
        }

        [Fact]
        public void ToByteArray_ContainsFunctionCode()
        {
            var header = new MbapHeader(0x0001, 0x0000, 0x0006, 0x01);
            var frame = new ModbusTcpFrame(header, 0x03, new byte[] { 0x00, 0x00, 0x00, 0x0A });
            var bytes = frame.ToByteArray();
            Assert.Equal((byte)0x03, bytes[7]);
        }

        [Fact]
        public void ToByteArray_ContainsData()
        {
            var header = new MbapHeader(0x0001, 0x0000, 0x0006, 0x01);
            byte[] data = new byte[] { 0x00, 0x00, 0x00, 0x0A };
            var frame = new ModbusTcpFrame(header, 0x03, data);
            var bytes = frame.ToByteArray();
            Assert.Equal((byte)0x00, bytes[8]);
            Assert.Equal((byte)0x00, bytes[9]);
            Assert.Equal((byte)0x00, bytes[10]);
            Assert.Equal((byte)0x0A, bytes[11]);
        }

        [Fact]
        public void ToByteArray_DoesNotContainCrc()
        {
            var header = new MbapHeader(0x0001, 0x0000, 0x0006, 0x01);
            var frame = new ModbusTcpFrame(header, 0x03, new byte[] { 0x00, 0x00, 0x00, 0x0A });
            var bytes = frame.ToByteArray();
            Assert.Equal(12, bytes.Length);
        }

        [Fact]
        public void ToString_ReturnsNonEmpty()
        {
            var header = new MbapHeader(0x0001, 0x0000, 0x0006, 0x01);
            var frame = new ModbusTcpFrame(header, 0x03, new byte[] { 0x00, 0x00, 0x00, 0x0A });
            var str = frame.ToString();
            Assert.False(string.IsNullOrEmpty(str));
        }
    }
}
