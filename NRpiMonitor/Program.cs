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
builder.Services.AddScoped<DialogService>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<TooltipService>();
builder.Services.AddScoped<ContextMenuService>();

builder.Services.AddDbContextFactory<DataContext>(opt => opt.UseSqlite($"Data Source=app.db"));
builder.Services.AddTransient<PingResultsRepository>();

builder.Services.AddHostedService<PingBackground>();

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
