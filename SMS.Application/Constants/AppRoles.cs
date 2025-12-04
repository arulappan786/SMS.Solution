namespace SMS.Application.Constants
{
    // This static class provides type-safe access to application role names
    public static class AppRoles
    {
        // Used in [Authorize(Roles = AppRoles.Admin)] attributes
        public const string Admin = "Admin";

        // Used when assigning roles during registration or user management
        public const string Teacher = "Teacher";
        public const string Student = "Student";
        public const string Parent = "Parent";
    }
}
