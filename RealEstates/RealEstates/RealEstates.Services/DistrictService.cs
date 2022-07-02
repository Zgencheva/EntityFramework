using AutoMapper.QueryableExtensions;
using RealEstates.Data;
using RealEstates.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstates.Services
{
    public class DistrictService : BaseService, IDistrictService
    {
        private readonly ApplicationDbContext dbContext;
        public DistrictService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public IEnumerable<DistrictInfoDto> GetMostExpensiveDistricts(int count)
        {
           var listOfProps = dbContext.Districts
                 .ProjectTo<DistrictInfoDto>(this.Mapper.ConfigurationProvider)
                 .OrderByDescending(x=> x.AveragePricePerSquareMeter)
                 .Take(count)
                 .ToList();
            return listOfProps;
        }
    }
}
