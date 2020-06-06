using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Core.Model
{
    [Table("scoring_factor_calculate")]
    public class ScoringFactorCalculate : BaseEntity
    {
        [Column("factor")]
        [ForeignKey("factor")]
        public int FactorId { get; set; }
        public ScoringFactor Factor { get; set; }
        [Column("base")]
        public decimal Base { get; set; }
        [Column("current")]
        public decimal Current { get; set; }
        public void Calculate()
        {
            Current = Base + 1 * Factor.Algorythm;
        }
    }
}
