using Xunit;
using SerialAssistant.App.ViewModels;
using System.Text;

namespace SerialAssistant.Tests.ViewModels
{
    /*
     * Tests for ReceiveDisplayViewModel
     */
    public class ReceiveDisplayViewModelTests
    {
        /*
         * Test default received text is empty
         */
        [Fact]
        public void DefaultReceivedText_IsEmpty()
        {
            /* Arrange */
            var viewModel = new ReceiveDisplayViewModel();

            /* Act & Assert */
            Assert.Equal(string.Empty, viewModel.ReceivedText);
        }

        /*
         * Test default received count is 0
         */
        [Fact]
        public void DefaultReceivedCount_Is0()
        {
            /* Arrange */
            var viewModel = new ReceiveDisplayViewModel();

            /* Act & Assert */
            Assert.Equal(0, viewModel.ReceivedBytesCount);
        }

        /*
         * Test default ShowTimestamp is true
         */
        [Fact]
        public void DefaultShowTimestamp_IsTrue()
        {
            /* Arrange */
            var viewModel = new ReceiveDisplayViewModel();

            /* Act & Assert */
            Assert.True(viewModel.ShowTimestamp);
        }

        /*
         * Test default ShowDirection is true
         */
        [Fact]
        public void DefaultShowDirection_IsTrue()
        {
            /* Arrange */
            var viewModel = new ReceiveDisplayViewModel();

            /* Act & Assert */
            Assert.True(viewModel.ShowDirection);
        }

        /*
         * Test clearing receive area clears text
         */
        [Fact]
        public void ClearReceive_ClearsText()
        {
            /* Arrange */
            var viewModel = new ReceiveDisplayViewModel();
            byte[] data = Encoding.UTF8.GetBytes("some data");
            viewModel.AddRxData(data);

            /* Act */
            viewModel.ClearCommand.Execute(null);

            /* Assert */
            Assert.Equal(string.Empty, viewModel.ReceivedText);
        }

        /*
         * Test clearing receive area resets count to 0
         */
        [Fact]
        public void ClearReceive_ResetsCountTo0()
        {
            /* Arrange */
            var viewModel = new ReceiveDisplayViewModel();
            byte[] data = Encoding.UTF8.GetBytes("some data");
            viewModel.AddRxData(data);

            /* Act */
            viewModel.ClearCommand.Execute(null);

            /* Assert */
            Assert.Equal(0, viewModel.ReceivedBytesCount);
        }

        /*
         * Test ClearCommand can be executed
         */
        [Fact]
        public void ClearCommand_CanExecute()
        {
            /* Arrange */
            var viewModel = new ReceiveDisplayViewModel();

            /* Act & Assert */
            Assert.NotNull(viewModel.ClearCommand);
            Assert.True(viewModel.ClearCommand.CanExecute(null));
        }

        /*
         * Test default IsHexDisplay is false
         */
        [Fact]
        public void DefaultIsHexDisplay_IsFalse()
        {
            /* Arrange */
            var viewModel = new ReceiveDisplayViewModel();

            /* Act & Assert */
            Assert.False(viewModel.IsHexDisplay);
        }

        /*
         * Test IsHexDisplay can be changed
         */
        [Fact]
        public void IsHexDisplay_CanBeChanged()
        {
            /* Arrange */
            var viewModel = new ReceiveDisplayViewModel();

            /* Act */
            viewModel.IsHexDisplay = true;

            /* Assert */
            Assert.True(viewModel.IsHexDisplay);
        }

        /*
         * Test AddReceivedData adds text data and updates display
         */
        [Fact]
        public void AddReceivedData_TextMode_AddsText()
        {
            /* Arrange */
            var viewModel = new ReceiveDisplayViewModel();
            viewModel.IsHexDisplay = false;
            viewModel.ShowTimestamp = false;
            viewModel.ShowDirection = false;
            byte[] data = Encoding.UTF8.GetBytes("ABC");

            /* Act */
            viewModel.AddReceivedData(data);

            /* Assert */
            Assert.Equal("ABC", viewModel.ReceivedText);
            Assert.Equal(3, viewModel.ReceivedBytesCount);
        }

        /*
         * Test AddTxData displays TX
         */
        [Fact]
        public void AddTxData_ShowsTx()
        {
            /* Arrange */
            var viewModel = new ReceiveDisplayViewModel();
            viewModel.ShowTimestamp = false;
            viewModel.ShowDirection = true;
            byte[] data = Encoding.UTF8.GetBytes("ABC");

            /* Act */
            viewModel.AddTxData(data);

            /* Assert */
            Assert.Contains("TX ", viewModel.ReceivedText);
        }

