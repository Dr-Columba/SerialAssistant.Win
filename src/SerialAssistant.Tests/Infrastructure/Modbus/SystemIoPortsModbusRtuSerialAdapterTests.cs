using SerialAssistant.Infrastructure.Modbus.Transport;
using Xunit;

namespace SerialAssistant.Tests.Infrastructure.Modbus;

public class SystemIoPortsModbusRtuSerialAdapterTests
{
    private const string ValidPortName = "COM99";
    private const int ValidBaudRate = 9600;
    private const int ValidDataBits = 8;
    private const string ValidParity = "None";
    private const string ValidStopBits = "One";

    private static string GetSolutionRoot()
    {
        var dir = Directory.GetCurrentDirectory();
        while (dir != null && !File.Exists(Path.Combine(dir, "SerialAssistant.Win.sln")))
        {
            dir = Directory.GetParent(dir)?.FullName;
        }
        return dir ?? Directory.GetCurrentDirectory();
    }

    [Fact]
    public void Constructor_SetsProperties()
    {
        var adapter = new SystemIoPortsModbusRtuSerialAdapter(
            ValidPortName, ValidBaudRate, ValidDataBits, ValidParity, ValidStopBits);

        Assert.Equal(ValidPortName, adapter.PortName);
        Assert.Equal(ValidBaudRate, adapter.BaudRate);
        Assert.Equal(ValidDataBits, adapter.DataBits);
        Assert.Equal(ValidParity, adapter.Parity);
        Assert.Equal(ValidStopBits, adapter.StopBits);
        Assert.Equal(1000, adapter.ReadTimeoutMilliseconds);
        Assert.Equal(1000, adapter.WriteTimeoutMilliseconds);
        Assert.False(adapter.IsOpen);
    }

    [Fact]
    public void Constructor_TrimsPortName()
    {
        var adapter = new SystemIoPortsModbusRtuSerialAdapter(
            "  " + ValidPortName + "  ", ValidBaudRate, ValidDataBits, ValidParity, ValidStopBits);

        Assert.Equal(ValidPortName, adapter.PortName);
    }

    [Fact]
    public void IsOpen_DefaultFalse()
    {
        var adapter = new SystemIoPortsModbusRtuSerialAdapter(
            ValidPortName, ValidBaudRate, ValidDataBits, ValidParity, ValidStopBits);

        Assert.False(adapter.IsOpen);
    }

    [Fact]
    public async Task OpenAsync_EmptyPortName_ReturnsFalse()
    {
        var adapter = new SystemIoPortsModbusRtuSerialAdapter(
            "", ValidBaudRate, ValidDataBits, ValidParity, ValidStopBits);

        var result = await adapter.OpenAsync();

        Assert.False(result);
        Assert.False(adapter.IsOpen);
    }

    [Fact]
    public async Task OpenAsync_WhitespacePortName_ReturnsFalse()
    {
        var adapter = new SystemIoPortsModbusRtuSerialAdapter(
            "   ", ValidBaudRate, ValidDataBits, ValidParity, ValidStopBits);

        var result = await adapter.OpenAsync();

        Assert.False(result);
        Assert.False(adapter.IsOpen);
    }

    [Fact]
    public async Task OpenAsync_InvalidBaudRate_Zero_ReturnsFalse()
    {
        var adapter = new SystemIoPortsModbusRtuSerialAdapter(
            ValidPortName, 0, ValidDataBits, ValidParity, ValidStopBits);

        var result = await adapter.OpenAsync();

        Assert.False(result);
        Assert.False(adapter.IsOpen);
    }

    [Fact]
    public async Task OpenAsync_InvalidBaudRate_Negative_ReturnsFalse()
    {
        var adapter = new SystemIoPortsModbusRtuSerialAdapter(
            ValidPortName, -1, ValidDataBits, ValidParity, ValidStopBits);

        var result = await adapter.OpenAsync();

        Assert.False(result);
        Assert.False(adapter.IsOpen);
    }

    [Theory]
    [InlineData(4)]
    [InlineData(9)]
    [InlineData(0)]
    [InlineData(100)]
    public async Task OpenAsync_InvalidDataBits_ReturnsFalse(int invalidDataBits)
    {
        var adapter = new SystemIoPortsModbusRtuSerialAdapter(
            ValidPortName, ValidBaudRate, invalidDataBits, ValidParity, ValidStopBits);

        var result = await adapter.OpenAsync();

        Assert.False(result);
        Assert.False(adapter.IsOpen);
    }

