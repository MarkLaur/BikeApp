using System.Text.Json.Serialization;

namespace WebApp.Models
{
    public class TripInsertResult
    {
        public bool AnyBadData => MissingStationInstances != 0 || TotalMissingStations != 0 || OtherInvalidData != 0;
        public int MissingStationInstances { get; private set; }
        public int TotalMissingStations { get; private set; }
        public int OtherInvalidData { get; private set; }
        public string Message { get; private set; }

        [JsonConstructor]
        public TripInsertResult(int missingStationInstances, int totalMissingStations, int otherInvalidData, string message)
        {
            MissingStationInstances = missingStationInstances;
            TotalMissingStations = totalMissingStations;
            OtherInvalidData = otherInvalidData;
            Message = message;
        }

        public override string ToString() =>
            $"{Message} MissingStationInstances: {MissingStationInstances}" +
            $" TotalMissingStations: {TotalMissingStations} OtherInvalidData: {OtherInvalidData}";
    }
}
