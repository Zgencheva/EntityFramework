using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Xml.Serialization;

namespace TeisterMask.DataProcessor.ImportDto
{
    [XmlType("Project")]
    public class ProjectXmlImportModel
    {
        [Required]
        [XmlElement("Name")]
        [MaxLength(40)]
        [MinLength(2)]
        public string Name { get; set; }
        [Required]
        [XmlElement("OpenDate")]
        public string OpenDate { get; set; }
        [XmlElement("DueDate")]
        public string DueDate { get; set; }
        [XmlArray("Tasks")]
        public TaskXmlImportModel[] Tasks { get; set; }
    }
}
