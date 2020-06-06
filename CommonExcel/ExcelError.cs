using System;
using System.ComponentModel.DataAnnotations;

namespace CommonExcel
{
    public class ExcelError
    {
        [Display(Name = "Error")]
        public string Error { get; set; }
        [Display(Name = "RowNumber")]
        public int RowNumber { get; set; }
        [Display(Name = "RowString")]
        public string RowString { get; set; }
    }
}
