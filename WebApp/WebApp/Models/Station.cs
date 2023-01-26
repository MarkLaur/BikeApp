using System.ComponentModel.DataAnnotations;
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
    }
}
