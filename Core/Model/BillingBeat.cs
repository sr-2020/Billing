using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Core.Model
{
    [Table("billing_beat")]
    public class BillingBeat : BaseEntity
    {
        [Column("start_time")]
        public DateTime StartTime { get; set; }
        [Column("finish_time")]
        public DateTime FinishTime { get; set; }
        [Column("number")]
        public int Number { get; set; }
        [Column("token")]
        public string Token { get; set; }
        [ForeignKey("billing_cycle")]
        [Column("billing_cycle")]
        public int CycleId { get; set; }
        public virtual BillingCycle Cycle { get; set; }
    }
}
