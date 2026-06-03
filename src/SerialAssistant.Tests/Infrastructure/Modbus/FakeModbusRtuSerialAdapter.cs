using SerialAssistant.Infrastructure.Modbus.Transport;

namespace SerialAssistant.Tests.Infrastructure.Modbus;

public class FakeModbusRtuSerialAdapter : IModbusRtuSerialAdapter
{
    private readonly Queue<byte[]> _responseQueue = new Queue<byte[]>();
    private readonly Queue<Exception> _readFailureQueue = new Queue<Exception>();
    
    public bool IsOpen { get; private set; }
    
    public bool WriteShouldFail { get; set; }
    
    public List<byte[]> WrittenRequests { get; } = new List<byte[]>();

    public Task<bool> OpenAsync(CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return Task.FromResult(false);
        }
        
        IsOpen = true;
        return Task.FromResult(true);
    }

    public Task CloseAsync(CancellationToken cancellationToken = default)
    {
        IsOpen = false;
        return Task.CompletedTask;
    }

    public Task<bool> WriteAsync(byte[] requestBytes, CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return Task.FromResult(false);
        }
        
        if (WriteShouldFail)
        {
            return Task.FromResult(false);
        }
        
        if (requestBytes != null)
        {
            var copy = new byte[requestBytes.Length];
            Array.Copy(requestBytes, copy, requestBytes.Length);
            WrittenRequests.Add(copy);
        }
        
        return Task.FromResult(true);
    }

    public Task<byte[]> ReadAsync(int maxBytes, TimeSpan timeout, CancellationToken cancellationToken = default)
    {
        if (_readFailureQueue.Count > 0)
        {
            var exception = _readFailureQueue.Dequeue();
            throw exception;
        }

        if (_responseQueue.Count == 0)
        {
            return Task.FromResult(Array.Empty<byte>());
        }

        var response = _responseQueue.Dequeue();
        
        if (response.Length > maxBytes)
        {
            var truncated = new byte[maxBytes];
            Array.Copy(response, truncated, maxBytes);
            return Task.FromResult(truncated);
        }
        
        return Task.FromResult(response);
    }

    public void QueueResponse(byte[] responseBytes)
    {
        if (responseBytes != null)
        {
            var copy = new byte[responseBytes.Length];
            Array.Copy(responseBytes, copy, responseBytes.Length);
            _responseQueue.Enqueue(copy);
        }
    }

    public void QueueReadFailure(Exception exception)
    {
        _readFailureQueue.Enqueue(exception);
    }

    public void QueueTimeout()
    {
        _readFailureQueue.Enqueue(new TimeoutException("Simulated timeout"));
    }

    public void Clear()
    {
        _responseQueue.Clear();
        _readFailureQueue.Clear();
        WrittenRequests.Clear();
        WriteShouldFail = false;
        IsOpen = false;
    }
}