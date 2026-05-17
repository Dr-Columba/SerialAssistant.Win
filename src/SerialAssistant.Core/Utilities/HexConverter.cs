using SerialAssistant.Core.Models;

namespace SerialAssistant.Core.Utilities
{
    /*
     * Utility class for converting between byte arrays and HEX strings
     */
    public static class HexConverter
    {
        private static readonly char[] HexChars = "0123456789ABCDEF".ToCharArray();

        public static string ToHexString(byte[] data)
        {
            if (data == null || data.Length == 0)
            {
                return string.Empty;
            }

            char[] hexChars = new char[data.Length * 3 - 1];
            for (int i = 0; i < data.Length; i++)
            {
                int value = data[i];
                hexChars[i * 3] = HexChars[value >> 4];
                hexChars[i * 3 + 1] = HexChars[value & 0x0F];
                if (i < data.Length - 1)
                {
                    hexChars[i * 3 + 2] = ' ';
                }
            }

            return new string(hexChars);
        }

        public static OperationResult<byte[]> FromHexString(string hexString)
        {
            if (string.IsNullOrEmpty(hexString))
            {
                return OperationResult<byte[]>.Success(Array.Empty<byte>());
            }

            string cleanedInput = hexString.Replace(" ", "").Replace("\t", "").Replace("\n", "").Replace("\r", "");

            if (cleanedInput.Length % 2 != 0)
            {
                return OperationResult<byte[]>.Failure("Hex string must have an even number of characters.");
            }

            if (cleanedInput.Length == 0)
            {
                return OperationResult<byte[]>.Success(Array.Empty<byte>());
            }

            byte[] result = new byte[cleanedInput.Length / 2];
            for (int i = 0; i < result.Length; i++)
            {
                string byteString = cleanedInput.Substring(i * 2, 2);
                if (!IsValidHexByte(byteString))
                {
                    return OperationResult<byte[]>.Failure($"Invalid hex character found: '{byteString}'.");
                }

                result[i] = ConvertHexByte(byteString);
            }

            return OperationResult<byte[]>.Success(result);
        }

        private static bool IsValidHexByte(string byteString)
        {
            if (byteString.Length != 2)
            {
                return false;
            }

            char c1 = char.ToUpper(byteString[0]);
            char c2 = char.ToUpper(byteString[1]);

            bool c1Valid = (c1 >= '0' && c1 <= '9') || (c1 >= 'A' && c1 <= 'F');
            bool c2Valid = (c2 >= '0' && c2 <= '9') || (c2 >= 'A' && c2 <= 'F');

            return c1Valid && c2Valid;
        }

        private static byte ConvertHexByte(string byteString)
        {
            char c1 = char.ToUpper(byteString[0]);
            char c2 = char.ToUpper(byteString[1]);

            int highNibble = (c1 >= '0' && c1 <= '9') ? c1 - '0' : c1 - 'A' + 10;
            int lowNibble = (c2 >= '0' && c2 <= '9') ? c2 - '0' : c2 - 'A' + 10;

            return (byte)((highNibble << 4) | lowNibble);
        }
    }
}
