using Xunit;
using SerialAssistant.Core.Modbus.Transport;

namespace SerialAssistant.Tests.Modbus.Transport
{
    public class ModbusTransportOptionsTests
    {
        [Fact]
        public void DefaultOptions_AreValid()
        {
            var options = new ModbusTransportOptions();

            Assert.True(options.IsValid());
        }

        [Fact]
        public void Validate_ReturnsFalse_WhenConnectTimeoutIsZero()
        {
            var options = new ModbusTransportOptions
            {
                ConnectTimeout = TimeSpan.Zero
            };

            Assert.False(options.IsValid());
        }

        [Fact]
        public void Validate_ReturnsFalse_WhenSendTimeoutIsZero()
        {
            var options = new ModbusTransportOptions
            {
                SendTimeout = TimeSpan.Zero
            };

            Assert.False(options.IsValid());
        }

        [Fact]
        public void Validate_ReturnsFalse_WhenReceiveTimeoutIsZero()
        {
            var options = new ModbusTransportOptions
            {
                ReceiveTimeout = TimeSpan.Zero
            };

            Assert.False(options.IsValid());
        }

        [Fact]
        public void Validate_ReturnsFalse_WhenMaxResponseBytesIsZero()
        {
            var options = new ModbusTransportOptions
            {
                MaxResponseBytes = 0
            };

            Assert.False(options.IsValid());
        }

        [Fact]
        public void Validate_ReturnsTrue_WhenAllValuesAreValid()
        {
            var options = new ModbusTransportOptions
            {
                ConnectTimeout = TimeSpan.FromSeconds(10),
                SendTimeout = TimeSpan.FromSeconds(5),
                ReceiveTimeout = TimeSpan.FromSeconds(3),
                MaxResponseBytes = 500,
                ValidateResponse = true
            };

            Assert.True(options.IsValid());
        }

        [Fact]
        public void DefaultValues_AreCorrect()
        {
            var options = new ModbusTransportOptions();

            Assert.Equal(TimeSpan.FromSeconds(5), options.ConnectTimeout);
            Assert.Equal(TimeSpan.FromSeconds(5), options.SendTimeout);
            Assert.Equal(TimeSpan.FromSeconds(5), options.ReceiveTimeout);
            Assert.True(options.ValidateResponse);
            Assert.Equal(260, options.MaxResponseBytes);
        }
    }
}
