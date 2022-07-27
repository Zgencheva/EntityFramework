namespace SoftJail.DataProcessor
{
    using Data;
    using Newtonsoft.Json;
    using SoftJail.Data.Models;
    using SoftJail.Data.Models.Enums;
    using SoftJail.DataProcessor.ImportDto;
    using SoftJail.XMLHelper;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Linq;
    using System.Text;

    public class Deserializer
    {
        public static string ImportDepartmentsCells(SoftJailDbContext context, string jsonString)
        {
            var departmentsCells =
                JsonConvert.DeserializeObject<IEnumerable<DepartmentCellInputModel>>(jsonString);
            var sb = new StringBuilder();
            var departments = new List<Department>();
            foreach (var departmentCell in departmentsCells)
            {

                if (!IsValid(departmentCell) 
                    || !departmentCell.Cells.All(IsValid)
                    || departmentCell.Cells.Count == 0)
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }

                var department = new Department
                {
                    Name = departmentCell.Name,
                    Cells = departmentCell.Cells.Select(x => new Cell
                    {
                        CellNumber = x.CellNumber,
                        HasWindow = x.HasWindow,
                    })
                    .ToList()
                };

                departments.Add(department);
                sb.AppendLine($"Imported {department.Name} with {department.Cells.Count} cells");
            }
            context.Departments.AddRange(departments);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportPrisonersMails(SoftJailDbContext context, string jsonString)
        {
            var prisonerMails =
                JsonConvert.DeserializeObject<IEnumerable<PrisonerMailInputModel>>(jsonString);
            var sb = new StringBuilder();
            var prisoners = new List<Prisoner>();
            foreach (var prisoner in prisonerMails)
            {

                if (!IsValid(prisoner)
                    || !prisoner.Mails.All(IsValid))  
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }
                var isValidReleaseDate = DateTime
                    .TryParseExact(prisoner.ReleaseDate, "dd/MM/yyyy",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out DateTime releaseDate);
                
                 var incDate = DateTime
                  .ParseExact(prisoner.IncarcerationDate, "dd/MM/yyyy",
                  CultureInfo.InvariantCulture);
               
               
                var pris = new Prisoner
                {
                    FullName = prisoner.FullName,
                    Nickname = prisoner.Nickname,
                    Age = prisoner.Age,
                    Bail = prisoner.Bail,
                    CellId = prisoner.CellId,
                    ReleaseDate = isValidReleaseDate ? (DateTime?)releaseDate : null,
                    IncarcerationDate = incDate,
                    Mails = prisoner.Mails.Select(x => new Mail
                    {
                        Description = x.Description,
                        Sender = x.Sender,
                        Address = x.Address
                    })
                    .ToList()
                };

                prisoners.Add(pris);
                sb.AppendLine($"Imported {pris.FullName} {pris.Age} years old");
            }
            context.Prisoners.AddRange(prisoners);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportOfficersPrisoners(SoftJailDbContext context, string xmlString)
        {
            StringBuilder sb = new StringBuilder();
            var allPrisoners = context.Prisoners.Select(x => x.Id).ToList();
            string root = "Officers";
            var offiers = new List<Officer>();
            var officersPrisonersDtos = XmlConverter.Deserializer<OfficersPrisonersInputModel>(xmlString, root);
            foreach (var officerPrisoner in officersPrisonersDtos)
            {
                if (!IsValid(officerPrisoner) || !officerPrisoner.OfficerPrisoners.All(IsValid))
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }
                var OffprisonersIds = officerPrisoner.OfficerPrisoners.Select(x => x.Id).Distinct();
                var prisonersIds = OffprisonersIds.Intersect(allPrisoners);

                var officer = new Officer
                {
                    FullName = officerPrisoner.FullName,
                    Salary = officerPrisoner.Salary,
                    Position = Enum.Parse<Position>(officerPrisoner.Position),
                    Weapon = Enum.Parse<Weapon>(officerPrisoner.Weapon),
                    DepartmentId = officerPrisoner.DepartmentId,
                    OfficerPrisoners = officerPrisoner.OfficerPrisoners.Select(x => new OfficerPrisoner
                    {
                        PrisonerId = x.Id
                    })
                    .ToList()
                };

                offiers.Add(officer);
                sb.AppendLine($"Imported {officer.FullName} ({officer.OfficerPrisoners.Count} prisoners)");
            }
            context.Officers.AddRange(offiers);
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }

        private static bool IsValid(object obj)
        {
            var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(obj);
            var validationResult = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(obj, validationContext, validationResult, true);
            return isValid;
        }
    }
}