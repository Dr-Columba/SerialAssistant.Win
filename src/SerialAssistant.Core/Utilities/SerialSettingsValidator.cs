using SerialAssistant.Core.Models;

namespace SerialAssistant.Core.Utilities
{
    /*
     * Validator for serial port settings
     */
    public static class SerialSettingsValidator
    {
        private static readonly int[] ValidDataBits = { 5, 6, 7, 8 };
        private static readonly string[] ValidParityValues = { "None", "Odd", "Even", "Mark", "Space" };
        private static readonly string[] ValidStopBitsValues = { "None", "One", "Two", "OnePointFive" };

        public static OperationResult Validate(SerialPortSettings? settings)
        {
            if (settings is null)
            {
                return OperationResult.Failure("Settings cannot be null.");
            }

            if (string.IsNullOrWhiteSpace(settings.PortName))
            {
                return OperationResult.Failure("Port name cannot be empty.");
            }

            if (settings.BaudRate <= 0)
            {
                return OperationResult.Failure("Baud rate must be greater than 0.");
            }

            if (!IsValidDataBits(settings.DataBits))
            {
                return OperationResult.Failure("Data bits must be 5, 6, 7, or 8.");
            }

            if (!IsValidParity(settings.Parity))
            {
                return OperationResult.Failure($"Parity must be one of: {string.Join(", ", ValidParityValues)}.");
            }

            if (!IsValidStopBits(settings.StopBits))
            {
                return OperationResult.Failure($"Stop bits must be one of: {string.Join(", ", ValidStopBitsValues)}.");
            }

            if (settings.ReadTimeout < 0)
            {
                return OperationResult.Failure("Read timeout must be greater than or equal to 0.");
            }

            if (settings.WriteTimeout < 0)
            {
                return OperationResult.Failure("Write timeout must be greater than or equal to 0.");
            }

            return OperationResult.Success();
        }

        private static bool IsValidDataBits(int dataBits)
        {
            foreach (int valid in ValidDataBits)
            {
                if (valid == dataBits)
                {
                    return true;
                }
            }
            return false;
        }

        private static bool IsValidParity(string parity)
        {
            foreach (string valid in ValidParityValues)
            {
                if (valid.Equals(parity, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        private static bool IsValidStopBits(string stopBits)
        {
            foreach (string valid in ValidStopBitsValues)
            {
                if (valid.Equals(stopBits, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
