namespace SerialAssistant.Infrastructure.Modbus.Transport;

/* Factory options for creating Modbus RTU transport.
 * Does not reference System.IO.Ports, WPF, or disk I/O APIs.
 * All timeout values are in milliseconds.
 */
public class ModbusRtuTransportFactoryOptions
{
    public string PortName { get; set; } = string.Empty;

    public int BaudRate { get; set; } = 9600;

    public int DataBits { get; set; } = 8;

    public string Parity { get; set; } = "None";

    public string StopBits { get; set; } = "One";

    public int ReadTimeoutMilliseconds { get; set; } = 1000;

    public int WriteTimeoutMilliseconds { get; set; } = 1000;

    public int SendTimeoutMilliseconds { get; set; } = 1000;

    public int ReceiveTimeoutMilliseconds { get; set; } = 1000;

    /* Validates options and returns error message if invalid.
     * Returns null if options are valid.
     */
    public string? Validate()
    {
        if (string.IsNullOrWhiteSpace(PortName))
        {
            return "PortName is required";
        }

        if (BaudRate <= 0)
        {
            return "BaudRate must be positive";
        }

        if (DataBits < 5 || DataBits > 8)
        {
            return "DataBits must be between 5 and 8";
        }

        if (ReadTimeoutMilliseconds < 0)
        {
            return "ReadTimeoutMilliseconds must be non-negative";
        }

        if (WriteTimeoutMilliseconds < 0)
        {
            return "WriteTimeoutMilliseconds must be non-negative";
        }

        if (SendTimeoutMilliseconds < 0)
        {
            return "SendTimeoutMilliseconds must be non-negative";
        }

        if (ReceiveTimeoutMilliseconds < 0)
        {
            return "ReceiveTimeoutMilliseconds must be non-negative";
        }

        return null;
    }
}