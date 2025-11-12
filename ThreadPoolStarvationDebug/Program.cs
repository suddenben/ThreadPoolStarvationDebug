using Microsoft.ApplicationInsights.Extensibility.EventCounterCollector;
using ThreadPoolStarvationDebug.Data;

namespace ThreadPoolStarvationDebug
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddHealthChecks();
            builder.Services.ConfigureTelemetryModule<EventCounterCollectionModule>((module, o) =>
            {
                module.Counters.Add(
                    new EventCounterCollectionRequest("System.Runtime", "threadpool-completed-items-count"));
                module.Counters.Add(
                    new EventCounterCollectionRequest("System.Runtime", "threadpool-queue-length"));
                module.Counters.Add(
                    new EventCounterCollectionRequest("System.Runtime", "threadpool-thread-count"));
            });

            builder.Services.AddSingleton<SqlDelayService>();
            builder.Services.AddControllers();
            builder.Services.AddApplicationInsightsTelemetry();

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
