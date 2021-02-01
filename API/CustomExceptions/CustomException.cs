using System;

namespace CustomExceptions
{
    public class CustomException : Exception
    {
        public int StatusCode { get; set; }
        public string ErrorMessage { get; set; }

        public CustomException() { }
        public CustomException(string errorMessage)
        {
            this.ErrorMessage = errorMessage;
        }
    }
}