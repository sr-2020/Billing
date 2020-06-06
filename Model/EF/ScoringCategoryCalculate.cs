using System.Collections.Generic;
using System;
using System.Text;
using System.Linq;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Model
{
    [Table("scoring_category_calculate")]
    public class ScoringCategoryCalculate : BaseEntity
    {
        [Column("scoring")]
        [ForeignKey("scoring")]
        public int ScoringId { get; set; }
        public Scoring Scoring { get; set; }
        [Column("category")]
        [ForeignKey("category")]
        public int CategoryId { get; set; }
        public ScoringCategory Category { get; set; }
        [Column("current")]
        public decimal Current { get; set; }

        public virtual List<ScoringFactorCalculate> Factors { get; set; }


        public void Calculate()
        {
            if (Factors == null)
                return;
            foreach (var factor in Factors)
            {
                factor.Calculate();
            }
            Current = Factors.Where(f => f.Factor.Category.Id == Category.Id).Sum(f => f.Current);
        }

    }
}
