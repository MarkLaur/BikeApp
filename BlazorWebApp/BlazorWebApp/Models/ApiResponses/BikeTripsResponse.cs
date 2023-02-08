using System.Text.Json.Serialization;

namespace BlazorWebApp.Models.ApiResponses
{
    public class BikeTripsResponse
    {
        //Elements per page is hardcoded in api.
        //TODO: Make elements per page configurable.
        private const int perPage = 100;

        /// <summary>
        /// The total amount of bike trips in database that matched the query.
        /// </summary>
        public int TotalBikeTrips { get; private set; }
        public BikeTripsWithStations Trips { get; private set; }

        /// <summary>
        /// Returns the last page by calculating the amount of pages from total trip count and trips per page amount.
        /// </summary>
        public int LastPage => (TotalBikeTrips - 1) / perPage + 1;

        [JsonConstructor]
        public BikeTripsResponse(int totalBikeTrips, BikeTripsWithStations trips)
        {
            TotalBikeTrips = totalBikeTrips;
            Trips = trips;
        }
    }
}
