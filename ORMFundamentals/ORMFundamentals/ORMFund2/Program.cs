// See https://aka.ms/new-console-template for more information
using ORMFund2.Models;



var db = new SoftUniContext();
var selectedDepartments = db.Departments
    .Where(x => x.Employees.Any(e => e.FirstName == "Niki"))
    .ToList();
var selectedEmployees = db.Employees
    .Where(x=> x.Manager.FirstName == "Niki")
    .ToList();
foreach (var department in selectedDepartments)
{
    department.ManagerId = null;
}
foreach (var employee in selectedEmployees)
{
    db.Employees.Remove(employee);
}
//var employees = db.Employees.ToList();
//foreach (var employee in employees)
//{
//    employee.Salary *= 1.1M;
//}

//db.SaveChanges();
//var employees = db.Employees.ToList(); //All employees
////db.Towns.Add(new Town { Name = "Pernik"});
////db.SaveChanges();
//Console.WriteLine(db.Employees.Count());
//var departments = db.Employees.GroupBy(x=>x.Department.Name)
//    .Select(x=> new { Name = x.Key, Count = x.Count() })
//    .ToList();
//foreach (var department in departments)

//{
//    Console.WriteLine($"{department.Name} -> {department.Count}");
//}
