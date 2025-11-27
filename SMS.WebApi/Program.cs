using Microsoft.AspNetCore.Identity;
using Serilog;
using SMS.Application;
using SMS.Infrastructure;
using SMS.Infrastructure.Persistence.Seed;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

// 1. Serilog Setup (Reading from configuration is best practice)
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(config) // Reads file sink, console, levels, etc., from appsettings.json
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog();

// --- Configuration is moved to AddApplication/AddInfrastructure ---

builder.Services.AddApplication(config);
builder.Services.AddInfrastructure(config);

builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

// 2. CORS Setup (Reading origins from configuration)
var allowedOrigins = config.GetSection("CorsOrigins").Get<string[]>() ?? Array.Empty<string>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

try
{
    Log.Information("Starting web host (Building application)...");
    var app = builder.Build();

    // 3. Data Seeding is called immediately after build
    await SeedDatabaseAsync(app);

    // --- Middleware Pipeline ---

    app.UseCors();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();
    app.UseAuthorization();
    app.MapControllers();

    Log.Information("Application starting up and running!");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Applicaiton terminated unexpectedly during startup.");
}
finally
{
    Log.CloseAndFlush();
}

// 4. Seeding Helper Function
async Task SeedDatabaseAsync(WebApplication application)
{
    using var scope = application.Services.CreateScope();
    try
    {
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        await RoleSeeder.SeedRolesAsync(roleManager);
        Log.Information("Identity roles seeded successfully.");
    }
    catch (Exception ex)
    {
        Log.Error(ex, "An error occurred while seeding roles.");
    }
}