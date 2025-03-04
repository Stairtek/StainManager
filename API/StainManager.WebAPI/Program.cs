using StainManager.WebAPI.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Enhanced logging configuration
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.SetMinimumLevel(LogLevel.Information);

// Add AWS logging if in production
builder.Logging.AddAWSProvider(builder.Configuration.GetAWSLoggingConfigSection());

var logger = LoggerFactory.Create(loggingBuilder =>
{
    loggingBuilder.AddConsole();
    loggingBuilder.SetMinimumLevel(LogLevel.Information);
}).CreateLogger("Program");

logger.LogInformation("Application starting up. Environment: {Environment}", 
    Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production");

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration, logger);
builder.Services.AddWebServices();

var app = builder.Build();

app.UseGlobalExceptionHandler();
app.UseRequestLogging();

app.MapOpenApi();
app.MapScalarApiReference();

app.UseHttpsRedirection();
app.UseRouting();
app.UseAntiforgery();
app.MapControllers();
app.MapEndpoints();

app.Run();
