using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Routing.Patterns;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text.Json;
using WebApp.Models;
using WebApp.Models.ApiResponses;
using WebApp.Services;

namespace WebApp.Pages
{
    public class StationInfoModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        private ApiService _apiService;

        /// <summary>
        /// Contains the error message shown to user.
        /// </summary>
        public string ErrorMessage { get; private set; } = "";

        public bool StationFound { get; private set; }
        public Station? SearchedStation { get; private set; }
        public BikeTripsWithStations? StationTrips { get; private set; }

        public int CurrentPage { get; private set; }
        public int LastPage { get; private set; }

        public StationInfoModel(ILogger<IndexModel> logger, ApiService apiService)
        {
            _logger = logger;

            _apiService = apiService;
            SearchedStation = null;
            StationTrips = null;
        }

        public async Task OnGetAsync([FromRoute] int stationID, [FromQuery, Range(1, int.MaxValue)] int page = 1)
        {
            //TODO: Do queries on client side using AJAX

            //Start both tasks
            Task<(Station?, string)> stationTask = _apiService.TryGetStation(stationID);
            Task<BikeTripsResponse> tripsTask = _apiService.GetBikeTrips(page, stationID);

            //Get station data
            (Station?, string) result = await stationTask;
            StationFound = result.Item1 != null;
            if (StationFound)
            {
                SearchedStation = result.Item1;
            }
            else 
            {
                ErrorMessage = $"Invalid station id {stationID}. {result.Item2}";
                return;
            }

            //Get trips from station
            BikeTripsResponse result2 = await tripsTask;

            StationTrips = result2.Trips;

            LastPage = result2.TotalBikeTrips;
            CurrentPage = page;
        }
    }
}
