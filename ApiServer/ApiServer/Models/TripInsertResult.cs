namespace ApiServer.Models
{
    public class TripInsertResult
    {
        public int InsertedItems { get; private set; }
        public int RejectedItems { get; private set; }
        public int MissingStationInstances { get; private set; }
        public int TotalMissingStations { get; private set; }
        public int OtherInvalidData { get; private set; }
        public string Message { get; private set; }

        public TripInsertResult(int insertedItems, int rejetedItems, int missingStationInstances, int totalMissingStations, int otherInvalidData)
        {
            InsertedItems = insertedItems;
            RejectedItems = rejetedItems;
            MissingStationInstances = missingStationInstances;
            TotalMissingStations = totalMissingStations;
            OtherInvalidData = otherInvalidData;

            if(RejectedItems != 0)
            {
                Message = "There was some bad trip data.";
            }
            else
            {
                Message = "All trip data was ok.";
            }

            if (MissingStationInstances != 0 || TotalMissingStations != 0) Message += " Stations must exist.";
            if (OtherInvalidData != 0) Message += " Trips are not allowed to have id fields if the insert mode is insert.";
        }

        public override string ToString() =>
            $"{Message} InsertedItems: {InsertedItems} RejectedItems: {RejectedItems} MissingStationInstances: {MissingStationInstances}" +
            $" TotalMissingStations: {TotalMissingStations} OtherInvalidData: {OtherInvalidData}";
    }
}
