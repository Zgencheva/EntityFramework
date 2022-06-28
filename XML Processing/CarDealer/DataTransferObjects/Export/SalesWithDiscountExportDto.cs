using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace CarDealer.DataTransferObjects.Export
{
    [XmlType("sale")]
    public class SalesWithDiscountExportDto
    {
        [XmlElement("car")]
        public CarExportDto Car { get; set; }
        [XmlElement("discount")]
        public decimal Disctount{ get; set; }
        [XmlElement("customer-name")]
        public string CustomerName{ get; set; }
        [XmlElement("price")]
        public decimal Price { get; set; }

        [XmlElement("price-with-discount")]
        public decimal PriceWithDiscount { get; set; }
    }
}
