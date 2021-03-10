using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.Json.Serialization;
using MySqlConnector;
using static WebAppJson.Models.SensorData;
using static WebAppJson.Models.SensorData.MysqlDataContext;

namespace WebAppJson.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LoraController : ControllerBase
    {
		// Reused Weather mockup defined in original Blazor sample and added new controler
        /*private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };*/
        
        
        private readonly ILogger<LoraController> _logger;

        public LoraController(ILogger<LoraController> logger)
        {
            _logger = logger;
        }
        
        // Route / is neccessary to tell controller where is the app that handles request
        [Route("/")]
        [HttpPost]
        // this is optional, tells the controller that requests are formatted as json
        [Consumes("application/json")]
        // Controller watches requests and is triggered by POST, 
        // it then checks http body and stores it in SensorOutput class var
        
        //public IActionResult JsonFromServer([FromBody] SensorOutput jsonBody)
        public IActionResult JsonFromServer([FromBody] JsonString jstr)
        {
            try
            {
				// Read JSON data from POST body and stores it into the JsonStr object
                string retstr = jstr.ToString();
                return Ok();
            }
            catch (Exception exc)
            {
                string err = exc.Message;
                return Ok(err);
            }
        }

        /* This is leftover from original sample, commented since GET is not used for this WebAPI
        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }*/
    }
}
