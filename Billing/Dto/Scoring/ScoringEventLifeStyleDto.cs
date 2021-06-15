using Core.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Billing.Dto.Scoring
{
    public class ScoringEventLifeStyleDto
    {
        public ScoringEventLifeStyleDto(ScoringEventLifestyle eventls)
        {
            FactorId = eventls.ScoringFactorId;
            EventLifeStyle = eventls.EventNumber;
            Value = eventls.Value;
        }
        public int FactorId { get; set; }
        public int EventLifeStyle { get; set; }
        public decimal Value { get; set; }
    }
}
