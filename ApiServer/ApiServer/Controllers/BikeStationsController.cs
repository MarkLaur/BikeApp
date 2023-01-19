using ApiServer.Models;
using ApiServer.Tools;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ApiServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BikeStationsController : ControllerBase
    {
        private readonly ILogger<BikeTripsController> _logger;

        public BikeStationsController(ILogger<BikeTripsController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetBikeStations")]
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
    }
}