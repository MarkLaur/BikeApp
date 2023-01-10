using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApp.Models;
using WebApp.Services;

namespace WebApp.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private BikeTripService _bikeTripService;
        public IEnumerable<BikeTrip> BikeTrips { get; private set; }

        public IndexModel(ILogger<IndexModel> logger, BikeTripService bikeTripService)
        {
            _logger = logger;
            _bikeTripService = bikeTripService;
        }

        public void OnGet()
        {
            BikeTrips = _bikeTripService.GetBikeTrips();
        }
    }
}
