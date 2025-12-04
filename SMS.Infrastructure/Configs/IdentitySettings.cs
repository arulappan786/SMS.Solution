namespace SMS.Infrastructure.Configs
{
    // Class used for binding from appsettings.json
    public class IdentitySettings
    {
        // Static constant to safely reference the configuration section name
        public const string SettingsKey = "IdentitySettings";

        // This property will be populated directly by the configuration binder (IConfiguration.GetSection)
        // If the configuration section is present but empty, this will be an empty List<string>.
        // If the configuration section is entirely missing, this will be null (unless you initialize it here).
        // To be safe against a missing section, initialize to an empty list.
        public List<string> InitialRoles { get; init; } = new List<string>();

        public string InitialAdminEmail { get; set; }

        public string InitialAdminPassword { get; set; }

        public string InitialAdminRoleName { get; set; } = "Admin";
    }
}