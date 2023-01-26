using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics.CodeAnalysis;
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

        public async Task OnGetAsync([FromQuery] int? stationID)
        {
            SearchedStationID = stationID;

            if(!stationID.HasValue)
            {
                return;
            }

            //TODO: Do queries on client side using AJAX

            //Start both tasks
            Task<(bool, Station?, string)> stationTask = TryGetStation(stationID.Value);
            Task<(bool, IEnumerable<BikeTrip>?, string)> tripsTask = TryGetBikeTrips(stationID.Value);

            //Get station data
            (bool, Station?, string) result = await stationTask;
            if (result.Item1)
            {
                SearchedStation = result.Item2;
            }

            //Get trips from station
            (bool, IEnumerable<BikeTrip>?, string) result2 = await tripsTask;
            if (result2.Item1)
            {
                StationTrips = result2.Item2;
            }

            //Build an error message for the user if something coudln't be found.
            if(!string.IsNullOrWhiteSpace(result.Item3) || !string.IsNullOrWhiteSpace(result2.Item3))
            {
                ErrorMessage = result.Item3 + " " + result2.Item3;
            }
            else
            {
                ErrorMessage = null;
            }
        }

        /// <summary>
        /// Returns a (success, station, userErrorMessage) tuple. Station is null when success is false.
        /// </summary>
        /// <param name="stationID"></param>
        /// <param name="station"></param>
        /// <param name="userErrorMessage"></param>
        /// <returns></returns>
        private async Task<(bool, Station?, string)> TryGetStation(int stationID)
        {
            (bool, Stream?) result;

            try
            {
                //TODO: Figure out what can throw errors in TryGetStationJson() and handle it.
                result = await _apiService.TryGetStationJson(stationID);
            }
            catch (Exception ex)
            {
                _logger.LogError("Bike trips api request failed");

                return (false, default, "Failed to get Station json: " + ex.Message);
            }

            //Check if TryGetStationJson() succeeded
            if (!result.Item1 || result.Item2 == null)
            {
                return (false, default, $"Station {stationID} cannot be found");
            }

            //Attempt to deserialize
            Station? station = await JsonSerializer.DeserializeAsync<Station>(result.Item2,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            //Station will be null if serialization fails
            if (station != null)
            {
                return (true, station, "");
            }
            else
            {
                return (false, default, "Station deserialization failed");
            }
        }

        /// <summary>
        /// Returns a (success, trips, userErrorMessage) tuple. Trips is null when success is false.
        /// </summary>
        /// <param name="stationID"></param>
        /// <param name="trips"></param>
        /// <param name="userErrorMessage"></param>
        /// <returns></returns>
        private async Task<(bool, IEnumerable<BikeTrip>?, string)> TryGetBikeTrips(int stationID)
        {
            Stream json;

            try
            {
                Uri stationUri = ApiDefinitions.BuildStationInfoUri(stationID);

                //TODO: Figure out what can throw errors in TryGetStationJson() and handle it.
                json = await _apiService.GetJson(stationUri);
            }
            catch (Exception ex)
            {
                _logger.LogError("Bike trips api request failed");

                return (false, null, "Failed to get BikeTrips json: " + ex.Message);
            }

            //Try to deserialize
            IEnumerable<BikeTrip>? trips = await JsonSerializer.DeserializeAsync<BikeTrip[]>(json,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            //trips will be null if serialization fails
            if (trips != null)
            {
                return (true, trips, "");
            }
            else
            {
                return (false, default, "Bike trip deserialization failed");
            }
        }
    }
}
