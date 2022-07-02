﻿using System.Collections.Generic;

namespace RealEstates.Services.Models
{
    public class PropertyInfoFullData
    {
        public int Id { get; set; }
        public string DistrictName { get; set; }
        public int Size { get; set; }
        public int Year { get; set; }
        public int? Price { get; set; }
        public string PropertyType { get; set; }
        public string BuildingType { get; set; }
        public IEnumerable<TagInfoDto> Tags { get; set; }
    }
}