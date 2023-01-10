namespace WebApp.Models
{
    public static class ApiDefinitions
    {
        private readonly static string ApiServerIP = "https://localhost";
        private readonly static int ApiServerPort = 7170;

        public static Uri ApiServer { get; private set; }
        public static Uri BikeTripsUri { get; private set; }

        static ApiDefinitions()
        {
            ApiServer = new Uri($"{ApiServerIP}:{ApiServerPort}");
            BikeTripsUri = new Uri(ApiServer, "BikeTrips");
        }
    }
}