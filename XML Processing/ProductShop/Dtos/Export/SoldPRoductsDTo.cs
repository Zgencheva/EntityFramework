using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace ProductShop.Dtos.Export
{
    [XmlType("SoldProducts")]
    public class SoldPRoductsDTo
    {
        [XmlElement("Count")]
        public int Count { get; set; }
        [XmlArray("products")]
        public ExportProductDTO[] Products { get; set; }
    }
}
