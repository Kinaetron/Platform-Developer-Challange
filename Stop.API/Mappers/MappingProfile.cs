using AutoMapper;
using Stop.API.Models;

namespace Stop.API.Mappers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CSVStop, StopDTO>()
                .ForMember(x => x.Location, y => y
                .MapFrom(z => new Location() { Latitude = z.Latitude, Longitude = z.Longitude }));

            CreateMap<Place, PlaceDTO>()
               .ForMember(x => x.Location, y => y
               .MapFrom(z => new Location() { Latitude = z.Geometry.Location.Lat, Longitude = z.Geometry.Location.Lng }));
        }
    }
}
