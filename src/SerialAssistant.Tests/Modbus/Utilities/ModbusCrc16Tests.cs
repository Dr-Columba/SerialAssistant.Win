using Xunit;
using SerialAssistant.Core.Modbus.Utilities;

namespace SerialAssistant.Tests.Modbus.Utilities
{
    public class ModbusCrc16Tests
    {
        [Fact]
        public void Compute_EmptyInput_ReturnsFFFF()
        {
            byte[] empty = Array.Empty<byte>();
            ushort result = ModbusCrc16.Compute(empty);
            Assert.Equal((ushort)0xFFFF, result);
        }

        [Fact]
        public void Compute_StandardRequest_ReturnsCorrectCrc()
        {
            byte[] data = new byte[] { 0x01, 0x03, 0x00, 0x00, 0x00, 0x0A };
            ushort result = ModbusCrc16.Compute(data);
            Assert.Equal((ushort)0xCDC5, result);
        }

        [Fact]
        public void AppendCrc_ReturnsFrameWithCrcLowHigh()
        {
            byte[] data = new byte[] { 0x01, 0x03, 0x00, 0x00, 0x00, 0x0A };
            byte[] result = ModbusCrc16.AppendCrc(data);
            Assert.Equal(8, result.Length);
            Assert.Equal((byte)0xC5, result[6]);
            Assert.Equal((byte)0xCD, result[7]);
        }

        [Fact]
        public void Validate_ValidFrame_ReturnsTrue()
        {
            byte[] frame = new byte[] { 0x01, 0x03, 0x00, 0x00, 0x00, 0x0A, 0xC5, 0xCD };
            Assert.True(ModbusCrc16.Validate(frame));
        }

        [Fact]
        public void Validate_InvalidCrc_ReturnsFalse()
        {
            byte[] frame = new byte[] { 0x01, 0x03, 0x00, 0x00, 0x00, 0x0A, 0x00, 0x00 };
            Assert.False(ModbusCrc16.Validate(frame));
        }

        [Fact]
        public void Validate_LengthLessThan3_ReturnsFalse()
        {
            byte[] shortFrame = new byte[] { 0x01, 0x02 };
            Assert.False(ModbusCrc16.Validate(shortFrame));
        }

        [Fact]
        public void AppendCrc_DoesNotModifyOriginalInput()
        {
            byte[] original = new byte[] { 0x01, 0x03, 0x00, 0x00, 0x00, 0x0A };
            byte[] originalCopy = (byte[])original.Clone();
            ModbusCrc16.AppendCrc(original);
            Assert.Equal(originalCopy, original);
        }

        [Fact]
        public void Compute_ArrayAndSpan_ReturnSameResult()
        {
            byte[] data = new byte[] { 0x01, 0x03, 0x00, 0x00, 0x00, 0x0A };
            ushort fromArray = ModbusCrc16.Compute(data);
            ushort fromSpan = ModbusCrc16.Compute(data.AsSpan());
            Assert.Equal(fromArray, fromSpan);
        }

        [Fact]
        public void Compute_NullArray_ThrowsArgumentNullException()
        {
            byte[]? nullArray = null;
            Assert.Throws<System.ArgumentNullException>(() => ModbusCrc16.Compute(nullArray!));
        }

        [Fact]
        public void LowByte_ReturnsLowByte()
        {
            Assert.Equal((byte)0xC5, ModbusCrc16.LowByte(0xCDC5));
        }

        [Fact]
        public void HighByte_ReturnsHighByte()
        {
            Assert.Equal((byte)0xCD, ModbusCrc16.HighByte(0xCDC5));
        }
    }
}
