using Quartz;
using Quartz.Logging;

using Serilog;

namespace QuartzDemo.AppRestart
{
    public class Program
    {
        public static CancellationTokenSource cancelTokenSource = new();

        public static async Task Main()
        {
            var p = new Program();
            while (true)
            {
                try
                {
                    cancelTokenSource = new CancellationTokenSource();
                    var task = p.LoadApp(cancelTokenSource.Token);
                    await task;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }

        public async Task LoadApp(CancellationToken token)
        {
            var builder = WebApplication.CreateBuilder();

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();

            builder.Host.ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddSerilog(Log.Logger);
            });

            builder.Services.AddQuartz(b =>
            {
                b.InterruptJobsOnShutdown = true;
                b.UseInMemoryStore();
                b.UseSimpleTypeLoader();
                b.SchedulerId = "1";
                b.SchedulerName = "SCHED1";
                b.UseMicrosoftDependencyInjectionJobFactory();
            });

            builder.Services.AddQuartzHostedService(options => { options.WaitForJobsToComplete = true; });

            var app = builder.Build();
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            var loggerFactory = app.Services.GetService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger("foo");


            var schedulerFactory = app.Services.GetService<ISchedulerFactory>();
            if (schedulerFactory != null)
            {
                await SetupScheduler(schedulerFactory);
            }


            app.UseAuthorization();
            app.MapControllers();

            await app.RunAsync(token);
        }

        private static async Task SetupScheduler(ISchedulerFactory schedulerFactory)
        {
            var sched = await schedulerFactory.GetScheduler();

            var job = JobBuilder.Create<SimpleJob>()
                .WithIdentity("job1", "group1")
                .Build();

            var trigger = TriggerBuilder.Create()
                .WithIdentity("trigger1", "group1")
                .WithSimpleSchedule(x => x.WithIntervalInSeconds(5).RepeatForever())
                .StartNow()
                .Build();

            await sched.ScheduleJob(job, trigger);
            await sched.Start();
        }
    }

    public class ConsoleLogProvider : ILogProvider
    {
        public Logger GetLogger(string name)
        {
            return (level, func, exception, parameters) =>
            {
                if (level >= Quartz.Logging.LogLevel.Info && func != null)
                {
                    Console.WriteLine("[" + DateTime.Now.ToLongTimeString() + "] [" + level + "] " + func(), parameters);
                }
                return true;
            };
        }

        public IDisposable OpenNestedContext(string message)
        {
            throw new NotImplementedException();
        }

        public IDisposable OpenMappedContext(string key, object value, bool destructure = false)
        {
            throw new NotImplementedException();
        }
    }
}