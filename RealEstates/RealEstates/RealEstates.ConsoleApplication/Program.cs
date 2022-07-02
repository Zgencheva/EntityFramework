using Microsoft.EntityFrameworkCore;
using RealEstates.Data;
using RealEstates.Services;
using System;
using System.Text;

namespace RealEstates.ConsoleApplication
{
    class Program
    {

        static void Main(string[] args)
        {
           
            Console.OutputEncoding = Encoding.Unicode;
            Console.InputEncoding = Encoding.Unicode;
            var db = new ApplicationDbContext();
            IPropertyService propertyService = new PropertyService(db);
            db.Database.Migrate();

            while (true)
            {
                Console.Clear();
                Console.WriteLine("Choose an Option:");
                Console.WriteLine("1. Property search");
                Console.WriteLine("2. Most expensive districts");
                Console.WriteLine("3. Average price per square meter");
                Console.WriteLine("4. Add tag");
                Console.WriteLine("5. Bulk tag to properties");
                Console.WriteLine("0. Exit");
                bool parsed = int.TryParse(Console.ReadLine(), out int option);
                if (parsed && option == 0)
                {
                    break;
                }
                if (parsed && (option >= 1 || option <= 5))
                {
                    switch (option)
                    {
                        case 1:
                            PropertySearch(db);
                            break;
                        case 2:
                            MostExpensiveDistricts(db);
                            break;
                        case 3:
                            AveragePricePerSquareMeter(db);
                            break;
                        case 4:
                            AddTag(db);
                            break;
                        case 5:
                            BulkTagToProperties(db, propertyService);
                            break;

                    }

                    Console.WriteLine("Press any key to continue..");
                    Console.ReadKey();
                }

            }

        }

        private static void BulkTagToProperties(ApplicationDbContext db, IPropertyService propertyService)
        {
            Console.WriteLine("Bulk opration started!");
            ITagService tagService = new TagService(db, propertyService);
            tagService.BulkTagToProperties();
            Console.WriteLine("Bulk opration finished!");
        }

        private static void AddTag(ApplicationDbContext db)
        {
            Console.WriteLine("Tag name:");
            string tagName = Console.ReadLine();
            Console.WriteLine("Tag importance(not required):");
            bool isParsed = int.TryParse(Console.ReadLine(), out int tagImportance);
            int? importance = isParsed ? tagImportance : null;
            ITagService tagService = new TagService(db);
            tagService.Add(tagName, importance);
        }

        private static void MostExpensiveDistricts(ApplicationDbContext db)
        {
            Console.WriteLine("District count:");
            int count = int.Parse(Console.ReadLine());
            IDistrictService districtService = new DistrictService(db);
            var districts = districtService.GetMostExpensiveDistricts(count);

            foreach (var district in districts)
            {
                Console.WriteLine($"{district.Name} => {district.AveragePricePerSquareMeter:f2}EUR/m^2" +
                    $" (Count of properties: {district.PropertiesCount})");
            }
            
        }
        private static void AveragePricePerSquareMeter(ApplicationDbContext db)
        {
            IPropertyService propertiesService = new PropertyService(db);
            Console.WriteLine($"Average price per square meter: {propertiesService.AveragePricePerSquareMEter():f2}EUR/m^2");

        }

        private static void PropertySearch(ApplicationDbContext db)
        {
            Console.WriteLine("Min price:");
            int minPice = int.Parse(Console.ReadLine());
            Console.WriteLine("Max price:");
            int maxPrice = int.Parse(Console.ReadLine());
            Console.WriteLine("Min size:");
            int minSize = int.Parse(Console.ReadLine());
            Console.WriteLine("Max size:");
            int maxSize = int.Parse(Console.ReadLine());

            IPropertyService service = new PropertyService(db);
            var properties = service.Search(minPice, maxPrice, minSize, maxSize);
            foreach (var property in properties)
            {
                Console.WriteLine($"{property.DistrictName}; " +
                    $"{property.BuildingType} => {property.Price}EUR => {property.Size}m^2");
            }
        }

       
    }
}
