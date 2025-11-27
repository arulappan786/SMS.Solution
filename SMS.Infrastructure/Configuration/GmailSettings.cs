namespace SMS.Infrastructure.Configuration
{
    public class GmailSettings
    {
        // Must match the property names in appsettings.json
        public const string SettingsKey = "GmailSettings";

        public string SmtpHost { get; set; } = string.Empty;
        public int SmtpPort { get; set; }
        public string FromAddress { get; set; } = string.Empty;
        public string AppPassword { get; set; } = string.Empty;
    }
}