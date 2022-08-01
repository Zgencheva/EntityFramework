namespace Artillery.DataProcessor
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Text;
    using Artillery.Data;
    using Artillery.Data.Models;
    using Artillery.Data.Models.Enums;
    using Artillery.DataProcessor.ImportDto;
    using Newtonsoft.Json;

    public class Deserializer
    {
        private const string ErrorMessage =
                "Invalid data.";
        private const string SuccessfulImportCountry =
            "Successfully import {0} with {1} army personnel.";
        private const string SuccessfulImportManufacturer =
            "Successfully import manufacturer {0} founded in {1}.";
        private const string SuccessfulImportShell =
            "Successfully import shell caliber #{0} weight {1} kg.";
        private const string SuccessfulImportGun =
            "Successfully import gun {0} with a total weight of {1} kg. and barrel length of {2} m.";

        public static string ImportCountries(ArtilleryContext context, string xmlString)
        {
            StringBuilder sb = new StringBuilder();
            var root = "Countries";
            var countries = new List<Country>();
            var countryDtos = XmlConverter.Deserializer<CountryXmlImportModel>(xmlString, root);
            foreach (var countryDto in countryDtos)
            {
                if (!IsValid(countryDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }
              

                var currentCountry = new Country
                {
                   CountryName = countryDto.CountryName,
                   ArmySize = countryDto.ArmySize,
                };

                countries.Add(currentCountry);
                sb.AppendLine(String.Format(SuccessfulImportCountry, currentCountry.CountryName, currentCountry.ArmySize));
            }
            context.Countries.AddRange(countries);
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }

        public static string ImportManufacturers(ArtilleryContext context, string xmlString)
        {
            StringBuilder sb = new StringBuilder();
            var root = "Manufacturers";
            var manufacturers = new List<Manufacturer>();
            var manufacturerDtos = XmlConverter.Deserializer<ManufacturerXmlImportDto>(xmlString, root);
            foreach (var manufacturerDto in manufacturerDtos)
            {
                if (!IsValid(manufacturerDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }
                if (manufacturers.Any(x=> x.ManufacturerName == manufacturerDto.ManufacturerName))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }
                var foundedToStringArray = manufacturerDto.Founded.Split(", ").ToArray();
                var foundedCountry = foundedToStringArray[foundedToStringArray.Length - 1];
                var foundedTown = foundedToStringArray[foundedToStringArray.Length - 2];

                var currentManufacturer = new Manufacturer
                {
                   ManufacturerName = manufacturerDto.ManufacturerName,
                   Founded = $"{foundedTown}, {foundedCountry}",
                };

                manufacturers.Add(currentManufacturer);
                sb.AppendLine(String.Format(SuccessfulImportManufacturer, currentManufacturer.ManufacturerName, currentManufacturer.Founded));
            }
            context.Manufacturers.AddRange(manufacturers);
            context.SaveChanges();
            return sb.ToString().TrimEnd();

        }

        public static string ImportShells(ArtilleryContext context, string xmlString)
        {
            StringBuilder sb = new StringBuilder();
            var root = "Shells";
            var shells = new List<Shell>();
            var shellDtos = XmlConverter.Deserializer<ShellExmlImportDto>(xmlString, root);
            foreach (var shellDto in shellDtos)
            {
                if (!IsValid(shellDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }
              
               

                var currentShell = new Shell
                {
                   ShellWeight = shellDto.ShellWeight,
                   Caliber = shellDto.Caliber,
                };

                shells.Add(currentShell);
                sb.AppendLine(String.Format(SuccessfulImportShell, currentShell.Caliber, currentShell.ShellWeight));

            }
            context.Shells.AddRange(shells);
            context.SaveChanges();
            return sb.ToString().TrimEnd();

        }

        public static string ImportGuns(ArtilleryContext context, string jsonString)
        {
            var gunsDtos =
            JsonConvert.DeserializeObject<IEnumerable<GunsJsonImportModel>>(jsonString);
            var sb = new StringBuilder();
            var guns = new List<Gun>();
            foreach (var gunDto in gunsDtos)
            {

                if (!IsValid(gunDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

               var currentGun = new Gun
                {
                   ManufacturerId = gunDto.ManufacturerId,
                   GunWeight = gunDto.GunWeight,
                   BarrelLength = gunDto.BarrelLength,
                   Range = gunDto.Range,
                   GunType = Enum.Parse<GunType>(gunDto.GunType),
                   ShellId = gunDto.ShellId,
                   CountriesGuns = gunDto.Countries.Select(x=> new CountryGun 
                   {
                        CountryId = x.Id
                   })
                   .ToArray()

               };
                if (gunDto.NumberBuild.HasValue)
                {
                    currentGun.NumberBuild = gunDto.NumberBuild.Value;
                }
                else
                {
                    currentGun.NumberBuild = null;
                }
                guns.Add(currentGun);
                sb.AppendLine(String.Format(SuccessfulImportGun, gunDto.GunType, gunDto.GunWeight, gunDto.BarrelLength));

            }
            context.Guns.AddRange(guns);
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }
        private static bool IsValid(object obj)
        {
            var validator = new ValidationContext(obj);
            var validationRes = new List<ValidationResult>();

            var result = Validator.TryValidateObject(obj, validator, validationRes, true);
            return result;
        }
    }
}
