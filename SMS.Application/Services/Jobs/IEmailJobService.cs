namespace SMS.Application.Services.Jobs
{
    public interface IEmailJobService
    {
        Task SendWelcomeEmailAsync(string toAddress, string subject, string body, bool isBodyHtml = false);
    }
}
