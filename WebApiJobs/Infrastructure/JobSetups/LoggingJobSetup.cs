using Microsoft.Extensions.Options;
using Quartz;
using WebApiJobs.Jobs;

namespace WebApiJobs.Infrastructure.JobSetups
{
    public class LoggingJobSetup : IConfigureOptions<QuartzOptions>
    {
        public void Configure(QuartzOptions options)
        {
            var interval = TimeSpan.FromSeconds(10);
            //if (interval.TotalMinutes < 1)
            //{
            //    interval = TimeSpan.FromMinutes(1);
            //}

            bool permiteMultiplasExecucoesSimultaneas = false;

            var jobKey = JobKey.Create(nameof(LoggingJob));
            options
                .AddJob<LoggingJob>(jobBuilder =>
                    jobBuilder
                        .WithIdentity(jobKey)
                        .PersistJobDataAfterExecution()
                        .DisallowConcurrentExecution(!permiteMultiplasExecucoesSimultaneas))
                .AddTrigger(trigger =>
                    trigger
                        .ForJob(jobKey).StartNow()
                        .WithSimpleSchedule(schedule =>
                            schedule
                                .WithInterval(interval)
                                .RepeatForever()));
        }
    }
}
