using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using WebApp.Models;
using WebApp.Models.ApiResponses;
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
        public ICollection<Station> BikeStations { get; private set; }

        public int CurrentPage { get; private set; }
        public int LastPage { get; private set; }

        public BikeStationsModel(ILogger<IndexModel> logger, ApiService apiService)
        {
            _logger = logger;

            _apiService = apiService;
            BikeStations = new Station[0];
        }

        public async Task OnGet([FromQuery, Range(1, int.MaxValue)] int page = 1)
        {
            //TODO: implement pagination

            BikeStationsResponse response;

            //TODO: Do query on client side using AJAX
            try
            {
                response = await _apiService.GetBikeStations(page);
            }
            catch (Exception ex)
            {
                _logger.LogError("Bike station api request failed. " + ex.Message);

                ErrorMessage = ex.Message;
                return;
            }

            LastPage = response.LastPage;
            CurrentPage = page;

            BikeStations = response.Stations;
        }
    }
}
