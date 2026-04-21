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
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddMemoryCache();
builder.Services.AddHttpClient<IWeatherService, WeatherService>();
builder.Host.UseSerilog();

//Build App
var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapControllers();
app.UseHttpsRedirection();


app.Run();


