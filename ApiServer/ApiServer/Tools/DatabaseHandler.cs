using ApiServer.Controllers;
using ApiServer.Models;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI;
using System.Data;
using System.Diagnostics;

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

        private static IEnumerable<BikeTrip> GetTrips(string query)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                List<BikeTrip> tripList = new List<BikeTrip>();

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
                            tripList.Add(new BikeTrip(
                                reader.GetInt32(DBTables.BikeTrips.ID),
                                reader.GetDateTime(DBTables.BikeTrips.Departure),
                                reader.GetDateTime(DBTables.BikeTrips.Return),
                                reader.GetInt32(DBTables.BikeTrips.DepartureStationID),
                                reader.GetString(DBTables.BikeTrips.DepartureStationName),
                                reader.GetInt32(DBTables.BikeTrips.ReturnStationID),
                                reader.GetString(DBTables.BikeTrips.ReturnStationName),
                                reader.GetInt32(DBTables.BikeTrips.Distance),
                                new TimeSpan(0, 0, reader.GetInt32(DBTables.BikeTrips.Duration))
                                ));
                        }
                    }
                }

                return tripList;
            }
        }

        public static IEnumerable<BikeTrip> GetTrips()
        {
            return GetTrips(DBTables.BikeTrips.BikeTripQueryWithStationNames);
        }

        public static IEnumerable<BikeTrip> GetTripsFromStation(int stationID)
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
        public static void PutStations(IEnumerable<Station> stations)
        {
            //DataTable stationsTable = BuildStationsTable(stations);

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                using (MySqlCommand cmd = conn.CreateCommand())
                using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                //using (MySqlCommandBuilder builder = new MySqlCommandBuilder(adapter))
                //using (MySqlTransaction tran = conn.BeginTransaction(IsolationLevel.Serializable))
                {
                    //TODO: add all values it insert statement
                    cmd.CommandText = DBTables.BikeStations.InsertBikeStationQuery;

                    //TODO: Batch inserts together somehow

                    foreach (Station station in stations)
                    {
                        //AddWithValue() should sanitize inputs by escaping all dangerous characters.
                        cmd.Parameters.Clear();

                        cmd.Parameters.AddWithValue("@id", station.ID);
                        cmd.Parameters.AddWithValue("@namefin", station.NameFin);
                        cmd.Parameters.AddWithValue("@name", station.Name);

                        cmd.ExecuteNonQuery();
                    }
                    /*
                    builder.SetAllValues = true;

                    builder.GetInsertCommand();
                    builder.GetUpdateCommand();
                    builder.GetDeleteCommand();
                    adapter.UpdateBatchSize = 1000;
                    adapter.ContinueUpdateOnError = true;
                    return adapter.Update(stationsTable);
                    */

                    //TODO: make sure query is sanitized

                    //TODO: sanitize station strings
                    //TODO: put data into database
                    // cmd.ExecuteNonQuery();
                    //tran.Commit();
                }
            }
        }

        public static bool TryGetStation(int stationID, out Station station)
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
