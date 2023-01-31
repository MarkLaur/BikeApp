using ApiServer.Models;
using ApiServer.Tools;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace ApiServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BikeStationsController : ControllerBase
    {
        public class BikeStationsResponse
        {
            public int TotalStations { get; private set; }
            public ICollection<Station> Stations { get; private set; }

            public BikeStationsResponse(int totalStations, ICollection<Station> stations)
            {
                TotalStations = totalStations;
                Stations = stations;
            }
        }

        private readonly ILogger<BikeTripsController> _logger;

        public BikeStationsController(ILogger<BikeTripsController> logger)
        {
            _logger = logger;
        }

        [HttpGet] //Using the HttpGet(name) override makes swagger die as it can't tell the difference between gets and puts
        public ActionResult<BikeStationsResponse> Get([FromQuery, Range(1, int.MaxValue)] int page = 1)
        {
            try
            {
                //Run database requests in parallel
                Task<(bool, int)> countTask = DatabaseHandler.TryGetStationCount();
                Task<ICollection<Station>> stationsTask = DatabaseHandler.GetStations(page - 1, 100);

                int rowCount = countTask.Result.Item2;

                if (!countTask.Result.Item1)
                {
                    //TODO: return an error message
                    rowCount = -1;
                }

                return new ActionResult<BikeStationsResponse>(new BikeStationsResponse(rowCount, stationsTask.Result));
            }
            catch (Exception ex)
            {
                //Log goes to the console window and the status code as a response to the client
                _logger.LogError($"Failed to respond to BikeTrips api request. {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPut] //Using the HttpPut(name) override makes swagger die as it can't tell the difference between gets and puts
        public ActionResult Put([FromBody] List<Station> stations)
        {
            try
            {
                //This class should automagically validate the model since the class is decorated with [ApiController]

                //DatabaseHandler handles sanitization
                DatabaseHandler.InsertStations(stations);

                return StatusCode(StatusCodes.Status200OK);
            }
            catch (Exception ex)
            {
                //Log goes to the console window and the status code as a response to the client
                _logger.LogError($"Failed to put bikestations into database. {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}