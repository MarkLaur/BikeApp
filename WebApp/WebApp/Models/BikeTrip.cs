using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace WebApp.Models
{
    public class BikeTrip
    {
        //Required attribute doesn't work on value types so we have to set the range to exclude the default value

        public int ID { get; private set; }

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

        [Required, Range(10, int.MaxValue)]
        public int DurationSeconds { get; private set; }

        //These are built from other data.
        //Duration is stored as seconds integer in db, we convert it to timespan for easier use.
        [JsonIgnore]
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
            int distance,
            int durationSeconds)
         //Call the other constructor and add set the id field.
         : this(
            departureTime,
            returnTime,
            departureStationID,
            returnStationID,
            distance,
            durationSeconds)
        {
            ID = id;
        }

        public BikeTrip(
            DateTime departureTime,
            DateTime returnTime,
            int departureStationID,
            int returnStationID,
            int distance,
            int durationSeconds)
        {
            DepartureTime = departureTime;
            ReturnTime = returnTime;
            DepartureStationID = departureStationID;
            ReturnStationID = returnStationID;
            Distance = distance;
            DurationSeconds = durationSeconds;

            Duration = TimeSpan.FromSeconds(durationSeconds);
        }

        /// <summary>
        /// Tries to build trip from a list of fields.
        /// </summary>
        /// <param name="fields"></param>
        /// <param name="station"></param>
        /// <returns></returns>
        public static bool TryBuildFromCSV(string[] fields, [NotNullWhen(true), MaybeNullWhen(false)] out BikeTrip trip)
        {
            //Departure,Return,Departure station id,Departure station name,Return station id,Return station name,Covered distance (m),Duration (sec.)

            CultureInfo culture = CultureInfo.InvariantCulture;

            //The imported csv can also have an unused fid field at the start, in this case we offset the fields by 1.
            int offset;
            bool withID;

            //Check if field length is valid and set proper offset
            if (fields.Length == 8)
            {
                offset = 0;
                withID = false;
            }
            else if (fields.Length == 9)
            {
                offset = 1;
                withID = true;
            }
            else
            {
                trip = null;
                return false;
            }

            //Check if all strings except id can be parsed and return if any of them fails
            if (
                !DateTime.TryParse(fields[0 + offset], out DateTime departure)
                || !DateTime.TryParse(fields[1 + offset], culture, out DateTime returnTime)
                || !int.TryParse(fields[2 + offset], out int departureStationID)
                || !int.TryParse(fields[4 + offset], out int returnStationID)
                || !int.TryParse(fields[6 + offset], out int distance)
                || !int.TryParse(fields[7 + offset], out int duration)
                )
            {
                trip = null;
                return false;
            }

            if (withID)
            {
                //Check if id can be parsed and return false if it can't
                if (!int.TryParse(fields[0], out int id))
                {
                    trip = null;
                    return false;
                }

                //Construct station from parsed data with id
                trip = new BikeTrip(
                    id,
                    departure,
                    returnTime,
                    departureStationID,
                    returnStationID,
                    distance,
                    duration
                    );
            }
            else
            {
                //Construct station from parsed data without id
                trip = new BikeTrip(
                    departure,
                    returnTime,
                    departureStationID,
                    returnStationID,
                    distance,
                    duration
                    );
            }

            //Validate the object using the built in validator. I'm not sure how efficient this is but it
            //validates the objects the same way as Asp.NET APIs do so we don't have to maintain another method.
            ValidationContext vc = new ValidationContext(trip);
            if (!Validator.TryValidateObject(trip, vc, null, true))
            {
                trip = null;
                return false;
            }

            return true;
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
