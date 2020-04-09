using System;
using System.Threading.Tasks;
using Coravel;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace App
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            host.ConfigureScheduledTasks();
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostContext, configApp) =>
                {
                    configApp.AddCommandLine(args);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddScheduler();
                    services.AddHostedService<Worker>();
                });

        public static void ConfigureScheduledTasks(this IHost host)
        {
            host.Services.UseScheduler(scheduler =>
            {
                scheduler
                    .Schedule(SynchronousTask)
                    .EveryFiveSeconds();

                scheduler
                    .ScheduleAsync(AsynchronousTask)
                    .EveryTenSeconds();
            });
        }

        private static void SynchronousTask()
        {
            ConsoleColor.Cyan.WriteLine("Running synchronous task.");
        }

        private static async Task AsynchronousTask()
        {
            await Task.Delay(TimeSpan.FromSeconds(1));
            ConsoleColor.Magenta.WriteLine("Running asynchronous task.");
        }
    }
}
