using Microsoft.AspNetCore.Identity.UI.Services;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using WebApiJobs.DevPack;

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

    public interface IEmailSenderService : IEmailSender
    {
        public async Task SendEmailAsync(List<string> emailsReceivers, string subject, string contentBody)
            => await SendEmailAsync(emailsReceivers, subject, contentBody, default, default);

        public async Task SendEmailAsync(List<string> emailsReceivers, string subject, string contentBody, List<string>? emailsCC)
            => await SendEmailAsync(emailsReceivers, subject, contentBody, emailsCC, default);

        public async Task SendEmailAsync(List<string> emailsReceivers, string subject, string contentBody, List<Attachment>? attachments)
            => await SendEmailAsync(emailsReceivers, subject, contentBody, default, attachments);

        public async Task SendEmailAsync(string emailReceiver, string subject, string contentBody, List<Attachment>? attachments)
            => await SendEmailAsync([emailReceiver], subject, contentBody, default, attachments);

        Task SendEmailAsync(List<string> emailsReceivers, string subject, string contentBody, List<string>? emailsCC, List<Attachment>? attachments);
    }

    public class EmailSenderService : IEmailSenderService
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

        public async Task SendEmailAsync(string emailReceiver, string subject, string contentBody)
            => await SendEmailAsync([emailReceiver], subject, contentBody, default, default);

        public async Task SendEmailAsync(List<string> emailsReceivers, string subject, string contentBody, List<string>? emailsCC, List<Attachment>? attachments)
        {
            using var mm = new MailMessage(
                from: _credentials.UserName,
                to: string.Join(',', emailsReceivers),
                subject: subject,
                body: contentBody
            )
            {
                IsBodyHtml = true,
            };

            mm.SubjectEncoding = Encoding.UTF8;
            mm.BodyEncoding = Encoding.UTF8;

            if (emailsCC != default)
            {
                foreach (var emailReceiver in emailsCC)
                {
                    mm.CC.Add(emailReceiver);
                }
            }
            try
            {
                if (attachments != null)
                {
                    foreach (var attachment in attachments)
                    {
                        mm.Attachments.Add(attachment);
                    }
                    //foreach (var attachment in attachments)
                    //{
                    //    var bytes = attachment.Value;
                    //    if (bytes == null || bytes.Length == 0)
                    //    {
                    //        continue;
                    //    }

                    //    var fileName = attachment.Key;
                    //    var ms = new MemoryStream(bytes);
                    //    mm.Attachments.Add(new Attachment(ms, fileName, MimeTypeHelper.GetMimeType(fileName)));
                    //}
                }

                using var client = new SmtpClient(_host.Host, _host.Port)
                {
                    EnableSsl = _host.Ssl,
                    Credentials = _credentials,
                };

                await client.SendMailAsync(mm);
            }
            finally
            {
                foreach (var a in mm.Attachments)
                {
                    a.Dispose();
                }
                mm.Attachments.Dispose();
            }
        }
    }
}
