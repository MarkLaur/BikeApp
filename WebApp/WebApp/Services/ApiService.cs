using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR.Protocol;
using Microsoft.Extensions.Primitives;
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
        /// Tries to get json string from given uri. Returns a (success, json) tuple.
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public async Task<string> GetJson(Uri uri)
        {
            //Perform api request
            HttpResponseMessage response = await client.GetAsync(uri);
            if (!response.IsSuccessStatusCode)
            {
                throw new BadHttpRequestException($"Couldn't get data from api. ({response.StatusCode})");
            }

            string json = await response.Content.ReadAsStringAsync();
            return json;
        }

        /// <summary>
        /// Tries to find station with given id from api. Returns null if station cannot be found.
        /// </summary>
        /// <param name="stationID"></param>
        /// <returns></returns>
        /// <exception cref="BadHttpRequestException"></exception>
        public async Task<string?> TryGetStationJson(int stationID)
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
                string json = await response.Content.ReadAsStringAsync();
                return json;
            }
            else
            {
                return null;
            }
        }

        public async Task<HttpResponse> UploadStations(IEnumerable<Station> stations)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
