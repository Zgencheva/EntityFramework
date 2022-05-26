// See https://aka.ms/new-console-template for more information
using CoeFirstDemo.Models;

Console.WriteLine("Hello, World!");

//var db = new ApplicationDbContext();
//db.Database.EnsureCreated();
Console.WriteLine(db.Comments.Count());
//db.Categories.Add(new Category
//{
//    Title = "Sport",
//    News = new List<News>
//        {
//        new News
//            {
//            Title="CSKA ne bie Levski",
//            Content = "Yes, true",
//            Comments = new List<Comment>
//                {
//                    new Comment { Author = "Zori", Content = "yes, yes"},
//                    new Comment { Author="Nen", Content = "no, no"}
                    
//                }
//            }

//        }

//}

//    ) ;
//db.SaveChanges();

//var news = db.News.Select(x => new
//{
//    Name = x.Title,
//    CategoryName = x.Category.Title
//}).ToList();

//foreach (var singleNew in news)
//{
//    Console.WriteLine(singleNew.CategoryName + " => " + singleNew.Name);
//}
//Migration - update DB without breaking the current data!
