using System.Collections.Generic;
using System;
using System.Text;
using System.Linq;

namespace Core.Model
{
    public class ScoringCategoryCalculate
    {
        public ScoringCategory Category { get; set; }
        public decimal Current { get; set; }


        public void Calculate(List<ScoringFactorCalculate> factors)
        {
            foreach (var factor in factors.Where(f => f.Factor.Category.Id == Category.Id))
            {
                factor.Calculate();
            }
            Current = factors.Where(f => f.Factor.Category.Id == Category.Id).Sum(f => f.Current);
        }

    }
}
