using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Core.Model
{
    [Table("event_lifestyle")]
    public class EventLifestyle : BaseEntity
    {
        [ForeignKey("scoring_factor")]
        [Column("scoring_factor")]
        public int ScoringFactorId { get; set; }
        public ScoringFactor ScoringFactor { get; set; }
        [Column("event_number")]
        public int EventNumber { get; set; }
        [Column("value")]
        public decimal Value { get; set; }
    }
}
