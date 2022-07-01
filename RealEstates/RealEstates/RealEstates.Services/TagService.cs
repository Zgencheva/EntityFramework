using RealEstates.Data;
using RealEstates.Models;
using System;
using System.Linq;

namespace RealEstates.Services
{
    public class TagService : ITagService
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IPropertyService propertyService;
        public TagService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public TagService(IPropertyService propertyService)
            :base()
        {
            this.propertyService = propertyService;
        }
        public void Add(string name, int? importance = null)
        {
            var tag = new Tag
            {
                Name = name,
                Importance = importance,
            };
            this.dbContext.Tags.Add(tag);
            this.dbContext.SaveChanges();
        }

        public void BulkTagToProperties()
        {
            //fetch all properties
            //set tags
            //saveChanges

            var allProperties = dbContext.Properties.ToList();
            foreach (var property in allProperties)
            {
                var averagePrice = this.propertyService.AveragePricePerSquareMEter(property.DistrictId);
                if (property.Price > averagePrice)
                {
                    var tag = dbContext.Tags.FirstOrDefault(x => x.Name == "Скъп-имот");
                    property.Tags.Add(tag);
                }
                if (property.Price < averagePrice)
                {
                    var tag = dbContext.Tags.FirstOrDefault(x => x.Name == "евтин-имот");
                    property.Tags.Add(tag);
                }
            }

            dbContext.SaveChanges();
        }
    }
}
