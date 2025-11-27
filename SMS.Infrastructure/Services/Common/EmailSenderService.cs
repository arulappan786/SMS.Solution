using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SMS.Application.Services.Interfaces.Common;
using SMS.Infrastructure.Configuration;
using System.Net;
using System.Net.Mail;

namespace SMS.Infrastructure.Services.Common
{
    public class EmailSenderService(ILogger<EmailSenderService> logger, IOptions<GmailSettings> gmailOptions) : IEmailSenderService
    {
        private readonly GmailSettings _settings = gmailOptions.Value;

        public async Task<bool> SendGmailAsync(string toAddress, string subject, string body, bool isBodyHtml = false)
        {
            bool emailSent = false;

            string SmtpHost = _settings.SmtpHost;
            int SmtpPort = _settings.SmtpPort;
            string fromAddress = _settings.FromAddress;
            string appPassword = _settings.AppPassword;

            try
            {
                // ... (3. Create the Mail Message, 4. Create and Configure the SMTP Client)

                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(fromAddress);
                    mail.To.Add(toAddress);
                    mail.Subject = subject;
                    mail.Body = body;
                    mail.IsBodyHtml = isBodyHtml;

                    using (SmtpClient smtp = new SmtpClient(SmtpHost, SmtpPort))
                    {
                        smtp.EnableSsl = true;
                        smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                        smtp.UseDefaultCredentials = false;
                        smtp.Credentials = new NetworkCredential(fromAddress, appPassword);

                        // 5. Send the Email
                        await smtp.SendMailAsync(mail);
                        emailSent = true;

                        // Use the logger for success messages (LogLevel.Information or Debug)
                        logger.LogInformation($"Email notification sent successfully to {toAddress}.");
                    }
                }
            }
            catch (SmtpException ex)
            {
                emailSent = false;
                logger.LogError(ex, $"SMTP Error sending email to {toAddress}. Status Code: {ex.StatusCode}");
            }
            catch (Exception ex)
            {
                emailSent = false;
                logger.LogError(ex, $"General error occurred while trying to send email to {toAddress}.");     
                return emailSent;
            }

            return emailSent;
        }
    }
}
