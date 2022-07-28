namespace SoftJail.DataProcessor
{
    using AutoMapper.QueryableExtensions;
    using Data;
    using Newtonsoft.Json;
    using SoftJail.DataProcessor.ExportDto;
    using SoftJail.XMLHelper;
    using System;
    using System.Globalization;
    using Microsoft.EntityFrameworkCore;
    using System.Linq;
    using System.Text;

    public class Serializer
    {
        public static string ExportPrisonersByCells(SoftJailDbContext context, int[] ids)
        {
            var prisoners = context.Prisoners
                .Where(x => ids.Contains(x.Id))
                .Select(x => new
                {
                    Id = x.Id,
                    Name = x.FullName,
                    CellNumber = x.Cell.CellNumber,
                    Officers = x.PrisonerOfficers.Select(o => new
                    {
                        OfficerName = o.Officer.FullName,
                        Department = o.Officer.Department.Name,
                    })
                    .OrderBy(r=> r.OfficerName)
                    .ToList(),
                    TotalOfficerSalary = decimal.Parse(x.PrisonerOfficers.Sum(s => s.Officer.Salary)
                    .ToString("F2")) 
                })
                .OrderBy(x=> x.Name)
                .ThenBy(x=> x.Id)
                .ToList();

            var result = JsonConvert.SerializeObject(prisoners, Formatting.Indented);
            return result;
        }

        public static string ExportPrisonersInbox(SoftJailDbContext context, string prisonersNames)
        {
            var root = "Prisoners";
            var names = prisonersNames.Split(",").ToArray();
            var prisoners = context.Prisoners
                .Include(y => y.Mails)
                 .Where(x => names.Contains(x.FullName))
                 .ProjectTo<PrisonerMailsExportModel>()
                 .OrderBy(x=> x.Name)
                 .ThenBy(x=> x.Id)
                 .ToArray();
            var result = XmlConverter.Serialize<PrisonerMailsExportModel>(prisoners, root);
            return result.ToString().TrimEnd();
        }

        private static string ReverseDescription(string description)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = description.Length -1; i >= 0; i--)
            {
                sb.Append(description[i]);
            }

            return sb.ToString();
        }
    }
}