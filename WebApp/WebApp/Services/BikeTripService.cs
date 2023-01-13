using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Formats.Asn1;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using WebApp.Models;

namespace WebApp.Services
{
    public class BikeTripService
    {
        public IWebHostEnvironment WebHostEnvironment { get; }

        public BikeTripService(IWebHostEnvironment webHostEnvironment)
        {
            WebHostEnvironment = webHostEnvironment;
        }

        public IEnumerable<BikeTrip> GetBikeTrips()
        {
            //TODO: implement pagination

            //TODO: this probably blocks the thread really bad, fix this
            Task<IEnumerable<BikeTrip>> task = GetTripsAsync();
            var result = task.Result;

            return result;
        }

        public async Task<IEnumerable<BikeTrip>> GetTripsAsync()
        {
            //TODO: create some kind of api service for this stuff

            //Initialize client
            //TODO: cache and reuse HttpClient
            HttpClient client = new HttpClient();
            client.BaseAddress = ApiDefinitions.BikeTripsUri;
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //Perform api request
            HttpResponseMessage response = await client.GetAsync("");
            if (!response.IsSuccessStatusCode)
            {
                //Default to an empty array.
                //TODO: return a failure error code and tell the user that api broke
                return new BikeTrip[0];
            }

            Stream jsonStream = await response.Content.ReadAsStreamAsync();
            IEnumerable<BikeTrip>? trips = await JsonSerializer.DeserializeAsync<BikeTrip[]>(jsonStream,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            //TODO: fix nested object deserialization
            //The built in json deserializer cant deserialize the nested struct for some reason. Consider using Newtonsoft.Json
            /*
            List<BikeTrip> tripsList = trips.ToList();
            Station station = tripsList[0].DepartureStation;
            */

            if (trips == null) return new BikeTrip[0];
            else return trips;
        }
    }
}
