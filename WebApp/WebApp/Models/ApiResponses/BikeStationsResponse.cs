using System.Text.Json.Serialization;

namespace WebApp.Models.ApiResponses
{
    public class BikeStationsResponse
    {
        public int TotalBikeStations { get; private set; }
        public IEnumerable<Station> Stations { get; private set; }

        [JsonConstructor]
        public BikeStationsResponse(int totalBikeStations, IEnumerable<Station> stations)
        {
            TotalBikeStations = totalBikeStations;
            Stations = stations;
        }
    }
}
