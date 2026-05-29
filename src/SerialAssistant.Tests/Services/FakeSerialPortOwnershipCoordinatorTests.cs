using Xunit;
using SerialAssistant.Core.Services;
using SerialAssistant.Tests.Infrastructure;

namespace SerialAssistant.Tests.Services;

public class FakeSerialPortOwnershipCoordinatorTests
{
    [Fact]
    public void GetCurrentOwner_UnknownPort_ReturnsNone()
    {
        var coordinator = new FakeSerialPortOwnershipCoordinator();
        var owner = coordinator.GetCurrentOwner("COM3");
        Assert.Equal(SerialPortOwner.None, owner);
    }

    [Fact]
    public void IsOwned_UnknownPort_ReturnsFalse()
    {
        var coordinator = new FakeSerialPortOwnershipCoordinator();
        var isOwned = coordinator.IsOwned("COM3");
        Assert.False(isOwned);
    }

    [Fact]
    public void TryClaimOwnership_EmptyPort_ReturnsFalse()
    {
        var coordinator = new FakeSerialPortOwnershipCoordinator();
        var result = coordinator.TryClaimOwnership("", SerialPortOwner.Terminal);
        Assert.False(result);
    }

    [Fact]
    public void TryClaimOwnership_NoneOwner_ReturnsFalse()
    {
        var coordinator = new FakeSerialPortOwnershipCoordinator();
        var result = coordinator.TryClaimOwnership("COM3", SerialPortOwner.None);
        Assert.False(result);
    }

    [Fact]
    public void TryClaimOwnership_UnownedPort_ReturnsTrue()
    {
        var coordinator = new FakeSerialPortOwnershipCoordinator();
        var result = coordinator.TryClaimOwnership("COM3", SerialPortOwner.Terminal);
        Assert.True(result);
    }

    [Fact]
    public void TryClaimOwnership_UnownedPort_SetsOwner()
    {
        var coordinator = new FakeSerialPortOwnershipCoordinator();
        coordinator.TryClaimOwnership("COM3", SerialPortOwner.Terminal);
        var owner = coordinator.GetCurrentOwner("COM3");
        Assert.Equal(SerialPortOwner.Terminal, owner);
    }

    [Fact]
    public void TryClaimOwnership_SameOwner_ReturnsTrue()
    {
        var coordinator = new FakeSerialPortOwnershipCoordinator();
        coordinator.TryClaimOwnership("COM3", SerialPortOwner.Terminal);
        var result = coordinator.TryClaimOwnership("COM3", SerialPortOwner.Terminal);
        Assert.True(result);
    }

    [Fact]
    public void TryClaimOwnership_SameOwner_DoesNotRaiseDuplicateEvent()
    {
        var coordinator = new FakeSerialPortOwnershipCoordinator();
        var eventRaisedCount = 0;
        coordinator.OwnershipChanged += (s, e) => eventRaisedCount++;
        coordinator.TryClaimOwnership("COM3", SerialPortOwner.Terminal);
        var initialCount = eventRaisedCount;
        coordinator.TryClaimOwnership("COM3", SerialPortOwner.Terminal);
        Assert.Equal(initialCount, eventRaisedCount);
    }

    [Fact]
    public void TryClaimOwnership_DifferentOwner_ReturnsFalse()
    {
        var coordinator = new FakeSerialPortOwnershipCoordinator();
        coordinator.TryClaimOwnership("COM3", SerialPortOwner.Terminal);
        var result = coordinator.TryClaimOwnership("COM3", SerialPortOwner.ModbusRtu);
        Assert.False(result);
    }

    [Fact]
    public void TryClaimOwnership_DifferentOwner_DoesNotChangeOwner()
    {
        var coordinator = new FakeSerialPortOwnershipCoordinator();
        coordinator.TryClaimOwnership("COM3", SerialPortOwner.Terminal);
        coordinator.TryClaimOwnership("COM3", SerialPortOwner.ModbusRtu);
        var owner = coordinator.GetCurrentOwner("COM3");
        Assert.Equal(SerialPortOwner.Terminal, owner);
    }

    [Fact]
    public void TryReleaseOwnership_CurrentOwner_ReturnsTrue()
    {
        var coordinator = new FakeSerialPortOwnershipCoordinator();
        coordinator.TryClaimOwnership("COM3", SerialPortOwner.Terminal);
        var result = coordinator.TryReleaseOwnership("COM3", SerialPortOwner.Terminal);
        Assert.True(result);
    }

    [Fact]
    public void TryReleaseOwnership_CurrentOwner_SetsOwnerNone()
    {
        var coordinator = new FakeSerialPortOwnershipCoordinator();
        coordinator.TryClaimOwnership("COM3", SerialPortOwner.Terminal);
        coordinator.TryReleaseOwnership("COM3", SerialPortOwner.Terminal);
        var owner = coordinator.GetCurrentOwner("COM3");
        Assert.Equal(SerialPortOwner.None, owner);
    }

