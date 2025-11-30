using SMS.Application.Services.Common;
using SMS.Application.Services.Jobs;
using SMS.Application.Services.Logging;

namespace SMS.Infrastructure.Services.Jobs
{
    public class EmailJobService : IEmailJobService
    {
        private readonly IEmailSenderService _emailSenderService;
        private readonly IAppLogger<EmailJobService> _logger;

        // Hangfire requires the job class to be public and its dependencies resolvable by DI.
        public EmailJobService(IEmailSenderService emailSenderService, IAppLogger<EmailJobService> logger)
        {
            _emailSenderService = emailSenderService;
            _logger = logger;
        }

        // This is the method Hangfire serializes and calls later.
        public async Task SendWelcomeEmailAsync(string toAddress, string subject, string body, bool isBodyHtml = false)
        {
            try
            {
                _logger.LogInfo($"Attempting to send welcome email to {toAddress} via Hangfire.");

                var emailResult = await _emailSenderService.SendGmailAsync(toAddress, subject, body, true);

                if (!emailResult)
                {
                    // Throwing an exception tells Hangfire the job FAILED, triggering a retry.
                    throw new Exception($"Email sending failed. The IEmailSenderService returned failure for {toAddress}.");
                }
                _logger.LogInfo($"Successfully sent welcome email to {toAddress}.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Hangfire job failed to send email to {toAddress}. This will be retried.");
                throw; // Re-throw to inform Hangfire of the failure.
            }
        }
    }
}
