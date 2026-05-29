namespace SerialAssistant.Core.Modbus.Transport
{
    public sealed class ModbusTransportOptions
    {
        public TimeSpan ConnectTimeout { get; set; }

        public TimeSpan SendTimeout { get; set; }

        public TimeSpan ReceiveTimeout { get; set; }

        public bool ValidateResponse { get; set; }

        public int MaxResponseBytes { get; set; }

        public ModbusTransportOptions()
        {
            ConnectTimeout = TimeSpan.FromSeconds(5);
            SendTimeout = TimeSpan.FromSeconds(5);
            ReceiveTimeout = TimeSpan.FromSeconds(5);
            ValidateResponse = true;
            MaxResponseBytes = 260;
        }

        public bool IsValid()
        {
            if (ConnectTimeout <= TimeSpan.Zero)
            {
                return false;
            }

            if (SendTimeout <= TimeSpan.Zero)
            {
                return false;
            }

            if (ReceiveTimeout <= TimeSpan.Zero)
            {
                return false;
            }

            if (MaxResponseBytes <= 0)
            {
                return false;
            }

            return true;
        }
    }
}
