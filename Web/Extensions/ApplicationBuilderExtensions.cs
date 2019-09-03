using Microsoft.AspNetCore.Builder;
using UsersAccounts.Middlewares;

namespace UsersAccounts.Extensions
{
	public static class ApplicationBuilderExtensions
	{
		public static IApplicationBuilder UseResponseWrapper(this IApplicationBuilder builder)
		{
			return builder.UseMiddleware<ResponseWrapper>();
		}
	}
}