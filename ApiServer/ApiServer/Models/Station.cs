using ApiServer.Tools;
using MySql.Data.MySqlClient;
using System.Text.Json.Serialization;

namespace ApiServer.Models
{
    //This could be a class, but then we would have to make sure that there is only one instance of each station so that
    //reference comparisons don't break. I might turn this into a class and make a station dictionary or something at some point.
    public struct Station
    {
        public int ID { get; private set; }
        /// <summary>
        /// Finnish name
        /// </summary>
        public string NameFin { get; private set; }
        /// <summary>
        /// Swedish name
        /// </summary>
        public string NameSwe { get; private set; }
        /// <summary>
        /// English name
        /// </summary>
        public string Name { get; private set; }
        public string AddressFin { get; private set; }
        public string AddressSwe { get; private set; }
        public string CityFin { get; private set; }
        public string CitySwe { get; private set; }
        public string OperatorName { get; private set; }
        public int Capacity { get; private set; }
        public decimal PosX { get; private set; }
        public decimal PosY { get; private set; }

        [JsonConstructor] //JsonSerializer can't serialize struct by default so the constructor has to be marked with this.
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
        /// Constructs the station from a MySqlDataReader. Make sure the reader has corretly formatted data as this doesn't validate anything.
        /// </summary>
        /// <param name="reader"></param>
        public Station(MySqlDataReader reader) :
            //Call the other constructor
            //We don't have any nullable columns so null check shouldn't be needed.
            this(
                reader.GetInt32(BikeStationTableStrings.Columns.ID),
                reader.GetString(BikeStationTableStrings.Columns.NameFin),
                reader.GetString(BikeStationTableStrings.Columns.NameSwe),
                reader.GetString(BikeStationTableStrings.Columns.Name),
                reader.GetString(BikeStationTableStrings.Columns.AddressFin),
                reader.GetString(BikeStationTableStrings.Columns.AddressSwe),
                reader.GetString(BikeStationTableStrings.Columns.CityFin),
                reader.GetString(BikeStationTableStrings.Columns.CitySwe),
                reader.GetString(BikeStationTableStrings.Columns.Operator),
                reader.GetInt32(BikeStationTableStrings.Columns.Capacity),
                reader.GetDecimal(BikeStationTableStrings.Columns.PosX),
                reader.GetDecimal(BikeStationTableStrings.Columns.PosY)
                )
        { }
    }
}