    [Fact]
    public void TryReleaseOwnership_WrongOwner_ReturnsFalse()
    {
        var coordinator = new FakeSerialPortOwnershipCoordinator();
        coordinator.TryClaimOwnership("COM3", SerialPortOwner.Terminal);
        var result = coordinator.TryReleaseOwnership("COM3", SerialPortOwner.ModbusRtu);
        Assert.False(result);
    }

    [Fact]
    public void TryReleaseOwnership_UnknownPort_ReturnsFalse()
    {
        var coordinator = new FakeSerialPortOwnershipCoordinator();
        var result = coordinator.TryReleaseOwnership("COM3", SerialPortOwner.Terminal);
        Assert.False(result);
    }

    [Fact]
    public void TryReleaseOwnership_RaisesOwnershipChanged()
    {
        var coordinator = new FakeSerialPortOwnershipCoordinator();
        var eventRaised = false;
        coordinator.TryClaimOwnership("COM3", SerialPortOwner.Terminal);
        coordinator.OwnershipChanged += (s, e) => eventRaised = true;
        coordinator.TryReleaseOwnership("COM3", SerialPortOwner.Terminal);
        Assert.True(eventRaised);
    }

    [Fact]
    public void OwnershipChanged_Claim_RaisesExpectedOwners()
    {
        var coordinator = new FakeSerialPortOwnershipCoordinator();
        SerialPortOwnershipChangedEventArgs? eventArgs = null;
        coordinator.OwnershipChanged += (s, e) => eventArgs = e;
        coordinator.TryClaimOwnership("COM3", SerialPortOwner.Terminal);
        Assert.NotNull(eventArgs);
        Assert.Equal("COM3", eventArgs.PortName);
        Assert.Equal(SerialPortOwner.None, eventArgs.PreviousOwner);
        Assert.Equal(SerialPortOwner.Terminal, eventArgs.CurrentOwner);
    }

    [Fact]
    public void OwnershipChanged_Release_RaisesExpectedOwners()
    {
        var coordinator = new FakeSerialPortOwnershipCoordinator();
        coordinator.TryClaimOwnership("COM3", SerialPortOwner.Terminal);
        SerialPortOwnershipChangedEventArgs? eventArgs = null;
        coordinator.OwnershipChanged += (s, e) => eventArgs = e;
        coordinator.TryReleaseOwnership("COM3", SerialPortOwner.Terminal);
        Assert.NotNull(eventArgs);
        Assert.Equal("COM3", eventArgs.PortName);
        Assert.Equal(SerialPortOwner.Terminal, eventArgs.PreviousOwner);
        Assert.Equal(SerialPortOwner.None, eventArgs.CurrentOwner);
    }

    [Fact]
    public void PortName_IsCaseInsensitive()
    {
        var coordinator = new FakeSerialPortOwnershipCoordinator();
        coordinator.TryClaimOwnership("COM3", SerialPortOwner.Terminal);
        var owner = coordinator.GetCurrentOwner("com3");
        Assert.Equal(SerialPortOwner.Terminal, owner);
    }

    [Fact]
    public void PortName_IsTrimmed()
    {
        var coordinator = new FakeSerialPortOwnershipCoordinator();
        coordinator.TryClaimOwnership("  COM3  ", SerialPortOwner.Terminal);
        var owner = coordinator.GetCurrentOwner("COM3");
        Assert.Equal(SerialPortOwner.Terminal, owner);
    }

    [Fact]
    public void MultiplePorts_AreTrackedIndependently()
    {
        var coordinator = new FakeSerialPortOwnershipCoordinator();
        coordinator.TryClaimOwnership("COM3", SerialPortOwner.Terminal);
        coordinator.TryClaimOwnership("COM4", SerialPortOwner.ModbusRtu);
        var owner3 = coordinator.GetCurrentOwner("COM3");
        var owner4 = coordinator.GetCurrentOwner("COM4");
        Assert.Equal(SerialPortOwner.Terminal, owner3);
        Assert.Equal(SerialPortOwner.ModbusRtu, owner4);
    }

    [Fact]
    public void TerminalAndModbus_CannotClaimSamePortAtSameTime()
    {
        var coordinator = new FakeSerialPortOwnershipCoordinator();
        coordinator.TryClaimOwnership("COM3", SerialPortOwner.Terminal);
        var result = coordinator.TryClaimOwnership("COM3", SerialPortOwner.ModbusRtu);
        Assert.False(result);
    }

    [Fact]
    public void ReleaseThenOtherOwnerCanClaim()
    {
        var coordinator = new FakeSerialPortOwnershipCoordinator();
        coordinator.TryClaimOwnership("COM3", SerialPortOwner.Terminal);
        coordinator.TryReleaseOwnership("COM3", SerialPortOwner.Terminal);
        var result = coordinator.TryClaimOwnership("COM3", SerialPortOwner.ModbusRtu);
        Assert.True(result);
    }
}
