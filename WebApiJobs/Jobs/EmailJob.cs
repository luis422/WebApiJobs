using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Quartz;
using System.Net.Mail;
using WebApiJobs.Data;
using WebApiJobs.Data.Entities;
using WebApiJobs.DevPack;
using WebApiJobs.Services;

namespace WebApiJobs.Jobs
{
    public class EmailJob : IJob
    {
        private readonly ILogger<EmailJob> _logger;
        private readonly IEmailSenderService _emailSenderService;
        private readonly AppDbContext _dbContext;

        public EmailJob(ILogger<EmailJob> logger, AppDbContext dbContext, IEmailSenderService emailSenderService)
        {
            _logger = logger;
            _dbContext = dbContext;
            _emailSenderService = emailSenderService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var dtEndExecution = context.NextFireTimeUtc - TimeSpan.FromSeconds(30);

            int qtd = 0;
            context.JobDetail.JobDataMap.Put("LastRun", DateTimeOffset.Now);
            try
            {
                List<EmailEntity> emails;

                do
                {
                    if (DateTimeOffset.UtcNow >= dtEndExecution || context.CancellationToken.IsCancellationRequested)
                        break;

                    emails = _dbContext.Emails.AsTracking()
                        .Include(e => e.Attachments)
                        .Where(e => e.Status == EEmailStatus.Pending)
                        .OrderBy(e => e.CreatedAt)
                        .Take(100)
                        .ToList();

                    qtd += emails.Count;

                    foreach (var email in emails)
                    {
                        try
                        {
                            var emailAttachments = email.Attachments?
                                .Select(e => new Attachment(new MemoryStream(e.FileBytes), e.FileName, e.FileContentType ?? MimeTypeHelper.GetMimeType(e.FileName)))
                                .ToList();

                            await _emailSenderService.SendEmailAsync(email.Receiver, email.Subject, email.Content, emailAttachments);

                            email.Status = EEmailStatus.Sent;
                            email.SentAt = DateTimeOffset.Now;
                        }
                        catch (Exception ex)
                        {
                            email.Status = EEmailStatus.Fail;
                            email.SentAt = default;
                            _logger.LogError(ex, "Error sending email to {Receiver}", email.Receiver);
                        }
                        finally
                        {
                            await _dbContext.SaveChangesAsync();
                        }
                    }

                } while (emails.Count > 0);
            }
            catch (Exception ex)
            {
                // do you want the job to refire?
                throw new JobExecutionException(msg: ex.Message, refireImmediately: true, cause: ex);
            }
            finally
            {
                context.JobDetail.JobDataMap.Put("LastRunCount", qtd);
                context.JobDetail.JobDataMap.Put("NextRun", context.NextFireTimeUtc?.ToLocalTime()!);
            }
        }
    }
}
