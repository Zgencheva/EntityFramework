using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Xml.Serialization;
using TeisterMask.Data.Models.Enums;

namespace TeisterMask.DataProcessor.ImportDto
{
    [XmlType("Task")]
    public class TaskXmlImportModel
    {
        [Required]
        [MaxLength(40)]
        [MinLength(2)]
        [XmlElement("Name")]
        public string Name { get; set; }
        [Required]
        [XmlElement("OpenDate")]
        public string OpenDate { get; set; }
        [Required]
        [XmlElement("DueDate")]
        public string DueDate { get; set; }
        //[EnumDataType(typeof(ExecutionType))]
        [Required]
        [XmlElement("ExecutionType")]
        public int ExecutionType { get; set; }
        //[EnumDataType(typeof(LabelType))]
        [Required]
        [XmlElement("LabelType")]
        public int LabelType { get; set; }
    }
}
