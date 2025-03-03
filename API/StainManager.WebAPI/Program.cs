var builder = WebApplication.CreateBuilder(args);

var logger = LoggerFactory.Create(loggingBuilder =>
{
    loggingBuilder.AddConsole();
}).CreateLogger("Program");

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration, logger);
builder.Services.AddWebServices();

var app = builder.Build();

app.MapOpenApi();
app.MapScalarApiReference();

app.UseExceptionHandler(options => { });

app.UseHttpsRedirection();

app.UseRouting();

app.UseAntiforgery();

app.MapControllers();

app.MapEndpoints();

app.Run();