using SerialAssistant.Core.Modbus.Rtu;
using SerialAssistant.Core.Modbus.Tcp;
using SerialAssistant.Core.Modbus.Transport;
using SerialAssistant.Core.Utilities;
using SerialAssistant.App.Commands;
using System.Windows.Input;

namespace SerialAssistant.App.ViewModels
{
    public class ModbusViewModel : BaseViewModel
    {
        private readonly IModbusTransport? _transport;
        private bool _isTransportAvailable;
        private bool _isConnected;
        private string _connectionStatus = "Not connected";
        private bool _isBusy;
        private string _lastTransportError = string.Empty;

        private ModbusTransportMode _selectedTransportMode = ModbusTransportMode.Rtu;
        private ModbusRequestKind _selectedRequestKind = ModbusRequestKind.ReadHoldingRegisters;
        private byte _unitId = 1;
        private ushort _transactionId = 1;
        private ushort _startAddress;
        private ushort _quantity = 1;
        private ushort _singleWriteValue;
        private string _multipleWriteValuesText = string.Empty;
        private string _requestHex = string.Empty;
        private string _responseHex = string.Empty;
        private string _parsedSummary = string.Empty;
        private string _statusMessage = "Ready";

        private static readonly IReadOnlyList<ModbusTransportMode> _transportModes = new List<ModbusTransportMode>
        {
            ModbusTransportMode.Rtu,
            ModbusTransportMode.Tcp
        }.AsReadOnly();

        private static readonly IReadOnlyList<ModbusRequestKind> _requestKinds = new List<ModbusRequestKind>
        {
            ModbusRequestKind.ReadHoldingRegisters,
            ModbusRequestKind.ReadInputRegisters,
            ModbusRequestKind.WriteSingleRegister,
            ModbusRequestKind.WriteMultipleRegisters
        }.AsReadOnly();

        public ModbusTransportMode SelectedTransportMode
        {
            get => _selectedTransportMode;
            set
            {
                if (SetProperty(ref _selectedTransportMode, value))
                {
                    OnPropertyChanged(nameof(IsRtu));
                    OnPropertyChanged(nameof(IsTcp));
                }
            }
        }

        public ModbusRequestKind SelectedRequestKind
        {
            get => _selectedRequestKind;
            set => SetProperty(ref _selectedRequestKind, value);
        }

        public byte UnitId
        {
            get => _unitId;
            set => SetProperty(ref _unitId, value);
        }

        public ushort TransactionId
        {
            get => _transactionId;
            set => SetProperty(ref _transactionId, value);
        }

        public ushort StartAddress
        {
            get => _startAddress;
            set => SetProperty(ref _startAddress, value);
        }

        public ushort Quantity
        {
            get => _quantity;
            set => SetProperty(ref _quantity, value);
        }

        public ushort SingleWriteValue
        {
            get => _singleWriteValue;
            set => SetProperty(ref _singleWriteValue, value);
        }

        public string MultipleWriteValuesText
        {
            get => _multipleWriteValuesText;
            set => SetProperty(ref _multipleWriteValuesText, value);
        }

        public string RequestHex
        {
            get => _requestHex;
            set => SetProperty(ref _requestHex, value);
        }

        public string ResponseHex
        {
            get => _responseHex;
            set => SetProperty(ref _responseHex, value);
        }

