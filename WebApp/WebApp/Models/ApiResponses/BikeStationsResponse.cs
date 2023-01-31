using System.Text.Json.Serialization;

namespace WebApp.Models.ApiResponses
{
    public class BikeStationsResponse
    {
        public int TotalStations { get; private set; }
        public ICollection<Station> Stations { get; private set; }

        [JsonConstructor]
        public BikeStationsResponse(int totalStations, ICollection<Station> stations)
        {
            TotalStations = totalStations;
            Stations = stations;
        }
    }
}
