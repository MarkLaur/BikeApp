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
            return Enumerable.Range(1, 5).Select(index => new BikeTrip
            {
                Name = $"Trip {index}",
                Length = index * index
            })
            .ToArray();
        }
    }
}