using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using SMS.Application.Services.Logging;
using SMS.Infrastructure.Configs;

namespace SMS.Infrastructure.Persistance.Seeders
{
    public class AppRolesSeeder
    {
        public async static Task SeedRolesAsync(
            RoleManager<IdentityRole> roleManager, IOptions<IdentitySettings> identitySettingsOptions,
            IAppLogger<AppRolesSeeder> logger)
        {
            // Define the roles based on your business needs
            var initialRoles = identitySettingsOptions.Value.InitialRoles;

            if(initialRoles == null || !initialRoles.Any())
            {
                logger.LogWarning("No initial roles defined in IdentitySettings.");
                return;
            }

            foreach (var roleName in initialRoles)
            {
                // Check if the role already exists to prevent duplicates
                if (await roleManager.FindByNameAsync(roleName) == null)
                {
                    // Create the IdentityRole object
                    var role = new IdentityRole(roleName);

                    // Use RoleManager to save the role to the database
                    var result = await roleManager.CreateAsync(role);

                    if (!result.Succeeded)
                    {
                        // Handle errors if role creation fails (e.g., logging)
                        logger.LogError($"Error creating role '{roleName}': {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    }
                }
                else
                {
                    logger.LogInfo($"Role '{roleName}' already exists. Skipping seeding for this role.");
                }
            }
        }
    }
}