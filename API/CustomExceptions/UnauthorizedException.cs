namespace CustomExceptions
{
    public class UnauthorizedException : CustomException
    {
        public UnauthorizedException()
        {
            this.StatusCode = 401;
            this.ErrorMessage = "Unauthorized";
        }

        public UnauthorizedException(string errorMessage)
        {
            this.StatusCode = 404;
            this.ErrorMessage = errorMessage;
        } 
    }
}