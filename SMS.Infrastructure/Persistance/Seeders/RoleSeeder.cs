using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using SMS.Application.Services.Logging;
using SMS.Infrastructure.Configs;

namespace SMS.Infrastructure.Persistance.Seeders
{
    public class RoleSeeder
    {
        public async static Task SeedRolesAsync(
            RoleManager<IdentityRole> roleManager, IOptions<IdentitySettings> identitySettingsOptions,
            IAppLogger<RoleSeeder> logger)
        {
            // Define the roles based on your business needs
            //string[] roles = { "Admin", "Teacher", "Student", "Parent" };
            var initialRoles = identitySettingsOptions.Value.InitialRoles;

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
                        // throw new Exception($"Failed to seed role {roleName}.");
                    }
                }
            }
        }
    }
}