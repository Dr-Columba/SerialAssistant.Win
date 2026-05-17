using SerialAssistant.Core.Models;

namespace SerialAssistant.Core.Services
{
    /*
     * Interface for scanning available serial ports
     */
    public interface ISerialPortScanner
    {
        OperationResult<IReadOnlyList<SerialPortInfo>> GetAvailablePorts();
    }
}
