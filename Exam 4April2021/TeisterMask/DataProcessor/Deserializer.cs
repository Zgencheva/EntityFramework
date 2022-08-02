namespace TeisterMask.DataProcessor
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using Data;
    using Newtonsoft.Json;
    using TeisterMask.Data.Models;
    using TeisterMask.Data.Models.Enums;
    using TeisterMask.DataProcessor.ImportDto;
    using ValidationContext = System.ComponentModel.DataAnnotations.ValidationContext;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedProject
            = "Successfully imported project - {0} with {1} tasks.";

        private const string SuccessfullyImportedEmployee
            = "Successfully imported employee - {0} with {1} tasks.";

        public static object XmlConverter { get; private set; }

        public static string ImportProjects(TeisterMaskContext context, string xmlString)
        {
            var root = "Projects";
            var projects = new List<Project>();
            StringBuilder sb = new StringBuilder();
            //var purchaseDtos = XmlConverter.Deserializer<ProjectXmlImportModel>(xmlString, root);
            var propjectDtos = TeisterMask.DataProcessor.XmlConverter
                .Deserializer<ProjectXmlImportModel>(xmlString, root);
            foreach (var propjectDto in propjectDtos)
            {
                if (!IsValid(propjectDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }
                var openDate = DateTime.TryParseExact(propjectDto.OpenDate, 
                                "dd/MM/yyyy", 
                                CultureInfo.InvariantCulture, 
                                DateTimeStyles.None, 
                                out DateTime resultOpenDate);
                
                if (!openDate)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }
                var currentProject = new Project
                {
                    Name = propjectDto.Name,
                    OpenDate = resultOpenDate,                    
                };
                var dueDate = DateTime.TryParseExact(propjectDto.DueDate,
                            "dd/MM/yyyy",
                            CultureInfo.InvariantCulture,
                            DateTimeStyles.None,
                            out DateTime resultDueDate);
                if (dueDate)
                {
                    currentProject.DueDate = resultDueDate;
                }

                foreach (var taskDto in propjectDto.Tasks)
                {
                    if (!IsValid(taskDto))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }
                    var taskOpenDate = DateTime.TryParse(taskDto.OpenDate, out var openDateResult);
                    var taskDueDate = DateTime.TryParse(taskDto.DueDate, out var dueDateResult);
                    if (taskOpenDate == false || taskDueDate == false)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }
                    if (openDateResult < currentProject.OpenDate)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }
                    if (currentProject.DueDate.HasValue && dueDateResult > currentProject.DueDate.Value)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }
                    var currentTask = new Task
                    {
                        Name = taskDto.Name,
                        OpenDate = openDateResult,
                        DueDate = dueDateResult,
                        ExecutionType = (ExecutionType)taskDto.ExecutionType,
                        LabelType = (LabelType)taskDto.LabelType,

                    };
                    currentProject.Tasks.Add(currentTask);
                }
                projects.Add(currentProject);
                sb.AppendLine(string.Format(SuccessfullyImportedProject, currentProject.Name, currentProject.Tasks.Count()));
            }
            context.Projects.AddRange(projects);
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }

        public static string ImportEmployees(TeisterMaskContext context, string jsonString)
        {
            var employeeDtos =
                JsonConvert.DeserializeObject<IEnumerable<EmployeesJsonImportModel>>(jsonString);
            var sb = new StringBuilder();
            var employees = new List<Employee>();
            foreach (var employeeDto in employeeDtos)
            {

                if (!IsValid(employeeDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }
              
                var currentEmployee = new Employee
                {
                   Username = employeeDto.Username,
                   Email = employeeDto.Email,
                   Phone = employeeDto.Phone,
                };
                context.Employees.Add(currentEmployee);
                context.SaveChanges();
                var taskIds = employeeDto.Tasks.Distinct();
                var employeeTasks = new List<EmployeeTask>();
                foreach (var taskId in taskIds)
                {
                    if (!context.Tasks.Any(x=> x.Id == taskId))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }
                    var currentEmployeeTask = new EmployeeTask
                    {
                        TaskId = taskId,
                        EmployeeId = currentEmployee.Id,
                        
                    };

                    employeeTasks.Add(currentEmployeeTask);
                }
                context.EmployeesTasks.AddRange(employeeTasks);
                sb.AppendLine(string.Format(SuccessfullyImportedEmployee, currentEmployee.Username, employeeTasks.Count()));
            }

            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}