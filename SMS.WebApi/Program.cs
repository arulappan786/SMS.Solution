using Serilog;
using SMS.Application;
using SMS.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration().Enrich.FromLogContext().WriteTo
    .Console().WriteTo
    .File("log/log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();
Log.Logger.Information("Application is building......!");

builder.Services.AddApplication(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);

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