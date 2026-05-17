using System.Windows.Input;

namespace SerialAssistant.App.Commands
{
    /*
     * A simple ICommand implementation for MVVM pattern
     */
    public class RelayCommand : ICommand
    {
        private readonly Action<object?> _execute;
        private readonly Func<object?, bool>? _canExecute;

        public RelayCommand(Action<object?> execute, Func<object?, bool>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public RelayCommand(Action execute, Func<bool>? canExecute = null)
            : this(_ => execute(), canExecute != null ? _ => canExecute() : null)
        {
        }

        private event EventHandler? CanExecuteChangedInternal;

        public event EventHandler? CanExecuteChanged
        {
            add
            {
                CanExecuteChangedInternal += value;
                CommandManager.RequerySuggested += value;
            }
            remove
            {
                CanExecuteChangedInternal -= value;
                CommandManager.RequerySuggested -= value;
            }
        }

        public bool CanExecute(object? parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        public void Execute(object? parameter)
        {
            _execute(parameter);
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChangedInternal?.Invoke(this, EventArgs.Empty);
            CommandManager.InvalidateRequerySuggested();
        }
    }
}
