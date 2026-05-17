namespace SerialAssistant.Core.Models
{
    /*
     * Represents serial port configuration settings
     */
    public class SerialPortSettings
    {
        public string PortName
        {
            get;
            set;
        } = string.Empty;

        public int BaudRate
        {
            get;
            set;
        } = 9600;

        public int DataBits
        {
            get;
            set;
        } = 8;

        public string Parity
        {
            get;
            set;
        } = "None";

        public string StopBits
        {
            get;
            set;
        } = "One";

        public int ReadTimeout
        {
            get;
            set;
        } = 1000;

        public int WriteTimeout
        {
            get;
            set;
        } = 1000;

        public static SerialPortSettings CreateDefault()
        {
            return new SerialPortSettings
            {
                PortName = string.Empty,
                BaudRate = 9600,
                DataBits = 8,
                Parity = "None",
                StopBits = "One",
                ReadTimeout = 1000,
                WriteTimeout = 1000
            };
        }
    }
}
