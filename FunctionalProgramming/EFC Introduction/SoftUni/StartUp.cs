// See https://aka.ms/new-console-template for more information
using SoftUni.Data;
using System.Text;

var db = new SoftUniContext();

Console.WriteLine(GetEmployeesFromResearchAndDevelopment(db));

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

