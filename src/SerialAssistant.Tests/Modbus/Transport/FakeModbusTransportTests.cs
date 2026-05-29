using Xunit;
using SerialAssistant.Core.Modbus.Transport;

namespace SerialAssistant.Tests.Modbus.Transport
{
    public class FakeModbusTransportTests
    {
        [Fact]
        public void Default_IsNotConnected()
        {
            var transport = new FakeModbusTransport();

            Assert.False(transport.IsConnected);
        }

        [Fact]
        public async Task ConnectAsync_SetsIsConnectedTrue()
        {
            var transport = new FakeModbusTransport();

            var result = await transport.ConnectAsync();

            Assert.True(result);
            Assert.True(transport.IsConnected);
        }

        [Fact]
        public async Task DisconnectAsync_SetsIsConnectedFalse()
        {
            var transport = new FakeModbusTransport();
            await transport.ConnectAsync();

            await transport.DisconnectAsync();

            Assert.False(transport.IsConnected);
        }

        [Fact]
        public async Task SendRequestAsync_WhenNotConnected_ReturnsNotConnected()
        {
            var transport = new FakeModbusTransport();
            var context = new ModbusRequestContext { UnitId = 1, FunctionCode = 0x03, Quantity = 1 };
            var requestBytes = new byte[] { 0x01, 0x03, 0x00, 0x00, 0x00, 0x0A };

            var result = await transport.SendRequestAsync(context, requestBytes);

            Assert.False(result.Success);
            Assert.Equal(ModbusTransportErrorCode.NotConnected, result.ErrorCode);
        }

        [Fact]
        public async Task SendRequestAsync_WhenConnectedAndResponseQueued_ReturnsSuccess()
        {
            var transport = new FakeModbusTransport();
            await transport.ConnectAsync();
            var context = new ModbusRequestContext { UnitId = 1, FunctionCode = 0x03, Quantity = 1 };
            var requestBytes = new byte[] { 0x01, 0x03, 0x00, 0x00, 0x00, 0x0A };
            var responseBytes = new byte[] { 0x01, 0x03, 0x14, 0x00, 0x01, 0x00, 0x02 };
            transport.QueueResponse(responseBytes);

            var result = await transport.SendRequestAsync(context, requestBytes);

            Assert.True(result.Success);
            Assert.NotNull(result.ResponseBytes);
            Assert.Equal(responseBytes, result.ResponseBytes);
        }

        [Fact]
        public async Task SendRequestAsync_RecordsRequestBytes()
        {
            var transport = new FakeModbusTransport();
            await transport.ConnectAsync();
            var context = new ModbusRequestContext { UnitId = 1, FunctionCode = 0x03, Quantity = 1 };
            var requestBytes = new byte[] { 0x01, 0x03, 0x00, 0x00, 0x00, 0x0A };
            transport.QueueResponse(new byte[] { 0x01 });

            await transport.SendRequestAsync(context, requestBytes);

            Assert.Single(transport.SentRequests);
            Assert.Equal(requestBytes, transport.SentRequests[0]);
        }

        [Fact]
        public async Task SendRequestAsync_RecordsContext()
        {
            var transport = new FakeModbusTransport();
            await transport.ConnectAsync();
            var context = new ModbusRequestContext
            {
                UnitId = 5,
                FunctionCode = 0x03,
                Quantity = 10,
                TransactionId = 42
            };
            var requestBytes = new byte[] { 0x05, 0x03 };
            transport.QueueResponse(new byte[] { 0x01 });

            await transport.SendRequestAsync(context, requestBytes);

            Assert.Single(transport.SentContexts);
            Assert.Equal(5, transport.SentContexts[0].UnitId);
            Assert.Equal(0x03, transport.SentContexts[0].FunctionCode);
            Assert.Equal(10, transport.SentContexts[0].Quantity);
            Assert.Equal(42u, transport.SentContexts[0].TransactionId);
        }

        [Fact]
        public async Task SendRequestAsync_CopiesRequestBytes()
        {
            var transport = new FakeModbusTransport();
            await transport.ConnectAsync();
            var context = new ModbusRequestContext { UnitId = 1, FunctionCode = 0x03, Quantity = 1 };
            var originalBytes = new byte[] { 0x01, 0x03, 0x00, 0x00, 0x00, 0x0A };
            transport.QueueResponse(new byte[] { 0x01 });

            await transport.SendRequestAsync(context, originalBytes);

            originalBytes[0] = 0xFF;
            Assert.Equal(0x01, transport.SentRequests[0][0]);
        }

