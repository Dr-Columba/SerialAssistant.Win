namespace SerialAssistant.Infrastructure.Modbus.Transport;

public class SystemIoPortsModbusRtuSerialAdapter : IModbusRtuSerialAdapter
{
    private System.IO.Ports.SerialPort? _serialPort;
    private readonly string _portName;
    private readonly int _baudRate;
    private readonly int _dataBits;
    private readonly System.IO.Ports.Parity _parity;
    private readonly System.IO.Ports.StopBits _stopBits;
    private readonly int _readTimeoutMilliseconds;
    private readonly int _writeTimeoutMilliseconds;

    public bool IsOpen => _serialPort?.IsOpen ?? false;

    public string PortName => _portName;
    public int BaudRate => _baudRate;
    public int DataBits => _dataBits;
    public string Parity => _parity.ToString();
    public string StopBits => _stopBits.ToString();
    public int ReadTimeoutMilliseconds => _readTimeoutMilliseconds;
    public int WriteTimeoutMilliseconds => _writeTimeoutMilliseconds;

    public SystemIoPortsModbusRtuSerialAdapter(
        string portName,
        int baudRate,
        int dataBits,
        string parity,
        string stopBits,
        int readTimeoutMilliseconds = 1000,
        int writeTimeoutMilliseconds = 1000)
    {
        _portName = portName.Trim();
        _baudRate = baudRate;
        _dataBits = dataBits;
        _parity = ParseParity(parity);
        _stopBits = ParseStopBits(stopBits);
        _readTimeoutMilliseconds = readTimeoutMilliseconds;
        _writeTimeoutMilliseconds = writeTimeoutMilliseconds;
    }

    public Task<bool> OpenAsync(CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return Task.FromResult(false);
        }

        if (string.IsNullOrWhiteSpace(_portName))
        {
            return Task.FromResult(false);
        }

        if (_baudRate <= 0)
        {
            return Task.FromResult(false);
        }

        if (_dataBits < 5 || _dataBits > 8)
        {
            return Task.FromResult(false);
        }

        if (_serialPort?.IsOpen == true)
        {
            return Task.FromResult(true);
        }

        try
        {
            _serialPort = new System.IO.Ports.SerialPort(_portName, _baudRate, _parity, _dataBits, _stopBits)
            {
                ReadTimeout = _readTimeoutMilliseconds,
                WriteTimeout = _writeTimeoutMilliseconds
            };
            _serialPort.Open();
            return Task.FromResult(true);
        }
        catch (UnauthorizedAccessException)
        {
            return Task.FromResult(false);
        }
        catch (IOException)
        {
            return Task.FromResult(false);
        }
        catch (ArgumentException)
        {
            return Task.FromResult(false);
        }
        catch (InvalidOperationException)
        {
            return Task.FromResult(false);
        }
    }

    public Task CloseAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            if (_serialPort?.IsOpen == true)
            {
                _serialPort.Close();
            }
        }
        catch
        {
            // Ignore close exceptions
        }
        finally
        {
            _serialPort?.Dispose();
            _serialPort = null;
        }

        return Task.CompletedTask;
    }

    public Task<bool> WriteAsync(byte[] requestBytes, CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return Task.FromResult(false);
        }

        if (_serialPort?.IsOpen != true)
        {
            return Task.FromResult(false);
        }

        if (requestBytes == null || requestBytes.Length == 0)
        {
            return Task.FromResult(false);
        }

        try
        {
            var bytesToWrite = new byte[requestBytes.Length];
            Array.Copy(requestBytes, bytesToWrite, requestBytes.Length);
            _serialPort.Write(bytesToWrite, 0, bytesToWrite.Length);
            return Task.FromResult(true);
        }
        catch
        {
            return Task.FromResult(false);
        }
    }

    public Task<byte[]> ReadAsync(int maxBytes, TimeSpan timeout, CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return Task.FromCanceled<byte[]>(cancellationToken);
        }

        if (_serialPort?.IsOpen != true)
        {
            return Task.FromResult(Array.Empty<byte>());
        }

        if (maxBytes <= 0)
        {
            return Task.FromResult(Array.Empty<byte>());
        }

        if (timeout <= TimeSpan.Zero)
        {
            return Task.FromResult(Array.Empty<byte>());
        }

        try
        {
            var deadline = DateTime.UtcNow.Add(timeout);
            var buffer = new List<byte>();
            var interByteIdle = TimeSpan.FromMilliseconds(20);

            while (DateTime.UtcNow < deadline)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var bytesToRead = _serialPort.BytesToRead;
                if (bytesToRead > 0)
                {
                    var readBuffer = new byte[Math.Min(bytesToRead, maxBytes - buffer.Count)];
                    var bytesRead = _serialPort.Read(readBuffer, 0, readBuffer.Length);
                    for (var i = 0; i < bytesRead; i++)
                    {
                        buffer.Add(readBuffer[i]);
                    }

                    if (buffer.Count >= maxBytes)
                    {
                        break;
                    }
                }

                cancellationToken.ThrowIfCancellationRequested();
                Thread.Sleep(interByteIdle);
            }

            return Task.FromResult(buffer.ToArray());
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch
        {
            return Task.FromResult(Array.Empty<byte>());
        }
    }

    private static System.IO.Ports.Parity ParseParity(string parity)
    {
        return parity?.ToUpperInvariant() switch
        {
            "NONE" => System.IO.Ports.Parity.None,
            "ODD" => System.IO.Ports.Parity.Odd,
            "EVEN" => System.IO.Ports.Parity.Even,
            "MARK" => System.IO.Ports.Parity.Mark,
            "SPACE" => System.IO.Ports.Parity.Space,
            _ => System.IO.Ports.Parity.None
        };
    }

    private static System.IO.Ports.StopBits ParseStopBits(string stopBits)
    {
        return stopBits?.ToUpperInvariant() switch
        {
            "ONE" => System.IO.Ports.StopBits.One,
            "ONEPOINTFIVE" => System.IO.Ports.StopBits.OnePointFive,
            "TWO" => System.IO.Ports.StopBits.Two,
            _ => System.IO.Ports.StopBits.One
        };
    }
}
