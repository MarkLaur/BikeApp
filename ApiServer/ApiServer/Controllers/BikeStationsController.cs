using ApiServer.Models;
using ApiServer.Tools;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ApiServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    //[ApiExplorerSettings(IgnoreApi = true)]
    public class BikeStationsController : ControllerBase
    {
        private readonly ILogger<BikeTripsController> _logger;

        public BikeStationsController(ILogger<BikeTripsController> logger)
        {
            _logger = logger;
        }

        [HttpGet] //Using the HttpGet(name) override makes swagger die as it can't tell the difference between gets and puts
        public ActionResult<IEnumerable<Station>> Get()
        {
            try
            {
                IEnumerable<Station> stations = DatabaseHandler.GetStations();
                return new ActionResult<IEnumerable<Station>>(stations);
            }
            catch (Exception ex)
            {
                //Log goes to the console window and the status code as a response to the client
                _logger.LogError($"Failed to respond to BikeTrips api request. {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPut] //Using the HttpPut(name) override makes swagger die as it can't tell the difference between gets and puts
        public ActionResult Put([FromBody] List<Station> stations)
        {
            try
            {
                //This class should automagically validate the model since the class is decorated with [ApiController]

                //DatabaseHandler handles sanitization
                DatabaseHandler.InsertStations(stations);

                return StatusCode(StatusCodes.Status200OK);
            }
            catch (Exception ex)
            {
                //Log goes to the console window and the status code as a response to the client
                _logger.LogError($"Failed to put bikestations into database. {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}