using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.VisualBasic.FileIO;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text.Json;
using WebApp.Models;
using WebApp.Services;
using WebApp.Tools;

namespace WebApp.Pages
{
    [RequestSizeLimit(200 * 1000 * 1024)]
    public class ImportDataModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private ApiService _apiService;

        public string Message { get; private set; } = string.Empty;

        public ImportDataModel(ILogger<IndexModel> logger, ApiService apiService)
        {
            _logger = logger;
            _apiService = apiService;
        }

        public async Task OnPostStations([FromForm] IFormFile file)
        {
            if (file == null)
            {
                Message = "Select a file";
                return;
            }

            //TODO: validate csv and upload data on client side. Also, give the user info about progress.

            if (!CsvParser.TryParseCSV(file.OpenReadStream(), out List<Station> stations, out int invalidLines, ","))
            {
                Message = $"Invalid CSV format.";
                return;
            }

            Message = $"Uploading stations: {stations.Count}. Invalid lines: {invalidLines}.";

            HttpResponseMessage response = await _apiService.UploadStations(stations);

            if (response.IsSuccessStatusCode)
            {
                Message += $" Upload succesful.";
            }
            else if (response.StatusCode == HttpStatusCode.BadRequest) //Api returns 400 if model validation fails
            {
                string text = await response.Content.ReadAsStringAsync();

                //TODO: Get broken fields from response and show them to user in a nice form to user

                Message += $" Upload failed. api response: {response.StatusCode}.\n\nFull response:\n\n{text}";
            }
            else
            {
                Message += $" Upload failed. api response: {response.StatusCode}";
            }
        }

        public async Task OnPostTrips([FromForm] IFormFile file)
        {
            if (file == null)
            {
                Message = "Select a file";
                return;
            }

            //TODO: validate csv and upload data on client side. Also, give the user info about progress.

            if (!CsvParser.TryParseCSV(file.OpenReadStream(), out List<BikeTrip> trips, out int invalidLines, ","))
            {
                Message = $"Invalid CSV format.";
                return;
            }

            Message = $"Uploading bike trips: {trips.Count}. Invalid lines: {invalidLines}.";

            (HttpResponseMessage, TripInsertResult?) response = await _apiService.UploadTrips(trips);

            if (response.Item1.IsSuccessStatusCode)
            {
                Message += $" Upload succesful.";

                if (response.Item2 == null) Message += $" Reponse data block is null.";
                else if (response.Item2.AnyBadData) Message += $" {response.Item2}";
                else Message += $" {response.Item2}";
            }
            else if (response.Item1.StatusCode == HttpStatusCode.BadRequest) //Api returns 400 if model validation fails
            {
                string text = await response.Item1.Content.ReadAsStringAsync();

                //TODO: Get broken fields from response and show them to user in a nice form to user

                Message += $" Upload failed. api response: {response.Item1.StatusCode}.\n\nFull response:\n\n{text}";
            }
            else
            {
                Message += $" Upload failed. api response: {response.Item1.StatusCode}";
            }
        }
    }
}
