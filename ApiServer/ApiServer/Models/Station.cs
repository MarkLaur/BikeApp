using System.Text.Json.Serialization;

namespace ApiServer.Models
{
    //This could be a class, but then we would have to make sure that there is only one instance of each station so that
    //reference comparisons don't break. I might turn this into a class and make a station dictionary or something at some point.
    public struct Station
    {
        public int ID { get; private set; }
        public string Name { get; private set; }

        [JsonConstructor] //C#'s JsonSerializer can't serialize struct by default so the constructor has to be marked with this.
        public Station(int id, string name)
        {
            ID = id;
            Name = name;
        }
    }
}
