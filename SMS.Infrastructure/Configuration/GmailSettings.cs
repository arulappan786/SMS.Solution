namespace SMS.Infrastructure.Configuration
{
    public class GmailSettings
    {
        public const string SettingsKey = "GmailSettings";

        public required string SmtpHost { get; init; }
        public required int SmtpPort { get; init; }
        public required string FromAddress { get; init; }
        public required string AppPassword { get; init; }
    }
}