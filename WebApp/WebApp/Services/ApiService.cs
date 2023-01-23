using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR.Protocol;
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

namespace WebApp.Services
{
    public class ApiService
    {
        //https://www.aspnetmonsters.com/2016/08/2016-08-27-httpclientwrong/
        //Caching and reusing http client is apparently highly recommended.
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
        #endregion
    }
}
