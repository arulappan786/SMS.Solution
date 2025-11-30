namespace SMS.Application.Services.Logging
{
    public interface IAppLogger<T>
    {
        void LogInfo(string message);

        void LogWarning(string message);

        void LogError(string message);

        void LogError(Exception ex, string message);

        /// <summary>
        /// Logs a message at the Critical level.
        /// </summary>
        void LogCritical(string message);

        /// <summary>
        /// Logs an exception and a related message at the Critical level.
        /// </summary>
        void LogCritical(Exception ex, string message);
    }
}
