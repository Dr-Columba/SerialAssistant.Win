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
         * Test clearing receive area clears text
         */
        [Fact]
        public void ClearReceive_ClearsText()
        {
            /* Arrange */
            var viewModel = new ReceiveDisplayViewModel();
            byte[] data = Encoding.UTF8.GetBytes("some data");
            viewModel.AddReceivedData(data);

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
            viewModel.AddReceivedData(data);

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
            byte[] data = Encoding.UTF8.GetBytes("ABC");

            /* Act */
            viewModel.AddReceivedData(data);

            /* Assert */
            Assert.Equal("ABC", viewModel.ReceivedText);
            Assert.Equal(3, viewModel.ReceivedBytesCount);
        }

        /*
         * Test AddReceivedData adds data and updates count
         */
        [Fact]
        public void AddReceivedData_IncreasesCount()
        {
            /* Arrange */
            var viewModel = new ReceiveDisplayViewModel();
            byte[] data1 = new byte[] { 0x41, 0x42 };
            byte[] data2 = new byte[] { 0x43, 0x44 };

            /* Act */
            viewModel.AddReceivedData(data1);
            viewModel.AddReceivedData(data2);

            /* Assert */
            Assert.Equal(4, viewModel.ReceivedBytesCount);
        }

        /*
         * Test HEX mode displays correct format
         */
        [Fact]
        public void AddReceivedData_HexMode_DisplaysHex()
        {
            /* Arrange */
            var viewModel = new ReceiveDisplayViewModel();
            viewModel.IsHexDisplay = true;
            byte[] data = new byte[] { 0x41, 0x42, 0x43 };

            /* Act */
            viewModel.AddReceivedData(data);

            /* Assert */
            Assert.Contains("41 42 43", viewModel.ReceivedText);
        }

        /*
         * Test switching to HEX mode re-displays existing data as HEX
         */
        [Fact]
        public void IsHexDisplay_True_ReDisplaysAsHex()
        {
            /* Arrange */
            var viewModel = new ReceiveDisplayViewModel();
            viewModel.IsHexDisplay = false;
            byte[] data = new byte[] { 0x41, 0x42, 0x43 };
            viewModel.AddReceivedData(data);
            Assert.Equal("ABC", viewModel.ReceivedText);

            /* Act */
            viewModel.IsHexDisplay = true;

            /* Assert */
            Assert.Contains("41 42 43", viewModel.ReceivedText);
        }

        /*
         * Test switching to text mode re-displays existing data as text
         */
        [Fact]
        public void IsHexDisplay_False_ReDisplaysAsText()
        {
            /* Arrange */
            var viewModel = new ReceiveDisplayViewModel();
            viewModel.IsHexDisplay = true;
            byte[] data = new byte[] { 0x41, 0x42, 0x43 };
            viewModel.AddReceivedData(data);

            /* Act */
            viewModel.IsHexDisplay = false;

            /* Assert */
            Assert.Equal("ABC", viewModel.ReceivedText);
        }

        /*
         * Test AddReceivedData with null data does nothing
         */
        [Fact]
        public void AddReceivedData_NullData_DoesNothing()
        {
            /* Arrange */
            var viewModel = new ReceiveDisplayViewModel();

            /* Act */
            viewModel.AddReceivedData(null!);

            /* Assert */
            Assert.Equal(string.Empty, viewModel.ReceivedText);
            Assert.Equal(0, viewModel.ReceivedBytesCount);
        }

        /*
         * Test AddReceivedData with empty array does nothing
         */
        [Fact]
        public void AddReceivedData_EmptyData_DoesNothing()
        {
            /* Arrange */
            var viewModel = new ReceiveDisplayViewModel();

            /* Act */
            viewModel.AddReceivedData(Array.Empty<byte>());

            /* Assert */
            Assert.Equal(string.Empty, viewModel.ReceivedText);
            Assert.Equal(0, viewModel.ReceivedBytesCount);
        }
    }
}
