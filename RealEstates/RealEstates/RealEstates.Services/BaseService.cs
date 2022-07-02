using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RealEstates.Services.Profiler;

namespace RealEstates.Services
{
    public abstract class BaseService
    {
       
        public BaseService()
        {
            InitializeAutoMapper();
        }
        protected IMapper Mapper { get; private set; }
        private void InitializeAutoMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<RealEstateProfile>();
            });
            this.Mapper = config.CreateMapper();
        }
    }

}
