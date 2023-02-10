using Microsoft.EntityFrameworkCore;
using NRpiMonitor.Database;
using NRpiMonitor.Database.Repositories;
using NRpiMonitor.Services;
using NRpiMonitor.Services.iperf;
using NRpiMonitor.Services.Models;
using Prometheus;
using Radzen;
using Serilog;
using Serilog.Formatting.Compact;
using Serilog.Formatting.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext()
    .WriteTo.Console(new RenderedCompactJsonFormatter())
);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Services.AddScoped<DialogService>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<TooltipService>();
builder.Services.AddScoped<ContextMenuService>();

var connstr = builder.Configuration.GetConnectionString("default");

builder.Services
    .AddDbContextFactory<DataContext>(opt => opt.UseSqlite(connstr))
    .AddTransient<PingResultsRepository>()
    .AddTransient<SpeedTestRepository>()
    .Configure<PingTargets>(builder.Configuration.GetSection(nameof(PingTargets)))
    .AddTransient<PingService>()
    .AddHostedService<PingBackground>()
    .AddTransient<SpeedtestService>()
    .AddTransient<BandwidthService>()
    .AddHostedService<SpeedBackground>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();
app.UseRouting();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");
app.MapMetrics();

Metrics.SuppressDefaultMetrics();
app.Run();
