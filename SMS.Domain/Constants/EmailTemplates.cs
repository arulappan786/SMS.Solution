namespace SMS.Domain.Constants
{
    public static class EmailTemplates
    {
        // Recommended approach for multi-assembly safety
        public static readonly string WelcomeUserTemplate = "WelcomeUserTemplate.html";

        // Change this one to match:
        public static readonly string PasswordResetTemplate = "PasswordResetTemplate.html";
    }
}
