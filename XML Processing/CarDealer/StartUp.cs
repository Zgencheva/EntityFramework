using System;
using AutoMapper;
using CarDealer.Data;
using System.IO;
using CarDealer.XMLHelper;
using CarDealer.DataTransferObjects.Import;
using CarDealer.DataTransferObjects.Export;
using CarDealer.Models;
using System.Xml.Serialization;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

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
            Console.WriteLine(GetSalesWithAppliedDiscount(context));
        }

        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {

            var root = "sales";
            var sales = context.Sales
                .Select(s => new SalesWithDiscountExportDto
                {
                   Car = new CarExportDto {
                        Make = s.Car.Make,
                        Model = s.Car.Model,
                        TravelledDistance = s.Car.TravelledDistance,
                   },
                   Disctount = s.Discount,
                   CustomerName = s.Customer.Name,
                   Price = s.Car.PartCars.Sum(x=> x.Part.Price),
                   PriceWithDiscount = s.Car.PartCars.Sum(x=> x.Part.Price) -
                   (s.Car.PartCars.Sum(x => x.Part.Price) * s.Discount/100)
                })
                .ToArray();
            var result = XmlConverter.Serialize<SalesWithDiscountExportDto>(sales, root);

            return result.ToString().TrimEnd();
        }
        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
           
            var root = "customers";
            var customers = context.Customers
               .Where(x=> x.Sales.Any())
                .Select(s => new CustomersTotalSalesDTO
                {
                    FullName = s.Name,
                    BoughtCars = s.Sales.Count,
                    SpentMoney = s.Sales
                                   .Select(x=> x.Car)
                                   .SelectMany(x=> x.PartCars)
                                   .Sum(x=> x.Part.Price),
                })
               .OrderByDescending(c => c.SpentMoney)
                .ToArray();
            var result = XmlConverter.Serialize<CustomersTotalSalesDTO>(customers, root);

            return result.ToString().TrimEnd();
        }
        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            var root = "cars";
            var cars = context.Cars
                .Include(x => x.PartCars)
                .ThenInclude(x => x.Part)
                .Select(car => new CarsWithListOfPartsDto
                {
                    Make = car.Make,
                    Model = car.Model,
                    TravelledDistance = car.TravelledDistance,
                    Parts = car.PartCars.Select(p => new CarPartsExportDto {
                        Name = p.Part.Name,
                        Price = p.Part.Price,
                    })
                    .OrderByDescending(p=> p.Price)
                    .ToArray()
                })
                .OrderByDescending(c=> c.TravelledDistance)
                .ThenBy(c=> c.Model)
                .Take(5)
                .ToArray();
            var result = XmlConverter.Serialize<CarsWithListOfPartsDto>(cars, root);
            //var serializer = new XmlSerializer(typeof(ProductsInangeExportDTO[]), 
            //    new XmlRootAttribute("Products"));
            //var texWriter = new StringWriter();
            //serializer.Serialize(texWriter, products, namespaces);

            return result.ToString().TrimEnd();
        }
        public static string GetLocalSuppliers(CarDealerContext context)
        {
            var root = "suppliers";
            var suppliers = context.Suppliers
                .Where(x => x.IsImporter == false)
                .Select(s => new SuppliersExportDto
                {
                    Id = s.Id,
                    Name = s.Name,
                   PartsCount = s.Parts.Count
                })
                .ToArray();
            var result = XmlConverter.Serialize<SuppliersExportDto>(suppliers, root);
            //var serializer = new XmlSerializer(typeof(ProductsInangeExportDTO[]), 
            //    new XmlRootAttribute("Products"));
            //var texWriter = new StringWriter();
            //serializer.Serialize(texWriter, products, namespaces);

            return result.ToString().TrimEnd();
        }
        public static string GetCarsFromMakeBmw(CarDealerContext context)
        {
            var root = "cars";
            var cars = context.Cars
                .Where(x => x.Make == "BMW")
                .Select(p => new CarsWithMakeBMWExportDto
                {
                    Id = p.Id,
                    Model = p.Model,
                    TravelledDistance = p.TravelledDistance
                })
                .OrderBy(x => x.Model)
                .ThenByDescending(x => x.TravelledDistance)
                .ToArray();
            var result = XmlConverter.Serialize<CarsWithMakeBMWExportDto>(cars, root);
            //var serializer = new XmlSerializer(typeof(ProductsInangeExportDTO[]), 
            //    new XmlRootAttribute("Products"));
            //var texWriter = new StringWriter();
            //serializer.Serialize(texWriter, products, namespaces);

            return result.ToString().TrimEnd();
        }
        public static string GetCarsWithDistance(CarDealerContext context) {
            var root = "cars";
            var cars = context.Cars
                .Where(x => x.TravelledDistance > 2000000)
                .Select(p => new CarsOver2bDistanceExportDto
                {
                    Make = p.Make,
                    Model = p.Model,
                    TravelledDistance = p.TravelledDistance
                })
                .OrderBy(x => x.Make)
                .ThenBy(x=>x.Model)
                .Take(10)
                .ToArray();
            var result = XmlConverter.Serialize<CarsOver2bDistanceExportDto>(cars, root);
            //var serializer = new XmlSerializer(typeof(ProductsInangeExportDTO[]), 
            //    new XmlRootAttribute("Products"));
            //var texWriter = new StringWriter();
            //serializer.Serialize(texWriter, products, namespaces);
            
            return result.ToString().TrimEnd();
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