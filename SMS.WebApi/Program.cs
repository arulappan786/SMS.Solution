using Hangfire;
using Hangfire.SqlServer;
using Serilog;
using SMS.Application;
using SMS.Infrastructure;
using SMS.Infrastructure.Persistence.Seeders;
using SMS.WebApi.Filters;

// --------------------------------------------------------------------------------
// I. HOST INITIALIZATION & CONFIGURATION
// --------------------------------------------------------------------------------

// Initializes a new instance of the WebApplicationBuilder, which is the starting point
// for configuring and hosting the web application. It sets up default host settings.
var builder = WebApplication.CreateBuilder(args);

// Gets the IConfiguration instance, which consolidates configuration sources 
// (appsettings.json, environment variables, command-line arguments, etc.).
var config = builder.Configuration;

// --------------------------------------------------------------------------------
// 1. Serilog Logging Setup (Early Initialization)
// --------------------------------------------------------------------------------

// Assigns the static Log.Logger property to a new LoggerConfiguration.
Log.Logger = new LoggerConfiguration()
    // Instructs Serilog to load all logging settings (sinks, levels) from the 
    // IConfiguration instance, typically found in appsettings.json.
    .ReadFrom.Configuration(config)
    // Adds contextual properties (like correlation IDs or action names) to log events,
    // which helps in tracing complex operations.
    .Enrich.FromLogContext()
    // Finalizes the configuration and creates the static Serilog logger instance.
    .CreateLogger();

// Writes the first log message using the static Serilog logger before the application is fully built.
Log.Information("Starting application...");

// Integrates Serilog with the ASP.NET Core hosting mechanism (builder.Host), 
// ensuring all subsequent framework logging goes through Serilog.
builder.Host.UseSerilog();

// --------------------------------------------------------------------------------
// II. SERVICE REGISTRATION (Dependency Injection)
// --------------------------------------------------------------------------------

// Logs the intent to register services for clarity in the startup logs.
Log.Information("Registering Application services...");

// Extension method (defined in SMS.Application) that registers all services,
// use cases (CQRS), and business logic dependencies for the Application layer.
builder.Services.AddApplication(config);

// Logs the intent to register services for clarity in the startup logs.
Log.Information("Registering Infrastructure services...");

// Extension method (defined in SMS.Infrastructure) that registers all 
// concrete implementations (e.g., DbContext, Repositories, external services)
// for the Infrastructure layer.
builder.Services.AddInfrastructure(config);

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
// (e.g., "CorsOrigins" section in appsettings.json).
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

// Starts a global exception block to catch critical startup failures.
try
{
    // Logs the imminent transition from configuration (builder) to runtime (app).
    Log.Information("Starting web host (Building application)...");

    // Builds the application host, resolving all configured services and creating the WebApplication instance.
    var app = builder.Build();

    // Logs the intent to run the database seeding logic.
    Log.Information("Attempting database seeding...");

    // Calls the idempotent database seeding extension method (from SMS.Infrastructure) 
    // to ensure foundational data (like Identity Roles) exists before running the app.
    await app.SeedDatabaseAsync();

    // --- Middleware Pipeline ---

    // Configures the Hangfire Dashboard UI (Web Interface)
    app.UseHangfireDashboard("/hangfire", new DashboardOptions
    {
        // Restrict access to administrators only in a real app!
        // Authorization = new[] { new HangfireAuthorizationFilter() } 
    });

    // Adds the CORS middleware to the pipeline, applying the default policy defined above.
    app.UseCors();

    // Checks if the application is running in the Development environment.
    if (app.Environment.IsDevelopment())
    {
        // Enables the Swagger JSON document endpoint.
        app.UseSwagger();
        // Enables the Swagger UI page for interactive API documentation.
        app.UseSwaggerUI();
    }

    // Redirects HTTP requests to HTTPS, ensuring secure communication.
    app.UseHttpsRedirection();

    // Adds the authorization middleware, which checks if the user has permission to access a resource.
    app.UseAuthorization();

    // Maps the controller endpoints (e.g., [Route("api/students")]) to the application's request pipeline.
    app.MapControllers();

    // Logs a successful startup confirmation.
    Log.Information("Application starting up and running!");

    // Runs the application, blocking until the app is shut down. This starts listening for requests.
    app.Run();
}
// Catches any unhandled exceptions that occurred during the build or run phase.
catch (Exception ex)
{
    // Logs a fatal error message and the exception details using the static Serilog logger.
    Log.Fatal(ex, "Application terminated unexpectedly during startup.");
}
// Ensures this block executes regardless of whether an exception was thrown.
finally
{
    // Flushes any buffered log entries (especially important for file/database sinks) 
    // and closes the static Serilog logger cleanly before the process exits.
    Log.CloseAndFlush();
}