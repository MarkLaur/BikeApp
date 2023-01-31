using System.Text.Json.Serialization;

namespace WebApp.Models.ApiResponses
{
    public class BikeTripsResponse
    {
        public int TotalBikeTrips { get; private set; }
        public BikeTripsWithStations Trips { get; private set; }

        [JsonConstructor]
        public BikeTripsResponse(int totalBikeTrips, BikeTripsWithStations trips)
        {
            TotalBikeTrips = totalBikeTrips;
            Trips = trips;
        }
    }
}
