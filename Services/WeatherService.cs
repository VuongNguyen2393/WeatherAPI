using System.Runtime.InteropServices;
using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using WeatherAPI.Exceptions;
using WeatherAPI.Models;

namespace WeatherAPI.Services
{
  public class WeatherService(HttpClient httpClient, IConfiguration configuration, ILogger<WeatherService> logger, IMemoryCache cache) : IWeatherService
  {
    private readonly HttpClient _httpClient = httpClient;
    private readonly string _myKey = configuration["WeatherApiKey"] ?? throw new InvalidOperationException("Has no weather api key");
    private readonly IMemoryCache _cache = cache;
    private readonly ILogger<WeatherService> _logger = logger;
    private const string API_BASE = "https://weather.visualcrossing.com/VisualCrossingWebServices/rest/services/timeline";
    public async Task<WeatherInfo> GetWeatherAsync(string city)
    {
      _logger.LogInformation("Fetching weather for city: {City}", city);
      try
      {
        var cacheKey = city.Trim().ToLowerInvariant();
        return await _cache.GetOrCreateAsync(cacheKey, async entry =>
        {
          entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);
          entry.SlidingExpiration = TimeSpan.FromMinutes(5);

          var url = $"{API_BASE}/{city}?key={_myKey}";
          var response = await _httpClient.GetAsync(url);
          if (!response.IsSuccessStatusCode)
          {
            _logger.LogWarning("Weather API return {StatusCode} for city {City}", response.StatusCode, city);
            throw new WeatherException($"Weather API return {response.StatusCode}");
          }
          var content = await response.Content.ReadAsStringAsync();
          var weatherResponse = JsonSerializer.Deserialize<WeatherResponse>(content) ?? throw new WeatherException("No information retrieve");
          _logger.LogInformation("Weather data retrieve successfully for {City}", city);
          return ParseWeatherReponse(weatherResponse);
        }) ?? throw new WeatherException("Weather API can't retrieve data for city");

      }
      catch (TaskCanceledException ex)
      {
        _logger.LogError(ex, "Timeout when retrieve weather for {City}", city);
        throw new WeatherException("Weather API Timeout", ex);
      }
      catch (HttpRequestException ex)
      {
        _logger.LogError(ex, "Network error when calling Weather API when retrieve weather for {City}", city);
        throw new WeatherException("Network error when calling Weather API", ex);
      }
      catch (JsonException ex)
      {
        _logger.LogError(ex, "Fail to deserialize when retrieve weather for {City}", city);
        throw new WeatherException("Fail to deserialize", ex);
      }
    }

    private WeatherInfo ParseWeatherReponse(WeatherResponse response)
    {
      return new WeatherInfo
      {
        Latitude = response.Latitude,
        Longitude = response.Longitude,
        City = response.Address,
        ForecastDate = response.Days?.FirstOrDefault()?.Datetime,
        Temperature = response.Days?.FirstOrDefault()?.Temp ?? 0,
        Humidity = response.Days?.FirstOrDefault()?.Humidity ?? 0,
        WindSpeed = response.Days?.FirstOrDefault()?.Windspeed ?? 0,
        UVIndex = response.Days?.FirstOrDefault()?.Uvindex ?? 0
      };
    }
  }
}