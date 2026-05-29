using Xunit;
using SerialAssistant.App.ViewModels;
using SerialAssistant.Core.Modbus.Transport;
using SerialAssistant.Core.Modbus.Rtu;
using SerialAssistant.Tests.Modbus.Transport;
using System.ComponentModel;

namespace SerialAssistant.Tests.ViewModels
{
    public class ModbusViewModelTransportTests
    {
        [Fact]
        public void DefaultConstructor_HasNoTransportAvailable()
        {
            var vm = new ModbusViewModel();

            Assert.False(vm.IsTransportAvailable);
        }

        [Fact]
        public void ConstructorWithTransport_HasTransportAvailable()
        {
            var fakeTransport = new FakeModbusTransport();
            var vm = new ModbusViewModel(fakeTransport);

            Assert.True(vm.IsTransportAvailable);
        }

        [Fact]
        public void ConstructorWithTransport_DefaultIsDisconnected()
        {
            var fakeTransport = new FakeModbusTransport();
            var vm = new ModbusViewModel(fakeTransport);

            Assert.False(vm.IsConnected);
        }

        [Fact]
        public void ConstructorWithTransport_CommandsAreNotNull()
        {
            var fakeTransport = new FakeModbusTransport();
            var vm = new ModbusViewModel(fakeTransport);

            Assert.NotNull(vm.ConnectTransportCommand);
            Assert.NotNull(vm.DisconnectTransportCommand);
            Assert.NotNull(vm.SendRequestCommand);
        }

        [Fact]
        public async Task ConnectTransportCommand_WhenNoTransport_SetsStatusMessage()
        {
            var vm = new ModbusViewModel();

            await vm.ConnectTransportAsync();

            Assert.Contains("No transport", vm.StatusMessage);
        }

        [Fact]
        public async Task ConnectTransportCommand_WhenTransportAvailable_ConnectsTransport()
        {
            var fakeTransport = new FakeModbusTransport();
            var vm = new ModbusViewModel(fakeTransport);

            await vm.ConnectTransportAsync();

            Assert.True(vm.IsConnected);
        }

        [Fact]
        public async Task ConnectTransportCommand_WhenConnectSucceeds_SetsIsConnectedTrue()
        {
            var fakeTransport = new FakeModbusTransport();
            var vm = new ModbusViewModel(fakeTransport);

            await vm.ConnectTransportAsync();

            Assert.True(vm.IsConnected);
            Assert.Equal("Connected successfully", vm.StatusMessage);
        }

        [Fact]
        public async Task DisconnectTransportCommand_WhenConnected_DisconnectsTransport()
        {
            var fakeTransport = new FakeModbusTransport();
            var vm = new ModbusViewModel(fakeTransport);
            await vm.ConnectTransportAsync();

            await vm.DisconnectTransportAsync();

            Assert.False(vm.IsConnected);
            Assert.Equal("Disconnected", vm.StatusMessage);
        }

        [Fact]
        public async Task DisconnectTransportCommand_WhenDisconnected_DoesNotThrow()
        {
            var fakeTransport = new FakeModbusTransport();
            var vm = new ModbusViewModel(fakeTransport);

            var exception = await Record.ExceptionAsync(() => vm.DisconnectTransportAsync());

            Assert.Null(exception);
        }

        [Fact]
        public async Task SendRequestCommand_WhenNoTransport_SetsError()
        {
            var vm = new ModbusViewModel();
            vm.RequestHex = "01 03 00 00 00 0A";

            await vm.SendRequestAsync();

            Assert.Contains("No transport", vm.StatusMessage);
            Assert.Contains("not available", vm.LastTransportError);
        }

        [Fact]
        public async Task SendRequestCommand_WhenNotConnected_SetsError()
        {
            var fakeTransport = new FakeModbusTransport();
            var vm = new ModbusViewModel(fakeTransport);
            vm.RequestHex = "01 03 00 00 00 0A";

            await vm.SendRequestAsync();

            Assert.Contains("Not connected", vm.StatusMessage);
            Assert.Contains("Not connected", vm.LastTransportError);
        }

