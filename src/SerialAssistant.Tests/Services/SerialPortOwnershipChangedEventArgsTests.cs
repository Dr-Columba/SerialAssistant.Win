using Xunit;
using SerialAssistant.Core.Services;

namespace SerialAssistant.Tests.Services;

public class SerialPortOwnershipChangedEventArgsTests
{
    [Fact]
    public void Constructor_SetsPortName()
    {
        var args = new SerialPortOwnershipChangedEventArgs("COM3", SerialPortOwner.None, SerialPortOwner.Terminal);
        Assert.Equal("COM3", args.PortName);
    }

    [Fact]
    public void Constructor_TrimsPortName()
    {
        var args = new SerialPortOwnershipChangedEventArgs("  COM3  ", SerialPortOwner.None, SerialPortOwner.Terminal);
        Assert.Equal("COM3", args.PortName);
    }

    [Fact]
    public void Constructor_NullPortName_UsesEmptyString()
    {
        var args = new SerialPortOwnershipChangedEventArgs(null!, SerialPortOwner.None, SerialPortOwner.Terminal);
        Assert.Equal(string.Empty, args.PortName);
    }

    [Fact]
    public void Constructor_EmptyPortName_UsesEmptyString()
    {
        var args = new SerialPortOwnershipChangedEventArgs("", SerialPortOwner.None, SerialPortOwner.Terminal);
        Assert.Equal(string.Empty, args.PortName);
    }

    [Fact]
    public void Constructor_SetsPreviousOwner()
    {
        var args = new SerialPortOwnershipChangedEventArgs("COM3", SerialPortOwner.None, SerialPortOwner.Terminal);
        Assert.Equal(SerialPortOwner.None, args.PreviousOwner);
    }

    [Fact]
    public void Constructor_SetsCurrentOwner()
    {
        var args = new SerialPortOwnershipChangedEventArgs("COM3", SerialPortOwner.None, SerialPortOwner.Terminal);
        Assert.Equal(SerialPortOwner.Terminal, args.CurrentOwner);
    }

    [Fact]
    public void Constructor_SetsChangedAt()
    {
        var before = DateTimeOffset.UtcNow;
        var args = new SerialPortOwnershipChangedEventArgs("COM3", SerialPortOwner.None, SerialPortOwner.Terminal);
        var after = DateTimeOffset.UtcNow;

        Assert.True(args.ChangedAt >= before);
        Assert.True(args.ChangedAt <= after);
    }

    [Fact]
    public void Constructor_WithChangedAtParameter_UsesProvidedValue()
    {
        var expectedTime = new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero);
        var args = new SerialPortOwnershipChangedEventArgs("COM3", SerialPortOwner.None, SerialPortOwner.Terminal, expectedTime);
        Assert.Equal(expectedTime, args.ChangedAt);
    }
}
