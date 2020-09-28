using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Core.Model
{
    [Table("billing_cycle")]
    public class BillingCycle : BaseEntity
    {
        [Column("start_time")]
        public DateTime StartTime { get; set; }
        [Column("finish_time")]
        public DateTime FinishTime { get; set; }
        [Column("rents")]
        public int Rents { get; set; }
    }
}
