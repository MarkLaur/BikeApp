using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApp.Models;
using WebApp.Services;

namespace WebApp.Pages
{
    public class BikeTripsModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        private BikeTripService _bikeTripService;
        public IEnumerable<BikeTrip> BikeTrips { get; private set; }

        public BikeTripsModel(ILogger<IndexModel> logger, BikeTripService bikeTripService)
        {
            _logger = logger;

            _bikeTripService = bikeTripService;
            BikeTrips = new BikeTrip[0];
        }

        public void OnGet()
        {
            BikeTrips = _bikeTripService.GetBikeTrips();
        }
    }
}
