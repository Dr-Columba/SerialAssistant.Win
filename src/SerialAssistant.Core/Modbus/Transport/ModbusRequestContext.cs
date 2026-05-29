namespace SerialAssistant.Core.Modbus.Transport
{
    public sealed class ModbusRequestContext
    {
        private const byte MinUnitId = 1;
        private const byte MaxUnitId = 247;

        public byte UnitId { get; set; }

        public ushort TransactionId { get; set; }

        public byte FunctionCode { get; set; }

        public ushort StartAddress { get; set; }

        public ushort Quantity { get; set; }

        public DateTimeOffset CreatedAt { get; }

        public ModbusRequestContext()
        {
            CreatedAt = DateTimeOffset.UtcNow;
            UnitId = 1;
            TransactionId = 0;
            FunctionCode = 0;
            StartAddress = 0;
            Quantity = 1;
        }

        public bool IsValid()
        {
            if (UnitId < MinUnitId || UnitId > MaxUnitId)
            {
                return false;
            }

            if (FunctionCode == 0)
            {
                return false;
            }

            if (Quantity == 0)
            {
                return false;
            }

            return true;
        }
    }
}
