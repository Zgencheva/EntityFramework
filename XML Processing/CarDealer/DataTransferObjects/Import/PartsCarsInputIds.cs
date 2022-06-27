using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace CarDealer.DataTransferObjects.Import
{
    [XmlType("partId")]
    public class PartsCarsInputIds
    {
        [XmlAttribute]
        public int Id { get; set; }
    }
}
