using SMS.Application.Services.Interfaces.Common;
using SMS.Infrastructure.Services.Common.Utilities;

namespace SMS.Infrastructure.Services.Common
{
    public class EmailService : IEmailService
    {
        public async Task<bool> SendEMailAsync(string toAddress, string subject, string body, bool isBodyHtml)
        {
            return await GmailSender.SendGmailAsync(toAddress, subject, body, isBodyHtml);
        }
    }
}