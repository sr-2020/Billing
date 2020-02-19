using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Model
{
    public class ScoringFactor : BaseEntity
    {
        public string Name { get; set; }
        public int Algorythm { get; set; }
        public ScoringCategory Category { get; set; }

    }
}
