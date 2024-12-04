using Serilog;
using Serilog.Context;
using System.Diagnostics;
using System.Security.Claims;

namespace WebApp.Entreo.Middleware
{
    internal class EndpointLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public EndpointLoggingMiddleware(RequestDelegate next)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            var timer = new Stopwatch();
            timer.Start();

            await _next(httpContext);

            timer.Stop();
            var elapsedMilliseconds = timer.Elapsed.TotalMilliseconds;

            try
            {
                LogCompletion(httpContext, elapsedMilliseconds);
            }
            catch
            {
            }
        }

        private void LogCompletion(HttpContext httpContext, double elapsedMilliseconds)
        {
            if (httpContext.Request.Path.HasValue && !httpContext.Request.Path.Value.StartsWith("/api/"))
            {
                return;
            }

            var endPointName = httpContext.GetEndpoint()?.DisplayName;
            var referrer = httpContext.Request.Headers.Referer.FirstOrDefault();
            var ipAddress = httpContext.Connection.RemoteIpAddress?.ToString();
            var statusCode = httpContext.Response.StatusCode;

            LogContext.PushProperty("EndPointName", endPointName);
            if (string.IsNullOrEmpty(ipAddress))
            {
                ipAddress = string.Empty;
            }
            if (ipAddress == "::1")
            {
                ipAddress = "127.0.0.1";
            }
            LogContext.PushProperty("IP", ipAddress);
            LogContext.PushProperty("StatusCode", statusCode);

            var userName = httpContext.User.FindFirst(ClaimTypes.Email)?.Value;
            if (!string.IsNullOrEmpty(userName))
            {
                LogContext.PushProperty("User", userName);
            }

            if (!string.IsNullOrEmpty(referrer))
            {
                LogContext.PushProperty("Referrer", referrer);
            }

            Log.Information("API call {RequestPath} responded in {Elapsed} ms", httpContext.Request.Path, elapsedMilliseconds);
        }

    }
}
