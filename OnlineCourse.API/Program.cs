using Microsoft.EntityFrameworkCore;
using OnlineCourse.Data;
using OnlineCourse.Service;
using Serilog;
using OnlineCourse.API.Middlewares;
using Microsoft.ApplicationInsights.Extensibility;

namespace OnlineCourse.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                .Enrich.FromLogContext()
                .CreateLogger();

            builder.Host.UseSerilog();

            try
            {
                Log.Information("Starting up the application");

                var configuration = builder.Configuration;

                // Configure Application Insights
                builder.Services.AddApplicationInsightsTelemetry(options =>
                {
                    options.ConnectionString = configuration["ApplicationInsights:ConnectionString"];
                });

                // Add logging providers
                builder.Logging.AddConsole();
                builder.Logging.AddDebug();
                builder.Logging.AddApplicationInsights();

                // Configure Database
                builder.Services.AddDbContext<OnlineCourse.Data.Entities.OnlineCourseDbContext>(options =>
                {
                    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                        providerOptions => providerOptions.EnableRetryOnFailure());
                });

                // Register Controllers
                builder.Services.AddControllers();

                // Register Services
                builder.Services.AddScoped<ICourseCategoryRepository, CourseCategoryRepository>();
                builder.Services.AddScoped<ICourseCategoryService, CourseCategoryService>();
                builder.Services.AddScoped<ICourseService, CourseService>();
                builder.Services.AddScoped<ICourseRepository, CourseRepository>();

                // Swagger
                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen();

                // CORS
                builder.Services.AddCors(options =>
                {
                    options.AddPolicy("AllowAngularApp", policy =>
                    {
                        policy.WithOrigins("http://localhost:4200", "http://localhost:4300")
                              .AllowAnyHeader()
                              .AllowAnyMethod()
                              .AllowCredentials();
                    });
                });

                var app = builder.Build();

                   app.UseSwagger();
                    app.UseSwaggerUI();
            

                app.UseHttpsRedirection();
                app.UseCors("AllowAngularApp");

                // Middleware Pipeline
                app.UseMiddleware<RequestBodyLoggingMiddleware>();
                app.UseMiddleware<ResponseBodyLoggingMiddleware>();
                app.UseMiddleware<RequestResponseLoggingMiddleware>();

                app.UseAuthorization();
                app.MapControllers();

                Log.Information("Application started successfully");
                app.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application start-up failed");
            }
            finally
            {
                Log.Information("Shutting down application and flushing telemetry");

                // Flush Application Insights telemetry
                try
                {
                    var telemetryConfiguration = TelemetryConfiguration.Active;
                    if (telemetryConfiguration != null)
                    {
                        telemetryConfiguration.TelemetryChannel?.Flush();
                        System.Threading.Thread.Sleep(5000); // Wait 5 seconds for telemetry to be sent
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error flushing telemetry: {ex.Message}");
                }

                Log.CloseAndFlush();
            }
        }
    }
}