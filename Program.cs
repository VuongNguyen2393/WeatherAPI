using WeatherAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddHttpClient<IWeatherService, WeatherService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapControllers();
app.UseHttpsRedirection();


app.Run();


