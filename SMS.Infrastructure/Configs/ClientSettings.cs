namespace SMS.Infrastructure.Configs
{
    public class ClientSettings
    {
        public const string SettingsKey = "ClientSettings";

        public required string LoginUrl { get; init; }
    }
}
