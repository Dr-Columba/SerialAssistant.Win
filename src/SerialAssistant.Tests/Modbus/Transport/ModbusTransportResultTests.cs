using Xunit;
using SerialAssistant.Core.Modbus.Transport;

namespace SerialAssistant.Tests.Modbus.Transport
{
    public class ModbusTransportResultTests
    {
        [Fact]
        public void SuccessResult_SetsSuccessTrue()
        {
            var responseBytes = new byte[] { 0x01, 0x02, 0x03 };
            var duration = TimeSpan.FromMilliseconds(100);

            var result = ModbusTransportResult.SuccessResult(responseBytes, duration);

            Assert.True(result.Success);
            Assert.Equal(ModbusTransportErrorCode.None, result.ErrorCode);
        }

        [Fact]
        public void SuccessResult_CopiesResponseBytes()
        {
            var originalBytes = new byte[] { 0x01, 0x02, 0x03 };
            var duration = TimeSpan.FromMilliseconds(100);

            var result = ModbusTransportResult.SuccessResult(originalBytes, duration);

            Assert.NotNull(result.ResponseBytes);
            Assert.Equal(originalBytes.Length, result.ResponseBytes.Length);
            Assert.Equal(originalBytes, result.ResponseBytes);

            originalBytes[0] = 0xFF;
            Assert.Equal(0x01, result.ResponseBytes[0]);
        }

        [Fact]
        public void Failure_SetsSuccessFalse()
        {
            var duration = TimeSpan.FromMilliseconds(50);

            var result = ModbusTransportResult.Failure(
                ModbusTransportErrorCode.Timeout,
                "Test error",
                duration);

            Assert.False(result.Success);
            Assert.Equal(ModbusTransportErrorCode.Timeout, result.ErrorCode);
            Assert.Equal("Test error", result.ErrorMessage);
        }

        [Fact]
        public void Failure_SetsErrorCode()
        {
            var duration = TimeSpan.FromMilliseconds(50);

            var result = ModbusTransportResult.Failure(
                ModbusTransportErrorCode.ConnectFailed,
                "Connection failed",
                duration);

            Assert.Equal(ModbusTransportErrorCode.ConnectFailed, result.ErrorCode);
        }

        [Fact]
        public void Failure_UsesEmptyMessageWhenNullOrEmptyIfApplicable()
        {
            var duration = TimeSpan.FromMilliseconds(50);

            var result = ModbusTransportResult.Failure(
                ModbusTransportErrorCode.Unknown,
                null!,
                duration);

            Assert.NotNull(result.ErrorMessage);
            Assert.Equal(string.Empty, result.ErrorMessage);
        }

        [Fact]
        public void ResponseBytes_ReturnsCopy()
        {
            var responseBytes = new byte[] { 0x0A, 0x0B, 0x0C };
            var result = ModbusTransportResult.SuccessResult(responseBytes, TimeSpan.FromMilliseconds(10));

            var copy = result.GetResponseBytesCopy();

            Assert.NotNull(copy);
            Assert.Equal(responseBytes, copy);

            copy![0] = 0xFF;
            Assert.Equal(0x0A, result.ResponseBytes![0]);
        }

        [Fact]
        public void SuccessResult_SetsDuration()
        {
            var duration = TimeSpan.FromMilliseconds(250);
            var result = ModbusTransportResult.SuccessResult(new byte[] { 0x01 }, duration);

            Assert.Equal(duration, result.Duration);
        }

        [Fact]
        public void Failure_SetsDuration()
        {
            var duration = TimeSpan.FromMilliseconds(75);
            var result = ModbusTransportResult.Failure(
                ModbusTransportErrorCode.SendFailed,
                "Send failed",
                duration);

            Assert.Equal(duration, result.Duration);
        }

        [Fact]
        public void SuccessResult_SetsCompletedAt()
        {
            var before = DateTimeOffset.UtcNow;
            var result = ModbusTransportResult.SuccessResult(new byte[] { 0x01 }, TimeSpan.Zero);
            var after = DateTimeOffset.UtcNow;

            Assert.True(result.CompletedAt >= before);
            Assert.True(result.CompletedAt <= after);
        }

        [Fact]
        public void SuccessResult_ThrowsWhenResponseBytesIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                ModbusTransportResult.SuccessResult(null!, TimeSpan.Zero));
        }
    }
}
