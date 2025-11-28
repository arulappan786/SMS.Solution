namespace SMS.Application.Configs
{
    public record StudentSettings
    {
        public const string SettingsKey = "StudentSettings";

        public required string CodePrefix { get; init; }
        public required int CodeLength { get; init; }

    }
}