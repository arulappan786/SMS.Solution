using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SMS.Application.Services.Logging;
using SMS.Domain.Entities.Identity;
using SMS.Infrastructure.Configs;
using SMS.Infrastructure.Persistance.Seeders;

namespace SMS.Infrastructure.Extensions
{
    public static class AppSeederExtensions
    {
        public static async Task SeedDatabaseAsync(this IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var services = scope.ServiceProvider;

                try
                {
                    // Change the type argument from the static ApplicationInitializer
                    // to a non-static class/interface.
                    var roleSeederLogger = services.GetRequiredService<IAppLogger<AppRolesSeeder>>(); // ✅ FIX APPLIED
                    var adminUserSeederLogger = services.GetRequiredService<IAppLogger<AdminUserSeeder>>(); // ✅ FIX APPLIED

                    // Resolve required dependencies
                    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                    var userManager = services.GetRequiredService<UserManager<AppUser>>();
                    var configuration = services.GetRequiredService<IConfiguration>();
                    var identitySettingsOptions = services.GetRequiredService<IOptions<IdentitySettings>>();

                    roleSeederLogger.LogInfo("Starting database seeding process...");

                    // --- 1. Seed Roles FIRST ---
                    // Note: The RoleSeeder.SeedRolesAsync method also needs to be updated 
                    // to accept ILogger<T> if you want to use the resolved logger inside it.
                    await AppRolesSeeder.SeedRolesAsync(roleManager, identitySettingsOptions, roleSeederLogger);

                    // --- 2. Seed Admin User SECOND ---
                    await AdminUserSeeder.SeedAdminUserAsync(userManager, roleManager, identitySettingsOptions, adminUserSeederLogger);

                    roleSeederLogger.LogInfo("Database seeding completed successfully.");
                }
                catch (Exception ex)
                {
                    var loggerFactory = services.GetRequiredService<ILoggerFactory>();
                    var fatalLogger = loggerFactory.CreateLogger(nameof(AppSeederExtensions));
                    fatalLogger.LogError(ex, "An error occurred while seeding the database.");
                }
            }
        }
    }
}