    [Fact]
    public async Task OpenAsync_InvalidPort_ReturnsFalse()
    {
        var adapter = new SystemIoPortsModbusRtuSerialAdapter(
            "INVALID_COM_PORT_THAT_SHOULD_NOT_EXIST", ValidBaudRate, ValidDataBits, ValidParity, ValidStopBits);

        var result = await adapter.OpenAsync();

        Assert.False(result);
        Assert.False(adapter.IsOpen);
    }

    [Fact]
    public async Task CloseAsync_WhenNotOpen_DoesNotThrow()
    {
        var adapter = new SystemIoPortsModbusRtuSerialAdapter(
            ValidPortName, ValidBaudRate, ValidDataBits, ValidParity, ValidStopBits);

        var exception = await Record.ExceptionAsync(() => adapter.CloseAsync());

        Assert.Null(exception);
        Assert.False(adapter.IsOpen);
    }

    [Fact]
    public async Task WriteAsync_WhenNotOpen_ReturnsFalse()
    {
        var adapter = new SystemIoPortsModbusRtuSerialAdapter(
            ValidPortName, ValidBaudRate, ValidDataBits, ValidParity, ValidStopBits);
        var requestBytes = new byte[] { 0x01, 0x03, 0x00, 0x00, 0x00, 0x0A };

        var result = await adapter.WriteAsync(requestBytes);

        Assert.False(result);
    }

    [Fact]
    public async Task WriteAsync_EmptyBytes_ReturnsFalse()
    {
        var adapter = new SystemIoPortsModbusRtuSerialAdapter(
            ValidPortName, ValidBaudRate, ValidDataBits, ValidParity, ValidStopBits);

        var result = await adapter.WriteAsync(Array.Empty<byte>());

        Assert.False(result);
    }

    [Fact]
    public async Task WriteAsync_NullBytes_ReturnsFalse()
    {
        var adapter = new SystemIoPortsModbusRtuSerialAdapter(
            ValidPortName, ValidBaudRate, ValidDataBits, ValidParity, ValidStopBits);

        var result = await adapter.WriteAsync(null!);

        Assert.False(result);
    }

