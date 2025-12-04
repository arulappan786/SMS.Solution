using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using SMS.Application.Extensions;
using SMS.Infrastructure.Configs;
using SMS.Infrastructure.Extensions;
using SMS.Infrastructure.Middlewares;
using SMS.Infrastructure.Persistance.Seeders;
using SMS.WebApi.Filters;

// --------------------------------------------------------------------------------
// I. HOST INITIALIZATION & CONFIGURATION
// --------------------------------------------------------------------------------

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

// --------------------------------------------------------------------------------
// 1. Serilog Logging Setup (Early Initialization)
// --------------------------------------------------------------------------------

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(config)
    .Enrich.FromLogContext()
    .CreateLogger();

Log.Information("Starting application...");
builder.Host.UseSerilog();

// --------------------------------------------------------------------------------
// II. SERVICE REGISTRATION (Dependency Injection)
// --------------------------------------------------------------------------------

Log.Information("Registering Application services...");
builder.Services.AddApplication(config);

Log.Information("Registering Infrastructure services...");
builder.Services.AddInfrastructure(config);

// --- JWT Configuration Setup ---
// 1. Retrieve the JWT Settings from configuration
var jwtSettings = config.GetSection(nameof(JwtSettings)).Get<JwtSettings>();
if (jwtSettings == null)
{
    Log.Fatal("JWT configuration is missing or invalid. Application cannot start.");
    throw new InvalidOperationException("JwtSettings configuration section is missing or invalid.");
}

// 2. Add Authentication Services (JWT Bearer) 🔑
builder.Services.AddAuthentication(options =>
{
    // Set the default scheme to JWT Bearer
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        // CORE VALIDATION CHECKS
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        // APPLY CONFIGURATION VALUES
        ValidIssuer = jwtSettings.ValidIssuer,
        ValidAudience = jwtSettings.ValidAudience,

        // Apply the secret key for signature validation (must be secure)
        IssuerSigningKey = jwtSettings.GetSymmetricSecurityKey(),

        // Reduce the tolerance window for token expiration (good security practice)
        ClockSkew = TimeSpan.Zero
    };
});
// 3. Add Authorization Services (must be added after Authentication)
builder.Services.AddAuthorization();


// Registers MVC controllers as services, enabling them to be used by the application.
builder.Services.AddControllers(options =>
{
    // Add the custom filter to the MVC options
    options.Filters.Add(typeof(ApiExceptionFilterAttribute));
});

// Registers the required services for Swagger/OpenAPI generation.
builder.Services.AddSwaggerGen();

// --- Hangfire Background Job Setup ---
var hangfireConnectionString = builder.Configuration.GetConnectionString("HangfireConnection");

// Registers Hangfire services and configures the SQL Server storage provider.
builder.Services.AddHangfire(configuration => configuration
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
    .UseSqlServerStorage(hangfireConnectionString, new SqlServerStorageOptions
    {
        CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
        SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
        QueuePollInterval = TimeSpan.Zero, // Poll continuously
        UseRecommendedIsolationLevel = true,
        DisableGlobalLocks = true // Optimization for modern SQL Server
    })
);

// Registers the Hangfire background worker process (the component that executes the jobs).
builder.Services.AddHangfireServer();

// --------------------------------------------------------------------------------
// 3. CORS Configuration
// --------------------------------------------------------------------------------

// Retrieves the list of allowed origins (URLs) from the application configuration 
var allowedOrigins = config.GetSection("CorsOrigins").Get<string[]>() ?? Array.Empty<string>();

// Registers the Cross-Origin Resource Sharing (CORS) service.
builder.Services.AddCors(options =>
{
    // Defines a default CORS policy.
    options.AddDefaultPolicy(policy =>
    {
        // Specifies the HTTP origins that are allowed to access the API.
        policy.WithOrigins(allowedOrigins)
            // Allows all HTTP headers in the request.
            .AllowAnyHeader()
            // Allows all HTTP methods (GET, POST, PUT, DELETE, etc.).
            .AllowAnyMethod()
            // Allows credentials (cookies, HTTP authentication) to be sent with cross-origin requests.
            .AllowCredentials();
    });
});

// --------------------------------------------------------------------------------
// III. APPLICATION BUILD AND RUNTIME PIPELINE
// --------------------------------------------------------------------------------

try
{
    Log.Information("Starting web host (Building application)...");

    var app = builder.Build();

    app.UseExceptionHandlingMiddleware();
    app.UseRequestLoggingMiddleware();

    Log.Information("Attempting database seeding...");
    await app.SeedDatabaseAsync();

    // --- Middleware Pipeline ---

    app.UseHangfireDashboard("/hangfire", new DashboardOptions
    {
        // Restrict access to administrators only in a real app!
        // Authorization = new[] { new HangfireAuthorizationFilter() } 
    });

    app.UseCors();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    // 🔑 IMPORTANT: Add UseAuthentication() before UseAuthorization()
    app.UseAuthentication();

    app.UseAuthorization();

    // Maps the controller endpoints (e.g., [Route("api/students")]) to the application's request pipeline.
    app.MapControllers();

    Log.Information("Application starting up and running!");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly during startup.");
}
finally
{
    Log.CloseAndFlush();
}