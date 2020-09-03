using Stop.Model;
using TinyCsvParser.Mapping;

namespace Stop.API.Mappers
{
    public class CSVStopMapping : CsvMapping<CSVStopViewModel>
    {
        public CSVStopMapping()
            :base()
        {
            MapProperty(0, x => x.StopId);
            MapProperty(1, x => x.StopName);
            MapProperty(2, x => x.Latitude);
            MapProperty(3, x => x.Longitude);
        }
    }
}
