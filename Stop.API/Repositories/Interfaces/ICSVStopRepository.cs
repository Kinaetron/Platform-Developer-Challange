using System.Collections.Generic;
using Stop.API.Models;

namespace Stop.API.Repositories
{
    public interface ICSVStopRepository
    {
        CSVStop Get(string id);
        IEnumerable<CSVStop> GetAll();
    }
}
