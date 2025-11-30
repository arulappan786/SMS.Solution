using Microsoft.Extensions.Logging;
using SMS.Application.Services.Logging;

namespace SMS.Infrastructure.Services.Logging
{
    public class SerilogLoggerAdaptor<T>(ILogger<T> logger) : IAppLogger<T>
    {
        public void LogError(Exception ex, string message) => logger.LogError(ex, message);

        public void LogError(string message) => logger.LogError(message);

        public void LogInfo(string message) => logger.LogInformation(message);

        public void LogWarning(string message) => logger.LogWarning(message);

        public void LogCritical(string message) => logger.LogCritical(message);

        public void LogCritical(Exception ex, string message) => logger.LogCritical(ex, message);

    }
}
