using System.Windows.Input;
using SerialAssistant.App.Commands;
using SerialAssistant.Core.Utilities;
using System.Text;

namespace SerialAssistant.App.ViewModels
{
    /*
     * ViewModel for receive display area UI
     */
    public class ReceiveDisplayViewModel : BaseViewModel
    {
        private List<byte> _receivedData;
        private string _receivedText;
        private bool _isHexDisplay;
        private int _receivedBytesCount;

        public ReceiveDisplayViewModel()
        {
            _receivedData = new List<byte>();
            _receivedText = string.Empty;
            _receivedBytesCount = 0;

            ClearCommand = new RelayCommand(Clear);
        }

        public string ReceivedText
        {
            get => _receivedText;
            private set => SetProperty(ref _receivedText, value);
        }

        public bool IsHexDisplay
        {
            get => _isHexDisplay;
            set
            {
                if (SetProperty(ref _isHexDisplay, value))
                {
                    UpdateDisplayText();
                }
            }
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

        public void AddReceivedData(byte[] data)
        {
            if (data == null || data.Length == 0)
            {
                return;
            }

            _receivedData.AddRange(data);
            ReceivedBytesCount = _receivedData.Count;
            UpdateDisplayText();
        }

        private void UpdateDisplayText()
        {
            if (_receivedData.Count == 0)
            {
                ReceivedText = string.Empty;
                return;
            }

            byte[] dataArray = _receivedData.ToArray();

            if (IsHexDisplay)
            {
                ReceivedText = HexConverter.ToHexString(dataArray);
            }
            else
            {
                ReceivedText = Encoding.UTF8.GetString(dataArray);
            }
        }

        private void Clear(object? parameter)
        {
            _receivedData.Clear();
            ReceivedText = string.Empty;
            ReceivedBytesCount = 0;
        }
    }
}
