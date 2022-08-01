using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Xml.Serialization;

namespace Artillery.DataProcessor.ImportDto
{
    [XmlType("Country")]
    public class CountryXmlImportModel
    {
        [Required]
        [MaxLength(60)]
        [MinLength(4)]
        [XmlElement("CountryName")]
        public string CountryName { get; set; }
        [Range(50000, 10000000)]
        [Required]
        [XmlElement("ArmySize")]
        public int ArmySize { get; set; }
    }
}
