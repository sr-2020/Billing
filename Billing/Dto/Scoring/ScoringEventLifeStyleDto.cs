using System;
using System.Collections.Generic;
using System.Text;

namespace Billing.Dto.Scoring
{
    public class ScoringEventLifeStyleDto
    {
        public int FactorId { get; set; }
        public int EventLifeStyle { get; set; }
        public decimal Value { get; set; }
    }
}
