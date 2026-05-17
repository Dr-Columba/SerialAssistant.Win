using SerialAssistant.Core.Enums;
using SerialAssistant.Core.Models;

namespace SerialAssistant.Core.Services
{
    /*
     * Interface for serial port communication service
     */
    public interface ISerialPortService
    {
        SerialConnectionState ConnectionState
        {
            get;
        }

        OperationResult Open(SerialPortSettings settings);

        OperationResult Close();

        OperationResult Send(byte[] data);

        event EventHandler<SerialReceiveData>? DataReceived;

        event EventHandler<Exception>? ErrorOccurred;

        event EventHandler<SerialConnectionState>? ConnectionStateChanged;
    }
}
