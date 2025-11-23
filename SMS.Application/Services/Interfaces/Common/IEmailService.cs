namespace SMS.Application.Services.Interfaces.Common
{
    public interface IEmailService
    {
        Task<bool> SendEMailAsync(string toAddress, string subject, string body, bool isBodyHtml);
    }
}
