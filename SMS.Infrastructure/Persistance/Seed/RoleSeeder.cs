using Microsoft.AspNetCore.Identity;

namespace SMS.Infrastructure.Persistance.Seed
{
    public static class RoleSeeder
    {
        public async static Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            // Define the roles based on your business needs
            string[] roles = { "Admin", "Teacher", "Student", "Parent" };

            foreach (var roleName in roles)
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