    [Fact]
    public async Task ReadAsync_WhenNotOpen_ReturnsEmptyArray()
    {
        var adapter = new SystemIoPortsModbusRtuSerialAdapter(
            ValidPortName, ValidBaudRate, ValidDataBits, ValidParity, ValidStopBits);

        var result = await adapter.ReadAsync(256, TimeSpan.FromSeconds(1));

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task ReadAsync_InvalidMaxBytes_ReturnsEmptyArray()
    {
        var adapter = new SystemIoPortsModbusRtuSerialAdapter(
            ValidPortName, ValidBaudRate, ValidDataBits, ValidParity, ValidStopBits);

        var result1 = await adapter.ReadAsync(0, TimeSpan.FromSeconds(1));
        var result2 = await adapter.ReadAsync(-1, TimeSpan.FromSeconds(1));

        Assert.Empty(result1);
        Assert.Empty(result2);
    }

    [Fact]
    public async Task ReadAsync_InvalidTimeout_ReturnsEmptyArray()
    {
        var adapter = new SystemIoPortsModbusRtuSerialAdapter(
            ValidPortName, ValidBaudRate, ValidDataBits, ValidParity, ValidStopBits);

        var result1 = await adapter.ReadAsync(256, TimeSpan.Zero);
        var result2 = await adapter.ReadAsync(256, TimeSpan.FromMilliseconds(-1));

        Assert.Empty(result1);
        Assert.Empty(result2);
    }

    [Fact]
    public void Adapter_SourceContainsSystemIoPorts()
    {
        var root = GetSolutionRoot();
        var sourceCode = File.ReadAllText(
            Path.Combine(root, "src/SerialAssistant.Infrastructure/Modbus/Transport/SystemIoPortsModbusRtuSerialAdapter.cs"));

        Assert.Contains("System.IO.Ports", sourceCode);
        Assert.Contains("SerialPort", sourceCode);
    }

    [Fact]
    public void AppViewModels_DoNotReferenceSystemIoPorts()
    {
        var root = GetSolutionRoot();
        var viewModelFiles = Directory.GetFiles(
            Path.Combine(root, "src/SerialAssistant.App/ViewModels"), "*.cs", SearchOption.AllDirectories);

        foreach (var file in viewModelFiles)
        {
            var content = File.ReadAllText(file);
            Assert.DoesNotContain("System.IO.Ports", content);
            Assert.DoesNotContain("SystemIoPortsModbusRtuSerialAdapter", content);
        }
    }

    [Fact]
    public void Core_DoesNotReferenceSystemIoPorts()
    {
        var root = GetSolutionRoot();
        var coreFiles = Directory.GetFiles(
            Path.Combine(root, "src/SerialAssistant.Core"), "*.cs", SearchOption.AllDirectories);

        foreach (var file in coreFiles)
        {
            var content = File.ReadAllText(file);
            Assert.DoesNotContain("using System.IO.Ports;", content);
            Assert.DoesNotContain("SystemIoPortsModbusRtuSerialAdapter", content);
        }
    }

    [Theory]
    [InlineData("None", "None")]
    [InlineData("Odd", "Odd")]
    [InlineData("Even", "Even")]
    [InlineData("Mark", "Mark")]
    [InlineData("Space", "Space")]
    [InlineData("none", "None")]
    [InlineData("NONE", "None")]
    public void ParseParity_KnownValues_AreAccepted(string input, string expected)
    {
        var adapter = new SystemIoPortsModbusRtuSerialAdapter(
            ValidPortName, ValidBaudRate, ValidDataBits, input, ValidStopBits);

        Assert.Equal(expected, adapter.Parity);
    }

    [Fact]
    public void ParseParity_UnknownValue_FallsBackToNone()
    {
        var adapter = new SystemIoPortsModbusRtuSerialAdapter(
            ValidPortName, ValidBaudRate, ValidDataBits, "UnknownParity", ValidStopBits);

        Assert.Equal("None", adapter.Parity);
    }

    [Theory]
    [InlineData("One", "One")]
    [InlineData("OnePointFive", "OnePointFive")]
    [InlineData("Two", "Two")]
    [InlineData("one", "One")]
    [InlineData("ONE", "One")]
    public void ParseStopBits_KnownValues_AreAccepted(string input, string expected)
    {
        var adapter = new SystemIoPortsModbusRtuSerialAdapter(
            ValidPortName, ValidBaudRate, ValidDataBits, ValidParity, input);

        Assert.Equal(expected, adapter.StopBits);
    }

    [Fact]
    public void ParseStopBits_UnknownValue_FallsBackToOne()
    {
        var adapter = new SystemIoPortsModbusRtuSerialAdapter(
            ValidPortName, ValidBaudRate, ValidDataBits, ValidParity, "UnknownStopBits");

        Assert.Equal("One", adapter.StopBits);
    }

    [Fact]
    public async Task OpenAsync_CancellationRequested_ReturnsFalse()
    {
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        var adapter = new SystemIoPortsModbusRtuSerialAdapter(
            ValidPortName, ValidBaudRate, ValidDataBits, ValidParity, ValidStopBits);

        var result = await adapter.OpenAsync(cts.Token);

        Assert.False(result);
    }

    [Fact]
    public async Task WriteAsync_CancellationRequested_ReturnsFalse()
    {
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        var adapter = new SystemIoPortsModbusRtuSerialAdapter(
            ValidPortName, ValidBaudRate, ValidDataBits, ValidParity, ValidStopBits);
        var requestBytes = new byte[] { 0x01, 0x03 };

        var result = await adapter.WriteAsync(requestBytes, cts.Token);

        Assert.False(result);
    }

    [Fact]
    public async Task ReadAsync_CancellationRequested_ThrowsTaskCanceledException()
    {
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        var adapter = new SystemIoPortsModbusRtuSerialAdapter(
            ValidPortName, ValidBaudRate, ValidDataBits, ValidParity, ValidStopBits);

        await Assert.ThrowsAsync<TaskCanceledException>(
            () => adapter.ReadAsync(256, TimeSpan.FromSeconds(5), cts.Token));
    }
}
