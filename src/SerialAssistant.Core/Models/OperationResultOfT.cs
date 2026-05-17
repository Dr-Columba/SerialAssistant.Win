namespace SerialAssistant.Core.Models
{
    /*
     * Represents the result of an operation that may fail and return a value
     */
    public class OperationResult<T>
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

        public T? Value
        {
            get;
            private set;
        }

        private OperationResult()
        {
        }

        public static OperationResult<T> Success(T value)
        {
            return new OperationResult<T>
            {
                IsSuccess = true,
                ErrorMessage = string.Empty,
                Value = value
            };
        }

        public static OperationResult<T> Failure(string errorMessage)
        {
            if (string.IsNullOrWhiteSpace(errorMessage))
            {
                throw new ArgumentException("Error message cannot be null or whitespace.", nameof(errorMessage));
            }

            return new OperationResult<T>
            {
                IsSuccess = false,
                ErrorMessage = errorMessage,
                Value = default
            };
        }
    }
}
