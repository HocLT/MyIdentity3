using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using MimeKit;

namespace MyIdentity.Mail
{
    public class MailSettings
    {
        public string? Mail {  get; set; }
        public string? DisplayName { get; set; }
        public string? Password { get; set; }
        public string? Host { get; set; }
        public int Port { get; set; }
    }

    public class SendMailService : IEmailSender
    {
        readonly MailSettings mailSettings;
        public SendMailService(IOptions<MailSettings> mailSettings) 
        {
            this.mailSettings = mailSettings.Value;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var message = new MimeMessage();
            message.Sender = new MailboxAddress(mailSettings.DisplayName, mailSettings.Mail);
            message.From.Add(new MailboxAddress(mailSettings.DisplayName, mailSettings.Mail));
            message.To.Add(MailboxAddress.Parse(email));
            message.Subject = subject;

            var builder = new BodyBuilder();
            builder.HtmlBody = htmlMessage;
            message.Body = builder.ToMessageBody();

            // khai báo stmp client
            using var smtp = new SmtpClient();

            try
            {
                smtp.Connect(mailSettings.Host, mailSettings.Port);
                smtp.Authenticate(mailSettings.Mail, mailSettings.Password);
                await smtp.SendAsync(message);
            }
            catch (Exception ex)
            {
                if (!Directory.Exists("mailssave"))
                {
                    Directory.CreateDirectory("mailssave");
                    var mailFile = $"mailssave/{Guid.NewGuid()}.eml";
                    await message.WriteToAsync(mailFile);
                }
            }
            smtp.Disconnect(true);
        }
    }
}
