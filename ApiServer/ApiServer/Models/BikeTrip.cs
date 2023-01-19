namespace ApiServer.Models
{
    public class BikeTrip
    {
        public int ID { get; private set; }
        public DateTime DepartureTime { get; private set; }
        public DateTime ReturnTime { get; private set; }
        public int DepartureStationID { get; private set; }
        public string DepartureStationName { get; private set; }
        public int ReturnStationID { get; private set; }
        public string ReturnStationName { get; private set; }
        /// <summary>
        /// The trip's distance in meters.
        /// </summary>
        public int Distance { get; private set; }
        //Duration is stored as seconds integer in db, we convert it to timespan for easier use.
        public TimeSpan Duration { get; private set; }

        public BikeTrip(
            //The json deserializer wants parameter names to match field names. A workaround would be to have an empty constructor.
            int id,
            DateTime departureTime,
            DateTime returnTime,
            int departureStationID,
            string departureStationName,
            int returnStationID,
            string returnStationName,
            int distance,
            TimeSpan duration)
        {
            ID = id;
            DepartureTime = departureTime;
            ReturnTime = returnTime;
            DepartureStationID = departureStationID;
            DepartureStationName = departureStationName;
            ReturnStationID = returnStationID;
            ReturnStationName = returnStationName;
            Distance = distance;
            Duration = duration;
        }
    }
}