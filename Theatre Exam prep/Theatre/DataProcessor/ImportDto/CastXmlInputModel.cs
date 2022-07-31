using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Xml.Serialization;

namespace Theatre.DataProcessor.ImportDto
{
    [XmlType("Cast")]
    public class CastXmlInputModel
    {
        [MaxLength(30)]
        [MinLength(4)]
        [Required]
        [XmlElement("FullName")]
        public string FullName { get; set; }
        [Required]
        [XmlElement("IsMainCharacter")]
        public bool IsMainCharacter { get; set; }
        [Required]
        [XmlElement("PhoneNumber")]
        [RegularExpression(@"^\+44-\d{2}-\d{3}-\d{4}$")]
        public string PhoneNumber { get; set; }
        [XmlElement("PlayId")]
        public int PlayId { get; set; }

    }
}