        [Fact]
        public async Task SendRequestCommand_WhenRequestHexEmpty_SetsError()
        {
            var fakeTransport = new FakeModbusTransport();
            var vm = new ModbusViewModel(fakeTransport);
            await vm.ConnectTransportAsync();

            await vm.SendRequestAsync();

            Assert.Contains("empty", vm.StatusMessage);
        }

        [Fact]
        public async Task SendRequestCommand_WhenContextInvalid_DoesNotSend()
        {
            var fakeTransport = new FakeModbusTransport();
            var vm = new ModbusViewModel(fakeTransport);
            await vm.ConnectTransportAsync();
            vm.RequestHex = "01 03 00 00 00 0A";
            vm.UnitId = 0;

            await vm.SendRequestAsync();

            Assert.Contains("Invalid request context", vm.StatusMessage);
            Assert.Empty(fakeTransport.SentRequests);
        }

        [Fact]
        public async Task SendRequestCommand_WhenConnectedAndResponseQueued_SendsRequest()
        {
            var fakeTransport = new FakeModbusTransport();
            var vm = new ModbusViewModel(fakeTransport);
            await vm.ConnectTransportAsync();
            vm.RequestHex = "01 03 00 00 00 0A";
            vm.UnitId = 1;
            vm.StartAddress = 0;
            vm.Quantity = 10;
            fakeTransport.QueueResponse(new byte[] { 0x01, 0x03, 0x14, 0x00, 0x01, 0x00, 0x02 });

            await vm.SendRequestAsync();

            Assert.Single(fakeTransport.SentRequests);
        }

        [Fact]
        public async Task SendRequestCommand_WhenSuccess_UpdatesResponseHex()
        {
            var fakeTransport = new FakeModbusTransport();
            var vm = new ModbusViewModel(fakeTransport);
            await vm.ConnectTransportAsync();
            vm.RequestHex = "01 03 00 00 00 0A";
            vm.UnitId = 1;
            vm.StartAddress = 0;
            vm.Quantity = 10;
            var responseBytes = new byte[] { 0x01, 0x03, 0x02, 0x00, 0xAB };
            fakeTransport.QueueResponse(responseBytes);

            await vm.SendRequestAsync();

            Assert.NotEmpty(vm.ResponseHex);
            Assert.Contains("AB", vm.ResponseHex);
        }

        [Fact]
        public async Task SendRequestCommand_WhenSuccess_ParsesResponse()
        {
            var fakeTransport = new FakeModbusTransport();
            var vm = new ModbusViewModel(fakeTransport);
            await vm.ConnectTransportAsync();
            vm.SelectedTransportMode = ModbusTransportMode.Rtu;
            vm.RequestHex = "010300000001C5CD";
            vm.UnitId = 1;
            vm.StartAddress = 0;
            vm.Quantity = 1;
            var responseBytes = new byte[] { 0x01, 0x03, 0x02, 0x00, 0xAB, 0x9A, 0x39 };
            fakeTransport.QueueResponse(responseBytes);

            await vm.SendRequestAsync();

            Assert.NotEmpty(vm.ResponseHex);
        }

        [Fact]
        public async Task SendRequestCommand_RecordsContextInFakeTransport()
        {
            var fakeTransport = new FakeModbusTransport();
            var vm = new ModbusViewModel(fakeTransport);
            await vm.ConnectTransportAsync();
            vm.RequestHex = "01 03 00 00 00 0A";
            vm.UnitId = 5;
            vm.TransactionId = 42;
            vm.StartAddress = 100;
            vm.Quantity = 10;
            fakeTransport.QueueResponse(new byte[] { 0x01 });

            await vm.SendRequestAsync();

            Assert.Single(fakeTransport.SentContexts);
            Assert.Equal(5, fakeTransport.SentContexts[0].UnitId);
            Assert.Equal(42, fakeTransport.SentContexts[0].TransactionId);
        }

        [Fact]
        public async Task SendRequestCommand_UsesRequestHexBytes()
        {
            var fakeTransport = new FakeModbusTransport();
            var vm = new ModbusViewModel(fakeTransport);
            await vm.ConnectTransportAsync();
            vm.RequestHex = "01 03 00 00 00 0A";
            vm.UnitId = 1;
            fakeTransport.QueueResponse(new byte[] { 0x01 });

            await vm.SendRequestAsync();

            Assert.Equal(6, fakeTransport.SentRequests[0].Length);
        }

