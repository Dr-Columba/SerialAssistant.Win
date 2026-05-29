using Xunit;
using SerialAssistant.Core.Modbus.Transport;

namespace SerialAssistant.Tests.Modbus.Transport
{
    public class ModbusRequestContextTests
    {
        [Fact]
        public void DefaultContext_HasCreatedAt()
        {
            var context = new ModbusRequestContext();

            Assert.True(context.CreatedAt > DateTimeOffset.MinValue);
            Assert.True(context.CreatedAt <= DateTimeOffset.UtcNow);
        }

        [Fact]
        public void Validate_ReturnsFalse_WhenUnitIdIsZero()
        {
            var context = new ModbusRequestContext
            {
                UnitId = 0
            };

            Assert.False(context.IsValid());
        }

        [Fact]
        public void Validate_ReturnsFalse_WhenUnitIdGreaterThan247()
        {
            var context = new ModbusRequestContext
            {
                UnitId = 248
            };

            Assert.False(context.IsValid());
        }

        [Fact]
        public void Validate_ReturnsFalse_WhenFunctionCodeIsZero()
        {
            var context = new ModbusRequestContext
            {
                FunctionCode = 0
            };

            Assert.False(context.IsValid());
        }

        [Fact]
        public void Validate_ReturnsFalse_WhenQuantityIsZero()
        {
            var context = new ModbusRequestContext
            {
                Quantity = 0
            };

            Assert.False(context.IsValid());
        }

        [Fact]
        public void Validate_ReturnsTrue_ForValidReadHoldingRegistersContext()
        {
            var context = new ModbusRequestContext
            {
                UnitId = 1,
                FunctionCode = 0x03,
                StartAddress = 0,
                Quantity = 10,
                TransactionId = 1
            };

            Assert.True(context.IsValid());
        }

        [Fact]
        public void Context_DoesNotRequireTransportMode()
        {
            var context = new ModbusRequestContext
            {
                UnitId = 1,
                FunctionCode = 0x03,
                StartAddress = 100,
                Quantity = 5
            };

            Assert.True(context.IsValid());
            Assert.Equal(0, context.TransactionId);
        }

        [Fact]
        public void Validate_ReturnsTrue_WhenUnitIdIsAtBoundary()
        {
            var contextMin = new ModbusRequestContext { UnitId = 1, FunctionCode = 0x03, Quantity = 1 };
            var contextMax = new ModbusRequestContext { UnitId = 247, FunctionCode = 0x03, Quantity = 1 };

            Assert.True(contextMin.IsValid());
            Assert.True(contextMax.IsValid());
        }

        [Fact]
        public void DefaultValues_AreCorrect()
        {
            var context = new ModbusRequestContext();

            Assert.Equal(1, context.UnitId);
            Assert.Equal(0, context.TransactionId);
            Assert.Equal(0, context.FunctionCode);
            Assert.Equal(0, context.StartAddress);
            Assert.Equal(1, context.Quantity);
        }
    }
}
