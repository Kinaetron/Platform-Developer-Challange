namespace Stop.API.Models
{
    public class Location
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    public class StopDTO
    {
        public string StopId { get; set; }
        public string StopName { get; set; }
        public Location Location { get; set; }
    }
}
