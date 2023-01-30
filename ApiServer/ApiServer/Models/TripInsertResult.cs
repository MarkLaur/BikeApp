namespace ApiServer.Models
{
    public class TripInsertResult
    {
        public bool AnyBadData => MissingStationInstances != 0 || TotalMissingStations != 0 || OtherInvalidData != 0;
        public int MissingStationInstances { get; private set; }
        public int TotalMissingStations { get; private set; }
        public int OtherInvalidData { get; private set; }
        public string Message { get; private set; }

        public TripInsertResult(int missingStationInstances, int totalMissingStations, int otherInvalidData)
        {
            MissingStationInstances = missingStationInstances;
            TotalMissingStations = totalMissingStations;
            OtherInvalidData = otherInvalidData;

            if (AnyBadData)
            {
                Message = "There was some bad trip data. Trips are not allowed to have id fields if the insert mode is insert.";
            }
            else
            {
                Message = "All trip data was ok";
            }
        }
    }
}
