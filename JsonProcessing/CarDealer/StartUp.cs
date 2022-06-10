using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper;
using CarDealer.Data;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Serialization;
using CarDealer.Models;
using Newtonsoft.Json;
using CarDealer.DTO;

namespace CarDealer
{
    public class StartUp
    {
        static IMapper mapper;
        public static void Main(string[] args)
        {
            CarDealerContext context = new CarDealerContext();
           //context.Database.EnsureDeleted();
           //context.Database.EnsureCreated();
         
            //InitializeAutoMapper();
            //var serilized = new JsonSerializerSettings
            //{
            //    Formatting = Formatting.Indented,
            //    NullValueHandling = NullValueHandling.Include,
            //};
            var suppliersJson = File.ReadAllText("../../../Datasets/suppliers.json");
            var partsJson = File.ReadAllText("../../../Datasets/parts.json");
            var carsJson = File.ReadAllText("../../../Datasets/cars.json");
            var customersJson = File.ReadAllText("../../../Datasets/customers.json");
            var salesJson = File.ReadAllText("../../../Datasets/sales.json");
            //ImportSuppliers(context, suppliersJson);
            //ImportParts(context, partsJson);
            //ImportCars(context, carsJson);
            //ImportCustomers(context, customersJson);
            Console.WriteLine(ImportSales(context, salesJson));

        }

        private static void InitializeAutoMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CarDealerProfile>();
            });
            mapper = config.CreateMapper();
        }

        public static string ImportSuppliers(CarDealerContext context, string inputJson)
        {
            InitializeAutoMapper();

            var supplierDto = JsonConvert.DeserializeObject<IEnumerable<SuppliersImportModel>>(inputJson);

            var suppliers = mapper.Map<IEnumerable<Supplier>>(supplierDto);
            context.Suppliers.AddRange(suppliers);

            return $"Successfully imported {context.SaveChanges()}.";
        }

        public static string ImportParts(CarDealerContext context, string inputJson)
        {
            InitializeAutoMapper();
            var options = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
            };
            var partsDto = JsonConvert.DeserializeObject<IEnumerable<PartsInputModel>>(inputJson, options);
            var parts = mapper.Map<IEnumerable<Part>>(partsDto);
            var suppliers = context.Suppliers.ToList();
            var partList = new List<Part>();

            foreach (var part in parts)
            {
                if (suppliers.Any(x => x.Id == part.SupplierId)) {
                    partList.Add(part);
                }
            }

            context.Parts.AddRange(partList);

            return $"Successfully imported {context.SaveChanges()}.";
            //return null;
        }

        public static string ImportCars(CarDealerContext context, string inputJson)
        {
            InitializeAutoMapper();
            var carsDto = JsonConvert.DeserializeObject<IEnumerable<CarsInputModel>>(inputJson);
            var cars = new List<Car>();

            foreach (var carDto in carsDto)
            {
                var car = new Car
                {
                    Model = carDto.Model,
                    Make = carDto.Make,
                    TravelledDistance = carDto.TravelledDistance,
                
                };

                // context.Cars.Add(car);
                if (carDto.PartsId == null)
                {
                    cars.Add(car);
                    continue;
                }

                foreach (var part in carDto.PartsId)
                {
                    var currentPart = context.Parts.FirstOrDefault(x => x.Id == part);
                    if (currentPart == null)
                    {
                        continue;
                    }
                    if (car.PartCars.Count != 0 && car.PartCars.Any(x=> x.Part == currentPart)) {
                        continue;
                    }
                    
                        var carPart = new PartCar
                        {
                            Car = car,
                            Part = currentPart,
                        };
                        
                        car.PartCars.Add(carPart);
                    
                }
                cars.Add(car);
            }

            context.Cars.AddRange(cars);
            context.SaveChanges();
            return $"Successfully imported {cars.Count}."; ;
        }

        public static string ImportCustomers(CarDealerContext context, string inputJson) {

            InitializeAutoMapper();
            var customersDto = JsonConvert.DeserializeObject<IEnumerable<CustomersInputDTO>>(inputJson);

            var customers = mapper.Map<IEnumerable<Customer>>(customersDto);
            context.Customers.AddRange(customers);

            return $"Successfully imported {context.SaveChanges()}.";
        }

        public static string ImportSales(CarDealerContext context, string inputJson)
        {

            InitializeAutoMapper();
            //var withod = JsonConvert.DeserializeObject<IEnumerable<SalesImportDto>>(inputJson);
            var salesDto = JsonConvert.DeserializeObject<IEnumerable<SalesImportDto>>(inputJson)
                .Where(x=> context.Customers.Any(y=> y.Id == x.CustomerId)
                 && context.Cars.Any(c=> c.Id == x.CarId));
            
            var sales = mapper.Map<IEnumerable<Sale>>(salesDto);
            
            context.Sales.AddRange(sales);

            return $"Successfully imported {context.SaveChanges()}.";
            
        }
    }
}