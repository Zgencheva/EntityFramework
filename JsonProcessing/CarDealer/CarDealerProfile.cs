using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using CarDealer.DTO;
using CarDealer.Models;

namespace CarDealer
{
    public class CarDealerProfile : Profile
    {
        public CarDealerProfile()
        {
            this.CreateMap<SuppliersImportModel, Supplier>();
            this.CreateMap<PartsInputModel, Part>();
            this.CreateMap<CarsInputModel, Car>();
            this.CreateMap<CarPartsDTo, Part>();
            this.CreateMap<CustomersInputDTO, Customer>();
            this.CreateMap<SalesImportDto, Sale>();

            this.CreateMap<Customer, OrderedCustomersDTO>();
            this.CreateMap<Car, ExportCarDTO>();
            this.CreateMap<Supplier, SuppliesPartsExport>()
                .ForMember(x=> x.PartsCount, y=> y.MapFrom(s=> s.Parts.Count));

        }
    }
}
