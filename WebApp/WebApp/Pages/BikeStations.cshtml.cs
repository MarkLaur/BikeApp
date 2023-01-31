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

        /// <summary>
        /// Will be null if there is no next page
        /// </summary>
        public string? NextPage { get; private set; } = null;

        /// <summary>
        /// Will be null if there is no previous page
        /// </summary>
        public string? PreviousPage { get; private set; } = null;

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

            //Elements per page is hardcoded in api.
            //TODO: Make elements per page configurable.
            int perPage = 100;
            LastPage = (response.TotalStations - 1) / perPage + 1;

            CurrentPage = page;
            //Check if next and previous pages exist and add the query string
            if (page > 1) PreviousPage = Request.Path + $"?page={page - 1}";
            if (page < LastPage) NextPage = Request.Path + $"?page={page + 1}";

            BikeStations = response.Stations;
        }
    }
}
