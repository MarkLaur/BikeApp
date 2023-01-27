using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.Json.Serialization;

namespace WebApp.Models
{
    public class Station
    {
        [Required, Range(0, int.MaxValue)]
        public int ID { get; private set; }
        /// <summary>
        /// Finnish name
        /// </summary>
        [StringLength(100)]
        public string NameFin { get; private set; }
        /// <summary>
        /// Swedish name
        /// </summary>
        [StringLength(100)]
        public string NameSwe { get; private set; }
        /// <summary>
        /// English name
        /// </summary>
        [StringLength(100)]
        public string Name { get; private set; }

        [StringLength(100)]
        public string AddressFin { get; private set; }

        [StringLength(100)]
        public string AddressSwe { get; private set; }

        [StringLength(100)]
        public string CityFin { get; private set; }

        [StringLength(100)]
        public string CitySwe { get; private set; }

        [StringLength(100)]
        public string OperatorName { get; private set; }

        [Range(0, int.MaxValue)]
        public int Capacity { get; private set; }

        [Required]
        public decimal PosX { get; private set; }

        [Required]
        public decimal PosY { get; private set; }

        [JsonConstructor] //JsonSerializer can't deserialize things with multiple constructors without this thing
        public Station(
            int id,
            string nameFin,
            string nameSwe,
            string name,
            string addressFin,
            string addressSwe,
            string cityFin,
            string citySwe,
            string operatorName,
            int capacity,
            decimal posX,
            decimal posY
            )
        {
            ID = id;
            NameFin = nameFin;
            NameSwe = nameSwe;
            Name = name;
            AddressFin = addressFin;
            AddressSwe = addressSwe;
            CityFin = cityFin;
            CitySwe = citySwe;
            OperatorName = operatorName;
            Capacity = capacity;
            PosX = posX;
            PosY = posY;
        }

        /// <summary>
        /// Tries to build station from a list of fields.
        /// </summary>
        /// <param name="fields"></param>
        /// <param name="station"></param>
        /// <returns></returns>
        public static bool TryBuildFromCSV(string[] fields, [NotNullWhen(true), MaybeNullWhen(false)] out Station station)
        {
            CultureInfo culture = CultureInfo.InvariantCulture;

            //The imported csv can also have an unused fid field at the start, in this case we offset the fields by 1.
            int offset;
            
            //Check if field length is valid and set proper offset
            if (fields.Length == 12)
            {
                offset = 0;
            }
            else if (fields.Length == 13)
            {
                offset = 1;
            }
            else
            {
                station = null;
                return false;
            }

            //Check if all strings can be parsed and return if any of them fails
            if (
                !int.TryParse(fields[0 + offset], out int id)
                || !int.TryParse(fields[9 + offset], out int capacity)
                || !decimal.TryParse(fields[10 + offset], culture, out decimal x)
                || !decimal.TryParse(fields[11 + offset], culture, out decimal y)
                )
            {
                station = null;
                return false;
            }

            //Construct station from parsed data
            station = new Station(
                id,
                fields[1 + offset],
                fields[2 + offset],
                fields[3 + offset],
                fields[4 + offset],
                fields[5 + offset],
                fields[6 + offset],
                fields[7 + offset],
                fields[8 + offset],
                capacity,
                x,
                y
                );

            //Validate the object using the built in validator. I'm not sure how efficient this is but it
            //validates the objects the same way as Asp.NET APIs do so we don't have to maintain another method.
            ValidationContext vc = new ValidationContext(station);
            if(!Validator.TryValidateObject(station, vc, null, true))
            {
                station = null;
                return false;
            }

            return true;
        }
    }
}
