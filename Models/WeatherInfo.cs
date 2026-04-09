using System.Text.Json.Serialization;

namespace WeatherAPI.Models
{
  public class WeatherInfo
  {
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public string? City { get; set; }
    public string? ForecastDate { get; set; }
    public decimal Temperature { get; set; }
    public decimal Humidity { get; set; }
    public decimal WindSpeed { get; set; }
    public decimal UVIndex { get; set; }
  }

  public class WeatherResponse
  {
    [JsonPropertyName("latitude")]
    public decimal Latitude { get; set; }
    [JsonPropertyName("longitude")]
    public decimal Longitude { get; set; }
    [JsonPropertyName("address")]
    public string? Address { get; set; }
    [JsonPropertyName("days")]
    public List<Day>? Days { get; set; }
  }

  public class Day
  {
    [JsonPropertyName("datetime")]
    public string? Datetime { get; set; }
    [JsonPropertyName("temp")]
    public decimal Temp { get; set; }
    [JsonPropertyName("humidity")]
    public decimal Humidity { get; set; }
    [JsonPropertyName("windspeed")]
    public decimal Windspeed { get; set; }
    [JsonPropertyName("uvindex")]
    public decimal Uvindex { get; set; }
  }
}