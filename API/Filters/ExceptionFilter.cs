using System.Text.Json;
using System.Threading.Tasks;
using API.Errors;
using CustomExceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Filters
{
	public class ExceptionFilter : IAsyncExceptionFilter
	{
		public async Task OnExceptionAsync(ExceptionContext context)
		{
			var response = new ApiException(500, "Internal server error");

			if (context.Exception is CustomException exception)
			{
				response = new ApiException(exception.StatusCode, exception.ErrorMessage);
			}
			
			var options = new JsonSerializerOptions
			{
				PropertyNamingPolicy = JsonNamingPolicy.CamelCase
			};

			var json = JsonSerializer.Serialize(response, options);

            await context.HttpContext.Response.WriteAsync(json);

			context.ExceptionHandled = true;
		}

  }
}