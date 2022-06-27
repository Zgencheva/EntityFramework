using System;
using AutoMapper;
using CarDealer.Data;
using System.IO;
using CarDealer.XMLHelper;
using CarDealer.DataTransferObjects.Import;
using CarDealer.Models;
using System.Xml.Serialization;
using System.Linq;

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
            var xmlSuppliers = File.ReadAllText("../../../Datasets/suppliers.xml");
            var xmlParts = File.ReadAllText("../../../Datasets/parts.xml");
            var xmlCars = File.ReadAllText("../../../Datasets/cars.xml");
            Console.WriteLine(ImportCars(context, xmlCars));
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
          
            

            var parts = mapper.Map<Car[]>(carsDtos);
            context.Cars.AddRange(parts);
            ;
            return $"Successfully imported {context.SaveChanges()}"; ; ;
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