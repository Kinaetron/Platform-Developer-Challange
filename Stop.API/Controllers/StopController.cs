using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Stop.Model;
using Stop.Repository;

namespace Stop.API.Controllers
{
    [Route("api/stops")]
    [ApiController]
    public class StopController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly ICSVStopRepository stopRepository;
        private readonly IPlacesRepository placesRepository;

        public StopController(ICSVStopRepository stopRepository, IPlacesRepository placesRepository, IMapper mapper)
        {
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this.stopRepository = stopRepository ?? throw new ArgumentNullException(nameof(stopRepository));
            this.placesRepository = placesRepository ?? throw new ArgumentNullException(nameof(placesRepository));
        }

        public IActionResult Get(double minLatitude, double minLongitude, 
                                 double maxLatitude, double maxLongitude)
        {
            var results = stopRepository.GetAll().Where(x => x.Latitude >= minLatitude && x.Latitude <= maxLatitude &&
                                                             x.Longitude >= minLongitude && x.Longitude <= maxLongitude);

            var models = mapper.Map<List<StopDTO>>(results);

            if (models.Count <= 0) {
                return NotFound();
            }

            return Ok(models);
        }

        [HttpGet("{stopId}")]
        public IActionResult Get(string stopId)
        {
            var result = stopRepository.Get(stopId);

            if(result == null) {
                return NotFound();
            }

            var stop = mapper.Map<StopDTO>(result);
            return Ok(stop);
        }

        [HttpGet("{stopId}/nearby")]
        public async Task<IActionResult> GetAsync(string stopId, double distance)
        {
            var stop = stopRepository.Get(stopId);

            if(stop == null) {
                return NotFound();
            }

            var results = await placesRepository.Find(distance, stop.Latitude, stop.Longitude);

            if(results.Count() <= 0) {
                return NotFound();
            }

            var places = mapper.Map<IEnumerable<PlaceDTO>>(results);

            return Ok(places);
        }

    }
}