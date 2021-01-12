using System;
using System.Text.Json;
using System.Threading.Tasks;
using API.Errors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Filters
{
	public class ExceptionFilter : IAsyncExceptionFilter
	{
		public async Task OnExceptionAsync(ExceptionContext context)
		{
			Console.WriteLine(context.Exception.Message);
			
			var response = new ApiException(context.HttpContext.Response.StatusCode, "Internal server error");
			
			var options = new JsonSerializerOptions
			{
				PropertyNamingPolicy = JsonNamingPolicy.CamelCase
			};

			var json = JsonSerializer.Serialize(response, options);

            await context.HttpContext.Response.WriteAsync(json);
		}

  }
}