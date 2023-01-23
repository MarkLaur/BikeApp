using ApiServer.Controllers;
using ApiServer.Models;
using MySql.Data.MySqlClient;
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
                                reader.GetInt32(BikeTripTableStrings.ID),
                                reader.GetDateTime(BikeTripTableStrings.Departure),
                                reader.GetDateTime(BikeTripTableStrings.Return),
                                reader.GetInt32(BikeTripTableStrings.DepartureStationID),
                                reader.GetString(BikeTripTableStrings.DepartureStationName),
                                reader.GetInt32(BikeTripTableStrings.ReturnStationID),
                                reader.GetString(BikeTripTableStrings.ReturnStationName),
                                reader.GetInt32(BikeTripTableStrings.Distance),
                                new TimeSpan(0, 0, reader.GetInt32(BikeTripTableStrings.Duration))
                                ));
                        }
                    }
                }

                return tripList;
            }
        }

        public static IEnumerable<BikeTrip> GetTrips()
        {
            return GetTrips(BikeTripTableStrings.BikeTripQueryWithStationNames);
        }

        public static IEnumerable<BikeTrip> GetTripsFromStation(int stationID)
        {
            return GetTrips(BikeTripTableStrings.BuildBikeTripsFromStationQuery(stationID));
        }

        public static IEnumerable<Station> GetStations()
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                List<Station> stationList = new List<Station>();

                using (MySqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = BikeStationTableStrings.GetBikeStationsQuery;
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

        public static bool TryGetStation(int stationID, out Station station)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                using (MySqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = BikeStationTableStrings.BuildBikeStationQuery(stationID);
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
