using Xunit;
using SerialAssistant.App.ViewModels;
using System.ComponentModel;
using SerialAssistant.Core.Modbus.Utilities;
using SerialAssistant.Core.Modbus.Rtu;
using SerialAssistant.Core.Utilities;

namespace SerialAssistant.Tests.ViewModels
{
    public class ModbusViewModelTests
    {
        [Fact]
        public void DefaultSelectedTransportMode_IsRtu()
        {
            var vm = new ModbusViewModel();
            Assert.Equal(ModbusTransportMode.Rtu, vm.SelectedTransportMode);
        }

        [Fact]
        public void DefaultSelectedRequestKind_IsReadHoldingRegisters()
        {
            var vm = new ModbusViewModel();
            Assert.Equal(ModbusRequestKind.ReadHoldingRegisters, vm.SelectedRequestKind);
        }

        [Fact]
        public void DefaultUnitId_Is1()
        {
            var vm = new ModbusViewModel();
            Assert.Equal(1, vm.UnitId);
        }

        [Fact]
        public void DefaultTransactionId_Is1()
        {
            var vm = new ModbusViewModel();
            Assert.Equal(1, vm.TransactionId);
        }

        [Fact]
        public void DefaultQuantity_Is1()
        {
            var vm = new ModbusViewModel();
            Assert.Equal(1, vm.Quantity);
        }

        [Fact]
        public void DefaultRequestHex_IsEmpty()
        {
            var vm = new ModbusViewModel();
            Assert.Empty(vm.RequestHex);
        }

        [Fact]
        public void DefaultResponseHex_IsEmpty()
        {
            var vm = new ModbusViewModel();
            Assert.Empty(vm.ResponseHex);
        }

        [Fact]
        public void DefaultHasRequest_IsFalse()
        {
            var vm = new ModbusViewModel();
            Assert.False(vm.HasRequest);
        }

        [Fact]
        public void DefaultHasParsedResponse_IsFalse()
        {
            var vm = new ModbusViewModel();
            Assert.False(vm.HasParsedResponse);
        }

        [Fact]
        public void DefaultStatusMessage_IsNotEmpty()
        {
            var vm = new ModbusViewModel();
            Assert.False(string.IsNullOrEmpty(vm.StatusMessage));
        }

        [Fact]
        public void IsRtu_WhenRtuMode_IsTrue()
        {
            var vm = new ModbusViewModel();
            vm.SelectedTransportMode = ModbusTransportMode.Rtu;
            Assert.True(vm.IsRtu);
        }

        [Fact]
        public void IsTcp_WhenRtuMode_IsFalse()
        {
            var vm = new ModbusViewModel();
            vm.SelectedTransportMode = ModbusTransportMode.Rtu;
            Assert.False(vm.IsTcp);
        }

        [Fact]
        public void IsTcp_WhenTcpMode_IsTrue()
        {
            var vm = new ModbusViewModel();
            vm.SelectedTransportMode = ModbusTransportMode.Tcp;
            Assert.True(vm.IsTcp);
        }

        [Fact]
        public void IsRtu_WhenTcpMode_IsFalse()
        {
            var vm = new ModbusViewModel();
            vm.SelectedTransportMode = ModbusTransportMode.Tcp;
            Assert.False(vm.IsRtu);
        }

        [Fact]
        public void SelectedTransportMode_Change_TriggersPropertyChanged()
        {
            var vm = new ModbusViewModel();
            bool isRtuChanged = false;
            bool isTcpChanged = false;

            vm.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(vm.IsRtu)) isRtuChanged = true;
                if (e.PropertyName == nameof(vm.IsTcp)) isTcpChanged = true;
            };

            vm.SelectedTransportMode = ModbusTransportMode.Tcp;

