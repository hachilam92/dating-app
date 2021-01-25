namespace CustomExceptions
{
    public class BadRequestException : CustomException
    {
        public BadRequestException()
        {
            this.StatusCode = 400;
            this.ErrorMessage = "This was not a good request";
        } 
        public BadRequestException(string errorMessage)
        {
            this.StatusCode = 400;
            this.ErrorMessage = errorMessage;
        } 
    }
}