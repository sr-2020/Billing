using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Core.Model
{
    [Table("current_category")]
    public class CurrentCategory : BaseEntity
    {
        [ForeignKey("scoring_category")]
        [Column("scoring_category")]
        public int CategoryId { get; set; }
        public ScoringCategory Category { get; set; }
        [ForeignKey("scoring")]
        [Column("scoring")]
        public int ScoringId { get; set; }
        public Scoring Scoring { get; set; }
        [Column("current_value")]
        public decimal Value { get; set; }

    }
}
