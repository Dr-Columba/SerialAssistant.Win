using Xunit;
using SerialAssistant.Core.Models;
using SerialAssistant.Core.Utilities;

namespace SerialAssistant.Tests.Validation
{
    /*
     * Tests for SerialSettingsValidator utility class
     */
    public class SerialSettingsValidatorTests
    {
        /*
         * Test valid default configuration passes validation
         */
        [Fact]
        public void Validate_ValidDefaultSettings_ReturnsSuccess()
        {
            /* Arrange */
            var settings = SerialPortSettings.CreateDefault();
            settings.PortName = "COM1";

            /* Act */
            var result = SerialSettingsValidator.Validate(settings);

            /* Assert */
            Assert.True(result.IsSuccess);
        }

        /*
         * Test null settings returns failure
         */
        [Fact]
        public void Validate_NullSettings_ReturnsFailure()
        {
            /* Arrange */
            SerialPortSettings? settings = null;

            /* Act */
            var result = SerialSettingsValidator.Validate(settings);

            /* Assert */
            Assert.False(result.IsSuccess);
            Assert.Contains("null", result.ErrorMessage);
        }

        /*
         * Test empty PortName returns failure
         */
        [Fact]
        public void Validate_EmptyPortName_ReturnsFailure()
        {
            /* Arrange */
            var settings = SerialPortSettings.CreateDefault();
            settings.PortName = string.Empty;

            /* Act */
            var result = SerialSettingsValidator.Validate(settings);

            /* Assert */
            Assert.False(result.IsSuccess);
            Assert.Contains("Port name", result.ErrorMessage);
        }

        /*
         * Test whitespace PortName returns failure
         */
        [Fact]
        public void Validate_WhitespacePortName_ReturnsFailure()
        {
            /* Arrange */
            var settings = SerialPortSettings.CreateDefault();
            settings.PortName = "   ";

            /* Act */
            var result = SerialSettingsValidator.Validate(settings);

            /* Assert */
            Assert.False(result.IsSuccess);
            Assert.Contains("Port name", result.ErrorMessage);
        }

        /*
         * Test zero BaudRate returns failure
         */
        [Fact]
        public void Validate_ZeroBaudRate_ReturnsFailure()
        {
            /* Arrange */
            var settings = SerialPortSettings.CreateDefault();
            settings.PortName = "COM1";
            settings.BaudRate = 0;

            /* Act */
            var result = SerialSettingsValidator.Validate(settings);

            /* Assert */
            Assert.False(result.IsSuccess);
            Assert.Contains("Baud rate", result.ErrorMessage);
        }

        /*
         * Test negative BaudRate returns failure
         */
        [Fact]
        public void Validate_NegativeBaudRate_ReturnsFailure()
        {
            /* Arrange */
            var settings = SerialPortSettings.CreateDefault();
            settings.PortName = "COM1";
            settings.BaudRate = -1;

            /* Act */
            var result = SerialSettingsValidator.Validate(settings);

            /* Assert */
            Assert.False(result.IsSuccess);
            Assert.Contains("Baud rate", result.ErrorMessage);
        }

        /*
         * Test invalid DataBits returns failure
         */
        [Fact]
        public void Validate_InvalidDataBits_ReturnsFailure()
        {
            /* Arrange */
            var settings = SerialPortSettings.CreateDefault();
            settings.PortName = "COM1";
            settings.DataBits = 4;

            /* Act */
            var result = SerialSettingsValidator.Validate(settings);

            /* Assert */
            Assert.False(result.IsSuccess);
            Assert.Contains("Data bits", result.ErrorMessage);
        }

        /*
         * Test each valid DataBits value passes
         */
        [Theory]
        [InlineData(5)]
        [InlineData(6)]
        [InlineData(7)]
        [InlineData(8)]
        public void Validate_ValidDataBits_ReturnsSuccess(int dataBits)
        {
            /* Arrange */
            var settings = SerialPortSettings.CreateDefault();
            settings.PortName = "COM1";
            settings.DataBits = dataBits;

            /* Act */
            var result = SerialSettingsValidator.Validate(settings);

            /* Assert */
            Assert.True(result.IsSuccess);
        }

