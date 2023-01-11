using ApiServer.Tools;
using Microsoft.AspNetCore.Mvc;

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
        public IEnumerable<BikeTrip> Get()
        {
            if(!DatabaseHandler.TryGetTrips(out IEnumerable<BikeTrip> trips))
            {
                _logger.LogError("Couldn't get trips.");
            }

            return trips;
        }
    }
}