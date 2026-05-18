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
        }

        public SendMode SendMode
        {
            get;
        }

        public SendHistoryItem(string content, SendMode sendMode)
        {
            Content = content ?? string.Empty;
            SendMode = sendMode;
        }
    }
}
