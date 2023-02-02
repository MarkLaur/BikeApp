
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
            /// Contains name strings of table columns.
            /// </summary>
            public static class Columns
            {
                public const string ID = "ID";
                public const string Departure = "DepartureTime";
                public const string Return = "ReturnTime";
                public const string DepartureStationID = "DepartureStationID";
                public const string ReturnStationID = "ReturnStationID";
                public const string Distance = "Distance";
                public const string DepartureStationName = "DepartureStationName";
                public const string ReturnStationName = "ReturnStationName";
            }

            public const string TableName = "biketrips";

            public const string RowCountQuery = $"SELECT COUNT(*) rows FROM {TableName}\n";

            /// <summary>
            /// Parametrized partial query that adds a where clause that limits result to ones where station id matches.
            /// Contained parameters: @stationID
            /// </summary>
            public const string WhereStationIdClause =
                $"WHERE {Columns.DepartureStationID} = @stationID\n" +
                $"OR {Columns.ReturnStationID} = @stationID\n";

            /// <summary>
            /// Parametrized query that gets bike trips with joined station data. Contained parameters: @startIndex, @limit
            /// </summary>
            public const string GetBikeTripsWithStationsQuery =
                $"SELECT {TableName} . * , " +                                  //Select bike trips
                $"departurestation.name AS {Columns.DepartureStationName}, " +  //Rename departure station name field so that it is unique
                $"returnstation.name AS {Columns.ReturnStationName}\r\n" +      //Rename return station name field so that it is unique
                $"FROM `{TableName}`\r\n" +
                $"LEFT JOIN bikestations AS departurestation ON {TableName}.{Columns.DepartureStationID} = departurestation.id\r\n" +//Join departure station using renamed field
                $"LEFT JOIN bikestations AS returnstation ON {TableName}.{Columns.ReturnStationID} = returnstation.id\r\n" +         //Join return station using renamed field
                "LIMIT @startIndex , @limit";

            /// <summary>
            /// Parametrized query that inserts a bike trip without an id field (no overwrite).
            /// Contained parameters: @departuretime, @returntime, @departurestationid, @returnstationid, @distance
            /// </summary>
            public const string InsertBikeTripsWithoutIDQuery =
                $"INSERT INTO biketrips ({Columns.Departure}, {Columns.Return}, {Columns.DepartureStationID}, " +
                $"{Columns.ReturnStationID}, {Columns.Distance})\r\n" +
                "VALUES (@departuretime, @returntime, @departurestationid, @returnstationid, @distance)";

            /// <summary>
            /// Parametrized that inserts or replaces the element in DB.
            /// Contained parameters:
            /// //TODO: implement this
            /// </summary>
            public const string InsertOrUpdateBikeTripsQuery = "";

            /// <summary>
            /// Parametrized query that gets bike trips departing or returning to a station.
            /// Contained parameters: @stationid, @startIndex, @limit
            /// </summary>
            public const string GetBikeTripsFromStationQuery =
                "SELECT biketrips . * , " +                                 //Select bike trips
                $"departurestation.name AS {Columns.DepartureStationName}, " +         //Rename departure station name field so that it is unique
                $"returnstation.name AS {Columns.ReturnStationName}\r\n" +             //Rename return station name field so that it is unique
                "FROM `biketrips`\r\n" +
                $"LEFT JOIN bikestations AS departurestation ON biketrips.{Columns.DepartureStationID} = departurestation.id\r\n" +//Join departure station using renamed field
                $"LEFT JOIN bikestations AS returnstation ON biketrips.{Columns.ReturnStationID} = returnstation.id\r\n" +         //Join return station using renamed field
                $"WHERE {Columns.DepartureStationID} = @stationid OR {Columns.ReturnStationID} = @stationid\r\n" +    //Select only trips that come from or end at this station.
                "LIMIT @startIndex , @limit";
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
            }

            public const string TableName = "bikestations";

            /// <summary>
            /// Query that gets the amount of rows that match the data.
            /// </summary>
            public const string RowCountQuery = $"SELECT COUNT(*) rows FROM {TableName}\n";

            /// <summary>
            /// Query that gets all bike stations. Doesn't contain a limit clause!
            /// </summary>
            public const string SelectAllQuery = $"SELECT * FROM `{TableName}`\n";

            /// <summary>
            /// Parametrized partial query that adds a where clause that limits result to ones where station name matches.
            /// Contained parameters: @stationName
            /// </summary>
            public const string WhereNameClause =
                $"WHERE {Columns.NameFin} LIKE @stationName\n" +
                $"OR {Columns.NameSwe} LIKE @stationName\n" +
                $"OR {Columns.Name} LIKE @stationName\n";

            /// <summary>
            /// A parametized SQL query that inserts a station into bikestations table. Updates existing key on duplicate key.
            /// Contained parameters: @id, @namefin, @nameswe, @name, @addressfin, @addressswe, @cityfin, @cityswe, @operator, @capacity, @posx, @posy
            /// </summary>
            public const string InsertBikeStationQuery =
                $"INSERT INTO {TableName}\r\n" +
                "VALUES (@id, @namefin, @nameswe, @name, @addressfin, @addressswe, @cityfin, @cityswe, @operator, @capacity, @posx, @posy)\r\n" +
                $"ON DUPLICATE KEY UPDATE {Columns.NameFin} = @namefin, {Columns.NameSwe} = @nameswe, {Columns.Name} = @name, " +
                $"{Columns.AddressFin} = @addressfin, {Columns.AddressSwe} = @addressswe, {Columns.CityFin} = @cityfin, {Columns.CitySwe} = @cityswe, " +
                $"{Columns.Operator} = @operator, {Columns.Capacity} = @capacity, {Columns.PosX} = @posx, {Columns.PosY} = @posy;";

            /// <summary>
            /// Parametrized query that gets a bike station from database. Contained parameters: @id
            /// </summary>
            public const string GetBikeStationQuery =
                    $"SELECT * FROM `{TableName}`\r\n" +
                    $"WHERE {Columns.ID} = @id\r\n" +
                    "LIMIT 1";
        }

        /// <summary>
        /// Parametrized partial query that adds a where clause that limits result to ones where station name matches.
        /// Contained parameters: @startIndex, @limit
        /// </summary>
        public const string LimitClause = "LIMIT @startIndex , @limit\n";
    }
}