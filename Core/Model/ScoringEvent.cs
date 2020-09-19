using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Core.Model
{
    [Table("scoring_event")]
    public class ScoringEvent : BaseEntity
    {
        [ForeignKey("current_factor")]
        [Column("current_factor")]
        public int CurrentFactorId { get; set; }
        public CurrentFactor CurrentFactor { get; set; }
        [Column("start_time")]
        public DateTime StartTime { get; set; }
        [Column("finish_time")]
        public DateTime FinishTime { get; set; }
    }
}
