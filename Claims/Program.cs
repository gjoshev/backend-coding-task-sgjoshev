using Claims.Application.Calculators;
using Claims.Application.Interfaces;
using Claims.Application.Services;
using Claims.Infrastructure.Auditing;
using Claims.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;
using System.Threading.Channels;
using Testcontainers.MongoDb;
using Testcontainers.MsSql;

var builder = WebApplication.CreateBuilder(args);

// Start Testcontainers for SQL Server and MongoDB
var sqlContainer = (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
        ? new MsSqlBuilder()
            .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
        : new()

    ).Build();

var mongoContainer = new MongoDbBuilder()
    .WithImage("mongo:latest")
    .Build();

await sqlContainer.StartAsync();
await mongoContainer.StartAsync();

// Add services to the container.
builder.Services
    .AddControllers()
    .AddJsonOptions(x =>
    {
        x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddDbContext<AuditContext>(options =>
    options.UseSqlServer(sqlContainer.GetConnectionString(),
        sql => sql.MigrationsAssembly(typeof(Program).Assembly.GetName().Name)));

builder.Services.AddDbContext<ClaimsDbContext>(options =>
{
    var client = new MongoClient(mongoContainer.GetConnectionString());
    var database = client.GetDatabase(builder.Configuration["MongoDb:DatabaseName"]);
    options.UseMongoDB(database.Client, database.DatabaseNamespace.DatabaseName);
});

// Register application services
builder.Services.AddSingleton(Channel.CreateUnbounded<object>());
builder.Services.AddSingleton<IAuditService, AuditService>();
builder.Services.AddHostedService<AuditBackgroundService>();
builder.Services.AddScoped<IClaimsRepository, ClaimsRepository>();
builder.Services.AddScoped<ICoversRepository, CoversRepository>();
builder.Services.AddScoped<IClaimsService, ClaimsService>();
builder.Services.AddScoped<ICoversService, CoversService>();
builder.Services.AddSingleton<IPremiumCalculator, PremiumCalculator>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Insurance Claims API",
        Version = "v1",
        Description = "API for managing insurance claims and covers, including premium computation and auditing."
    });

    // Include XML comments from the project for richer Swagger docs
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

var app = builder.Build();

// Swagger is always available for local execution and debugging
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Insurance Claims API v1");
    options.DocumentTitle = "Insurance Claims API - Swagger";
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AuditContext>();
    context.Database.Migrate();
}

app.Run();

public partial class Program { }
