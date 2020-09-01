using Stop.Model;
using System.Collections.Generic;

namespace Stop.Repository
{
    public interface ICSVStopRepository
    {
        CSVStopViewModel Get(string id);
        IEnumerable<CSVStopViewModel> GetAll();
    }
}
