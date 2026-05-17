using Xunit;
using SerialAssistant.Core.Utilities;

namespace SerialAssistant.Tests.Utilities
{
    /*
     * Tests for HexConverter utility class
     */
    public class HexConverterTests
    {
        /*
         * Test byte array to HEX string conversion
         */
        [Fact]
        public void ToHexString_ValidByteArray_ReturnsCorrectHexString()
        {
            /* Arrange */
            byte[] data = new byte[] { 0x41, 0x42, 0x43 };

            /* Act */
            string result = HexConverter.ToHexString(data);

            /* Assert */
            Assert.Equal("41 42 43", result);
        }

        /*
         * Test empty byte array conversion
         */
        [Fact]
        public void ToHexString_EmptyByteArray_ReturnsEmptyString()
        {
            /* Arrange */
            byte[] data = Array.Empty<byte>();

            /* Act */
            string result = HexConverter.ToHexString(data);

            /* Assert */
            Assert.Equal(string.Empty, result);
        }

        /*
         * Test null byte array conversion
         */
        [Fact]
        public void ToHexString_NullByteArray_ReturnsEmptyString()
        {
            /* Arrange */
            byte[]? data = null;

            /* Act */
            string result = HexConverter.ToHexString(data!);

            /* Assert */
            Assert.Equal(string.Empty, result);
        }

        /*
         * Test HEX string with spaces to byte array conversion
         */
        [Fact]
        public void FromHexString_SpacedHexString_ReturnsCorrectByteArray()
        {
            /* Arrange */
            string hexString = "41 42 43";

            /* Act */
            var result = HexConverter.FromHexString(hexString);

            /* Assert */
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(3, result.Value.Length);
            Assert.Equal(0x41, result.Value[0]);
            Assert.Equal(0x42, result.Value[1]);
            Assert.Equal(0x43, result.Value[2]);
        }

        /*
         * Test continuous HEX string to byte array conversion
         */
        [Fact]
        public void FromHexString_ContinuousHexString_ReturnsCorrectByteArray()
        {
            /* Arrange */
            string hexString = "414243";

            /* Act */
            var result = HexConverter.FromHexString(hexString);

            /* Assert */
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(3, result.Value.Length);
            Assert.Equal(0x41, result.Value[0]);
            Assert.Equal(0x42, result.Value[1]);
            Assert.Equal(0x43, result.Value[2]);
        }

        /*
         * Test lowercase HEX conversion
         */
        [Fact]
        public void FromHexString_LowercaseHex_ReturnsCorrectByteArray()
        {
            /* Arrange */
            string hexString = "aa bb cc";

            /* Act */
            var result = HexConverter.FromHexString(hexString);

            /* Assert */
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(3, result.Value.Length);
            Assert.Equal(0xAA, result.Value[0]);
            Assert.Equal(0xBB, result.Value[1]);
            Assert.Equal(0xCC, result.Value[2]);
        }

        /*
         * Test odd length HEX string returns failure
         */
        [Fact]
        public void FromHexString_OddLengthHex_ReturnsFailure()
        {
            /* Arrange */
            string hexString = "123";

            /* Act */
            var result = HexConverter.FromHexString(hexString);

            /* Assert */
            Assert.False(result.IsSuccess);
            Assert.Contains("even number of characters", result.ErrorMessage);
        }

        /*
         * Test illegal character HEX returns failure
         */
        [Fact]
        public void FromHexString_IllegalCharacterHex_ReturnsFailure()
        {
            /* Arrange */
            string hexString = "1Z";

            /* Act */
            var result = HexConverter.FromHexString(hexString);

            /* Assert */
            Assert.False(result.IsSuccess);
            Assert.Contains("Invalid hex character", result.ErrorMessage);
        }

        /*
         * Test empty string returns empty byte array
         */
        [Fact]
        public void FromHexString_EmptyString_ReturnsEmptyByteArray()
        {
            /* Arrange */
            string hexString = string.Empty;

            /* Act */
            var result = HexConverter.FromHexString(hexString);

            /* Assert */
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Empty(result.Value);
        }

        /*
         * Test whitespace only string returns empty byte array
         */
        [Fact]
        public void FromHexString_WhitespaceOnlyString_ReturnsEmptyByteArray()
        {
            /* Arrange */
            string hexString = "   ";

            /* Act */
            var result = HexConverter.FromHexString(hexString);

            /* Assert */
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Empty(result.Value);
        }

        /*
         * Test mixed case HEX string conversion
         */
        [Fact]
        public void FromHexString_MixedCaseHex_ReturnsCorrectByteArray()
        {
            /* Arrange */
            string hexString = "AaBbCc";

            /* Act */
            var result = HexConverter.FromHexString(hexString);

            /* Assert */
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(3, result.Value.Length);
            Assert.Equal(0xAA, result.Value[0]);
            Assert.Equal(0xBB, result.Value[1]);
            Assert.Equal(0xCC, result.Value[2]);
        }
    }
}
