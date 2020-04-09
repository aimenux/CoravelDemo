using System;
using System.Threading.Tasks;
using Coravel;
using Coravel.Invocable;
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
                    services.AddTransient<Job>();
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

                scheduler
                    .Schedule<Job>()
                    .EveryThirtySeconds();

            }).OnError(ex => ConsoleColor.Red.WriteLine(ex));
        }

        private static void SynchronousTask()
        {
            ConsoleColor.Cyan.WriteLine("Running synchronous task.");
        }

        private static Task AsynchronousTask()
        {
            ConsoleColor.Magenta.WriteLine("Running asynchronous task.");
            return Task.CompletedTask;
        }

        private class Job : IInvocable
        {
            public Task Invoke()
            {
                ConsoleColor.Blue.WriteLine("Running invocable job.");
                return Task.CompletedTask;
            }
        }
    }
}
