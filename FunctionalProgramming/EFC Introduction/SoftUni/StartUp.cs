// See https://aka.ms/new-console-template for more information
using SoftUni.Data;
using SoftUni.Models;
using System.Text;

var db = new SoftUniContext();

Console.WriteLine(GetEmployeesInPeriod(db));


static string GetEmployeesFullInformation(SoftUniContext context)
{
    var sb = new StringBuilder();
    var query = context.Employees
        .Select(y=> new { y.EmployeeId, y.FirstName, y.LastName, y.JobTitle, y.Salary})
        .OrderBy(x => x.EmployeeId)
        .ToList();

    foreach (var employee in query)
    {
        sb.AppendLine($"{employee.FirstName} {employee.LastName} {employee.JobTitle} {employee.Salary:f2}");

    }
    return sb.ToString();
}

static string GetEmployeesWithSalaryOver50000(SoftUniContext context) { 
        
    var sb = new StringBuilder();
    var query = context.Employees
    .Select(y => new { y.EmployeeId, y.FirstName, y.Salary })
    .Where(y=> y.Salary> 50000)
    .OrderBy(x => x.FirstName)
    .ToList();

    foreach (var employee in query)
    {
        sb.AppendLine($"{employee.FirstName} - {employee.Salary:f2}");

    }
    return sb.ToString().TrimEnd();
}

static string GetEmployeesFromResearchAndDevelopment(SoftUniContext context)
{

    var sb = new StringBuilder();
    var query = context.Employees
    .Select(y => new { y.EmployeeId, y.FirstName, y.LastName, y.Department, y.Salary })
    .Where(x=> x.Department.Name == "Research and Development")
    .OrderBy(x => x.Salary)
    .ThenByDescending(y=> y.FirstName)
    .ToList();

    foreach (var employee in query)
    {
        sb.AppendLine($"{employee.FirstName} {employee.LastName} " +
            $"from {employee.Department.Name} - ${employee.Salary:f2}");

    }
    return sb.ToString().TrimEnd();
}

static string AddNewAddressToEmployee(SoftUniContext context) {
    StringBuilder sb = new StringBuilder();
    var employee = context.Employees.FirstOrDefault(x => x.LastName == "Nakov");

    employee.Address = new Address()
    {
        AddressText = "Vitoshka 15",
        TownId = 4,
    };

    context.SaveChanges();

    var query = context.Employees
    .Select(y => new { y.Address })
    .OrderByDescending(x => x.Address)
    .ToList()
    .Take(10);

    foreach (var item in query)
    {
        sb.AppendLine(item.Address.AddressText.ToString());
    }
    return sb.ToString().TrimEnd();
}

static string GetEmployeesInPeriod(SoftUniContext context) {
    StringBuilder sb = new StringBuilder();

    var query = context.Employees
    .Select(y => new { y.FirstName, y.LastName, 
        ManagerFirstName = y.Manager.FirstName == null ? "" : y.Manager.FirstName,
    ManagerLastName = y.Manager.LastName == null ? "" : y.Manager.LastName,
    ProjectStartDate = y.Projects.Where(x => 2001 <= x.StartDate.Year && x.StartDate.Year <= 2003)
    })
    .ToList()
    .Take(10);

    foreach (var emp in query)
    {
        sb.AppendLine($"{emp.FirstName} {emp.LastName} - Manager: {emp.ManagerFirstName} {emp.ManagerLastName}");
        foreach (var project in emp.ProjectStartDate)
        {
            if (project.EndDate == null)
            {
                sb.AppendLine($"-- {project.Name} - {project.StartDate} - not finished");
            }
            else
            {
                sb.AppendLine($"-- {project.Name} - {project.StartDate} - {project.EndDate}");
            }
           
        }
    }
    return sb.ToString().TrimEnd();
}

static string GetAddressesByTown(SoftUniContext context) {
    StringBuilder sb = new StringBuilder();

    var query = context.Addresses
        .OrderByDescending(x => x.Employees.Count)
        .ThenBy(x => x.Town)
        .ThenBy(x => x.AddressText)
        .Select(x => new { x.AddressText, townName = x.Town.Name, empCount = x.Employees.Count })
        .Take(10);

    foreach (var address in query)
    {
        sb.AppendLine($"{address.AddressText}, {address.townName} - {address.empCount} employees");
    }
    return sb.ToString();
}

