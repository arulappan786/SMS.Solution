namespace SMS.Application.Services.Logging
{
    public interface IAppLogger<T>
    {
        void LogInfo(string message);

        void LogWarning(string message);

        void LogError(Exception ex, string message);
    }
}
