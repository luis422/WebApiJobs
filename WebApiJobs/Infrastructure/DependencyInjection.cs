using Microsoft.AspNetCore.Identity.UI.Services;
using Quartz;
using WebApiJobs.Infrastructure.JobSetups;
using WebApiJobs.Services;

namespace WebApiJobs.Infrastructure
{
    public static class DependencyInjection
    {
        public static void AddInfrastructure(this IServiceCollection services)
        {
            ConfigureEmailSender(services);
            ConfigureJobs(services);
        }

        private static void ConfigureEmailSender(IServiceCollection services)
        {
            services.AddSingleton<IEmailSender>((s) =>
            {
                var configuration = s.GetRequiredService<IConfiguration>();
                var hostType = Enum.Parse<EEmailHostType>(configuration["SMTP:TypeHost"] ?? nameof(EEmailHostType.Custom));
                EmailHost emailHost;
                if (hostType != EEmailHostType.Custom)
                {
                    emailHost = EmailHost.GetFromType(hostType)!;
                }
                else
                {
                    emailHost = new EmailHost(
                        configuration["SMTP:Host"]!,
                        int.Parse(configuration["SMTP:Port"]!),
                        bool.Parse(configuration["SMTP:UseSSL"]!)
                    );
                }
                var credentials = new System.Net.NetworkCredential(
                    configuration["SMTP:UserName"]!,
                    configuration["SMTP:Password"]!
                );
                return new EmailSenderService(emailHost, credentials);
            });
        }

        private static void ConfigureJobs(IServiceCollection services)
        {
            services.AddQuartz(options =>
            {
                options.InterruptJobsOnShutdown = true;
                options.InterruptJobsOnShutdownWithWait = true;
            });
            services.AddQuartzHostedService(options =>
            {
                options.WaitForJobsToComplete = true;
                options.AwaitApplicationStarted = true;
                options.StartDelay = TimeSpan.FromSeconds(10);
            });

            // Adicionando o IScheduler do AddQuartz para injeção nos Controllers.
            services.AddSingleton<IScheduler>(provider => provider.GetRequiredService<ISchedulerFactory>().GetScheduler().GetAwaiter().GetResult());

            services.ConfigureOptions<LoggingJobSetup>();
            services.ConfigureOptions<EmailJobSetup>();
        }
    }
}
