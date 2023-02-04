using ApiServer.Controllers;
using ApiServer.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace ApiServer.Tools
{
    public enum DBConnectionTestResult
    {
        /// <summary>
        /// Main connection test was succesful.
        /// </summary>
        Success,
        /// <summary>
        /// Main connection test failed but default connection test was succesful.
        /// </summary>
        DefaultConnectionSuccess,
        /// <summary>
        /// All connection tests failed.
        /// </summary>
        Failure,
    }

    public enum BikeTripInsertMode
    {
        /// <summary>
        /// Inserts trips to database, or updates them if trip has an id and a matching entry already exists.
        /// </summary>
        InsertOrUpdate,
        /// <summary>
        /// Only inserts trips. Trips cannot have id fields set.
        /// </summary>
        Insert
    }

    public static class DatabaseHandler
    {
        //TODO: create a user for database read and update only
        private const string connectionString = "Server=localhost; Port=3307; Database=bikeapp; Uid=root; Pwd=BikeAppUsbwpw; Convert Zero Datetime=True";
        private const string uswbDefaultConnectionString = "Server=localhost; Port=3307; Database=test; Uid=root; Pwd=usbw;";

        public static DBConnectionTestResult TestConnection()
        {
            using (MySqlConnection conn = new MySqlConnection())
            {
                //Try to open main connection
                try
                {
                    conn.ConnectionString = connectionString;
                    conn.Open();
                    return DBConnectionTestResult.Success;
                }
                catch { }

                //Try to open connection with default usbw settings
                try
                {
                    conn.ConnectionString = uswbDefaultConnectionString;
                    conn.Open();
                    return DBConnectionTestResult.DefaultConnectionSuccess;
                }
                catch { }

                //Return failure if both failed
                return DBConnectionTestResult.Failure;
            }
        }

        /// <summary>
        /// Tries to execute given sql command and to construct a BikeTripsWithStations from the data.
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        private static async Task<BikeTripsWithStations> GetTrips(MySqlCommand cmd)
        {
            List<BikeTrip> tripList = new();
            Dictionary<int, Station> stations = new();

            using (DbDataReader reader = await cmd.ExecuteReaderAsync())
            {
                //TODO: check if all needed columns exist

                //Read() return false when end of query is reached
                while (reader.Read())
                {
                    //We don't have any nullable columns so null check shouldn't be needed.

                    //Create a new BikeTrip with data from query and add it to trip list
                    BikeTrip trip = new BikeTrip(
                        reader.GetInt32(DBTables.BikeTrips.Columns.ID),
                        reader.GetDateTime(DBTables.BikeTrips.Columns.Departure),
                        reader.GetDateTime(DBTables.BikeTrips.Columns.Return),
                        reader.GetInt32(DBTables.BikeTrips.Columns.DepartureStationID),
                        reader.GetInt32(DBTables.BikeTrips.Columns.ReturnStationID),
                        reader.GetInt32(DBTables.BikeTrips.Columns.Distance)
                        );

                    //Check if departure station exists
                    if (!stations.ContainsKey(trip.DepartureStationID))
                    {
                        //Build a new station and add it to the dictionary
                        stations.Add(trip.DepartureStationID, new Station(
                            trip.DepartureStationID,
                            reader.GetString(DBTables.BikeTrips.Columns.DepartureStationName),
                            "TODO", "TODO", "TODO", "TODO", "TODO", "TODO", "TODO", 0, 0, 0 //TODO: get these aswell
                            ));
                    }

                    //Check if return station exists
                    if (trip.ReturnStationID != trip.DepartureStationID //We already checked if this id exists in there
                        && !stations.ContainsKey(trip.ReturnStationID))
                    {
                        //Build a new station and add it to the dictionary
                        stations.Add(trip.ReturnStationID, new Station(
                            trip.ReturnStationID,
                            reader.GetString(DBTables.BikeTrips.Columns.ReturnStationName),
                            "TODO", "TODO", "TODO", "TODO", "TODO", "TODO", "TODO", 0, 0, 0 //TODO: get these aswell
                            ));
                    }

                    tripList.Add(trip);
                }
            }

            return new BikeTripsWithStations(tripList, stations);
        }

        /// <summary>
        /// Returns the amount of trips from given station. Returns the total amount of trips if station isn't specified.
        /// </summary>
        /// <param name="stationID"></param>
        /// <returns></returns>
        public static async Task<(bool, int)> TryGetTripCount(int? stationID = null)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                await conn.OpenAsync();

                using (MySqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = DBTables.BikeTrips.RowCountQuery;

                    if (stationID.HasValue)
                    {
                        cmd.CommandText += DBTables.BikeTrips.WhereStationIdClause;
                        cmd.Parameters.AddWithValue("@stationID", stationID);
                    }

                    using (DbDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        if (reader.Read())
                        {
                            return (true, reader.GetInt32("rows"));
                        }
                        else
                        {
                            return (false, 0);
                        }
                    }
                }
            }
        }
        
        /// <summary>
        /// Gets all trips from a given page, limited by items per page.
        /// </summary>
        /// <param name="page"></param>
        /// <param name="itemsPerPage"></param>
        /// <returns></returns>
        public static async Task<BikeTripsWithStations> GetTrips(int page, int itemsPerPage)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                await conn.OpenAsync();

                using (MySqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = DBTables.BikeTrips.GetBikeTripsWithStationsQuery;
                    cmd.Parameters.AddWithValue("@startIndex", page * itemsPerPage);
                    cmd.Parameters.AddWithValue("@limit", itemsPerPage);

                    return await GetTrips(cmd);
                }
            }
        }

        /// <summary>
        /// Gets all trips from given station, limited by page and itemsPerPage.
        /// </summary>
        /// <param name="stationID"></param>
        /// <param name="page"></param>
        /// <param name="itemsPerPage"></param>
        /// <returns></returns>
        public static async Task<BikeTripsWithStations> GetTrips(int stationID, int page, int itemsPerPage)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                await conn.OpenAsync();

                using (MySqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = DBTables.BikeTrips.GetBikeTripsFromStationQuery;
                    cmd.Parameters.AddWithValue("@stationid", stationID);
                    cmd.Parameters.AddWithValue("@startIndex", page * itemsPerPage);
                    cmd.Parameters.AddWithValue("@limit", itemsPerPage);

                    return await GetTrips(cmd);
                }
            }
        }

        /// <summary>
        /// Gets the amount of stations that fit the passed data. Returns a (success, rowCount) tuple.
        /// </summary>
        /// <returns></returns>
        public static async Task<(bool, int)> TryGetStationCount(string stationName)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                await conn.OpenAsync();

                using (MySqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = DBTables.BikeStations.RowCountQuery;
                    if (!string.IsNullOrWhiteSpace(stationName)) cmd.CommandText += DBTables.BikeStations.WhereNameClause;

                    //AddWithValue should sanitize the string
                    //TODO: make sure it actually does
                    cmd.Parameters.AddWithValue("@stationName", $"%{stationName}%");

                    using (DbDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        if (reader.Read())
                        {
                            return (true, reader.GetInt32("rows"));
                        }
                        else
                        {
                            return (false, 0);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets stations from database. Page is zero indexed.
        /// </summary>
        /// <returns></returns>
        public static async Task<ICollection<Station>> GetStations(string stationName, int page, int itemsPerPage)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                await conn.OpenAsync();

                List<Station> stationList = new List<Station>();

                using (MySqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = DBTables.BikeStations.SelectAllQuery;
                    if (!string.IsNullOrWhiteSpace(stationName)) cmd.CommandText += DBTables.BikeStations.WhereNameClause;
                    cmd.CommandText += DBTables.LimitClause;

                    //AddWithValue should sanitize the string
                    //TODO: make sure it actually does
                    cmd.Parameters.AddWithValue("@stationName", $"%{stationName}%");
                    cmd.Parameters.AddWithValue("@startIndex", page * itemsPerPage);
                    cmd.Parameters.AddWithValue("@limit", itemsPerPage);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        //TODO: check if all needed columns exist

                        //Read() return false when end of query is reached
                        while (reader.Read())
                        {
                            //We don't have any nullable columns so null check shouldn't be needed.

                            //Create a new Station with data from query and add it to trip list
                            stationList.Add(new Station(reader));
                        }
                    }
                }

                return stationList;
            }
        }

        /// <summary>
        /// Puts stations into database
        /// </summary>
        /// <param name="stations"></param>
        /// <exception cref="NotImplementedException"></exception>
        public static void InsertStations(ICollection<Station> stations)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                //TODO: Do transactions asnychronously in batches of 100 inserts each

                using (MySqlCommand cmd = conn.CreateCommand())
                using (MySqlTransaction transaction = conn.BeginTransaction())
                {
                    cmd.CommandText = DBTables.BikeStations.InsertBikeStationQuery;

                    //This might not be the best way to do a massive amount of inserts but it works well enough for now.
                    foreach (Station station in stations)
                    {
                        cmd.Parameters.Clear();

                        //TODO: make sure AddWithValue() actually sanitizes everything
                        //AddWithValue() should sanitize inputs by escaping all dangerous characters.
                        cmd.Parameters.AddWithValue("@id", station.ID);
                        cmd.Parameters.AddWithValue("@namefin", station.NameFin);
                        cmd.Parameters.AddWithValue("@nameswe", station.NameSwe);
                        cmd.Parameters.AddWithValue("@name", station.Name);
                        cmd.Parameters.AddWithValue("@addressfin", station.AddressFin);
                        cmd.Parameters.AddWithValue("@addressswe", station.AddressSwe);
                        cmd.Parameters.AddWithValue("@cityfin", station.CityFin);
                        cmd.Parameters.AddWithValue("@cityswe", station.CitySwe);
                        cmd.Parameters.AddWithValue("@operator", station.OperatorName);
                        cmd.Parameters.AddWithValue("@capacity", station.Capacity);
                        cmd.Parameters.AddWithValue("@posx", station.PosX);
                        cmd.Parameters.AddWithValue("@posy", station.PosY);

                        cmd.ExecuteNonQuery();
                    }

                    //There's also CommitAsync(), but we have to send a status code back anyway so we might as well wait for this to complete.
                    transaction.Commit();
                }
            }
        }

        /// <summary>
        /// Puts stations into database. Returns the amount of incorrect trips
        /// </summary>
        /// <param name="trips"></param>
        /// <param name="requireID"></param>
        public static async Task<TripInsertResult> InsertBikeTrips(ICollection<BikeTrip> trips, BikeTripInsertMode mode, bool createMissingStations, ILogger logger)
        {
            if(mode == BikeTripInsertMode.InsertOrUpdate || createMissingStations == true)
            {
                throw new NotImplementedException();
            }

            int insertedTrips = 0;
            int invalidTrips = 0;
            int missingStationInstances = 0;
            int otherInvalidData = 0;

            HashSet<int> uniqueStations = new();
            HashSet<int> missingStations = new();

            //Build a list of stations referenced by trips
            logger.LogInformation("Building relevant station hash set.");
            foreach(BikeTrip trip in trips)
            {
                uniqueStations.Add(trip.DepartureStationID);
                if(trip.ReturnStationID != trip.DepartureStationID) uniqueStations.Add(trip.ReturnStationID);
            }

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                //Check which of those stations can be found in database so that only try to insert valid trips
                logger.LogInformation($"Fetching {uniqueStations.Count} relevant stations from db.");
                using (MySqlCommand cmd = conn.CreateCommand())
                {
                    //Get all stations that exist in database
                    cmd.CommandText = DBTables.BikeStations.GetBikeStationQuery;
                    foreach (int station in uniqueStations)
                    {
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@id", station);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (!reader.Read())
                            {
                                missingStations.Add(station);
                            }
                        }
                    }

                    //TODO: Do transactions asnychronously in batches of 100 inserts each

                    //Insert all valid trips into database
                    using (MySqlTransaction transaction = conn.BeginTransaction())
                    {
                        cmd.CommandText = DBTables.BikeTrips.InsertBikeTripsWithoutIDQuery;

                        int total = trips.Count;
                        int current = 0;

                        logger.LogInformation($"Processing {total} trips.");

                        foreach (BikeTrip trip in trips)
                        {
                            current++;
                            if(current % 10000 == 0) logger.LogInformation($"Processing trips and inserting valid ones to db. ({current}/{total})");

                            bool invalidTrip = false;

                            //Check if station is missing
                            if (missingStations.Contains(trip.DepartureStationID))
                            {
                                invalidTrip = true;
                                missingStationInstances++;
                            }

                            //Check if other station is missing separately to correcly tick the tracker
                            if (missingStations.Contains(trip.ReturnStationID))
                            {
                                invalidTrip = true;
                                missingStationInstances++;
                            }

                            //Check if data is valid for the insert mode
                            if (mode == BikeTripInsertMode.Insert && trip.ID != 0)
                            {
                                invalidTrip = true;
                                otherInvalidData++;
                            }

                            if (invalidTrip)
                            {
                                invalidTrips++;
                                continue;
                            }

                            cmd.Parameters.Clear();

                            //TODO: make sure AddWithValue() actually sanitizes everything
                            //AddWithValue() should sanitize inputs by escaping all dangerous characters.
                            cmd.Parameters.AddWithValue("@departuretime", trip.DepartureTime);
                            cmd.Parameters.AddWithValue("@returntime", trip.ReturnTime);
                            cmd.Parameters.AddWithValue("@departurestationid", trip.DepartureStationID);
                            cmd.Parameters.AddWithValue("@returnstationid", trip.ReturnStationID);
                            cmd.Parameters.AddWithValue("@distance", trip.Distance);

                            cmd.ExecuteNonQuery();
                            insertedTrips++;
                        }

                        await transaction.CommitAsync();
                    }
                }
            }

            TripInsertResult result = new TripInsertResult(insertedTrips, invalidTrips, missingStationInstances, missingStations.Count, otherInvalidData);
            logger.LogInformation($"Trip processing finished. Result: {result}");

            return result;
        }

        public static bool TryGetStation(int stationID, [NotNullWhen(true), MaybeNullWhen(false)] out Station station)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                using (MySqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = DBTables.BikeStations.GetBikeStationQuery;
                    cmd.Parameters.AddWithValue("@id", stationID);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        //TODO: check if all needed columns exist
                        
                        //Read() return false when end of query is reached
                        if (reader.Read())
                        {
                            //We don't have any nullable columns so null check shouldn't be needed.
                            station = new Station(reader);
                            return true;
                        }
                        else
                        {
                            station = default;
                            return false;
                        }
                    }
                }
            }
        }
    }
}
