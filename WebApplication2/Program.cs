using Quartz;
using Quartz.Logging;

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
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder
                    .SetMinimumLevel(LogLevel.Debug)
                    .AddSimpleConsole(options =>
                    {
                        options.IncludeScopes = true;
                        options.SingleLine = true;
                        options.TimestampFormat = "hh:mm:ss ";
                    });
            });
            LogProvider.SetLogProvider(loggerFactory);

            var builder = WebApplication.CreateBuilder();

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
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
}