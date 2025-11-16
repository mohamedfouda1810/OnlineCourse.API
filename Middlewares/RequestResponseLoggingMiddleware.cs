using Microsoft.ApplicationInsights.DataContracts;
using Serilog;
using System.Text;

namespace OnlineCourse.API.Middlewares
{
    public class RequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestResponseLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Get the request telemetry
            var requestTelemetry = context.Features.Get<RequestTelemetry>();

            // ===== LOG REQUEST =====
            var method = context.Request.Method;
            var path = context.Request.Path;

            Log.Information("Incoming Request: {Method} {Path}", method, path);

            // Read and log request body
            string requestBody = null;
            if (context.Request.Body.CanRead && (method == HttpMethods.Post || method == HttpMethods.Put))
            {
                context.Request.EnableBuffering();

                using var reader = new StreamReader(
                    context.Request.Body,
                    Encoding.UTF8,
                    detectEncodingFromByteOrderMarks: false,
                    bufferSize: 512,
                    leaveOpen: true);

                requestBody = await reader.ReadToEndAsync();
                context.Request.Body.Position = 0;

                if (!string.IsNullOrWhiteSpace(requestBody))
                {
                    Log.Information("Request Body: {RequestBody}", requestBody);
                    requestTelemetry?.Properties.Add("RequestBody", requestBody);
                }
            }

            // ===== CAPTURE RESPONSE =====
            var originalBodyStream = context.Response.Body;

            using var responseBodyStream = new MemoryStream();
            context.Response.Body = responseBodyStream;

            try
            {
                // Execute the rest of the pipeline
                await _next(context);

                // Read response body
                responseBodyStream.Seek(0, SeekOrigin.Begin);
                var responseBody = await new StreamReader(responseBodyStream).ReadToEndAsync();
                responseBodyStream.Seek(0, SeekOrigin.Begin);

                // Log response
                var statusCode = context.Response.StatusCode;
                Log.Information("Response: {StatusCode} - Body: {ResponseBody}", statusCode, responseBody);

                // Add to Application Insights
                if (requestTelemetry != null && !string.IsNullOrWhiteSpace(responseBody))
                {
                    // Check if property already exists before adding
                    if (!requestTelemetry.Properties.ContainsKey("ResponseBody"))
                    {
                        requestTelemetry.Properties.Add("ResponseBody", responseBody);
                    }

                    if (!requestTelemetry.Properties.ContainsKey("ResponseStatusCode"))
                    {
                        requestTelemetry.Properties.Add("ResponseStatusCode", statusCode.ToString());
                    }
                }

                // Copy response back to original stream
                await responseBodyStream.CopyToAsync(originalBodyStream);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error in RequestResponseLoggingMiddleware");

                // Add exception to telemetry
                requestTelemetry?.Properties.Add("MiddlewareException", ex.Message);

                throw;
            }
            finally
            {
                context.Response.Body = originalBodyStream;
            }
        }
    }
}