using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Core.Model
{
    [Table("scoring")]
    public class Scoring : BaseEntity
    {
        [Column("current_scoring")]
        public decimal CurrentScoring { get; set; }
        public virtual List<ScoringCategoryCalculate> CategoryCalculates { get; set; }
    }
}
