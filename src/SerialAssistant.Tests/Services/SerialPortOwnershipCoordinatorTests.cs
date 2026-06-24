using SerialAssistant.Core.Services;
using SerialAssistant.Infrastructure.Services;
using Xunit;

namespace SerialAssistant.Tests.Services;

/* Tests for real SerialPortOwnershipCoordinator implementation.
 * All tests verify ownership tracking behavior without real hardware.
 */
public class SerialPortOwnershipCoordinatorTests
{
    private readonly SerialPortOwnershipCoordinator _coordinator;

    public SerialPortOwnershipCoordinatorTests()
    {
        _coordinator = new SerialPortOwnershipCoordinator();
    }

    /* Test 1: GetCurrentOwner_WhenUnowned_ReturnsNone */
    [Fact]
    public void GetCurrentOwner_WhenUnowned_ReturnsNone()
    {
        var owner = _coordinator.GetCurrentOwner("COM1");

        Assert.Equal(SerialPortOwner.None, owner);
    }

    /* Test 2: IsOwned_WhenUnowned_ReturnsFalse */
    [Fact]
    public void IsOwned_WhenUnowned_ReturnsFalse()
    {
        var isOwned = _coordinator.IsOwned("COM1");

        Assert.False(isOwned);
    }

    /* Test 3: TryClaimOwnership_WhenUnowned_ReturnsTrue */
    [Fact]
    public void TryClaimOwnership_WhenUnowned_ReturnsTrue()
    {
        var result = _coordinator.TryClaimOwnership("COM1", SerialPortOwner.Terminal);

        Assert.True(result);
    }

    /* Test 4: TryClaimOwnership_WhenClaimed_SetsOwner */
    [Fact]
    public void TryClaimOwnership_WhenClaimed_SetsOwner()
    {
        _coordinator.TryClaimOwnership("COM1", SerialPortOwner.Terminal);

        var owner = _coordinator.GetCurrentOwner("COM1");

        Assert.Equal(SerialPortOwner.Terminal, owner);
    }

    /* Test 5: TryClaimOwnership_WhenSameOwner_ReturnsTrue */
    [Fact]
    public void TryClaimOwnership_WhenSameOwner_ReturnsTrue()
    {
        _coordinator.TryClaimOwnership("COM1", SerialPortOwner.Terminal);

        var result = _coordinator.TryClaimOwnership("COM1", SerialPortOwner.Terminal);

        Assert.True(result);
    }

    /* Test 6: TryClaimOwnership_WhenDifferentOwner_ReturnsFalse */
    [Fact]
    public void TryClaimOwnership_WhenDifferentOwner_ReturnsFalse()
    {
        _coordinator.TryClaimOwnership("COM1", SerialPortOwner.Terminal);

        var result = _coordinator.TryClaimOwnership("COM1", SerialPortOwner.ModbusRtu);

        Assert.False(result);
    }

    /* Test 7: TryReleaseOwnership_WhenSameOwner_ReturnsTrue */
    [Fact]
    public void TryReleaseOwnership_WhenSameOwner_ReturnsTrue()
    {
        _coordinator.TryClaimOwnership("COM1", SerialPortOwner.Terminal);

        var result = _coordinator.TryReleaseOwnership("COM1", SerialPortOwner.Terminal);

        Assert.True(result);
    }

    /* Test 8: TryReleaseOwnership_WhenDifferentOwner_ReturnsFalse */
    [Fact]
    public void TryReleaseOwnership_WhenDifferentOwner_ReturnsFalse()
    {
        _coordinator.TryClaimOwnership("COM1", SerialPortOwner.Terminal);

        var result = _coordinator.TryReleaseOwnership("COM1", SerialPortOwner.ModbusRtu);

        Assert.False(result);
    }

    /* Test 9: TryReleaseOwnership_WhenUnowned_ReturnsFalse */
    [Fact]
    public void TryReleaseOwnership_WhenUnowned_ReturnsFalse()
    {
        var result = _coordinator.TryReleaseOwnership("COM1", SerialPortOwner.Terminal);

        Assert.False(result);
    }

    /* Test 10: TryReleaseOwnership_RemovesOwnership */
    [Fact]
    public void TryReleaseOwnership_RemovesOwnership()
    {
        _coordinator.TryClaimOwnership("COM1", SerialPortOwner.Terminal);
        _coordinator.TryReleaseOwnership("COM1", SerialPortOwner.Terminal);

        var owner = _coordinator.GetCurrentOwner("COM1");

        Assert.Equal(SerialPortOwner.None, owner);
    }

