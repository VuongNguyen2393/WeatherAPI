using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;
using Serilog;
using WeatherAPI.Services;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File(
    path: "Logs/log-.txt",
    rollingInterval: RollingInterval.Day,
    retainedFileCountLimit: 7
).CreateLogger();


// Add services to the container.
builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddMemoryCache();
builder.Services.AddHttpClient<IWeatherService, WeatherService>();
builder.Host.UseSerilog();
builder.Services.AddRateLimiter(option =>
{
    option.AddFixedWindowLimiter("fixedWindow", opt =>
    {
        opt.PermitLimit = 5;
        opt.Window = TimeSpan.FromSeconds(10);
        opt.QueueLimit = 0;
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
    });

    option.OnRejected = async (context, token) =>
    {
        context.HttpContext.Response.StatusCode = 429;
        await context.HttpContext.Response.WriteAsync("To Much Request");
    };
});

//Build App
var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapControllers();
app.UseHttpsRedirection();
app.UseRateLimiter();

app.Run();