            Assert.True(isRtuChanged);
            Assert.True(isTcpChanged);
        }

        [Fact]
        public void RtuBuildRequest_ReadHoldingRegisters_ContainsFunctionCode03()
        {
            var vm = new ModbusViewModel();
            vm.SelectedTransportMode = ModbusTransportMode.Rtu;
            vm.SelectedRequestKind = ModbusRequestKind.ReadHoldingRegisters;
            vm.UnitId = 1;
            vm.StartAddress = 0;
            vm.Quantity = 1;

            vm.BuildRequestCommand.Execute(null);

            Assert.Contains("03", vm.RequestHex);
            Assert.True(vm.HasRequest);
        }

        [Fact]
        public void RtuBuildRequest_ReadInputRegisters_ContainsFunctionCode04()
        {
            var vm = new ModbusViewModel();
            vm.SelectedTransportMode = ModbusTransportMode.Rtu;
            vm.SelectedRequestKind = ModbusRequestKind.ReadInputRegisters;
            vm.UnitId = 1;
            vm.StartAddress = 0;
            vm.Quantity = 1;

            vm.BuildRequestCommand.Execute(null);

            Assert.Contains("04", vm.RequestHex);
            Assert.True(vm.HasRequest);
        }

        [Fact]
        public void RtuBuildRequest_WriteSingleRegister_ContainsFunctionCode06()
        {
            var vm = new ModbusViewModel();
            vm.SelectedTransportMode = ModbusTransportMode.Rtu;
            vm.SelectedRequestKind = ModbusRequestKind.WriteSingleRegister;
            vm.UnitId = 1;
            vm.StartAddress = 0;
            vm.SingleWriteValue = 0x1234;

            vm.BuildRequestCommand.Execute(null);

            Assert.Contains("06", vm.RequestHex);
            Assert.True(vm.HasRequest);
        }

        [Fact]
        public void RtuBuildRequest_WriteMultipleRegisters_ContainsFunctionCode10()
        {
            var vm = new ModbusViewModel();
            vm.SelectedTransportMode = ModbusTransportMode.Rtu;
            vm.SelectedRequestKind = ModbusRequestKind.WriteMultipleRegisters;
            vm.UnitId = 1;
            vm.StartAddress = 0;
            vm.MultipleWriteValuesText = "0001 0002";

            vm.BuildRequestCommand.Execute(null);

            Assert.Contains("10", vm.RequestHex);
            Assert.True(vm.HasRequest);
        }

        [Fact]
        public void RtuBuildRequest_Success_UpdatesStatusMessage()
        {
            var vm = new ModbusViewModel();
            vm.SelectedTransportMode = ModbusTransportMode.Rtu;
            vm.SelectedRequestKind = ModbusRequestKind.ReadHoldingRegisters;

            vm.BuildRequestCommand.Execute(null);

            Assert.Contains("successfully", vm.StatusMessage);
        }

        [Fact]
        public void TcpBuildRequest_ReadHoldingRegisters_ContainsFunctionCode03()
        {
            var vm = new ModbusViewModel();
            vm.SelectedTransportMode = ModbusTransportMode.Tcp;
            vm.SelectedRequestKind = ModbusRequestKind.ReadHoldingRegisters;
            vm.UnitId = 1;
            vm.TransactionId = 1;
            vm.StartAddress = 0;
            vm.Quantity = 1;

            vm.BuildRequestCommand.Execute(null);

            Assert.Contains("03", vm.RequestHex);
            Assert.True(vm.HasRequest);
        }

        [Fact]
        public void TcpBuildRequest_ReadInputRegisters_ContainsFunctionCode04()
        {
            var vm = new ModbusViewModel();
            vm.SelectedTransportMode = ModbusTransportMode.Tcp;
            vm.SelectedRequestKind = ModbusRequestKind.ReadInputRegisters;

            vm.BuildRequestCommand.Execute(null);

            Assert.Contains("04", vm.RequestHex);
        }

        [Fact]
        public void TcpBuildRequest_WriteSingleRegister_ContainsFunctionCode06()
        {
            var vm = new ModbusViewModel();
            vm.SelectedTransportMode = ModbusTransportMode.Tcp;
            vm.SelectedRequestKind = ModbusRequestKind.WriteSingleRegister;

            vm.BuildRequestCommand.Execute(null);

            Assert.Contains("06", vm.RequestHex);
        }

        [Fact]
        public void TcpBuildRequest_WriteMultipleRegisters_ContainsFunctionCode10()
        {
            var vm = new ModbusViewModel();
            vm.SelectedTransportMode = ModbusTransportMode.Tcp;
            vm.SelectedRequestKind = ModbusRequestKind.WriteMultipleRegisters;
            vm.MultipleWriteValuesText = "0001,0002";

            vm.BuildRequestCommand.Execute(null);

            Assert.Contains("10", vm.RequestHex);
        }

        [Fact]
        public void TcpBuildRequest_DoesNotContainCrc()
        {
            var vm = new ModbusViewModel();
            vm.SelectedTransportMode = ModbusTransportMode.Tcp;
            vm.SelectedRequestKind = ModbusRequestKind.ReadHoldingRegisters;

            vm.BuildRequestCommand.Execute(null);

            string hex = vm.RequestHex.Replace(" ", "");
            Assert.Equal(12, hex.Length / 2);
        }

        [Fact]
        public void MultipleWriteValuesText_SpaceSeparated_ParseSuccessfully()
        {
            var vm = new ModbusViewModel();
            vm.SelectedTransportMode = ModbusTransportMode.Rtu;
            vm.SelectedRequestKind = ModbusRequestKind.WriteMultipleRegisters;
            vm.MultipleWriteValuesText = "0001 0002 00FF";

            vm.BuildRequestCommand.Execute(null);

            Assert.True(vm.HasRequest);
        }

        [Fact]
        public void MultipleWriteValuesText_CommaSeparated_ParseSuccessfully()
        {
            var vm = new ModbusViewModel();
            vm.SelectedTransportMode = ModbusTransportMode.Rtu;
            vm.SelectedRequestKind = ModbusRequestKind.WriteMultipleRegisters;
            vm.MultipleWriteValuesText = "0001,0002,00FF";

            vm.BuildRequestCommand.Execute(null);

            Assert.True(vm.HasRequest);
        }

        [Fact]
        public void MultipleWriteValuesText_MixedSeparators_ParseSuccessfully()
        {
            var vm = new ModbusViewModel();
            vm.SelectedTransportMode = ModbusTransportMode.Rtu;
            vm.SelectedRequestKind = ModbusRequestKind.WriteMultipleRegisters;
            vm.MultipleWriteValuesText = "0001 , 0002 00FF";

            vm.BuildRequestCommand.Execute(null);

            Assert.True(vm.HasRequest);
        }

        [Fact]
        public void MultipleWriteValuesText_InvalidHex_ShowsError()
        {
            var vm = new ModbusViewModel();
            vm.SelectedTransportMode = ModbusTransportMode.Rtu;
            vm.SelectedRequestKind = ModbusRequestKind.WriteMultipleRegisters;
            vm.MultipleWriteValuesText = "0001 GHIJ 00FF";

            vm.BuildRequestCommand.Execute(null);

            Assert.False(vm.HasRequest);
            Assert.Contains("Error", vm.StatusMessage);
        }

        [Fact]
        public void MultipleWriteValuesText_Empty_ShowsError()
        {
            var vm = new ModbusViewModel();
            vm.SelectedTransportMode = ModbusTransportMode.Rtu;
            vm.SelectedRequestKind = ModbusRequestKind.WriteMultipleRegisters;
            vm.MultipleWriteValuesText = "";

            vm.BuildRequestCommand.Execute(null);

            Assert.False(vm.HasRequest);
            Assert.Contains("Error", vm.StatusMessage);
        }

        [Fact]
        public void RtuParseResponse_ReadHoldingRegisters_Success()
        {
            var vm = new ModbusViewModel();
            vm.SelectedTransportMode = ModbusTransportMode.Rtu;
            byte[] frame = { 0x01, 0x03, 0x02, 0x00, 0x0A };
            ushort crc = ModbusCrc16.Compute(frame);
            byte[] fullFrame = frame.Concat(new[] { ModbusCrc16.LowByte(crc), ModbusCrc16.HighByte(crc) }).ToArray();
            vm.ResponseHex = HexConverter.ToHexString(fullFrame);

            vm.ParseResponseCommand.Execute(null);

            Assert.True(vm.HasParsedResponse);
            Assert.Contains("0x03", vm.ParsedSummary);
        }

        [Fact]
        public void RtuParseResponse_ReadInputRegisters_Success()
        {
            var vm = new ModbusViewModel();
            vm.SelectedTransportMode = ModbusTransportMode.Rtu;
            byte[] frame = { 0x01, 0x04, 0x02, 0x00, 0x0B };
            ushort crc = ModbusCrc16.Compute(frame);
            byte[] fullFrame = frame.Concat(new[] { ModbusCrc16.LowByte(crc), ModbusCrc16.HighByte(crc) }).ToArray();
            vm.ResponseHex = HexConverter.ToHexString(fullFrame);

            vm.ParseResponseCommand.Execute(null);

            Assert.True(vm.HasParsedResponse);
            Assert.Contains("0x04", vm.ParsedSummary);
        }

        [Fact]
        public void RtuParseResponse_WriteSingleRegister_Success()
        {
            var vm = new ModbusViewModel();
            vm.SelectedTransportMode = ModbusTransportMode.Rtu;
            byte[] frame = { 0x01, 0x06, 0x00, 0x00, 0x00, 0x0A };
            ushort crc = ModbusCrc16.Compute(frame);
            byte[] fullFrame = frame.Concat(new[] { ModbusCrc16.LowByte(crc), ModbusCrc16.HighByte(crc) }).ToArray();
            vm.ResponseHex = HexConverter.ToHexString(fullFrame);

            vm.ParseResponseCommand.Execute(null);

            Assert.True(vm.HasParsedResponse);
            Assert.Contains("0x06", vm.ParsedSummary);
        }

        [Fact]
        public void RtuParseResponse_WriteMultipleRegisters_Success()
        {
            var vm = new ModbusViewModel();
            vm.SelectedTransportMode = ModbusTransportMode.Rtu;
            byte[] frame = { 0x01, 0x10, 0x00, 0x00, 0x00, 0x02 };
            ushort crc = ModbusCrc16.Compute(frame);
            byte[] fullFrame = frame.Concat(new[] { ModbusCrc16.LowByte(crc), ModbusCrc16.HighByte(crc) }).ToArray();
            vm.ResponseHex = HexConverter.ToHexString(fullFrame);

            vm.ParseResponseCommand.Execute(null);

            Assert.True(vm.HasParsedResponse);
            Assert.Contains("0x10", vm.ParsedSummary);
        }

        [Fact]
        public void RtuParseResponse_ExceptionResponse_Success()
        {
            var vm = new ModbusViewModel();
            vm.SelectedTransportMode = ModbusTransportMode.Rtu;
            byte[] frame = { 0x01, 0x83, 0x02 };
            ushort crc = ModbusCrc16.Compute(frame);
            byte[] fullFrame = frame.Concat(new[] { ModbusCrc16.LowByte(crc), ModbusCrc16.HighByte(crc) }).ToArray();
            vm.ResponseHex = HexConverter.ToHexString(fullFrame);

            vm.ParseResponseCommand.Execute(null);

            Assert.True(vm.HasParsedResponse);
            Assert.Contains("Exception", vm.ParsedSummary);
        }

        [Fact]
        public void RtuParseResponse_InvalidCrc_Fails()
        {
            var vm = new ModbusViewModel();
            vm.SelectedTransportMode = ModbusTransportMode.Rtu;
            vm.ResponseHex = "01 03 02 00 0A FF FF";

            vm.ParseResponseCommand.Execute(null);

            Assert.False(vm.HasParsedResponse);
            Assert.Contains("Error", vm.StatusMessage);
        }

        [Fact]
        public void TcpParseResponse_ReadHoldingRegisters_Success()
        {
            var vm = new ModbusViewModel();
            vm.SelectedTransportMode = ModbusTransportMode.Tcp;
            vm.ResponseHex = "00 01 00 00 00 04 01 03 02 00 0A";

            vm.ParseResponseCommand.Execute(null);

            Assert.True(vm.HasParsedResponse);
            Assert.Contains("0x03", vm.ParsedSummary);
        }

        [Fact]
        public void TcpParseResponse_ReadInputRegisters_Success()
        {
            var vm = new ModbusViewModel();
            vm.SelectedTransportMode = ModbusTransportMode.Tcp;
            vm.ResponseHex = "00 01 00 00 00 04 01 04 02 00 0B";

            vm.ParseResponseCommand.Execute(null);

            Assert.True(vm.HasParsedResponse);
            Assert.Contains("0x04", vm.ParsedSummary);
        }

        [Fact]
        public void TcpParseResponse_WriteSingleRegister_Success()
        {
            var vm = new ModbusViewModel();
            vm.SelectedTransportMode = ModbusTransportMode.Tcp;
            vm.ResponseHex = "00 01 00 00 00 05 01 06 00 00 00 0A";

            vm.ParseResponseCommand.Execute(null);

            Assert.True(vm.HasParsedResponse);
            Assert.Contains("0x06", vm.ParsedSummary);
        }

        [Fact]
        public void TcpParseResponse_WriteMultipleRegisters_Success()
        {
            var vm = new ModbusViewModel();
            vm.SelectedTransportMode = ModbusTransportMode.Tcp;
            vm.ResponseHex = "00 01 00 00 00 05 01 10 00 00 00 02";

            vm.ParseResponseCommand.Execute(null);

            Assert.True(vm.HasParsedResponse);
            Assert.Contains("0x10", vm.ParsedSummary);
        }

        [Fact]
        public void TcpParseResponse_ExceptionResponse_Success()
        {
            var vm = new ModbusViewModel();
            vm.SelectedTransportMode = ModbusTransportMode.Tcp;
            vm.ResponseHex = "00 01 00 00 00 02 01 83 02";

            vm.ParseResponseCommand.Execute(null);

            Assert.True(vm.HasParsedResponse);
            Assert.Contains("Exception", vm.ParsedSummary);
        }

        [Fact]
        public void TcpParseResponse_InvalidLength_Fails()
        {
            var vm = new ModbusViewModel();
            vm.SelectedTransportMode = ModbusTransportMode.Tcp;
            vm.ResponseHex = "00 01 00 00 00 0A 01 03 02 00 0A";

            vm.ParseResponseCommand.Execute(null);

            Assert.False(vm.HasParsedResponse);
            Assert.Contains("Error", vm.StatusMessage);
        }

        [Fact]
        public void ParseResponse_EmptyResponseHex_ShowsError()
        {
            var vm = new ModbusViewModel();
            vm.ResponseHex = "";

            vm.ParseResponseCommand.Execute(null);

            Assert.False(vm.HasParsedResponse);
            Assert.Contains("empty", vm.StatusMessage);
        }

        [Fact]
        public void ParseResponse_InvalidHex_ShowsError()
        {
            var vm = new ModbusViewModel();
            vm.ResponseHex = "XX YY ZZ";

            vm.ParseResponseCommand.Execute(null);

            Assert.False(vm.HasParsedResponse);
            Assert.Contains("Error", vm.StatusMessage);
        }

        [Fact]
        public void BuildRequest_UnitIdZero_ShowsError()
        {
            var vm = new ModbusViewModel();
            vm.UnitId = 0;

            vm.BuildRequestCommand.Execute(null);

            Assert.False(vm.HasRequest);
            Assert.Contains("Error", vm.StatusMessage);
        }

        [Fact]
        public void BuildRequest_QuantityZero_ShowsError()
        {
            var vm = new ModbusViewModel();
            vm.Quantity = 0;

            vm.BuildRequestCommand.Execute(null);

            Assert.False(vm.HasRequest);
            Assert.Contains("Error", vm.StatusMessage);
        }

        [Fact]
        public void ClearCommand_ClearsRequestHex()
        {
            var vm = new ModbusViewModel();
            vm.RequestHex = "01 02 03";
            vm.ResponseHex = "04 05 06";
            vm.ParsedSummary = "Test";

            vm.ClearCommand.Execute(null);

            Assert.Empty(vm.RequestHex);
        }

        [Fact]
        public void ClearCommand_ClearsResponseHex()
        {
            var vm = new ModbusViewModel();
            vm.RequestHex = "01 02 03";
            vm.ResponseHex = "04 05 06";
            vm.ParsedSummary = "Test";

            vm.ClearCommand.Execute(null);

            Assert.Empty(vm.ResponseHex);
        }

        [Fact]
        public void ClearCommand_ClearsParsedSummary()
        {
            var vm = new ModbusViewModel();
            vm.RequestHex = "01 02 03";
            vm.ResponseHex = "04 05 06";
            vm.ParsedSummary = "Test";

            vm.ClearCommand.Execute(null);

            Assert.Empty(vm.ParsedSummary);
        }

        [Fact]
        public void ClearCommand_SetsHasRequestFalse()
        {
            var vm = new ModbusViewModel();
            vm.RequestHex = "01 02 03";

            vm.ClearCommand.Execute(null);

            Assert.False(vm.HasRequest);
        }

        [Fact]
        public void ClearCommand_SetsHasParsedResponseFalse()
        {
            var vm = new ModbusViewModel();
            vm.ParsedSummary = "Test";

            vm.ClearCommand.Execute(null);

            Assert.False(vm.HasParsedResponse);
        }

        [Fact]
        public void ClearCommand_UpdatesStatusMessage()
        {
            var vm = new ModbusViewModel();

            vm.ClearCommand.Execute(null);

            Assert.Contains("Cleared", vm.StatusMessage);
        }

        [Fact]
        public void BuildRequest_Failure_DoesNotLeavePartialData()
        {
            var vm = new ModbusViewModel();
            vm.SelectedRequestKind = ModbusRequestKind.WriteMultipleRegisters;
            vm.MultipleWriteValuesText = "invalid";

            vm.BuildRequestCommand.Execute(null);

            Assert.Empty(vm.RequestHex);
        }

        [Fact]
        public void PropertyChanged_TriggeredForProperties()
        {
            var vm = new ModbusViewModel();
            bool triggered = false;

            vm.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(vm.UnitId))
                {
                    triggered = true;
                }
            };

            vm.UnitId = 2;

            Assert.True(triggered);
        }
    }
}