static string GetEmployee147(SoftUniContext context) {
    StringBuilder sb = new StringBuilder();
    var query = context.Employees
        .Where(x=> x.EmployeeId == 147)
        .Select(x => new { x.FirstName, x.LastName, x.JobTitle, x.Projects });


    foreach (var employee in query)
    {
        sb.AppendLine($"{employee.FirstName}, {employee.LastName} - {employee.JobTitle}");
        foreach (var project in employee.Projects.OrderBy(x=> x.Name))
        {
            sb.AppendLine(project.Name.ToString());
        }
    }

    return sb.ToString();
}

static string GetDepartmentsWithMoreThan5Employees(SoftUniContext context)
{
    StringBuilder sb = new StringBuilder();
    var query = context.Departments
        .Where(x => x.Employees.Count > 5)
        .OrderBy(x => x.Employees.Count)
        .ThenBy(x=> x.Name)
        .Select(x => new {x.Name, 
            ManagerFirstName = x.Manager.FirstName, 
            ManagerLastNAme = x.Manager.LastName, x.Employees});


    foreach (var department in query)
    {
        sb.AppendLine($"{department.Name} - {department.ManagerFirstName} {department.ManagerLastNAme}");
        foreach (var employee in department.Employees.OrderBy(x => x.FirstName))
        { 
        
            sb.AppendLine($"{employee.FirstName} {employee.LastName} - {employee.JobTitle}");
        }
    }

    return sb.ToString();
}

static string GetLatestProjects(SoftUniContext context)
{
    StringBuilder sb = new StringBuilder();
    var query = context.Projects
        .OrderByDescending(x => x.StartDate)
        .Select(x=> new { x.Name, x.Description, x.StartDate})
        .Take(10);
    foreach (var project in query.OrderBy(x=> x.Name))
    {
        sb.AppendLine($"{project.Name}\n{project.Description}\n{project.StartDate.ToString("M/d/yyyy h:mm:ss tt")}");


    }
      

    return sb.ToString();
}

static string IncreaseSalaries(SoftUniContext context)
{
    StringBuilder sb = new StringBuilder();
    var employees = context.Employees.Where(x => x.Department.Name == "Engineering"
    || x.Department.Name == "Tool Design"
    || x.Department.Name == "Marketing "
    || x.Department.Name == "Information Services")
        .OrderBy(x => x.FirstName)
        .ThenBy(x => x.LastName);
     
    foreach (var employee in employees)
    {


        employee.Salary = employee.Salary * 1.2M;
        sb.AppendLine($"{employee.FirstName} {employee.LastName} ${employee.Salary:f2}");
        
        
    }
    return sb.ToString();
}

static string GetEmployeesByFirstNameStartingWithSa(SoftUniContext context)
{
    StringBuilder sb = new StringBuilder();
    var query = context.Employees
        .Where(x => x.FirstName.StartsWith("Sa"))
        .Select(x => new { x.FirstName, x.LastName, x.JobTitle, x.Salary })
        .OrderBy(x=> x.FirstName)
        .ThenBy(l=> l.LastName);


    foreach (var employee in query)
    {
        sb.AppendLine($"{employee.FirstName} {employee.LastName} - {employee.JobTitle} {employee.Salary:f2}");
   
    }

    return sb.ToString();
}

static string DeleteProjectById(SoftUniContext context)
{
    StringBuilder sb = new StringBuilder();

    var project = context.Projects.First(p => p.ProjectId == 2);


    context.Projects.Remove(project);

    context.SaveChanges();


    return sb.ToString().TrimEnd();
}

static string RemoveTown(SoftUniContext context)
{
    StringBuilder sb = new StringBuilder();
    var town = context.Towns.FirstOrDefault(x=> x.Name == "Seattle");
    var addresses = context.Addresses.Where(x=> x.Town.Name == "Seattle").ToList();
    var addressesCount = addresses.Count;
    var allAddressIds = addresses.Select(x=> x.AddressId).ToList();

    var employees = context.Employees.Where(x => allAddressIds.Contains(x.AddressId.Value));

    foreach (var empl in employees)
    {
        empl.Address = null;
    }
    context.SaveChanges();
    foreach (var addressId in allAddressIds)
    {
        var current = context.Addresses.FirstOrDefault(x=> x.AddressId == addressId);
        context.Addresses.Remove(current);
    }
    context.Towns.Remove(town);

    context.SaveChanges();
    sb.AppendLine($"{addressesCount} addresses in Seattle were deleted");

    return sb.ToString().TrimEnd();
}

