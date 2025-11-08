using Microsoft.ApplicationInsights.DataContracts;
using Serilog;
using System.Text;

namespace OnlineCourse.API.Middlewares
{
    public class RequestBodyLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestBodyLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var method = context.Request.Method;

            context.Request.EnableBuffering();

            if (context.Request.Body.CanRead && (method == HttpMethods.Post || method == HttpMethods.Put))
            {
                using var reader = new StreamReader(
                    context.Request.Body,
                    Encoding.UTF8,
                    detectEncodingFromByteOrderMarks: false,
                    bufferSize: 512,
                    leaveOpen: true);

                var requestBody = await reader.ReadToEndAsync();
                context.Request.Body.Position = 0;

                var requestTelemetry = context.Features.Get<RequestTelemetry>();

                if (!string.IsNullOrWhiteSpace(requestBody) && requestTelemetry != null)
                {
                    if (!requestTelemetry.Properties.ContainsKey("RequestBody"))
                    {
                        requestTelemetry.Properties.Add("RequestBody", requestBody);
                    }
                    Log.Information("Request Body: {RequestBody}", requestBody);
                }
            }

            await _next(context);
        }
    }
}
