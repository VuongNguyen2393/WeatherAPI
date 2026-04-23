# WeatherAPI

A modern ASP.NET Core 10 REST API that fetches real-time weather information from the Visual Crossing Weather API, with enterprise-grade features including caching, logging, and rate limiting.

## 📋 Table of Contents

- [Features](#features)
- [Tech Stack](#tech-stack)
- [Project Structure](#project-structure)
- [Prerequisites](#prerequisites)
- [Installation & Setup](#installation--setup)
- [Configuration](#configuration)
- [API Documentation](#api-documentation)
- [Features in Detail](#features-in-detail)
  - [Logging](#logging)
  - [Caching](#caching)
  - [Rate Limiting](#rate-limiting)
- [Running the Application](#running-the-application)
- [Testing](#testing)

---

## ✨ Features

- ✅ **Real-time Weather Data**: Fetch weather information from Visual Crossing API
- ✅ **Structured Logging**: Comprehensive logging with Serilog to console and files
- ✅ **In-Memory Caching**: Cache weather data with configurable expiration
- ✅ **Rate Limiting**: Fixed-window rate limiter to prevent API abuse
- ✅ **Exception Handling**: Custom exceptions with detailed error messages
- ✅ **Async/Await**: Fully async operations for better performance
- ✅ **Dependency Injection**: Clean DI pattern following SOLID principles
- ✅ **Configuration Management**: Environment-based configuration support

---

## 🛠 Tech Stack

| Component | Version | Purpose |
|-----------|---------|---------|
| **ASP.NET Core** | 10.0 | Web framework |
| **C#** | Latest | Programming language |
| **Serilog** | 10.0.0 | Structured logging |
| **Serilog.Sinks.File** | 7.0.0 | File logging sink |
| **HttpClient** | Built-in | HTTP requests |
| **IMemoryCache** | Built-in | In-memory caching |
| **Rate Limiter** | Built-in (10.0) | API rate limiting |

### Optional (for Redis caching)

```bash
dotnet add package StackExchange.Redis
dotnet add package Microsoft.Extensions.Caching.StackExchangeRedis
```

---

## 📁 Project Structure

```
WeatherAPI/
├── Controllers/
│   └── WeatherController.cs          # API endpoints
├── Services/
│   ├── IWeatherService.cs            # Service interface
│   └── WeatherService.cs             # Weather data logic
├── Models/
│   └── WeatherInfo.cs                # Data models (WeatherInfo, WeatherResponse, Day)
├── Exceptions/
│   └── WeatherException.cs           # Custom exception
├── Properties/
│   └── launchSettings.json           # Launch configuration
├── Program.cs                        # App configuration & startup
├── appsettings.json                  # Default settings
├── appsettings.Development.json      # Dev environment settings
├── WeatherAPI.csproj                 # Project file
├── WeatherAPI.sln                    # Solution file
├── WeatherAPI.http                   # HTTP test file (VS Code)
├── Logs/                             # Log files (auto-created)
└── bin/, obj/                        # Build artifacts

```

---

## 📦 Prerequisites

- **.NET 10 SDK** or higher
- **Visual Studio Code** or Visual Studio 2024
- **Git** (optional, for version control)
- **Redis** (optional, if using Redis caching instead of in-memory)

---

## 🚀 Installation & Setup

### 1. Clone the Repository (if applicable)

```bash
git clone <repository-url>
cd WeatherAPI
```

### 2. Restore Dependencies

```bash
dotnet restore
```

### 3. Build the Project

```bash
dotnet build
```

### 4. Configure Application

See [Configuration](#configuration) section below.

### 5. Run the Application

```bash
dotnet run
```

The API will start at `https://localhost:5011` (or `http://localhost:5000` in development).

---

## ⚙️ Configuration

### appsettings.json

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

### appsettings.Development.json

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "WeatherApiKey": "YOUR_VISUAL_CROSSING_API_KEY"
}
```

### Getting Your API Key

1. Visit [Visual Crossing Weather API](https://www.visualcrossing.com/weather-api)
2. Sign up for a free account
3. Copy your API key
4. Add it to `appsettings.Development.json` as `WeatherApiKey`

---

## 📡 API Documentation

### Get Weather by City

**Endpoint**
```
GET /api/weather?city={city}
```

**Query Parameters**

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `city` | string | Yes | City name (e.g., "Hanoi", "New York") |

**Example Request**

```bash
curl "http://localhost:5011/api/weather?city=Hanoi"
```

**Example Response (200 OK)**

```json
{
  "latitude": 21.0285,
  "longitude": 105.8542,
  "city": "Hanoi",
  "forecastDate": "2026-04-23",
  "temperature": 28.5,
  "humidity": 65,
  "windSpeed": 4.2,
  "uvIndex": 7.8
}
```

**Error Responses**

| Status | Description |
|--------|-------------|
| 400 | Missing `city` query parameter |
| 429 | Too Many Requests (rate limit exceeded) |
| 500 | Internal server error |

---

## 🎯 Features in Detail

### 📝 Logging

**What's Logged:**
- API requests and responses
- Weather API calls and status codes
- Deserialization success/failure
- Exception details and stack traces
- Cache hits/misses

**Log Levels:**
- `Information`: Normal operation flow
- `Warning`: API returned non-success status code
- `Error`: Exceptions (timeouts, network errors, JSON parsing)

**Log Output:**
- **Console**: Real-time logs in terminal
- **Files**: Daily rolling logs in `Logs/log-YYYY-MM-DD.txt`
  - Keeps 7 most recent files
  - Format: `{Timestamp} [{Level}] {Message}`

**Structured Logging Example:**
```csharp
_logger.LogInformation("Fetching weather for city: {City}", city);
_logger.LogWarning("Weather API return {StatusCode} for city {City}", response.StatusCode, city);
_logger.LogError(ex, "Timeout when retrieve weather for {City}", city);
```

---

### 💾 Caching

**Strategy**: In-memory caching (can switch to Redis)

**Configuration in WeatherService:**
- Cache key: `"{city}".ToLowerInvariant()`
- **Absolute expiration**: 10 minutes from creation
- **Sliding expiration**: 5 minutes of inactivity

**How It Works:**
1. First request for a city → Fetches from API, caches result
2. Subsequent requests within 10 minutes → Returns cached data
3. After 10 minutes → Cache expires, next request fetches fresh data
4. Inactive for 5 minutes → Cache is evicted

**To Switch to Redis:**
```bash
dotnet add package StackExchange.Redis
dotnet add package Microsoft.Extensions.Caching.StackExchangeRedis
```

Update `Program.cs`:
```csharp
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "localhost:6379";
});
```

Update `WeatherService`:
```csharp
public class WeatherService(..., IDistributedCache cache)
{
    private readonly IDistributedCache _cache = cache;
}
```

---

### 🚦 Rate Limiting

**Configuration:**
- **Type**: Fixed-window rate limiter
- **Limit**: 5 requests per 10 seconds
- **Response**: 429 Too Many Requests

**Where It's Applied:**
- Applied to `WeatherController` with `[EnableRateLimiting("fixedWindow")]`

**Testing Rate Limit:**
```bash
# First 5 requests succeed
for i in {1..5}; do curl "http://localhost:5011/api/weather?city=Hanoi"; done

# 6th request fails with 429
curl "http://localhost:5011/api/weather?city=Hanoi"
```

**To Adjust Limits** (in `Program.cs`):
```csharp
builder.Services.AddRateLimiter(option =>
{
    option.AddFixedWindowLimiter("fixedWindow", opt =>
    {
        opt.PermitLimit = 10;           // Change to 10 requests
        opt.Window = TimeSpan.FromMinutes(1);  // Per 1 minute
        opt.QueueLimit = 0;
    });
});
```

---

## 🏃 Running the Application

### Development Mode

```bash
dotnet run
```

Access API at: `https://localhost:5011`

### With Watch Mode (Auto-reload on changes)

```bash
dotnet watch run
```

### Publish (Production)

```bash
dotnet publish -c Release
```

Output goes to `bin/Release/net10.0/publish/`

---

## 🧪 Testing

### Using VS Code REST Client Extension

Create `test.http` file:
```http
### Get weather for Hanoi
GET http://localhost:5011/api/weather?city=Hanoi

### Get weather for New York
GET http://localhost:5011/api/weather?city=New%20York

### Missing city parameter
GET http://localhost:5011/api/weather

### Test rate limiting (consecutive requests)
GET http://localhost:5011/api/weather?city=Tokyo
```

### Using cURL

```bash
# Successful request
curl "http://localhost:5011/api/weather?city=Hanoi"

# Missing parameter
curl "http://localhost:5011/api/weather"

# Test rate limiting
for i in {1..10}; do curl "http://localhost:5011/api/weather?city=Hanoi"; done
```

### Viewing Logs

```bash
# Real-time console logs during `dotnet run`

# View file logs
tail -f Logs/log-*.txt

# Or use VS Code to open a log file
```

---

## 🔧 Troubleshooting

### Issue: "Has no weather api key"

**Solution**: Add `WeatherApiKey` to `appsettings.Development.json`

### Issue: Weather data returns all zeros/nulls

**Solution**: The JSON response uses lowercase property names. Ensure `PropertyNameCaseInsensitive = true` in deserialization or use `[JsonPropertyName]` attributes.

### Issue: Logs not appearing in console

**Solution**: Ensure `dotnet run` is running in the terminal where you started it. Logs go to that terminal's output.

### Issue: Rate limiter not working

**Solution**: Ensure `[EnableRateLimiting("fixedWindow")]` is on the controller and middleware is registered in `Program.cs`.

---

## 📚 Additional Resources

- [ASP.NET Core Documentation](https://learn.microsoft.com/en-us/dotnet/core/introduction)
- [Serilog Documentation](https://serilog.net/)
- [Visual Crossing Weather API](https://www.visualcrossing.com/weather-api)
- [Rate Limiting in .NET 10](https://learn.microsoft.com/en-us/aspnet/core/performance/rate-limit)

---

## 📜 License

This project is provided as-is for educational purposes.

---

## 👤 Author

Created as a personal project to practice ASP.NET Core 10 and enterprise-grade API development patterns.

---

## 🚀 Future Enhancements

- [ ] Add SQL Server database for historical data
- [ ] Implement JWT authentication
- [ ] Add OpenAPI/Swagger documentation
- [ ] Migrate to Redis for distributed caching
- [ ] Add unit tests and integration tests
- [ ] Implement circuit breaker pattern
- [ ] Add API versioning
- [ ] Deploy to Azure/AWS

---

**Last Updated**: April 2026
