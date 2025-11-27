namespace SMS.Application.Services.Interfaces.Common
{
    public interface IEmailSenderService
    {
        Task<bool> SendGmailAsync(string toAddress, string subject, string body, bool isBodyHtml = false);
    }
}
