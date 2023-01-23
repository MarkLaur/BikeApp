using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;

namespace WebApp.Services
{
    /// <summary>
    /// Contains hardcoded values that define api server resources.
    /// </summary>
    public static class ApiDefinitions
    {
        public readonly static string ApiKeyHeaderName = "apiKey";
        public readonly static string ApiKey = "1111";

        private readonly static string ApiServerIP = "https://localhost";
        private readonly static int ApiServerPort = 7170;

        private readonly static string StationInfoRoute = "StationInfo";
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
        /// Station trips search uri without station id in the route
        /// </summary>
        public static readonly Uri StationInfoBaseUri = new Uri(ApiServer, StationInfoRoute);
        /// <summary>
        /// Station search uri without station id in the route
        /// </summary>
        public static readonly Uri BikeStationBaseUri = new Uri(ApiServer, StationRoute);

        /// <summary>
        /// Builds and Uri with station ID in the route
        /// </summary>
        /// <param name="stationID"></param>
        /// <returns></returns>
        public static Uri BuildStationInfoUri(int stationID)
        {
            return new Uri(ApiServer, $"{StationInfoRoute}/{stationID}");
        }

        public static Uri BuildBikeStationUri(int stationID)
        {
            return new Uri(ApiServer, $"{StationRoute}/{stationID}");
        }
    }
}