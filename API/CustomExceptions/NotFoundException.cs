namespace CustomExceptions
{
    public class NotFoundException : CustomException
    {
        public NotFoundException()
        {
            this.StatusCode = 404;
            this.ErrorMessage = "Not Found";
        }

        public NotFoundException(string errorMessage)
        {
            this.StatusCode = 404;
            this.ErrorMessage = errorMessage;
        }
    }
}