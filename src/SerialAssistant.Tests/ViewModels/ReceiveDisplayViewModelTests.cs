using Xunit;
using SerialAssistant.App.ViewModels;

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
         * Test changing received text raises property change
         */
        [Fact]
        public void ReceivedText_ChangeRaisesPropertyChanged()
        {
            /* Arrange */
            var viewModel = new ReceiveDisplayViewModel();
            bool propertyChangedRaised = false;
            viewModel.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(ReceiveDisplayViewModel.ReceivedText))
                {
                    propertyChangedRaised = true;
                }
            };

            /* Act */
            viewModel.ReceivedText = "test data";

            /* Assert */
            Assert.True(propertyChangedRaised);
        }

        /*
         * Test clearing receive area clears text
         */
        [Fact]
        public void ClearReceive_ClearsText()
        {
            /* Arrange */
            var viewModel = new ReceiveDisplayViewModel();
            viewModel.ReceivedText = "some data";

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
            viewModel.ReceivedText = "some data";

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
         * Test ReceivedBytesCount updates when text changes
         */
        [Fact]
        public void ReceivedText_ChangeUpdatesCount()
        {
            /* Arrange */
            var viewModel = new ReceiveDisplayViewModel();

            /* Act */
            viewModel.ReceivedText = "hello";

            /* Assert */
            Assert.Equal(5, viewModel.ReceivedBytesCount);
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
         * Test AppendText adds text to existing text
         */
        [Fact]
        public void AppendText_AddsToExistingText()
        {
            /* Arrange */
            var viewModel = new ReceiveDisplayViewModel();
            viewModel.ReceivedText = "first ";

            /* Act */
            viewModel.AppendText("second");

            /* Assert */
            Assert.Equal("first second", viewModel.ReceivedText);
        }
    }
}
