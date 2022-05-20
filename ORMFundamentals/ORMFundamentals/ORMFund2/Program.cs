// See https://aka.ms/new-console-template for more information
using ORMFund2.Models;



var db = new SoftUniContext();
//var employees = db.Employees.ToList(); //All employees
////db.Towns.Add(new Town { Name = "Pernik"});
////db.SaveChanges();
//Console.WriteLine(db.Employees.Count());
var departments = db.Employees.GroupBy(x=>x.Department.Name)
    .Select(x=> new { Name = x.Key, Count = x.Count() })
    .ToList();
foreach (var department in departments)

{
    Console.WriteLine($"{department.Name} -> {department.Count}");
}
