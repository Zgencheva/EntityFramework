using System;
using System.Collections.Generic;
using System.Text;

namespace ProductShop.DataTransferObjects
{
    public class ProductExportDto
    {
        public string name { get; set; }
        public decimal price { get; set; }

        public string seller { get; set; }
    }
}
