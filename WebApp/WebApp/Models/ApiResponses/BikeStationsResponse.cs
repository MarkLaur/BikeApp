using System.Text.Json.Serialization;

namespace WebApp.Models.ApiResponses
{
    public class BikeStationsResponse
    {
        //Elements per page is hardcoded in api.
        //TODO: Make elements per page configurable.
        private const int perPage = 100;

        /// <summary>
        /// The total amount of stations in database that matched the query.
        /// </summary>
        public int TotalStations { get; private set; }
        public ICollection<Station> Stations { get; private set; }

        /// <summary>
        /// Returns the last page by calculating the amount of pages from total station count and trips per page amount.
        /// </summary>
        public int LastPage => (TotalStations - 1) / perPage + 1;

        [JsonConstructor]
        public BikeStationsResponse(int totalStations, ICollection<Station> stations)
        {
            TotalStations = totalStations;
            Stations = stations;
        }
    }
}
