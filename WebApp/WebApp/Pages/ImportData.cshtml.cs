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

namespace WebApp.Pages
{
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

            //TODO: validate csv and upload data on client side
            TryParseCSV(file, out List<Station> stations, out int invalidLines, ",");
            Message = $"Uploading Stations: {stations.Count}. Invalid lines: {invalidLines}.";

            //TODO: this probably blocks the main thread pretty bad, fix this
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
        }

        private bool TryParseCSV(IFormFile file, out List<Station> stations, out int invalidLines, params string[] delimiters)
        {
            invalidLines = 0;

            using (TextFieldParser parser = new TextFieldParser(file.OpenReadStream()))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(delimiters);
                stations = new List<Station>();

                string[]? rowCache;

                if (parser.EndOfData)
                {
                    return false;
                }

                //Check header length
                rowCache = parser.ReadFields();
                if (rowCache == null || rowCache.Length == 0)
                {
                    _logger.LogInformation($"First row is invalid");
                    return false;
                }

                //Save first row length to make sure length doens't change in the file
                int headerFieldCount = rowCache.Length;

                //Check if first row is a header or a data row
                if (TryFromCSV(rowCache, out Station? stat))
                {
                    stations.Add(stat);
                }

                while (!parser.EndOfData)
                {
                    //TODO: figure out how to check if ReadFields() can run.
                    //ReadFields() will throw an exception for every broken line, which can be somewhat expensive if there are a lot of them
                    try
                    {
                        rowCache = parser.ReadFields();

                        if (rowCache != null
                            && rowCache.Length == headerFieldCount
                            && TryFromCSV(rowCache, out Station? station))
                        {
                            stations.Add(station);
                        }
                        else
                        {
                            invalidLines++;
                        }
                    }
                    catch
                    {
                        invalidLines++;
                    }
                }

                return true;
            }
        }

        private bool TryFromCSV(string[] fields, [NotNullWhen(true), MaybeNullWhen(false)] out Station station)
        {
            //This might be needed depending on what the parser does
            /*
            if (fields.Contains(null))
            {
                station = default;
                return false;
            }
            */

            CultureInfo culture = CultureInfo.InvariantCulture;

            int id;
            int capacity;
            decimal x;
            decimal y;

            //Check length and try to parse strings
            if(fields.Length == 12
                && int.TryParse(fields[0], out id)
                && int.TryParse(fields[9], out capacity)
                && decimal.TryParse(fields[10], culture, out x)
                && decimal.TryParse(fields[11], culture, out y)
                )
            {
                //Construct station from parsed data
                station = new Station(id,
                    fields[1],
                    fields[2],
                    fields[3],
                    fields[4],
                    fields[5],
                    fields[6],
                    fields[7],
                    fields[8],
                    capacity,
                    x,
                    y
                    );
            }
            //The imported csv can also have a fid field at the start
            else if (fields.Length == 13
                && int.TryParse(fields[1], out id)
                && int.TryParse(fields[10], out capacity)
                && decimal.TryParse(fields[11], culture, out x)
                && decimal.TryParse(fields[12], culture, out y)
                )
            {
                //Construct station from parsed data

                station = new Station(id,
                    fields[2],
                    fields[3],
                    fields[4],
                    fields[5],
                    fields[6],
                    fields[7],
                    fields[8],
                    fields[9],
                    capacity,
                    x,
                    y
                    );
            }
            else
            {
                station = null;
                return false;
            }

            ValidationContext vc = new ValidationContext(station);
            return Validator.TryValidateObject(station, vc, null, true);
        }
    }
}
