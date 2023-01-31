using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Web;
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

        public int Page { get; private set; }

        /// <summary>
        /// Will be null if there is no next page
        /// </summary>
        public string? NextPage { get; private set; } = null;

        /// <summary>
        /// Will be null if there is no previous page
        /// </summary>
        public string? PreviousPage { get; private set; } = null;

        public BikeTripsModel(ILogger<IndexModel> logger, ApiService apiService)
        {
            _logger = logger;

            _apiService = apiService;
            BikeTrips = new(new(), new());
        }

        public async Task OnGetAsync([FromQuery, Range(1, int.MaxValue)] int page = 1)
        {
            BikeTripsResponse tripsResponse;

            //TODO: Do query on client side using AJAX
            try
            {
                tripsResponse = await _apiService.GetBikeTrips(page);
            }
            catch (Exception ex)
            {
                _logger.LogError("Bike trips api request failed. " + ex.Message);

                ErrorMessage = ex.Message;
                return;
            }

            Page = page;
            //Check if next and previous pages exist and add the query string
            if (page > 1) PreviousPage = Request.Path + $"?page={page - 1}";
            if (page - 1 < tripsResponse.TotalBikeTrips) NextPage = Request.Path + $"?page={page + 1}";

            BikeTrips = tripsResponse.Trips;
        }
    }
}
