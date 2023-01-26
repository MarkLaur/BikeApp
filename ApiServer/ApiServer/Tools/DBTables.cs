using ApiServer.Controllers;
using ApiServer.Models;
using MySql.Data.MySqlClient;
using System.Data;
using System.Reflection.PortableExecutable;

namespace ApiServer.Tools
{
    /// <summary>
    /// Contains database column name strings and mysql query builders.
    /// </summary>
    public static class DBTables
    {
        /// <summary>
        /// Contains database field name definition strings for the biketrips table.
        /// </summary>
        public static class BikeTrips
        {
            /// <summary>
            /// Query that gets first 30 elements from biketrips table.
            /// </summary>
            public const string BikeTripQuery = "SELECT *\r\nFROM `biketrips`\r\nLIMIT 0 , 30";

            public const string BikeTripQueryWithStationNames =
                "SELECT biketrips . * , " +                                 //Select bike trips
                $"departurestation.name AS {DepartureStationName}, " +         //Rename departure station name field so that it is unique
                $"returnstation.name AS {ReturnStationName}\r\n" +             //Rename return station name field so that it is unique
                "FROM `biketrips`\r\n" +
                $"LEFT JOIN bikestations AS departurestation ON biketrips.{DepartureStationID} = departurestation.id\r\n" +//Join departure station using renamed field
                $"LEFT JOIN bikestations AS returnstation ON biketrips.{DepartureStationID} = returnstation.id\r\n" +         //Join return station using renamed field
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

            public static string BuildBikeTripsFromStationQuery(int stationID)
            {
                //This will be built every time the method is called. The compiler might reduce the concatenation amounts a bit.
                return
                "SELECT biketrips . * , " +                                 //Select bike trips
                $"departurestation.name AS {DepartureStationName}, " +         //Rename departure station name field so that it is unique
                $"returnstation.name AS {ReturnStationName}\r\n" +             //Rename return station name field so that it is unique
                "FROM `biketrips`\r\n" +
                $"LEFT JOIN bikestations AS departurestation ON biketrips.{DepartureStationID} = departurestation.id\r\n" +//Join departure station using renamed field
                $"LEFT JOIN bikestations AS returnstation ON biketrips.{ReturnStationID} = returnstation.id\r\n" +         //Join return station using renamed field
                $"WHERE {DepartureStationID} = {stationID} OR {ReturnStationID} = {stationID}\r\n" +    //Select only trips that come from or end at this station.
                "LIMIT 0 , 30";
            }
        }

        public static class BikeStations
        {
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

                /*
                /// <summary>
                /// Builds an array of the columns defined here.
                /// </summary>
                /// <returns></returns>
                public static DataColumn[] BuildColumns()
                {
                    return new DataColumn[]
                    {
                    new DataColumn(ID, typeof(int)),
                    new DataColumn(NameFin, typeof(string)),
                    new DataColumn(NameSwe, typeof(string)),
                    new DataColumn(Name, typeof(string)),
                    new DataColumn(AddressFin, typeof(string)),
                    new DataColumn(AddressSwe, typeof(string)),
                    new DataColumn(CityFin, typeof(string)),
                    new DataColumn(CitySwe, typeof(string)),
                    new DataColumn(Operator, typeof(string)),
                    new DataColumn(Capacity, typeof(int)),
                    new DataColumn(PosX, typeof(double)),
                    new DataColumn(PosY, typeof(double))
                    };
                }
                */
            }

            public const string TableName = "bikestations";

            public const string GetBikeStationsQuery =
                "SELECT bikestations . * " +
                "FROM `bikestations`\r\n" +
                "LIMIT 0 , 30";

            /// <summary>
            /// A parametized SQL query that inserts a station into bikestations table. Updates existing key on duplicate key.
            /// Contained parameters: @id, @namefin, @nameswe, @name, @addressfin, @addressswe, @cityfin, @cityswe, @operator, @capacity, @posx, @posy
            /// </summary>
            public const string InsertBikeStationQuery =
                "INSERT INTO bikestations\r\n" +
                "VALUES (@id, @namefin, @nameswe, @name, @addressfin, @addressswe, @cityfin, @cityswe, @operator, @capacity, @posx, @posy)\r\n" +
                $"ON DUPLICATE KEY UPDATE {Columns.NameFin} = @namefin, {Columns.NameSwe} = @nameswe, {Columns.Name} = @name, " +
                $"{Columns.AddressFin} = @addressfin, {Columns.AddressSwe} = @addressswe, {Columns.CityFin} = @cityfin, {Columns.CitySwe} = @cityswe, " +
                $"{Columns.Operator} = @operator, {Columns.Capacity} = @capacity, {Columns.PosX} = @posx, {Columns.PosY} = @posy;";

            public static string BuildBikeStationQuery(int stationID)
            {
                return
                    "SELECT * FROM `bikestations`\r\n" +
                    $"WHERE {Columns.ID} = {stationID}\r\n" +
                    "LIMIT 0 , 5"; //Limit max rows just in case
            }

            public static string BuildInsertQuery(IEnumerable<Station> stations)
            {
                MySqlDataAdapter query = new MySqlDataAdapter();
                throw new NotImplementedException();
            }

            /*
            public static DataTable BuildStationsTable(IEnumerable<Station> stations)
            {
                DataTable stationsTable = BikeStationTable.BuildTable();

                foreach (Station station in stations)
                {
                    DataRow row = stationsTable.NewRow();
                    row[BikeStationTable.Columns.ID] = station.ID;
                    row[BikeStationTable.Columns.NameFin] = station.NameFin;
                    row[BikeStationTable.Columns.NameSwe] = station.NameSwe;
                    row[BikeStationTable.Columns.Name] = station.Name;
                    row[BikeStationTable.Columns.AddressFin] = station.AddressFin;
                    row[BikeStationTable.Columns.AddressSwe] = station.AddressSwe;
                    row[BikeStationTable.Columns.CityFin] = station.CityFin;
                    row[BikeStationTable.Columns.CitySwe] = station.CitySwe;
                    row[BikeStationTable.Columns.Operator] = station.OperatorName;
                    row[BikeStationTable.Columns.Capacity] = station.Capacity;
                    row[BikeStationTable.Columns.PosX] = station.PosX;
                    row[BikeStationTable.Columns.PosY] = station.PosY;

                    stationsTable.Rows.Add(row);
                }

                return stationsTable;
            }

            /// <summary>
            /// Builds a datatable that matches the definitions here.
            /// </summary>
            /// <returns></returns>
            public static DataTable BuildTable()
            {
                DataTable table = new DataTable(TableName);
                table.Columns.AddRange(Columns.BuildColumns());

                return table;
            }
            */
        }
    }
}