    /* Test 11: IsOwnedBy_WhenOwnerMatches_ReturnsTrue */
    [Fact]
    public void IsOwnedBy_WhenOwnerMatches_ReturnsTrue()
    {
        _coordinator.TryClaimOwnership("COM1", SerialPortOwner.Terminal);

        var result = _coordinator.IsOwnedBy("COM1", SerialPortOwner.Terminal);

        Assert.True(result);
    }

    /* Test 12: IsOwnedBy_WhenOwnerDoesNotMatch_ReturnsFalse */
    [Fact]
    public void IsOwnedBy_WhenOwnerDoesNotMatch_ReturnsFalse()
    {
        _coordinator.TryClaimOwnership("COM1", SerialPortOwner.Terminal);

        var result = _coordinator.IsOwnedBy("COM1", SerialPortOwner.ModbusRtu);

        Assert.False(result);
    }

    /* Test 13: PortNameComparison_IsCaseInsensitive */
    [Fact]
    public void PortNameComparison_IsCaseInsensitive()
    {
        _coordinator.TryClaimOwnership("com1", SerialPortOwner.Terminal);

        var owner = _coordinator.GetCurrentOwner("COM1");

        Assert.Equal(SerialPortOwner.Terminal, owner);
    }

    /* Test 14: EmptyPortName_ClaimReturnsFalse */
    [Fact]
    public void EmptyPortName_ClaimReturnsFalse()
    {
        var result = _coordinator.TryClaimOwnership("", SerialPortOwner.Terminal);

        Assert.False(result);
    }

    /* Test 15: EmptyPortName_ReleaseReturnsFalse */
    [Fact]
    public void EmptyPortName_ReleaseReturnsFalse()
    {
        var result = _coordinator.TryReleaseOwnership("", SerialPortOwner.Terminal);

        Assert.False(result);
    }

    /* Test 16: OwnershipChanged_RaisedOnSuccessfulClaim */
    [Fact]
    public void OwnershipChanged_RaisedOnSuccessfulClaim()
    {
        SerialPortOwnershipChangedEventArgs? eventArgs = null;
        _coordinator.OwnershipChanged += (sender, e) => eventArgs = e;

        _coordinator.TryClaimOwnership("COM1", SerialPortOwner.Terminal);

        Assert.NotNull(eventArgs);
        Assert.Equal("COM1", eventArgs!.PortName);
        Assert.Equal(SerialPortOwner.None, eventArgs.PreviousOwner);
        Assert.Equal(SerialPortOwner.Terminal, eventArgs.CurrentOwner);
    }

    /* Test 17: OwnershipChanged_RaisedOnSuccessfulRelease */
    [Fact]
    public void OwnershipChanged_RaisedOnSuccessfulRelease()
    {
        _coordinator.TryClaimOwnership("COM1", SerialPortOwner.Terminal);
        SerialPortOwnershipChangedEventArgs? eventArgs = null;
        _coordinator.OwnershipChanged += (sender, e) => eventArgs = e;

        _coordinator.TryReleaseOwnership("COM1", SerialPortOwner.Terminal);

        Assert.NotNull(eventArgs);
        Assert.Equal("COM1", eventArgs!.PortName);
        Assert.Equal(SerialPortOwner.Terminal, eventArgs.PreviousOwner);
        Assert.Equal(SerialPortOwner.None, eventArgs.CurrentOwner);
    }

    /* Test 18: OwnershipChanged_NotRaisedOnFailedClaim */
    [Fact]
    public void OwnershipChanged_NotRaisedOnFailedClaim()
    {
        _coordinator.TryClaimOwnership("COM1", SerialPortOwner.Terminal);
        SerialPortOwnershipChangedEventArgs? eventArgs = null;
        _coordinator.OwnershipChanged += (sender, e) => eventArgs = e;

        _coordinator.TryClaimOwnership("COM1", SerialPortOwner.ModbusRtu);

        Assert.Null(eventArgs);
    }

    /* Test 19: OwnershipChanged_NotRaisedOnFailedRelease */
    [Fact]
    public void OwnershipChanged_NotRaisedOnFailedRelease()
    {
        _coordinator.TryClaimOwnership("COM1", SerialPortOwner.Terminal);
        SerialPortOwnershipChangedEventArgs? eventArgs = null;
        _coordinator.OwnershipChanged += (sender, e) => eventArgs = e;

        _coordinator.TryReleaseOwnership("COM1", SerialPortOwner.ModbusRtu);

        Assert.Null(eventArgs);
    }

