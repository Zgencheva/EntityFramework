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

        public TagService(ApplicationDbContext dbContext, IPropertyService propertyService)
            :this(dbContext)
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
                //скъп-евтин
                var averagePrice = this.propertyService.AveragePricePerSquareMEter(property.DistrictId);
                if (property.Price >= averagePrice)
                {
                    var tag = GetTagByName("скъп-имот");
                    property.Tags.Add(tag);
                }
                if (property.Price < averagePrice)
                {
                    var tag = GetTagByName("евтин-имот");
                    property.Tags.Add(tag);
                }
                //стар-нов
                var currentDate = DateTime.Now.AddYears(-15);
                if (property.Year.HasValue && property.Year <= currentDate.Year)
                {
                    Tag tag = GetTagByName("старо-строителство");
                    property.Tags.Add(tag);
                }
                else if (property.Year.HasValue && property.Year >currentDate.Year)
                {
                    Tag tag = GetTagByName("ново-строителство");
                    property.Tags.Add(tag);
                }
                //голям-малък
                var averagePropertySize = this.propertyService.AverageSize(property.DistrictId);
                if (property.Size >= averagePropertySize)
                {
                    Tag tag = GetTagByName("голям-имот");
                    property.Tags.Add(tag);
                }
                else if (property.Size < averagePropertySize)
                {
                    Tag tag = GetTagByName("малък-имот");
                    property.Tags.Add(tag);
                }
                //posleden-pyrvi etaj
                if (property.Floor.HasValue && property.Floor.Value == 1)
                {
                    Tag tag = GetTagByName("първи-етаж");
                    property.Tags.Add(tag);
                }
                else if (property.Floor.HasValue && property.Floor.Value > 6)
                {
                    Tag tag = GetTagByName("хубава-гледка");
                    property.Tags.Add(tag);
                }
            }

            dbContext.SaveChanges();
        }

        private Tag GetTagByName(string name)
        {
            var tag = dbContext.Tags.FirstOrDefault(x => x.Name == name);
            if (tag == null)
            {
                Tag current = new Tag { Name = name};
                dbContext.Tags.Add(tag);
            }

            return tag;
        }
    }
}
