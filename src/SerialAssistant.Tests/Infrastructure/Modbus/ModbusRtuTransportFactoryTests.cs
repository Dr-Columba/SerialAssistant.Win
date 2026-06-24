using SerialAssistant.Core.Modbus.Transport;
using SerialAssistant.Core.Services;
using SerialAssistant.Infrastructure.Modbus.Transport;
using SerialAssistant.Infrastructure.Services;
using Xunit;

namespace SerialAssistant.Tests.Infrastructure.Modbus;

/* Tests for ModbusRtuTransportFactory.
 * All tests verify factory behavior without real hardware.
 * No real serial port is opened during tests.
 * Tests verify factory implements Core IModbusRtuTransportFactory contract.
 */
public class ModbusRtuTransportFactoryTests
{
    private readonly ISerialPortOwnershipCoordinator _coordinator;
    private readonly ModbusRtuTransportFactory _factory;
    private readonly IModbusRtuTransportFactory _factoryInterface;

    public ModbusRtuTransportFactoryTests()
    {
        _coordinator = new SerialPortOwnershipCoordinator();
        _factory = new ModbusRtuTransportFactory(_coordinator);
        _factoryInterface = _factory;
    }

    /* Test 1: Constructor_NullCoordinator_ThrowsArgumentNullException */
    [Fact]
    public void Constructor_NullCoordinator_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new ModbusRtuTransportFactory(null!));
    }

    /* Test 2: Create_NullOptions_ThrowsArgumentNullException */
    [Fact]
    public void Create_NullOptions_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => _factory.Create(null!));
    }

    /* Test 3: Create_ValidOptions_ReturnsTransport */
    [Fact]
    public void Create_ValidOptions_ReturnsTransport()
    {
        var options = new ModbusRtuTransportFactoryOptions
        {
            PortName = "COM1",
            BaudRate = 9600,
            DataBits = 8,
            Parity = "None",
            StopBits = "One"
        };

        var transport = _factory.Create(options);

        Assert.NotNull(transport);
    }

    /* Test 4: Create_ValidOptions_ReturnsIModbusRtuTransport */
    [Fact]
    public void Create_ValidOptions_ReturnsIModbusRtuTransport()
    {
        var options = new ModbusRtuTransportFactoryOptions
        {
            PortName = "COM1",
            BaudRate = 9600,
            DataBits = 8,
            Parity = "None",
            StopBits = "One"
        };

        var transport = _factory.Create(options);

        Assert.IsAssignableFrom<IModbusRtuTransport>(transport);
    }

    /* Test 5: Create_DoesNotOpenTransport */
    [Fact]
    public void Create_DoesNotOpenTransport()
    {
        var options = new ModbusRtuTransportFactoryOptions
        {
            PortName = "COM1",
            BaudRate = 9600,
            DataBits = 8,
            Parity = "None",
            StopBits = "One"
        };

        var transport = _factory.Create(options);

        Assert.False(transport.IsConnected);
    }

    /* Test 6: Options_DefaultValues_AreStable */
    [Fact]
    public void Options_DefaultValues_AreStable()
    {
        var options = new ModbusRtuTransportFactoryOptions();

        Assert.Equal(string.Empty, options.PortName);
        Assert.Equal(9600, options.BaudRate);
        Assert.Equal(8, options.DataBits);
        Assert.Equal("None", options.Parity);
        Assert.Equal("One", options.StopBits);
        Assert.Equal(1000, options.ReadTimeoutMilliseconds);
        Assert.Equal(1000, options.WriteTimeoutMilliseconds);
        Assert.Equal(1000, options.SendTimeoutMilliseconds);
        Assert.Equal(1000, options.ReceiveTimeoutMilliseconds);
    }

    /* Test 7: Options_PortName_DefaultsToEmpty */
    [Fact]
    public void Options_PortName_DefaultsToEmpty()
    {
        var options = new ModbusRtuTransportFactoryOptions();

        Assert.Equal(string.Empty, options.PortName);
    }

    /* Test 8: Options_BaudRate_DefaultsTo9600 */
    [Fact]
    public void Options_BaudRate_DefaultsTo9600()
    {
        var options = new ModbusRtuTransportFactoryOptions();

        Assert.Equal(9600, options.BaudRate);
    }

    /* Test 9: Options_Parity_DefaultsToNone */
    [Fact]
    public void Options_Parity_DefaultsToNone()
    {
        var options = new ModbusRtuTransportFactoryOptions();

        Assert.Equal("None", options.Parity);
    }

    /* Test 10: Options_StopBits_DefaultsToOne */
    [Fact]
    public void Options_StopBits_DefaultsToOne()
    {
        var options = new ModbusRtuTransportFactoryOptions();

        Assert.Equal("One", options.StopBits);
    }

    /* Test 11: Create_AllowsCustomSerialSettings */
    [Fact]
    public void Create_AllowsCustomSerialSettings()
    {
        var options = new ModbusRtuTransportFactoryOptions
        {
            PortName = "COM3",
            BaudRate = 19200,
            DataBits = 7,
            Parity = "Even",
            StopBits = "Two"
        };

        var transport = _factory.Create(options);

        Assert.NotNull(transport);
    }

    /* Test 12: Create_AllowsCustomTimeouts */
    [Fact]
    public void Create_AllowsCustomTimeouts()
    {
        var options = new ModbusRtuTransportFactoryOptions
        {
            PortName = "COM1",
            BaudRate = 9600,
            DataBits = 8,
            Parity = "None",
            StopBits = "One",
            ReadTimeoutMilliseconds = 500,
            WriteTimeoutMilliseconds = 500,
            SendTimeoutMilliseconds = 2000,
            ReceiveTimeoutMilliseconds = 2000
        };

        var transport = _factory.Create(options);

        Assert.NotNull(transport);
    }

    /* Test 13: Factory_DoesNotRequireHardware */
    [Fact]
    public void Factory_DoesNotRequireHardware()
    {
        /* This test proves factory can be instantiated without real hardware */
        var coordinator = new SerialPortOwnershipCoordinator();
        var factory = new ModbusRtuTransportFactory(coordinator);

        Assert.NotNull(factory);
    }

    /* Test 14: Create_InvalidPortName_ThrowsArgumentException */
    [Fact]
    public void Create_InvalidPortName_ThrowsArgumentException()
    {
        var options = new ModbusRtuTransportFactoryOptions
        {
            PortName = "",
            BaudRate = 9600,
            DataBits = 8
        };

        Assert.Throws<ArgumentException>(() => _factory.Create(options));
    }

    /* Test 15: Create_InvalidBaudRate_ThrowsArgumentException */
    [Fact]
    public void Create_InvalidBaudRate_ThrowsArgumentException()
    {
        var options = new ModbusRtuTransportFactoryOptions
        {
            PortName = "COM1",
            BaudRate = 0,
            DataBits = 8
        };

        Assert.Throws<ArgumentException>(() => _factory.Create(options));
    }

    /* Test 16: Create_InvalidDataBits_ThrowsArgumentException */
    [Fact]
    public void Create_InvalidDataBits_ThrowsArgumentException()
    {
        var options = new ModbusRtuTransportFactoryOptions
        {
            PortName = "COM1",
            BaudRate = 9600,
            DataBits = 4
        };

        Assert.Throws<ArgumentException>(() => _factory.Create(options));
    }

    /* Test 17: Create_InvalidDataBitsTooHigh_ThrowsArgumentException */
    [Fact]
    public void Create_InvalidDataBitsTooHigh_ThrowsArgumentException()
    {
        var options = new ModbusRtuTransportFactoryOptions
        {
            PortName = "COM1",
            BaudRate = 9600,
            DataBits = 9
        };

        Assert.Throws<ArgumentException>(() => _factory.Create(options));
    }

    /* Test 18: Options_Validate_ValidOptions_ReturnsNull */
    [Fact]
    public void Options_Validate_ValidOptions_ReturnsNull()
    {
        var options = new ModbusRtuTransportFactoryOptions
        {
            PortName = "COM1",
            BaudRate = 9600,
            DataBits = 8
        };

        var result = options.Validate();

        Assert.Null(result);
    }

    /* Test 19: Options_Validate_EmptyPortName_ReturnsError */
    [Fact]
    public void Options_Validate_EmptyPortName_ReturnsError()
    {
        var options = new ModbusRtuTransportFactoryOptions
        {
            PortName = "",
            BaudRate = 9600,
            DataBits = 8
        };

        var result = options.Validate();

        Assert.NotNull(result);
        Assert.Contains("PortName", result);
    }

    /* Test 20: Options_Validate_ZeroBaudRate_ReturnsError */
    [Fact]
    public void Options_Validate_ZeroBaudRate_ReturnsError()
    {
        var options = new ModbusRtuTransportFactoryOptions
        {
            PortName = "COM1",
            BaudRate = 0,
            DataBits = 8
        };

        var result = options.Validate();

        Assert.NotNull(result);
        Assert.Contains("BaudRate", result);
    }

    /* Test 21: Create_TransportHasCorrectOptions */
    [Fact]
    public void Create_TransportHasCorrectOptions()
    {
        var options = new ModbusRtuTransportFactoryOptions
        {
            PortName = "COM1",
            BaudRate = 9600,
            DataBits = 8,
            Parity = "None",
            StopBits = "One",
            SendTimeoutMilliseconds = 2000,
            ReceiveTimeoutMilliseconds = 3000
        };

        var transport = _factory.Create(options);

        Assert.Equal(TimeSpan.FromMilliseconds(2000), transport.Options.SendTimeout);
        Assert.Equal(TimeSpan.FromMilliseconds(3000), transport.Options.ReceiveTimeout);
    }

    /* Test 22: Create_MultipleTransportsForDifferentPorts */
    [Fact]
    public void Create_MultipleTransportsForDifferentPorts()
    {
        var options1 = new ModbusRtuTransportFactoryOptions { PortName = "COM1", BaudRate = 9600, DataBits = 8 };
        var options2 = new ModbusRtuTransportFactoryOptions { PortName = "COM2", BaudRate = 9600, DataBits = 8 };

        var transport1 = _factory.Create(options1);
        var transport2 = _factory.Create(options2);

        Assert.NotNull(transport1);
        Assert.NotNull(transport2);
        Assert.NotSame(transport1, transport2);
    }

    /* Test 23: Create_DoesNotClaimOwnership */
    [Fact]
    public void Create_DoesNotClaimOwnership()
    {
        var options = new ModbusRtuTransportFactoryOptions
        {
            PortName = "COM1",
            BaudRate = 9600,
            DataBits = 8
        };

        _factory.Create(options);

        Assert.False(_coordinator.IsOwned("COM1"));
    }

    /* Test 24: Create_WhitespacePortName_ThrowsArgumentException */
    [Fact]
    public void Create_WhitespacePortName_ThrowsArgumentException()
    {
        var options = new ModbusRtuTransportFactoryOptions
        {
            PortName = "   ",
            BaudRate = 9600,
            DataBits = 8
        };

        Assert.Throws<ArgumentException>(() => _factory.Create(options));
    }

    /* Test 25: Create_NegativeTimeout_ThrowsArgumentException */
    [Fact]
    public void Create_NegativeTimeout_ThrowsArgumentException()
    {
        var options = new ModbusRtuTransportFactoryOptions
        {
            PortName = "COM1",
            BaudRate = 9600,
            DataBits = 8,
            ReadTimeoutMilliseconds = -1
        };

        Assert.Throws<ArgumentException>(() => _factory.Create(options));
    }

    /* Test 26: Factory_ImplementsCoreInterface */
    [Fact]
    public void Factory_ImplementsCoreInterface()
    {
        Assert.IsAssignableFrom<IModbusRtuTransportFactory>(_factory);
    }

    /* Test 27: Interface_Create_ValidOptions_ReturnsTransport */
    [Fact]
    public void Interface_Create_ValidOptions_ReturnsTransport()
    {
        var options = new ModbusRtuTransportFactoryOptions
        {
            PortName = "COM1",
            BaudRate = 9600,
            DataBits = 8
        };

        var transport = _factoryInterface.Create(options);

        Assert.NotNull(transport);
        Assert.IsAssignableFrom<IModbusRtuTransport>(transport);
    }

    /* Test 28: Interface_Create_DoesNotOpenTransport */
    [Fact]
    public void Interface_Create_DoesNotOpenTransport()
    {
        var options = new ModbusRtuTransportFactoryOptions
        {
            PortName = "COM1",
            BaudRate = 9600,
            DataBits = 8
        };

        var transport = _factoryInterface.Create(options);

        Assert.False(transport.IsConnected);
    }

    /* Test 29: Interface_Create_DoesNotClaimOwnership */
    [Fact]
    public void Interface_Create_DoesNotClaimOwnership()
    {
        var options = new ModbusRtuTransportFactoryOptions
        {
            PortName = "COM1",
            BaudRate = 9600,
            DataBits = 8
        };

        _factoryInterface.Create(options);

        Assert.False(_coordinator.IsOwned("COM1"));
    }

    /* Test 30: Options_DataBits_DefaultsTo8 */
    [Fact]
    public void Options_DataBits_DefaultsTo8()
    {
        var options = new ModbusRtuTransportFactoryOptions();

        Assert.Equal(8, options.DataBits);
    }

    /* Test 31: Options_Validate_ValidOptions_ReturnsTrue */
    [Fact]
    public void Options_Validate_ValidOptions_ReturnsTrue()
    {
        var options = new ModbusRtuTransportFactoryOptions
        {
            PortName = "COM1",
            BaudRate = 9600,
            DataBits = 8
        };

        var result = options.Validate();

        Assert.Null(result);
    }

    /* Test 32: Options_IsInCoreNamespace */
    [Fact]
    public void Options_IsInCoreNamespace()
    {
        var options = new ModbusRtuTransportFactoryOptions();

        Assert.Equal("SerialAssistant.Core.Modbus.Transport", options.GetType().Namespace);
    }

    /* Test 33: Interface_IsInCoreNamespace */
    [Fact]
    public void Interface_IsInCoreNamespace()
    {
        Assert.Equal("SerialAssistant.Core.Modbus.Transport", typeof(IModbusRtuTransportFactory).Namespace);
    }
}