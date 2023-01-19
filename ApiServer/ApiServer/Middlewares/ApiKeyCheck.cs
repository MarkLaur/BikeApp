using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System.Threading.Tasks;

namespace ApiServer.Middlewares
{
    /// <summary>
    /// Checks if the apiKey value is set correctly in the header.
    /// We don't and wont have personal authentication for this app.
    /// We just check a global apiKey here.
    /// </summary>
    public class ApiKeyCheck
    {
        private readonly RequestDelegate _next;

        private const string APIKEY = "1111";

        public ApiKeyCheck(RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext httpContext)
        {
            //Check if api key value is included in the header
            if(!httpContext.Request.Headers.TryGetValue("apiKey", out StringValues apiKeyHeader))
            {
                httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return httpContext.Response.WriteAsync("apiKey missing");
            }

            //Check if api key matches
            if(apiKeyHeader != APIKEY)
            {
                httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return httpContext.Response.WriteAsync("Unauthorized client");
            }

            //Allow request to continue if all checks passed
            return _next(httpContext);
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ApiKeyCheck>();
        }
    }
}