        [Fact]
        public async Task SendRequestAsync_CopiesResponseBytes()
        {
            var transport = new FakeModbusTransport();
            await transport.ConnectAsync();
            var context = new ModbusRequestContext { UnitId = 1, FunctionCode = 0x03, Quantity = 1 };
            var originalResponse = new byte[] { 0x01, 0x03, 0x02, 0x00, 0xAB };
            transport.QueueResponse(originalResponse);

            var result = await transport.SendRequestAsync(context, new byte[] { 0x01 });

            originalResponse[0] = 0xFF;
            Assert.Equal(0x01, result.ResponseBytes![0]);
        }

        [Fact]
        public async Task SendRequestAsync_WhenNoResponseQueued_ReturnsTimeoutOrReceiveFailed()
        {
            var transport = new FakeModbusTransport();
            await transport.ConnectAsync();
            var context = new ModbusRequestContext { UnitId = 1, FunctionCode = 0x03, Quantity = 1 };
            var requestBytes = new byte[] { 0x01, 0x03 };

            var result = await transport.SendRequestAsync(context, requestBytes);

            Assert.False(result.Success);
            Assert.True(
                result.ErrorCode == ModbusTransportErrorCode.Timeout ||
                result.ErrorCode == ModbusTransportErrorCode.ReceiveFailed);
        }

        [Fact]
        public async Task QueueFailure_ReturnsFailure()
        {
            var transport = new FakeModbusTransport();
            await transport.ConnectAsync();
            var context = new ModbusRequestContext { UnitId = 1, FunctionCode = 0x03, Quantity = 1 };
            var requestBytes = new byte[] { 0x01, 0x03 };
            transport.QueueFailure(ModbusTransportErrorCode.SendFailed, "Simulated send failure");

            var result = await transport.SendRequestAsync(context, requestBytes);

            Assert.False(result.Success);
            Assert.Equal(ModbusTransportErrorCode.SendFailed, result.ErrorCode);
            Assert.Equal("Simulated send failure", result.ErrorMessage);
        }

        [Fact]
        public async Task MultipleQueuedResponses_ReturnInOrder()
        {
            var transport = new FakeModbusTransport();
            await transport.ConnectAsync();
            var context = new ModbusRequestContext { UnitId = 1, FunctionCode = 0x03, Quantity = 1 };
            var requestBytes = new byte[] { 0x01, 0x03 };
            transport.QueueResponse(new byte[] { 0xAA });
            transport.QueueResponse(new byte[] { 0xBB });
            transport.QueueResponse(new byte[] { 0xCC });

            var result1 = await transport.SendRequestAsync(context, requestBytes);
            var result2 = await transport.SendRequestAsync(context, requestBytes);
            var result3 = await transport.SendRequestAsync(context, requestBytes);

            Assert.Equal(0xAA, result1.ResponseBytes![0]);
            Assert.Equal(0xBB, result2.ResponseBytes![0]);
            Assert.Equal(0xCC, result3.ResponseBytes![0]);
        }

        [Fact]
        public async Task CancellationTokenCanceled_IsHandledConsistently()
        {
            var transport = new FakeModbusTransport();
            await transport.ConnectAsync();
            var context = new ModbusRequestContext { UnitId = 1, FunctionCode = 0x03, Quantity = 1 };
            var requestBytes = new byte[] { 0x01, 0x03 };
            using var cts = new CancellationTokenSource();
            cts.Cancel();

            var result = await transport.SendRequestAsync(context, requestBytes, cts.Token);

            Assert.False(result.Success);
            Assert.Equal(ModbusTransportErrorCode.Unknown, result.ErrorCode);
            Assert.Equal("Operation cancelled", result.ErrorMessage);
        }

        [Fact]
        public async Task SendRequestAsync_BeforeConnect_StillReturnsNotConnected()
        {
            var transport = new FakeModbusTransport();
            var context = new ModbusRequestContext { UnitId = 1, FunctionCode = 0x03, Quantity = 1 };
            var requestBytes = new byte[] { 0x01, 0x03 };
            transport.QueueResponse(new byte[] { 0x01 });

            var result = await transport.SendRequestAsync(context, requestBytes);

            Assert.False(result.Success);
            Assert.Equal(ModbusTransportErrorCode.NotConnected, result.ErrorCode);
            Assert.Empty(transport.SentRequests);
        }
    }
}
