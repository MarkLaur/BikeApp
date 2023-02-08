using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;

namespace BlazorWebApp.Services
{
    /// <summary>
    /// Contains hardcoded values that define api server resources.
    /// </summary>
    public static class ApiDefinitions
    {
        public readonly static string ApiKeyHeaderName = "apiKey";
        public readonly static string ApiKey = "1111";

        private readonly static string ApiServerIP = "http://localhost";
        private readonly static int ApiServerPort = 7170;

        private readonly static string StationRoute = "BikeStation";

        //The Uri class doesn't have any setters so they should be immutable.
        //If they aren't immutable then this could cause problems and we might have to construct them every time.

        /// <summary>
        /// Api server's base url
        /// </summary>
        public static readonly Uri ApiServer = new Uri($"{ApiServerIP}:{ApiServerPort}");
        /// <summary>
        /// Full path to bike trips api.
        /// </summary>
        public static readonly Uri BikeTripsUri = new Uri(ApiServer, "BikeTrips");
        public static readonly Uri BikeStationsUri = new Uri(ApiServer, "BikeStations");
        /// <summary>
        /// Station search uri without station id in the route
        /// </summary>
        public static readonly Uri BikeStationBaseUri = new Uri(ApiServer, StationRoute);

        public static Uri BuildBikeStationUri(int stationID)
        {
            return new Uri(ApiServer, $"{StationRoute}/{stationID}");
        }
    }
}