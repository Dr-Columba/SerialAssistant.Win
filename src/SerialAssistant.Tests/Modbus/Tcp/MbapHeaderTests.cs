using Xunit;
using SerialAssistant.Core.Modbus.Tcp;

namespace SerialAssistant.Tests.Modbus.Tcp
{
    public class MbapHeaderTests
    {
        [Fact]
        public void Constructor_SetsTransactionIdCorrectly()
        {
            var header = new MbapHeader(0x0001, 0x0000, 0x0006, 0x01);
            Assert.Equal((ushort)0x0001, header.TransactionId);
        }

        [Fact]
        public void Constructor_SetsProtocolIdCorrectly()
        {
            var header = new MbapHeader(0x0001, 0x0000, 0x0006, 0x01);
            Assert.Equal((ushort)0x0000, header.ProtocolId);
        }

        [Fact]
        public void Constructor_SetsLengthCorrectly()
        {
            var header = new MbapHeader(0x0001, 0x0000, 0x0006, 0x01);
            Assert.Equal((ushort)0x0006, header.Length);
        }

        [Fact]
        public void Constructor_SetsUnitIdCorrectly()
        {
            var header = new MbapHeader(0x0001, 0x0000, 0x0006, 0x01);
            Assert.Equal((byte)0x01, header.UnitId);
        }

        [Fact]
        public void ToByteArray_UsesBigEndian()
        {
            var header = new MbapHeader(0x0001, 0x0000, 0x0006, 0x01);
            var bytes = header.ToByteArray();
            Assert.Equal(7, bytes.Length);
            Assert.Equal((byte)0x00, bytes[0]);
            Assert.Equal((byte)0x01, bytes[1]);
            Assert.Equal((byte)0x00, bytes[2]);
            Assert.Equal((byte)0x00, bytes[3]);
            Assert.Equal((byte)0x00, bytes[4]);
            Assert.Equal((byte)0x06, bytes[5]);
            Assert.Equal((byte)0x01, bytes[6]);
        }

        [Fact]
        public void Parse_Parses7BytesCorrectly()
        {
            byte[] data = new byte[] { 0x00, 0x01, 0x00, 0x00, 0x00, 0x06, 0x01 };
            var header = MbapHeader.Parse(data);
            Assert.Equal((ushort)0x0001, header.TransactionId);
            Assert.Equal((ushort)0x0000, header.ProtocolId);
            Assert.Equal((ushort)0x0006, header.Length);
            Assert.Equal((byte)0x01, header.UnitId);
        }

        [Fact]
        public void Parse_ThrowsOnInsufficientData()
        {
            byte[] data = new byte[] { 0x00, 0x01, 0x00, 0x00, 0x00, 0x06 };
            Assert.Throws<ArgumentException>(() => MbapHeader.Parse(data));
        }

        [Fact]
        public void ToString_ReturnsNonEmpty()
        {
            var header = new MbapHeader(0x0001, 0x0000, 0x0006, 0x01);
            var str = header.ToString();
            Assert.False(string.IsNullOrEmpty(str));
        }
    }
}
