using Xunit;
using SerialAssistant.App.Commands;
using System.Windows.Input;

namespace SerialAssistant.Tests.Commands
{
    /*
     * Tests for RelayCommand class
     */
    public class RelayCommandTests
    {
        /*
         * Test Execute calls the target delegate
         */
        [Fact]
        public void Execute_CallsTargetDelegate()
        {
            /* Arrange */
            bool wasCalled = false;
            var command = new RelayCommand(_ => wasCalled = true);

            /* Act */
            command.Execute(null);

            /* Assert */
            Assert.True(wasCalled);
        }

        /*
         * Test CanExecute returns true by default when no predicate provided
         */
        [Fact]
        public void CanExecute_NoPredicateProvided_ReturnsTrue()
        {
            /* Arrange */
            var command = new RelayCommand(_ => { });

            /* Act */
            bool canExecute = command.CanExecute(null);

            /* Assert */
            Assert.True(canExecute);
        }

        /*
         * Test CanExecute returns false when predicate returns false
         */
        [Fact]
        public void CanExecute_PredicateReturnsFalse_ReturnsFalse()
        {
            /* Arrange */
            var command = new RelayCommand(_ => { }, _ => false);

            /* Act */
            bool canExecute = command.CanExecute(null);

            /* Assert */
            Assert.False(canExecute);
        }

        /*
         * Test CanExecute returns true when predicate returns true
         */
        [Fact]
        public void CanExecute_PredicateReturnsTrue_ReturnsTrue()
        {
            /* Arrange */
            var command = new RelayCommand(_ => { }, _ => true);

            /* Act */
            bool canExecute = command.CanExecute(null);

            /* Assert */
            Assert.True(canExecute);
        }

        /*
         * Test RaiseCanExecuteChanged triggers event
         */
        [Fact]
        public void RaiseCanExecuteChanged_TriggersEvent()
        {
            /* Arrange */
            var command = new RelayCommand(_ => { });
            bool eventRaised = false;
            EventHandler handler = (sender, args) => eventRaised = true;
            command.CanExecuteChanged += handler;

            /* Act */
            command.RaiseCanExecuteChanged();

            /* Assert */
            Assert.True(eventRaised);

            /* Cleanup */
            command.CanExecuteChanged -= handler;
        }

        /*
         * Test RelayCommand with no parameter action
         */
        [Fact]
        public void Execute_NoParameterAction_Works()
        {
            /* Arrange */
            int counter = 0;
            var command = new RelayCommand(() => counter++);

            /* Act */
            command.Execute(null);

            /* Assert */
            Assert.Equal(1, counter);
        }

        /*
         * Test RelayCommand with parameter passed to action
         */
        [Fact]
        public void Execute_ParameterPassedToAction()
        {
            /* Arrange */
            object? receivedParameter = null;
            var command = new RelayCommand(p => receivedParameter = p);

            /* Act */
            command.Execute("test parameter");

            /* Assert */
            Assert.Equal("test parameter", receivedParameter);
        }
    }
}
