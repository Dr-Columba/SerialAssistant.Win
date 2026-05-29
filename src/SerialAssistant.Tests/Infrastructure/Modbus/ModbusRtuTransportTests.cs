using SerialAssistant.Core.Modbus.Transport;
using SerialAssistant.Core.Modbus.Utilities;
using SerialAssistant.Core.Services;
using SerialAssistant.Infrastructure.Modbus.Transport;
using SerialAssistant.Tests.Services;
using Xunit;

namespace SerialAssistant.Tests.Infrastructure.Modbus;

public class ModbusRtuTransportTests
{
    private const string TestPortName = "COM1";

    private ModbusRtuTransport CreateTransport(
        string portName = TestPortName,
        FakeModbusRtuSerialAdapter? adapter = null,
        FakeSerialPortOwnershipCoordinator? coordinator = null,
        ModbusTransportOptions? options = null)
    {
        return new ModbusRtuTransport(
            portName,
            adapter ?? new FakeModbusRtuSerialAdapter(),
            coordinator ?? new FakeSerialPortOwnershipCoordinator(),
            options);
    }

    private byte[] CreateValidRtuResponse()
    {
        byte[] responseBody = { 0x01, 0x03, 0x04, 0x12, 0x34, 0x56, 0x78 };
        return ModbusCrc16.AppendCrc(responseBody);
    }

