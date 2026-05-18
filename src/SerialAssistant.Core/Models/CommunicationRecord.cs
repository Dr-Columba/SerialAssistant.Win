using SerialAssistant.Core.Enums;

namespace SerialAssistant.Core.Models
{
    /*
     * Represents a single communication record
     */
    public class CommunicationRecord
    {
        public CommunicationDirection Direction
        {
            get;
        }

        public byte[] Data
        {
            get;
        }

        public DateTime Timestamp
        {
            get;
        }

        public CommunicationRecord(CommunicationDirection direction, byte[] data, DateTime timestamp)
        {
            Direction = direction;
            Data = (byte[])data.Clone();
            Timestamp = timestamp;
        }
    }
}
