using AutoMapper;
using Stop.Model;

namespace Stop.API.Mappers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CSVStopViewModel, StopDTO>()
                .ForMember(x => x.Location, y => y
                .MapFrom(z => new LocationDTO() { Latitude = z.Latitude, Longitude = z.Longitude }));

            CreateMap<Point, PlaceDTO>()
               .ForMember(x => x.Location, y => y
               .MapFrom(z => new LocationDTO() { Latitude = z.Geometry.Location.Lat, Longitude = z.Geometry.Location.Lng }));
        }
    }
}
