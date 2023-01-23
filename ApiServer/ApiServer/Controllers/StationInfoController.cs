using ApiServer.Models;
using ApiServer.Tools;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ApiServer.Controllers
{
    [ApiController]
    [Route("[controller]/{stationID}")]
    public class StationInfoController : ControllerBase
    {
        private readonly ILogger<BikeTripsController> _logger;

        public StationInfoController(ILogger<BikeTripsController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetStationInfo")]
        public ActionResult<IEnumerable<BikeTrip>> Get([FromRoute] int stationID)
        {
            try
            {
                IEnumerable<BikeTrip> trips = DatabaseHandler.GetTripsFromStation(stationID);
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