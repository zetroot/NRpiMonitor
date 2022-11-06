using Microsoft.EntityFrameworkCore;
using NRpiMonitor.Database;
using NRpiMonitor.Database.Repositories;
using NRpiMonitor.Services;
using Radzen;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddTransient<PingService>();
builder.Services.AddTransient<SpeedtestService>();

builder.Services.AddScoped<DialogService>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<TooltipService>();
builder.Services.AddScoped<ContextMenuService>();

var connstr = builder.Configuration.GetConnectionString("default");
builder.Services.AddDbContextFactory<DataContext>(opt => opt.UseSqlite(connstr));
builder.Services.AddTransient<PingResultsRepository>();
builder.Services.AddTransient<SpeedTestRepository>();

builder.Services.AddHostedService<PingBackground>();
builder.Services.AddHostedService<SpeedBackground>();

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

app.Run();