        /*
         * Test AddRxData displays RX
         */
        [Fact]
        public void AddRxData_ShowsRx()
        {
            /* Arrange */
            var viewModel = new ReceiveDisplayViewModel();
            viewModel.ShowTimestamp = false;
            viewModel.ShowDirection = true;
            byte[] data = Encoding.UTF8.GetBytes("ABC");

            /* Act */
            viewModel.AddRxData(data);

            /* Assert */
            Assert.Contains("RX ", viewModel.ReceivedText);
        }

        /*
         * Test ShowTimestamp true shows timestamp
         */
        [Fact]
        public void ShowTimestamp_True_ShowsTimestamp()
        {
            /* Arrange */
            var viewModel = new ReceiveDisplayViewModel();
            viewModel.ShowTimestamp = true;
            viewModel.ShowDirection = false;
            byte[] data = Encoding.UTF8.GetBytes("ABC");

            /* Act */
            viewModel.AddRxData(data);

            /* Assert */
            Assert.Matches(@"\[\d{2}:\d{2}:\d{2}\.\d{3}\]", viewModel.ReceivedText);
        }

        /*
         * Test ShowTimestamp false hides timestamp
         */
        [Fact]
        public void ShowTimestamp_False_HidesTimestamp()
        {
            /* Arrange */
            var viewModel = new ReceiveDisplayViewModel();
            viewModel.ShowTimestamp = false;
            viewModel.ShowDirection = false;
            byte[] data = Encoding.UTF8.GetBytes("ABC");

            /* Act */
            viewModel.AddRxData(data);

            /* Assert */
            Assert.DoesNotContain("[", viewModel.ReceivedText);
            Assert.DoesNotContain("]", viewModel.ReceivedText);
        }

        /*
         * Test ShowDirection true shows TX/RX
         */
        [Fact]
        public void ShowDirection_True_ShowsDirection()
        {
            /* Arrange */
            var viewModel = new ReceiveDisplayViewModel();
            viewModel.ShowTimestamp = false;
            viewModel.ShowDirection = true;
            byte[] data = Encoding.UTF8.GetBytes("ABC");

            /* Act */
            viewModel.AddTxData(data);

            /* Assert */
            Assert.Contains("TX ", viewModel.ReceivedText);
        }

        /*
         * Test ShowDirection false hides TX/RX
         */
        [Fact]
        public void ShowDirection_False_HidesDirection()
        {
            /* Arrange */
            var viewModel = new ReceiveDisplayViewModel();
            viewModel.ShowTimestamp = false;
            viewModel.ShowDirection = false;
            byte[] data = Encoding.UTF8.GetBytes("ABC");

            /* Act */
            viewModel.AddTxData(data);

            /* Assert */
            Assert.DoesNotContain("TX ", viewModel.ReceivedText);
            Assert.DoesNotContain("RX ", viewModel.ReceivedText);
        }

        /*
         * Test text mode TX ABC displays ABC
         */
        [Fact]
        public void AddTxData_TextMode_DisplaysText()
        {
            /* Arrange */
            var viewModel = new ReceiveDisplayViewModel();
            viewModel.IsHexDisplay = false;
            viewModel.ShowTimestamp = false;
            viewModel.ShowDirection = false;
            byte[] data = Encoding.UTF8.GetBytes("ABC");

            /* Act */
            viewModel.AddTxData(data);

            /* Assert */
            Assert.Equal("ABC", viewModel.ReceivedText);
        }

        /*
         * Test HEX mode TX ABC displays 41 42 43
         */
        [Fact]
        public void AddTxData_HexMode_DisplaysHex()
        {
            /* Arrange */
            var viewModel = new ReceiveDisplayViewModel();
            viewModel.IsHexDisplay = true;
            viewModel.ShowTimestamp = false;
            viewModel.ShowDirection = false;
            byte[] data = Encoding.UTF8.GetBytes("ABC");

            /* Act */
            viewModel.AddTxData(data);

            /* Assert */
            Assert.Contains("41 42 43", viewModel.ReceivedText);
        }

        /*
         * Test text mode RX OK displays OK
         */
        [Fact]
        public void AddRxData_TextMode_DisplaysText()
        {
            /* Arrange */
            var viewModel = new ReceiveDisplayViewModel();
            viewModel.IsHexDisplay = false;
            viewModel.ShowTimestamp = false;
            viewModel.ShowDirection = false;
            byte[] data = Encoding.UTF8.GetBytes("OK");

            /* Act */
            viewModel.AddRxData(data);

            /* Assert */
            Assert.Equal("OK", viewModel.ReceivedText);
        }

