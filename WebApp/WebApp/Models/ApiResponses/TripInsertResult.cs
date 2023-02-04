using System.Text.Json.Serialization;

namespace WebApp.Models.ApiResponses
{
    public class TripInsertResult
    {
        public int InsertedItems { get; private set; }
        public int RejectedItems { get; private set; }
        public int MissingStationInstances { get; private set; }
        public int TotalMissingStations { get; private set; }
        public int OtherInvalidData { get; private set; }
        public string Message { get; private set; }

        [JsonConstructor]
        public TripInsertResult(
            int insertedItems,
            int rejectedItems,
            int missingStationInstances,
            int totalMissingStations,
            int otherInvalidData,
            string message)
        {
            InsertedItems = insertedItems;
            RejectedItems = rejectedItems;
            MissingStationInstances = missingStationInstances;
            TotalMissingStations = totalMissingStations;
            OtherInvalidData = otherInvalidData;
            Message = message;
        }

        public override string ToString() =>
            $"{Message} InsertedItems: {InsertedItems} RejectedItems: {RejectedItems} MissingStationInstances: {MissingStationInstances}" +
            $" TotalMissingStations: {TotalMissingStations} OtherInvalidData: {OtherInvalidData}";
    }
}
