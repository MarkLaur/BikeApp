using System.Text.Json.Serialization;

namespace WebApp.Models
{
    //This could be a class, but then we would have to make sure that there is only one instance of each station so that
    //reference comparisons don't break. I might turn this into a class and make a station dictionary or something at some point.
    [System.Serializable]
    public struct Station
    {
        //[JsonPropertyName("id")]
        public int ID { get; private set; }
        //[JsonPropertyName("name")]
        public string Name { get; private set; }

        public Station(int id, string name)
        {
            ID = id;
            Name = name;
        }
    }
}
