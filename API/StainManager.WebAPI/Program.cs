using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var logger = LoggerFactory.Create(loggingBuilder =>
{
    loggingBuilder.AddConsole();
}).CreateLogger("Program");

builder.Services.AddApplication();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    logger.LogInformation("Using connection string: {ConnectionString}", connectionString);
    if (string.IsNullOrEmpty(connectionString))
    {
        throw new InvalidOperationException("Connection string 'DefaultConnection' is not set.");
    }
    options.UseSqlServer("Data Source=awseb-e-gkdkcruri3-stack-awsebrdsdatabase-bihhtneld34v.crrxev2p5657.us-east-2.rds.amazonaws.com,1433; Initial Catalog=StainManager; Integrated Security=false; User ID=admin; Password=!Cakeja5047;");
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
