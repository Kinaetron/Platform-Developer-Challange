using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Stop.API.Models
{
    public class Coordinates
    {
        [JsonPropertyName("lat")]
        public double Lat { get; set; }

        [JsonPropertyName("lng")]
        public double Lng { get; set; }
    }

    public class Geometry
    {
        [JsonPropertyName("location")]
        public Coordinates Location { get; set; }
    }

    public class Place
    {
        [JsonPropertyName("geometry")]
        public Geometry Geometry { get; set; }

        [JsonPropertyName("name")]
        public string PlaceName { get; set; }
    }

    public class Places
    {
        [JsonPropertyName("results")]
        public List<Place> Results { get; set; }
    }
}
