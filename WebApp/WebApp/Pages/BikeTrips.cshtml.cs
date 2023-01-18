using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using WebApp.Models;
using WebApp.Services;

namespace WebApp.Pages
{
    public class BikeTripsModel : PageModel
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
        public IEnumerable<BikeTrip> BikeTrips { get; private set; }

        public BikeTripsModel(ILogger<IndexModel> logger, ApiService apiService)
        {
            _logger = logger;

            _apiService = apiService;
            BikeTrips = new BikeTrip[0];
        }

        public void OnGet()
        {
            //TODO: implement pagination

            string json;

            try
            {
                //TODO: this probably blocks the thread really bad, fix this
                Task<string> task = _apiService.GetJson(ApiDefinitions.BikeTripsUri);
                json = task.Result;
            }
            catch (Exception ex)
            {
                _logger.LogError("Bike trips api request failed");

                ErrorMessage = ex.Message;
                return;
            }

            IEnumerable<BikeTrip>? trips = JsonSerializer.Deserialize<BikeTrip[]>(json,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            //TODO: fix nested object deserialization
            //The built in json deserializer cant deserialize the nested struct for some reason. Consider using Newtonsoft.Json
            /*
            List<BikeTrip> tripsList = trips.ToList();
            Station station = tripsList[0].DepartureStation;
            */

            //Set empty array and return if deserialization failed
            if (trips == null)
            {
                //Bike trips should be an empty array already so no need to re-create it
                ErrorMessage = "Bike trip deserialization failed";
                _logger.LogError(ErrorMessage);
                return;
            }

            BikeTrips = trips;
        }
    }
}
