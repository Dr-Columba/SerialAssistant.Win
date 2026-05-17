using System.Windows.Input;
using SerialAssistant.App.Commands;

namespace SerialAssistant.App.ViewModels
{
    /*
     * ViewModel for receive display area UI
     */
    public class ReceiveDisplayViewModel : BaseViewModel
    {
        private string _receivedText;
        private bool _isHexDisplay;
        private int _receivedBytesCount;

        public ReceiveDisplayViewModel()
        {
            _receivedText = string.Empty;
            _receivedBytesCount = 0;

            ClearCommand = new RelayCommand(Clear);
        }

        public string ReceivedText
        {
            get => _receivedText;
            set
            {
                if (SetProperty(ref _receivedText, value))
                {
                    ReceivedBytesCount = System.Text.Encoding.UTF8.GetByteCount(value);
                }
            }
        }

        public bool IsHexDisplay
        {
            get => _isHexDisplay;
            set => SetProperty(ref _isHexDisplay, value);
        }

        public int ReceivedBytesCount
        {
            get => _receivedBytesCount;
            private set => SetProperty(ref _receivedBytesCount, value);
        }

        public ICommand ClearCommand
        {
            get;
            private set;
        }

        private void Clear(object? parameter)
        {
            ReceivedText = string.Empty;
            ReceivedBytesCount = 0;
        }

        public void AppendText(string text)
        {
            ReceivedText += text;
        }
    }
}
