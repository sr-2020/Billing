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
        public int ProductDiscountType { get; set; }
        [Column(4, false)]
        public string Code { get; set; }
        [Column(5, false)]
        public int BasePrice { get; set; }
        [Column(6, false)]
        public int Corporation { get; set; }
        [Column(7, false)]
        public int Enabled { get; set; }
        [Column(8, false)]
        public int Count { get; set; }
        [Column(9, false)]
        public int LifeStyle { get; set; }
        [Column(10, false)]
        public string Description { get; set; }
    }
}
