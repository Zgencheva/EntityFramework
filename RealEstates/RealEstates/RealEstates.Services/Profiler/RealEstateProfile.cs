using AutoMapper;
using RealEstates.Models;
using RealEstates.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        }
    }
}
