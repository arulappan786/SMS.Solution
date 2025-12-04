using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using SMS.Application.Services.Logging;
using SMS.Domain.Entities.Identity;
using SMS.Infrastructure.Configs;

namespace SMS.Infrastructure.Persistance.Seeders
{
    public class AdminUserSeeder
    {
        public async static Task SeedAdminUserAsync(
            UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IOptions<IdentitySettings> identitySettingsOptions, IAppLogger<AdminUserSeeder> logger)
        {
            var settings = identitySettingsOptions.Value;

            if (string.IsNullOrWhiteSpace(settings.InitialAdminEmail) || string.IsNullOrWhiteSpace(settings.InitialAdminPassword))
            {
                logger.LogWarning("Admin credentials are not configured in IdentitySettings. Skipping Admin user seeding.");
                return;
            }

            // --- 1. Check if the Admin user already exists ---
            var adminUser = await userManager.FindByEmailAsync(settings.InitialAdminEmail);

            if (adminUser == null)
            {
                logger.LogInfo($"Creating default Admin user: {settings.InitialAdminEmail}");

                var newUser = new AppUser
                {
                    UserName = settings.InitialAdminEmail,
                    Email = settings.InitialAdminEmail,
                    EmailConfirmed = true, // Auto-confirm the seeded admin
                    TwoFactorEnabled = false
                };

                // --- 2. Create the User ---
                var createResult = await userManager.CreateAsync(newUser, settings.InitialAdminPassword);

                if (createResult.Succeeded)
                {
                    // --- 3. Assign the Admin Role ---
                    // Ensure the role itself exists before assigning it
                    var adminRole = settings.InitialAdminRoleName; // e.g., "Admin"

                    if (await roleManager.FindByNameAsync(adminRole) != null)
                    {
                        var roleResult = await userManager.AddToRoleAsync(newUser, adminRole);

                        if (roleResult.Succeeded)
                        {
                            logger.LogInfo($"Default Admin user created and assigned role '{adminRole}'.");
                        }
                        else
                        {
                            logger.LogError($"Failed to assign role '{adminRole}' to admin user. Errors: {string.Join(", ", roleResult.Errors.Select(e => e.Description))}");
                        }
                    }
                    else
                    {
                        logger.LogError($"Role '{adminRole}' does not exist. Ensure RoleSeeder runs before AdminUserSeeder.");
                    }
                }
                else
                {
                    logger.LogError($"Failed to create default Admin user. Errors: {string.Join(", ", createResult.Errors.Select(e => e.Description))}");
                }
            }
            else
            {
                logger.LogWarning($"Admin user {settings.InitialAdminEmail} already exists. Skipping creation.");
            }
        }
    }
}