using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using WebApp.Models;
using WebApp.Models.ApiResponses;
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
        public BikeTripsWithStations BikeTrips { get; private set; }

        public BikeTripsModel(ILogger<IndexModel> logger, ApiService apiService)
        {
            _logger = logger;

            _apiService = apiService;
            BikeTrips = new(new(), new());
        }

        public async Task OnGetAsync()
        {
            //TODO: implement pagination

            Stream json;

            //TODO: Do query on client side using AJAX
            try
            {
                json = await _apiService.GetJson(ApiDefinitions.BikeTripsUri);
            }
            catch (Exception ex)
            {
                _logger.LogError("Bike trips api request failed");

                ErrorMessage = ex.Message;
                return;
            }

            //Try to deserialize trips
            BikeTripsResponse? tripsResponse = await JsonSerializer.DeserializeAsync<BikeTripsResponse>(json,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            //Trips will be null if deserialization failed
            if (tripsResponse == null)
            {
                //Bike trips should be an empty array already so no need to re-create it
                ErrorMessage = "Bike trip deserialization failed";
                _logger.LogError(ErrorMessage);
                return;
            }

            //TODO: figure out how to make this happen automatically
            tripsResponse.Trips.OnDeserialized();

            BikeTrips = tripsResponse.Trips;
        }
    }
}
