using ApiServer.Controllers;
using ApiServer.Models;
using MySql.Data.MySqlClient;
using System.Data;
using System.Diagnostics;

namespace ApiServer.Tools
{
    public static class DatabaseHandler
    {
        //TODO: change default password and user
        private const string connectionString = "Server=localhost; Port=3307; Database=bikeapp; Uid=root; Pwd=usbw;";

        public static bool TestConnection()
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                }
                catch
                {
                    //TODO: return a enum result or log something
                    return false;
                }

                return true;
            }
        }

        public static bool TryGetTrips(out IEnumerable<BikeTrip> trips)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                }
                catch
                {
                    //TODO: return a enum result or log something
                    trips = new BikeTrip[0];
                    return false;
                }

                List<BikeTrip> tripList = new List<BikeTrip>();

                using (MySqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = BikeTripTableStrings.BikeTripQueryWithStationNames;
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
                                new Station(reader.GetInt32(BikeTripTableStrings.DepartureStationID), reader.GetString(BikeTripTableStrings.DepartureStationName)),
                                new Station(reader.GetInt32(BikeTripTableStrings.ReturnStationID), reader.GetString(BikeTripTableStrings.ReturnStationName)),
                                reader.GetInt32(BikeTripTableStrings.Distance),
                                new TimeSpan(0, 0, reader.GetInt32(BikeTripTableStrings.Duration))
                                ));
                        }
                    }
                }

                trips = tripList;
                return true;
            }
        }
    }
}
