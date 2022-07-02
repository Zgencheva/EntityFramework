using RealEstates.Services.Models;
using System.Collections.Generic;


namespace RealEstates.Services
{
    public interface IPropertyService
    {
        void Add(string district, int floor, 
            int maxFloor, int size, int yardSize, 
            int year, string propertyType, 
            string buildingType, int price);

        decimal AveragePricePerSquareMEter();

        decimal AveragePricePerSquareMEter(int districtId);
        public IEnumerable<PropertyInfoFullData> GetFullData(int count);

        public double AverageSize(int districtId);
        IEnumerable<PropertyInfoDto> Search(int minPrice, int maxPrice, int minSize, int maxSize);
        
    }
}