        public string ParsedSummary
        {
            get => _parsedSummary;
            set => SetProperty(ref _parsedSummary, value);
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        public bool IsRtu => SelectedTransportMode == ModbusTransportMode.Rtu;

        public bool IsTcp => SelectedTransportMode == ModbusTransportMode.Tcp;

        public bool HasRequest => !string.IsNullOrEmpty(RequestHex);

        public bool HasParsedResponse => !string.IsNullOrEmpty(ParsedSummary);

        public IReadOnlyList<ModbusTransportMode> TransportModes => _transportModes;

        public IReadOnlyList<ModbusRequestKind> RequestKinds => _requestKinds;

        public bool IsTransportAvailable => _isTransportAvailable;

        public bool IsConnected
        {
            get => _isConnected;
            private set
            {
                if (SetProperty(ref _isConnected, value))
                {
                    OnPropertyChanged(nameof(CanSendRequest));
                    OnPropertyChanged(nameof(ConnectionStatus));
                }
            }
        }

        public string ConnectionStatus
        {
            get => _connectionStatus;
            private set => SetProperty(ref _connectionStatus, value);
        }

        public bool CanSendRequest => IsTransportAvailable && IsConnected && HasRequest && !IsBusy;

        public bool IsBusy
        {
            get => _isBusy;
            private set
            {
                if (SetProperty(ref _isBusy, value))
                {
                    OnPropertyChanged(nameof(CanSendRequest));
                }
            }
        }

        public string LastTransportError
        {
            get => _lastTransportError;
            private set => SetProperty(ref _lastTransportError, value);
        }

        public ICommand BuildRequestCommand { get; }
        public ICommand ParseResponseCommand { get; }
        public ICommand ClearCommand { get; }
        public ICommand ConnectTransportCommand { get; }
        public ICommand DisconnectTransportCommand { get; }
        public ICommand SendRequestCommand { get; }

        public ModbusViewModel()
        {
            _isTransportAvailable = false;
            BuildRequestCommand = new RelayCommand(BuildRequest);
            ParseResponseCommand = new RelayCommand(ParseResponse);
            ClearCommand = new RelayCommand(Clear);
            ConnectTransportCommand = new RelayCommand(async _ => await ConnectTransportAsync(), _ => CanExecuteConnect());
            DisconnectTransportCommand = new RelayCommand(async _ => await DisconnectTransportAsync(), _ => CanExecuteDisconnect());
            SendRequestCommand = new RelayCommand(async _ => await SendRequestAsync(), _ => CanSendRequest);
        }

        public ModbusViewModel(IModbusTransport transport)
        {
            _transport = transport ?? throw new ArgumentNullException(nameof(transport));
            _isTransportAvailable = true;
            _isConnected = transport.IsConnected;
            UpdateConnectionStatus();

            BuildRequestCommand = new RelayCommand(BuildRequest);
            ParseResponseCommand = new RelayCommand(ParseResponse);
            ClearCommand = new RelayCommand(Clear);
            ConnectTransportCommand = new RelayCommand(async _ => await ConnectTransportAsync(), _ => CanExecuteConnect());
            DisconnectTransportCommand = new RelayCommand(async _ => await DisconnectTransportAsync(), _ => CanExecuteDisconnect());
            SendRequestCommand = new RelayCommand(async _ => await SendRequestAsync(), _ => CanSendRequest);
        }

        private bool CanExecuteConnect()
        {
            return IsTransportAvailable && !IsConnected && !IsBusy;
        }

        private bool CanExecuteDisconnect()
        {
            return IsTransportAvailable && IsConnected && !IsBusy;
        }

        public async Task ConnectTransportAsync()
        {
            if (!IsTransportAvailable)
            {
                StatusMessage = "Error: No transport available";
                return;
            }

            if (IsConnected)
            {
                StatusMessage = "Already connected";
                return;
            }

            try
            {
                IsBusy = true;
                LastTransportError = string.Empty;
                StatusMessage = "Connecting...";

                var success = await _transport!.ConnectAsync();

                if (success)
                {
                    IsConnected = true;
                    UpdateConnectionStatus();
                    StatusMessage = "Connected successfully";
                }
                else
                {
                    IsConnected = false;
                    UpdateConnectionStatus();
                    LastTransportError = "Connection failed";
                    StatusMessage = "Error: Connection failed";
                }
            }
            catch (Exception ex)
            {
                IsConnected = false;
                UpdateConnectionStatus();
                LastTransportError = ex.Message;
                StatusMessage = $"Error: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task DisconnectTransportAsync()
        {
            if (!IsTransportAvailable)
            {
                return;
            }

            if (!IsConnected)
            {
                return;
            }

            try
            {
                IsBusy = true;
                StatusMessage = "Disconnecting...";

                await _transport!.DisconnectAsync();

                IsConnected = false;
                UpdateConnectionStatus();
                StatusMessage = "Disconnected";
            }
            catch (Exception ex)
            {
                LastTransportError = ex.Message;
                StatusMessage = $"Error during disconnect: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task SendRequestAsync()
        {
            if (!IsTransportAvailable)
            {
                StatusMessage = "Error: No transport available";
                LastTransportError = "Transport not available";
                return;
            }

            if (!IsConnected)
            {
                StatusMessage = "Error: Not connected";
                LastTransportError = "Not connected";
                return;
            }

            if (string.IsNullOrEmpty(RequestHex))
            {
                StatusMessage = "Error: Request HEX is empty. Please build a request first.";
                LastTransportError = "Request empty";
                return;
            }

            var parseResult = HexConverter.FromHexString(RequestHex);
            if (!parseResult.IsSuccess || parseResult.Value == null)
            {
                StatusMessage = "Error: Invalid request HEX";
                LastTransportError = "Invalid request HEX";
                return;
            }

            byte[] requestBytes = parseResult.Value;

            var context = CreateRequestContext();
            if (!context.IsValid())
            {
                StatusMessage = "Error: Invalid request context";
                LastTransportError = "Invalid context";
                return;
            }

            try
            {
                IsBusy = true;
                LastTransportError = string.Empty;
                StatusMessage = "Sending request...";

                var result = await _transport!.SendRequestAsync(context, requestBytes);

                if (result.Success && result.ResponseBytes != null && result.ResponseBytes.Length > 0)
                {
                    ResponseHex = HexConverter.ToHexString(result.ResponseBytes);
                    OnPropertyChanged(nameof(ResponseHex));

                    StatusMessage = $"Success in {result.Duration.TotalMilliseconds:F0}ms";

                    ParseResponse();
                }
                else
                {
                    IsConnected = _transport.IsConnected;
                    UpdateConnectionStatus();

                    string errorMessage = result.ErrorMessage ?? $"Error: {result.ErrorCode}";
                    LastTransportError = $"{result.ErrorCode} - {errorMessage}";
                    StatusMessage = $"Error: {errorMessage}";

                    if (result.ErrorCode == ModbusTransportErrorCode.NotConnected)
                    {
                        IsConnected = false;
                    }
                }
            }
            catch (Exception ex)
            {
                LastTransportError = ex.Message;
                StatusMessage = $"Error: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }

        private ModbusRequestContext CreateRequestContext()
        {
            var context = new ModbusRequestContext
            {
                UnitId = UnitId,
                TransactionId = TransactionId,
                FunctionCode = GetFunctionCode(),
                StartAddress = StartAddress,
                Quantity = GetQuantity()
            };

            return context;
        }

        private byte GetFunctionCode()
        {
            return SelectedRequestKind switch
            {
                ModbusRequestKind.ReadHoldingRegisters => 0x03,
                ModbusRequestKind.ReadInputRegisters => 0x04,
                ModbusRequestKind.WriteSingleRegister => 0x06,
                ModbusRequestKind.WriteMultipleRegisters => 0x10,
                _ => 0x00
            };
        }

        private ushort GetQuantity()
        {
            if (SelectedRequestKind == ModbusRequestKind.WriteSingleRegister)
            {
                return 1;
            }

            return Quantity > 0 ? Quantity : (ushort)1;
        }

        private void UpdateConnectionStatus()
        {
            if (!IsTransportAvailable)
            {
                ConnectionStatus = "No transport";
            }
            else if (IsConnected)
            {
                ConnectionStatus = "Connected";
            }
            else
            {
                ConnectionStatus = "Disconnected";
            }
        }

        private void BuildRequest()
        {
            try
            {
                if (SelectedTransportMode == ModbusTransportMode.Rtu)
                {
                    BuildRtuRequest();
                }
                else
                {
                    BuildTcpRequest();
                }

                StatusMessage = "Request built successfully";
                OnPropertyChanged(nameof(HasRequest));
                OnPropertyChanged(nameof(CanSendRequest));
            }
            catch (Exception ex)
            {
                RequestHex = string.Empty;
                StatusMessage = $"Error: Build failed - {ex.Message}";
                OnPropertyChanged(nameof(HasRequest));
                OnPropertyChanged(nameof(CanSendRequest));
            }
        }

        private void BuildRtuRequest()
        {
            ModbusRtuFrame frame = SelectedRequestKind switch
            {
                ModbusRequestKind.ReadHoldingRegisters =>
                    ModbusRtuRequestBuilder.BuildReadHoldingRegisters(UnitId, StartAddress, Quantity),
                ModbusRequestKind.ReadInputRegisters =>
                    ModbusRtuRequestBuilder.BuildReadInputRegisters(UnitId, StartAddress, Quantity),
                ModbusRequestKind.WriteSingleRegister =>
                    ModbusRtuRequestBuilder.BuildWriteSingleRegister(UnitId, StartAddress, SingleWriteValue),
                ModbusRequestKind.WriteMultipleRegisters =>
                    ModbusRtuRequestBuilder.BuildWriteMultipleRegisters(UnitId, StartAddress, ParseMultipleValues()),
                _ => throw new ArgumentOutOfRangeException(nameof(SelectedRequestKind))
            };

            RequestHex = HexConverter.ToHexString(frame.ToByteArray());
        }

        private void BuildTcpRequest()
        {
            ModbusTcpFrame frame = SelectedRequestKind switch
            {
                ModbusRequestKind.ReadHoldingRegisters =>
                    ModbusTcpRequestBuilder.BuildReadHoldingRegisters(TransactionId, UnitId, StartAddress, Quantity),
                ModbusRequestKind.ReadInputRegisters =>
                    ModbusTcpRequestBuilder.BuildReadInputRegisters(TransactionId, UnitId, StartAddress, Quantity),
                ModbusRequestKind.WriteSingleRegister =>
                    ModbusTcpRequestBuilder.BuildWriteSingleRegister(TransactionId, UnitId, StartAddress, SingleWriteValue),
                ModbusRequestKind.WriteMultipleRegisters =>
                    ModbusTcpRequestBuilder.BuildWriteMultipleRegisters(TransactionId, UnitId, StartAddress, ParseMultipleValues()),
                _ => throw new ArgumentOutOfRangeException(nameof(SelectedRequestKind))
            };

            RequestHex = HexConverter.ToHexString(frame.ToByteArray());
        }

        private IReadOnlyList<ushort> ParseMultipleValues()
        {
            string input = MultipleWriteValuesText?.Trim() ?? string.Empty;

            if (string.IsNullOrEmpty(input))
            {
                throw new ArgumentException("Multiple write values cannot be empty");
            }

            string[] parts = input.Split(new[] { ' ', ',', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length == 0)
            {
                throw new ArgumentException("No valid values found in input");
            }

            List<ushort> values = new List<ushort>();

            foreach (string part in parts)
            {
                string trimmedPart = part.Trim();

                if (trimmedPart.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                {
                    trimmedPart = trimmedPart.Substring(2);
                }

                if (!ushort.TryParse(trimmedPart, System.Globalization.NumberStyles.HexNumber, null, out ushort value))
                {
                    throw new ArgumentException($"Invalid hex value: {part}");
                }

                values.Add(value);
            }

            return values.AsReadOnly();
        }

        private void ParseResponse()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(ResponseHex))
                {
                    ParsedSummary = string.Empty;
                    StatusMessage = "Error: Response HEX cannot be empty";
                    OnPropertyChanged(nameof(HasParsedResponse));
                    return;
                }

                var parseResult = HexConverter.FromHexString(ResponseHex);

                if (!parseResult.IsSuccess)
                {
                    ParsedSummary = string.Empty;
                    StatusMessage = $"Error: {parseResult.ErrorMessage}";
                    OnPropertyChanged(nameof(HasParsedResponse));
                    return;
                }

                byte[]? responseBytes = parseResult.Value;
                if (responseBytes == null)
                {
                    ParsedSummary = string.Empty;
                    StatusMessage = "Error: Response bytes is null";
                    OnPropertyChanged(nameof(HasParsedResponse));
                    return;
                }

                if (SelectedTransportMode == ModbusTransportMode.Rtu)
                {
                    ParseRtuResponse(responseBytes);
                }
                else
                {
                    ParseTcpResponse(responseBytes);
                }

                OnPropertyChanged(nameof(HasParsedResponse));
            }
            catch (Exception ex)
            {
                ParsedSummary = string.Empty;
                StatusMessage = $"Parse failed: {ex.Message}";
                OnPropertyChanged(nameof(HasParsedResponse));
            }
        }

        private void ParseRtuResponse(byte[] responseBytes)
        {
            var result = ModbusRtuResponseParser.Parse(responseBytes);

            if (result.IsExceptionResponse)
            {
                ParsedSummary = $"Exception Response - Function: 0x{result.FunctionCode:X2}, Exception Code: 0x{result.ExceptionCode:X2}";
                StatusMessage = "Exception response parsed";
                OnPropertyChanged(nameof(ParsedSummary));
            }
            else if (result.IsSuccess)
            {
                ParsedSummary = FormatParseResult(
                    result.SlaveAddress,
                    result.FunctionCode,
                    result.Address,
                    result.Quantity,
                    result.Value,
                    result.Registers);
                StatusMessage = "Response parsed successfully";
                OnPropertyChanged(nameof(ParsedSummary));
            }
            else
            {
                ParsedSummary = string.Empty;
                StatusMessage = $"Error: Parse failed - {result.ErrorCode}";
                OnPropertyChanged(nameof(ParsedSummary));
            }
        }

        private void ParseTcpResponse(byte[] responseBytes)
        {
            var result = ModbusTcpResponseParser.Parse(responseBytes);

            if (result.IsExceptionResponse)
            {
                ParsedSummary = $"Exception Response - Function: 0x{result.FunctionCode:X2}, Exception Code: 0x{result.ExceptionCode:X2}";
                StatusMessage = "Exception response parsed";
                OnPropertyChanged(nameof(ParsedSummary));
            }
            else if (result.IsSuccess)
            {
                ParsedSummary = FormatParseResult(
                    result.UnitId,
                    result.FunctionCode,
                    result.Address,
                    result.Quantity,
                    result.Value,
                    result.Registers);
                StatusMessage = "Response parsed successfully";
                OnPropertyChanged(nameof(ParsedSummary));
            }
            else
            {
                ParsedSummary = string.Empty;
                StatusMessage = $"Error: Parse failed - {result.ErrorCode}";
                OnPropertyChanged(nameof(ParsedSummary));
            }
        }

        private string FormatParseResult(byte? slaveOrUnitId, byte functionCode, ushort? address, ushort? quantity, ushort? value, IReadOnlyList<ushort>? registers)
        {
            string prefix = slaveOrUnitId.HasValue ? $"Unit/Slave: {slaveOrUnitId.Value}, " : string.Empty;

            return functionCode switch
            {
                0x03 or 0x04 =>
                    $"{prefix}Function: 0x{functionCode:X2}, Quantity: {quantity}, Registers: {FormatRegisters(registers)}",
                0x06 =>
                    $"{prefix}Function: 0x{functionCode:X2}, Address: 0x{address:X4}, Value: 0x{value:X4}",
                0x10 =>
                    $"{prefix}Function: 0x{functionCode:X2}, Address: 0x{address:X4}, Quantity: {quantity}",
                _ =>
                    $"{prefix}Function: 0x{functionCode:X2}"
            };
        }

        private string FormatRegisters(IReadOnlyList<ushort>? registers)
        {
            if (registers == null || registers.Count == 0)
            {
                return "[]";
            }

            return "[" + string.Join(", ", registers.Select(r => $"0x{r:X4}")) + "]";
        }

        private void Clear()
        {
            RequestHex = string.Empty;
            ResponseHex = string.Empty;
            ParsedSummary = string.Empty;
            StatusMessage = "Cleared";
            OnPropertyChanged(nameof(HasRequest));
            OnPropertyChanged(nameof(HasParsedResponse));
            OnPropertyChanged(nameof(CanSendRequest));
        }
    }
}
