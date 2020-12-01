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
        /*private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };*/
        
        
        private readonly ILogger<LoraController> _logger;

        public LoraController(ILogger<LoraController> logger)
        {
            _logger = logger;
        }
        /*
        public async Task InsertAsync()
        {
            using var cmd = Db.Connection.CreateCommand();
            cmd.CommandText = @"INSERT INTO `sensors.history` (`devid`, `time`, `lat`, `lon`, `alt`, `rssi`, `snr`, `freq`, `port`, `payload_hex`) VALUES (@devid, @time, @lat, @lon, @alt, @rssi, @snr, @freq, @port, @payload_hex);";
            BindParams(cmd);
            await cmd.ExecuteNonQueryAsync();
            //Id = (int)cmd.LastInsertedId;
        }*/
        // Route / is neccessary to tell controller where is the app that handles request
        [Route("/")]
        [HttpPost]
        // this is optional, tells the controller that requests are formatted as json
        [Consumes("application/json")]
        // Controller watches requests and is triggered by POST, 
        // it then checks http body and stores it in SensorOutput class var
        public IActionResult JsonFromServer([FromBody] SensorOutput jsonBody)
        {
            try
            {
                //DevEUIUplink devroot = new DevEUIUplink();
                //devroot = jsonBody.DevEUI_uplink;
                // Get data from POST body sent by Lora server, only subset defined in the class SensorOutput is imported.
                // This class will be inserted in DB. Class and methods defined in SensorData,cs
                if (MysqlDataContext.GetOneSensor(jsonBody.DevEUI_uplink.devid).devid is null)
                {
                    MysqlDataContext.InsertNewSensorId(jsonBody);
                    MysqlDataContext.InsertSensorHistory(jsonBody);
                }
                else
                {
                    MysqlDataContext.InsertSensorHistory(jsonBody);
                    MysqlDataContext.UpdateSensorTime(jsonBody);
                }
                return Ok();
            }
            catch (Exception exc)
            {
                string err = exc.Message;
                return Ok(err);
            }
        }

        /*
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
