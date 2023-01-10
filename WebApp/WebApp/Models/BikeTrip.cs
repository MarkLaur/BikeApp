using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.Models
{
    public class BikeTrip
    {
        public string Name { get; private set; }
        public float Length { get; private set; }

        public BikeTrip(string name, float length)
        {
            Name = name;
            Length = length;
        }
    }
}
