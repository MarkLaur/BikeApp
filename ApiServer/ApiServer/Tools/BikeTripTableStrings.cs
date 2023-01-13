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

        public const string BikeTripQueryWithStationNames = 
            "SELECT biketrips . * , " +
            "departurestation.name AS DepartureStationName, " +
            "returnstation.name AS ReturnStationName\r\n" +
            "FROM `biketrips`\r\n" +
            "LEFT JOIN bikestations AS departurestation ON biketrips.departurestationid = departurestation.id\r\n" +
            "LEFT JOIN bikestations AS returnstation ON biketrips.returnstationid = returnstation.id\r\n" +
            "LIMIT 0 , 30";

        //Column names of biketrips table.
        public const string ID = "ID";
        public const string Departure = "DepartureTime";
        public const string Return = "ReturnTime";
        public const string DepartureStationID = "DepartureStationID";
        public const string ReturnStationID = "ReturnStationID";
        public const string Distance = "Distance";
        public const string Duration = "Duration";
        public const string DepartureStationName = "DepartureStationName";
        public const string ReturnStationName = "ReturnStationName";
    }
}