    /* Test 20: MultiplePorts_AreTrackedIndependently */
    [Fact]
    public void MultiplePorts_AreTrackedIndependently()
    {
        _coordinator.TryClaimOwnership("COM1", SerialPortOwner.Terminal);
        _coordinator.TryClaimOwnership("COM2", SerialPortOwner.ModbusRtu);

        var owner1 = _coordinator.GetCurrentOwner("COM1");
        var owner2 = _coordinator.GetCurrentOwner("COM2");

        Assert.Equal(SerialPortOwner.Terminal, owner1);
        Assert.Equal(SerialPortOwner.ModbusRtu, owner2);
    }

    /* Test 21: SameOwnerClaim_IsIdempotentAndDoesNotRaiseDuplicateEvent */
    [Fact]
    public void SameOwnerClaim_IsIdempotentAndDoesNotRaiseDuplicateEvent()
    {
        _coordinator.TryClaimOwnership("COM1", SerialPortOwner.Terminal);
        var eventCount = 0;
        _coordinator.OwnershipChanged += (sender, e) => eventCount++;

        _coordinator.TryClaimOwnership("COM1", SerialPortOwner.Terminal);

        Assert.Equal(0, eventCount);
    }

    /* Test 22: NullPortName_ClaimReturnsFalse */
    [Fact]
    public void NullPortName_ClaimReturnsFalse()
    {
        var result = _coordinator.TryClaimOwnership(null!, SerialPortOwner.Terminal);

        Assert.False(result);
    }

    /* Test 23: NullPortName_ReleaseReturnsFalse */
    [Fact]
    public void NullPortName_ReleaseReturnsFalse()
    {
        var result = _coordinator.TryReleaseOwnership(null!, SerialPortOwner.Terminal);

        Assert.False(result);
    }

    /* Test 24: WhitespacePortName_ClaimReturnsFalse */
    [Fact]
    public void WhitespacePortName_ClaimReturnsFalse()
    {
        var result = _coordinator.TryClaimOwnership("   ", SerialPortOwner.Terminal);

        Assert.False(result);
    }

    /* Test 25: ClaimWithNoneOwner_ReturnsFalse */
    [Fact]
    public void ClaimWithNoneOwner_ReturnsFalse()
    {
        var result = _coordinator.TryClaimOwnership("COM1", SerialPortOwner.None);

        Assert.False(result);
    }

    /* Test 26: ReleaseWithNoneOwner_ReturnsFalse */
    [Fact]
    public void ReleaseWithNoneOwner_ReturnsFalse()
    {
        var result = _coordinator.TryReleaseOwnership("COM1", SerialPortOwner.None);

        Assert.False(result);
    }

    /* Test 27: IsOwnedBy_WhenNoneOwner_ReturnsFalse */
    [Fact]
    public void IsOwnedBy_WhenNoneOwner_ReturnsFalse()
    {
        _coordinator.TryClaimOwnership("COM1", SerialPortOwner.Terminal);

        var result = _coordinator.IsOwnedBy("COM1", SerialPortOwner.None);

        Assert.False(result);
    }

    /* Test 28: IsOwnedBy_WhenUnowned_ReturnsFalse */
    [Fact]
    public void IsOwnedBy_WhenUnowned_ReturnsFalse()
    {
        var result = _coordinator.IsOwnedBy("COM1", SerialPortOwner.Terminal);

        Assert.False(result);
    }

    /* Test 29: GetCurrentOwner_WithNullPortName_ReturnsNone */
    [Fact]
    public void GetCurrentOwner_WithNullPortName_ReturnsNone()
    {
        var owner = _coordinator.GetCurrentOwner(null!);

        Assert.Equal(SerialPortOwner.None, owner);
    }

    /* Test 30: IsOwned_WithNullPortName_ReturnsFalse */
    [Fact]
    public void IsOwned_WithNullPortName_ReturnsFalse()
    {
        var result = _coordinator.IsOwned(null!);

        Assert.False(result);
    }

    /* Test 31: IsOwnedBy_WithNullPortName_ReturnsFalse */
    [Fact]
    public void IsOwnedBy_WithNullPortName_ReturnsFalse()
    {
        var result = _coordinator.IsOwnedBy(null!, SerialPortOwner.Terminal);

        Assert.False(result);
    }
}