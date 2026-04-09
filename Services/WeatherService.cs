using System.Runtime.InteropServices;
using System.Text.Json;
using WeatherAPI.Exceptions;
using WeatherAPI.Models;

namespace WeatherAPI.Services
{
  public class WeatherService(HttpClient httpClient, IConfiguration configuration) : IWeatherService
  {
    private readonly HttpClient _httpClient = httpClient;
    private readonly string _myKey = configuration["WeatherApiKey"] ?? throw new InvalidOperationException("Has no weather api key");
    private const string API_BASE = "https://weather.visualcrossing.com/VisualCrossingWebServices/rest/services/timeline";
    public async Task<WeatherInfo> GetWeatherAsync(string city)
    {
      try
      {
        var url = $"{API_BASE}/{city}?key={_myKey}";
        var response = await _httpClient.GetAsync(url);
        if (!response.IsSuccessStatusCode)
        {
          throw new WeatherException($"Weather API return {response.StatusCode}");
        }
        var content = await response.Content.ReadAsStringAsync();
        var weatherResponse = JsonSerializer.Deserialize<WeatherResponse>(content) ?? throw new WeatherException("No information retrieve");

        return ParseWeatherReponse(weatherResponse);
      }
      catch (TaskCanceledException ex)
      {
        throw new WeatherException("Weather API Timeout", ex);
      }
      catch (HttpRequestException ex)
      {
        throw new WeatherException("Network error when calling Weather API", ex);
      }
      catch (JsonException ex)
      {
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