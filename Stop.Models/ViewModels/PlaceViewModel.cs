using System.Text.Json.Serialization;

namespace Stop.Model
{
    public class PlaceViewModel
    {
        [JsonPropertyName("placeName")]
        public string PlaceName { get; set; }

        [JsonPropertyName("location")]
        public Location Location { get; set; }
    }
}