    [Fact]
    public void Constructor_WithValidDependencies_CreatesTransport()
    {
        var adapter = new FakeModbusRtuSerialAdapter();
        var coordinator = new FakeSerialPortOwnershipCoordinator();
        
        var transport = new ModbusRtuTransport(TestPortName, adapter, coordinator);
        
        Assert.NotNull(transport);
        Assert.Equal(TestPortName, transport.GetType().GetField("_portName", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.GetValue(transport));
    }

    [Fact]
    public void IsConnected_DefaultFalse()
    {
        var transport = CreateTransport();
        
        Assert.False(transport.IsConnected);
    }

    [Fact]
    public void Options_DefaultsWhenNull()
    {
        var transport = CreateTransport();
        
        Assert.NotNull(transport.Options);
        Assert.Equal(TimeSpan.FromSeconds(5), transport.Options.ConnectTimeout);
        Assert.Equal(TimeSpan.FromSeconds(5), transport.Options.ReceiveTimeout);
        Assert.True(transport.Options.ValidateResponse);
        Assert.Equal(260, transport.Options.MaxResponseBytes);
    }

    [Fact]
    public async Task ConnectAsync_ClaimsOwnership()
    {
        var adapter = new FakeModbusRtuSerialAdapter();
        var coordinator = new FakeSerialPortOwnershipCoordinator();
        var transport = CreateTransport(TestPortName, adapter, coordinator);
        
        await transport.ConnectAsync();
        
        Assert.Equal(SerialPortOwner.ModbusRtu, coordinator.GetCurrentOwner(TestPortName));
    }

    [Fact]
    public async Task ConnectAsync_OpensAdapter()
    {
        var adapter = new FakeModbusRtuSerialAdapter();
        var transport = CreateTransport(adapter: adapter);
        
        await transport.ConnectAsync();
        
        Assert.True(adapter.IsOpen);
    }

    [Fact]
    public async Task ConnectAsync_WhenPortNameEmpty_ReturnsFalse()
    {
        var transport = CreateTransport(portName: string.Empty);
        
        var result = await transport.ConnectAsync();
        
        Assert.False(result);
        Assert.False(transport.IsConnected);
    }

    [Fact]
    public async Task ConnectAsync_WhenOwnershipClaimFails_ReturnsFalse()
    {
        var coordinator = new FakeSerialPortOwnershipCoordinator();
        coordinator.TryClaimOwnership(TestPortName, SerialPortOwner.Terminal);
        
        var transport = CreateTransport(coordinator: coordinator);
        
        var result = await transport.ConnectAsync();
        
        Assert.False(result);
        Assert.False(transport.IsConnected);
    }

    [Fact]
    public async Task ConnectAsync_WhenAdapterOpenFails_ReleasesOwnership()
    {
        var adapter = new FakeModbusRtuSerialAdapter();
        var coordinator = new FakeSerialPortOwnershipCoordinator();
        var transport = new ModbusRtuTransport(TestPortName, adapter, coordinator);
        
        var mockAdapter = new MockAdapter(false);
        transport = new ModbusRtuTransport(TestPortName, mockAdapter, coordinator);
        
        await transport.ConnectAsync();
        
        Assert.Equal(SerialPortOwner.None, coordinator.GetCurrentOwner(TestPortName));
    }

    [Fact]
    public async Task ConnectAsync_WhenAdapterThrows_ReturnsFalseAndReleasesOwnership()
    {
        var mockAdapter = new MockAdapter(throwException: true);
        var coordinator = new FakeSerialPortOwnershipCoordinator();
        var transport = new ModbusRtuTransport(TestPortName, mockAdapter, coordinator);
        
        var result = await transport.ConnectAsync();
        
        Assert.False(result);
        Assert.Equal(SerialPortOwner.None, coordinator.GetCurrentOwner(TestPortName));
    }

    [Fact]
    public async Task DisconnectAsync_ClosesAdapter()
    {
        var adapter = new FakeModbusRtuSerialAdapter();
        var transport = CreateTransport(adapter: adapter);
        
        await transport.ConnectAsync();
        await transport.DisconnectAsync();
        
        Assert.False(adapter.IsOpen);
    }

    [Fact]
    public async Task DisconnectAsync_ReleasesOwnership()
    {
        var coordinator = new FakeSerialPortOwnershipCoordinator();
        var transport = CreateTransport(coordinator: coordinator);
        
        await transport.ConnectAsync();
        await transport.DisconnectAsync();
        
        Assert.Equal(SerialPortOwner.None, coordinator.GetCurrentOwner(TestPortName));
    }

    [Fact]
    public async Task DisconnectAsync_WhenAdapterThrows_StillReleasesOwnership()
    {
        var mockAdapter = new MockAdapter(throwOnClose: true);
        var coordinator = new FakeSerialPortOwnershipCoordinator();
        var transport = new ModbusRtuTransport(TestPortName, mockAdapter, coordinator);
        
        await transport.ConnectAsync();
        await transport.DisconnectAsync();
        
        Assert.Equal(SerialPortOwner.None, coordinator.GetCurrentOwner(TestPortName));
    }

    [Fact]
    public async Task SendRequestAsync_WhenNotConnected_ReturnsNotConnected()
    {
        var transport = CreateTransport();
        var context = new ModbusRequestContext { UnitId = 1, FunctionCode = 3 };
        
        var result = await transport.SendRequestAsync(context, new byte[] { 0x01, 0x03 });
        
        Assert.False(result.Success);
        Assert.Equal(ModbusTransportErrorCode.NotConnected, result.ErrorCode);
    }

    [Fact]
    public async Task SendRequestAsync_WhenRequestBytesEmpty_ReturnsFailure()
    {
        var adapter = new FakeModbusRtuSerialAdapter();
        var transport = CreateTransport(adapter: adapter);
        var context = new ModbusRequestContext { UnitId = 1, FunctionCode = 3 };
        
        await transport.ConnectAsync();
        
        var result = await transport.SendRequestAsync(context, Array.Empty<byte>());
        
        Assert.False(result.Success);
        Assert.Equal(ModbusTransportErrorCode.InvalidOptions, result.ErrorCode);
    }

    [Fact]
    public async Task SendRequestAsync_WhenContextInvalid_ReturnsInvalidOptions()
    {
        var adapter = new FakeModbusRtuSerialAdapter();
        var transport = CreateTransport(adapter: adapter);
        var context = new ModbusRequestContext { UnitId = 0, FunctionCode = 0 };
        
        await transport.ConnectAsync();
        
        var result = await transport.SendRequestAsync(context, new byte[] { 0x01, 0x03 });
        
        Assert.False(result.Success);
        Assert.Equal(ModbusTransportErrorCode.InvalidOptions, result.ErrorCode);
    }

    [Fact]
    public async Task SendRequestAsync_WhenConnected_WritesRequest()
    {
        var adapter = new FakeModbusRtuSerialAdapter();
        adapter.QueueResponse(CreateValidRtuResponse());
        var transport = CreateTransport(adapter: adapter);
        var context = new ModbusRequestContext { UnitId = 1, FunctionCode = 3 };
        var requestBytes = new byte[] { 0x01, 0x03, 0x00, 0x00, 0x00, 0x01 };
        
        await transport.ConnectAsync();
        await transport.SendRequestAsync(context, requestBytes);
        
        Assert.Single(adapter.WrittenRequests);
        Assert.Equal(requestBytes, adapter.WrittenRequests[0]);
    }

    [Fact]
    public async Task SendRequestAsync_WhenResponseQueued_ReturnsSuccess()
    {
        var adapter = new FakeModbusRtuSerialAdapter();
        var response = CreateValidRtuResponse();
        adapter.QueueResponse(response);
        var transport = CreateTransport(adapter: adapter);
        var context = new ModbusRequestContext { UnitId = 1, FunctionCode = 3 };
        
        await transport.ConnectAsync();
        var result = await transport.SendRequestAsync(context, new byte[] { 0x01, 0x03 });
        
        Assert.True(result.Success);
        Assert.Equal(response, result.ResponseBytes);
    }

    [Fact]
    public async Task SendRequestAsync_CopiesRequestBytes()
    {
        var adapter = new FakeModbusRtuSerialAdapter();
        adapter.QueueResponse(CreateValidRtuResponse());
        var transport = CreateTransport(adapter: adapter);
        var context = new ModbusRequestContext { UnitId = 1, FunctionCode = 3 };
        var requestBytes = new byte[] { 0x01, 0x03, 0x00, 0x00, 0x00, 0x01 };
        
        await transport.ConnectAsync();
        await transport.SendRequestAsync(context, requestBytes);
        
        requestBytes[0] = 0xFF;
        
        Assert.NotEqual(requestBytes, adapter.WrittenRequests[0]);
        Assert.Equal(0x01, adapter.WrittenRequests[0][0]);
    }

    [Fact]
    public async Task SendRequestAsync_ReturnsCopiedResponseBytes()
    {
        var adapter = new FakeModbusRtuSerialAdapter();
        var response = CreateValidRtuResponse();
        adapter.QueueResponse(response);
        var transport = CreateTransport(adapter: adapter);
        var context = new ModbusRequestContext { UnitId = 1, FunctionCode = 3 };
        
        await transport.ConnectAsync();
        var result = await transport.SendRequestAsync(context, new byte[] { 0x01, 0x03 });
        
        Assert.NotNull(result.ResponseBytes);
        Assert.Equal(response, result.ResponseBytes);
    }

    [Fact]
    public async Task SendRequestAsync_ValidCrc_WhenValidateResponseTrue_ReturnsSuccess()
    {
        var adapter = new FakeModbusRtuSerialAdapter();
        var response = CreateValidRtuResponse();
        adapter.QueueResponse(response);
        var options = new ModbusTransportOptions { ValidateResponse = true };
        var transport = CreateTransport(options: options, adapter: adapter);
        var context = new ModbusRequestContext { UnitId = 1, FunctionCode = 3 };
        
        await transport.ConnectAsync();
        var result = await transport.SendRequestAsync(context, new byte[] { 0x01, 0x03 });
        
        Assert.True(result.Success);
    }

    [Fact]
    public async Task SendRequestAsync_WhenWriteFails_ReturnsSendFailed()
    {
        var adapter = new FakeModbusRtuSerialAdapter { WriteShouldFail = true };
        var transport = CreateTransport(adapter: adapter);
        var context = new ModbusRequestContext { UnitId = 1, FunctionCode = 3 };
        
        await transport.ConnectAsync();
        var result = await transport.SendRequestAsync(context, new byte[] { 0x01, 0x03 });
        
        Assert.False(result.Success);
        Assert.Equal(ModbusTransportErrorCode.SendFailed, result.ErrorCode);
    }

    [Fact]
    public async Task SendRequestAsync_WhenNoResponse_ReturnsReceiveFailed()
    {
        var adapter = new FakeModbusRtuSerialAdapter();
        var transport = CreateTransport(adapter: adapter);
        var context = new ModbusRequestContext { UnitId = 1, FunctionCode = 3 };
        
        await transport.ConnectAsync();
        var result = await transport.SendRequestAsync(context, new byte[] { 0x01, 0x03 });
        
        Assert.False(result.Success);
        Assert.Equal(ModbusTransportErrorCode.ReceiveFailed, result.ErrorCode);
    }

    [Fact]
    public async Task SendRequestAsync_WhenResponseTooShort_ReturnsInvalidResponse()
    {
        var adapter = new FakeModbusRtuSerialAdapter();
        adapter.QueueResponse(new byte[] { 0x01, 0x03 });
        var transport = CreateTransport(adapter: adapter);
        var context = new ModbusRequestContext { UnitId = 1, FunctionCode = 3 };
        
        await transport.ConnectAsync();
        var result = await transport.SendRequestAsync(context, new byte[] { 0x01, 0x03 });
        
        Assert.False(result.Success);
        Assert.Equal(ModbusTransportErrorCode.InvalidResponse, result.ErrorCode);
    }

    [Fact]
    public async Task SendRequestAsync_WhenCrcInvalid_ReturnsCrcInvalid()
    {
        var adapter = new FakeModbusRtuSerialAdapter();
        var invalidResponse = new byte[] { 0x01, 0x03, 0x04, 0x12, 0x34, 0x56, 0x78, 0x00, 0x00 };
        adapter.QueueResponse(invalidResponse);
        var transport = CreateTransport(adapter: adapter);
        var context = new ModbusRequestContext { UnitId = 1, FunctionCode = 3 };
        
        await transport.ConnectAsync();
        var result = await transport.SendRequestAsync(context, new byte[] { 0x01, 0x03 });
        
        Assert.False(result.Success);
        Assert.Equal(ModbusTransportErrorCode.CrcInvalid, result.ErrorCode);
    }

    [Fact]
    public async Task SendRequestAsync_WhenAdapterReadThrows_ReturnsReceiveFailed()
    {
        var adapter = new FakeModbusRtuSerialAdapter();
        adapter.QueueReadFailure(new IOException("Simulated IO error"));
        var transport = CreateTransport(adapter: adapter);
        var context = new ModbusRequestContext { UnitId = 1, FunctionCode = 3 };
        
        await transport.ConnectAsync();
        var result = await transport.SendRequestAsync(context, new byte[] { 0x01, 0x03 });
        
        Assert.False(result.Success);
        Assert.Equal(ModbusTransportErrorCode.ReceiveFailed, result.ErrorCode);
    }

    [Fact]
    public async Task SendRequestAsync_WhenReadTimeout_ReturnsTimeout()
    {
        var adapter = new FakeModbusRtuSerialAdapter();
        adapter.QueueTimeout();
        var transport = CreateTransport(adapter: adapter);
        var context = new ModbusRequestContext { UnitId = 1, FunctionCode = 3 };
        
        await transport.ConnectAsync();
        var result = await transport.SendRequestAsync(context, new byte[] { 0x01, 0x03 });
        
        Assert.False(result.Success);
        Assert.Equal(ModbusTransportErrorCode.Timeout, result.ErrorCode);
    }

    [Fact]
    public async Task TerminalOwnedPort_CannotConnectModbusRtu()
    {
        var coordinator = new FakeSerialPortOwnershipCoordinator();
        coordinator.TryClaimOwnership(TestPortName, SerialPortOwner.Terminal);
        
        var transport = CreateTransport(coordinator: coordinator);
        
        var result = await transport.ConnectAsync();
        
        Assert.False(result);
        Assert.False(transport.IsConnected);
    }

    [Fact]
    public async Task SameModbusOwner_ReconnectIsIdempotent()
    {
        var coordinator = new FakeSerialPortOwnershipCoordinator();
        var transport = CreateTransport(coordinator: coordinator);
        
        await transport.ConnectAsync();
        var result = await transport.ConnectAsync();
        
        Assert.True(result);
        Assert.True(transport.IsConnected);
    }

    [Fact]
    public async Task ReleaseThenCanConnectAgain()
    {
        var coordinator = new FakeSerialPortOwnershipCoordinator();
        var transport = CreateTransport(coordinator: coordinator);
        
        await transport.ConnectAsync();
        await transport.DisconnectAsync();
        var result = await transport.ConnectAsync();
        
        Assert.True(result);
        Assert.True(transport.IsConnected);
    }

    private class MockAdapter : IModbusRtuSerialAdapter
    {
        private readonly bool _openResult;
        private readonly bool _throwException;
        private readonly bool _throwOnClose;
        
        public MockAdapter(bool openResult = true, bool throwException = false, bool throwOnClose = false)
        {
            _openResult = openResult;
            _throwException = throwException;
            _throwOnClose = throwOnClose;
        }
        
        public bool IsOpen { get; private set; }
        
        public Task<bool> OpenAsync(CancellationToken cancellationToken = default)
        {
            if (_throwException)
            {
                throw new InvalidOperationException("Simulated exception");
            }
            IsOpen = _openResult;
            return Task.FromResult(_openResult);
        }
        
        public Task CloseAsync(CancellationToken cancellationToken = default)
        {
            if (_throwOnClose)
            {
                throw new InvalidOperationException("Simulated close exception");
            }
            IsOpen = false;
            return Task.CompletedTask;
        }
        
        public Task<bool> WriteAsync(byte[] requestBytes, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(true);
        }
        
        public Task<byte[]> ReadAsync(int maxBytes, TimeSpan timeout, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Array.Empty<byte>());
        }
    }
}