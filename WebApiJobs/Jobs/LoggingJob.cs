using Quartz;

namespace WebApiJobs.Jobs
{
    public class LoggingJob : IJob
    {
        private readonly ILogger<LoggingJob> _logger;

        public LoggingJob(ILogger<LoggingJob> logger)
        {
            _logger = logger;
        }

        public Task Execute(IJobExecutionContext context)
        {
            try
            {
                context.JobDetail.JobDataMap.Put("LastRun", DateTimeOffset.Now);

                _logger.LogInformation("{UtcNow}", DateTimeOffset.Now);
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                // do you want the job to refire?
                throw new JobExecutionException(msg: ex.Message, refireImmediately: true, cause: ex);
            }
        }
    }
}
