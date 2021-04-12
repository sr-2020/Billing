using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Core.Model
{
    [Table("beat_history")]
    public class BeatHistory : BaseEntity
    {
        [Column("comment")]
        public string Comment { get; set; }
        [ForeignKey("billing_beat")]
        [Column("billing_beat")]
        public int BeatId { get; set; }
        public virtual BillingBeat Beat { get; set; }
    }
}
