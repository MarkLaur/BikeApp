using System.Text.Json.Serialization;

namespace WebApp.Models
{
    //This could be a class, but then we would have to make sure that there is only one instance of each station so that
    //reference comparisons don't break. I might turn this into a class and make a station dictionary or something at some point.
    public struct Station
    {
        //public bool Valid => ID != -1;

        public int ID { get; private set; } = -1; //Default to invalid value, this is set to a valid value in constructor
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
        public string Capacity { get; private set; }
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
            string capacity,
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
    }
}
