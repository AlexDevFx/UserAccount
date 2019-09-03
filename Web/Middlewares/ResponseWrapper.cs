using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using UsersAccounts.Models;

namespace UsersAccounts.Middlewares
{
	public class ResponseWrapper
	{
		private readonly RequestDelegate _next;

		public ResponseWrapper(RequestDelegate next)
		{
			_next = next;
		}

		public async Task Invoke(HttpContext context)
		{
			var currentBody = context.Response.Body;

			using (var memoryStream = new MemoryStream())
			{
				//set the current response to the memorystream.
				context.Response.Body = memoryStream;

				await _next(context);

				//reset the body 
				context.Response.Body = currentBody;
				memoryStream.Seek(0, SeekOrigin.Begin);

				var readToEnd = new StreamReader(memoryStream).ReadToEnd();
				var objResult = JsonConvert.DeserializeObject(readToEnd);
				var result = new ApiResponse(objResult);
				await context.Response.WriteAsync(JsonConvert.SerializeObject(result));
			}
		}
	}
}