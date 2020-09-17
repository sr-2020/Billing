using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Core.Model
{
    [Table("current_factor")]
    public class CurrentFactor : BaseEntity
    {
        [ForeignKey("scoring_factor")]
        [Column("scoring_factor")]
        public int ScoringFactorId { get; set; }
        public ScoringFactor ScoringFactor { get; set; }
        [ForeignKey("scoring")]
        [Column("scoring")]
        public int ScoringId { get; set; }
        public Scoring Scoring { get; set; }
        [Column("current_value")]
        public decimal Value { get; set; }
    }
}
