using System.Text.Json;
using System.Threading.Tasks;
using API.Errors;
using CustomExceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Serilog;

namespace Filters
{
	public class ExceptionFilter : IAsyncExceptionFilter
	{
		public async Task OnExceptionAsync(ExceptionContext context)
		{
			var errorCode = 500;
			var errorMessage = "Internal server error";

			if (context.Exception is CustomException exception)
			{
				errorCode = exception.StatusCode;
				errorMessage = exception.ErrorMessage;
			}
			var response = new ApiException(
				errorCode,
				errorMessage
			);
			
			var options = new JsonSerializerOptions
			{
				PropertyNamingPolicy = JsonNamingPolicy.CamelCase
			};

			Log.Error(errorMessage + context.Exception.StackTrace);

			var json = JsonSerializer.Serialize(response, options);

			context.HttpContext.Response.StatusCode = errorCode;
            await context.HttpContext.Response.WriteAsync(json);

			context.ExceptionHandled = true;
		}

  }
}