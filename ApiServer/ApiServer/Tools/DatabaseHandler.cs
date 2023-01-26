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
