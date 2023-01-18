using ApiServer.Models;
using ApiServer.Tools;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ApiServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BikeTripsController : ControllerBase
    {
        private readonly ILogger<BikeTripsController> _logger;

        public BikeTripsController(ILogger<BikeTripsController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetBikeTrips")]
        public ActionResult<IEnumerable<BikeTrip>> Post([FromHeader] string apiKey)
        {
            //TODO: implement a proper api key test
            if(apiKey != "1111")
            {
                return StatusCode(StatusCodes.Status401Unauthorized);
            }

            try
            {
                IEnumerable<BikeTrip> trips = DatabaseHandler.GetTrips();
                return new ActionResult<IEnumerable<BikeTrip>>(trips);
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