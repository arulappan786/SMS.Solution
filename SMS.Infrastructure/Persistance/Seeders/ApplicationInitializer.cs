using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SMS.Infrastructure.Persistance.Seeders; // Assuming RoleSeeder uses domain constants
namespace SMS.Infrastructure.Persistence.Seeders;

public static class ApplicationInitializer
{
    // This is the extension method that runs the seeding logic
    public static async Task SeedDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<WebApplication>>();

        try
        {
            // Seed Identity Roles
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            await RoleSeeder.SeedRolesAsync(roleManager);
            logger.LogInformation("Identity roles seeded successfully.");

            // Optionally, add data seeding here (e.g., student data)
            // var dbContext = services.GetRequiredService<AppDbContext>();
            // await DbSeeder.SeedDataAsync(dbContext);
        }
        catch (Exception ex)
        {
            // Use the standard ILogger here, as your application is now fully running
            logger.LogError(ex, "An error occurred while seeding the database.");
        }
    }
}