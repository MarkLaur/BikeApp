using ApiServer.Models;
using ApiServer.Tools;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ApiServer.Controllers
{
    [ApiController]
    [Route("[controller]/{stationID:int}")]
    public class BikeStationController : ControllerBase
    {
        private readonly ILogger<BikeStationController> _logger;

        public BikeStationController(ILogger<BikeStationController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetBikeStation")]
        public ActionResult<Station?> Get([FromRoute] int stationID)
        {
            try
            {
                if(DatabaseHandler.TryGetStation(stationID, out Station? station))
                {
                    //Return success bool as a header so we don't have to create a custom struct jsut for this
                    Response.Headers.Add("StationFound", "true");
                    return new ActionResult<Station?>(station);
                }
                else
                {
                    //Return success bool as a header so we don't have to create a custom struct jsut for this
                    Response.Headers.Add("StationFound", "false");
                    //Return blank station when station isn't found
                    return new ActionResult<Station?>(default(Station));
                }
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