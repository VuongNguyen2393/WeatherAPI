using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using WeatherAPI.Services;

namespace WeatherAPI.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  [EnableRateLimiting("fixedWindow")]
  public class WeatherController(IWeatherService weatherService) : ControllerBase
  {
    private readonly IWeatherService _weatherService = weatherService;
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery][Required] string city)
    {
      var response = await _weatherService.GetWeatherAsync(city);
      return Ok(response);
    }
  }
}