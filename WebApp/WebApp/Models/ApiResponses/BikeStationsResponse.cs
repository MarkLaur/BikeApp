using System.Text.Json.Serialization;

namespace WebApp.Models.ApiResponses
{
    public class BikeStationsResponse
    {
        public int TotalBikeStations { get; private set; }
        public ICollection<Station> Stations { get; private set; }

        [JsonConstructor]
        public BikeStationsResponse(int totalBikeStations, ICollection<Station> stations)
        {
            TotalBikeStations = totalBikeStations;
            Stations = stations;
        }
    }
}
