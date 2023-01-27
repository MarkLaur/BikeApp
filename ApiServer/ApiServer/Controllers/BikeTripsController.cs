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

        [HttpGet] //Using the HttpGet(name) override makes swagger die as it can't tell the difference between gets and puts
        public ActionResult<IEnumerable<BikeTrip>> Get()
        {
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

        [HttpPut] //Using the HttpPut(name) override makes swagger die as it can't tell the difference between gets and puts
        public ActionResult Put([FromBody] List<BikeTrip> trips)
        {
            try
            {
                //This class should automagically validate the model since the class is decorated with [ApiController]

                //TODO: handle the case where trips have defined id fields.

                //DatabaseHandler handles sanitization
                DatabaseHandler.InsertBikeTrips(trips, BikeTripInsertMode.Insert);

                return StatusCode(StatusCodes.Status200OK);
            }
            catch (Exception ex)
            {
                //Log goes to the console window and the status code as a response to the client
                _logger.LogError($"Failed to put bike trips into database. {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}