        /*
         * Test HEX mode RX OK displays 4F 4B
         */
        [Fact]
        public void AddRxData_HexMode_DisplaysHex()
        {
            /* Arrange */
            var viewModel = new ReceiveDisplayViewModel();
            viewModel.IsHexDisplay = true;
            viewModel.ShowTimestamp = false;
            viewModel.ShowDirection = false;
            byte[] data = Encoding.UTF8.GetBytes("OK");

            /* Act */
            viewModel.AddRxData(data);

            /* Assert */
            Assert.Contains("4F 4B", viewModel.ReceivedText);
        }

        /*
         * Test text mode to HEX mode re-displays all records as HEX
         */
        [Fact]
        public void IsHexDisplay_True_ReDisplaysAllAsHex()
        {
            /* Arrange */
            var viewModel = new ReceiveDisplayViewModel();
            viewModel.IsHexDisplay = false;
            viewModel.ShowTimestamp = false;
            viewModel.ShowDirection = false;
            byte[] data = new byte[] { 0x41, 0x42, 0x43 };
            viewModel.AddRxData(data);
            Assert.Equal("ABC", viewModel.ReceivedText);

            /* Act */
            viewModel.IsHexDisplay = true;

            /* Assert */
            Assert.Contains("41 42 43", viewModel.ReceivedText);
        }

        /*
         * Test HEX mode to text mode re-displays all records as text
         */
        [Fact]
        public void IsHexDisplay_False_ReDisplaysAllAsText()
        {
            /* Arrange */
            var viewModel = new ReceiveDisplayViewModel();
            viewModel.IsHexDisplay = true;
            viewModel.ShowTimestamp = false;
            viewModel.ShowDirection = false;
            byte[] data = new byte[] { 0x41, 0x42, 0x43 };
            viewModel.AddRxData(data);

            /* Act */
            viewModel.IsHexDisplay = false;

            /* Assert */
            Assert.Equal("ABC", viewModel.ReceivedText);
        }

        /*
         * Test AddReceivedData adds data and updates count
         */
        [Fact]
        public void AddRxData_IncreasesCount()
        {
            /* Arrange */
            var viewModel = new ReceiveDisplayViewModel();
            byte[] data1 = new byte[] { 0x41, 0x42 };
            byte[] data2 = new byte[] { 0x43, 0x44 };

            /* Act */
            viewModel.AddRxData(data1);
            viewModel.AddRxData(data2);

            /* Assert */
            Assert.Equal(4, viewModel.ReceivedBytesCount);
        }

        /*
         * Test AddTxData does not increase count
         */
        [Fact]
        public void AddTxData_DoesNotIncreaseCount()
        {
            /* Arrange */
            var viewModel = new ReceiveDisplayViewModel();
            byte[] data = new byte[] { 0x41, 0x42 };

            /* Act */
            viewModel.AddTxData(data);

            /* Assert */
            Assert.Equal(0, viewModel.ReceivedBytesCount);
        }

        /*
         * Test AddReceivedData with null data does nothing
         */
        [Fact]
        public void AddRxData_NullData_DoesNothing()
        {
            /* Arrange */
            var viewModel = new ReceiveDisplayViewModel();

            /* Act */
            viewModel.AddRxData(null!);

            /* Assert */
            Assert.Equal(string.Empty, viewModel.ReceivedText);
            Assert.Equal(0, viewModel.ReceivedBytesCount);
        }

        /*
         * Test AddReceivedData with empty array does nothing
         */
        [Fact]
        public void AddRxData_EmptyData_DoesNothing()
        {
            /* Arrange */
            var viewModel = new ReceiveDisplayViewModel();

            /* Act */
            viewModel.AddRxData(Array.Empty<byte>());

            /* Assert */
            Assert.Equal(string.Empty, viewModel.ReceivedText);
            Assert.Equal(0, viewModel.ReceivedBytesCount);
        }

        /*
         * Test AddTxData with null data does nothing
         */
        [Fact]
        public void AddTxData_NullData_DoesNothing()
        {
            /* Arrange */
            var viewModel = new ReceiveDisplayViewModel();

            /* Act */
            viewModel.AddTxData(null!);

            /* Assert */
            Assert.Equal(string.Empty, viewModel.ReceivedText);
            Assert.Equal(0, viewModel.ReceivedBytesCount);
        }

        /*
         * Test AddTxData with empty array does nothing
         */
        [Fact]
        public void AddTxData_EmptyData_DoesNothing()
        {
            /* Arrange */
            var viewModel = new ReceiveDisplayViewModel();

            /* Act */
            viewModel.AddTxData(Array.Empty<byte>());

            /* Assert */
            Assert.Equal(string.Empty, viewModel.ReceivedText);
            Assert.Equal(0, viewModel.ReceivedBytesCount);
        }
    }
}
