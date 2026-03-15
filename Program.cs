using Azure.Identity;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Vindi.Webhook.Infrastructure.Azure;
using Vindi.Webhook.Infrastructure.Middlewares;
using Vindi.Webhook.Models.Managers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddScoped<TenantContext>();
builder.Services.AddSingleton<ServiceBusService>();
builder.Services.AddControllers(options => { options.Conventions.Add(new TenantRoutePrefixConvention()); });

builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy("API está rodando"), tags: new[] { "live" });

// Adiciona o Key Vault como fonte de configuração
builder.Configuration.AddAzureKeyVault(
    new Uri(builder.Configuration["Azure_KeyVault"]),
    new DefaultAzureCredential()
);
builder.Services.AddApplicationInsightsTelemetry(new Microsoft.ApplicationInsights.AspNetCore.Extensions.ApplicationInsightsServiceOptions
{
    ConnectionString = builder.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"]
});


var app = builder.Build();

// Configure the HTTP request pipeline.

app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("live"),
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

// Endpoint de readiness — indica se o app está pronto para receber tráfego
app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready"),
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

// Endpoint completo — todos os checks
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});


app.UseRouting();
app.UseMiddleware<TenantMiddleware>();

app.UseHttpsRedirection();
app.MapControllers();

app.Run();