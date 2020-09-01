using System.Text.Json.Serialization;

namespace Stop.API.IntegrationTests.Models
{
    public class Place
    {
        [JsonPropertyName("placeName")]
        public string PlaceName { get; set; }

        [JsonPropertyName("location")]
        public Location Location { get; set; }
    }
}
