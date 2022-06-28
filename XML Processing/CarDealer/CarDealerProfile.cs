using AutoMapper;
using CarDealer.DataTransferObjects.Import;
using CarDealer.Models;

namespace CarDealer
{
    public class CarDealerProfile : Profile
    {
        public CarDealerProfile()
        {

            this.CreateMap<SuppliersInputModel, Supplier>();
            this.CreateMap<PartsInputModel, Part>();
            this.CreateMap<CarsInputModel, Car>();
            this.CreateMap<CustomersInputModel, Customer>();
            this.CreateMap<SalesImputModel, Sale>();
        }
    }
}
