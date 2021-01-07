using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace IdentityApp.Infrastructure.Middleware
{
    /// <summary>
    /// After successful login the jwt is passed to the client via httpOnly cookie, so there is no way for
    /// the client to send the jwt back to the server when making authorized requests.
    /// HttpAuthMiddleware is solving this problem, pulling out the jwt from httpOnly cookie and setting Authorization header.
    /// With this approach we are secured from cookie compromising (httpOnly cookies are not available via javascript) and able to use jwt authentication.
    /// </summary>
    public class HttpAuthMiddleware
    {
        private readonly RequestDelegate _next;

        public HttpAuthMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var token = context.Request.Cookies[".AspNetCore.Application.Id"];
            if (!string.IsNullOrEmpty(token))
                context.Request.Headers.Add("Authorization", "Bearer " + token);

            await _next.Invoke(context);
        }
    }
}
