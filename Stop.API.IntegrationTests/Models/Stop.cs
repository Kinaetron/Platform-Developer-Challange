using System.Text.Json.Serialization;

namespace Stop.API.IntegrationTests.Models
{
    public class Location
    {
        [JsonPropertyName("latitude")]
        public double Latitude { get; set; }
        [JsonPropertyName("longitude")]
        public double Longitude { get; set; }
    }

    public class Stop
    {
        [JsonPropertyName("stopId")]
        public string StopId { get; set; }

        [JsonPropertyName("stopName")]
        public string StopName { get; set; }

        [JsonPropertyName("location")]
        public Location Location { get; set; }
    }
}
