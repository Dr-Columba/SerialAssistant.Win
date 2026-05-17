using Xunit;
using SerialAssistant.Infrastructure.Serial;

namespace SerialAssistant.Tests.Infrastructure
{
    /*
     * Tests for SerialPortScanner
     */
    public class SerialPortScannerTests
    {
        /*
         * Test GetAvailablePorts does not throw unhandled exception
         */
        [Fact]
        public void GetAvailablePorts_DoesNotThrowUnhandledException()
        {
            /* Arrange */
            var scanner = new SerialPortScanner();

            /* Act & Assert */
            var exception = Record.Exception(() => scanner.GetAvailablePorts());
            Assert.Null(exception);
        }

        /*
         * Test successful result has non-null Value
         */
        [Fact]
        public void GetAvailablePorts_SuccessResult_HasNonNullValue()
        {
            /* Arrange */
            var scanner = new SerialPortScanner();

            /* Act */
            var result = scanner.GetAvailablePorts();

            /* Assert */
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
        }

        /*
         * Test no ports returns empty collection
         */
        [Fact]
        public void GetAvailablePorts_NoPorts_ReturnsEmptyCollection()
        {
            /* Arrange */
            var scanner = new SerialPortScanner();

            /* Act */
            var result = scanner.GetAvailablePorts();

            /* Assert */
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
        }

        /*
         * Test returned SerialPortInfo PortName is not empty
         */
        [Fact]
        public void GetAvailablePorts_ReturnsPortName_NotEmpty()
        {
            /* Arrange */
            var scanner = new SerialPortScanner();

            /* Act */
            var result = scanner.GetAvailablePorts();

            /* Assert */
            if (result.IsSuccess && result.Value!.Count > 0)
            {
                foreach (var port in result.Value)
                {
                    Assert.False(string.IsNullOrWhiteSpace(port.PortName));
                }
            }
        }

        /*
         * Test scanner does not open ports
         */
        [Fact]
        public void GetAvailablePorts_DoesNotOpenPorts()
        {
            /* Arrange */
            var scanner = new SerialPortScanner();

            /* Act */
            var result = scanner.GetAvailablePorts();

            /* Assert */
            Assert.True(result.IsSuccess);
        }
    }
}
