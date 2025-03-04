using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var logger = LoggerFactory.Create(loggingBuilder =>
{
    loggingBuilder.AddConsole();
}).CreateLogger("Program");

builder.Services.AddApplication();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection") ?? 
                           builder.Configuration.GetConnectionString("DefaultConnection");
    
    if (string.IsNullOrEmpty(connectionString))
    {
        throw new InvalidOperationException("Connection string is not set.");
    }

    var connectionStringLogMessage =
        string.Concat(connectionString.AsSpan(0, Math.Min(20, connectionString.Length)), "...");
    logger.LogInformation("Using connection string: {ConnectionString}", connectionStringLogMessage);
    
    options.UseSqlServer(connectionString);
});

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
