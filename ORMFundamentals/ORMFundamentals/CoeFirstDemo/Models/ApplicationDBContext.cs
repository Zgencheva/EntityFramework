using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoeFirstDemo.Models
{
    internal class ApplicationDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=.; Integrated Security=true; Database = CodeFirstDemo2022");
        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<News> News { get; set; }

        public DbSet<Comment> Comments { get; set; }



    }
}
