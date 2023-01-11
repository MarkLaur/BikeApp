using ApiServer.Controllers;
using MySql.Data.MySqlClient;
using System.Data;
using System.Diagnostics;

namespace ApiServer.Tools
{
    public static class DatabaseHandler
    {
        //TODO: change default password and user
        private const string connectionString = "Server=localhost; Port=3307; Database=test; Uid=root; Pwd=usbw;";

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

                //TODO: get data from database
                trips = Enumerable.Range(1, 5).Select(index => new BikeTrip
                {
                    Name = $"Trip {index}",
                    Length = index * index
                })
                .ToArray();

                return true;
            }
        }
    }
}
