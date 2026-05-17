namespace SerialAssistant.Core.Models
{
    /*
     * Represents information about an available serial port
     */
    public class SerialPortInfo
    {
        public string PortName
        {
            get;
            set;
        } = string.Empty;

        public string DisplayName
        {
            get;
            set;
        } = string.Empty;

        public string Description
        {
            get;
            set;
        } = string.Empty;

        public static SerialPortInfo Create(string portName, string description = "")
        {
            return new SerialPortInfo
            {
                PortName = portName,
                DisplayName = string.IsNullOrEmpty(description) ? portName : $"{portName} - {description}",
                Description = description ?? string.Empty
            };
        }
    }
}
