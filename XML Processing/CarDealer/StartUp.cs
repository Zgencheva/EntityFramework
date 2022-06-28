using System;
using AutoMapper;
using CarDealer.Data;
using System.IO;
using CarDealer.XMLHelper;
using CarDealer.DataTransferObjects.Import;
using CarDealer.Models;
using System.Xml.Serialization;
using System.Linq;
using System.Collections.Generic;

namespace CarDealer
{
    public class StartUp
    {
        static IMapper mapper;
        public static void Main(string[] args)
        {
            var context = new CarDealerContext();
            //context.Database.EnsureDeleted();
            //context.Database.EnsureCreated();
            //var xmlSuppliers = File.ReadAllText("../../../Datasets/suppliers.xml");
            //var xmlParts = File.ReadAllText("../../../Datasets/parts.xml");
            //var xmlCars = File.ReadAllText("../../../Datasets/cars.xml");
            //var xmlCustomers = File.ReadAllText("../../../Datasets/customers.xml");
            //var xmlSales = File.ReadAllText("../../../Datasets/sales.xml");
            //Console.WriteLine(ImportSuppliers(context, xmlSuppliers));
            // Console.WriteLine(ImportParts(context, xmlParts));
            //Console.WriteLine(ImportCars(context, xmlCars));
            //Console.WriteLine(ImportCustomers(context, xmlCustomers));
            //Console.WriteLine(ImportSales(context, xmlSales));
        }
        public static string ImportSales(CarDealerContext context, string inputXml)
        {
            InitializeAutoMapper();
            const string root = "Sales";
            var cars = context.Cars.Select(x => x.Id).ToList();
            var salesDtos = XmlConverter.Deserializer<SalesImputModel>(inputXml, root)
                .Where(x=> cars.Contains(x.CarId));
            var sales = mapper.Map<Sale[]>(salesDtos);

            context.Sales.AddRange(sales);

            return $"Successfully imported {context.SaveChanges()}";
        }
        public static string ImportCustomers(CarDealerContext context, string inputXml)
        {
            InitializeAutoMapper();
            const string root = "Customers";

            var customersDtos = XmlConverter.Deserializer<CustomersInputModel>(inputXml, root);
            var customers = mapper.Map<Customer[]>(customersDtos);
            
            context.Customers.AddRange(customers);
   
            return $"Successfully imported {context.SaveChanges()}";
        }
        public static string ImportSuppliers(CarDealerContext context, string inputXml)
        {
            InitializeAutoMapper();
            const string root = "Suppliers";
            //var suppliersDtos = XmlConverter.Deserializer<SuppliersInputModel>(inputXml, root);

            var xmlSerializer = new XmlSerializer(typeof(SuppliersInputModel[]),
             new XmlRootAttribute(root));
            var textRead = new StringReader(inputXml);
            var suppliersDtos = (SuppliersInputModel[])xmlSerializer.Deserialize(textRead);
            var suppliers = mapper.Map<Supplier[]>(suppliersDtos);
            context.Suppliers.AddRange(suppliers);

            return $"Successfully imported {context.SaveChanges()}"; ; ;
        }

        public static string ImportParts(CarDealerContext context, string inputXml)
        {
            InitializeAutoMapper();
            const string root = "Parts";
            var suppliers = context.Suppliers.Select(x => x.Id).ToList();
            var partsDtos = XmlConverter.Deserializer<PartsInputModel>(inputXml, root);
            var dtos = partsDtos.ToList().Where(x => suppliers.Contains(x.SupplierId.Value));

            var parts = mapper.Map<Part[]>(dtos);
            context.Parts.AddRange(parts);
            ;
            return $"Successfully imported {context.SaveChanges()}"; ; ;
        }

        public static string ImportCars(CarDealerContext context, string inputXml)
        {
            InitializeAutoMapper();
            const string root = "Cars";

            var carsDtos = XmlConverter.Deserializer<CarsInputModel>(inputXml, root);
            var allParts = context.Parts.Select(x => x.Id).ToList();
            var cars = new List<Car>();
            foreach (var carDto in carsDtos)
            {
                var distinctedParts = carDto.Parts.Select(x => x.Id).Distinct();
                var parts = distinctedParts.Intersect(allParts);
                var car = new Car
                {
                    Model = carDto.Model,
                    Make = carDto.Make,
                    TravelledDistance = carDto.TraveledDistance,
                   
                };
                foreach (var part in parts)
                {
                    var partCar = new PartCar { PartId = part };
                    car.PartCars.Add(partCar);
                }
                cars.Add(car);
              
            }
         ;
            context.Cars.AddRange(cars);
            context.SaveChanges();
            return $"Successfully imported {cars.Count}";
        }
        private static void InitializeAutoMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CarDealerProfile>();
            });
            mapper = config.CreateMapper();
        }
    }
}