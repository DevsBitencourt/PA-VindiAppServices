using Vindi.Webhook.Infrastructure.Azure;
using Vindi.Webhook.Infrastructure.Middlewares;
using Vindi.Webhook.Models.Managers;
using Azure.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddScoped<TenantContext>();
builder.Services.AddSingleton<ServiceBusService>();
builder.Services.AddControllers(options => { options.Conventions.Add(new TenantRoutePrefixConvention()); });


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


app.UseRouting();
app.UseMiddleware<TenantMiddleware>();

app.UseHttpsRedirection();
app.MapControllers();

app.Run();