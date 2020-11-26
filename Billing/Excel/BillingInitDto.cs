using CommonExcel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Billing.Excel
{
    public class BillingInitDto
    {
        [Column(0, false)]
        public string Model { get; set; }
        [Column(1, false)]
        public string StartCash { get; set; }
        [Column(2, false)]
        public string StartFak { get; set; }
        [Column(3, false)]
        public string Citizenship { get; set; }
        [Column(4, false)]
        public string Nation { get; set; }
        [Column(5, false)]
        public string Status { get; set; }
    }
}
