using Xunit;
using SerialAssistant.Core.Models;

namespace SerialAssistant.Tests.Models
{
    /*
     * Tests for OperationResult and OperationResult<T> classes
     */
    public class OperationResultTests
    {
        /*
         * Test Success result has IsSuccess true
         */
        [Fact]
        public void Success_IsSuccess_ReturnsTrue()
        {
            /* Act */
            var result = OperationResult.Success();

            /* Assert */
            Assert.True(result.IsSuccess);
            Assert.Equal(string.Empty, result.ErrorMessage);
        }

        /*
         * Test Failure result has IsSuccess false
         */
        [Fact]
        public void Failure_IsSuccess_ReturnsFalse()
        {
            /* Act */
            var result = OperationResult.Failure("Test error message");

            /* Assert */
            Assert.False(result.IsSuccess);
            Assert.Equal("Test error message", result.ErrorMessage);
        }

        /*
         * Test Failure with whitespace error message throws
         */
        [Fact]
        public void Failure_WhitespaceErrorMessage_ThrowsArgumentException()
        {
            /* Act & Assert */
            Assert.Throws<ArgumentException>(() => OperationResult.Failure("   "));
        }

        /*
         * Test Failure with null error message throws
         */
        [Fact]
        public void Failure_NullErrorMessage_ThrowsArgumentException()
        {
            /* Act & Assert */
            Assert.Throws<ArgumentException>(() => OperationResult.Failure(null!));
        }

        /*
         * Test OperationResult<T> Success has correct value
         */
        [Fact]
        public void OperationResultOfT_Success_HasCorrectValue()
        {
            /* Act */
            var result = OperationResult<int>.Success(42);

            /* Assert */
            Assert.True(result.IsSuccess);
            Assert.Equal(42, result.Value);
            Assert.Equal(string.Empty, result.ErrorMessage);
        }

        /*
         * Test OperationResult<T> Failure has correct error message
         */
        [Fact]
        public void OperationResultOfT_Failure_HasCorrectErrorMessage()
        {
            /* Act */
            var result = OperationResult<int>.Failure("Error occurred");

            /* Assert */
            Assert.False(result.IsSuccess);
            Assert.Equal("Error occurred", result.ErrorMessage);
            Assert.Equal(0, result.Value);
        }

        /*
         * Test OperationResult<T> Success with string value
         */
        [Fact]
        public void OperationResultOfT_SuccessWithString_HasCorrectValue()
        {
            /* Act */
            var result = OperationResult<string>.Success("test");

            /* Assert */
            Assert.True(result.IsSuccess);
            Assert.Equal("test", result.Value);
        }

        /*
         * Test OperationResult<T> Success with object value
         */
        [Fact]
        public void OperationResultOfT_SuccessWithObject_HasCorrectValue()
        {
            /* Arrange */
            var obj = new SerialPortSettings { PortName = "COM1", BaudRate = 9600 };

            /* Act */
            var result = OperationResult<SerialPortSettings>.Success(obj);

            /* Assert */
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal("COM1", result.Value.PortName);
            Assert.Equal(9600, result.Value.BaudRate);
        }

        /*
         * Test OperationResult<T> Failure with whitespace throws
         */
        [Fact]
        public void OperationResultOfT_FailureWhitespaceErrorMessage_ThrowsArgumentException()
        {
            /* Act & Assert */
            Assert.Throws<ArgumentException>(() => OperationResult<int>.Failure(""));
        }

        /*
         * Test OperationResult<T> Success with empty array
         */
        [Fact]
        public void OperationResultOfT_SuccessWithEmptyArray_HasCorrectValue()
        {
            /* Act */
            var result = OperationResult<byte[]>.Success(Array.Empty<byte>());

            /* Assert */
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Empty(result.Value);
        }
    }
}
