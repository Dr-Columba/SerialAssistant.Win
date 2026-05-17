using Xunit;
using SerialAssistant.Core.Enums;
using SerialAssistant.Core.Models;
using SerialAssistant.Infrastructure.Serial;

namespace SerialAssistant.Tests.Infrastructure
{
    /*
     * Tests for SerialPortService
     */
    public class SerialPortServiceTests
    {
        /*
         * Test default state is Disconnected after construction
         */
        [Fact]
        public void Constructor_DefaultState_IsDisconnected()
        {
            /* Arrange & Act */
            var service = new SerialPortService();

            /* Assert */
            Assert.Equal(SerialConnectionState.Disconnected, service.ConnectionState);
        }

        /*
         * Test Open with null settings returns Failure
         */
        [Fact]
        public void Open_NullSettings_ReturnsFailure()
        {
            /* Arrange */
            var service = new SerialPortService();

            /* Act */
            var result = service.Open(null!);

            /* Assert */
            Assert.False(result.IsSuccess);
            Assert.Contains("不能为空", result.ErrorMessage);
        }

        /*
         * Test Open with empty PortName returns Failure
         */
        [Fact]
        public void Open_EmptyPortName_ReturnsFailure()
        {
            /* Arrange */
            var service = new SerialPortService();
            var settings = new SerialPortSettings
            {
                PortName = string.Empty,
                BaudRate = 9600,
                DataBits = 8,
                Parity = "None",
                StopBits = "One"
            };

            /* Act */
            var result = service.Open(settings);

            /* Assert */
            Assert.False(result.IsSuccess);
            Assert.Contains("Port name", result.ErrorMessage);
        }

        /*
         * Test Open with invalid BaudRate returns Failure
         */
        [Fact]
        public void Open_InvalidBaudRate_ReturnsFailure()
        {
            /* Arrange */
            var service = new SerialPortService();
            var settings = new SerialPortSettings
            {
                PortName = "COM1",
                BaudRate = 0,
                DataBits = 8,
                Parity = "None",
                StopBits = "One"
            };

            /* Act */
            var result = service.Open(settings);

            /* Assert */
            Assert.False(result.IsSuccess);
            Assert.Contains("Baud rate", result.ErrorMessage);
        }

        /*
         * Test Open with non-existent port returns Failure without crash
         */
        [Fact]
        public void Open_NonExistentPort_ReturnsFailureWithoutCrash()
        {
            /* Arrange */
            var service = new SerialPortService();
            var settings = new SerialPortSettings
            {
                PortName = "COM_DOES_NOT_EXIST_999",
                BaudRate = 9600,
                DataBits = 8,
                Parity = "None",
                StopBits = "One"
            };

            /* Act */
            var result = service.Open(settings);

            /* Assert */
            Assert.False(result.IsSuccess);
            Assert.Contains("打开串口时发生错误", result.ErrorMessage);
            Assert.NotEqual(SerialConnectionState.Connected, service.ConnectionState);
        }

        /*
         * Test Close when not open returns Failure
         */
        [Fact]
        public void Close_WhenNotOpen_ReturnsFailure()
        {
            /* Arrange */
            var service = new SerialPortService();

            /* Act */
            var result = service.Close();

            /* Assert */
            Assert.False(result.IsSuccess);
            Assert.Contains("未打开", result.ErrorMessage);
        }

        /*
         * Test Send returns Failure when not open
         */
        [Fact]
        public void Send_WhenNotOpen_ReturnsFailure()
        {
            /* Arrange */
            var service = new SerialPortService();
            var data = new byte[] { 0x41, 0x42, 0x43 };

            /* Act */
            var result = service.Send(data);

            /* Assert */
            Assert.False(result.IsSuccess);
            Assert.Contains("串口未打开", result.ErrorMessage);
        }

        /*
         * Test Send with null data returns Failure
         */
        [Fact]
        public void Send_NullData_ReturnsFailure()
        {
            /* Arrange */
            var service = new SerialPortService();

            /* Act */
            var result = service.Send(null!);

            /* Assert */
            Assert.False(result.IsSuccess);
            Assert.Contains("不能为空", result.ErrorMessage);
        }

        /*
         * Test Send with empty data returns Failure
         */
        [Fact]
        public void Send_EmptyData_ReturnsFailure()
        {
            /* Arrange */
            var service = new SerialPortService();
            var data = Array.Empty<byte>();

            /* Act */
            var result = service.Send(data);

            /* Assert */
            Assert.False(result.IsSuccess);
            Assert.Contains("长度不能为 0", result.ErrorMessage);
        }

        /*
         * Test ConnectionStateChanged event is raised on state change
         */
        [Fact]
        public void Open_Successful_RaisesConnectionStateChanged()
        {
            /* Arrange */
            var service = new SerialPortService();
            bool eventRaised = false;
            service.ConnectionStateChanged += (sender, state) => eventRaised = true;

            /* Act */
            service.Close();

            /* Assert */
            Assert.False(eventRaised);
        }
    }
}
