using System.Windows.Input;
using SerialAssistant.App.Commands;
using SerialAssistant.Core.Utilities;
using SerialAssistant.Core.Enums;
using SerialAssistant.Core.Models;
using System.Text;

namespace SerialAssistant.App.ViewModels
{
    /*
     * ViewModel for receive display area UI
     */
    public class ReceiveDisplayViewModel : BaseViewModel
    {
        private List<CommunicationRecord> _records;
        private string _receivedText;
        private bool _isHexDisplay;
        private bool _showTimestamp;
        private bool _showDirection;
        private int _receivedBytesCount;

        public ReceiveDisplayViewModel()
        {
            _records = new List<CommunicationRecord>();
            _receivedText = string.Empty;
            _showTimestamp = true;
            _showDirection = true;
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

        public bool ShowTimestamp
        {
            get => _showTimestamp;
            set
            {
                if (SetProperty(ref _showTimestamp, value))
                {
                    UpdateDisplayText();
                }
            }
        }

        public bool ShowDirection
        {
            get => _showDirection;
            set
            {
                if (SetProperty(ref _showDirection, value))
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
            AddRxData(data);
        }

        public void AddTxData(byte[] data)
        {
            if (data == null || data.Length == 0)
            {
                return;
            }

            CommunicationRecord record = new CommunicationRecord(
                CommunicationDirection.Tx,
                data,
                DateTime.Now);

            _records.Add(record);
            UpdateDisplayText();
        }

        public void AddRxData(byte[] data)
        {
            if (data == null || data.Length == 0)
            {
                return;
            }

            CommunicationRecord record = new CommunicationRecord(
                CommunicationDirection.Rx,
                data,
                DateTime.Now);

            _records.Add(record);
            ReceivedBytesCount += data.Length;
            UpdateDisplayText();
        }

        private void UpdateDisplayText()
        {
            if (_records.Count == 0)
            {
                ReceivedText = string.Empty;
                return;
            }

            List<string> lines = new List<string>();

            foreach (CommunicationRecord record in _records)
            {
                string line = string.Empty;

                if (ShowTimestamp)
                {
                    line += $"[{record.Timestamp.ToString("HH:mm:ss.fff")}] ";
                }

                if (ShowDirection)
                {
                    line += record.Direction == CommunicationDirection.Tx ? "TX " : "RX ";
                }

                if (IsHexDisplay)
                {
                    line += HexConverter.ToHexString(record.Data);
                }
                else
                {
                    line += Encoding.UTF8.GetString(record.Data);
                }

                lines.Add(line);
            }

            ReceivedText = string.Join(Environment.NewLine, lines);
        }

        private void Clear(object? parameter)
        {
            _records.Clear();
            ReceivedText = string.Empty;
            ReceivedBytesCount = 0;
        }
    }
}
