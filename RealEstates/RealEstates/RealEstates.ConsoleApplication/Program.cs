using Microsoft.EntityFrameworkCore;
using RealEstates.Data;
using RealEstates.Models;
using System;

namespace RealEstates.ConsoleApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            var db = new ApplicationDbContext();
            db.Database.Migrate();

            while (true)
            {
                Console.Clear();
                Console.WriteLine("Choose an Option:");
                Console.WriteLine("1. Property search");
                Console.WriteLine("2. Most expensive districts");
                bool parsed = int.TryParse(Console.ReadLine(), out int option);
                if (parsed && (option >= 1 || option <=2))
                {
                    switch (option)
                    {
                        case 1: PropertySearch();
                            break;
                        case 2:
                            MostExpensiveDistricts();
                                break;
                  
                    }

                    Console.WriteLine("Press any key to continue..");
                    Console.ReadKey();
                }
                
            }
            
        }

        private static void MostExpensiveDistricts()
        {
            
        }

        private static void PropertySearch()
        {
           
        }
    }
}
