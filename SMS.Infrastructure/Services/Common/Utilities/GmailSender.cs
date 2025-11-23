using System.Net;
using System.Net.Mail;

namespace SMS.Infrastructure.Services.Common.Utilities
{
    public static class GmailSender
    {
        public static async Task<bool> SendGmailAsync(string toAddress, string subject, string body, bool isBodyHtml = false)
        {
            bool emailSent = false;

            // 1. SMTP Server Details
            const string SmtpHost = "smtp.gmail.com";
            const int SmtpPort = 587;

            // 2. Authentication (MUST use App Password)
            string fromAddress = "rayaarul@gmail.com";
            // *** Replace with your actual 16-character App Password ***
            string appPassword = "rmjwxmycfkcnopbd";

            try
            {
                // 3. Create the Mail Message
                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(fromAddress);
                    mail.To.Add(toAddress);
                    mail.Subject = subject;
                    mail.Body = body;
                    mail.IsBodyHtml = isBodyHtml; // Set to true if sending HTML

                    // 4. Create and Configure the SMTP Client
                    using (SmtpClient smtp = new SmtpClient(SmtpHost, SmtpPort))
                    {
                        smtp.EnableSsl = true;
                        smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                        smtp.UseDefaultCredentials = false;
                        smtp.Credentials = new NetworkCredential(fromAddress, appPassword);

                        // 5. Send the Email
                        // If you are forced to use the two-argument method
                        await smtp.SendMailAsync(mail);
                        emailSent = true;
                        Console.WriteLine("Email notification sent successfully via Gmail!");
                    }
                }
            }
            catch (SmtpException ex)
            {
                emailSent = false;
                // Handle specific SMTP errors (e.g., failed authentication if password is wrong)
                Console.WriteLine($"SMTP Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                emailSent = false;
                Console.WriteLine($"General Error: {ex.Message}");
            }

            return emailSent;
        }
    }
}
