using FileHelper;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommonExcel.Model
{
    public class ProductsExcelModel
    {
        [Column(0, false)]
        public string SkuName { get; set; }
        [Column(1, false)]
        public string NomenklaturaName { get; set; }
        [Column(2, false)]
        public string ProductType { get; set; }
        [Column(3, false)]
        public string ProductDiscountType { get; set; }
        [Column(4, false)]
        public string Code { get; set; }
        [Column(5, false)]
        public string BasePrice { get; set; }
        [Column(6, false)]
        public string Corporation { get; set; }
        [Column(7, false)]
        public string Enabled { get; set; }
        [Column(8, false)]
        public string Count { get; set; }
        [Column(9, false)]
        public string LifeStyle { get; set; }
        [Column(10, false)]
        public string Description { get; set; }
    }
}
