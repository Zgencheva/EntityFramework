namespace TeisterMask.DataProcessor
{
    using System;
    using System.Globalization;
    using System.Linq;
    using Data;
    using Microsoft.EntityFrameworkCore;
    using Newtonsoft.Json;
    using TeisterMask.DataProcessor.ExportDto;
    using Formatting = Newtonsoft.Json.Formatting;

    public class Serializer
    {
        public static string ExportProjectWithTheirTasks(TeisterMaskContext context)
        {
            var root = "Projects";
            var projects = context.Projects
                .Include(x => x.Tasks)
                .ToList()
                .Where(x => x.Tasks.Count() > 0)
                .Select(x => new ProjectWithTasksExportModel
                {
                    TasksCount = x.Tasks.Count(),
                    ProjectName = x.Name,
                    HasEndDate = x.DueDate.HasValue ? "Yes" : "No",
                    Tasks = x.Tasks.Select(t=> new TaskExportModel 
                    {
                        Name = t.Name,
                        Label = t.LabelType.ToString(),
                    })
                    .OrderBy(x=> x.Name)
                    .ToArray()

                })
                .OrderByDescending(x=> x.TasksCount)
                .ThenBy(x=> x.ProjectName)
                .ToArray();
         
           

            var result = XmlConverter.Serialize<ProjectWithTasksExportModel>(projects, root);
            //XmlSerializer xmlSerializer = new XmlSerializer(typeof(UserPurchasesExportDto[]),
            //     new XmlRootAttribute(root));
            //var sw = new StringWriter();
            //xmlSerializer.Serialize(sw, users);
            return result.ToString().TrimEnd(); ;
        }

        public static string ExportMostBusiestEmployees(TeisterMaskContext context, DateTime date)
        {
            var employees = context.Employees
                .Include(x => x.EmployeesTasks)
                .ThenInclude(x => x.Task)
                .ToList()
                .Where(x => x.EmployeesTasks.Any(t => t.Task.OpenDate >= date))
                .Select(x => new
                {
                    Username = x.Username,
                    Tasks = x.EmployeesTasks
                            .Where(et => et.Task.OpenDate >= date)
                            .Select(t => new
                            {
                                TaskName = t.Task.Name,
                                OpenDate = t.Task.OpenDate.ToString("d", CultureInfo.InvariantCulture),
                                DueDate = t.Task.DueDate.ToString("d", CultureInfo.InvariantCulture),
                                LabelType = t.Task.LabelType.ToString(),
                                ExecutionType = t.Task.ExecutionType.ToString(),
                            })
                            .OrderByDescending(x=> DateTime.ParseExact(x.DueDate, "d", CultureInfo.InvariantCulture))
                            .ThenBy(x=> x.TaskName)
                            .ToArray()
                })
                .OrderByDescending(x=> x.Tasks.Count())
                .ThenBy(x=> x.Username)
                .Take(10)
                .ToList();
            var result = JsonConvert.SerializeObject(employees, Formatting.Indented);
            return result;

        }
    }
}