using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System.Threading.Tasks;

namespace ApiServer.Middlewares
{
    /// <summary>
    /// Checks if the apiKey value is set correctly in the header.
    /// These keys are hardcoded and platform specific and allow us to, for example, kill our android app or old versions of the app.
    /// </summary>
    public class ApiKeyCheck
    {
        //List of defined api keys
        private static readonly ICollection<string> APIKEYS = new HashSet<string>
        {
            "1111", //Web app key
            "2222"  //Android app key
        };

        private readonly RequestDelegate _next;

        public ApiKeyCheck(RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext httpContext)
        {
            //Check if api key value is included in the header
            if (!httpContext.Request.Headers.TryGetValue("apiKey", out StringValues apiKeyHeaders))
            {
                httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return httpContext.Response.WriteAsync("apiKey missing");
            }

            //Check if api key matches
            if (!APIKEYS.Contains(apiKeyHeaders.First()))
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
