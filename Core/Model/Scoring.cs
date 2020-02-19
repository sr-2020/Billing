using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Core.Model
{
    [Table("Scoring")]
    public class Scoring : BaseEntity
    {
        public decimal CurrentScoring { get; set; }
        public List<ScoringCategoryCalculate> CategoryCalculates { get; set; }
        public List<ScoringFactorCalculate> FactorCalculates { get; set; }
    }
}
