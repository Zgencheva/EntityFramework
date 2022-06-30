using RealEstates.Data;
using RealEstates.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace RealEstates.Importer
{
    class Program
    {
        static void Main(string[] args)
        {
            ImportJsonFile("../../../imtBgHouses.json");
            Console.WriteLine();
            ImportJsonFile("../../../imotBgRowData.json");
        }

        public static void ImportJsonFile(string fileName) {
            var dbContext = new ApplicationDbContext();
            IPropertyService propertyService = new PropertyService(dbContext);

            var properties = JsonSerializer.Deserialize<IEnumerable<PropertyAsJson>>(File.ReadAllText(fileName));
            foreach (var jsonProperty in properties)
            {
                propertyService.Add(jsonProperty.District, jsonProperty.Floor,
                    jsonProperty.TotalFloors, jsonProperty.Size,
                    jsonProperty.YardSize, jsonProperty.Year, jsonProperty.Type,
                    jsonProperty.BuildingType, jsonProperty.Price);
                Console.WriteLine(".");
            }
            //propertyService.Add();
        }
    }
}
