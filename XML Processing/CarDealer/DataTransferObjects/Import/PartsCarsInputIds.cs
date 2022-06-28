using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace CarDealer.DataTransferObjects.Import
{
    [XmlType("partId")]
    public class PartsCarsInputIds
    {
        [XmlAttribute("id")]
        public int Id { get; set; }
    }
}
