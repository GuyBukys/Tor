using Google.Cloud.Diagnostics.AspNetCore;
using Microsoft.AspNetCore.HttpLogging;
using Swashbuckle.AspNetCore.Filters;
using System.Configuration;
using System.Text.Json.Serialization;
using Tor.Api;
using Tor.Application;
using Tor.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();

string projectId = builder.Configuration.GetValue<string>("ProjectId")!;

if (!builder.Configuration.GetValue("IsTestMode", false))
{
    builder.Services.AddGoogleDiagnosticsForAspNetCore(projectId, Constants.ServiceName, "1.0");
}

builder.Services
    .AddPresentation()
    .AddApplication(builder.Configuration)
    .AddInfrastructure(builder.Configuration);

builder.Services.AddMemoryCache();
builder.Services.AddOutputCache(options =>
{
    options.AddBasePolicy(policy => policy.NoCache());
});

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});
builder.Services.AddControllers()
    .AddJsonOptions(x =>
    {
        x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        x.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    });
builder.Services.AddSwaggerExamplesFromAssemblyOf(typeof(Program));
builder.Services.AddSwaggerGen(c => c.ExampleFilters());

builder.Services.AddHttpLogging(c =>
{
    c.LoggingFields = HttpLoggingFields.All;
    c.CombineLogs = true;
});

var app = builder.Build();

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.MapControllers();
app.UseOutputCache();
app.UseHttpLogging();

await app.UseInfrastructure(app.Configuration);

app.Run();

public partial class Program { }