using System.IO.Ports;
using SerialAssistant.Core.Enums;
using SerialAssistant.Core.Models;
using SerialAssistant.Core.Services;
using SerialAssistant.Core.Utilities;

namespace SerialAssistant.Infrastructure.Serial
{
    /*
     * Implementation of ISerialPortService for serial port communication
     */
    public class SerialPortService : ISerialPortService, IDisposable
    {
        private SerialPort? _serialPort;
        private SerialConnectionState _connectionState;
        private bool _disposed;

        public SerialPortService()
        {
            _connectionState = SerialConnectionState.Disconnected;
        }

        public SerialConnectionState ConnectionState
        {
            get => _connectionState;
            private set
            {
                if (_connectionState != value)
                {
                    _connectionState = value;
                    ConnectionStateChanged?.Invoke(this, value);
                }
            }
        }

        public OperationResult Open(SerialPortSettings settings)
        {
            if (_disposed)
            {
                return OperationResult.Failure("串口服务已释放。");
            }

            try
            {
                if (settings == null)
                {
                    return OperationResult.Failure("串口配置不能为空。");
                }

                var validationResult = SerialSettingsValidator.Validate(settings);
                if (!validationResult.IsSuccess)
                {
                    return validationResult;
                }

                if (ConnectionState == SerialConnectionState.Connected)
                {
                    return OperationResult.Failure("串口已打开，请先关闭。");
                }

                ConnectionState = SerialConnectionState.Connecting;

                if (_serialPort != null)
                {
                    _serialPort.Close();
                    _serialPort.Dispose();
                    _serialPort = null;
                }

                _serialPort = new SerialPort(
                    settings.PortName,
                    settings.BaudRate,
                    ParseParity(settings.Parity),
                    settings.DataBits,
                    ParseStopBits(settings.StopBits))
                {
                    ReadTimeout = settings.ReadTimeout,
                    WriteTimeout = settings.WriteTimeout
                };

                _serialPort.Open();
                ConnectionState = SerialConnectionState.Connected;

                return OperationResult.Success();
            }
            catch (UnauthorizedAccessException ex)
            {
                ConnectionState = SerialConnectionState.Faulted;
                return OperationResult.Failure($"无法打开串口：{ex.Message}");
            }
            catch (IOException ex)
            {
                ConnectionState = SerialConnectionState.Faulted;
                return OperationResult.Failure($"串口打开失败：{ex.Message}");
            }
            catch (Exception ex)
            {
                ConnectionState = SerialConnectionState.Faulted;
                return OperationResult.Failure($"打开串口时发生错误：{ex.Message}");
            }
        }

        public OperationResult Close()
        {
            if (_disposed)
            {
                return OperationResult.Failure("串口服务已释放。");
            }

            try
            {
                if (ConnectionState != SerialConnectionState.Connected && ConnectionState != SerialConnectionState.Faulted)
                {
                    return OperationResult.Failure("串口未打开，无需关闭。");
                }

                ConnectionState = SerialConnectionState.Disconnecting;

                if (_serialPort != null)
                {
                    _serialPort.Close();
                    _serialPort.Dispose();
                    _serialPort = null;
                }

                ConnectionState = SerialConnectionState.Disconnected;
                return OperationResult.Success();
            }
            catch (Exception ex)
            {
                ConnectionState = SerialConnectionState.Faulted;
                return OperationResult.Failure($"关闭串口时发生错误：{ex.Message}");
            }
        }

        public OperationResult Send(byte[] data)
        {
            if (_disposed)
            {
                return OperationResult.Failure("串口服务已释放。");
            }

            return OperationResult.Failure("当前阶段尚未实现数据发送功能。");
        }

        public event EventHandler<SerialReceiveData>? DataReceived;

        public event EventHandler<Exception>? ErrorOccurred;

        public event EventHandler<SerialConnectionState>? ConnectionStateChanged;

        private static Parity ParseParity(string parity)
        {
            return parity?.ToUpperInvariant() switch
            {
                "NONE" => Parity.None,
                "ODD" => Parity.Odd,
                "EVEN" => Parity.Even,
                "MARK" => Parity.Mark,
                "SPACE" => Parity.Space,
                _ => Parity.None
            };
        }

        private static StopBits ParseStopBits(string stopBits)
        {
            return stopBits?.ToUpperInvariant() switch
            {
                "NONE" => StopBits.None,
                "ONE" => StopBits.One,
                "TWO" => StopBits.Two,
                "ONEPOINTFIVE" => StopBits.OnePointFive,
                _ => StopBits.One
            };
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            if (_serialPort != null)
            {
                if (_serialPort.IsOpen)
                {
                    _serialPort.Close();
                }
                _serialPort.Dispose();
                _serialPort = null;
            }

            _disposed = true;
            GC.SuppressFinalize(this);
        }
    }
}
