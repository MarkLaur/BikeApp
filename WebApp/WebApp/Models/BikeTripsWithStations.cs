using System.Runtime.Serialization;

namespace WebApp.Models
{
    /// <summary>
    /// Contains biketrips and a dictionary with the trips' stations.
    /// </summary>
    public class BikeTripsWithStations
    {
        public List<BikeTrip> BikeTrips { get; private set; }
        public Dictionary<int, Station> Stations { get; private set; }

        public BikeTripsWithStations(List<BikeTrip> bikeTrips, Dictionary<int, Station> stations)
        {
            BikeTrips = bikeTrips;
            Stations = stations;
        }

        /// <summary>
        /// Finds trip station references from Stations.
        /// </summary>
        [OnDeserialized]
        public void OnDeserialized()
        {
            if (BikeTrips.Count == 0)
            {
                //No need to find anything
                return;
            }
            else if (Stations.Count == 0)
            {
                //Can't find any station references even though there are trips, maybe throw an error
                return;
            }

            //Initialize to first value
            Station depStation = Stations.First().Value;
            Station retStation = depStation;

            //Checks if cached station IDs match trip station IDs, gets correct stations if needed and sets station references.
            foreach (BikeTrip trip in BikeTrips)
            {
                //Look for departure station
                if (trip.DepartureStationID == depStation.ID)
                {
                    trip.SetDepartureStationsRef(depStation);
                }
                //Check if the other station cached is correct so we only check the dictionary if needed.
                else if (trip.DepartureStationID == retStation.ID)
                {
                    trip.SetDepartureStationsRef(retStation);
                }
                else if (Stations.TryGetValue(trip.DepartureStationID, out Station? tempStation))
                {
                    trip.SetDepartureStationsRef(tempStation);
                    //Save station in case the next trip uses the same one
                    depStation = tempStation;
                }
                else
                {
                    //Station couldn't be found, maybe throw error.
                }

                //Look for return station
                if (trip.ReturnStationID == retStation.ID)
                {
                    trip.SetReturnStationRef(retStation);
                }
                //Check if the other cached station is correct so we only check the dictionary if needed.
                else if (trip.ReturnStationID == depStation.ID)
                {
                    trip.SetReturnStationRef(depStation);
                }
                else if (Stations.TryGetValue(trip.ReturnStationID, out Station? tempStation))
                {
                    trip.SetReturnStationRef(tempStation);
                    retStation = tempStation;
                }
                else
                {
                    //Station couldn't be found, maybe throw error.
                }
            }
        }
    }
}
