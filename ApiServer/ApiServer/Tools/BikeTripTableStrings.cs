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
            "SELECT biketrips . * , " +                                 //Select bike trips
            "departurestation.name AS DepartureStationName, " +         //Rename departure station name field so that it is unique
            "returnstation.name AS ReturnStationName\r\n" +             //Rename return station name field so that it is unique
            "FROM `biketrips`\r\n" +
            "LEFT JOIN bikestations AS departurestation ON biketrips.departurestationid = departurestation.id\r\n" +//Join departure station using renamed field
            "LEFT JOIN bikestations AS returnstation ON biketrips.returnstationid = returnstation.id\r\n" +         //Join return station using renamed field
            "LIMIT 0 , 30";                                             //Limit to 30 elements

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

    public static class BikeStationTableStrings
    {
        public const string GetBikeStationsQuery =
            "SELECT bikestations . * " +
            "FROM `bikestations`\r\n" +
            "LIMIT 0 , 30";

        /// <summary>
        /// Contains name strings of table columns.
        /// </summary>
        public static class Columns
        {
            public const string ID = "ID";
            public const string NameFin = "NameFin";
            public const string NameSwe = "NameSwe";
            public const string Name = "Name";
            public const string AddressFin = "AddressFin";
            public const string AddressSwe = "AddressSwe";
            public const string CityFin = "CityFin";
            public const string CitySwe = "CitySwe";
            public const string Operator = "Operator";
            public const string Capacity = "Capacity";
            public const string PosX = "PosX";
            public const string PosY = "PosY";
        }
    }
}
