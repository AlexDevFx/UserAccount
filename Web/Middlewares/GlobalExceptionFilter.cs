using System.Collections.Generic;
using System.Net;
using Core.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using UsersAccounts.Models;

namespace UsersAccounts.Middlewares
{
	public class GlobalExceptionFilter: IExceptionFilter
	{
		public void OnException(ExceptionContext context)
		{
			ApiResponse response = new ApiResponse(null, context.Exception.Message, "Error");

			int statusCode = (int)HttpStatusCode.InternalServerError;
			var exception = context.Exception;

			switch (exception)
			{
				case BadRequestException ex:
					statusCode = (int)HttpStatusCode.BadRequest;
					break;
				case NotFoundException ex:
					statusCode = (int)HttpStatusCode.NotFound;
					break;
				default:
					statusCode = (int)HttpStatusCode.InternalServerError;
					break;
			}
			
			context.Result = new ObjectResult(response)
			{
				StatusCode = statusCode,
				DeclaredType = typeof(ApiResponse)
			};
		}
	}
}