namespace WebApp.Models
{
    /// <summary>
    /// Contains hardcoded values that define api server resources.
    /// </summary>
    public static class ApiDefinitions
    {
        private readonly static string ApiServerIP = "https://localhost";
        private readonly static int ApiServerPort = 7170;

        /// <summary>
        /// Api server's base url
        /// </summary>
        public static Uri ApiServer { get; private set; }
        /// <summary>
        /// Full path to bike trips api.
        /// </summary>
        public static Uri BikeTripsUri { get; private set; }
        public static Uri BikeStationsUri { get; private set; }

        static ApiDefinitions()
        {
            ApiServer = new Uri($"{ApiServerIP}:{ApiServerPort}");
            BikeTripsUri = new Uri(ApiServer, "BikeTrips");
            BikeStationsUri = new Uri(ApiServer, "BikeStations");
        }
    }
}