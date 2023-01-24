﻿using ApiServer.Tools;
using MySql.Data.MySqlClient;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ApiServer.Models
{
    //This could be a class, but then we would have to make sure that there is only one instance of each station so that
    //reference comparisons don't break. I might turn this into a class and make a station dictionary or something at some point.
    public struct Station
    {
        [Required, Range(0, int.MaxValue)]
        public int ID { get; private set; }
        /// <summary>
        /// Finnish name
        /// </summary>
        [Required, StringLength(100)]
        public string NameFin { get; private set; }
        /// <summary>
        /// Swedish name
        /// </summary>
        [Required, StringLength(100)]
        public string NameSwe { get; private set; }
        /// <summary>
        /// English name
        /// </summary>
        [Required, StringLength(100)]
        public string Name { get; private set; }

        [Required, StringLength(100)]
        public string AddressFin { get; private set; }

        [Required, StringLength(100)]
        public string AddressSwe { get; private set; }

        [Required, StringLength(100)]
        public string CityFin { get; private set; }

        [Required, StringLength(100)]
        public string CitySwe { get; private set; }

        [Required, StringLength(100)]
        public string OperatorName { get; private set; }

        [Required, Range(0, int.MaxValue)]
        public int Capacity { get; private set; }

        [Required]
        public decimal PosX { get; private set; }

        [Required]
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
                reader.GetInt32(BikeStations.Columns.ID),
                reader.GetString(BikeStations.Columns.NameFin),
                reader.GetString(BikeStations.Columns.NameSwe),
                reader.GetString(BikeStations.Columns.Name),
                reader.GetString(BikeStations.Columns.AddressFin),
                reader.GetString(BikeStations.Columns.AddressSwe),
                reader.GetString(BikeStations.Columns.CityFin),
                reader.GetString(BikeStations.Columns.CitySwe),
                reader.GetString(BikeStations.Columns.Operator),
                reader.GetInt32(BikeStations.Columns.Capacity),
                reader.GetDecimal(BikeStations.Columns.PosX),
                reader.GetDecimal(BikeStations.Columns.PosY)
                )
        { }
    }
}
