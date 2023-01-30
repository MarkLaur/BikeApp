using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR.Protocol;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text.Json;
using WebApp.Models;
using WebApp.Pages;

namespace WebApp.Services
{
    public class ApiService
    {
        #region Static Implementation

        //https://www.aspnetmonsters.com/2016/08/2016-08-27-httpclientwrong/
        //Caching and reusing http client is apparently highly recommended.
        //Though this says that it doesn't really matter what we do because HttpClient manages reusing the important bit.
        //https://stackoverflow.com/questions/54597303/should-i-cache-and-reuse-httpclient-created-from-httpclientfactory
        //
        //This should get initialized only once and before first instance is created.
        //Here's some info about static member initialization in case this breaks.
        //https://stackoverflow.com/questions/1405709/what-is-the-static-variable-initialization-order-across-classes-in-c
        //
        //We could instead add the service as a singleton
        private static HttpClient client = new HttpClient();

        static ApiService()
        {
            //Initialize client, we don't have anything but json api requests for now so this is all set only once.
            client.BaseAddress = ApiDefinitions.ApiServer;
            client.DefaultRequestHeaders.Add(ApiDefinitions.ApiKeyHeaderName, ApiDefinitions.ApiKey);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        #endregion Static Implementation

        public IWebHostEnvironment WebHostEnvironment { get; }

        public ApiService(IWebHostEnvironment webHostEnvironment)
        {
            WebHostEnvironment = webHostEnvironment;
        }

        #region Public Methods
        #endregion

        #region Async Tasks

        /// <summary>
        /// Tries to get json string from given uri.
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public async Task<Stream> GetJson(Uri uri)
        {
            //Perform api request
            HttpResponseMessage response = await client.GetAsync(uri);
            if (!response.IsSuccessStatusCode)
            {
                throw new BadHttpRequestException($"Couldn't get data from api. ({response.StatusCode})");
            }

            return await response.Content.ReadAsStreamAsync();
        }

        /// <summary>
        /// Tries to find station with given id from api. Returns null if station cannot be found.
        /// Returns a (success, jsonStream) tuple. JsonStream is not null when success is true.
        /// </summary>
        /// <param name="stationID"></param>
        /// <returns></returns>
        /// <exception cref="BadHttpRequestException"></exception>
        public async Task<(bool, Stream?)> TryGetStationJson(int stationID)
        {
            //Perform api request
            HttpResponseMessage response = await client.GetAsync(ApiDefinitions.BuildBikeStationUri(stationID));
            if (!response.IsSuccessStatusCode)
            {
                throw new BadHttpRequestException($"Couldn't get data from api. ({response.StatusCode})");
            }

            if (response.Headers.TryGetValues("StationFound", out IEnumerable<string>? values)
                && values != null && values.First() == "true")
            {
                return (true, await response.Content.ReadAsStreamAsync());
            }
            else
            {
                return (false, default);
            }
        }

        /// <summary>
        /// Uploads given stations to api server.
        /// </summary>
        /// <param name="stations"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<HttpResponseMessage> UploadStations(IEnumerable<Station> stations)
        {
            HttpResponseMessage response = await client.PutAsJsonAsync(ApiDefinitions.BikeStationsUri, stations);
            return response;
        }

        public async Task<(HttpResponseMessage, TripInsertResult?)> UploadTrips(IEnumerable<BikeTrip> trips)
        {
            HttpResponseMessage response = await client.PutAsJsonAsync(ApiDefinitions.BikeTripsUri, trips);
            Stream responseContent = await response.Content.ReadAsStreamAsync();

            //Attempt to deserialize
            TripInsertResult? result = await JsonSerializer.DeserializeAsync<TripInsertResult>(responseContent,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return (response, result);
        }
        #endregion
    }
}
