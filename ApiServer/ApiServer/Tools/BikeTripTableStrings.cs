using System.Reflection.PortableExecutable;

namespace ApiServer.Tools
{
    /// <summary>
    /// Contains database field name definition strings for the biketrips table.
    /// </summary>
    public static class BikeTripTableStrings
    {
        /// <summary>
        /// Query that gets first 30 elements from biketrips table.
        /// </summary>
        public const string BikeTripQuery = "SELECT *\r\nFROM `biketrips`\r\nLIMIT 0 , 30";

        //Column names of biketrips table.
        public const string ID = "ID";
        public const string Departure = "Departure";
        public const string Return = "Return";
        public const string DepartureStationID = "DepartureStationID";
        public const string ReturnStationID = "ReturnStationID";
        public const string Distance = "Distance";
        public const string Duration = "Duration";
    }
}
