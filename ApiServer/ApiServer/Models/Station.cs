using ApiServer.Tools;
using MySql.Data.MySqlClient;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ApiServer.Models
{
    public class Station
    {
        [Required, Range(1, int.MaxValue)]
        public int ID { get; private set; } = -1;
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

        [Range(1, int.MaxValue)]
        public int Capacity { get; private set; }

        [Required]
        public decimal? PosX { get; private set; } = decimal.MinValue; //Has to be nullable to make the Required attribute work

        [Required]
        public decimal? PosY { get; private set; } = decimal.MinValue; //Has to be nullable to make the Required attribute work

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
            decimal? posX,
            decimal? posY
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
        /// Constructs the station from a MySqlDataReader. Make sure the reader has corretly formatted data as this doesn't validate anything.
        /// </summary>
        /// <param name="reader"></param>
        public Station(MySqlDataReader reader) :
            //Call the other constructor
            //We don't have any nullable columns so null check shouldn't be needed.
            this(
                reader.GetInt32(DBTables.BikeStations.Columns.ID),
                reader.GetString(DBTables.BikeStations.Columns.NameFin),
                reader.GetString(DBTables.BikeStations.Columns.NameSwe),
                reader.GetString(DBTables.BikeStations.Columns.Name),
                reader.GetString(DBTables.BikeStations.Columns.AddressFin),
                reader.GetString(DBTables.BikeStations.Columns.AddressSwe),
                reader.GetString(DBTables.BikeStations.Columns.CityFin),
                reader.GetString(DBTables.BikeStations.Columns.CitySwe),
                reader.GetString(DBTables.BikeStations.Columns.Operator),
                reader.GetInt32(DBTables.BikeStations.Columns.Capacity),
                reader.GetDecimal(DBTables.BikeStations.Columns.PosX),
                reader.GetDecimal(DBTables.BikeStations.Columns.PosY)
                )
        { }
    }
}
