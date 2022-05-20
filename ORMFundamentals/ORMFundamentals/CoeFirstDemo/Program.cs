// See https://aka.ms/new-console-template for more information
using CoeFirstDemo.Models;

Console.WriteLine("Hello, World!");

var db = new ApplicationDbContext();
db.Database.EnsureCreated();