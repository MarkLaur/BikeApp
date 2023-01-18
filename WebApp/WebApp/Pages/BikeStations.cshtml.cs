using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using WebApp.Models;
using WebApp.Services;

namespace WebApp.Pages
{
    public class BikeStationsModel : PageModel
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
        public IEnumerable<Station> BikeStations { get; private set; }

        public BikeStationsModel(ILogger<IndexModel> logger, ApiService apiService)
        {
            _logger = logger;

            _apiService = apiService;
            BikeStations = new Station[0];
        }

        public void OnGet()
        {
            //TODO: implement pagination

            string json;

            try
            {
                //TODO: this probably blocks the thread really bad, fix this
                Task<string> task = _apiService.GetJson(ApiDefinitions.BikeStationsUri);
                json = task.Result;
            }
            catch (Exception ex)
            {
                _logger.LogError("Bike station api request failed");

                ErrorMessage = ex.Message;
                return;
            }

            IEnumerable<Station>? stations = JsonSerializer.Deserialize<Station[]>(json,
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
            if (stations == null)
            {
                //Bike trips should be an empty array already so no need to re-create it
                ErrorMessage = "Bike station deserialization failed";
                _logger.LogError(ErrorMessage);
                return;
            }

            BikeStations = stations;
        }
    }
}
