using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Quartz;
using WebApiJobs.Data;
using WebApiJobs.Data.Entities;

namespace WebApiJobs.Jobs
{
    public class EmailJob : IJob
    {
        private readonly ILogger<EmailJob> _logger;

        private readonly IEmailSender _emailSenderService;
        private readonly AppDbContext _dbContext;

        public EmailJob(ILogger<EmailJob> logger, AppDbContext dbContext, IEmailSender emailSenderService)
        {
            _logger = logger;
            _dbContext = dbContext;
            _emailSenderService = emailSenderService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                List<EmailEntity> emails;

                do
                {
                    emails = _dbContext.Emails.AsTracking()
                        .Where(e => e.Status == EEmailStatus.Pending)
                        .OrderBy(e => e.CreatedAt)
                        .Take(100)
                        .ToList();

                    foreach (var e in emails)
                    {
                        try
                        {
                            await _emailSenderService.SendEmailAsync(e.Receiver, e.Subject, e.Content);

                            e.Status = EEmailStatus.Sent;
                            e.SentAt = DateTime.UtcNow;
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error sending email to {Receiver}", e.Receiver);
                        }
                    }

                    if (emails.Count > 0)
                        _dbContext.SaveChanges();

                } while (emails.Count > 0);
            }
            catch (Exception ex)
            {
                // do you want the job to refire?
                throw new JobExecutionException(msg: ex.Message, refireImmediately: true, cause: ex);
            }
        }
    }
}
