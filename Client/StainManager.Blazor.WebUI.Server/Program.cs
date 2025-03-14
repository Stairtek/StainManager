using Cropper.Blazor.Extensions;
using Microsoft.AspNetCore.Components.Server.Circuits;
using MudBlazor.Services;
using StainManager.Blazor.WebUI.Server;
using StainManager.Blazor.WebUI.Server.Infrastructure;
using StainManager.Blazor.WebUI.Server.Middleware;
using StainManager.Blazor.WebUI.Server.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSentry();

builder.WebHost.UseSentry(options =>
{
    builder.Configuration.GetSection("Sentry").Bind(options);
    
    var environment = builder.Environment.EnvironmentName;
    
    if (string.IsNullOrEmpty(environment))
        environment = "Other";
    
    options.Environment = environment;
    
    options.SetBeforeSend(sentryEvent =>
    {
        sentryEvent.SetTag("app_type", "frontend");
        return sentryEvent;
    });
});

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddCropper();
builder.Services.AddMudServices();
builder.Services.AddScoped<CircuitHandler, SentryCircuitHandler>();
builder.Services.AddScoped<IGlobalExceptionHandler, GlobalExceptionHandler>();
builder.Services.AddScoped<ISentryHandler, SentryHandler>();

builder.Services.AddLogging(logging =>
{
    logging.ClearProviders();
    logging.AddConsole();
    logging.AddDebug();
    logging.AddFilter("Microsoft.AspNetCore.SignalR", LogLevel.Debug);
    logging.AddFilter("Microsoft.AspNetCore.Http.Connections", LogLevel.Debug);
});

builder.Services.AddSignalR(options =>
{
    options.MaximumReceiveMessageSize = 10 * 1024 * 1024; // Set the maximum message size to 10MB
});

builder.Services.AddWebAPIServices();

builder.Services.AddHttpContextAccessor();

builder.Services.AddHttpClient<IStainManagerAPIClient, StainManagerAPIClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiBaseAddress"] ?? string.Empty);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAntiforgery();

app.UseMiddleware<ExceptionMiddleware>();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();