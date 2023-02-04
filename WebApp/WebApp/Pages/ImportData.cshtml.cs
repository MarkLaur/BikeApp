using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net;
using WebApp.Models;
using WebApp.Models.ApiResponses;
using WebApp.Services;
using WebApp.Tools;

namespace WebApp.Pages
{
    [RequestSizeLimit(200 * 1000 * 1024)]
    public class ImportDataModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private ApiService _apiService;

        public string StationsMessage { get; private set; } = string.Empty;
        public string TripsMessage { get; private set; } = string.Empty;
        public string StationMessage { get; private set; } = string.Empty;
        public string TripMessage { get; private set; } = string.Empty;

        public ImportDataModel(ILogger<IndexModel> logger, ApiService apiService)
        {
            _logger = logger;
            _apiService = apiService;
        }

        public async Task OnPostStations([FromForm] IFormFile file)
        {
            if (file == null)
            {
                StationsMessage = "Select a file";
                return;
            }

            //TODO: validate csv and upload data on client side. Also, give the user info about progress.

            if (!CsvParser.TryParseCSV(file.OpenReadStream(), out List<Station> stations, out int invalidLines, ","))
            {
                StationsMessage = $"Invalid CSV format.";
                return;
            }

            StationsMessage = $"Uploading stations: {stations.Count}. Invalid lines: {invalidLines}.";

            HttpResponseMessage response = await _apiService.UploadStations(stations);

            if (response.IsSuccessStatusCode)
            {
                StationsMessage += $" Upload succesful.";
            }
            else if (response.StatusCode == HttpStatusCode.BadRequest) //Api returns 400 if model validation fails
            {
                string text = await response.Content.ReadAsStringAsync();

                //TODO: Get broken fields from response and show them to user in a nice form to user

                StationsMessage += $" Upload failed. api response: {response.StatusCode}.\n\nFull response:\n\n{text}";
            }
            else
            {
                StationsMessage += $" Upload failed. api response: {response.StatusCode}";
            }
        }

        public async Task OnPostStation(
            [FromForm] int id,
            [FromForm] string nameFin,
            [FromForm] string nameSwe,
            [FromForm] string name,
            [FromForm] string addressFin,
            [FromForm] string addressSwe,
            [FromForm] string cityFin,
            [FromForm] string citySwe,
            [FromForm] string operatorName,
            [FromForm] int capacity,
            [FromForm] decimal posX,
            [FromForm] decimal posY)
        {
            Station[] station = new Station[] { 
                new Station( id, nameFin, nameSwe, name, addressFin, addressSwe, cityFin, citySwe, operatorName, capacity, posX, posY)};
            //TODO: validate station

            //TODO: validate csv and upload data on client side. Also, give the user info about progress.

            StationMessage = $"Uploading station.";

            HttpResponseMessage response = await _apiService.UploadStations(station);

            if (response.IsSuccessStatusCode)
            {
                StationMessage += $" Station added to db.";
            }
            else if (response.StatusCode == HttpStatusCode.BadRequest) //Api returns 400 if model validation fails
            {
                string text = await response.Content.ReadAsStringAsync();

                //TODO: Get broken fields from response and show them to user in a nice form to user

                StationMessage += $" Upload failed. api response: {response.StatusCode}.\n\nFull response:\n\n{text}";
            }
            else
            {
                StationMessage += $" Upload failed. api response: {response.StatusCode}";
            }
        }

        public async Task OnPostTrips([FromForm] IFormFile file)
        {
            if (file == null)
            {
                TripsMessage = "Select a file";
                return;
            }

            _logger.LogInformation("File received. Starting CSV parsing.");

            //TODO: validate csv and upload data on client side. Also, give the user info about progress.

            if (!CsvParser.TryParseCSV(file.OpenReadStream(), out List<BikeTrip> trips, out int invalidLines, ","))
            {
                TripsMessage = $"Invalid CSV format.";
                return;
            }

            _logger.LogInformation($"CSV parsed. Invalid line count: {invalidLines}. Starting upload.");
            TripsMessage = $"Uploading bike trips: {trips.Count}. Invalid lines: {invalidLines}.";

            (HttpResponseMessage, TripInsertResult?) response = await _apiService.UploadTrips(trips);

            //TODO: Make messaging in the app good so that we don't have to use the debug windows

            // Log the result
            if (response.Item1.IsSuccessStatusCode)
            {
                TripsMessage += $" Upload succesful.";

                if (response.Item2 == null)
                {
                    _logger.LogInformation($" Upload succesful. Response data block is null.");
                    TripsMessage += $" Reponse data block is null.";
                }
                else if (response.Item2.RejectedItems != 0)
                {
                    _logger.LogInformation($" Upload succesful. Some invalid data was rejected. {response.Item2}");
                    TripsMessage += $" Some invalid data was rejected. {response.Item2}";
                }
                else
                {
                    _logger.LogInformation($" Upload succesful. All data was accepted.");
                    TripsMessage += $" All data was accepted. Trips added: {response.Item2.InsertedItems}";
                }
            }
            else if (response.Item1.StatusCode == HttpStatusCode.BadRequest) //Api returns 400 if model validation fails
            {
                string text = await response.Item1.Content.ReadAsStringAsync();

                //TODO: Get broken fields from response and show them to user in a nice form to user

                TripsMessage += $" Upload failed. api response: {response.Item1.StatusCode}.\n\nFull response:\n\n{text}";
            }
            else
            {
                TripsMessage += $" Upload failed. api response: {response.Item1.StatusCode}";
            }
        }

        public async Task OnPostTrip(
            [FromForm] DateTime departureTime,
            [FromForm] DateTime returnTime,
            [FromForm] int departureStation,
            [FromForm] int returnStation,
            [FromForm] int distance)
        {
            BikeTrip[] trip = new BikeTrip[] { new BikeTrip(departureTime, returnTime, departureStation, returnStation, distance) };
            //TODO: validate trip

            //TODO: validate trip and upload data on client side. Also, give the user info about progress.

            TripMessage = $"Uploading bike trip.";

            (HttpResponseMessage, TripInsertResult?) response = await _apiService.UploadTrips(trip);

            if (response.Item1.IsSuccessStatusCode)
            {
                if (response.Item2 == null) TripMessage += $" Reponse data block is null.";
                else if (response.Item2.RejectedItems != 0)
                {
                    TripMessage += $" Some trip data was invalid. {response.Item2}";
                }
                else
                {
                    TripMessage += $" Trip added to db.";
                }
            }
            else if (response.Item1.StatusCode == HttpStatusCode.BadRequest) //Api returns 400 if model validation fails
            {
                string text = await response.Item1.Content.ReadAsStringAsync();

                //TODO: Get broken fields from response and show them to user in a nice form to user

                TripMessage += $" Upload failed. api response: {response.Item1.StatusCode}.\n\nFull response:\n\n{text}";
            }
            else
            {
                TripMessage += $" Upload failed. api response: {response.Item1.StatusCode}";
            }
        }
    }
}
