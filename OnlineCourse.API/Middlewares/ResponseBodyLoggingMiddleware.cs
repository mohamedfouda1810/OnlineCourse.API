using Microsoft.ApplicationInsights.DataContracts;
using Serilog;

namespace OnlineCourse.API.Middlewares
{
    public class ResponseBodyLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public ResponseBodyLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var originalBodyStream = context.Response.Body;

            try
            {
                using var memoryStream = new MemoryStream();
                context.Response.Body = memoryStream;

                await _next(context);

                memoryStream.Position = 0;
                var reader = new StreamReader(memoryStream);
                var responseBody = await reader.ReadToEndAsync();

                memoryStream.Position = 0;
                await memoryStream.CopyToAsync(originalBodyStream);

                var requestTelemetry = context.Features.Get<RequestTelemetry>();

                if (requestTelemetry != null && !string.IsNullOrWhiteSpace(responseBody))
                {
                    if (!requestTelemetry.Properties.ContainsKey("ResponseBody"))
                    {
                        requestTelemetry.Properties.Add("ResponseBody", responseBody);
                    }

                    Log.Information("Response Body: {ResponseBody}", responseBody);
                }
            }
            finally
            {
                context.Response.Body = originalBodyStream;
            }
        }
    }
}