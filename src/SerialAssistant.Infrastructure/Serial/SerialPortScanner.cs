using System.IO.Ports;
using SerialAssistant.Core.Models;
using SerialAssistant.Core.Services;

namespace SerialAssistant.Infrastructure.Serial
{
    /*
     * Implementation of ISerialPortScanner using System.IO.Ports
     */
    public class SerialPortScanner : ISerialPortScanner
    {
        public OperationResult<IReadOnlyList<SerialPortInfo>> GetAvailablePorts()
        {
            try
            {
                string[] portNames = SerialPort.GetPortNames();
                List<SerialPortInfo> ports = new List<SerialPortInfo>();

                foreach (string portName in portNames)
                {
                    if (!string.IsNullOrWhiteSpace(portName))
                    {
                        ports.Add(SerialPortInfo.Create(portName, string.Empty));
                    }
                }

                return OperationResult<IReadOnlyList<SerialPortInfo>>.Success(ports);
            }
            catch (Exception ex)
            {
                return OperationResult<IReadOnlyList<SerialPortInfo>>.Failure(
                    $"串口扫描失败：{ex.Message}");
            }
        }
    }
}
