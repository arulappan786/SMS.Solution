using Microsoft.AspNetCore.Identity;
using Serilog;
using SMS.Application;
using SMS.Application.Services.Implements.Core;
using SMS.Infrastructure;
using SMS.Infrastructure.Persistence.Seed;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

builder.Services.Configure<StudentSettings>(config.GetSection("StudentSettings"));

Log.Logger = new LoggerConfiguration().Enrich.FromLogContext().WriteTo
    .Console().WriteTo
    .File("log/log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

Log.Logger.Information("Application is building......!");

builder.Services.AddApplication(config);
builder.Services.AddInfrastructure(config);

builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(builder =>
{
    builder.AddDefaultPolicy(option =>
    {
        option
        .WithOrigins("https://localhost:7076")
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
    });
});

try
{
    var app = builder.Build();

    using (var scope = app.Services.CreateScope())
    {
        var roleManager = scope.ServiceProvider
            .GetRequiredService<RoleManager<IdentityRole>>();
        await RoleSeeder.SeedRolesAsync(roleManager);
    }

    app.UseCors();

    Log.Logger.Information("Application is built......!");

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    Log.Logger.Information("Application is running......!");

    app.Run();

}
catch (Exception ex)
{

    Log.Logger.Error(ex, "Applicaiton failed to start......!");
}
finally
{
    Log.CloseAndFlush();
}