using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Xml.Serialization;
using VaporStore.Data.Models.Enums;

namespace VaporStore.DataProcessor.Dto.Import
{
    [XmlType("Purchase")]
    public class PurchaseInputDto
    {
        [XmlAttribute("title")]
        [Required]
        public string Title { get; set; }
        [XmlElement("Type")]
        [EnumDataType(typeof(PurchaseType))]
        [Required]
        public PurchaseType? Type { get; set; }
        [XmlElement("Key")]
        [Required]
        [RegularExpression(@"^[A-Z\d]{4}-[A-Z\d]{4}-[A-Z\d]{4}$")]
        public string Key { get; set; }
        [XmlElement("Card")]
        [Required]
        [RegularExpression(@"^\d{4}\s\d{4}\s\d{4}\s\d{4}$")]
        public string Card { get; set; }
        [XmlElement("Date")]
        [Required]
        public string Date { get; set; }

    }
}
