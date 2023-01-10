using Microsoft.AspNetCore.Mvc;

namespace ApiServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BikeTripController : ControllerBase
    {
        private readonly ILogger<BikeTripController> _logger;

        public BikeTripController(ILogger<BikeTripController> logger)
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