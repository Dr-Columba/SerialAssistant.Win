using Xunit;
using SerialAssistant.Core.Services;

namespace SerialAssistant.Tests.Services;

public class SerialPortOwnerTests
{
    [Fact]
    public void SerialPortOwner_HasExpectedValues()
    {
        Assert.True(Enum.IsDefined(typeof(SerialPortOwner), SerialPortOwner.None));
        Assert.True(Enum.IsDefined(typeof(SerialPortOwner), SerialPortOwner.Terminal));
        Assert.True(Enum.IsDefined(typeof(SerialPortOwner), SerialPortOwner.ModbusRtu));
        Assert.Equal(3, Enum.GetValues(typeof(SerialPortOwner)).Length);
    }

    [Fact]
    public void SerialPortOwner_None_IsDefault()
    {
        var defaultOwner = default(SerialPortOwner);
        Assert.Equal(SerialPortOwner.None, defaultOwner);
    }
}
