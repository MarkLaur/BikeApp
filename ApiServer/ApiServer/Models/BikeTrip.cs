using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace ApiServer.Models
{
    public class BikeTrip
    {
        //Required attribute doesn't work on value types so we have to set the range to exclude the default value

        public int ID { get; private set; }

        //TODO: add validation that checks if departure time is before return time
        [Required, Range(typeof(DateTime), "1/1/0002", "1/1/2200")]
        public DateTime DepartureTime { get; private set; }

        [Required, Range(typeof(DateTime), "1/1/0002", "1/1/2200")]
        public DateTime ReturnTime { get; private set; }

        [Required, Range(1, int.MaxValue)]
        public int DepartureStationID { get; private set; }

        [Required, Range(1, int.MaxValue)]
        public int ReturnStationID { get; private set; }
        /// <summary>
        /// The trip's distance in meters.
        /// </summary>
        [Required, Range(10, int.MaxValue)]
        public int Distance { get; private set; }

        //These are built from other data.

        //Duration is calculated from return and departure time.
        //Trip length minimum is 10 seconds and maximum is 1 year.
        [JsonIgnore, Range(typeof(TimeSpan), "00:00:10", "365.00:00:00", ErrorMessage = "Trips must be longer than 10 seconds and shorter than 1 year.")]
        public TimeSpan Duration { get; private set; }

        [JsonIgnore]
        public Station? DepartureStation { get; private set; } = null;

        [JsonIgnore]
        public Station? ReturnStation { get; private set; } = null;

        [JsonConstructor]
        public BikeTrip(
            //The json deserializer wants parameter names to match field names. A workaround would be to have an empty constructor.
            int id,
            DateTime departureTime,
            DateTime returnTime,
            int departureStationID,
            int returnStationID,
            int distance)
         //Call the other constructor and add set the id field.
         : this(
            departureTime,
            returnTime,
            departureStationID,
            returnStationID,
            distance)
        {
            ID = id;
        }

        public BikeTrip(
            DateTime departureTime,
            DateTime returnTime,
            int departureStationID,
            int returnStationID,
            int distance)
        {
            DepartureTime = departureTime;
            ReturnTime = returnTime;
            DepartureStationID = departureStationID;
            ReturnStationID = returnStationID;
            Distance = distance;

            Duration = ReturnTime - DepartureTime;
        }

        public void SetDepartureStationsRef(Station departureStation)
        {
            if (DepartureStationID != departureStation.ID)
            {
                throw new ArgumentException("Station id must match");
            }

            DepartureStation = departureStation;
        }

        public void SetReturnStationRef(Station returnStation)
        {
            if (ReturnStationID != returnStation.ID)
            {
                throw new ArgumentException("Station id must match");
            }

            ReturnStation = returnStation;
        }
    }
}