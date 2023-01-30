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
        public ActionResult<BikeTripsWithStations> Get()
        {
            try
            {
                BikeTripsWithStations trips = DatabaseHandler.GetTrips(0, 100);
                return new ActionResult<BikeTripsWithStations>(trips);
            }
            catch (Exception ex)
            {
                //Log goes to the console window and the status code as a response to the client
                _logger.LogError($"Failed to respond to BikeTrips api request. {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPut, RequestSizeLimit(200 * 1000 * 1024)] //Using the HttpPut(name) override makes swagger die as it can't tell the difference between gets and puts
        public ActionResult<TripInsertResult> Put([FromBody] List<BikeTrip> trips)
        {
            try
            {
                //This class should automagically validate the model since the class is decorated with [ApiController]

                //Either all trips should have IDs or none of them should to prevent broken values hidden in hundreds of lines from getting though.
                
                //TODO: handle the case where trips have defined id fields.

                //DatabaseHandler handles sanitization
                _logger.LogInformation("Trips received.");
                Task<TripInsertResult> insertTask = DatabaseHandler.InsertBikeTrips(trips, BikeTripInsertMode.Insert, false, _logger);

                TripInsertResult result = insertTask.Result;

                return StatusCode(StatusCodes.Status200OK, result);
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