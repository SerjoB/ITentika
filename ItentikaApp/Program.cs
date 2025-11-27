using System.Threading.Channels;
using ItentikaApp.Background;
using ItentikaApp.Data;
using ItentikaApp.Models;
using ItentikaApp.Repositories;
using ItentikaApp.Services;
using ItentikaApp.Utilities;
using ItentikaApp.Utilities.Patterns;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// DB
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// CHANNEL
builder.Services.AddSingleton(Channel.CreateUnbounded<Event>());

// SERVICES
builder.Services.AddSingleton<IEventGenerator, EventGeneratorService>();
builder.Services.AddSingleton<IEventProcessor, EventProcessorService>();
builder.Services.AddScoped<IIncidentService, IncidentService>();

// BACKGROUND SERVICES
builder.Services.AddHostedService<EventProcessorBackgroundService>();
builder.Services.AddHostedService<EventGeneratorBackgroundService>();

// REPO
builder.Services.AddScoped<IIncidentRepository, IncidentRepository>();

// PATTERNS
builder.Services.AddSingleton<PatternHandler1>();
builder.Services.AddSingleton<PatternHandler2>();
builder.Services.AddSingleton<PatternHandler3>();
builder.Services.AddSingleton<IIncidentCreatedListener>(sp => sp.GetRequiredService<PatternHandler3>());





// HTTP CLIENT
builder.Services.AddHttpClient("processor", client =>
{
    var baseAddress = builder.Configuration["ProcessorBaseAddress"];
    if (!string.IsNullOrWhiteSpace(baseAddress))
        client.BaseAddress = new Uri(baseAddress);
    else
        client.BaseAddress = new Uri("http://localhost:5000");
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Urls.Add("http://localhost:5000");

app.Run();