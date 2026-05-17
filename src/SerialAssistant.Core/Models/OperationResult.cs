namespace SerialAssistant.Core.Models
{
    /*
     * Represents the result of an operation that may fail
     */
    public class OperationResult
    {
        public bool IsSuccess
        {
            get;
            private set;
        }

        public string ErrorMessage
        {
            get;
            private set;
        } = string.Empty;

        private OperationResult()
        {
        }

        public static OperationResult Success()
        {
            return new OperationResult
            {
                IsSuccess = true,
                ErrorMessage = string.Empty
            };
        }

        public static OperationResult Failure(string errorMessage)
        {
            if (string.IsNullOrWhiteSpace(errorMessage))
            {
                throw new ArgumentException("Error message cannot be null or whitespace.", nameof(errorMessage));
            }

            return new OperationResult
            {
                IsSuccess = false,
                ErrorMessage = errorMessage
            };
        }
    }
}
