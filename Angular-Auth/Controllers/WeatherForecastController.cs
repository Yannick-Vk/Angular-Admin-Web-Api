using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Angular_Auth.Controllers;

[Authorize]
[ApiController]
[Route("/api/v1/weather")]
public class WeatherForecastController(ILogger<WeatherForecastController> logger) : ControllerBase {

    [HttpGet("")]
    public IActionResult Get() {
        logger.LogInformation("Weather::Get()");
        return Ok();
    }
}