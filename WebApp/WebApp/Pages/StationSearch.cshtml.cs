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

        public int? SearchedStationID { get; private set; }
        public Station? SearchedStation { get; private set; }
        public IEnumerable<BikeTrip>? StationTrips { get; private set; }

        public StationSearchModel(ILogger<IndexModel> logger, ApiService apiService)
        {
            _logger = logger;

            _apiService = apiService;
            SearchedStation = null;
            StationTrips = null;
        }

        public void OnGet([FromQuery] int? stationID)
        {
            SearchedStationID = stationID;

            if(!stationID.HasValue)
            {
                return;
            }

            //TODO: Do queries on client side using AJAX

            //TODO: get searched station info
            if(TryGetStation(stationID.Value, out Station tempStation, out string stationError))
            {
                SearchedStation = tempStation;
            }

            if(TryGetBikeTrips(stationID.Value, out IEnumerable<BikeTrip>? tripsTemp, out string tripsError)
                && tripsTemp != null)
            {
                StationTrips = tripsTemp;
            }

            if(!string.IsNullOrWhiteSpace(stationError) || !string.IsNullOrWhiteSpace(tripsError))
            {
                ErrorMessage = stationError + " " + tripsError;
            }
            else
            {
                ErrorMessage = null;
            }
        }

        private bool TryGetStation(int stationID, out Station station, out string userErrorMessage)
        {
            userErrorMessage = "";
            string json;

            try
            {
                //TODO: this probably blocks the thread really bad, fix this
                Task<string?> task = _apiService.TryGetStationJson(stationID);
                string? result = task.Result;

                if(result != null)
                {
                    json = result;
                }
                else
                {
                    userErrorMessage = $"Station {stationID} cannot be found";
                    station = default;
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Bike trips api request failed");

                userErrorMessage = "Failed to get Station json: " + ex.Message;
                station = default;
                return false;
            }

            station = JsonSerializer.Deserialize<Station>(json,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            //TODO: Figure out if deserialization can fail and how to test it.
            /*
            //Set empty array and return if deserialization failed
            if (!station.Valid)
            {
                userErrorMessage = "Bike trip deserialization failed";
                _logger.LogError(userErrorMessage);
                return false;
            }
            */

            return true;
        }

        private bool TryGetBikeTrips(int stationID, out IEnumerable<BikeTrip>? trips, out string userErrorMessage)
        {
            userErrorMessage = "";
            string json;
            Uri stationUri = ApiDefinitions.BuildStationInfoUri(stationID);

            try
            {
                //TODO: this probably blocks the thread really bad, fix this
                Task<string> task = _apiService.GetJson(stationUri);
                json = task.Result;
            }
            catch (Exception ex)
            {
                _logger.LogError("Bike trips api request failed");

                userErrorMessage = "Failed to get BikeTrips json: " + ex.Message;
                trips = null;
                return false;
            }

            trips = JsonSerializer.Deserialize<BikeTrip[]>(json,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            //Set empty array and return if deserialization failed
            if (trips == null)
            {
                userErrorMessage = "Bike trip deserialization failed";
                _logger.LogError(userErrorMessage);
                return false;
            }

            return true;
        }
    }
}
