using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text.Json;
using WebApp.Models;
using WebApp.Models.ApiResponses;
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

        public int? StationIdQuery { get; private set; }
        public string? StationNameQuery { get; private set; }

        public ICollection<Station> FoundStations { get; private set; } = new List<Station>();

        public int CurrentPage { get; private set; }
        public int LastPage { get; private set; }

        public StationSearchModel(ILogger<IndexModel> logger, ApiService apiService)
        {
            _logger = logger;

            _apiService = apiService;
        }

        public async Task OnGetAsync([FromQuery] int? stationID, [FromQuery] string? stationName, [FromQuery, Range(1, int.MaxValue)] int page = 1)
        {
            //TODO: Do queries on client side using AJAX
            StationIdQuery = stationID;
            StationNameQuery = stationName;

            //Search with id
            if (stationID.HasValue)
            {
                StationNameQuery = null;
                (Station?, string) result = await _apiService.TryGetStation(stationID.Value);
                if (result.Item1 != null)
                {
                    FoundStations.Add(result.Item1);
                }
                else
                {
                    ErrorMessage = result.Item2;
                }
            }
            //Search with name, gets all stations if stationName is null or empty
            else
            {
                StationIdQuery = null;
                BikeStationsResponse response = await _apiService.GetBikeStations(1, stationName);
                FoundStations = response.Stations;

                LastPage = response.LastPage;
                CurrentPage = page;
            }
        }
    }
}
