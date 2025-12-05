using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using SMS.Application.Extensions;
using SMS.Infrastructure.Configs;
using SMS.Infrastructure.Extensions;
using SMS.Infrastructure.Middlewares;
using SMS.WebApi.Filters;
using System.Text;

// --------------------------------------------------------------------------------
// I. HOST INITIALIZATION & CONFIGURATION
// --------------------------------------------------------------------------------

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

// --------------------------------------------------------------------------------
// 1. Serilog Logging Setup (Early Initialization)
// --------------------------------------------------------------------------------

Log.Logger = new LoggerConfiguration()
    // Loads logging configuration from appsettings (e.g., sink configuration, minimum levels)
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

// Note: Assuming AddSwaggerAuth is an extension method containing Swagger services.
builder.Services.AddSwaggerAuth();

// --- SECURITY MEASURE: JWT Bearer Authentication Configuration ---
// This registers services to process and validate JWTs sent in the Authorization header.

var jwtSettings = config.GetSection(nameof(JwtSettings)).Get<JwtSettings>();
if (jwtSettings == null)
{
    // SECURITY: Fail fast if the secret key or other critical settings are missing.
    Log.Fatal("JWT configuration is missing or invalid. Application cannot start.");
    throw new InvalidOperationException("JwtSettings configuration section is missing or invalid.");
}

// 2. Add Authentication Services (JWT Bearer) 
builder.Services.AddAuthentication(options =>
{
    // Sets JWT as the primary mechanism for authentication checks.
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        // SECURITY: CRITICAL VALIDATION CHECKS
        ValidateIssuer = true,          // Ensures token came from the correct authority.
        ValidateAudience = true,        // Ensures token is intended for this recipient API.
        ValidateLifetime = true,        // Ensures the token has not expired (short life = better security).
        ValidateIssuerSigningKey = true,// Ensures the token signature is genuine (not tampered with).

        // APPLY CONFIGURATION VALUES
        ValidIssuer = jwtSettings.ValidIssuer,
        ValidAudience = jwtSettings.ValidAudience,

        // Apply the secret key for signature validation.
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),

        // SECURITY: Eliminates clock skew tolerance. Prevents expired tokens from being accepted 
        // even for a short grace period.
        ClockSkew = TimeSpan.Zero
    };
});
// 3. Add Authorization Services (Required for [Authorize] attributes)
builder.Services.AddAuthorization();


// Registers MVC controllers as services.
builder.Services.AddControllers(options =>
{
    // SECURITY: Global custom exception filter to prevent sensitive error details 
    // from being returned to the client (fail securely).
    options.Filters.Add(typeof(ApiExceptionFilterAttribute));
});

// Registers the required services for Swagger/OpenAPI generation.
builder.Services.AddSwaggerGen();

// --- Hangfire Background Job Setup (Not a security measure, kept for context) ---
var hangfireConnectionString = builder.Configuration.GetConnectionString("HangfireConnection");

builder.Services.AddHangfire(configuration => configuration
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
    .UseSqlServerStorage(hangfireConnectionString, new SqlServerStorageOptions
    {
        CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
        SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
        QueuePollInterval = TimeSpan.Zero,
        UseRecommendedIsolationLevel = true,
        DisableGlobalLocks = true
    })
);

builder.Services.AddHangfireServer();

// --------------------------------------------------------------------------------
// 3. SECURITY MEASURE: CORS Configuration
// --------------------------------------------------------------------------------

var allowedOrigins = config.GetSection("CorsOrigins").Get<string[]>() ?? Array.Empty<string>();

// SECURITY: Registers the CORS service to strictly control which external domains 
// are allowed to make requests to the API. Prevents unauthorized domain access.
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(allowedOrigins) // SECURITY: Only allow specified, trusted origins (Not AllowAnyOrigin).
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials(); // Allows cookies/Auth headers to be sent.
    });
});

// --------------------------------------------------------------------------------
// III. APPLICATION BUILD AND RUNTIME PIPELINE
// --------------------------------------------------------------------------------

try
{
    Log.Information("Starting web host (Building application)...");

    var app = builder.Build();

    // --- SECURITY MIDDLEWARE (Early in the pipeline) ---

    // SECURITY: Prevents Clickjacking by instructing browsers not to display the page in an iframe.
    //app.UseXfo(xfo => xfo.Deny());

    // SECURITY: Content Security Policy. Restricts the sources from which content 
    // (scripts, styles, etc.) can be loaded, mitigating XSS risks.
    //app.UseCsp(options => options
    //    .DefaultSources(s => s.Self()) // Only allows resources from the API's origin by default
    //    .ScriptSources(s => s.None())  // Strict policy: No inline or external scripts
    //    .FrameAncestors(s => s.None()) // Modern equivalent of X-Frame-Options: DENY
    //);

    // SECURITY: Custom middleware to catch unhandled exceptions and return safe, generic error responses.
    app.UseExceptionHandlingMiddleware();
    // Log ALL requests/responses for auditing and debugging.
    app.UseRequestLoggingMiddleware();


    Log.Information("Attempting database seeding...");
    // SECURITY: Ensure foundational data (like Identity Roles) are present before launch.
    await app.SeedDatabaseAsync();

    // --- Core Pipeline ---

    app.UseHangfireDashboard("/hangfire", new DashboardOptions
    {
        // CRITICAL SECURITY: Must implement authorization to restrict dashboard access to Admins only!
        // Authorization = new[] { new HangfireAuthorizationFilter() } 
    });

    app.UseCors();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    // SECURITY: HTTP Strict Transport Security (HSTS). Forces clients to use HTTPS 
    // for future connections after the first successful secure visit.
    if (!app.Environment.IsDevelopment())
    {
        app.UseHsts();
    }

    // SECURITY: Redirects all incoming HTTP requests to HTTPS, enforcing encryption.
    app.UseHttpsRedirection();

    // 🔑 SECURITY: Authentication Middleware (Identifies the user from the JWT). Must run first.
    app.UseAuthentication();

    // 🔑 SECURITY: Authorization Middleware (Checks the user's roles/permissions against policy). Must run second.
    app.UseAuthorization();

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