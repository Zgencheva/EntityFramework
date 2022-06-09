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
           //ImportSuppliers(context, suppliersJson); 
           Console.WriteLine(ImportParts(context, partsJson));
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

            Random rnd = new Random();

            foreach (var part in parts)
            {
                int randomSupplierIndex = rnd.Next(0, suppliers.Count - 1);

                part.Supplier = suppliers[randomSupplierIndex];
            }


            context.Parts.AddRange(parts);

            return $"Successfully imported {context.SaveChanges()}.";
            //return null;
        }
    }
}