using SerialAssistant.Core.Enums;

namespace SerialAssistant.Core.Models
{
    /*
     * Represents a send history item containing user input and send mode
     */
    public class SendHistoryItem
    {
        public string Content
        {
            get;
            set;
        }

        public SendMode SendMode
        {
            get;
            set;
        }

        public SendHistoryItem()
        {
            Content = string.Empty;
            SendMode = SendMode.Text;
        }

        public SendHistoryItem(string content, SendMode sendMode)
        {
            Content = content ?? string.Empty;
            SendMode = sendMode;
        }
    }
}
