using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Model
{
    public class ScoringFactorCalculate
    {
        public int SinId { get; set; }
        public SINDetails Sin { get; set; }
        public int FactorId { get; set; }
        public ScoringFactor Factor { get; set; }
        public decimal Base { get; set; }
        public decimal Current { get; set; }
        public void Calculate()
        {
            Current = Base + 1 * Factor.Algorythm;
        }
    }
}
