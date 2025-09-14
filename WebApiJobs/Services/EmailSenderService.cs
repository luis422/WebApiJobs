using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net;
using System.Net.Mail;

namespace WebApiJobs.Services
{
    public enum EEmailHostType
    {
        Custom,
        Gmail,
        Outlook,
        Uol,
        UolMailPro
    }

    public class EmailHost
    {
        public string Host { get; private set; }
        public int Port { get; private set; }
        public bool Ssl { get; private set; }
        public EEmailHostType TypeHost { get; private set; }


        public EmailHost(string host, int port, bool ssl)
        {
            Host = host;
            Port = port;
            Ssl = ssl;
            TypeHost = EEmailHostType.Custom;
        }

        public static EmailHost? GetFromType(EEmailHostType typeHost, EmailHost? coalesce = null)
            => typeHost switch
            {
                EEmailHostType.Gmail => CreateGmail(),
                EEmailHostType.Outlook => CreateOutlook(),
                EEmailHostType.Uol => CreateUol(),
                EEmailHostType.UolMailPro => CreateUolMailPro(),
                _ => coalesce // ?? throw new NotImplementedException($"Email host type '{typeHost}' is not implemented."),
            };

        private static EmailHost CreateGmail() => new(
            host: "smtp.gmail.com",
            port: 587,
            ssl: true
        )
        { TypeHost = EEmailHostType.Gmail };

        private static EmailHost CreateOutlook() => new(
            host: "smtp-mail.outlook.com",
            port: 587,
            ssl: true
        )
        { TypeHost = EEmailHostType.Outlook };

        private static EmailHost CreateUol() => new(
            host: "smtps.uhserver.com",
            port: 465,
            ssl: true
        )
        { TypeHost = EEmailHostType.Uol };

        private static EmailHost CreateUolMailPro(bool ssl = false) => new(
            host: "smtps.uol.com.br",
            port: ssl ? 465 : 587,
            ssl: ssl
        )
        { TypeHost = EEmailHostType.UolMailPro };

    }

    public class EmailSenderService : IEmailSender
    {
        private readonly EmailHost _host;
        private readonly NetworkCredential _credentials;

        public EmailSenderService(string host, int port, bool ssl, string username, string password)
            : this(new EmailHost(host, port, ssl), new NetworkCredential(username, password))
        {
        }
        public EmailSenderService(EmailHost host, NetworkCredential credentials)
        {
            _host = host;
            _credentials = credentials;
        }

        public Task SendEmailAsync(string emailReceiver, string subject, string content)
            => SendEmailAsync([emailReceiver], subject, content);

        public Task SendEmailAsync(List<string> emailReceivers, string subject, string content)
        {
            var client = new SmtpClient(_host.Host, _host.Port)
            {
                EnableSsl = _host.Ssl,
                Credentials = _credentials,
            };

            return client.SendMailAsync(new MailMessage(
                from: _credentials.UserName,
                to: string.Join(',', emailReceivers),
                subject: subject,
                body: content
                ));
        }
    }
}
