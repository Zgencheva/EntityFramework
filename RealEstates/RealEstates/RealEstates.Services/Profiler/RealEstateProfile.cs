using AutoMapper;
using RealEstates.Models;
using RealEstates.Services.Models;
using System.Linq;


namespace RealEstates.Services.Profiler
{
    public class RealEstateProfile : Profile
    {
        public RealEstateProfile()
        {
            this.CreateMap<Property, PropertyInfoDto>()
                .ForMember(x=> x.BuildingType, y=> y.MapFrom(p=> p.BuildingType.Name));
            this.CreateMap<District, DistrictInfoDto>()
             .ForMember(x => x.PropertiesCount, y => y.MapFrom(p => p.Properties.Count))
             .ForMember(x=> x.AveragePricePerSquareMeter, y=> y.MapFrom(d=> 
                        d.Properties
                        .Where(x => x.Price.HasValue)
                        .Average(p => p.Price/ (decimal)p.Size) ?? 0));
            this.CreateMap<Property, PropertyInfoFullData>();
            this.CreateMap<Tag, TagInfoDto>();

        }
    }
}