        /*
         * Test invalid Parity returns failure
         */
        [Fact]
        public void Validate_InvalidParity_ReturnsFailure()
        {
            /* Arrange */
            var settings = SerialPortSettings.CreateDefault();
            settings.PortName = "COM1";
            settings.Parity = "Invalid";

            /* Act */
            var result = SerialSettingsValidator.Validate(settings);

            /* Assert */
            Assert.False(result.IsSuccess);
            Assert.Contains("Parity", result.ErrorMessage);
        }

        /*
         * Test each valid Parity value passes
         */
        [Theory]
        [InlineData("None")]
        [InlineData("Odd")]
        [InlineData("Even")]
        [InlineData("Mark")]
        [InlineData("Space")]
        public void Validate_ValidParity_ReturnsSuccess(string parity)
        {
            /* Arrange */
            var settings = SerialPortSettings.CreateDefault();
            settings.PortName = "COM1";
            settings.Parity = parity;

            /* Act */
            var result = SerialSettingsValidator.Validate(settings);

            /* Assert */
            Assert.True(result.IsSuccess);
        }

        /*
         * Test invalid StopBits returns failure
         */
        [Fact]
        public void Validate_InvalidStopBits_ReturnsFailure()
        {
            /* Arrange */
            var settings = SerialPortSettings.CreateDefault();
            settings.PortName = "COM1";
            settings.StopBits = "Invalid";

            /* Act */
            var result = SerialSettingsValidator.Validate(settings);

            /* Assert */
            Assert.False(result.IsSuccess);
            Assert.Contains("Stop bits", result.ErrorMessage);
        }

        /*
         * Test each valid StopBits value passes
         */
        [Theory]
        [InlineData("None")]
        [InlineData("One")]
        [InlineData("Two")]
        [InlineData("OnePointFive")]
        public void Validate_ValidStopBits_ReturnsSuccess(string stopBits)
        {
            /* Arrange */
            var settings = SerialPortSettings.CreateDefault();
            settings.PortName = "COM1";
            settings.StopBits = stopBits;

            /* Act */
            var result = SerialSettingsValidator.Validate(settings);

            /* Assert */
            Assert.True(result.IsSuccess);
        }

        /*
         * Test negative ReadTimeout returns failure
         */
        [Fact]
        public void Validate_NegativeReadTimeout_ReturnsFailure()
        {
            /* Arrange */
            var settings = SerialPortSettings.CreateDefault();
            settings.PortName = "COM1";
            settings.ReadTimeout = -1;

            /* Act */
            var result = SerialSettingsValidator.Validate(settings);

            /* Assert */
            Assert.False(result.IsSuccess);
            Assert.Contains("Read timeout", result.ErrorMessage);
        }

        /*
         * Test zero ReadTimeout passes validation
         */
        [Fact]
        public void Validate_ZeroReadTimeout_ReturnsSuccess()
        {
            /* Arrange */
            var settings = SerialPortSettings.CreateDefault();
            settings.PortName = "COM1";
            settings.ReadTimeout = 0;

            /* Act */
            var result = SerialSettingsValidator.Validate(settings);

            /* Assert */
            Assert.True(result.IsSuccess);
        }

        /*
         * Test negative WriteTimeout returns failure
         */
        [Fact]
        public void Validate_NegativeWriteTimeout_ReturnsFailure()
        {
            /* Arrange */
            var settings = SerialPortSettings.CreateDefault();
            settings.PortName = "COM1";
            settings.WriteTimeout = -1;

            /* Act */
            var result = SerialSettingsValidator.Validate(settings);

            /* Assert */
            Assert.False(result.IsSuccess);
            Assert.Contains("Write timeout", result.ErrorMessage);
        }

        /*
         * Test zero WriteTimeout passes validation
         */
        [Fact]
        public void Validate_ZeroWriteTimeout_ReturnsSuccess()
        {
            /* Arrange */
            var settings = SerialPortSettings.CreateDefault();
            settings.PortName = "COM1";
            settings.WriteTimeout = 0;

            /* Act */
            var result = SerialSettingsValidator.Validate(settings);

            /* Assert */
            Assert.True(result.IsSuccess);
        }
    }
}
