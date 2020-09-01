using System.Threading.Tasks;
using System.Collections.Generic;
using Stop.API.Models;

namespace Stop.API.Repositories
{
    public interface IPlacesRepository
    {
        Task<IEnumerable<Place>> Find(double distance, double latitude, double longtitude);
    }
}
