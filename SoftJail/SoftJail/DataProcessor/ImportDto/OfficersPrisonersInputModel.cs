using SoftJail.Data.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Xml.Serialization;

namespace SoftJail.DataProcessor.ImportDto
{
    [XmlType("Officer")]
    public class OfficersPrisonersInputModel
    {
        [Required]
        [XmlElement("Name")]
        [StringLength(30, MinimumLength = 3)]
        public string FullName { get; set; }
        [Required]
        [XmlElement("Money")]
        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        public decimal Salary  { get; set; }
        [Required]
        [XmlElement("Position")]
        [EnumDataType(typeof(Position))]
        public string Position { get; set; }
        [Required]
        [XmlElement("Weapon")]
        [EnumDataType(typeof(Weapon))]
        public string Weapon { get; set; }
        [Required]
        [XmlElement("DepartmentId")]

        public int DepartmentId { get; set; }
        [XmlArray("Prisoners")]
        public PrisonerIdInputModel[] OfficerPrisoners { get; set; }

    }

  //  <Officer>
		//<Name>Minerva Kitchingman</Name>
		//<Money>2582</Money>
		//<Position>Invalid</Position>
		//<Weapon>ChainRifle</Weapon>
		//<DepartmentId>2</DepartmentId>
		//<Prisoners>
		//	<Prisoner id = "15" />

  //      </ Prisoners >

  //  </ Officer >
}
