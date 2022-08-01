
namespace Artillery.DataProcessor
{
    using Artillery.Data;
    using Artillery.DataProcessor.ExportDto;
    using Microsoft.EntityFrameworkCore;
    using Newtonsoft.Json;
    using System;
    using System.Linq;

    public class Serializer
    {
        public static string ExportShells(ArtilleryContext context, double shellWeight)
        {
            var shells = context
                .Shells
                .Include(x => x.Guns)
                .ThenInclude(x => x.CountriesGuns)
                .ToList()
                .Where(x => x.ShellWeight > shellWeight)
                .Select(x => new
                {
                    ShellWeight = x.ShellWeight,
                    Caliber = x.Caliber,
                    Guns = x.Guns.
                    Where(g => g.GunType.ToString() == "AntiAircraftGun")
                    .Select(g => new
                    {
                        GunType = g.GunType.ToString(),
                        GunWeight = g.GunWeight,
                        BarrelLength = g.BarrelLength,
                        Range = g.Range > 3000 ? "Long-range" : "Regular range",
                    })
                    .OrderByDescending(x => x.GunWeight)
                    .ToArray()
                })
                .OrderBy(x => x.ShellWeight)
                .ToList();
            var result = JsonConvert.SerializeObject(shells, Formatting.Indented);
            return result;
        }

        public static string ExportGuns(ArtilleryContext context, string manufacturer)
        {
            var root = "Guns";
            var guns = context.Guns
                .ToList()
                .Where(x=> x.Manufacturer.ManufacturerName == manufacturer)
                .Select(x=> new ExportGunsXmlModel 
                {
                    Manufacturer = x.Manufacturer.ManufacturerName,
                    GunType = x.GunType.ToString(),
                    GunWeight = x.GunWeight.ToString(),
                    BarrelLength = x.BarrelLength.ToString(),
                    Range = x.Range.ToString(),
                    Countries = x.CountriesGuns
                    .Where(c=> c.Country.ArmySize > 4500000)
                    .Select(c=> new CountryXmlExportModel
                    { 
                        Country = c.Country.CountryName,
                        ArmySize = c.Country.ArmySize.ToString(),
                    })
                    .OrderBy(c=> int.Parse(c.ArmySize))
                    .ToArray()
                })
                .OrderBy(x=> double.Parse(x.BarrelLength))
                .ToArray();
            
            var result = XmlConverter.Serialize<ExportGunsXmlModel>(guns, root);
            //XmlSerializer xmlSerializer = new XmlSerializer(typeof(UserPurchasesExportDto[]),
            //     new XmlRootAttribute(root));
            //var sw = new StringWriter();
            //xmlSerializer.Serialize(sw, users);
            return result.ToString().TrimEnd(); ;

        }
    }
}
