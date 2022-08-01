﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Xml.Serialization;

namespace Artillery.DataProcessor.ImportDto
{
    [XmlType("Shell")]
    public class ShellExmlImportDto
    {
        [Required]
        [Range(2, 1680)]
        [XmlElement("ShellWeight")]
        public double ShellWeight { get; set; }
        [Required]
        [MaxLength(30)]
        [MinLength(4)]
        [XmlElement("Caliber")]
        public string Caliber { get; set; }
    }
}
