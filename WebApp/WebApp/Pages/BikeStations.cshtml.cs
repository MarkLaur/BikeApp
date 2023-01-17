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

            //TODO: this probably blocks the thread really bad, fix this
            Task<(bool, string?)> task = _apiService.TryGetJson(ApiDefinitions.BikeStationsUri);
            (bool, string?) result = task.Result;

            //Set empty array and return if request failed
            if (!result.Item1 || result.Item2 == null)
            {
                //Bike trips should be an empty array already so no need to re-create it
                _logger.LogError("Bike station api request failed");
                return;
            }

            IEnumerable<Station>? stations = JsonSerializer.Deserialize<Station[]>(result.Item2,
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
                _logger.LogError("Bike station deserialization failed");
                return;
            }

            BikeStations = stations;
        }
    }
}
