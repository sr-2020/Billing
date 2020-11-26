using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Core.Model
{
    [Table("tmp_billing_init")]
    public class BillingInit : BaseEntity
    {
        [Column("model")]
        public int Model { get; set; }
        [Column("start_cash")]
        public decimal StartCash { get; set; }
        [Column("start_fak")]
        public decimal StartFak { get; set; }
        [Column("citizen")]
        public string Citizenship { get; set; }
        [Column("nation")]
        public string Nation { get; set; }
        [Column("status")]
        public string Status { get; set; }
    }
}
