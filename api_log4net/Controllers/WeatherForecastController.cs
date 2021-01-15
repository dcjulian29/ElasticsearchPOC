using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace api_log4net.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
            _logger.LogInformation("Creating WeatherForecast Controller.");
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            _logger.LogDebug("Getting Weather...");

            var rng = new Random();
            var forcast = Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();

            foreach (var item in forcast)
            {
                _logger.LogInformation($"For {item.Date}, The temperature will be {item.TemperatureF} F and {item.Summary}");
            }

            return forcast;
        }
    }
}
