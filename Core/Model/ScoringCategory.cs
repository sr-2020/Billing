using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Model
{
    public class ScoringCategory : BaseEntity
    {
        public string Name { get; set; }
        public decimal Weight { get; set; }
    }
}
