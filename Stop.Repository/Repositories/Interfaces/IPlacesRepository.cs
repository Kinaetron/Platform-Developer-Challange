using System.Threading.Tasks;
using System.Collections.Generic;
using Stop.Model;

namespace Stop.Repository
{
    public interface IPlacesRepository
    {
        Task<IEnumerable<Point>> Find(double distance, double latitude, double longtitude);
    }
}
