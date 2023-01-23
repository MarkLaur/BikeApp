using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Runtime.CompilerServices;
using System.Text.Json;
using WebApp.Models;
using WebApp.Services;

namespace WebApp.Pages
{
    public class StationSearchModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        private ApiService _apiService;

        /// <summary>
        /// Contains the error message shown to user. Will be null if no errors occurred.
        /// </summary>
        public string? ErrorMessage { get; private set; }
        /// <summary>
        /// Contains station data if ErrorMessage isn't null.
        /// </summary>

        public int? SearchedStation { get; private set; }
        public IEnumerable<BikeTrip> StationTrips { get; private set; }

        public StationSearchModel(ILogger<IndexModel> logger, ApiService apiService)
        {
            _logger = logger;

            _apiService = apiService;
            StationTrips = new BikeTrip[0];
        }

        public void OnGet([FromQuery] int? stationID)
        {
            SearchedStation = stationID;

            if(!stationID.HasValue)
            {
                return;
            }

            string json;
            Uri stationUri = ApiDefinitions.BuildStationInfoUri(stationID.Value);

            //TODO: get searched station info

            //TODO: Do query on client side using AJAX
            try
            {
                //TODO: this probably blocks the thread really bad, fix this
                Task<string> task = _apiService.GetJson(stationUri);
                json = task.Result;
            }
            catch (Exception ex)
            {
                _logger.LogError("Bike trips api request failed");

                ErrorMessage = "Failed to get json" + ex.Message;
                return;
            }

            IEnumerable<BikeTrip>? trips = JsonSerializer.Deserialize<BikeTrip[]>(json,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            //Set empty array and return if deserialization failed
            if (trips == null)
            {
                //Bike trips should be an empty array already so no need to re-create it
                ErrorMessage = "Bike trip deserialization failed";
                _logger.LogError(ErrorMessage);
                return;
            }

            StationTrips = trips;
        }
    }
}
