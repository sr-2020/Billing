using Core.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Billing.Dto.Scoring
{
    public class ScoringFactorDto
    {
        public ScoringFactorDto(ScoringFactor factor)
        {
            Id = factor.Id;
            Name = factor.Name;
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Value { get; set; }
    }
}
