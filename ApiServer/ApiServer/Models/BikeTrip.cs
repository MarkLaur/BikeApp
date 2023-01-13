namespace ApiServer.Models
{
    public class BikeTrip
    {
        public int ID { get; private set; }
        public DateTime DepartureTime { get; private set; }
        public DateTime ReturnTime { get; private set; }
        public Station DepartureStation { get; private set; }
        public Station ReturnStation { get; private set; }
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
            Station departureStation,
            Station returnStation,
            int distance,
            TimeSpan duration)
        {
            ID = id;
            DepartureTime = departureTime;
            ReturnTime = returnTime;
            DepartureStation = departureStation;
            ReturnStation = returnStation;
            Distance = distance;
            Duration = duration;
        }
    }
}