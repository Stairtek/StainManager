using StainManager.WebAPI.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddCors(options =>
{
    options.AddPolicy("Blazor.WebUI.Server",
        policy => { policy.AllowAnyHeader().AllowAnyMethod().WithOrigins("http://localhost:5146"); });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapScalarApiReference();
    app.MapOpenApi();
}

app.UseCors("Blazor.WebUI.Server");

app.UseHttpsRedirection();

app.MapEndpoints();

app.Run();