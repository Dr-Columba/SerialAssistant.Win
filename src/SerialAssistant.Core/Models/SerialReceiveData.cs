namespace SerialAssistant.Core.Models
{
    /*
     * Represents data received from a serial port
     */
    public class SerialReceiveData
    {
        public byte[] Data
        {
            get;
            set;
        } = Array.Empty<byte>();

        public DateTime ReceivedAt
        {
            get;
            set;
        } = DateTime.Now;

        public int Length => Data?.Length ?? 0;

        public static SerialReceiveData Create(byte[] data, DateTime? receivedAt = null)
        {
            return new SerialReceiveData
            {
                Data = data ?? Array.Empty<byte>(),
                ReceivedAt = receivedAt ?? DateTime.Now
            };
        }
    }
}
