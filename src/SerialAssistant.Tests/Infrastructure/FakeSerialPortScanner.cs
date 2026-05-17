using SerialAssistant.Core.Models;
using SerialAssistant.Core.Services;

namespace SerialAssistant.Tests.Infrastructure
{
    /*
     * Fake implementation of ISerialPortScanner for testing
     */
    public class FakeSerialPortScanner : ISerialPortScanner
    {
        private readonly List<SerialPortInfo> _ports;
        private readonly bool _shouldFail;
        private readonly string? _errorMessage;

        public FakeSerialPortScanner(List<SerialPortInfo>? ports = null, bool shouldFail = false, string? errorMessage = null)
        {
            _ports = ports ?? new List<SerialPortInfo>();
            _shouldFail = shouldFail;
            _errorMessage = errorMessage ?? "Fake scanner error";
        }

        public OperationResult<IReadOnlyList<SerialPortInfo>> GetAvailablePorts()
        {
            if (_shouldFail)
            {
                return OperationResult<IReadOnlyList<SerialPortInfo>>.Failure(_errorMessage!);
            }

            return OperationResult<IReadOnlyList<SerialPortInfo>>.Success(_ports);
        }
    }
}
