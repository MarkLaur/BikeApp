using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApp.Models;

namespace WebApp.Services
{
    public class BikeTripService
    {
        public IWebHostEnvironment WebHostEnvironment { get; }

        private Uri BikeTripsApi { get; set; }

        public BikeTripService(IWebHostEnvironment webHostEnvironment)
        {
            WebHostEnvironment = webHostEnvironment;

            //TODO: create api server and set correct url
            //TODO: move ip and port definition to some global config file
            BikeTripsApi = new Uri("localhost:555/BikeTrips");
        }

        public IEnumerable<BikeTrip> GetBikeTrips()
        {
            //TODO: implement api server json query
            //TOOD: implement json deserialization
            //TODO: implement pagination

            return new List<BikeTrip> { new BikeTrip("Trip1"), new BikeTrip("Trip2") };
        }
    }
}
