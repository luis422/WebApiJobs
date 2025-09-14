using Microsoft.Extensions.Options;
using Quartz;
using WebApiJobs.Jobs;

namespace WebApiJobs.Infrastructure.JobSetups
{
    public class EmailJobSetup : IConfigureOptions<QuartzOptions>
    {
        public void Configure(QuartzOptions options)
        {
            var interval = TimeSpan.FromSeconds(60);
            //if (interval.TotalMinutes < 1)
            //{
            //    interval = TimeSpan.FromMinutes(1);
            //}

            bool permiteMultiplasExecucoesSimultaneas = false;

            var jobKey = JobKey.Create(nameof(EmailJob));
            options
                .AddJob<EmailJob>(jobBuilder =>
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
