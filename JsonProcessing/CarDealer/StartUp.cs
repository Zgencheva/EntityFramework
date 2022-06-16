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
            //    NullValueHandling = NullValueHandling.Include
            //};
            //var suppliersJson = File.ReadAllText("../../../Datasets/suppliers.json");
            //var partsJson = File.ReadAllText("../../../Datasets/parts.json");
            //var carsJson = File.ReadAllText("../../../Datasets/cars.json");
            //var customersJson = File.ReadAllText("../../../Datasets/customers.json");
            //var salesJson = File.ReadAllText("../../../Datasets/sales.json");
            //ImportSuppliers(context, suppliersJson);
            //ImportParts(context, partsJson);
            //ImportCars(context, carsJson);
            //ImportCustomers(context, customersJson);
            //ImportSales(context, salesJson);
            //Console.WriteLine(GetOrderedCustomers(context));
            Console.WriteLine(GetSalesWithAppliedDiscount(context));

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

        public static string GetOrderedCustomers(CarDealerContext context)
        {

            InitializeAutoMapper();
            var orderedCustomers = context.Customers
                            .OrderBy(x => x.BirthDate)
                            .ThenBy(x => x.IsYoungDriver)
                            .ToList();
            var customersDTOs = mapper.Map<IEnumerable<OrderedCustomersDTO>>(orderedCustomers);
            var serilized = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Include,
                DateFormatHandling = DateFormatHandling.MicrosoftDateFormat,
                DateFormatString = "dd/MM/yyyy",

        };
            var outputResult = JsonConvert.SerializeObject(customersDTOs, serilized);

            return outputResult;
        }

        public static string GetCarsFromMakeToyota(CarDealerContext context)
        {

            InitializeAutoMapper();
            var orderedCars = context.Cars
                            .Where(c=> c.Make == "Toyota")
                            .OrderBy(x => x.Model)
                            .ThenByDescending(x => x.TravelledDistance)
                            .ToList();
            var carsDTOs = mapper.Map<IEnumerable<ExportCarDTO>>(orderedCars);
            var serilized = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                //NullValueHandling = NullValueHandling.Include,
                //DateFormatHandling = DateFormatHandling.MicrosoftDateFormat,
                //DateFormatString = "dd/MM/yyyy",

            };
            var outputResult = JsonConvert.SerializeObject(carsDTOs, serilized);

            return outputResult;
        }

        public static string GetLocalSuppliers(CarDealerContext context)
        {

            InitializeAutoMapper();
            var suppliers = context.Suppliers
                            .Include(x=> x.Parts)
                            .Where(c => c.IsImporter == false)
                            .ToList();
            var suppliersDto = mapper.Map<IEnumerable<SuppliesPartsExport>>(suppliers);
            var serilized = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                //NullValueHandling = NullValueHandling.Include,
                //DateFormatHandling = DateFormatHandling.MicrosoftDateFormat,
                //DateFormatString = "dd/MM/yyyy",

            };
            var outputResult = JsonConvert.SerializeObject(suppliersDto, serilized);

            return outputResult;
        }

        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {

            InitializeAutoMapper();
            var carProection = context.Cars
                            .Include(x => x.PartCars)
                            .ThenInclude(c => c.Part)
                            .Select(x => new
                            {
                                car = new {
                                    Make = x.Make,
                                    Model = x.Model,
                                    TravelledDistance = x.TravelledDistance
                                },
                                parts =
                                x.PartCars.Select(p=> new { 
                                    Name = p.Part.Name,
                                    Price = $"{p.Part.Price:F2}",
                                }).ToArray(),
                            })
                            .ToList();
           
            var serilized = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                FloatFormatHandling = FloatFormatHandling.DefaultValue,
               
            };
            var outputResult = JsonConvert.SerializeObject(carProection, serilized);

            return outputResult;
        }

        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {

            InitializeAutoMapper();
            var salesProection = context.Customers
                            .Include(x => x.Sales)
                            .ThenInclude(x=> x.Car)
                            .ThenInclude(x=> x.PartCars)
                            .ThenInclude(x=> x.Part)
                            .Where(c => c.Sales.Count() > 0)
                            .Select(x => new
                            {

                                fullName = x.Name,
                                boughtCars = x.Sales.Count,
                                spentMoney = x.Sales.Sum(s=> s.Car.PartCars.Sum(p=> p.Part.Price))

                            })
                             .OrderByDescending(c => c.spentMoney)
                            .ThenByDescending(c => c.boughtCars)
                            .ToArray();
                           

            var serilized = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                FloatFormatHandling = FloatFormatHandling.DefaultValue,

            };
            var outputResult = JsonConvert.SerializeObject(salesProection, serilized);

            return outputResult;
        }
        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            var sales = context.Sales
                .Take(10)
                .Select(s => new
                {
                    car = new
                    {
                        Make = s.Car.Make,
                        Model = s.Car.Model,
                        TravelledDistance = s.Car.TravelledDistance
                    },
                    customerName = s.Customer.Name,
                    Discount = $"{s.Discount:F2}",
                    price = $"{s.Car.PartCars.Sum(pc => pc.Part.Price):F2}",
                    priceWithDiscount = $"{s.Car.PartCars.Sum(pc => pc.Part.Price) * (1 - (s.Discount / 100)):F2}",
                })
                .ToArray();

            var jsonFile = JsonConvert.SerializeObject(sales, Formatting.Indented);

            return jsonFile;
        }
    }
}