        [Fact]
        public async Task SendRequestCommand_DoesNotClearRequestHex()
        {
            var fakeTransport = new FakeModbusTransport();
            var vm = new ModbusViewModel(fakeTransport);
            await vm.ConnectTransportAsync();
            vm.RequestHex = "01 03 00 00 00 0A";
            vm.UnitId = 1;
            fakeTransport.QueueResponse(new byte[] { 0x01 });

            await vm.SendRequestAsync();

            Assert.Equal("01 03 00 00 00 0A", vm.RequestHex);
        }

        [Fact]
        public async Task SendRequestCommand_WhenTransportReturnsTimeout_SetsLastTransportError()
        {
            var fakeTransport = new FakeModbusTransport();
            var vm = new ModbusViewModel(fakeTransport);
            await vm.ConnectTransportAsync();
            vm.RequestHex = "01 03 00 00 00 0A";
            vm.UnitId = 1;

            await vm.SendRequestAsync();

            Assert.Contains("Timeout", vm.LastTransportError);
        }

        [Fact]
        public async Task SendRequestCommand_WhenTransportReturnsSendFailed_SetsStatusMessage()
        {
            var fakeTransport = new FakeModbusTransport();
            var vm = new ModbusViewModel(fakeTransport);
            await vm.ConnectTransportAsync();
            vm.RequestHex = "01 03 00 00 00 0A";
            vm.UnitId = 1;
            fakeTransport.QueueFailure(ModbusTransportErrorCode.SendFailed, "Simulated send failure");

            await vm.SendRequestAsync();

            Assert.Contains("Simulated send failure", vm.StatusMessage);
        }

        [Fact]
        public async Task SendRequestCommand_WhenTransportReturnsReceiveFailed_SetsStatusMessage()
        {
            var fakeTransport = new FakeModbusTransport();
            var vm = new ModbusViewModel(fakeTransport);
            await vm.ConnectTransportAsync();
            vm.RequestHex = "01 03 00 00 00 0A";
            vm.UnitId = 1;
            fakeTransport.QueueFailure(ModbusTransportErrorCode.ReceiveFailed, "Receive failed");

            await vm.SendRequestAsync();

            Assert.Contains("Receive failed", vm.StatusMessage);
        }

        [Fact]
        public async Task SendRequestCommand_SetsIsBusyDuringOperation_AndRestoresFalse()
        {
            var fakeTransport = new FakeModbusTransport();
            var vm = new ModbusViewModel(fakeTransport);
            await vm.ConnectTransportAsync();
            vm.RequestHex = "01 03 00 00 00 0A";
            vm.UnitId = 1;

            Assert.False(vm.IsBusy);

            await vm.SendRequestAsync();

            Assert.False(vm.IsBusy);
            Assert.Contains("Error", vm.StatusMessage);
        }

        [Fact]
        public void ModbusViewModel_DoesNotReferenceSystemIoPorts()
        {
            var vm = new ModbusViewModel();

            var type = vm.GetType();
            var assembly = type.Assembly;

            Assert.DoesNotContain("System.IO.Ports", assembly.GetReferencedAssemblies().Select(a => a.Name));
        }

        [Fact]
        public void ModbusViewModel_DoesNotReferenceTcpClientOrSocket()
        {
            var vm = new ModbusViewModel();

            var type = vm.GetType();
            var assembly = type.Assembly;

            Assert.DoesNotContain("System.Net.Sockets", assembly.GetReferencedAssemblies().Select(a => a.Name));
        }

        [Fact]
        public async Task MultipleQueuedResponses_ReturnInOrder()
        {
            var fakeTransport = new FakeModbusTransport();
            var vm = new ModbusViewModel(fakeTransport);
            await vm.ConnectTransportAsync();
            vm.RequestHex = "01 03 00 00 00 01";
            vm.UnitId = 1;
            fakeTransport.QueueResponse(new byte[] { 0x01, 0x03, 0x02, 0x00, 0xAA });
            fakeTransport.QueueResponse(new byte[] { 0x01, 0x03, 0x02, 0x00, 0xBB });

            await vm.SendRequestAsync();
            vm.ResponseHex = "";
            await vm.SendRequestAsync();

            Assert.Contains("BB", vm.ResponseHex);
        }
    }
}
