namespace WeatherAPI.Exceptions
{
  public class WeatherException : System.Exception
  {
    public WeatherException() { }
    public WeatherException(string message) : base(message) { }
    public WeatherException(string message, System.Exception inner) : base(message, inner) { }
  }
}

