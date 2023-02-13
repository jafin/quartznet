using Quartz;

namespace QuartzDemo.AppRestart;

public class SimpleJob : IJob
{
    private readonly ILogger<SimpleJob> _logger;

    public SimpleJob(ILogger<SimpleJob> logger)
    {
        _logger = logger;
    }

    public virtual Task Execute(IJobExecutionContext context)
    {
        // Say Hello to the World and display the date/time
        var timestamp = DateTime.Now;
        Console.WriteLine($"Hello World! - {timestamp:yyyy-MM-dd HH:mm:ss.fff}");
        _logger.LogInformation("Job Fired");
        return Task.CompletedTask;
    }
}