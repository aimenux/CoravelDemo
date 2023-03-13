using Coravel;
using Coravel.Invocable;

namespace App;

public static class Program
{
    public static void Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();
        host.ConfigureScheduledTasks();
        host.Run();
    }

    private static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((_, configApp) =>
            {
                configApp.AddCommandLine(args);
            })
            .ConfigureServices((_, services) =>
            {
                services.AddScheduler();
                services.AddTransient<Job>();
            });

    private static void ConfigureScheduledTasks(this IHost host)
    {
        host.Services.UseScheduler(scheduler =>
        {
            scheduler
                .Schedule(SynchronousTask)
                .EverySeconds(10);

            scheduler
                .ScheduleAsync(AsynchronousTask)
                .EverySeconds(30);

            scheduler
                .Schedule<Job>()
                .Cron("* * * * *");

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