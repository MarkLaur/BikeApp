using ApiServer.Controllers;
using ApiServer.Models;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI;
using System.Data;
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
        private const string connectionString = "Server=localhost; Port=3307; Database=bikeapp; Uid=root; Pwd=BikeAppUsbwpw;";
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

        private static BikeTripsWithStations GetTrips(string query)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                List<BikeTrip> tripList = new();
                Dictionary<int, Station> stations = new();

                using (MySqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = query;
                    using (MySqlDataReader reader = cmd.ExecuteReader())
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
                                reader.GetInt32(DBTables.BikeTrips.Columns.Distance),
                                reader.GetInt32(DBTables.BikeTrips.Columns.Duration)
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
                }

                BikeTripsWithStations dataSet = new BikeTripsWithStations(tripList, stations);
                return dataSet;
            }
        }

        public static BikeTripsWithStations GetTrips()
        {
            return GetTrips(DBTables.BikeTrips.BikeTripQueryWithStationNames);
        }

        public static BikeTripsWithStations GetTripsFromStation(int stationID)
        {
            return GetTrips(DBTables.BikeTrips.BuildBikeTripsFromStationQuery(stationID));
        }

        /// <summary>
        /// Gets stations from database
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Station> GetStations()
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                List<Station> stationList = new List<Station>();

                using (MySqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = DBTables.BikeStations.GetBikeStationsQuery;
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
        public static void InsertStations(IEnumerable<Station> stations)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                using (MySqlCommand cmd = conn.CreateCommand())
                using (MySqlTransaction transaction = conn.BeginTransaction())
                using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
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
        public static async Task<int> InsertBikeTrips(IEnumerable<BikeTrip> trips, BikeTripInsertMode mode)
        {
            if(mode == BikeTripInsertMode.InsertOrUpdate)
            {
                throw new NotImplementedException();
            }

            int badTrips = 0;

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                using (MySqlCommand cmd = conn.CreateCommand())
                using (MySqlTransaction transaction = conn.BeginTransaction())
                using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                {
                    cmd.CommandText = DBTables.BikeTrips.InsertBikeTripsWithoutIDQuery;

                    //This might not be the best way to do a massive amount of inserts but it works well enough for now.
                    foreach (BikeTrip trip in trips)
                    {
                        cmd.Parameters.Clear();

                        if (mode == BikeTripInsertMode.Insert && trip.ID != 0)
                        {
                            badTrips++;
                            continue;
                        }

                        //TODO: make sure AddWithValue() actually sanitizes everything
                        //AddWithValue() should sanitize inputs by escaping all dangerous characters.
                        cmd.Parameters.AddWithValue("@departuretime", trip.DepartureTime);
                        cmd.Parameters.AddWithValue("@returntime", trip.ReturnTime);
                        cmd.Parameters.AddWithValue("@departurestationid", trip.DepartureStationID);
                        cmd.Parameters.AddWithValue("@returnstationid", trip.ReturnStationID);
                        cmd.Parameters.AddWithValue("@distance", trip.Distance);
                        cmd.Parameters.AddWithValue("@duration", (int)trip.Duration.TotalSeconds);

                        cmd.ExecuteNonQuery();
                    }

                    await transaction.CommitAsync();
                }
            }

            return badTrips;
        }

        public static bool TryGetStation(int stationID, [NotNullWhen(true), MaybeNullWhen(false)] out Station station)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                using (MySqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = DBTables.BikeStations.BuildBikeStationQuery(stationID);
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
