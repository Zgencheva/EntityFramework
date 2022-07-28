using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using VaporStore.Data.Models.Enums;

namespace VaporStore.DataProcessor.Dto.Import
{
    public class CardInputModel
    {
        [Required]
        [RegularExpression(@"^\d{4}\s\d{4}\s\d{4}\s\d{4}$")]
        public string Number { get; set; }
        [Required]
        [RegularExpression(@"^\d{3}$")]
        public string CVC { get; set; }
        [Required]
        [EnumDataType(typeof(CardType))]
        public string Type { get; set; }

    }
}
